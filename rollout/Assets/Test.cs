using UnityEngine;
using System.Collections;

using System.Net;
using System.Net.Sockets;

public class Test : MonoBehaviour {

	// Use this for initialization
    void Awake () {
        var message = new Server.Message(Server.MessageType.RollSphero);
        message.AddContent(180.0f);
        message.AddContent(0.25f);
        message.AddContent("SPHERO-BOO");

        Server.OpenConnection("127.0.0.1", 7777);

        Server.SendEndianness();
        Server.Send(message);
	}

    void OnApplicationQuit()
    {
        Server.CloseConnection();
    }
}
