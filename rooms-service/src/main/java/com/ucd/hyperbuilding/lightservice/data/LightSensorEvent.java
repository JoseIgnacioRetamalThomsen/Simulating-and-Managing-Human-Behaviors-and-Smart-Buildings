package com.ucd.hyperbuilding.lightservice.data;

public class LightSensorEvent {
    public String sensorId;
    public String type;
    public long timeStamp;

    @Override
    public String toString() {
        return "LightSensorEvent{" +
                "sensorId='" + sensorId + '\'' +
                ", type='" + type + '\'' +
                ", timeStamp=" + timeStamp +
                '}';
    }
}
