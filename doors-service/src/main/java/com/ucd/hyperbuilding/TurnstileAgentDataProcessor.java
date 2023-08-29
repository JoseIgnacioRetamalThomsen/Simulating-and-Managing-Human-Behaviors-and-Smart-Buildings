package com.ucd.hyperbuilding;

import astra.core.Agent;
import astra.core.Module;
import astra.formula.Predicate;
import astra.term.Primitive;
import astra.term.Term;
import com.google.gson.Gson;
import com.ucd.hyperbuilding.data.Thing;

import java.util.Optional;

public class TurnstileAgentDataProcessor extends Module implements AgentEnhancer{
    Gson gson = new Gson();

    @ACTION
    public boolean processTurnstileThing(String thingJsonString) {
        Thing turnstileThing = gson.fromJson(thingJsonString, Thing.class);
        String cardEventUrl = turnstileThing.Events.get(0).Forms.get(0).Href;
        String toggleUrl = turnstileThing.actions.get(0).Forms.get(0).Href;
        String sensor1Url = turnstileThing.Events.get(1).Forms.get(0).Href;
        String statusUrl = turnstileThing.properties.get(0).Forms.get(0).Href;
        setBelief("subscribeToTurnstileEvents_cardUrl_sensor1Url",cardEventUrl, sensor1Url);
        setBelief("toggleUrl", toggleUrl);
        setBelief("statusUrl",statusUrl);
        setBelief("doorStatus","closed");
        setBelief("thingProcessed", true);
        return true;
    }

    @ACTION
    public boolean updateStatus(String jsonContent){
        StatusJson turnstileThing = gson.fromJson(jsonContent, StatusJson.class);
        Optional<Predicate> actualStatusPredicate = agent.beliefs().beliefs().stream()
                .filter(f -> f instanceof Predicate)
                .map(f -> (Predicate)f)
                .filter(p -> p.predicate().equals("doorStatus"))
                .findFirst();
        if(actualStatusPredicate.isPresent()){
            agent.beliefs().dropBelief(actualStatusPredicate.get());
        }
        agent.beliefs().addBelief(new Predicate("doorStatus" ,new Term[]{Primitive.newPrimitive(
                turnstileThing.status.toLowerCase()
        )}));
        return true;
    }

    @Override
    public Agent getAgent() {
        return agent;
    }

    private static class StatusJson{
        public String status;
    }
}


