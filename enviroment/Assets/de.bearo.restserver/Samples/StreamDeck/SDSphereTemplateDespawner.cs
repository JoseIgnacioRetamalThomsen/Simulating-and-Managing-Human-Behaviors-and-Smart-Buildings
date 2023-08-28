using UnityEngine;

namespace de.bearo.restserver.Samples.StreamDeck {
    public class SDSphereTemplateDespawner : MonoBehaviour {
        public float maxRadius = 100.0f;
        public float maxTime = 20.0f;

        private float creationTime;

        // Start is called before the first frame update
        void Start() {
            creationTime = Time.time;
        }

        // Update is called once per frame
        void Update() {
            if (transform.position.magnitude > maxRadius || Time.time > creationTime + maxTime) {
                Destroy(gameObject);
            }
        }
    }
}