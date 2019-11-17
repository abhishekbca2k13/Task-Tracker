using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Utilities;
using Utilities.Models;

namespace TaskTracker.Models
{
    public class AttachmentRepositery
    {
        TaskTrackerEntities db = new TaskTrackerEntities();
        ExceptionLogsRepositery exrepositery = new ExceptionLogsRepositery();
        
        Attachment attachment = new Attachment();
        List<Attachment> attachmentList = new List<Attachment>();

        /// <summary>
        /// to insert details in attachement
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="path"></param>
        /// <param name="extension"></param>
        /// <param name="taskId"></param>
        public void AddAttachment(string fileName, string path, string extension, int taskId)
        {
            try
            {
                attachment.FileName = fileName;
                attachment.Path = path;
                attachment.Extension = extension;
                attachment.TaskId =taskId;
                db.Attachment.Add(attachment);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
               
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exrepositery.InsertException(ex, url, site.Name);
            }
        }

        /// <summary>
        /// to find the all Attachment details 
        /// </summary>
        /// <returns>Attachment details list</returns>
        public List<Attachment> GetAllAttachments()
        {
            try
            {
                attachmentList = db.Attachment.ToList();
            }
            catch (Exception ex)
            {
                
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exrepositery.InsertException(ex, url, site.Name);
            }
            
            return attachmentList;
        }

        /// <summary>
        /// Get all attachements by task id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Attachment> GetAttachmentsByTaskId(int id)
        {
            try
            {
                attachmentList = db.Attachment.Where(x => x.TaskId == id).ToList();
            }
            catch (Exception ex)
            {

                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exrepositery.InsertException(ex, url, site.Name);
            }
            return attachmentList;
        }

        public void PermanentDeleteAttachement(List<Attachment> attachmentList)
        {
            try
            {
                db.Attachment.RemoveRange(attachmentList);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exrepositery.InsertException(ex, url, site.Name);
            }
        }

        /// <summary>
        /// Get attachement detail
        /// </summary>
        /// <param name="id"></param>
        public Attachment GetAttachementById(int id)
        {
            try
            {
                attachment = db.Attachment.Where(x => x.Id == id).FirstOrDefault();
                db.SaveChanges();
            }
            catch (Exception ex)
            {

                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exrepositery.InsertException(ex, url, site.Name);
            }
            return attachment;
        }

        /// <summary>
        /// Permanent delete attachement 
        /// </summary>
        /// <param name="id"></param>
        public void PermanentDeleteAttachementById(int id)
        {
            try
            {
                attachment = db.Attachment.Where(x => x.Id == id).FirstOrDefault();
                db.SaveChanges();
                db.Attachment.Remove(attachment);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exrepositery.InsertException(ex, url, site.Name);
            }
        }
    }
}