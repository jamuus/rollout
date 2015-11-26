package com.ammolite.rollout;

import android.content.Context;
import android.graphics.Canvas;
import android.graphics.Paint;
import android.graphics.Point;
import android.opengl.GLSurfaceView;
import android.view.MotionEvent;
import android.view.TextureView;
import android.view.View;
import android.view.ViewGroup;
import android.view.animation.AnimationUtils;
import android.widget.RelativeLayout;

/**
 * Created by Dominic on 19/11/2015.
 */
public class ThumbstickControl extends View
{
    private class Nub
    {
        float angle;
        float magnitude;

        public Nub()
        {
            //Initialise the member variables
            angle = 0;
            magnitude = 0;
        }

        void setPosition(float x, float y)
        {
            //Get relative positions
            float xRel = x - centre.x;
            float yRel = y - centre.y;

            //Set angle and magnitude
            angle = (float)Math.atan(yRel / xRel);
            magnitude = (float)Math.sqrt(Math.pow(xRel,2) + Math.pow(yRel,2));

            //Correct for negative values
            angle += xRel < 0? Math.PI : 0;
        }

        Point getPosition()
        {
            //Get the x and y components
            Double xComponent = magnitude * Math.cos(angle);
            Double yComponent = magnitude * Math.sin(angle);

            //Return the result as a point
            return new Point(xComponent.intValue(), yComponent.intValue());
        }
    }

    Paint backgroundPaint, nubPaint;
    Nub nub;
    Point centre;
    boolean nubHeld;

    private Runnable animator = new Runnable()
    {
        @Override
        public void run()
        {
            if (Math.abs(nub.magnitude) > 0.5 && !nubHeld)
            {
                postDelayed(this, 15);
                invalidate();
            }
        }
    };

    public ThumbstickControl(Context context)
    {
        super(context);
        setId(R.id.thumbstick);

        //Define the paints
        backgroundPaint = new Paint();
        backgroundPaint.setColor(getResources().getColor(R.color.red));
        nubPaint = new Paint();
        nubPaint.setColor(getResources().getColor(R.color.white));

        //Initialise the nub
        nub = new Nub();
    }

    @Override
    protected void onSizeChanged(int w, int h, int oldw, int oldh)
    {
        super.onSizeChanged(w, h, oldw, oldh);

        //Redefine the centre
        centre = new Point(w/2,h/2);

        //Set the size of the control
        ViewGroup.LayoutParams params = this.getLayoutParams();
        params.width = this.getHeight();
        params.height = this.getHeight();
        this.setLayoutParams(params);
    }

    @Override
    protected void onDraw(Canvas canvas)
    {
        super.onDraw(canvas);

        //Draw the background
        canvas.drawCircle(canvas.getWidth() / 2, canvas.getHeight() / 2, (float)(canvas.getWidth()*0.45), backgroundPaint);

        //Update the nub
        nub.magnitude -= nub.magnitude/12;

        //Draw the nub
        canvas.drawCircle(centre.x + nub.getPosition().x, centre.y + nub.getPosition().y,  (float)(canvas.getWidth()* (nubHeld?0.12:0.13)), nubPaint);
    }

    @Override
    public boolean onTouchEvent(MotionEvent event)
    {
        //Get the position of the touch event
        nub.setPosition(event.getX(), event.getY());

        //Determine if this is the last call for onTouch and therefore if the nub is being held
        nubHeld = event.getActionMasked() != MotionEvent.ACTION_UP;

        //Depending on if the nub is being held, either just redraw or animate returning to centre
        if (nubHeld) invalidate();
        else animator.run();

        return true;
    }
}
