
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using TaskTracker.Models;
using System.Security.Cryptography;
using System.Text;
using PagedList;
using TaskTracker.Areas.Employee.Authentication;
using System.IO;
using System.Web.Hosting;
using Utilities;
using Utilities.Models;

namespace TaskTracker.Areas.Employee.Controllers
{
    [OutputCache(NoStore = true, Duration = 0)]
    public class HomeController : Controller
    {
        private TaskTrackerEntities db = new TaskTrackerEntities();
        private UsersRepositery _UsersRepositeryObject = new UsersRepositery();
        private ExceptionLogsRepositery _ExceptionLogsRepositeryObject = new ExceptionLogsRepositery();
        private TaskRepositery _TaskRepositeryObject = new TaskRepositery();
        private AttachmentRepositery _AttachmentRepositeryObject = new AttachmentRepositery();
        private RoleRepositery _RoleRepositeryObject = new RoleRepositery();
        private EmailQueueRepositery _EmailQueueRepositeryObject = new EmailQueueRepositery();       
        private MentorRepositery mentorRepositery = new MentorRepositery();

        Role role = new Role();

        List<Attachment> attachementList = new List<Attachment>();

        List<Task> taskList = new List<Task>();

        List<Mentor> traineeList = new List<Mentor>();

        Users user = new Users();
        List<Users> userList = new List<Users>();

        //==============Employee dashboard=======================================================================================================
        [EmployeeAuthentication]
        public ActionResult EmployeeDashboard(int? page)
        {
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            List<Users> traineeUsersList = new List<Users>();
            List<Task> taskList1 = new List<Task>();
            int id = Convert.ToInt32(Session["UserId"]);

            //All task Submiteed by Employee //read by EmployeeId
            ViewBag.task = _TaskRepositeryObject.GetAllTaskByEmployeeId(id);

            //Employee personal details //read by EmployeeId
            ViewBag.Employee = _UsersRepositeryObject.GetUsersDetailUsingId(id);

            //Employee's trainees details //read by EmployeeMentorId
            traineeList = _TaskRepositeryObject.GetAllTaineesByEmployeeMentorId(id);

            //Read user details for a perticulae employee for there deytails
            foreach (var item in traineeList)
            {
                traineeUsersList.Add(_UsersRepositeryObject.GetUsersDetailUsingId(item.EmployeeId));
            }
            ViewBag.traineeUsersList = traineeUsersList;

            //Get all task from the task table
            taskList = _TaskRepositeryObject.GetAllTask();

            //remove all other employee task who are not under the particular employee
            taskList.RemoveAll(x => !traineeList.Exists(i => x.EmployeeId == i.EmployeeId));
            ViewBag.traineesTask = taskList;

            //read all attechement 
            attachementList = _AttachmentRepositeryObject.GetAllAttachments();
            ViewBag.attachement = attachementList.ToList();
            ViewBag.Message = TempData["Message"];


            return View();
        }
        //===========================================================Employee task=======================================================================
        [EmployeeAuthentication]
        public ActionResult EmployeeTask(int? page)
        {
            ViewBag.Message = TempData["Message"];
            int id = Convert.ToInt32(Session["UserId"]);

            //All task Submiteed by Employee //read by EmployeeId
            var task = _TaskRepositeryObject.GetAllTaskByEmployeeId(id);

            //read all attechement 
            attachementList = _AttachmentRepositeryObject.GetAllAttachments();
            ViewBag.attachement = attachementList.ToList();
            ViewBag.attachementCount = attachementList.Count;
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            return View(task.ToList().ToPagedList(pageNumber, pageSize));
        }
        [EmployeeAuthentication]
        public ActionResult EmployeeTaskDetails(int id)
        {
            //Get task by task id
            ViewBag.task = _TaskRepositeryObject.GetTaskdetailUsingId(id);

            //Get task attachement by task id
            ViewBag.attachement = _AttachmentRepositeryObject.GetAttachmentsByTaskId(id);

            return View();
        }
        //============================================================Trainees Details=================================================================================
        [EmployeeAuthentication]
        public ActionResult TraineesDetails(int? page)
        {
            int id = Convert.ToInt32(Session["UserId"]);
            List<Users> traineeUsersList = new List<Users>();

            //Employee's trainees details //read by EmployeeMentorId
            traineeList = _TaskRepositeryObject.GetAllTaineesByEmployeeMentorId(id);

            //Read user details for a perticulae employee for there deytails
            foreach (var item in traineeList)
            {
                traineeUsersList.Add(_UsersRepositeryObject.GetUsersDetailUsingId(item.EmployeeId));
            }

            int pageSize = 15;
            int pageNumber = (page ?? 1);
            return View(traineeUsersList.ToList().ToPagedList(pageNumber, pageSize));
        }
        //============================================================Trainees task===================================================================================
        [EmployeeAuthentication]
        public ActionResult TraineesTask(int? page)
        {
            int id = Convert.ToInt32(Session["UserId"]);
            //Employee's trainees details //read by EmployeeMentorId
            traineeList = _TaskRepositeryObject.GetAllTaineesByEmployeeMentorId(id);

            //Get all task from the task table
            taskList = _TaskRepositeryObject.GetAllTask();

            //remove all other employee task who are not under the particular employee
            taskList.RemoveAll(x => !traineeList.Exists(i => x.EmployeeId == i.EmployeeId));


            //read all attechement 
            attachementList = _AttachmentRepositeryObject.GetAllAttachments();
            ViewBag.attachement = attachementList.ToList();

            int pageSize = 15;
            int pageNumber = (page ?? 1);
            return View(taskList.ToList().ToPagedList(pageNumber, pageSize));
        }

