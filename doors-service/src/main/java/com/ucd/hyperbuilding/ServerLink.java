package com.ucd.hyperbuilding;

import astra.core.Module;
import astra.event.Event;
import astra.reasoner.Unifier;
import astra.term.Primitive;
import astra.term.Term;
import com.google.gson.Gson;
import com.ucd.hyperbuilding.data.CardEvent;
import com.ucd.hyperbuilding.data.SensorEventJson;
import com.ucd.hyperbuilding.event.CardReaderEvent;
import com.ucd.hyperbuilding.event.CardReaderEventUnifier;
import com.ucd.hyperbuilding.event.SensorEvent;
import com.ucd.hyperbuilding.event.SensorEventUnifier;
import io.netty.handler.codec.http.HttpResponseStatus;
import mams.utils.Utils;
import server.core.Path;
import server.core.ResponseEntity;
import server.core.WebServer;

import java.io.IOException;

public class ServerLink extends Module {

    public static final String PATH_CARDEVENT_ID = "/cardevent/{id}";
    public static final String PATH_SENSORS_ID = "/sensors/{id}";

    static {
        Unifier.eventFactory.put(CardReaderEvent.class, new CardReaderEventUnifier());
        Unifier.eventFactory.put(SensorEvent.class, new SensorEventUnifier());
    }

    private final Gson gson = new Gson();
    private WebServer webServer;

    @ACTION
    public boolean startServer(String port) {
        System.out.println("Starting server server:" + port);
        webServer = WebServer.getInstance(Integer.parseInt(port));
        Path cardEventPath = new Path(PATH_CARDEVENT_ID);
        Path sensorEndPoint = new Path(PATH_SENSORS_ID);
        webServer.addPath(cardEventPath);
        cardEventPath.put(state -> {
            String agentID = state.binding.get("id");
            try {

                CardEvent cardId = gson.fromJson(Utils.getBody(state.request), CardEvent.class);
                agent.addEvent(new CardReaderEvent(Primitive.newPrimitive(agentID),
                        Primitive.newPrimitive(cardId.cardId),
                        Primitive.newPrimitive(cardId.turnstileId),
                        Primitive.newPrimitive(String.valueOf(cardId.timeStampSeconds))));
                WebServer.writeResponse(state,
                        ResponseEntity.type(state.contentType)
                                .status(HttpResponseStatus.CREATED));
            } catch (IOException e) {
                WebServer.writeErrorResponse(state, HttpResponseStatus.INTERNAL_SERVER_ERROR);
            }
        });


        webServer.addPath(sensorEndPoint);
        sensorEndPoint.put(state -> {
            String agentID = state.binding.get("id");
            try {
                SensorEventJson sensorEvent = gson.fromJson(Utils.getBody(state.request), SensorEventJson.class);
                System.out.println(sensorEvent);
                agent.addEvent(new SensorEvent(
                        Primitive.newPrimitive(agentID),
                        Primitive.newPrimitive(sensorEvent.sensorId),
                        Primitive.newPrimitive(sensorEvent.type),
                        Primitive.newPrimitive(String.valueOf(sensorEvent.timeStampSeconds))

                ));
                WebServer.writeResponse(state,
                        ResponseEntity.type(state.contentType)
                                .status(HttpResponseStatus.CREATED));
            } catch (IOException e) {
                WebServer.writeErrorResponse(state, HttpResponseStatus.INTERNAL_SERVER_ERROR);
            }
        });
        return true;
    }

    @EVENT(types = {"string"}, signature = "$com.ucd.doors.event.CardReaderEvent", symbols = {})
    public Event cardEvent(Term value1, Term value2, Term value3, Term value4) {
        return new CardReaderEvent(value1, value2, value3, value4);
    }

    @EVENT(types = {"string"}, signature = "$com.ucd.doors.event.SensorEvent", symbols = {})
    public Event sensorEvent(Term agentId, Term sensorId, Term type, Term timeStampSeconds) {
        return new SensorEvent(agentId, sensorId, type, timeStampSeconds);
    }

    @ACTION
    public boolean checkStatus() {
        System.out.println("webserver:" + webServer);
        return true;
    }

}