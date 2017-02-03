using System;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;

namespace AutomaticRSSToMailSender
{
    public class RSSMailer
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent _runCompleteEvent = new ManualResetEvent(false);

        public void Start()
        {
            try
            {
                StartWorkerThread();
                Console.WriteLine("Starting...");
                Logger.Instance.Info("Starting...");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Logger.Instance.Error(ex);
            }
        }

        public void Stop() {
            try
            {
                Console.WriteLine("Stopping...");
                Logger.Instance.Info("Stopping...");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Logger.Instance.Error(ex);
            }
        }

        private void StartWorkerThread()
        {
            try
            {
                // Create another thread to continuosly get the RSS feeds
                ThreadPool.QueueUserWorkItem(o => RefreshRSSFeeds());

                //RunAsync(_cancellationTokenSource.Token).Wait();
            }
            finally
            {
                //_runCompleteEvent.Set();
            }
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000);
            }
        }

        private void RefreshRSSFeeds()
        {
            while (true)
            {
                if (_cancellationTokenSource.IsCancellationRequested)
                {
                    break;
                }
                var feeds = ConfigurationManager.AppSettings["RSSFeeds"].Split(';');
                foreach (var feed in feeds)
                {
                    try
                    {
                        Logger.Instance.Info($"Processing {feed}");
                        // Read the feed
                        SyndicationFeed syndFeed = null;
                        Retry.Do(() =>
                        {
                            using (var r = System.Xml.XmlReader.Create(feed))
                            {
                                syndFeed = SyndicationFeed.Load(r);
                            }
                        }, TimeSpan.FromSeconds(3));

                        var item = syndFeed.Items?.OrderByDescending(x => x.PublishDate).FirstOrDefault();
                        
                        if (item != null)
                        {
                            if (!SendInfoTracker.HasSentEmail(feed, item.PublishDate.DateTime))
                            {
                                var emailsString = AppSettings.GetStringValue("Emails");
                                var emails = emailsString.Split(';');
                                foreach (var email in emails)
                                {
                                    MailSender.SendMail(item, email);
                                }
                                SendInfoTracker.AddOrUpdateSendInformation(feed, item.PublishDate.DateTime);
                            }
                        }
                        else
                        {
                            Logger.Instance.Warn("item is null");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Error(ex);
                    }
                }

                Thread.Sleep(AppSettings.RefreshIntervalInMinutes());
            }
        }

    }
}
