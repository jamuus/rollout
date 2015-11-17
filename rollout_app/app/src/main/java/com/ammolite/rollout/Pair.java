package com.ammolite.rollout;

public class Pair<F, S> {
    private F first_;
    private S second_;

    public Pair(F first, S second) {
        this.first_ = first;
        this.second_ = second;
    }

    public F getFirst() {
        return first_;
    }

    public S getSecond() {
        return second_;
    }
}
