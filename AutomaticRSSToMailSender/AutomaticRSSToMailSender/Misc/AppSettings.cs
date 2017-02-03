using System;
using System.Configuration;
using System.Linq;

namespace AutomaticRSSToMailSender
{
    class AppSettings
    {
        public static string GetStringValue(string key)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(key))
            {
                return ConfigurationManager.AppSettings[key];
            }
            else
            {
                throw new Exception($"Missing {key} in App settings");
            }
        }

        public static int GetIntValue(string key, int defaultValue = 5)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(key))
            {
                try
                {
                    return int.Parse(ConfigurationManager.AppSettings[key]);
                }
                catch (Exception)
                {
                    Logger.Instance.Error($"{key} in App Settings doesn't contain an integer. ");
                }
            }
            else
            {
                throw new Exception($"Missing {key} in App settings");
            }

            return defaultValue;
        }

        public static int RefreshIntervalInMinutes()
        {
            return GetIntValue("RefreshIntervalInMinutes") * 60 * 1000;
        }
    }
}
