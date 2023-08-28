using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using HyperBuilding.JsonObjects;
using UnityEngine;
using UnityEngine.Serialization;


namespace HyperBuilding.ThingObjects
{
    /// <summary>
    /// Manage the root agent bodies end point and keep reference to each agent body
    /// </summary>
    public class BodyThing : MonoBehaviour
    {
        [SerializeField] private Agent AgentPrefab;
        [SerializeField] private Grid GridRef;

        [FormerlySerializedAs("bodiEndPoints")] [FormerlySerializedAs("_agentsEndPoints")] [SerializeField]
        private BodyEndPoints bodyEndPoints;

        private Thing _Thing;
        private HyperBuildingThing _hyperBuildingThing;
        public string LocalPath => $"/{Configuration.Configuration.MainPath}/{Configuration.Configuration.BodyPath}";

        public string Url => $"{_hyperBuildingThing.BaseUrl}{LocalPath}";

        public static string newAgentLocalPAth => "";

        private readonly Dictionary<string, Thing> _agentIdToAgentThing = new();
        private readonly Dictionary<string, Agent> _agentsRefs = new();

        private static int agentCount = 0;

        private void Awake()
        {
            _hyperBuildingThing = GetComponentInParent<HyperBuildingThing>();
            UpdateThing();
        }

        public Thing CreateNewAgent()
        {
            string agentId = "body" + ++agentCount;
            string baseLocalPath = LocalPath + "/" + agentId;
            string baseUrl = Url + "/" + agentId;
            string viewEndPointLocalPAth = baseLocalPath + "/" + "move";
            string statusEndPointLocalPath = baseLocalPath + "/" + "status";
            string actionEndPointLocalPath = baseLocalPath + "/" + "action";
            string viewEndPointUrl = _hyperBuildingThing.BaseUrl + viewEndPointLocalPAth;
            string statusEndPointUrl = _hyperBuildingThing.BaseUrl + statusEndPointLocalPath;
            string actionEndPointUrl = _hyperBuildingThing.BaseUrl + actionEndPointLocalPath;

            Thing agentThing = new ThingBuilder()
                .WithContext(Configuration.Configuration.ThingVersion, baseUrl)
                .WithTitle("Agent")
                .WithDescription("New Agent  Body")
                .WithId(agentId)
                .AddProperty("Agent view", "string",
                    new List<string> { "readproperty" }, viewEndPointUrl, "GET", "application/json", null, false)
                .AddProperty("Agent status", "string",
                    new List<string> { "readproperty" }, statusEndPointUrl, "PUT", "application/json", null, false)
                .AddAction("Perform action.", new List<string> { "invocation" }, actionEndPointUrl, "PUT",
                    "application/json", false)
                .AddAction("Move to position", new List<string> { "invocation" }, viewEndPointUrl, "PUT",
                    "application/json", false)
                .Build();

            _agentIdToAgentThing.Add(agentId, agentThing);
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                AddAgentBody(agentId, viewEndPointLocalPAth, statusEndPointLocalPath, actionEndPointLocalPath);
            });
            return agentThing;
        }

        private void UpdateThing()
        {
            ThingBuilder thingBuilder = new();
            thingBuilder
                .WithId("bodyendpoin-id")
                .WithTitle("AgentBody")
                .WithDescription("End points for body's in the Hyper-building")
                .WithContext(Configuration.Configuration.ThingVersion, Url)
                .AddAction("Create a new body.", new List<string> { "invocation" }, Url, "POST",
                    "application/json", false);
            foreach (Thing agentThing in _agentIdToAgentThing.Values)
            {
                string url = ((Dictionary<string, string>)agentThing.Context[1])["cov"];
                thingBuilder
                    .AddProperty(agentThing.id, "string",
                        new List<string> { "readproperty" }, url, "GET", "application/json", null, false);
            }

            _Thing = thingBuilder.Build();
        }

        public Thing GetThing()
        {
            UpdateThing();
            return _Thing;
        }

        private void AddAgentBody(string agentId, string movesEndPoint, string statusEndPoint,
            string useCardReaderEndPoint)
        {
            GridNode firstFreeNodeFound = GridRef.SectionNameToSectionObjectDic["reception"]
                .NodesList.FirstOrDefault(node => node.isFree);
            if (firstFreeNodeFound == null)
            {
                throw new Exception("Can't add agent because lobby is full.");
            }

            Agent newAgent = Instantiate(AgentPrefab);
            newAgent.id = agentId;
            bool isAdded = GridRef.AddAgent(newAgent, firstFreeNodeFound.GetPosition());
            Debug.Log("new Agent added id=" + agentId);
            _agentsRefs.Add(agentId, newAgent);

            if (isAdded)
            {
                bodyEndPoints.RegisterAgent
                (agentId, LocalPath + "/" + agentId, newAgent, movesEndPoint, statusEndPoint,
                    useCardReaderEndPoint);
            }
        }

        public Thing GetAgentThing(string agentId)
        {
            return _agentIdToAgentThing[agentId];
        }
    }
}