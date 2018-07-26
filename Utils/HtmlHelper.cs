using System.Net;

namespace AparmentListingMonitor
{
    public static class HtmlHelper
    {
        public static string GetHtml(string url)
        {
            using (var wc = new WebClient())
            {
                return wc.DownloadString(url);
            }
        }
    }
}