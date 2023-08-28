package com.ucd.hyperbuilding.lightservice.data;

import java.util.List;

public class Thing {
    public List<Object> Context;
    public String id;
    public String title;
    public String description;
    public List<String> Security;
    public List<Property> properties;
    public List<Action> actions;
    public List<Event> Events;

    @Override
    public String toString() {
        return "Thing{" +
                "Context=" + Context +
                ", id='" + id + '\'' +
                ", title='" + title + '\'' +
                ", description='" + description + '\'' +
                ", Security=" + Security +
                ", properties=" + properties +
                ", actions=" + actions +
                ", Events=" + Events +
                '}';
    }
}

