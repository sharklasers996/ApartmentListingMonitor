using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace AparmentListingMonitor
{
    class Program
    {
        private static string _mailManUsername = string.Empty;
        private static string _mailMainPassword = string.Empty;
        private static string _adminEmail = string.Empty;
        private static int _reportingHour = -1;
        private static DateTime? _lastReportDate = null;
        private static int _sleepTime = (int)TimeSpan.FromMinutes(10).TotalMilliseconds;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");

            AppDomain.CurrentDomain.UnhandledException += UnhandledException;

            GetSettings();

            while (true)
            {
                Monitor();
                Report();

                Console.WriteLine("Sleeping...");
                Thread.Sleep(_sleepTime);
            }
        }

        private static void GetSettings()
        {
            var reportingHour = ConfigurationManager.AppSettings["ReportingHour"];
            if (!String.IsNullOrEmpty(reportingHour))
            {
                _reportingHour = Convert.ToInt32(reportingHour);
                Console.WriteLine($"Reporting after {_reportingHour} hour.");
            }
            else
            {
                Console.WriteLine("Reporting not set.");
            }

            _mailManUsername = ConfigurationManager.AppSettings["MailManUsername"];
            _mailMainPassword = ConfigurationManager.AppSettings["MailManPassword"];

            if (String.IsNullOrEmpty(_mailManUsername)
            || String.IsNullOrEmpty(_mailMainPassword))
            {
                throw new Exception("MailMan username and/or password is missing.");
            }

            _adminEmail = ConfigurationManager.AppSettings["AdminEmail"];
            if (String.IsNullOrEmpty(_adminEmail))
            {
                Console.WriteLine("Admin email not set.");
            }
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("Unhandled exception occured.");
            if (e.ExceptionObject is Exception exception)
            {
                Console.WriteLine($"{exception.Message}\n{exception.StackTrace}");
            }
        }

        private static void Monitor()
        {
            var userManager = new UserManager();
            var userProfiles = userManager.GetUserProfiles();

            foreach (var userProfile in userProfiles)
            {
                var newItems = new List<ApartmentListing>();

                foreach (var url in userProfile.ListingUrls)
                {
                    var plugin = PluginHelper.GetPlugin(url);
                    if (plugin == null)
                    {
                        Console.WriteLine($"Could not find plugin for '{url}'.");
                        continue;
                    }

                    Console.WriteLine("Searching for new listings...");
                    var listing = plugin.GetApartmentListing(url);
                    foreach (var l in listing)
                    {
                        if (userProfile.History.Contains(l.Link))
                        {
                            continue;
                        }

                        userManager.AddToHistory(userProfile.Username, url);

                        newItems.Add(l);
                    }
                }

                if (!newItems.Any())
                {
                    Console.WriteLine("No new items found.");
                    continue;
                }

                Console.WriteLine($"Sending {newItems.Count} new listings.");

                var mailMan = new MailMan(_mailManUsername, _mailMainPassword);
                foreach (var listing in newItems)
                {
                    mailMan.SendMail(userProfile.Email, listing.Link, $"{listing.Title} {listing.Price}");
                    userManager.AddToHistory(userProfile.Username, listing.Link);
                }
            }

            Console.WriteLine("Monitor done.");
        }

        private static void Report()
        {
            if (String.IsNullOrEmpty(_adminEmail)
                || DateTime.UtcNow.Hour < _reportingHour
                || DateTime.UtcNow.Day == _lastReportDate?.Day)
            {
                return;
            }

            var userManager = new UserManager();
            var userProfiles = userManager.GetUserProfiles();

            var emailBody = new StringBuilder();
            emailBody.AppendLine("Username\tNew Items");
            emailBody.AppendLine();

            foreach (var userProfile in userProfiles)
            {
                var count = userManager.GetNewItemCount(userProfile.Username, out string userNewItemCountFile);
                emailBody.AppendLine($"{userProfile.Username}\t{count}");

                File.Delete(userNewItemCountFile);
            }

            var mailMan = new MailMan(_mailManUsername, _mailMainPassword);
            mailMan.SendMail(_adminEmail, emailBody.ToString(), $"Stats: {DateTime.Now:dd-mm-yyyy}");
        }
    }
}
