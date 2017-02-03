using System;
using System.Collections.Generic;
using System.IO;

namespace AutomaticRSSToMailSender
{
    class SendInfoTracker
    {
        private const string fileName = "SendInformation.txt";

        public static void AddOrUpdateSendInformation(string rssAddress, DateTime date)
        {
            var path = GetPath(fileName);
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(rssAddress + ";" + date);
                }
                return;
            }
            string[] lines = System.IO.File.ReadAllLines(path);

            var updatedLines = new List<string>();

            bool found = false;

            foreach (string line in lines)
            {
                var nameToDate = line.Split(';');

                var rssName = nameToDate[0];
                var dateInFile = nameToDate[1];

                if (rssName != rssAddress)
                {
                    updatedLines.Add(line);
                    continue;
                }

                found = true;

                DateTime parsedDate;
                if (DateTime.TryParse(dateInFile, out parsedDate))
                {
                    if (date > parsedDate)
                    {
                        updatedLines.Add($"{rssName};{date}");
                        break;
                    }
                    updatedLines.Add(line);
                    break;
                }
                Logger.Instance.Error($"Could not parse: {dateInFile}");
                throw new Exception($"Could not parse: {dateInFile}");
            }

            if (!found)
            {
                updatedLines.Add(rssAddress + ";" + date);
            }

            //  Update file with new info
            UpdateFileWithNewInfo(updatedLines, path);
        }

        public static bool HasSentEmail(string rssAddress, DateTime date)
        {
            var path = GetPath(fileName);
            if (!File.Exists(path))
            {
                return false;
            }
            string[] lines = System.IO.File.ReadAllLines(path);

            Logger.Instance.Info($"Contents of {path}");

            bool found = false;

            foreach (string line in lines)
            {
                var nameToDate = line.Split(';');

                var rssName = nameToDate[0];
                var dateInFile = nameToDate[1];
                Logger.Instance.Info($"{rssName} : {dateInFile}");

                if (rssName != rssAddress)
                    continue;

                found = true;
                DateTime parsedDate;
                if (DateTime.TryParse(dateInFile, out parsedDate))
                {
                    if (date > parsedDate)
                    {
                        return false;
                    }
                    return true;
                }
                Logger.Instance.Error($"Could not parse: {dateInFile}");

                throw new Exception($"Could not parse: {dateInFile}");
            }

            if (!found)
            {
                return false;
            }

            return true;
        }
        
        private static void UpdateFileWithNewInfo(List<string> updatedLines, string path)
        {
            File.Delete(path);
            using (StreamWriter sw = File.AppendText(path))
            {
                foreach (var line in updatedLines)
                {
                    sw.WriteLine(line);
                }
            }
        }

        private static string GetPath(string fileName)
        {
            return Path.Combine(Environment.CurrentDirectory, fileName);
        }
    }
}
