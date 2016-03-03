package com.ammolite.rollout;

import android.os.Handler;
import android.os.Message;
import android.os.SystemClock;

public abstract class FixedCountDownTimer {
    private final long millisInFuture;
    private final long countdownInterval;
    private long stopTimeInFuture;
    private boolean cancelled = false;

    public FixedCountDownTimer(long millisInFuture, long countdownInterval){
        this.millisInFuture = millisInFuture;
        this.countdownInterval = countdownInterval;
    }

    public synchronized final void cancel() {
        cancelled = true;
        handler.removeMessages(MSG);
    }

    public synchronized final FixedCountDownTimer start() {
        cancelled = false;
        if (millisInFuture < 0) {
            onFinish();
            return this;
        }

        stopTimeInFuture = SystemClock.elapsedRealtime() + millisInFuture;
        handler.sendMessage(handler.obtainMessage(MSG));
        return this;
    }

    public abstract void onTick(long millisUntilFinished);
    public abstract void onFinish();

    private static final int MSG = 1;

    private Handler handler = new Handler() {
        @Override
        public void handleMessage(Message message) {
            synchronized (FixedCountDownTimer.this) {
                if (cancelled)
                    return;

                final long millisLeft = stopTimeInFuture - SystemClock.elapsedRealtime();

                if (millisLeft <= 0) {
                    onFinish();
                } else {
                    long lastTickStart = SystemClock.elapsedRealtime();
                    onTick(millisLeft);
                    long delay = lastTickStart + countdownInterval - SystemClock.elapsedRealtime();
                    while (delay < 0)
                        delay += countdownInterval;

                    sendMessageDelayed(obtainMessage(MSG), delay);
                }
            }
        }
    };
}
