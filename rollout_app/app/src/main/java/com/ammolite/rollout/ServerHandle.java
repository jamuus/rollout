package com.ammolite.rollout;

import java.net.InetAddress;

public class ServerHandle {
    private InetAddress target;
    private String      name;

    public ServerHandle() { }

    public ServerHandle(InetAddress target, String name) {
        this.target = target;
        this.name = name;
    }

    public InetAddress getTarget() {
        return target;
    }

    public String getName() {
        return name;
    }

    public void setTarget(InetAddress target) {
        this.target = target;
    }

    public void setName(String name) {
        this.name = name;
    }

    @Override
    public String toString() {
        return name + " - " + target.getHostAddress();
    }

    @Override
    public boolean equals(Object handle) {
        boolean same = false;

        if (handle != null && handle instanceof ServerHandle) {
            same = (name.equals(((ServerHandle)handle).name) && target.equals(((ServerHandle)handle).target));
        }

        return same;
    }
}
