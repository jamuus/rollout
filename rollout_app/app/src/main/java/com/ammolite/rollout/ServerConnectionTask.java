package com.ammolite.rollout;

import android.app.ProgressDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.os.AsyncTask;
import android.util.Log;
import android.widget.Toast;

import java.util.List;

public class ServerConnectionTask extends AsyncTask<Void, Void, List<Pair<String, Integer>>> {
    private ProgressDialog dialog;
    private Context context;

    public ServerConnectionTask(Context ctx) {
        super();
        dialog = new ProgressDialog(ctx);
        dialog.setTitle("Searching for server...");
        dialog.setMessage("Attempting to connect to server, please wait.");
        dialog.setCancelable(false);
        dialog.setOnCancelListener(new DialogInterface.OnCancelListener() {
            @Override
            public void onCancel(DialogInterface dialog) {
                Log.d("SERVER", "Cancel called.");
                ServerConnectionTask.this.cancel(true);
                cancel(true);
            }
        });
        context = ctx;
    }

    @Override
    protected void onPreExecute() {
        dialog.show();
    }

    @Override
    protected List<Pair<String, Integer>> doInBackground(Void... params) {
        Log.d("SERVER", "Running server connection task.");
        return Server.discoverServersSync();
    }

    @Override
    protected void onPostExecute(List<Pair<String, Integer>> result) {
        if (result != null) {
            final Pair<String, Integer> server = result.get(0);
            final String message = "Connected to server on " + server.getFirst() + ":" + server.getSecond() + ".";

            Server.openConnection(server.getFirst(), server.getSecond());

            dialog.dismiss();
            Toast.makeText(context, message, Toast.LENGTH_LONG).show();
            context.startActivity(new Intent(context, ControllerActivity.class));
        } else {
            dialog.dismiss();
            Toast.makeText(context, "Failed to connect to server.", Toast.LENGTH_LONG).show();
        }
    }
}
