namespace RestServer {
    /// <summary>Keep track of internal redirects</summary>
    public class HttpRequestRedirectHelper {
        /// <summary>
        /// Path to redirect to. Do not modify directly use RestRequest#ScheduleInternalRedirect
        /// </summary>
        public string InternalRedirectPath = null;

        /// <summary>
        /// Original Path.
        /// </summary>
        public readonly string OriginalPath;

        /// <summary>
        /// Count of internal redirections; Do not modify directly.
        /// </summary>
        public int RedirectCount = 0;

        public HttpRequestRedirectHelper(string originalPath) {
            OriginalPath = originalPath;
        }
    }
}