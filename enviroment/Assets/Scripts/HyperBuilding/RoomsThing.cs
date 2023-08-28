using System;
using System.Collections;
using System.Collections.Generic;
using HyperBuilding.JsonObjects;
using UnityEngine;

namespace HyperBuilding
{
    public class RoomsThing : MonoBehaviour
    {

        [SerializeField] private List<RoomThing> _ledAreas;

        private Thing _roomsThing;
        [SerializeField]
        private HyperBuildingThing _hyperBuildingThing;
        public string LocalPath => $"/{Configuration.Configuration.MainPath}/{Configuration.Configuration.RoomsPath}";
        public string Url => $"{_hyperBuildingThing.BaseUrl}{LocalPath}";

        private void Start()
        {
            _ledAreas = new List<RoomThing>(GetComponentsInChildren<RoomThing>());
            ThingBuilder builder =
                new ThingBuilder()
                    .WithTitle("Hyper Building Central Control System")
                    .WithDescription("Hyper Building Central Control System")
                    .WithId("HyperBuildincLightSystem");
            builder.WithContext(Configuration.Configuration.ThingVersion, Url);
            _ledAreas.ForEach(ledArea =>
            {
                builder.AddProperty(ledArea.getId(), "string",
                    new List<string> { "readproperty" }, ledArea.Url, "GET", "application/json", null, false);
            });
            _roomsThing = builder.Build();

        }
        
        public Thing getThingDescription()
        {
            return _roomsThing;
        }

        public string getBaseUrl()
        {
            return _hyperBuildingThing.BaseUrl;
        }
    }
}