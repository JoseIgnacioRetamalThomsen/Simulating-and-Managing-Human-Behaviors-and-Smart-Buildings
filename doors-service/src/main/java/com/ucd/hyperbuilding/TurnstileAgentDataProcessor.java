package com.ucd.hyperbuilding;

import astra.core.Module;
import astra.formula.Predicate;
import astra.term.Primitive;
import astra.term.Term;
import com.google.gson.Gson;
import com.ucd.hyperbuilding.data.Thing;

import java.util.Optional;

public class TurnstileAgentDataProcessor extends Module {
    Gson gson = new Gson();

    @ACTION
    public boolean processTurnstileThing(String thingJsonString) {
        Thing turnstileThing = gson.fromJson(thingJsonString, Thing.class);

        System.out.println(turnstileThing.description);
        String cardEventUrl = turnstileThing.Events.get(0).Forms.get(0).Href;
        String toggleUrl = turnstileThing.actions.get(0).Forms.get(0).Href;
        String sensor1Url = turnstileThing.Events.get(1).Forms.get(0).Href;
        String statusUrl = turnstileThing.properties.get(0).Forms.get(0).Href;
        System.out.println("Url= " + cardEventUrl);

        agent.beliefs().addBelief
                (new Predicate("subscribeToTurnstileEvents_cardUrl_sensor1Url",
                        new Term[]{
                                Primitive.newPrimitive((String) cardEventUrl),
                                Primitive.newPrimitive((String) sensor1Url)
                        }));


        agent.beliefs().addBelief(new Predicate("toggleUrl", new Term[]{
                Primitive.newPrimitive((String)
                        toggleUrl)}));

        agent.beliefs().addBelief(new Predicate("doorStatus", new Term[]{
                Primitive.newPrimitive((String)
                        "closed")}));

        agent.beliefs().addBelief(new Predicate("thingProcessed", new Term[]{
                Primitive.newPrimitive((boolean)
                        true)}));

        agent.beliefs().addBelief(new Predicate("statusUrl", new Term[]{
                Primitive.newPrimitive(statusUrl
                )}));

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
}


class StatusJson{
    public String status;
}