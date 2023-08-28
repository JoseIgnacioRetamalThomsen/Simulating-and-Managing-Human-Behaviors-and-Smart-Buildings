using System.Collections.Generic;
using HyperBuilding.JsonObjects;
using Newtonsoft.Json;
using RestServer;
using UnityEngine;
using UnityEngine.Serialization;

namespace HyperBuilding
{
    public class RoomsEndPoint : MonoBehaviour
    {
        [SerializeField] private RestServer.RestServer server;

        [SerializeField] private List<RoomThing> _ledAreas;

        [FormerlySerializedAs("_ledManager")] [SerializeField]
        private RoomsThing roomsThing;

        void Start()
        {
            Debug.Log("Light end point main url=" + roomsThing.Url);
            server.EndpointCollection.RegisterEndpoint(HttpMethod.GET, roomsThing.LocalPath,
                r =>
                {
                    string jsonThing = JsonConvert.SerializeObject(roomsThing.getThingDescription());
                    r.CreateResponse()
                        .Body(jsonThing)
                        .Header(HttpHeader.CONTENT_TYPE, MimeType.APPLICATION_JSON_UTF_8)
                        .Status(200).SendAsync();
                });
            foreach (RoomThing ledArea in _ledAreas)
            {
                //description end point
                server.EndpointCollection.RegisterEndpoint(HttpMethod.GET, ledArea.LocalPath,
                    (request) =>
                    {
                        Thing thingDescription = ledArea.getThingDescription();
                        string json = JsonConvert.SerializeObject(thingDescription);
                        request.CreateResponse().Body(json)
                            .Header(HttpHeader.CONTENT_TYPE, MimeType.APPLICATION_JSON_UTF_8).Status(200).SendAsync();
                    });
                //status end point
                Debug.Log("Status statusLocalPath=" + ledArea.statusLocalPath);
                server.EndpointCollection.RegisterEndpoint(HttpMethod.GET, ledArea.statusLocalPath,
                    r =>
                    {
                        LigthStatus status = new LigthStatus(ledArea.getStatus());
                        string json = JsonConvert.SerializeObject(status);
                        r.CreateResponse().Body(json)
                            .Header(HttpHeader.CONTENT_TYPE, MimeType.APPLICATION_JSON_UTF_8).Status(200).SendAsync();
                    });
                Debug.Log(
                    "New Light area thingUrl=" + ledArea.LocalPath + ", toggleUrl=" + ledArea.toggleUrl +
                    ", toogleLocalPath=" +
                    ledArea.toggleLocalPath);
                //toggle end point
                server.EndpointCollection.RegisterEndpoint(HttpMethod.PUT, ledArea.toggleLocalPath,
                    (request) =>
                    {
                        bool canAddAction = ledArea.AddToggleAction(new ToggleLight());
                        int status = 400;
                        if (canAddAction)
                        {
                            status = 200;
                        }

                        request.CreateResponse().Body("{}").Status(status).SendAsync();
                    });
                //subscribe to sensor end point
                server.EndpointCollection.RegisterEndpoint(HttpMethod.POST, ledArea.subscribeEventsLocalPath,
                    r =>
                    {
                        EventSubscription eventSubscription =
                            JsonConvert.DeserializeObject<EventSubscription>(r.Body);
                        ledArea.SubscriveToSensor(eventSubscription);
                        r.CreateResponse().Status(200).SendAsync();
                    });
            }
        }
    }

    class LigthStatus
    {
        public LigthStatus(string status)
        {
            this.status = status;
        }

        public string status;
    }
}