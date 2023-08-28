using System;
using System.Collections;
using System.Collections.Generic;
using HyperBuilding.JsonObjects;
using Newtonsoft.Json;
using RestServer;
using UnityEngine;

namespace HyperBuilding
{
    public class TurnstileEndPoint : MonoBehaviour
    {
        [SerializeField] private RestServer.RestServer server;

        [SerializeField] private TurnstileThing[] _turnstileThings;
        [SerializeField] private HyperBuildingThing _hyperBuildingThing;

        private void Start()
        {
            string rootThingUrl = _hyperBuildingThing.GetRoomsThing().LocalPath;
            server.EndpointCollection.RegisterEndpoint(HttpMethod.GET, rootThingUrl, r =>
            {
                string jsonThing = JsonConvert.SerializeObject(_hyperBuildingThing.GetRoomsThing().GetThing());
                HttpResponseUtilities.SendJsonResponseAsync(r, jsonThing);
            });
            
            foreach (var turnstileThing in _turnstileThings)
            {
                //string baseUrl = "/turnstile" + "/" + turnstileThing._thing.id;
                string baseUrl = turnstileThing.baseUrl;
                string cardReaderUrl = turnstileThing._thing.Events[0].Forms[0].Href;
                server.EndpointCollection.RegisterEndpoint(HttpMethod.GET, turnstileThing.LocalPath,
                    (request) =>
                    {
                        Thing thingDescription = turnstileThing.getThingDescription();
                        string json = JsonConvert.SerializeObject(thingDescription);
                        // request.CreateResponse().Body(json).Status(200).SendAsync();
                        HttpResponseUtilities.SendJsonResponseAsync(request, json);
                    });

                server.EndpointCollection.RegisterEndpoint(HttpMethod.PUT, turnstileThing.toggleLocalPath,
                    (request) =>
                    {
                        bool canAddAction = turnstileThing.AddToggleAction(new ToggleTurnstile());
                        int status = 400;
                        if (canAddAction)
                        {
                            status = 200;
                        }

                        request.CreateResponse().Body("{}").Status(status).SendAsync();
                    });

                server.EndpointCollection.RegisterEndpoint(HttpMethod.GET, baseUrl + "/status",
                    request =>
                    {
                        string json = "{'status':'" + turnstileThing.status + "'}";
                        request.CreateResponse().Body(json).Status(200).SendAsync();
                    });

                server.EndpointCollection.RegisterEndpoint(HttpMethod.POST, turnstileThing.cardEventLocalPath,
                    request =>
                    {
                        Debug.Log("New Agent card suscription, request.HttpResponse.Body=" + request.HttpResponse.Body);
                        Debug.Log("response=" + request.Body);
                        EventSubscription eventSubscription =
                            JsonConvert.DeserializeObject<EventSubscription>(request.Body);
                        turnstileThing.SubscribeToCardReader(eventSubscription.url);
                        request.CreateResponse().Status(200).SendAsync();
                    });

                server.EndpointCollection.RegisterEndpoint((HttpMethod.POST), turnstileThing.sensorEventLocalPath,
                    request =>
                    {
                        EventSubscription eventSubscription =
                            JsonConvert.DeserializeObject<EventSubscription>(request.Body);
                        turnstileThing.SubscribeToSensor("sensor1", eventSubscription.url);
                        request.CreateResponse().Status(200).SendAsync();
                    });
                server.EndpointCollection.RegisterEndpoint(HttpMethod.GET, turnstileThing.statusLocalPath,
                    request =>
                    {
                        string status = turnstileThing.getStatus();
                        string statusJson = "{'status':'" + status + "'}";
                        request.CreateResponse().Body(statusJson).Status(200).SendAsync();
                    });

            }
        }
    }
}