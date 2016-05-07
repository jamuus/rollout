using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

using UnityEngine;

public class HttpServerModule
{
    private interface IHttpServerFile
    {
        byte[] Data { get; set; }
    }

    private class HttpServerTextFile : IHttpServerFile
    {
        public byte[] Data { get; set; }

        public HttpServerTextFile(string path)
        {
            Data = Encoding.UTF8.GetBytes(File.ReadAllText(path));
        }
    }

    private class HttpServerBinaryFile : IHttpServerFile
    {
        public byte[] Data { get; set; }

        public HttpServerBinaryFile(string path)
        {
            Data = File.ReadAllBytes(path);
        }
    }

    private HttpListener listener;
    private Thread listeningThread;

    private static readonly Dictionary<string, IHttpServerFile> Files;

    private static string VoteUrl = "/webapp/vote";

    private static string EventButtonLeft = "event-btn-left";
    private static string EventButtonRight = "event-btn-right";

    static HttpServerModule()
    {
        Files = new Dictionary<string, IHttpServerFile>();
        string[] textFileExtensions = new string[] { ".html", ".css", ".js" };
        const string root = "../webapp/";
        string[] files = Directory.GetFiles(root, "*", SearchOption.AllDirectories);
        foreach (string s in files)
        {
            string path = s.Substring(root.Length).Replace('\\', '/');

            if (Array.IndexOf(textFileExtensions, Path.GetExtension(path)) > -1)
                Files["/webapp/" + path] = new HttpServerTextFile(root + path);
            else
                Files["/webapp/" + path] = new HttpServerBinaryFile(root + path);

            Log("Loaded file \"{0}\".", path);
        }
    }

    public HttpServerModule(string prefix)
    {
        if (!HttpListener.IsSupported)
            throw new NotSupportedException("HttpListener is not supported.");

        listener = new HttpListener();
        listener.Prefixes.Add(prefix);
    }

    public void Start()
    {
        listener.Start();

        listeningThread = new Thread(() =>
        {
            Thread.CurrentThread.IsBackground = true;
            while (listener.IsListening)
            {
                HttpListenerContext ctx = listener.GetContext();
                try
                {
                    byte[] data = HandleRequest(ctx.Request);
                    ctx.Response.ContentLength64 = data.Length;
                    ctx.Response.OutputStream.Write(data, 0, data.Length);
                }
                catch
                {
                    Debug.LogFormat("[HttpServerModule] Exception.");
                }
                finally
                {
                    ctx.Response.OutputStream.Close();
                }
            }
        });
        listeningThread.Start();

        Log("Started listening for GET/POST.");
    }

    public void Stop()
    {
        listener.Stop();
        listener.Close();
        listeningThread.Join();

        Log("Stopped listening successfully.");
    }

    private byte[] HandleRequest(HttpListenerRequest request)
    {
        if (request.HttpMethod == "GET")
        {
            Log("Received GET request for \"{0}\".", request.RawUrl);

            IHttpServerFile file;
            if (Files.TryGetValue(request.RawUrl, out file))
                return file.Data;
            else if (request.RawUrl == "/webapp/events")
                return Encoding.UTF8.GetBytes(BuildEventStatusString());
        }
        else if (request.HttpMethod == "POST")
        {
            Log("Received POST request for \"{0}\".", request.RawUrl);

            using (StreamReader input = new StreamReader(request.InputStream))
            {
                string str = input.ReadToEnd();

                if (str == EventButtonLeft)
                    SpectatorManager.EventVote(SpectatorManager.ActiveEventIDs[0]);
                else if (str == EventButtonRight)
                    SpectatorManager.EventVote(SpectatorManager.ActiveEventIDs[1]);
                else
                    Log("Tried to vote for an invalid event.");
            }
        }

        return Encoding.UTF8.GetBytes(string.Empty);
    }

    private string BuildEventStatusString()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("{\"time\":").Append(SpectatorManager.EventsLastUpdated)
            .Append(",\"event0\":").Append("\"EVENT NAME 0\"")
            .Append(",\"event1\":").Append("\"EVENT NAME 1\"")
            .Append("}");
        return builder.ToString();
    }

    private static void Log(string format, params object[] args)
    {
        Debug.LogFormat("[HttpServerModule@{0}|{1}] " + string.Format(format, args), Thread.CurrentThread.ManagedThreadId, (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
    }
}
