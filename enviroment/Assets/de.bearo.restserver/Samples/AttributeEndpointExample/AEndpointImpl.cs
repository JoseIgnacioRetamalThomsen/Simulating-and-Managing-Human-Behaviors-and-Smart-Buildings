using RestServer;
using RestServer.AutoEndpoints;
using UnityEngine;

namespace de.bearo.restserver.Samples.AttributeEndpointExample {
    public class AEndpointImpl : MonoBehaviour {
        public GameObject go;

        [Endpoint("/get")]
        public void Endpoint_Get(RestRequest r) {
            Debug.Log("GET Called");
            r.CreateResponse().Body(CreateHtmlResponse()).Header(HttpHeader.CONTENT_TYPE, MimeType.TEXT_HTML).SendAsync();
        }

        [Endpoint(HttpMethod.POST, "/post")]
        public void Endpoint_Post(RestRequest r) {
            Debug.Log("POST Called");
            go.SetActive(!go.activeSelf);
            r.CreateResponse().Body(CreateHtmlResponse()).Header(HttpHeader.CONTENT_TYPE, MimeType.TEXT_HTML).SendAsync();
        }


        private string CreateHtmlResponse() {
            var lazyHtml = $"<html><body><p>Game Object is active? {go.activeSelf}</p>" +
                           $"<p>Toggle active state: <form action=\"/api/v1/post\" method=\"POST\"><input type=\"submit\">" +
                           $"</form></p></body></html>";
            return lazyHtml;
        }
    }
}