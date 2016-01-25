package com.ammolite.rollout;

import android.content.Intent;
import android.support.v7.app.ActionBarActivity;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.widget.ListView;

import java.util.ArrayList;

public class ServerListActivity extends ActionBarActivity {
    private static final String SPECTATOR_NAME = "Spectator";

    private ListView                    listView;
    private ServerHandleListAdapter     adapter;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_server_list);

        listView = (ListView)findViewById(R.id.server_list);
        adapter = new ServerHandleListAdapter(this, new ArrayList<ServerHandle>());
        listView.setAdapter(adapter);

        Server.discoverServersAsync(this);
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
}