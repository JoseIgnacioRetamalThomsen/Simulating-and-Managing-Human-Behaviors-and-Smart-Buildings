using System;
using RestServer;
using RestServer.Helper;
using UnityEngine;

namespace de.bearo.restserver.Samples.BinaryResponse {
    public class BRBinaryResponse : MonoBehaviour {
        public RestServer.RestServer server;

        // Texture must have the "Advanced Settings -> Read/Write" checkbox set, no compression allowed. These are Unity/EncodeToPNG restrictions!
        public Texture2D texture;

        private byte[] binaryTexture;

        // Start is called before the first frame update
        void Start() {
            binaryTexture = texture.EncodeToPNG();
            server.EndpointCollection.RegisterEndpoint(HttpMethod.GET, "/image", GetImage);
        }

        void GetImage(RestRequest request) {
            request.CreateResponse()
                .Header(HttpHeader.CONTENT_TYPE, MimeType.IMAGE_PNG)
                .Body(binaryTexture)
                .SendAsync();
        }
    }
}