using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HyperBuilding.JsonObjects
{
    [Serializable]
    public class Thing
    {
        public List<object> Context { get; set; }

        public string id;
        public string title;
        public string description;

        public List<string> Security { get; set; }
        public List<Property> properties { get; set; }
        public List<ThingAction> actions { get; set; }
        public List<Event> Events { get; set; }
    }

    [Serializable]
    public class Property
    {
        public string Description { get; set; }
        public string Type { get; set; }
        public List<Form> Forms { get; set; }
        public string Observable { get; set; }
        public bool ReadOnly { get; set; }
    }

    [Serializable]
    public class ThingAction
    {
        public string Description { get; set; }
        public List<Form> Forms { get; set; }

        public bool Safe { get; set; }
    }

    [Serializable]
    public class Event
    {
        public string Description { get; set; }
        public Dictionary<string, string> Data { get; set; }
        public List<Form> Forms { get; set; }
    }

    [Serializable]
    public class Form
    {
        public List<string> op;
        public string Href { get; set; }
        [JsonProperty("cov:methodName")] public string CovMethodName { get; set; }
        public string Subprotocol { get; set; }
        public string ContentType { get; set; }
    }


    public class ThingBuilder
    {
        private readonly Thing _thing;

        public ThingBuilder()
        {
            _thing = new Thing
            {
                Context = new List<object>() // Assuming you want to initialize the Context list
            };
        }

        public ThingBuilder WithContext(string version, string url)
        {
            _thing.Context = new List<object>()
            {
                version,
                new Dictionary<string, string>
                {
                    { "cov", url }
                }
            };
            return this;
        }

        public ThingBuilder WithId(string id)
        {
            _thing.id = id;
            return this;
        }

        public ThingBuilder WithTitle(string title)
        {
            _thing.title = title;
            return this;
        }

        public ThingBuilder WithDescription(string description)
        {
            _thing.description = description;
            return this;
        }

        public ThingBuilder AddProperty(string description, string type, List<string> op, string href,
            string covMethodName,
            string contentType, string observable, bool readOnly)
        {
            if (_thing.properties == null)
            {
                _thing.properties = new List<Property>();
            }

            Property property = new Property
            {
                Description = description,
                Type = type,
                Forms = new List<Form>
                {
                    new Form
                    {
                        op = op,
                        Href = href,
                        CovMethodName = covMethodName,
                        ContentType = contentType
                    }
                },
                Observable = observable,
                ReadOnly = readOnly
            };

            _thing.properties.Add(property);
            return this;
        }

        public ThingBuilder AddAction(string description, List<string> op, string href, string covMethodName,
            string contentType, bool safe)
        {
            if (_thing.actions == null)
            {
                _thing.actions = new List<ThingAction>();
            }

            ThingAction thingAction = new ThingAction
            {
                Description = description,
                Forms = new List<Form>
                {
                    new Form
                    {
                        op = op,
                        Href = href,
                        CovMethodName = covMethodName,
                        ContentType = contentType
                    }
                },
                Safe = safe
            };

            _thing.actions.Add(thingAction);
            return this;
        }

        public ThingBuilder AddEvent(string description, Dictionary<string, string> data, List<string> op, string href,
            string subprotocol)
        {
            if (_thing.Events == null)
            {
                _thing.Events = new List<Event>();
            }

            Event eventObj = new Event
            {
                Description = description,
                Data = data,
                Forms = new List<Form>
                {
                    new Form
                    {
                        op = op,
                        Href = href,
                        Subprotocol = subprotocol,
                        ContentType = "application/json"
                    }
                }
            };

            _thing.Events.Add(eventObj);
            return this;
        }

        // Method to add an additional form to the last action in the actions list
        public ThingBuilder AddFormToLastAction(List<string> op, string href, string covMethodName, string contentType)
        {
            if (_thing.actions != null && _thing.actions.Count > 0)
            {
                ThingAction lastThingAction = _thing.actions[_thing.actions.Count - 1];
                lastThingAction.Forms.Add(new Form
                {
                    op = op,
                    Href = href,
                    CovMethodName = covMethodName,
                    ContentType = contentType
                });
            }

            return this;
        }

        // Method to add an additional form to the last property in the properties list
        public ThingBuilder AddFormToLastProperty(List<string> op, string href, string covMethodName,
            string contentType)
        {
            if (_thing.properties != null && _thing.properties.Count > 0)
            {
                Property lastProperty = _thing.properties[_thing.properties.Count - 1];
                lastProperty.Forms.Add(new Form
                {
                    op = op,
                    Href = href,
                    CovMethodName = covMethodName,
                    ContentType = contentType
                });
            }

            return this;
        }

        // Method to add an additional form to the last event in the events list
        public ThingBuilder AddFormToLastEvent(List<string> op, string href, string subprotocol)
        {
            if (_thing.Events != null && _thing.Events.Count > 0)
            {
                Event lastEvent = _thing.Events[_thing.Events.Count - 1];
                lastEvent.Forms.Add(new Form
                {
                    op = op,
                    Href = href,
                    Subprotocol = subprotocol
                });
            }

            return this;
        }

        public Thing Build()
        {
            return _thing;
        }
    }
}