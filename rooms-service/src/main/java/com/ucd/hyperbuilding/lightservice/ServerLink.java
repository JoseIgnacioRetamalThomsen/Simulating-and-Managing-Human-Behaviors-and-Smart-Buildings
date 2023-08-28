package com.ucd.hyperbuilding.lightservice;

import astra.core.Agent;
import astra.core.Module;
import astra.event.Event;
import astra.reasoner.Unifier;
import astra.term.Term;
import com.google.gson.Gson;
import com.ucd.hyperbuilding.lightservice.data.LightSensorEvent;
import com.ucd.hyperbuilding.lightservice.data.SensorEvent;
import com.ucd.hyperbuilding.lightservice.data.SensorEventUnifier;
import server.core.Path;
import server.core.Utils;
import server.core.WebServer;

import java.io.IOException;

public class ServerLink extends Module implements AgentEnhancer{
    static {
        Unifier.eventFactory.put(SensorEvent.class, new SensorEventUnifier());
    }
    private final Gson gson = new Gson();
    private WebServer webServer;

    @ACTION
    public boolean startServer(String port) {
        System.out.println("Starting server server:" + port);
        webServer = WebServer.getInstance(Integer.parseInt(port));


        Path sensorPath = new Path("/sensors/{id}");
        webServer.addPath(sensorPath);
        sensorPath.put(state ->{
            String agentID = state.binding.get("id");
            log("new sensor event, agentId= " +agentID);
            try {
                LightSensorEvent event = gson.fromJson(Utils.getBody(state.request),LightSensorEvent.class);
                log("Event parse, event={}", event.toString());
                agent.addEvent(new SensorEvent(agentID,event.sensorId,event.type, String.valueOf(event.timeStamp)));
            } catch (IOException e) {
                throw new RuntimeException(e);
            }
        });

        return true;

    }

    @EVENT(types = {"string"}, signature = "$com.ucd.hyperbuilding.lightservice.date.SensorEvent", symbols = {})
    public Event sensorEvent(Term value1, Term value2, Term value3, Term value4) {
        return new SensorEvent(value1, value2, value3, value4);
    }

    @Override
    public Agent getAgent() {
        return agent;
    }
}
