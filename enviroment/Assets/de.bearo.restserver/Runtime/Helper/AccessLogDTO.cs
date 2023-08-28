using System.Net;

namespace RestServer.Helper {
    public struct AccessLogDTO {
        public readonly IPAddress IPAddress;
        public readonly string HttpMethod;
        public readonly string RequestUri;
        public readonly string Protocol;
        public readonly int ResponseStatus;

        public AccessLogDTO(IPAddress ipAddress, string httpMethod, string requestUri, string protocol, int responseStatus) {
            IPAddress = ipAddress;
            HttpMethod = httpMethod;
            RequestUri = requestUri;
            Protocol = protocol;
            ResponseStatus = responseStatus;
        }
    }
}