package com.ucd.hyperbuilding.event;

import astra.event.Event;
import astra.reasoner.util.LogicVisitor;
import astra.term.Term;

public class CardReaderEvent implements Event {
    public Term agentId;
    public Term cardId;
    public Term turnstileId;
    public Term timeStamp;

    public CardReaderEvent(Term value, Term value2, Term value3, Term value4) {
        this.agentId = value;
        this.cardId =value2;
        this.turnstileId = value3;
        this.timeStamp = value4;
    }

    @Override
    public Object getSource() {
        return null;
    }

    @Override
    public String signature() {
        return "$com.ucd.doors.event.CardReaderEvent";
    }

    @Override
    public Event accept(LogicVisitor visitor) {
        return null;
    }
}
