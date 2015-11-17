package com.ammolite.rollout;

import android.app.ProgressDialog;
import android.support.v7.app.ActionBarActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;

import java.util.List;
import java.util.concurrent.Future;


public class MainActivity extends ActionBarActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        /*
        ServerMessage message = new ServerMessage();
        message.setType(ServerMessage.Type.ROLL_SPHERO);
        message.addContent(180.0f);
        message.addContent(34.5f);
        message.addContent("SPHERO-BOO");

        Server.openConnection("192.168.0.5", 7777);
        Server.send(message);*/

        new ServerConnectionTask(this).execute();
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.menu_main, menu);
        return true;
    }

    @Override
    protected void onStart()  {
        super.onStart();



       /* try {
            Future<List<Pair<String, Integer>>> serverFuture = Server.discoverServers();
            //List<Pair<String, Integer>> servers = serverFuture.get();
            //for (Pair<String, Integer> p : servers) {
            //    Log.d("MAIN", "SERVER: " +  p.getFirst() + ":" + p.getSecond());
            //}
        } catch (Exception ex) {
            Log.d("MAIN", "Failed to do future.", ex);
        }*/

        //pd.dismiss();
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();

        //noinspection SimplifiableIfStatement
        if (id == R.id.action_settings) {
            return true;
        }

        return super.onOptionsItemSelected(item);
    }
}
