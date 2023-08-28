package com.ucd.hyperbuilding.lightservice.data;

import java.util.List;
import java.util.Map;

public class Event {
    public String Description;
    public Map<String, String> Data;
    public List<Form> Forms;

    @Override
    public String toString() {
        return "Event{" +
                "Description='" + Description + '\'' +
                ", Data=" + Data +
                ", Forms=" + Forms +
                '}';
    }
}
