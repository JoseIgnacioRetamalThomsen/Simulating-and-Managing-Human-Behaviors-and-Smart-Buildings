using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using HyperBuilding.JsonObjects;
using HyperBuilding.ThingObjects;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace HyperBuilding

{
    public class TurnstileThing : MonoBehaviour
    {
        private const string STATUS_DESCRIPTION =
            "Show the current status of the turnstile. It can be open, closed, or failed.";

        private const string TOGGLE_DESCRIPTION = "Open or close the door";


        public string GetId()
        {
            return _thing.id;
        }

        private TurnstilesThing _parentThing;
        public string LocalPath => $"/{Configuration.Configuration.MainPath}/{Configuration.Configuration.TurnstilePath}/{GetId()}";
        public string Url => $"{_parentThing.getBaseUrl()}{LocalPath}";
        public string toggleLocalPath => $"{LocalPath}/{Configuration.Configuration.Toggle}";
        public string toggleUrl => $"{Url}/{Configuration.Configuration.Toggle}";
        
        public string statusLocalPath => $"{LocalPath}/{Configuration.Configuration.Status}";
        public string statusUrl => $"{Url}/{Configuration.Configuration.Status}";
        
        public string sensorEventLocalPath => $"{LocalPath}/{Configuration.Configuration.Sensor}";
        public string sensorEventUrl => $"{Url}/{Configuration.Configuration.Sensor}";
        
        public string cardEventLocalPath => $"{LocalPath}/{Configuration.Configuration.Card}";
        public string cardEventUrl => $"{Url}/{Configuration.Configuration.Card}";
        
        private string cardEndPoint = "http://localhost:8082//turnstile1/card";

        [SerializeField] public Thing _thing = new Thing();

        private Queue<ITurnstileAction> _actiosToPerfor = new Queue<ITurnstileAction>();

        public TurnstileStatus status { get; set; }

        [SerializeField] private GameObject door;

        private string TURNSTILE_URL;

        public string host = "http://localhost:8080";

        public string baseUrl;

        public bool isOPen()
        {
            return status == TurnstileStatus.OPEN;
        }

        // eng 62
        private void Awake()
        {
            _parentThing = GetComponentInParent<TurnstilesThing>();
            baseUrl = "/turnstile" + "/" + _thing.id;
            CreateThing();
            status = TurnstileStatus.CLOSED;
        }

        
        private void CreateThing()
        {
            ThingBuilder builder = new ThingBuilder();
            Thing thing = builder.WithId(_thing.id)
                .WithTitle(_thing.title)
                .WithDescription(_thing.description)
                .WithContext(Configuration.Configuration.ThingVersion, Url)
                .AddProperty("Show the current status of the turnstile. It can be open, closed, or failed.", "string",
                    new List<string> { "readproperty" }, statusUrl, "GET", "application/json", null,
                    false)
                .AddAction("Open or close the door", new List<string> { "invocation" }, toggleUrl,
                    "PUT",
                    "application/json", false)
                .AddEvent("An agent uses a card to try to open the door.", new Dictionary<string, string>(),
                    new List<string> { "subscribeevent", "unsubscribeevent" },
                    cardEventUrl, "cov:observe")
                .AddEvent("Sensors", new Dictionary<string, string>(),
                    new List<string> { "subscribeevent", "unsubscribeevent" }, sensorEventUrl,
                    "cov:observe")
                .Build();
            _thing = thing;

        }
        public string[] GetActions()
        {
            if (status == TurnstileStatus.CLOSED)
            {
                return new[] { "Card_Reader" };
            }

            return new string[] { };
        }

        private bool haveRequestToOpen = false;

        void Update()
        {
            if (_actiosToPerfor.Count > 0)
            {
                ITurnstileAction iAction = _actiosToPerfor.Dequeue();
                if (iAction.GetType() == typeof(ToggleTurnstile))
                {
                    Toggle();
                    haveRequestToOpen = false;
                }
            }
        }

        private List<string> cardReaderSuscribers = new List<string>();

        public void SubscribeToCardReader(string url)
        {
            Debug.Log("Subscribing to card reader, url=" + url);
            cardReaderSuscribers.Add(url);
        }

        private bool isSending = false;

        public void CHECK_CARD_TEST()
        {
            if (!isSending)
            {
                isSending = true;
                StartCoroutine(CheckCard("ID1"));
            }
        }

        public void Action_Card_Reader(string cardId)
        {
            if (!isSending)
            {
                isSending = true;
                StartCoroutine(CheckCard(cardId));
            }
        }

        IEnumerator AllowNewRequest()
        {
            yield return new WaitForSeconds(0.5f);
            isSending = false;
        }

        IEnumerator CheckCard(String cardId)
        {
            Debug.Log("SENDING POST TO=" + cardReaderSuscribers[0]);
            long utcTimeInSeconds = (long)System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            CardEvent av = new CardEvent()
            {
                cardId = cardId,
                turnstileId = _thing.id,
                timeStampSeconds = utcTimeInSeconds
            };
            string evenJson = JsonUtility.ToJson(av);

            using (UnityWebRequest request1 = UnityWebRequest.Put(cardReaderSuscribers[0], evenJson))
            {
                yield return request1.SendWebRequest();

                if (request1.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("put request succeeded.");
                }
                else
                {
                    Debug.Log("Put request failed: " + request1.error);
                }

                StartCoroutine(AllowNewRequest());
            }
        }

        public static IEnumerator SendPutRequest(string url, string data)
        {
            using (UnityWebRequest request = UnityWebRequest.Put(url, data))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("PUT request succeeded.");
                }
                else
                {
                    Debug.Log("PUT request failed: " + request.error);
                }
            }
        }

        public bool AddToggleAction(ITurnstileAction action)
        {
            if (haveRequestToOpen)
            {
                return false;
            }

            haveRequestToOpen = true;
            _actiosToPerfor.Enqueue(action);
            return true;
        }

        public void Toggle()
        {
            if (status == TurnstileStatus.OPEN)
            {
                status = TurnstileStatus.CLOSED;
                door.SetActive(true);
            }
            else if (status == TurnstileStatus.CLOSED)
            {
                status = TurnstileStatus.OPEN;
                door.SetActive(false);
            }
    
        }
        public Thing getThingDescription()
        {
            return _thing;
        }

        private List<String> sensorsSubscriberUrls = new List<string>();

        public void SubscribeToSensor(string sensorName, string url)
        {
            Debug.Log("Subscribing to sensor, sensorName=" + sensorName + ", url=" + url);
            switch (sensorName)
            {
                case "sensor1":
                    sensorsSubscriberUrls.Add(url);
                    break;
            }
        }

        public void SensorEnter(string sensorId, string eventType)
        {
            long utcTimeInSeconds = (long)System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            TurnstileSensorEvent turnstileSensorEvent = new TurnstileSensorEvent()
            {
                sensorId = sensorId,
                type = eventType,
                timeStampSeconds = utcTimeInSeconds
            };
            string evenJson = JsonUtility.ToJson(turnstileSensorEvent);
            string url = "";
            url = sensorsSubscriberUrls[0];
            Debug.Log("Sensor enter,url=" + url + ", sensorEvent=" + evenJson);
            StartCoroutine(TurnstileThing.SendPutRequest(url, evenJson));
        }
        
        public string getStatus()
        {
            return status.ToString();
        }
    }

    public enum TurnstileStatus
    {
        CLOSED,
        OPEN,
        FAILED
    }

    public class TurnstileSensorEvent
    {
        public string sensorId;
        public string type;
        public long timeStampSeconds;

    }


    public class CardEvent
    {
        public string cardId;
        public string turnstileId;
        public long timeStampSeconds;
    }

    public interface ITurnstileAction
    {
    }

    public class ToggleTurnstile : ITurnstileAction
    {
    }

    public class EventSubscription
    {
        public string url { get; set; }
    }

    public interface ILightAction
    {
    }

    public class ToggleLight : ILightAction
    {
    }
}