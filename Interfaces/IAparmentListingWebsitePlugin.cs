using System.Collections.Generic;

namespace AparmentListingMonitor
{
    public interface IApartmentListingWebsitePlugin
    {
        string Host { get; }
        List<ApartmentListing> GetApartmentListing(string searchUrl);
    }
}