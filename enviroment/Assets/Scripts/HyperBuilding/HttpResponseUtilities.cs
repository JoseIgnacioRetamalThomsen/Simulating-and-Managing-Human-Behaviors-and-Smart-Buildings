using System.Threading.Tasks;
using RestServer;

namespace HyperBuilding
{
    public static class HttpResponseUtilities
    {
        private const string CONTENT_TYPE_HEADER = "Content-Type";
        private const string JSON_UTF_8 = "application/json; charset=UTF-8";

        public static void SendJsonResponseAsync(RestRequest r, string jsonContent, int statusCode = 200)
        {
            r.CreateResponse()
                .Body(jsonContent)
                .Header(CONTENT_TYPE_HEADER, JSON_UTF_8)
                .Status(statusCode)
                .SendAsync();
        }
    }
}