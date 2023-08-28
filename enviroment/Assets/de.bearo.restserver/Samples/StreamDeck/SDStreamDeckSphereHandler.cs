using System.Collections;
using RestServer;
using UnityEngine;

namespace de.bearo.restserver.Samples.StreamDeck {
    public class SDStreamDeckSphereHandler : MonoBehaviour {
        public RestServer.RestServer restServer;

        public GameObject prefab;

        public GameObject pool;

        public Vector3 spawnLocation;

        public Vector3 forceMultiplier;

        public float multipleSpawnPause = 0.1f;

        // Start is called before the first frame update
        void Start() {
            restServer.EndpointCollection.RegisterEndpoint(HttpMethod.GET, "/spawn", GetSpawnHandler);
            restServer.EndpointCollection.RegisterEndpoint(HttpMethod.GET, "/spawnMultiple", GetSpawnMultipleHandler);
        }

        #region /spawn

        private void GetSpawnHandler(RestRequest request) {
            ThreadingHelper.Instance.ExecuteAsync(SpawnOne);
            request.CreateResponse().SendAsync();
        }

        #endregion

        #region /spawnMultiple

        private void GetSpawnMultipleHandler(RestRequest request) {
            var countStr = request.QueryParameters["count"];
            var count = 1;
            if (!string.IsNullOrEmpty(countStr)) {
                if (int.TryParse(countStr, out var temp)) {
                    count = temp;
                }
            }

            // Use Co-Routine so spheres don't spawn at the same second and fly off horizontally because they partially overlap
            // We also purposely don't lock this, so if it's called while spawning you can experience the effects ;) 
            ThreadingHelper.Instance.ExecuteAsyncCoroutine(() => GetSpawnMultipleHandler_SpawnSphere(count));
            request.CreateResponse().SendAsync();
        }

        IEnumerator GetSpawnMultipleHandler_SpawnSphere(int count) {
            Debug.Log($"Spawn {count}");
            for (var i = 0; i < count; i++) {
                SpawnOne();

                yield return new WaitForSeconds(multipleSpawnPause);
            }
        }

        #endregion

        private void SpawnOne() {
            var newGo = Instantiate(prefab, spawnLocation, Quaternion.identity, pool.transform);
            newGo.SetActive(true);

            var rb = newGo.GetComponent<Rigidbody>();

            var rndVector = new Vector3(
                Random.value * (Random.value < 0.5 ? -1.0f : 1.0f) * forceMultiplier.x,
                Random.value * forceMultiplier.y,
                Random.value * (Random.value < 0.5 ? -1.0f : 1.0f) * forceMultiplier.z
            );


            rb.AddForce(rndVector);
        }

        // Update is called once per frame
        void Update() { }
    }
}