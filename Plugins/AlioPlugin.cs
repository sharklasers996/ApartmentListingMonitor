using System.Collections.Generic;
using HtmlAgilityPack;

namespace AparmentListingMonitor
{
    public class AlioPlugin : IApartmentListingWebsitePlugin
    {
        public string Host => "www.alio.lt";

        public List<ApartmentListing> GetApartmentListing(string searchUrl)
        {
            var listing = new List<ApartmentListing>();

            var source = HtmlHelper.GetHtml(searchUrl);
            var doc = new HtmlDocument();
            doc.LoadHtml(source);

            var nodes = doc.DocumentNode.SelectNodes("//div[starts-with(@id, 'lv_ad_id_')]");
            if (nodes != null)
            {

                foreach (var n in nodes)
                {
                    try
                    {
                        listing.Add(new ApartmentListing
                        {
                            Link = n.SelectSingleNode(".//a[@class='advertisement-link cursor-pointer']").Attributes["href"].Value,
                            Title = n.SelectSingleNode(".//div[@class='title']/a[@class='advertisement-link cursor-pointer']").InnerText.Trim(),
                            Price = n.SelectSingleNode(".//span[contains(@class, 'main_price')]").InnerText
                        });
                    }
                    catch { }
                }
            }

            return listing;
        }
    }
}