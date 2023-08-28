package com.ucd.hyperbuilding.lightservice.data;

import astra.event.Event;
import astra.reasoner.util.LogicVisitor;
import astra.term.Primitive;
import astra.term.Term;

public class SensorEvent implements Event {
    public Term agentId;
    public Term sensorId;
    public Term eventType;
    public Term timeStamp;

    public SensorEvent(Term agentId, Term sensorId, Term eventType, Term timeStamp) {
        this.agentId = agentId;
        this.sensorId = sensorId;
        this.eventType = eventType;
        this.timeStamp = timeStamp;
    }
    public SensorEvent(String agentId, String sensorId, String eventType, String timeStamp) {
        this.agentId = Primitive.newPrimitive(agentId);
        this.sensorId = Primitive.newPrimitive(sensorId);
        this.eventType = Primitive.newPrimitive(eventType);
        this.timeStamp =Primitive.newPrimitive(timeStamp);
    }

    @Override
    public Object getSource() {
        return null;
    }

    @Override
    public String signature() {
        return "$com.ucd.hyperbuilding.lightservice.date.SensorEvent";
    }

    @Override
    public Event accept(LogicVisitor visitor) {
        return null;
    }
}
