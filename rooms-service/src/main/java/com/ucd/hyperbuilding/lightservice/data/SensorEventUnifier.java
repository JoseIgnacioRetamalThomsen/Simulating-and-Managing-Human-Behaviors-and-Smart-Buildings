package com.ucd.hyperbuilding.lightservice.data;

import astra.core.Agent;
import astra.reasoner.EventUnifier;
import astra.reasoner.Unifier;
import astra.term.Term;

import java.util.HashMap;
import java.util.Map;

public class SensorEventUnifier implements EventUnifier<SensorEvent> {
    @Override
    public Map<Integer, Term> unify(SensorEvent source, SensorEvent target, Agent agent) {
        return Unifier.unify(
          new Term[]{source.agentId, source.sensorId, source.eventType, source.timeStamp},
                new Term[]{target.agentId, target.sensorId, target.eventType, target.timeStamp},
                new HashMap<>(),
                agent
        );
    }
}
