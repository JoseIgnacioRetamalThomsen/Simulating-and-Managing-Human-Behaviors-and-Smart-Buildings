package com.ucd.hyperbuilding.lightservice.data;

import java.util.List;

public class Property {
    public String Description;
    public String Type;
    public List<Form> Forms;
    public String Observable;
    public boolean ReadOnly;

    @Override
    public String toString() {
        return "Property{" +
                "Description='" + Description + '\'' +
                ", Type='" + Type + '\'' +
                ", Forms=" + Forms +
                ", Observable='" + Observable + '\'' +
                ", ReadOnly=" + ReadOnly +
                '}';
    }
}
