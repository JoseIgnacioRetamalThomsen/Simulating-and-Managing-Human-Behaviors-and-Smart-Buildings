package com.ucd.hyperbuilding.data;

import java.util.List;
import java.util.Objects;

public class GridNodeData {
    private int x;
    private int y;
    private boolean IsFree;

    private List<String> actions;

    private String section;

    public String getSection() {
        return section;
    }

    public void setSection(String section) {
        this.section = section;
    }

    public List<String> getActions() {
        return actions;
    }

    public void setActions(List<String> actions) {
        this.actions = actions;
    }

    public int getX() {
        return x;
    }

    public void setX(int x) {
        this.x = x;
    }

    public int getY() {
        return y;
    }

    public void setY(int y) {
        this.y = y;
    }

    public boolean isFree() {
        return IsFree;
    }

    public void setFree(boolean free) {
        IsFree = free;
    }

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;
        GridNodeData that = (GridNodeData) o;
        return x == that.x && y == that.y;
    }

    @Override
    public int hashCode() {
        return Objects.hash(x, y);
    }

    @Override
    public String toString() {
        return "GridNodeData{" +
                "x=" + x +
                ", y=" + y +
                ", IsFree=" + IsFree +
                ", actions=" + actions +
                ", section='" + section + '\'' +
                '}';
    }
}
