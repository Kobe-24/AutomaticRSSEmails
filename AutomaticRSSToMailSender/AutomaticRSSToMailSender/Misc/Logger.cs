using System;

namespace AutomaticRSSToMailSender
{
    public class Logger
    {
        private static readonly Lazy<log4net.ILog> lazy =
            new Lazy<log4net.ILog>(() => log4net.LogManager.GetLogger
                (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType));

        public static log4net.ILog Instance { get { return lazy.Value; } }

        private Logger()
        {
        }

    }


}
