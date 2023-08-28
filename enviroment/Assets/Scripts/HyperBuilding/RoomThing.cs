using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HyperBuilding.JsonObjects;
using UnityEngine;

namespace HyperBuilding
{
    public class RoomThing : MonoBehaviour
    {
        [SerializeField] private List<MainLed> _leds;

        [SerializeField] private string ledAreaId;

        private RoomsThing _parentRoomsThing;

        private Queue<ILightAction> _actionsToPerform = new();
        public string LocalPath => $"/{Configuration.Configuration.MainPath}/{Configuration.Configuration.RoomsPath}/{getId()}";
        public string Url => $"{_parentRoomsThing.getBaseUrl()}{LocalPath}";

        public string statusLocalPath => $"{LocalPath}/{Configuration.Configuration.Light}/{Configuration.Configuration.Status}";
        public string statusUrl => $"{Url}/{Configuration.Configuration.Light}/{Configuration.Configuration.Status}";
        public string subscribeEventsLocalPath => $"{LocalPath}/{Configuration.Configuration.Sensor}";
        public string subscribeEventsUrl => $"{Url}/{Configuration.Configuration.Sensor}";
        
        public string toggleLocalPath => $"{LocalPath}/{Configuration.Configuration.Light}/{Configuration.Configuration.Toggle}";
        
        public string toggleUrl => $"{Url}/{Configuration.Configuration.Light}/{Configuration.Configuration.Toggle}";
        
        private Thing LedAreaThing;

        private string localPath;

        public string host = "http://localhost:8080";

        
        private void Awake()
        {
            _leds = new List<MainLed>(GetComponentsInChildren<MainLed>());
            _parentRoomsThing = GetComponentInParent<RoomsThing>();
            ThingBuilder builder = new();
            LedAreaThing = builder.WithId(getId())
                .WithContext(Configuration.Configuration.ThingVersion,Url)
                .WithDescription("A led light in " + getId())
                .AddProperty("Show the current status of the light. It can be on or off.", "string",
                    new List<string> { "readproperty" }, statusUrl, "GET", "application/json",
                    null, false)
                .AddAction("Turn on/off the light", new List<string> { "invocation" }, toggleUrl, "PUT",
                    "application/json", false)
                .AddEvent("Enter/exit the room sensor event.", new Dictionary<string, string>(),
                    new List<string> { "subscribeevent", "unsubscribeevent" },
                    subscribeEventsUrl, "cov:observe")
                .Build();
        }
        
        void Update()
        {
            if (_actionsToPerform.Count > 0)
            {
                ILightAction iAction = _actionsToPerform.Dequeue();
                if (iAction.GetType() == typeof(ToggleLight))
                {
                    Toggle();
                }
            }
        }

        
        public string getId()
        {
            return ledAreaId;
        }
        
        public void TURN_ON()
        {
            foreach (MainLed led in _leds)
            {
                led.TURN_ON();
            }
        }

        public void TURN_OFF()
        {
            foreach (MainLed led in _leds)
            {
                led.TURN_OFF();
            }
        }

        private LedAreaStatus status = LedAreaStatus.OFF;

        public void Toggle()
        {
            if (status == LedAreaStatus.OFF)
            {
                foreach (MainLed led in _leds)
                {
                    led.TURN_ON();
                }

                status = LedAreaStatus.ON;
            }
            else if (status == LedAreaStatus.ON)
            {
                foreach (MainLed led in _leds)
                {
                    led.TURN_OFF();
                }

                status = LedAreaStatus.OFF;
            }
        }


        public Thing getThingDescription()
        {
            return LedAreaThing;
        }

        public bool AddToggleAction(ToggleLight toggleLight)
        {
            _actionsToPerform.Enqueue(toggleLight);
            return true;
        }
        
        public void SensorEnter(string sensorId, string enter)
        {
            long utcTimeInSeconds = (long)System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            Debug.Log("Sensor sensorId=" + sensorId + ", event=" + enter);
            LightSensorEvent lightEvent = new LightSensorEvent()
            {
                sensorId = sensorId,
                type = enter,
                timeStamp = utcTimeInSeconds
            };

            string evenJson = JsonUtility.ToJson(lightEvent);
            string url = "";
            if (_sensorsSubscribersUrls.Count > 0)
            {
                url = _sensorsSubscribersUrls.First();
                Debug.Log("Sensor enter,url=" + url + ", sensorEvent=" + evenJson);
                StartCoroutine(TurnstileThing.SendPutRequest(url, evenJson));
            }
        }

        public string getStatus()
        {
            return this.status.ToString();
        }

        private HashSet<string> _sensorsSubscribersUrls = new();

        public void SubscriveToSensor(EventSubscription eventSubscription)
        {
            Debug.Log("[" + this.getId() + "]- New subscription to sensor event");
            _sensorsSubscribersUrls.Add(eventSubscription.url);
        }
    }

    public enum LedAreaStatus
    {
        ON,
        OFF
    }
    public class LightSensorEvent
    {
        public string sensorId;
        public string type;
        public long timeStamp;
    }
}