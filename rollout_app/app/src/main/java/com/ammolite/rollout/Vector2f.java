package com.ammolite.rollout;

public class Vector2f {
    private float x, y;

    public Vector2f() {
        this(0.0f);
    }

    public Vector2f(float value) {
        x = y = value;
    }

    public Vector2f(float x, float y) {
        this.x = x;
        this.y = y;
    }

    public float x() {
        return x;
    }

    public float y() {
        return y;
    }

    public void x(float f) {
        x = f;
    }

    public void y(float f) {
        y = f;
    }

    public float lengthSquared() {
        return x * x + y * y;
    }

    public float length() {
        return (float)Math.sqrt(lengthSquared());
    }

    public float dot(Vector2f v) {
        return x * v.x + y * v.y;
    }

    public float angle(Vector2f v) {
        return (float)Math.acos(dot(v) / (length() * v.length()));
    }
}