        [EmployeeAuthentication]
        [HttpGet]
        public ActionResult AllTraineeTask(int? page)
        {
            int id = Convert.ToInt32(Session["UserId"]);
            //Employee's trainees details //read by EmployeeMentorId
            traineeList = _TaskRepositeryObject.GetAllTaineesByEmployeeMentorId(id);


            //Get all employee details
            List<Users> usersList = _UsersRepositeryObject.GetAllUsers();

            //remove all other employee task who are not under the particular employee
            usersList.RemoveAll(x => !traineeList.Exists(i => x.Id == i.EmployeeId));

            //Remove Admin from list
            usersList.RemoveAll(x => x.RoleId == 1);
            ViewBag.Message = TempData["Message"];
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(usersList.ToList().ToPagedList(pageNumber, pageSize));
        }

        [EmployeeAuthentication]
        public ActionResult TraineeTask(int id, int? page)
        {
            //Get all Task by employee Id
            taskList = _TaskRepositeryObject.GetAllTaskByEmployeeId(id);

            //Get all Attachment
            List<Attachment> attachmentList = _AttachmentRepositeryObject.GetAllAttachments();
            ViewBag.attachement = attachmentList.ToList();
            ViewBag.attachementCount = attachmentList.Count;

            //Get Employee Detail by id
            ViewBag.EmployeeDetails = _UsersRepositeryObject.GetUsersDetailUsingId(id);
            ViewBag.Message = TempData["Message"];
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(taskList.ToPagedList(pageNumber, pageSize));
        }

        [EmployeeAuthentication]
        public ActionResult TaskDetails(int id)
        {
            //Get task by task id
            ViewBag.task = _TaskRepositeryObject.GetTaskdetailUsingId(id);

            //Get task attachement by task id
            ViewBag.attachement = _AttachmentRepositeryObject.GetAttachmentsByTaskId(id);

            return View();
        }

        //============================================================================================================================================
        //===============================================Task=========================================================================================
        //============================================================================================================================================

        // GET: Tasks/Create==========================================================================================================================
        [EmployeeAuthentication]
        public ActionResult CreateTask()
        {
            //ViewBag.EmployeeId = new SelectList(db.Users, "Id", "Name");
            return View();
        }

