using System.Collections;
using System.Collections.Generic;
using System.Threading;
using RestServer;
using UnityEngine;

namespace de.bearo.restserver.Samples.MethodAutoEndpointExample {
    public class MAExampleImpl : MonoBehaviour {
        public GameObject go;

        public void EndpointPath_Get(RestRequest r) {
            Debug.Log("GET Called");
            r.CreateResponse().Body(CreateHtmlResponse()).Header(HttpHeader.CONTENT_TYPE, MimeType.TEXT_HTML).SendAsync();
        }

        private string CreateHtmlResponse() {
            var lazyHtml = $"<html><body><p>Game Object is active? {go.activeSelf}</p>" +
                           $"<p>Toggle active state: <form action=\"/api/v1/post\" method=\"POST\"><input type=\"submit\">" +
                           $"</form></p></body></html>";
            return lazyHtml;
        }

        public void EndpointPath_Post(RestRequest r) {
            Debug.Log("POST Called");
            go.SetActive(!go.activeSelf);
            r.CreateResponse().Body(CreateHtmlResponse()).Header(HttpHeader.CONTENT_TYPE, MimeType.TEXT_HTML).SendAsync();
        }
    }
}