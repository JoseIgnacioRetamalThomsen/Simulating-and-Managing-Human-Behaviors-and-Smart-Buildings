package com.ucd.hyperbuilding.event;

import astra.core.Agent;
import astra.reasoner.EventUnifier;
import astra.reasoner.Unifier;
import astra.term.Term;

import java.util.HashMap;
import java.util.Map;

public class CardReaderEventUnifier implements EventUnifier<CardReaderEvent> {
    @Override
    public Map<Integer, Term> unify(CardReaderEvent source, CardReaderEvent target, Agent agent) {
        return Unifier.unify(
                new Term[] {source.agentId, source.cardId, source.turnstileId, source.timeStamp},
                new Term[] {target.agentId, target.cardId, target.turnstileId, target.timeStamp},

                new HashMap(),
                agent);
    }
}
