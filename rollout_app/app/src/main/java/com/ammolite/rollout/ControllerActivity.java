package com.ammolite.rollout;

import android.app.ActionBar;
import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.os.PersistableBundle;
import android.os.Vibrator;
import android.support.v7.app.ActionBarActivity;
import android.os.Bundle;
import android.telephony.TelephonyManager;
import android.util.LayoutDirection;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.widget.FrameLayout;
import android.widget.RelativeLayout;
import android.widget.TextView;


public class ControllerActivity extends ActionBarActivity implements SensorEventListener {
    private SensorManager sensorManager;
    private Sensor gyroscope;
    private Vibrator vibrator;
    private TextView txtGyroscope;

    private TextView txtSpheroName;
    private TextView txtSpheroHealth;
    private TextView txtSpheroVoltage;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_controller);

        txtGyroscope = (TextView)findViewById(R.id.txt_gyro);

        sensorManager = (SensorManager)getSystemService(Context.SENSOR_SERVICE);
        gyroscope = sensorManager.getDefaultSensor(Sensor.TYPE_GYROSCOPE);

        vibrator = (Vibrator)getSystemService(Context.VIBRATOR_SERVICE);

        findViewById(R.id.btn_vibrate).setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                vibrator.vibrate(1000);
            }
        });

        txtSpheroName = (TextView)findViewById(R.id.txt_name);
        txtSpheroHealth = (TextView)findViewById(R.id.txt_health);
        txtSpheroVoltage = (TextView)findViewById(R.id.txt_voltage);

        txtSpheroName.setText(Sphero.getDeviceName());
        txtSpheroHealth.setText("" + Sphero.getHealth());
        txtSpheroVoltage.setText("" + Sphero.getBatteryVoltage());

        //Create the thumbstick control and add it to the page
        ThumbstickControl thumbstick = new ThumbstickControl(this);

        //Set the thumbsticks parameters
        RelativeLayout.LayoutParams params = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WRAP_CONTENT, ViewGroup.LayoutParams.MATCH_PARENT);
        params.setMargins(60, 150, 60, 60);
        params.addRule(RelativeLayout.ALIGN_PARENT_RIGHT);
        thumbstick.setLayoutParams(params);

        //Align the label below the thumbstick
        RelativeLayout.LayoutParams thumbstickLabelParams = (RelativeLayout.LayoutParams)((TextView) findViewById(R.id.thumbstick_label)).getLayoutParams();

                ((RelativeLayout) findViewById(R.id.root)).addView(thumbstick);
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.menu_controller, menu);
        return true;
    }

    @Override
    protected void onResume() {
        super.onResume();
        sensorManager.registerListener(this, gyroscope, SensorManager.SENSOR_DELAY_FASTEST);
    }

    @Override
    protected void onStop() {
        super.onStop();
        sensorManager.unregisterListener(this);
    }

    @Override
    public void onAccuracyChanged(Sensor arg0, int arg1) {

    }

    @Override
    public void onSensorChanged(SensorEvent event) {
        txtGyroscope.setText("x: " + event.values[2] + ", y: " + event.values[1] +
        ", z: " + event.values[0] + ".");
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

    public void usePowerup(View v)
    {
        //Alternate between used and not used
        if (v.getTag().equals(1))
        {
            v.setTag(0);
            ((FrameLayout)v.getParent()).setBackgroundColor(getResources().getColor(R.color.tranparentgrey));
            ((TextView)v).setTextSize(36);
            ((TextView)v).setText("?");
        }
        else
        {
            v.setTag(1);
            ((FrameLayout)v.getParent()).setBackgroundColor(getResources().getColor(R.color.blue));
            ((TextView)v).setTextSize(14);
            ((TextView)v).setText("Missles");
        }
    }
}
