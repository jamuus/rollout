package com.ammolite.rollout;

import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;
import android.widget.Button;
import android.widget.TextView;

import java.util.List;

public class ServerHandleListAdapter extends BaseAdapter {
    private static LayoutInflater inflater;

    private Context             context;
    private List<ServerHandle>  data;

    public ServerHandleListAdapter(Context context, List<ServerHandle> data) {
        this.context = context;
        this.data = data;

        inflater = (LayoutInflater)context.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
    }

    @Override
    public int getCount() {
        return data.size();
    }

    @Override
    public Object getItem(int position) {
        return data.get(position);
    }

    @Override
    public long getItemId(int position) {
        return position;
    }

    @Override
    public View getView(final int position, View convertView, ViewGroup parent) {
        View v = convertView;
        if (v == null)
            v = inflater.inflate(R.layout.server_list_row, null);
        ServerHandle item = data.get(position);
        ((TextView)v.findViewById(R.id.server_name)).setText(item.getName());

        Button btnConnect = (Button)v.findViewById(R.id.server_connect_button);
        btnConnect.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                AlertDialog.Builder builder = new AlertDialog.Builder(context);
                builder.setMessage("Player or Spectator?");
                builder.setPositiveButton("Player", new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        Server.connectToAsync(data.get(position), true);
                    }
                });
                builder.setNegativeButton("Spectator", new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialog, int which) {
                        Server.connectToAsync(data.get(position), false);
                    }
                });
                builder.show();
            }
        });

        return v;
    }

    public void add(ServerHandle server) {
        if (!data.contains(server)) {
            data.add(server);
            notifyDataSetChanged();
        }
    }
}