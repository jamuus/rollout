package com.ammolite.rollout;

import android.content.Intent;
import android.support.v7.app.ActionBarActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.ListView;
import android.widget.TextView;

import java.util.ArrayList;

public class ServerListActivity extends ActionBarActivity {
    private static final String SPECTATOR_NAME  = "Spectator";
    private static final String TAG             = "ServerListActivity";

    private TextView                    txtSearching;
    private ListView                    listView;
    private ServerHandleListAdapter     adapter;
    private Thread                      serverDiscoverThread;
    private Runnable                    serverDiscoverAction;
    private boolean                     discoverServers;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_server_list);

        listView = (ListView)findViewById(R.id.server_list);
        adapter = new ServerHandleListAdapter(this, new ArrayList<ServerHandle>());
        listView.setAdapter(adapter);

        txtSearching = (TextView)findViewById(R.id.txt_searching_for_servers);

        serverDiscoverAction = new Runnable() {
            @Override
            public void run() {
                while (discoverServers) {
                    Server.discoverServers(ServerListActivity.this);
                    try {
                        Thread.sleep(3000);
                    } catch (InterruptedException ex) {
                        Log.d(TAG, "Exception sleeping discover thread.", ex);
                    }
                }
            }
        };

        resume();
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.menu_server_list, menu);
        return true;
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

    public void addServerHandle(final ServerHandle server) {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                txtSearching.setVisibility(View.GONE);
                listView.setVisibility(View.VISIBLE);
                adapter.add(server);
            }
        });
    }

    public void joinServerAs(final String name) {
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if (name.equals(SPECTATOR_NAME)) {
                    startActivity(new Intent(ServerListActivity.this, SpectatorControllerActivity.class));
                } else {
                    Sphero.setName(name);
                    startActivity(new Intent(ServerListActivity.this, SpheroControllerActivity.class));
                }
            }
        });
    }

    private void stopOrPause() {
        discoverServers = false;
        new Thread(new Runnable() {
            @Override
            public void run() {
                try {
                    serverDiscoverThread.join();
                } catch (InterruptedException ex) {
                    Log.d(TAG, "Exception stopping discovery thread.", ex);
                }
            }
        }).start();

        Server.stopListening();
    }

    private void resume() {
        discoverServers = true;
        Server.startListening();
        serverDiscoverThread = new Thread(serverDiscoverAction);
        serverDiscoverThread.start();
    }

    @Override
    public void onBackPressed() {
        super.onBackPressed();
        stopOrPause();
    }

    @Override
    public void onResume() {
        super.onResume();
        resume();
    }

    @Override
    public void onPause() {
        super.onPause();
        stopOrPause();
    }
}