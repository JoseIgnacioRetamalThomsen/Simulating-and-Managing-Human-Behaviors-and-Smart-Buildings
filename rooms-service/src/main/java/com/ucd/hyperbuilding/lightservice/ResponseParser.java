package com.ucd.hyperbuilding.lightservice;

import astra.core.Agent;
import astra.core.Module;
import astra.formula.Predicate;
import astra.term.Primitive;
import astra.term.Term;
import com.google.gson.Gson;
import com.ucd.hyperbuilding.lightservice.data.Property;
import com.ucd.hyperbuilding.lightservice.data.Thing;

import java.util.Optional;

public class ResponseParser extends Module implements AgentEnhancer {

    private Gson gson = new Gson();

    @ACTION
    public boolean processMainSystemThing(String thingJsonString) {
        Thing mainThing = gson.fromJson(thingJsonString, Thing.class);
        String predicate = "lightAreaUrl";
        String firstTerm = mainThing.properties.get(0).Forms.get(0).Href;
        mainThing.properties.forEach(p ->

                    setBelief(predicate, p.Forms.get(0).Href)

                );
        log("Light System thing parse, thing={}", mainThing.toString());
        setBelief(predicate, firstTerm);
        dropBelief("isParsing");
        setBelief("isParsing",false);
        return true;
    }

    @ACTION
    public boolean subscribeLightArea(String thing){
        Thing lightAreaThing = gson.fromJson(thing, Thing.class);
        log("Subscribin to light area, lightArea=" + lightAreaThing.toString());
        String toggleUrl = lightAreaThing.actions.get(0).Forms.get(0).Href;
        String subscribeToSensorUrl = lightAreaThing.Events.get(0).Forms.get(0).Href;
        String statusUrl = lightAreaThing.properties.get(0).Forms.get(0).Href;
        setBelief("toggleUrl", toggleUrl);
        setBelief("subscribeToSensorUrl", subscribeToSensorUrl);
        setBelief("statusUrl", statusUrl);
        return true;
    }
//    private void addLigthAreaBeliefs(Thing mainThing) {
//        mainThing.properties.stream()
//                .map(this::convertToPredicate)
//                .forEach(predicate -> agent.beliefs().addBelief(predicate));
//    }
//
//    private Predicate convertToPredicate(Property property) {
//        return new Predicate("lightAreaUrl", new Term[] {
//                Primitive.newPrimitive(property.Forms.get(0).Href)
//        });
//    }

    @Override
    public Agent getAgent() {
        return agent;
    }


}
