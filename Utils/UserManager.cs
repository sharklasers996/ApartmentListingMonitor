using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AparmentListingMonitor
{
    public class UserManager
    {
        private string UserDataFile = @"users.txt";
        private string HistoryPath = "History";
        private string NewItemsCountFolder = "Reporting";

        public List<UserProfile> GetUserProfiles()
        {
            if (!File.Exists(UserDataFile))
            {
                throw new FileNotFoundException("Could not find user data file.");
            }

            var userProfiles = new List<UserProfile>();

            using (var sr = new StreamReader(UserDataFile))
            {
                var dataLine = sr.ReadLine();
                while (!String.IsNullOrEmpty(dataLine))
                {
                    userProfiles.Add(ParseUserData(dataLine));
                    dataLine = sr.ReadLine();
                }
            }

            return userProfiles;
        }

        private UserProfile ParseUserData(string line)
        {
            var data = line.Split('|');

            var userProfile = new UserProfile
            {
                Email = data[0],
                Username = data[0].Substring(0, data[0].IndexOf("@")),
                ListingUrls = new List<string>()
            };

            for (var i = 1; i < data.Length; i++)
            {
                userProfile.ListingUrls.Add(data[i]);
            }

            userProfile.History = GetUserHistory(userProfile.Username);

            return userProfile;
        }

        private List<string> GetUserHistory(string username)
        {
            var userHistoryFile = Path.Combine(HistoryPath, username);
            if (!File.Exists(userHistoryFile))
            {
                return new List<string>();
            }

            return File.ReadAllLines(userHistoryFile)
                .ToList()
                .Where(x => !String.IsNullOrEmpty(x))
                .ToList();
        }

        public void AddToHistory(string username, string url)
        {
            IncreaseNewIemCount(username);

            if (!Directory.Exists(HistoryPath))
            {
                Directory.CreateDirectory(HistoryPath);
            }

            var userHistoryFile = Path.Combine(HistoryPath, username);
            using (var sw = new StreamWriter(userHistoryFile, true))
            {
                sw.WriteLine(url);
            }
        }

        private void IncreaseNewIemCount(string username)
        {
            var count = GetNewItemCount(username, out string userNewItemCountFile);
            count++;

            File.WriteAllText(userNewItemCountFile, count.ToString());
        }

        public int GetNewItemCount(string username, out string userNewItemCountFile)
        {
            if (!Directory.Exists(NewItemsCountFolder))
            {
                Directory.CreateDirectory(NewItemsCountFolder);
            }

            userNewItemCountFile = Path.Combine(NewItemsCountFolder, username);
            if (File.Exists(userNewItemCountFile))
            {
                var fileText = File.ReadAllText(userNewItemCountFile);
                if (int.TryParse(fileText, out int count))
                {
                    return count;
                }
            }

            return 0;
        }
    }
}