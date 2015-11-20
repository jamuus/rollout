package com.ammolite.rollout;

import android.content.Context;
import android.graphics.Canvas;
import android.graphics.Paint;
import android.view.View;

/**
 * Created by Dominic on 19/11/2015.
 */
public class ThumbstickControl extends View
{
    public ThumbstickControl(Context context)
    {
        super(context);
        setId(R.id.thumbstick);
    }

    @Override
    protected void onDraw(Canvas canvas)
    {
        super.onDraw(canvas);

        Paint paint = new Paint();
        paint.setColor(getResources().getColor(R.color.material_blue_grey_800));
        canvas.drawCircle(0,0, 500, paint);
    }
}
