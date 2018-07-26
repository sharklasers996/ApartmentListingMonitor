using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace AparmentListingMonitor
{
    public class AruodasPlugin : IApartmentListingWebsitePlugin
    {
        public string Host => "www.aruodas.lt";

        public List<ApartmentListing> GetApartmentListing(string searchUrl)
        {
            var listing = new List<ApartmentListing>();

            var source = HtmlHelper.GetHtml(searchUrl);
            var doc = new HtmlDocument();
            doc.LoadHtml(source);

            var nodes = doc.DocumentNode.SelectNodes("//tr[contains(@class,'list-row')]");
            if (nodes != null)
            {
                try
                {
                    listing.AddRange(
                        nodes.Select(n => new ApartmentListing
                        {
                            Link = n.SelectSingleNode(".//h3/a").Attributes["href"].Value.Trim(),
                            Title = n.SelectSingleNode(".//h3/a").InnerHtml.Replace("<br>", " ").Trim(),
                            Price = n.SelectSingleNode(".//span[@class='list-item-price']").InnerText
                        }));
                }
                catch { }
            }

            return listing;
        }
    }
}