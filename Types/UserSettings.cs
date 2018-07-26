using System.Collections.Generic;

namespace AparmentListingMonitor
{
    public class UserProfile
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public List<string> ListingUrls { get; set; }
        public List<string> History { get; set; }
    }
}