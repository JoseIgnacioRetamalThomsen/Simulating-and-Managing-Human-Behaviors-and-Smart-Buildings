package com.ucd.hyperbuilding.lightservice.data;

import java.util.List;

public class Action {
    public String Description;
    public List<Form> Forms;
    public boolean Safe;

    @Override
    public String toString() {
        return "Action{" +
                "Description='" + Description + '\'' +
                ", Forms=" + Forms +
                ", Safe=" + Safe +
                '}';
    }
}
