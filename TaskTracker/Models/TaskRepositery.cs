using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using Utilities;
using Utilities.Models;

namespace TaskTracker.Models
{
    public class TaskRepositery         
    {
       
        ExceptionLogsRepositery exrepo = new ExceptionLogsRepositery();
        TaskTrackerEntities db = new TaskTrackerEntities();
        List<Task> taskList = new List<Task>();
        Task task = new Task();
        List<Mentor> mentorList = new List<Mentor>();


        /// <summary>
        ////Get all task submitted by an employee using employeeId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Task> GetAllTaskByEmployeeId(int id)
        {
            try
            {                
                taskList = db.Task.Where(x => x.EmployeeId == id && x.IsDeleted == false).OrderByDescending(x=>x.CreatedOn).ToList();
                
            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exrepo.InsertException(ex, url, site.Name);
            }
            return taskList;
        }


        /// <summary>
        ////Get all trainee of a particular Employee from mentor table using employee id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Mentor> GetAllTaineesByEmployeeMentorId(int id)
        {
            try
            {
                mentorList = db.Mentor.Where(x => x.EmployeeMentorId == id && x.IsDeleted == false).ToList();
                
            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exrepo.InsertException(ex, url, site.Name);
            }
            return mentorList;
        }

        
        /// <summary>
        ////Get all task from task table
        /// </summary>
        /// <returns></returns>
        public List<Task> GetAllTask()
        {
            try
            {
                taskList = db.Task.ToList();
            }
            catch(Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;//request.Url.AbsoluteUri;
                exrepo.InsertException(ex, url, site.Name);
            }
            return taskList;
        }

        
        /// <summary>
        ///create task 
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public int CreateTask(Task task)
        {
            int taskId=0;
            try
            {
                db.Task.Add(task);
                db.SaveChanges();
                taskId= task.Id;
            }
            catch(Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exrepo.InsertException(ex, url, site.Name);
            }
            return taskId;
        }


        /// <summary>
        ////delete task
        /// </summary>
        /// <param name="task"></param>
        public void DeleteTask(Task task)
        {
            try
            {
                Task nTask = db.Task.Single(x => x.Id == task.Id);
                nTask.IsDeleted = true;
                nTask.DeletedOn = DateTime.Now;

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exrepo.InsertException(ex, url, site.Name);
            }
        }

        /// <summary>
        /// edit a task submitted by employee
        /// </summary>
        /// <param name="task"></param>
        public void EditTask(Task task)
        {
            try
            {
                db.Entry(task).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exrepo.InsertException(ex, url, site.Name);
            }
        }


        /// <summary>
        ////Get tark tetail using id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task GetTaskdetailUsingId(int? id)
        {
            try
            {
                task=db.Task.Where(x => x.Id == id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exrepo.InsertException(ex, url, site.Name);
            }
            return task;
        }

        /// <summary>
        /// to get the task details where IsDeleted is false 
        /// </summary>
        /// <returns></returns>
        public IQueryable<Task> GetTaskForIndex()
        {
            var task = db.Task.OrderByDescending(x=>x.CreatedOn).Where(x=>x.IsDeleted== false && x.Users.IsDeleted == false).Include(t => t.Users);
            return task;
        }

        /// <summary>
        /// To check if Employee submitted his today's task if yes return true
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CheckTodayTaskByEmployeeId(int id)
        {
            try
            {
                
                var task = db.Task.Where(x => x.EmployeeId == id && x.CreatedOn>DateTime.Today).FirstOrDefault();
                if (task != null)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = "";// request.Url.AbsoluteUri;
                exrepo.InsertException(ex, url, site.Name);
            }
            return false;
        }

        public List<Task> GetTaskDetailOfTodaySubmittedTask()
        {
            try
            {
                taskList = db.Task.Where(x => x.CreatedOn < DateTime.Now && x.CreatedOn > DateTime.Today).ToList();

            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = "";// request.Url.AbsoluteUri;
                exrepo.InsertException(ex, url, site.Name);
            }
            return taskList;
        }

        
        public void PermanentDeleteTask(List<Task> taskList)
        {
            try
            {
                db.Task.RemoveRange(taskList);

            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = "";// request.Url.AbsoluteUri;
                exrepo.InsertException(ex, url, site.Name);
            }            
        }

        /// <summary>
        /// to get the task between given time
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<Task> GetTaskByDate(DateTime start, DateTime end)
        {
            try
            {
                taskList = db.Task.Where(x => x.CreatedOn < start && x.CreatedOn > end.Date).ToList();

            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = "";// request.Url.AbsoluteUri;
                exrepo.InsertException(ex, url, site.Name);
            }
            return taskList;
        }

    }
}