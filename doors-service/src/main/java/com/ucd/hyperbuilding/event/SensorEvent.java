package com.ucd.hyperbuilding.event;

import astra.event.Event;
import astra.reasoner.util.LogicVisitor;
import astra.term.Term;

public class SensorEvent implements Event {
    public Term agentId;
    public Term sensorIdString;
    public Term typeString;
    public Term timeStampSecondsLong;

    public SensorEvent(Term agentId, Term sensorIdString, Term typeString, Term timeStampSecondsLong) {
        this.sensorIdString = sensorIdString;
        this.typeString = typeString;
        this.timeStampSecondsLong = timeStampSecondsLong;
        this.agentId =agentId;
    }


    @Override
    public Object getSource() {
        return null;
    }

    @Override
    public String signature() {
        return "$com.ucd.doors.event.SensorEvent";
    }

    @Override
    public Event accept(LogicVisitor visitor) {
        return null;
    }
}
