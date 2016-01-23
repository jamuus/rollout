package com.ammolite.rollout;

import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.os.Vibrator;
import android.support.v7.app.ActionBarActivity;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.widget.FrameLayout;
import android.widget.RelativeLayout;
import android.widget.TextView;

import java.util.concurrent.Callable;


public class SpheroControllerActivity extends ActionBarActivity implements SensorEventListener {
    private Callable<Void>      updateFunc;
    private Vibrator            vibrator;
    private SensorManager       sensorManager;
    private Sensor              accelerometer;
    private ThumbstickControl   thumbstick;
    private float               x, y, z;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_sphero_controller);

        // Thumbstick control.
        thumbstick = new ThumbstickControl(this);
        RelativeLayout.LayoutParams params = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WRAP_CONTENT, ViewGroup.LayoutParams.MATCH_PARENT);
        params.setMargins(60, 150, 60, 60);
        params.addRule(RelativeLayout.ALIGN_PARENT_RIGHT);
        thumbstick.setLayoutParams(params);
        ((RelativeLayout)findViewById(R.id.root)).addView(thumbstick);

        vibrator = (Vibrator)this.getSystemService(Context.VIBRATOR_SERVICE);
        sensorManager = (SensorManager)this.getSystemService(Context.SENSOR_SERVICE);
        accelerometer = sensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER);
        sensorManager.registerListener(this, accelerometer, SensorManager.SENSOR_DELAY_GAME);

        updateFunc = new Callable<Void>() {
            @Override
            public Void call() throws Exception {
                if (thumbstick.getAbsoluteMagnitude() > ThumbstickControl.DEAD_ZONE_MAGNITUDE) {
                    Sphero.shoot(thumbstick.getAngle());
                }
                if (Sphero.getHasRecentDamage()) {
                    Sphero.setHasRecentDamage(false);
                    vibrator.vibrate(750);
                }
                //TODO Rolling with x, y, z.
                return null;
            }
        };

        Sphero.startUpdateThread(updateFunc);
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.menu_sphero_controller, menu);
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

    @Override
    public void onBackPressed() {
        super.onBackPressed();
        sensorManager.unregisterListener(this);
        Sphero.stopUpdateThread();
    }

    @Override
    public void onPause() {
        super.onPause();
        sensorManager.unregisterListener(this);
        Sphero.stopUpdateThread();
    }

    @Override
    public void onResume() {
        super.onResume();
        sensorManager.registerListener(this, accelerometer, SensorManager.SENSOR_DELAY_GAME);
        Sphero.startUpdateThread(updateFunc);
    }

    @Override
    public void onSensorChanged(SensorEvent event) {
        Sensor sensor = event.sensor;
        if (sensor.getType() == Sensor.TYPE_ACCELEROMETER) {
            x = event.values[0];
            y = event.values[1];
            z = event.values[2];
        }
    }

    @Override
    public void onAccuracyChanged(Sensor sensor, int accuracy) {
        // TODO
    }

    public void btnPowerUpOnClick(View v) {
        FrameLayout frameLayout = (FrameLayout)v.getParent();
        TextView textView = (TextView)v;

        // Alternate between used and not used.
        if (v.getTag().equals(1)) {
            v.setTag(0);
            frameLayout.setBackgroundColor(getResources().getColor(R.color.transparentgrey));
            textView.setTextSize(36);
            textView.setText("?");
        } else {
            v.setTag(1);
            frameLayout.setBackgroundColor(getResources().getColor(R.color.blue));
            textView.setTextSize(14);
            textView.setText("Missiles");
        }
    }
}