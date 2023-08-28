package com.ucd.hyperbuilding.lightservice;

import astra.core.Agent;
import astra.core.Module;
import astra.formula.Predicate;
import astra.term.Primitive;
import astra.term.Term;

import java.util.Optional;
import java.util.PriorityQueue;

public class EventsQueue extends Module implements AgentEnhancer{

    PriorityQueue<SensorEvent> eventQueue;

    @ACTION
    public boolean init() {
        eventQueue = new PriorityQueue<>();
        return true;
    }

    @ACTION
    public boolean AddEvent(String eventClass, String sensorId, String type, String timeStamp) {
        log("Adding event, eventClass={}", eventClass);
        SensorEvent newEvent = new SensorEvent(
                eventClass.replace("\"", ""),
                sensorId.replace("\"", ""),
                type.replace("\"", ""),
                timeStamp.replace("\"", ""));
        eventQueue.add(newEvent);
        pollEventFromBeliefsAndAddItToQueue();
        SensorEvent event = eventQueue.poll();
        setBelief("event_class_id_type_timeStamp",event.eventClass,event.sensorId,
                event.type, String.valueOf(event.timeStamp) );
        updateIsUpdating();
        return true;
    }

    private void pollEventFromBeliefsAndAddItToQueue() {
        Optional<Predicate> eventInAgentBeliefs = agent.beliefs().beliefs().stream()
                .filter(formula -> formula instanceof Predicate)
                .map(formula -> (Predicate) formula)
                .filter(predicate -> predicate.predicate().equals("event_class_id_type_timeStamp"))
                .findFirst();

        if (eventInAgentBeliefs.isPresent()) {
            Predicate p = eventInAgentBeliefs.get();
            agent.beliefs().dropBelief(p);
            eventQueue.add(new SensorEvent(
                    p.termAt(0).toString().replace("\"", "")
                    , p.termAt(1).toString().replace("\"", "")
                    , p.termAt(2).toString().replace("\"", "")
                    , p.termAt(3).toString().replace("\"", "")));
        }
    }

    private void updateIsUpdating() {
        dropBelief("isUpdatingQueue");
        setBelief("isUpdatingQueue",false);
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


