using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using Utilities;
using Utilities.Models;

namespace TaskTracker.Models
{
    public class MentorRepositery
    {
        ExceptionLogsRepositery exceptionLogsRepositery = new ExceptionLogsRepositery();
        
        TaskTrackerEntities db = new TaskTrackerEntities();
        Mentor mentor = new Mentor();
        List<Mentor> mentorList = new List<Mentor>();

        public void EditMentor(Mentor mentor)
        {
            try
            {
                db.Entry(mentor).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }
        }
        /// <summary>
        /// to create mentor
        /// </summary>
        /// <param name="mentor"></param>
        public void CreateMentor(Mentor mentor)
        {
            try
            {
                db.Mentor.Add(mentor);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }
        }

        /// <summary>
        /// find mentor using mentor id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Mentor GetMentorUsingId(int? id)
        {
            try
            {
                mentor = db.Mentor.Find(id);
            }
            catch (Exception ex)
            {
                
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }
            return mentor;
        }

        /// <summary>
        ////Get Mento rDetails Using EmployeeId 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Mentor GetMentorDetailsUsingEmployeeId (int id)
        {
            try
            {
                mentor=db.Mentor.Where(x => x.EmployeeId == id && x.IsDeleted==false).FirstOrDefault();
            }
            catch (Exception ex)
            {
                
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }
            return mentor; 
        }

        public Mentor GetMentorDetailsUsingEmployeeMentorId(int id)
        {
            try
            {
                mentor = db.Mentor.Where(x => x.EmployeeMentorId == id && x.IsDeleted == false).FirstOrDefault();
            }
            catch (Exception ex)
            {

                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }
            return mentor;
        }

        /// <summary>
        ////Delete mentor 
        /// </summary>
        /// <param name="mentor"></param>
        /// <returns></returns>
        public Mentor DeleteMentor(Mentor mentor)
        {
            try
            {
                Mentor nMentor = db.Mentor.Single(x => x.Id == mentor.Id);
                nMentor.IsDeleted = true;
                nMentor.DeletedOn = DateTime.Now;

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }
            return mentor;
        }

        /// <summary>
        /// delete mentor for trainee
        /// </summary>
        /// <param name="id"></param>
        public void DeleteMentorByTraineeId(int id)
        {
            try
            {
               
                db.Mentor.Where(x => x.EmployeeId == id && x.IsDeleted == false).ToList().ForEach(
                    x =>
                    {
                        x.IsDeleted = true;
                        x.DeletedOn = DateTime.Now;
                    });
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                //HttpRequest request = HttpContext.Current.Request;
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = "";// request.Url.AbsoluteUri;
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }
        }

        /// <summary>
        /// delete mentor 
        /// </summary>
        /// <param name="id"></param>
        public void DeleteMentorByTraineerId(int id)
        {
            try
            {
                
                db.Mentor.Where(x => x.EmployeeMentorId == id && x.IsDeleted == false).ToList().ForEach(
                    x =>
                    {
                        x.IsDeleted = true;
                        x.DeletedOn = DateTime.Now;
                    });
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                //HttpRequest request = HttpContext.Current.Request;
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = "";// request.Url.AbsoluteUri;
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }
        }

        

        /// <summary>
        ////Get all mentor from mentor table
        /// </summary>
        /// <returns></returns>
        public List<Mentor> GetAllMentorDetails()
        {
            try
            {
                mentorList = db.Mentor.Where(x=>x.IsDeleted==false).ToList();
            }
            catch (Exception ex)
            {
                
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }
            return mentorList;
        }

        /// <summary>
        /// to get all mentor where isdeleted is false 
        /// </summary>
        /// <returns>mentor list</returns>
        public IQueryable<Mentor> GetMentorsForIndex()
        {
            var mentor = db.Mentor.Where(x=>x.IsDeleted==false && x.Users.IsDeleted == false).Include(m => m.Users);
            return mentor;
        }
        //Method to Read mentor by mentor Id
        public List<Mentor> ReadMentor_ByMentorId(int id)
        {
            try
            {
                mentorList = db.Mentor.Where(x => x.IsDeleted == false && x.EmployeeMentorId == id).ToList();
            }
            catch (Exception ex)
            {

                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }
            return mentorList;
        }

        public void  DeleteMentorByTraineeIdPermanent(Mentor mentor)
        {
            try
            {
                mentor = db.Mentor.Remove(mentor);
            }
            catch (Exception ex)
            {

                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }

        }

        public void DeleteMentorByTraineerIdPemanent(Mentor mentor)
        {
            try
            {
                mentor = db.Mentor.Remove(mentor);
            }
            catch (Exception ex)
            {

                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }

        }
        public void DeleteMentorPermanent(List<Mentor> mentorList)
        {
            try
            {
                //mentor = db.Mentor.Remove(mentor);
                db.Mentor.RemoveRange(mentorList);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }

        }

        public List<Mentor> GetALLMentorByEmployeeMentorId(int id)
        {
            try
            {
                mentorList = db.Mentor.Where(x => x.EmployeeMentorId == id).ToList();
            }
            catch (Exception ex)
            {

                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }
            return mentorList;
        }

        public List<Mentor> GetAllMentorByEmployeeId(int id)
        {
            try
            {
                mentorList = db.Mentor.Where(x => x.EmployeeId == id).ToList();
            }
            catch (Exception ex)
            {

                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }
            return mentorList;
        }

    }
}