using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Utilities;
using Utilities.Models;

namespace TaskTracker.Models
{
    public class EmailQueueRepositery
    {
        TaskTrackerEntities db = new TaskTrackerEntities();
        EmailQueue emailQueue = new EmailQueue();
        EmailQueue nEmailQueue = new EmailQueue();
        List<EmailQueue> emailQueuesList = new List<EmailQueue>();
        ExceptionLogsRepositery exceptionLogsRepositery = new ExceptionLogsRepositery();
        


        // //insert into emailqueue table
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public void InsertEmailDeatailsInEmailQueue(string from,string to,string subject,string body)
        {
            try
            {
                EmailQueue NewemailQueue = new EmailQueue();
                NewemailQueue.FromAddress = from;
                NewemailQueue.ToAddress = to;
                NewemailQueue.Subject = subject;
                NewemailQueue.Body = body;
                NewemailQueue.CreatedOn = DateTime.Now;
                NewemailQueue.ModifiedOn = DateTime.Now;
                db.EmailQueue.Add(NewemailQueue);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
               
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = "";// HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }
            
        }


        /// <summary>
        ////read all record where IsDeleted is 0
        /// </summary>
        /// <returns>emailQueue list</returns>
        public List<EmailQueue> GetEamilQueueDetail()
        {
            try
            {
                emailQueuesList = db.EmailQueue.Where(x => x.IsDeleted == false).ToList();
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = "";// HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }
           
            return emailQueuesList;
        }

        /// <summary>
        /// detete row from emailqueue
        /// </summary>
        /// <param name="emailQueue"></param>
        public void DeletedFromEmailQueue(EmailQueue emailQueue)
        {
            try
            {
                db.EmailQueue.Remove(emailQueue);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = "";//  HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }
                      
            
        }

        /// <summary>
        /// update isdeleted to true in email queue
        /// </summary>
        /// <param name="emailQueue"></param>
        public void UpdateIsDeletedTrue(EmailQueue emailQueue)
        {
            try
            {
                
                nEmailQueue = db.EmailQueue.Where(x => x.Id == emailQueue.Id).FirstOrDefault();
                if (nEmailQueue != null)
                {
                    nEmailQueue.IsDeleted = true;
                    db.SaveChanges();
                }                
            }
            catch (Exception ex)
            {
                
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = "";// HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }
            
        }

        /// <summary>
        ///  update isdeleted to false in email queue
        /// </summary>
        /// <param name="emailQueue"></param>
        public void UpdateIsDeletedFalse(EmailQueue emailQueue)
        {
            try
            {
                EmailQueue nEmailQueue = db.EmailQueue.Where(x => x.Id == emailQueue.Id).FirstOrDefault();
                nEmailQueue.IsDeleted = false;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = "";// HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }
            
        }


        /// <summary>
        /// insrement tries in emailqueue
        /// </summary>
        /// <param name="emailQueue"></param>
        public void IncrementTries(EmailQueue emailQueue)
        {
            try
            {
                EmailQueue nEmailQueue = db.EmailQueue.Where(x => x.Id == emailQueue.Id).FirstOrDefault();
                nEmailQueue.Tries++;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
               
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = "";//  HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }
            

        }

        /// <summary>
        /// read tries from emailqueue
        /// </summary>
        /// <param name="emailQueue"></param>
        /// <returns>no of tries</returns>
        public int GetTries(EmailQueue emailQueue)
        {
            EmailQueue nEmailQueue = new EmailQueue();
            try
            {
                nEmailQueue = db.EmailQueue.Where(x => x.Id == emailQueue.Id).FirstOrDefault();
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = "";// HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }
            if (nEmailQueue != null)
            {
                return nEmailQueue.Tries;
            }
            return 0;
        }

        /// <summary>
        /// Get top email detail from email queue 
        /// </summary>
        /// <returns></returns>
        public EmailQueue GetTopEmailQueueDetail()
        {            
            try
            {           
                emailQueue = db.EmailQueue.Take(1).FirstOrDefault();
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = "";// HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }
            return emailQueue;
        }

        internal void InsertEmailDeatailsInEmailQueue(string v1, string email, string v2, object p)
        {
            throw new NotImplementedException();
        }
    }

    

}