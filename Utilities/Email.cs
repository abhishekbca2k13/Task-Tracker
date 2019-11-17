using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utilities.Models;

namespace Utilities
{
    public class Email
    {           
        private ExceptionLogsRepositery _ExceptionLogsRepositeryObject = new ExceptionLogsRepositery();
        //Send mail 
        public int SendEmail(string from, string to, string subject, string body)
        {
            Console.WriteLine("SendEmail()");
            int success = 0;
            try
            {
                MailMessage mm = new MailMessage();
                mm.To.Add(to);
                mm.Subject = subject;
                //Body of the email with email varification link
                mm.Body = string.Format(body);
                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.UseDefaultCredentials = true;
                smtp.Send(mm);
                success = 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in Send Email Function ()----");

                string error = ex.Message;
                MethodBase site = MethodBase.GetCurrentMethod();
                ////HttpRequest request = HttpContext.Current.Request;
                string url = "";// request.Url.AbsoluteUri;
                try
                {
                    _ExceptionLogsRepositeryObject.InsertException(ex, url, site.Name);
                }
                catch (Exception exnew)
                {
                    Console.WriteLine("****Parent");
                    Console.WriteLine("Message : " + ex.Message);
                    Console.WriteLine("StackTrace : " + ex.StackTrace);
                    Console.WriteLine("Data : " + ex.Data);
                    Console.WriteLine("Source : " + ex.Source);
                    Console.WriteLine("****Child");
                    Console.WriteLine("Message : " + exnew.Message.ToString());
                    Console.WriteLine("StackTrace : " + exnew.StackTrace);
                    Console.WriteLine("Data : " + exnew.Data);
                    Console.WriteLine("Source : " + exnew.Source);


                }

            }
            return success;
        }
    }
}
