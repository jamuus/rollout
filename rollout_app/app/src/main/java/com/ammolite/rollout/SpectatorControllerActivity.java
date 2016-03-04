package com.ammolite.rollout;

import android.support.v7.app.ActionBarActivity;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;


public class SpectatorControllerActivity extends ActionBarActivity {
    private Button[]    eventButtons;
    private TextView    txtTimeRemaining;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_spectator_controller);

        Server.setSpectatorControllerActivity(this);

        eventButtons = new Button[2];
        eventButtons[0] = (Button)findViewById(R.id.btn_event_1);
        eventButtons[1] = (Button)findViewById(R.id.btn_event_2);

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
        eventButtons[0].setText("" + event0);
        eventButtons[1].setText("" + event1);
    }

    public void startCountdown(int ms) {
        new FixedCountDownTimer(ms, 1000) {
            public void onTick(long millisUntilFinished) {
                txtTimeRemaining.setText("" + Math.round(millisUntilFinished / 1000.0f));
            }

            public void onFinish() {
                txtTimeRemaining.setText("0");
                for (int i = 0; i < eventButtons.length; ++i) {
                    eventButtons[i].setOnClickListener(null);
                    eventButtons[i].setBackgroundColor(getResources().getColor(R.color.transparentgrey));
                    eventButtons[i].setText("Waiting for next vote.");
                }
            }
        }.start();
    }

    public void eventClicked(View v)
    {
        //Get the tag of the element which was selected
        v.getTag();
    }
}