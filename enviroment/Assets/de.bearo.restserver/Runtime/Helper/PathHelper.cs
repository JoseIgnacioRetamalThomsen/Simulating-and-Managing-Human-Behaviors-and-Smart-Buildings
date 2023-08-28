namespace RestServer.Helper {
    public static class PathHelper {
        public static string EnsureSlashPrefix(string s) {
            if (string.IsNullOrEmpty(s)) {
                return "/";
            }

            if (!s.StartsWith("/")) {
                return "/" + s;
            }

            return s;
        }

        public static string RemoveEndingSlash(string s) {
            if (string.IsNullOrEmpty(s)) {
                return "";
            }

            return s.EndsWith("/") ? s.Remove(s.Length - 1) : s;
        }

        public static string ConcatPath(string a, string b) {
            if (string.IsNullOrEmpty(b)) {
                return a; // prevent trailing slash if b is empty
            }

            return RemoveEndingSlash(a) + EnsureSlashPrefix(b);
        }
    }
}