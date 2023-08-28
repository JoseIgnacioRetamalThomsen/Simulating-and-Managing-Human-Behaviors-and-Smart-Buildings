package com.ucd.hyperbuilding.event;

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
                new Term[]{source.agentId, source.sensorIdString, source.typeString, source.timeStampSecondsLong},
                new Term[]{target.agentId, target.sensorIdString, target.typeString, target.timeStampSecondsLong},
                new HashMap(),
                agent);
    }
}
