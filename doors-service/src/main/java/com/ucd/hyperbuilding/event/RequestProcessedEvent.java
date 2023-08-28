package com.ucd.hyperbuilding.event;

import astra.event.Event;
import astra.reasoner.util.LogicVisitor;
import astra.term.Term;

public class RequestProcessedEvent implements Event {
    public Term value;

    public RequestProcessedEvent(Term value) {
        this.value = value;
    }

    @Override
    public Object getSource() {
        return null;
    }

    @Override
    public String signature() {
        return "$com.ucd.doors.event.RequestProcessedEvent";
    }

    @Override
    public Event accept(LogicVisitor visitor) {
        return null;
    }

}
