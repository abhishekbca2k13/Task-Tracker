using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Utilities;
using Utilities.Models;

namespace TaskTracker.Models
{
    public class EmailLogsRepositery
    {
        TaskTrackerEntities db = new TaskTrackerEntities();
        EmailLogs emailLogs = new EmailLogs();
        List<EmailLogs> emailLogsList = new List<EmailLogs>();
        ExceptionLogsRepositery exceptionLogsRepositery = new ExceptionLogsRepositery();
       


        /// <summary>
        ////Insert email log details into EmailLogTable
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="smtp"></param>
        /// <param name="ipAddress"></param>
        /// <param name="status"></param>
        public void InsertEmaillogs(string from,string to,string subject,string body,string smtp,string ipAddress,string status)
        {
            try
            {
                emailLogs.FromAddress = from;
                emailLogs.ToAddress = to;
                emailLogs.Subject = subject;
                emailLogs.Body = body;
                emailLogs.Smtp = smtp;
                emailLogs.IpAddress = ipAddress;
                emailLogs.Status = status;

                db.EmailLogs.Add(emailLogs);
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = "";// HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }
        }

        
        /// <summary>
        /// All eamil id of employee's  who submitted today's task
        /// </summary>
        /// <returns>email logs list</returns>
        public List<EmailLogs> GetEmailLogsDetailOfTodaySubmittedTask()
        {
            try
            {
                emailLogsList = db.EmailLogs.Where(x => x.CreatedOn > DateTime.Today && x.Subject== "Task Submitted Successfully").ToList();
            }
            catch (Exception ex)
            {
               
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = "";// HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }
            
            return emailLogsList;
        }


        /// <summary>
        /// All eamil id of employee's  who got the today's first Reminder email
        /// </summary>
        /// <returns>email logs list</returns>
        public List<EmailLogs> GetEmailLogsDetailOfTodaySentReminder()
        {
            try
            {
                emailLogsList = db.EmailLogs.Where(x => x.CreatedOn > DateTime.Today && x.Subject == "First Task Reminder").ToList();
            }
            catch (Exception ex)
            {

                MethodBase site = MethodBase.GetCurrentMethod();
                string url = "";// HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }

            return emailLogsList;
        }

        /// <summary>
        /// All eamil id of employee's  who got the today's second Reminder email
        /// </summary>
        /// <returns>email logs list</returns>
        public List<EmailLogs> GetEmailLogsDetailOfTodaySentSecondReminder()
        {
            try
            {
                emailLogsList = db.EmailLogs.Where(x => x.CreatedOn > DateTime.Today && x.Subject == "Second Task Reminder").ToList();
            }
            catch (Exception ex)
            {

                MethodBase site = MethodBase.GetCurrentMethod();
                string url = "";// HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }

            return emailLogsList;
        }

        /// <summary>
        /// All eamil id of employee's  who got the today's second Reminder email
        /// </summary>
        /// <returns>email logs list</returns>
        public List<EmailLogs> GetEmailLogsDetailOfTodaySentThirdReminder()
        {
            try
            {
                emailLogsList = db.EmailLogs.Where(x => x.CreatedOn > DateTime.Today && x.Subject == "Third Task Reminder").ToList();
            }
            catch (Exception ex)
            {

                MethodBase site = MethodBase.GetCurrentMethod();
                string url = "";// HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }

            return emailLogsList;
        }

        /// <summary>
        /// All eamil id of employee's  who got the today's warning email
        /// </summary>
        /// <returns>email logs list</returns>
        public List<EmailLogs> GetEmailLogsDetailOfTodaySentWarning()
        {
            try
            {
                emailLogsList = db.EmailLogs.Where(x => x.CreatedOn > DateTime.Today && x.Subject == "Regarding Task Warning").ToList();
            }
            catch (Exception ex)
            {

                MethodBase site = MethodBase.GetCurrentMethod();
                string url = "";// HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }

            return emailLogsList;
        }

        public List<EmailLogs> GetAllEmail()
        {
            try
            {
                emailLogsList = db.EmailLogs.OrderByDescending(x => x.CreatedOn).ToList();
            }
            catch (Exception ex)
            {

                MethodBase site = MethodBase.GetCurrentMethod();
                string url = "";// HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }
            return emailLogsList;
        }
    }
}