        // POST: Tasks/Create=========================================================================================================================
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [EmployeeAuthentication]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTask([Bind(Include = "Id,Title,Description,DateTime,EmployeeId,CreatedOn,ModifiedOn,DeletedOn,IsDeleted")] Task task, HttpPostedFileBase[] files)
        {
            int taskId;
            bool success;
            try
            {
                if (ModelState.IsValid)
                {
                    task.EmployeeId = Convert.ToInt32(Session["UserId"]);                   
                    //Check if employee submnitted Today's task
                    success = _TaskRepositeryObject.CheckTodayTaskByEmployeeId(task.EmployeeId);
                    if (success == true)
                    {
                        TempData["Message"] = "Multiple task can't be submitted for a day.";
                        return RedirectToAction("EmployeeDashboard", "Home");
                    }
                    taskId = _TaskRepositeryObject.CreateTask(task);

                    //for file upload                

                    if (files[0] != null)
                    {
                        foreach (HttpPostedFileBase file in files)
                        {
                            string fileName = Path.GetFileName(file.FileName);
                            //System.Web.Hosting.HostingEnvironment.MapPath("~/SignatureImages/");
                            //server.MapPath();
                            string path = Path.Combine(HostingEnvironment.MapPath("~/UploadedFile"), fileName);
                            //string path = Path.Combine("ftp://demo8@clanstech.com/UploadedFile", fileName);
                            string extension = Path.GetExtension(path);
                            file.SaveAs(path);

                            //inserting attachement details into attachment table
                            _AttachmentRepositeryObject.AddAttachment(fileName, path, extension, taskId);
                        }

                        //Mail to Employee
                        //Insert email details into emailQueue table
                        string email = Convert.ToString(Session["UserEmail"]);
                        _EmailQueueRepositeryObject.InsertEmailDeatailsInEmailQueue("abhishek.keshari@clanstech.com", email, "Task Submitted Successfully", "<html>Dear " + Session["UserName"]+",<br/><br/><br> You have successfully submitted today's task status.<br/><br/><b>Title: </b>" + task.Title + "<br/><b>Description: </b>" + task.Description + "<br><br/><br>Thanks & regards<br/><br/><img width=196 height=153 src=" + "http://" + Request.Url.Authority+"/Content/Image/Logo.jpg"+"></b><br/><br><address><b>Adderss:</b><br/>G-282 , Sector-63<br/>   Noida -201301 ,<br/>Gautam Buddh Nagar<br/>U.P, India<br><br><b>Website: </b><a href=" + "https://clanstech.com" + ">www.clanstech.com</a><br/><b>Phone:</b> <phone>+91-7042.615.658</phone><br/><b>Maps: </b><a href=" + "https://www.google.com/maps/place/Clanstech+%7C+Providing+Online+Presence/@28.614499,77.3887193,17z/data=!3m1!4b1!4m5!3m4!1s0x390cef928e84423b:0xde8604eb59d84354!8m2!3d28.614499!4d77.390908" + ">Google Maps</a></address></html>");

                        if (Session["MentorEmail"] != null)
                        {
                            //Mail to Employee Mentor
                            string mentorEmail = Convert.ToString(Session["MentorEmail"]);
                            //Insert email details into emailQueue table
                            _EmailQueueRepositeryObject.InsertEmailDeatailsInEmailQueue("abhishek.keshari@clanstech.com", mentorEmail, "Trainee Task Update", "<html>Dear Mentor,<br/><br/><br/> Your trainee have successfully submitted today's task status.<br/><br/><br/><b>Trainee Name: </b>" + Session["UserName"] + "< br/><b>Title: </b>" + task.Title + "<br/><b>Description: </b>" + task.Description + "<br/><br/><br/>Thanks & regards<br/><br/><img width=196 height=153 src=" + "http://" + Request.Url.Authority + "/Content/Image/Logo.jpg" + "></b><br/><br><address><b>Adderss:</b><br/>G-282 , Sector-63<br/>   Noida -201301 ,<br/>Gautam Buddh Nagar<br/>U.P, India<br><br><b>Website: </b><a href=" + "https://clanstech.com" + ">www.clanstech.com</a><br/><b>Phone:</b> <phone>+91-7042.615.658</phone><br/><b>Maps: </b><a href=" + "https://www.google.com/maps/place/Clanstech+%7C+Providing+Online+Presence/@28.614499,77.3887193,17z/data=!3m1!4b1!4m5!3m4!1s0x390cef928e84423b:0xde8604eb59d84354!8m2!3d28.614499!4d77.390908" + ">Google Maps</a></address></html>");
                        }
                        ////Mail to Admin  
                        //string adminEmail = "anuj.chauhan@clanstech.com";
                        //////Insert email details into emailQueue table
                        //emailQueueRepositeryObj.InsertEmailDeatailsInEmailQueue("abhishek.keshari@clanstech.com", adminEmail, "Task", "Dear Admin,<br/> Your Employee have successfully submitted today's task status.<br/>Employee Name:" + Session["UserName"] + "< br/>Title:" + task.Title + "<br/>Description:" + task.Description + "<br>Thanks & Regards.");
                    }
                    else
                    {
                        // Mail to Employee
                        string email = Convert.ToString(Session["UserEmail"]);
                        //Insert email details into emailQueue table
                        _EmailQueueRepositeryObject.InsertEmailDeatailsInEmailQueue("abhishek.keshari@clanstech.com", email, "Task Submitted Successfully", "<html>Dear " + Session["UserName"]+",<br/><br/><br/> You have successfully submitted today's task status.<br/><br/><b>Title: </b>" + task.Title + "<br/><b>Description: </b>" + task.Description + "<br/><br/><br/>Thanks & regards<br/><br/><img width=196 height=153 src="+"http://" + Request.Url.Authority + "/Content/Image/Logo.jpg" + "></b><br/><br><address><b>Adderss:</b><br/>G-282 , Sector-63<br/>   Noida -201301 ,<br/>Gautam Buddh Nagar<br/>U.P, India<br><br><b>Website: </b><a href=" + "https://clanstech.com" + ">www.clanstech.com</a><br/><b>Phone:</b> <phone>+91-7042.615.658</phone><br/><b>Maps: </b><a href=" + "https://www.google.com/maps/place/Clanstech+%7C+Providing+Online+Presence/@28.614499,77.3887193,17z/data=!3m1!4b1!4m5!3m4!1s0x390cef928e84423b:0xde8604eb59d84354!8m2!3d28.614499!4d77.390908" + ">Google Maps</a></address></html>");

                        if (Session["MentorEmail"]!=null)
                        {
                            //Mail to Employee Mentor
                            string mentorEmail = Convert.ToString(Session["MentorEmail"]);
                            //Insert email details into emailQueue table
                            _EmailQueueRepositeryObject.InsertEmailDeatailsInEmailQueue("abhishek.keshari@clanstech.com", mentorEmail, "Trainee Task Update", "<html>Dear Mentor,<br/><br/><br/> Your trainee have successfully submitted today's task status.<br/><br/><b>Trainee Name: </b>" + Session["UserName"] + "<br/><b>Title: </b>" + task.Title + "<br/><b>Description: </b>" + task.Description + "<br/><br/><br/>Thanks & regards<br/><br/><img width=196 height=153 src=" + "http://" + Request.Url.Authority + "/Content/Image/Logo.jpg" + "></b><br/><br><address><b>Adderss:</b><br/>G-282 , Sector-63<br/>   Noida -201301 ,<br/>Gautam Buddh Nagar<br/>U.P, India<br><br><b>Website: </b><a href=" + "https://clanstech.com" + ">www.clanstech.com</a><br/><b>Phone:</b> <phone>+91-7042.615.658</phone><br/><b>Maps: </b><a href=" + "https://www.google.com/maps/place/Clanstech+%7C+Providing+Online+Presence/@28.614499,77.3887193,17z/data=!3m1!4b1!4m5!3m4!1s0x390cef928e84423b:0xde8604eb59d84354!8m2!3d28.614499!4d77.390908" + ">Google Maps</a></address></html>");
                        }
                        ////Mail to Admin
                        ////Insert email details into emailQueue table
                        //string adminEmail = "anuj.chauhan@clanstech.com";
                        //emailQueueRepositeryObj.InsertEmailDeatailsInEmailQueue("abhishek.keshari@clanstech.com", adminEmail, "Task", "Dear Admin,<br/> Your Employee have successfully submitted today's task status.<br/>Employee Name:" + Session["UserName"] + "<br/>Title:" + task.Title + "<br/>Description:" + task.Description + "< br>Thanks & Regards.");
                    }

                    if (Convert.ToInt32(Session["RoleId"]) == 4)
                    {
                        TempData["Message"] = "Task submitted successfullly";
                        return RedirectToAction("Index", "Tasks");
                    }
                    else
                    {
                        TempData["Message"] = "Task submitted successfullly";
                        return RedirectToAction("EmployeeDashboard", "Home");
                    }


                }
            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = Request.Url.ToString();
                _ExceptionLogsRepositeryObject.InsertException(ex, url, site.Name);
            }

            //ViewBag.EmployeeId = new SelectList(db.Users, "Id", "Name", task.EmployeeId);
            return View(task);
        }

