using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace AutomaticRSSToMailSender
{
    public class Program
    {
        public static void Main()
        {
            HostFactory.Run(x =>                                 
            {
                x.Service<RSSMailer>(s =>                        
                {
                    s.ConstructUsing(name => new RSSMailer());     
                    s.WhenStarted(tc => tc.Start());              
                    s.WhenStopped(tc => tc.Stop());               
                });
                x.RunAsLocalSystem();                            

                x.SetDescription("RSS 2 mail sender");        
                x.SetDisplayName("RSS mail sender");                       
                x.SetServiceName("RSS mail sender");                       
            });                                                  
        }
    }
}
