package com.ammolite.rollout;

import android.app.ProgressDialog;
import android.content.Context;
import android.os.AsyncTask;
import android.widget.Toast;

import java.util.List;

public class ServerConnectionTask extends AsyncTask<Void, Void, List<Pair<String, Integer>>> {
    private ProgressDialog dialog;
    private Context context;

    public ServerConnectionTask(Context ctx) {
        super();
        dialog = new ProgressDialog(ctx);
        context = ctx;
    }

    @Override
    protected void onPreExecute() {
        dialog.setTitle("Searching for server...");
        dialog.setMessage("Attempting to connect to server, please wait.");
        dialog.setCancelable(false);
        dialog.show();
    }

    @Override
    protected List<Pair<String, Integer>> doInBackground(Void... params) {
        return Server.discoverServersSync();
    }

    @Override
    protected void onPostExecute(List<Pair<String, Integer>> result) {
        final Pair<String, Integer> server = result.get(0);
        final String message = "Connected to server on " + server.getFirst() + ":" + server.getSecond() + ".";

        dialog.dismiss();
        Toast.makeText(context, message, Toast.LENGTH_LONG).show();
    }
}
