using System;
using System.Diagnostics;

namespace AparmentListingMonitor
{
    [DebuggerDisplay("{Title} {Price}")]
    public class ApartmentListing : IEquatable<ApartmentListing>
    {
        public string Link { get; set; }
        public string Title { get; set; }
        public string Price { get; set; }

        public bool Equals(ApartmentListing other)
        {
            return String.Equals(Link, other.Link);
        }
    }
}