        //==========================================Edit task=================================================================
        [EmployeeAuthentication]
        public ActionResult EditTask(int? id)
        {
            Task task = new Task();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                task = _TaskRepositeryObject.GetTaskdetailUsingId(id);

               
                if (task == null)
                {
                    return HttpNotFound();
                }
                DateTime today8pm = DateTime.Today.AddHours(20);
                if (task.CreatedOn > DateTime.Today && task.CreatedOn < today8pm)
                {
                    //get all task attachement
                    attachementList = _AttachmentRepositeryObject.GetAttachmentsByTaskId(task.Id);
                    ViewBag.attachementList = attachementList;
                    return View(task);
                }

                

            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = Request.Url.ToString();
                _ExceptionLogsRepositeryObject.InsertException(ex, url, site.Name);
            }
            //ViewBag.A = new SelectList(db.Role, "Id", "Type", users.RoleId);
            TempData["Message"] = "Time out:Task can't be edited after 8:00 pm of your submission date ";
            return RedirectToAction("EmployeeTask");

        }

        [EmployeeAuthentication]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTask([Bind(Include = "Id,Title,Description,DateTime,EmployeeId,CreatedOn,ModifiedOn,DeletedOn,IsDeleted")] Task task, string[] attchements, HttpPostedFileBase[] files)
        {
            try
            {
                Task EditedTask = _TaskRepositeryObject.GetTaskdetailUsingId(task.Id);
                EditedTask.Title = task.Title;
                EditedTask.Description = task.Description;
                EditedTask.ModifiedOn = DateTime.Now;
                _TaskRepositeryObject.EditTask(EditedTask);

                //delete attachement                 
                if (attchements != null)
                {
                    foreach (string val in attchements)
                    {
                        int id = Convert.ToInt32(val);

                        //read attachement 
                        Attachment attachment = _AttachmentRepositeryObject.GetAttachementById(id);
                        //delete attachement from folder
                        var filePath = Server.MapPath("~/UploadedFile" + attachment.FileName);
                        if (System.IO.File.Exists(attachment.Path))
                        {
                            System.IO.File.Delete(attachment.Path);
                        }
                        //delete attachement from table
                        _AttachmentRepositeryObject.PermanentDeleteAttachementById(id);
                    }
                }

                //Uploade file
                if (files[0] != null)
                {
                    foreach (HttpPostedFileBase file in files)
                    {
                        string fileName = Path.GetFileName(file.FileName);

                        string path = Path.Combine(HostingEnvironment.MapPath("~/UploadedFile"), fileName);

                        string extension = Path.GetExtension(path);
                        file.SaveAs(path);
                        //inserting attachement details into attachment table
                        _AttachmentRepositeryObject.AddAttachment(fileName, path, extension, task.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = Request.Url.ToString();
                _ExceptionLogsRepositeryObject.InsertException(ex, url, site.Name);
            }
            TempData["Message"] = "Task updated Successfully";
            return RedirectToAction("EmployeeTask");
        }
    }
}