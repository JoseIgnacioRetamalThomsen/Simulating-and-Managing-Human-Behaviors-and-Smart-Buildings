using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using RestServer;
using UnityEngine;
using UnityEngine.Networking;

namespace de.bearo.restserver.Samples.ReverseProxyExample {
    public class ReverseProxyExample : MonoBehaviour {
        public RestServer.RestServer server;
        public string targetUrl = "https://www.markus-seidl.de";

        private readonly ConcurrentQueue<ProxyRequest> _proxyRequestStack = new ConcurrentQueue<ProxyRequest>();

        void Start() {
            server.EndpointCollection.RegisterEndpoint(HttpMethod.GET, new Regex(".*"), request => {
                Debug.Log("Received request for url: " + request.HttpRequest.Url);
                var pr = new ProxyRequest(targetUrl + request.HttpRequest.Url);

                _proxyRequestStack.Enqueue(pr);
                // Wait for the request to finish on the main thread, or timeout if it takes too long (timeout is also used, when
                // the main thread request fails)
                pr.WaitHandle.WaitOne(5000);


                // Response is ready at this point
                var r = pr.ProxyResponse;
                if (r != null) {
                    request.CreateResponse()
                        .Status((int)r.ResponseCode)
                        .Body(r.Data) // this sets the content type, therefore the header call must be below so the original content type is preserved
                        .Headers(ConvertHeaders(r.Headers))
                        .SendAsync();
                }
                else {
                    request.CreateResponse()
                        .InternalServerError()
                        .SendAsync();
                }
            });
        }

        private Dictionary<string, List<string>> ConvertHeaders(Dictionary<string, string> simpleHeaders) {
            var ret = new Dictionary<string, List<string>>();
            foreach (var header in simpleHeaders) {
                ret[header.Key] = new List<string>(new[] { header.Value });
            }

            // Remove GZIP indications from the header, as unity transparently decodes them
            // and the response is no longer GZIP compressed
            ret.Remove("Transfer-Encoding");
            ret.Remove("Content-Encoding");

            return ret;
        }


        void Update() {
            if (_proxyRequestStack.IsEmpty) {
                return;
            }

            while (_proxyRequestStack.TryDequeue(out var request)) {
                // If frame timing is important, this should be throttled, otherwise FPS will stall
                // when there are many incoming requests
                StartCoroutine(DoRequest(request));
            }
        }

        IEnumerator DoRequest(ProxyRequest proxyRequest) {
            Debug.Log("Request started: " + proxyRequest.RequestUri);

            using var uwr = UnityWebRequest.Get(proxyRequest.RequestUri);
            yield return uwr.SendWebRequest();

            Debug.Log("Request finished: " + proxyRequest.RequestUri + " with: " + uwr.result);

            proxyRequest.ProxyResponse = new ProxyResponse(uwr);
            proxyRequest.WaitHandle.Set();
        }
    }

    public class ProxyRequest {
        public readonly string RequestUri;
        public readonly AutoResetEvent WaitHandle;
        public ProxyResponse ProxyResponse;

        public ProxyRequest(string requestUri) {
            RequestUri = requestUri;
            WaitHandle = new AutoResetEvent(false);
        }
    }

    public class ProxyResponse {
        public readonly long ResponseCode;
        public readonly byte[] Data;
        public readonly Dictionary<string, string> Headers;

        public ProxyResponse(UnityWebRequest uwr) {
            ResponseCode = uwr.responseCode;
            Data = uwr.downloadHandler.data;
            Headers = uwr.GetResponseHeaders();
        }
    }
}