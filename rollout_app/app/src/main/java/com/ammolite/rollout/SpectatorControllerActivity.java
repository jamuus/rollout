package com.ammolite.rollout;

import android.support.v7.app.ActionBarActivity;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.FrameLayout;
import android.widget.TextView;
import android.widget.Toast;


public class SpectatorControllerActivity extends ActionBarActivity {
    private Button[]        eventButtons;
    private TextView        txtTimeRemaining;
    private FrameLayout[]   eventButtonParents;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_spectator_controller);

        Server.setSpectatorControllerActivity(this);

        eventButtons = new Button[2];
        eventButtons[0] = (Button)findViewById(R.id.btn_event_1);
        eventButtons[1] = (Button)findViewById(R.id.btn_event_2);

        eventButtonParents = new FrameLayout[2];
        eventButtonParents[0] = (FrameLayout)findViewById(R.id.event_1);
        eventButtonParents[1] = (FrameLayout)findViewById(R.id.event_2);

        txtTimeRemaining = (TextView)findViewById(R.id.countdown);
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.menu_spectator_controller, menu);
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

    public void setEvents(int event0, int event1) {
        GameEvent[] events = new GameEvent[2];
        events[0] = GameEventHelper.get(event0);
        events[1] = GameEventHelper.get(event1);

        for (int i = 0; i < eventButtons.length; ++i) {
            final GameEvent e = events[i];

            eventButtons[i].setTag(e.getId());
            eventButtons[i].setText(e.getName());
            eventButtons[i].setOnLongClickListener(new View.OnLongClickListener() {
                @Override
                public boolean onLongClick(View v) {
                    Toast.makeText(SpectatorControllerActivity.this, e.getDescription(), Toast.LENGTH_LONG).show();
                    return true;
                }
            });
            eventButtonParents[i].setBackgroundColor(e.getColour());
        }
    }

    public void startCountdown(int ms) {
        new FixedCountDownTimer(ms, 1000) {
            public void onTick(long millisUntilFinished) {
                txtTimeRemaining.setText("" + Math.round(millisUntilFinished / 1000.0f));
            }

            public void onFinish() {
                txtTimeRemaining.setText("0");
                for (int i = 0; i < eventButtons.length; ++i) {
                    eventButtons[i].setOnLongClickListener(null);
                    eventButtons[i].setText("Waiting for next vote.");
                    eventButtonParents[i].setBackgroundColor(getResources().getColor(R.color.transparentgrey));
                    eventButtons[i].setTag(-1);
                }
            }
        }.start();
    }

    public void eventClicked(View v)
    {
        int id = (int)v.getTag();

        //Get the tag of the element which was selected
        if (id > -1) {
            Server.voteEvent(id);
        }
    }
}