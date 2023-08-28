using System;
using System.Collections.Generic;
using HyperBuilding.JsonObjects;
using UnityEngine;

namespace HyperBuilding.ThingObjects
{
    public class TurnstilesThing: MonoBehaviour
    {
        [SerializeField] private List<TurnstileThing> _turnstileThings;
        [SerializeField] private HyperBuildingThing _hyperBuildingThing;
        
        public string LocalPath => $"/{Configuration.Configuration.MainPath}/{Configuration.Configuration.DoorsPath}";
        public string Url => $"{_hyperBuildingThing.BaseUrl}{LocalPath}";


        private Thing _thing;

        private void Awake()
        {
            _turnstileThings = new List<TurnstileThing>(GetComponentsInChildren<TurnstileThing>());

        
        }

        private void Start()
        {
            ThingBuilder builder = new ThingBuilder();
            builder.WithContext(Configuration.Configuration.ThingVersion, Url)
                .WithDescription("Doors management")
                .WithTitle("Doors");
            foreach (TurnstileThing thing in _turnstileThings)
            {
                builder.AddProperty(thing.GetId(), "string",
                    new List<string> { "readproperty" }, thing.Url, "GET", "application/json", null, false);
            }
            

            _thing = builder.Build();
        }

        public Thing GetThing()
        {
            return _thing;
        }

        public string getBaseUrl()
        {
            return _hyperBuildingThing.BaseUrl;
        }
    }
}