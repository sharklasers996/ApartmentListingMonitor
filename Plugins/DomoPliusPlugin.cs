using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace AparmentListingMonitor
{
    public class DomoPliusPlugin : IApartmentListingWebsitePlugin
    {
        public string Host => "domo.plius.lt";

        public List<ApartmentListing> GetApartmentListing(string searchUrl)
        {
            var listing = new List<ApartmentListing>();

            var source = HtmlHelper.GetHtml(searchUrl);
            var doc = new HtmlDocument();
            doc.LoadHtml(source);

            var nodes = doc.DocumentNode.SelectNodes("//div[@class='item']");
            if (nodes != null)
            {
                try
                {
                    listing.AddRange(
                        nodes.Select(n => new ApartmentListing
                        {
                            Link = n.SelectSingleNode(".//h2[@class='title-list']/a").Attributes["href"].Value.Trim(),
                            Title = n.SelectSingleNode(".//h2[@class='title-list']/a").Attributes["title"].Value,
                            Price = n.SelectSingleNode(".//div[@class='price']/p/strong").InnerText
                        }));
                }
                catch { }
            }

            return listing;
        }
    }
}