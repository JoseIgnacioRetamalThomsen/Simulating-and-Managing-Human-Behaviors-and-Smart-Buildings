using System;
using System.Collections.Generic;
using HyperBuilding.JsonObjects;
using HyperBuilding.ThingObjects;
using UnityEngine;

namespace HyperBuilding
{
    public class HyperBuildingThing : MonoBehaviour
    {
        private Thing _Thing;

        private BodyThing _bodyThing;

        private RoomsThing _roomsThing;

        private TurnstilesThing _turnstilesThing;

        [SerializeField] private RestServer.RestServer _restServer;
        public  string BaseUrl => $"{Configuration.Configuration.Protocol}://{_restServer.ListenAddress}:{_restServer.port}";

        public  string LocalPath => $"/{Configuration.Configuration.MainPath}";

        public  string Url => $"{BaseUrl}{LocalPath}";

        private void Awake()
        {
            _bodyThing = GetComponentInChildren<BodyThing>();
            _roomsThing = GetComponentInChildren<RoomsThing>();
            _turnstilesThing = GetComponentInChildren<TurnstilesThing>();
            Debug.Log("Initializing Hyper Building.");
            Debug.Log("Loading Root Thing Components url=" +Url +", bodiesEndPoint=" +_bodyThing.Url + 
                ", roomsEndPoint= " + _roomsThing.Url );
            CreateThing();
            
        }

        private void CreateThing()
        {
            ThingBuilder thingBuilder = new ();
            thingBuilder
                .WithId("HyperBuilding-id")
                .WithTitle("HyperBuilding")
                .WithDescription("HyperBuilding Simulation")
                .WithContext(Configuration.Configuration.ThingVersion, Url)
                .AddProperty("Human Bodies", "string",
                    new List<string> { "readproperty" }, _bodyThing.Url, "GET", "application/json", null, false)
                .AddProperty("Rooms Managements System", "string",
                    new List<string> { "readproperty" }, _roomsThing.Url, "GET", "application/json", null, false)
                .AddProperty("Doors management system", "string",
                    new List<string> { "readproperty" }, _turnstilesThing.Url, "GET", "application/json", null, false);
                
                
            _Thing = thingBuilder.Build();
        }
        
        public Thing GetThing()
        {
            return _Thing;
        }

        public BodyThing GetBodyThing()
        {
            return _bodyThing;
        }

        public TurnstilesThing GetRoomsThing()
        {
            return _turnstilesThing;
        }
    }
}