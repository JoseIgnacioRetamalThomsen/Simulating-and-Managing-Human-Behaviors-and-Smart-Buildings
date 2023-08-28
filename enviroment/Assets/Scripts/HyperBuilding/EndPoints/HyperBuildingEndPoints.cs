using System;
using Newtonsoft.Json;
using RestServer;
using UnityEngine;

namespace HyperBuilding.EndPoints
{
    public class HyperBuildingEndPoints : MonoBehaviour
    {
        [SerializeField] private RestServer.RestServer _server;

        [SerializeField] private HyperBuildingThing _hyperBuildingThing;

        private void Awake()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        private void Start()
        {
            Debug.Log("Creating main end point localPath =" + _hyperBuildingThing.LocalPath);
            _server.EndpointCollection.RegisterEndpoint(HttpMethod.GET, _hyperBuildingThing.LocalPath,
                r =>
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    };

                    string jsonThing = JsonConvert.SerializeObject(_hyperBuildingThing.GetThing(),settings);
                    HttpResponseUtilities.SendJsonResponseAsync(r, jsonThing);
                });
        }
    }
}