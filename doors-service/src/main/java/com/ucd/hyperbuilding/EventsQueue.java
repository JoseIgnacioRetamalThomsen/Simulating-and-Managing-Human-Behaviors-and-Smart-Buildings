package com.ucd.hyperbuilding;

import astra.core.Module;
import astra.formula.Predicate;
import astra.term.Primitive;
import astra.term.Term;

import java.util.Optional;
import java.util.PriorityQueue;

public class EventsQueue extends Module {

    PriorityQueue<SensorEvent> eventQueue;

    @ACTION
    public boolean init() {
        eventQueue = new PriorityQueue<>();
        return true;
    }


    @ACTION
    public boolean AddEvent(String eventClass, String sensorId, String type, String timeStamp) {
        SensorEvent newEvent = new SensorEvent(eventClass.replace("\"",""),sensorId.replace("\"",""),
                type.replace("\"",""), timeStamp.replace("\"",""));
        Optional<Predicate> eventInAgentBeliefs = agent.beliefs().beliefs().stream()
                .filter(formula -> formula instanceof Predicate)
                .map(formula -> (Predicate) formula)
                .filter(predicate -> predicate.predicate().equals("event_class_id_type_timeStamp"))
                .findFirst();
        eventQueue.add(newEvent);
        if (eventInAgentBeliefs.isPresent()) {
            Predicate p = eventInAgentBeliefs.get();
            System.out.println("PREDICATE= " + p.toString());
            agent.beliefs().dropBelief(p);

            eventQueue.add(new SensorEvent(p.termAt(0).toString().replace("\"","")
                    , p.termAt(1).toString().replace("\"","")
                    , p.termAt(2).toString().replace("\"","")
                    , p.termAt(3).toString().replace("\"","")));

        }
        SensorEvent event = eventQueue.poll();
        Predicate predicateToAdd = new Predicate("event_class_id_type_timeStamp", new Term[]{
                Primitive.newPrimitive(event.eventClass),
                Primitive.newPrimitive(event.sensorId),
                Primitive.newPrimitive(event.type),
                Primitive.newPrimitive(String.valueOf(event.timeStamp))
        });
        agent.beliefs().addBelief(predicateToAdd);
        updateIsUpdating();
        System.out.println("QUEUE SIZE= " + eventQueue.size());
        return true;
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
            this.dateFloat = Float.parseFloat(timeStamp.replace("\"",""));
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


