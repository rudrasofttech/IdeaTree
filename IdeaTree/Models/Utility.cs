using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace IdeaTree
{
    public class Utility
    {
        public static MailAddress AdminEmail { get
            {
                return new MailAddress("admin@ideatree.com", "Admin");
            }
        }

        public static String SiteName { get { return "Idea Tree"; } }

        public static string TextToURL(string text)
        {
            return text.Trim().Replace(" ", "-");
        }

        public static void ReportException(string Message, string body)
        {
            
        }
    }
}
