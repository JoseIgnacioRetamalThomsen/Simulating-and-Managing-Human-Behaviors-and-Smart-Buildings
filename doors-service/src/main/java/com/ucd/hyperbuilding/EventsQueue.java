package com.ucd.hyperbuilding;

import astra.core.Agent;
import astra.core.Module;
import astra.formula.Predicate;
import astra.term.Primitive;
import astra.term.Term;

import java.util.Optional;
import java.util.PriorityQueue;

public class EventsQueue extends Module implements AgentEnhancer {

    PriorityQueue<SensorEvent> eventQueue;

    @ACTION
    public boolean init() {
        eventQueue = new PriorityQueue<>();
        return true;
    }

    @ACTION
    public boolean AddEvent(String eventClass, String sensorId, String type, String timeStamp) {
        eventQueue.add(new SensorEvent(
                removeQuotes(eventClass),
                removeQuotes(sensorId),
                removeQuotes(type),
                removeQuotes(timeStamp)
        ));
        Optional<Predicate> eventInAgentBeliefs = agent.beliefs().beliefs().stream()
                .filter(formula -> formula instanceof Predicate)
                .map(formula -> (Predicate) formula)
                .filter(predicate -> predicate.predicate().equals("event_class_id_type_timeStamp"))
                .findFirst();
        if (eventInAgentBeliefs.isPresent()) {
            Predicate actualEventInAgentBeliefs = eventInAgentBeliefs.get();
            agent.beliefs().dropBelief(actualEventInAgentBeliefs);
            eventQueue.add(new SensorEvent(
                    removeQuotes(actualEventInAgentBeliefs.termAt(0).toString()),
                    removeQuotes(actualEventInAgentBeliefs.termAt(1).toString()),
                    removeQuotes(actualEventInAgentBeliefs.termAt(2).toString()),
                    removeQuotes(actualEventInAgentBeliefs.termAt(3).toString())
            ));

        }
        SensorEvent event = eventQueue.poll();
        setBelief("event_class_id_type_timeStamp", event.eventClass, event.sensorId, event.type, event.timeStamp);
        updateIsUpdating();

        return true;
    }

    private String removeQuotes(String input) {
        return input.replace("\"", "");
    }

    private void updateIsUpdating() {
        Optional<Predicate> isUpdating = agent.beliefs().beliefs().stream()
                .filter(formula -> formula instanceof Predicate)
                .map(formula -> (Predicate) formula)
                .filter(predicate -> predicate.predicate().equals("isUpdatingQueue"))
                .findFirst();
        agent.beliefs().dropBelief(isUpdating.get());
        agent.beliefs().addBelief(new Predicate("isUpdatingQueue",
                new Term[]{Primitive.newPrimitive(false)}));
    }

    @Override
    public Agent getAgent() {
        return agent;
    }

    static class SensorEvent implements Comparable<SensorEvent> {

        public String eventClass;
        public String sensorId;
        public String type;
        public String timeStamp;

        public float dateFloat;

        public SensorEvent(String eventClass, String sensorId, String type, String timeStamp) {
            this.eventClass = eventClass;
            this.sensorId = sensorId;
            this.type = type;
            this.timeStamp = timeStamp;
            this.dateFloat = Float.parseFloat(timeStamp.replace("\"", ""));
        }

        @Override
        public int compareTo(SensorEvent other) {
            return Float.compare(other.dateFloat, this.dateFloat);
        }

        @Override
        public String toString() {
            return "SensorEvent{" +
                    "sensorId='" + sensorId + '\'' +
                    ", type='" + type + '\'' +
                    ", timeStamp=" + timeStamp +
                    '}';
        }
    }
}


