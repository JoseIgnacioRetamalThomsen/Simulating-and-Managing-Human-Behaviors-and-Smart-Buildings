using System;
using HyperBuilding.JsonObjects;
using RestServer;
using UnityEngine;
using Newtonsoft.Json;

namespace HyperBuilding
{
    /// <summary>
    /// Expose end points for the agent body's
    /// </summary>
    public class BodyEndPoints : MonoBehaviour
    {
        [SerializeField] private RestServer.RestServer server;
        [SerializeField] private HyperBuildingThing _hyperBuildingThing;

        void Start()
        {
            string baseEndPoint = _hyperBuildingThing.GetBodyThing().LocalPath;
            Debug.Log("Creating main body think endpoint localPath=" + baseEndPoint);
            server.EndpointCollection.RegisterEndpoint(HttpMethod.GET, baseEndPoint, r =>
            {
                string jsonThing = JsonConvert.SerializeObject(_hyperBuildingThing.GetBodyThing().GetThing());
                HttpResponseUtilities.SendJsonResponseAsync(r, jsonThing);
            });
            server.EndpointCollection.RegisterEndpoint(HttpMethod.POST, baseEndPoint, (request) =>
            {
                Debug.Log("Creating a new agent, request=" + request);
                Thing newAgentThing = _hyperBuildingThing.GetBodyThing().CreateNewAgent();
                string json = JsonConvert.SerializeObject(newAgentThing);
                HttpResponseUtilities.SendJsonResponseAsync(request, json);
            });
        }

        public void RegisterAgent(string agentId, string localPath, Agent agentRef, string viewEndPoint, string statusEndPoint,
            string useCardReaderEndPoint)
        {
            Debug.Log("Registering agent movesEndPoint=" + viewEndPoint);
            
            server.EndpointCollection.RegisterEndpoint(HttpMethod.GET,localPath, r =>
            {
                Thing thingJson = _hyperBuildingThing.GetBodyThing().GetAgentThing(agentId);
                string json =  JsonConvert.SerializeObject(thingJson);
                HttpResponseUtilities.SendJsonResponseAsync(r, json);
            });

            server.EndpointCollection.RegisterEndpoint(HttpMethod.GET, viewEndPoint, (request) =>
            {
                AgentView view = agentRef.GetShortView();
                string json = JsonConvert.SerializeObject(view);
                HttpResponseUtilities.SendJsonResponseAsync(request, json);
            });

            server.EndpointCollection.RegisterEndpoint(HttpMethod.GET, statusEndPoint, (request) =>
            {
                string status = agentRef.GetStatus();
                string json = JsonConvert.SerializeObject(status);
                request.CreateResponse().Body(json).Status(200).SendAsync();
            });

            server.EndpointCollection.RegisterEndpoint(HttpMethod.PUT, viewEndPoint, request =>
            {
                GridPoint point = JsonConvert.DeserializeObject<GridPoint>(request.Body.ToString());
                IAgentAction action = new MoveToPointAction()
                {
                    pointToMove = point
                };
                agentRef.AddAction(action);
                request.CreateResponse().Body("{id:move1}").Status(200).SendAsync();
            });

            server.EndpointCollection.RegisterEndpoint(HttpMethod.PUT, useCardReaderEndPoint, request =>
            {
                Guid uniqueId = Guid.NewGuid();
                string idString = uniqueId.ToString();
                CardReaderAction action = JsonConvert.DeserializeObject<CardReaderAction>(request.Body.ToString());
                action.id = idString;
                agentRef.AddAction(action);
                request.CreateResponse().Body("{'id':'" + idString + "'}").Status(200).SendAsync();
            });
        }
    }
}