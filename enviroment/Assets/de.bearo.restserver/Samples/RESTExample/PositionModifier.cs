using System.Collections;
using RestServer;
using RestServer.Helper;
using UnityEngine;

namespace de.bearo.restserver.Samples.RESTExample {
    public class PositionModifier : MonoBehaviour {
        public RestServer.RestServer server;

        private void Start() {
            // Register Endpoint GET /position and allow modification like /position?x=4&y=2 
            server.EndpointCollection.RegisterEndpoint(HttpMethod.GET, "/position", (request) => {
                var qp = request.QueryParameters;
                var position = UnityNetHelper.GetPosition(this);

                if (qp.Count > 0) {
                    var x = position.x;
                    var y = position.y;
                    var z = position.z;
                    if (!string.IsNullOrEmpty(qp["x"])) {
                        x = int.Parse(qp["x"]);
                    }

                    if (!string.IsNullOrEmpty(qp["y"])) {
                        y = int.Parse(qp["y"]);
                    }

                    if (!string.IsNullOrEmpty(qp["z"])) {
                        z = int.Parse(qp["z"]);
                    }

                    var newPosition = new Vector3(x, y, z);
                    UnityNetHelper.SetPosition(this, newPosition);
                }

                // Test Async
                ThreadingHelper.Instance.ExecuteAsync(() => { transform.position += new Vector3(0.001f, 0.001f, 0.001f); }, "/position Position Updates");

                request.CreateResponse().BodyJson(UnityNetHelper.GetPosition(this)).SendAsync();
            });

            // Register Endpoint POST /position and allow a body like { x: 2 } and add it to the current position
            server.EndpointCollection.RegisterEndpoint(HttpMethod.POST, "/position", (request) => {
                var e = request.JsonBody<Vector3>();

                var h = ThreadingHelper.Instance;
                var newSize = h.ExecuteSync(() => {
                    transform.position += e;

                    return transform.position;
                });
                
                request.CreateResponse()
                    .BodyJson(newSize)
                    .SendAsync();
            });

            // Register Endpoint for a co-routine
            server.EndpointCollection.RegisterEndpoint(HttpMethod.GET, "/triggeranimation", (request) => {
                var h = ThreadingHelper.Instance;
                h.ExecuteAsyncCoroutine(AnimationCoRoutine);
                
                request.CreateResponse().SendAsync();
            });
        }

        private IEnumerator AnimationCoRoutine() {
            transform.position = Vector3.zero;

            for (var i = 0; i < 10; i++) {
                var curX = transform.position.x;
                transform.position = new Vector3(curX + 1, 0, 0);
                
                yield return new WaitForSeconds(1f);
            }

            yield return null;
        }
    }
}