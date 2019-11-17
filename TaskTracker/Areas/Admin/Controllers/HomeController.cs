
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
using TaskTracker.Areas.Admin.Authentication;
using Utilities;
using System.Diagnostics;
using Utilities.Models;

namespace TaskTracker.Areas.Admin.Controllers
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
        private MentorRepositery _MentorRepositeryObject = new MentorRepositery();
        private HashPassword _HashPasswordObject = new HashPassword();
        private TimerSettingRepositery _TimerSettingRepositeryObject = new TimerSettingRepositery();


        Role role = new Role();
        List<Role> roleList = new List<Role>();

        Attachment attachment = new Attachment();
        List<Attachment> attachmentList = new List<Attachment>();

        Task task = new Task();
        List<Task> taskList = new List<Task>();

        Users user = new Users();
        List<Users> usersList = new List<Users>();

        Mentor mentor = new Mentor();
        List<Mentor> mentorList = new List<Mentor>();
        List<Mentor> traineeList = new List<Mentor>();

        List<TimerSetting> timerSettingsList = new List<TimerSetting>();


        //=============================================================================================================================
        //=================================================Admin=======================================================================
        //=============================================================================================================================

        // GET: Users==================================================================================================================
        [AdminAuthentication]
        public ActionResult UserIndex()
        {

            var users = _UsersRepositeryObject.GetUsers();
            ViewBag.CreateMessage = TempData["Message"];
            ViewBag.DeleteMessage = TempData["Deletemessage"];
            ViewBag.EditMessage = TempData["EditMessage"];
            return View(users.ToList());
        }

        // GET: Users/Create=============================================================================================================
        [AdminAuthentication]
        public ActionResult CreateUser()
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.RoleId = new SelectList(db.Role, "Id", "Type");
            return View();
        }

        // POST: Users/Create===============================================================================================================
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthentication]
        public ActionResult CreateUser([Bind(Include = "Id,Name,Phone,Email,Password,RoleId,CreatedOn,ModifiedOn,DeletedOn,IsDeleted,EmployeeId")] Users users)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    //validate if user all ready registered
                    Users sucess = _UsersRepositeryObject.GetUserDetailUsingEmailAndEmployeeId(users.Email, users.EmployeeId);
                    if (sucess == null)
                    {

                        users.CreatedOn = DateTime.Now;
                        users.ModifiedOn = DateTime.Now;
                        //To get the hash password 
                        string password = users.Password;
                        string hashPassword = _HashPasswordObject.GetHashPassword(users.Password);
                        users.Password = hashPassword;
                        _UsersRepositeryObject.CreateUser(users);
                        //Send Email to emplyee //With login credentials for task tracker 
                        _EmailQueueRepositeryObject.InsertEmailDeatailsInEmailQueue("abhishek.Keshari@clanstech.com", users.Email, "Task Tracker Login Credentials", "<html>Dear " + users.Name + ",<br/><br/><br/>Your account for Task Tracker has been created. Please submit your task details befor leaving the office.<br/><br/><b>Login Id: </b>" + users.Email + "<br/><b>Password: </b>" + password + "<br/><br/><br/>Thanks & regards<br/><br/><img width=196 height=153 src=" + "http://" + Request.Url.Authority + "/Content/Image/logo.jpg" + "></b><br/><br><address><b>Adderss:</b><br/>G-282 , Sector-63<br/>   Noida -201301 ,<br/>Gautam Buddh Nagar<br/>U.P, India<br><br><b>Website: </b><a href=" + "https://clanstech.com" + ">www.clanstech.com</a><br/><b>Phone:</b> <phone>+91-7042.615.658</phone><br/><b>Maps: </b><a href=" + "https://www.google.com/maps/place/Clanstech+%7C+Providing+Online+Presence/@28.614499,77.3887193,17z/data=!3m1!4b1!4m5!3m4!1s0x390cef928e84423b:0xde8604eb59d84354!8m2!3d28.614499!4d77.390908" + ">Google Maps</a></address></html>");
                        TempData["Message"] = "Employee Created Successfully";
                        return RedirectToAction("UserIndex");

                    }
                    else
                    {
                        TempData["Message"] = "Error : User All ready registered : Try Again";
                        return RedirectToAction("CreateUser");
                    }

                }
            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = Request.Url.ToString();
                _ExceptionLogsRepositeryObject.InsertException(ex, url, site.Name);
            }


            ViewBag.RoleId = new SelectList(db.Role, "Id", "Type", users.RoleId);
            return View(users);
        }

        // GET: Users/Edit/5======================================================================================================================
        [AdminAuthentication]
        public ActionResult EditUser(int? id)
        {
            Users users = new Users();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                users = _UsersRepositeryObject.GetUsersDetailUsingId(id);

                if (users == null)
                {
                    return HttpNotFound();
                }

            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = Request.Url.ToString();
                _ExceptionLogsRepositeryObject.InsertException(ex, url, site.Name);
            }
            ViewBag.RoleId = new SelectList(db.Role, "Id", "Type", users.RoleId);
            return View(users);

        }

        // POST: Users/Edit/5=======================================================================================================================
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthentication]
        public ActionResult EditUser([Bind(Include = "Id,Name,Phone,Email,Password,RoleId,CreatedOn,ModifiedOn,DeletedOn,IsDeleted,EmployeeId")] Users users)
        {
            try
            {
              
                    //check if email id all ready registered 
                    user = _UsersRepositeryObject.GetUserByEmailForEdit(users.Email, users.Id);
                    if (user != null)
                    {
                        TempData["EditMessage"] = "Email Id exist in database:Try other email id to edit user";
                        return RedirectToAction("UserIndex");
                    }

                    //check if Employee id all ready registered 
                    user = _UsersRepositeryObject.GetUserByEmployeeIdForEdit(users.EmployeeId, users.Id);
                    if (user != null)
                    {
                        TempData["EditMessage"] = "Employee Id exist in database:Try other unique Employee id to edit user";
                        return RedirectToAction("UserIndex");
                    }
                    users.ModifiedOn = DateTime.Now;
                    _UsersRepositeryObject.EditUser(users);
                    TempData["EditMessage"] = "Employee details modified successfully";
                    return RedirectToAction("UserIndex");
                
            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = Request.Url.ToString();
                _ExceptionLogsRepositeryObject.InsertException(ex, url, site.Name);
            }
            ViewBag.RoleId = new SelectList(db.Role, "Id", "Type", users.RoleId);
            return View(users);
        }

        // GET: Users/Delete/5===================================================================================================================
        [AdminAuthentication]
        public ActionResult DeleteUser(int? id)
        {
            Users users = new Users();
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                users = _UsersRepositeryObject.GetUsersDetailUsingId(id);
                if (users == null)
                {
                    return HttpNotFound();
                }
            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = Request.Url.ToString();
                _ExceptionLogsRepositeryObject.InsertException(ex, url, site.Name);
            }
            return View(users);
        }

        // GET: Users/Details/5======================================================================================================================
        [AdminAuthentication]
        public ActionResult UserDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = _UsersRepositeryObject.GetUsersDetailUsingId(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: Users/Delete/5======================================================================================================================
        [AdminAuthenticationAttribute]
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUserConfirmed(int id)
        {
            int success = 0;
            Users users = new Users();
            try
            {
                users = _UsersRepositeryObject.GetUsersDetailUsingId(id);
                success = _UsersRepositeryObject.DeleteUser(users);
                if (success == 1)
                {
                    TempData["DeleteMessage"] = "User Deleted Successfully";
                }
            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = Request.Url.ToString();
                _ExceptionLogsRepositeryObject.InsertException(ex, url, site.Name);
            }

            return RedirectToAction("Trash");
        }

        //===============Admin dashboard====================================================================================================================================
        [AdminAuthenticationAttribute]
        public ActionResult AdminDashboard()
        {
            ViewBag.Message = TempData["Message"];
            int id = Convert.ToInt32(Session["UserId"]);
            ViewBag.Admin = _UsersRepositeryObject.GetUsersDetailUsingId(id);
            return View();
        }

        [AdminAuthentication]
        public ActionResult DeleteUserPermanent(int id)
        {
            //get mentor 
            mentorList = _MentorRepositeryObject.GetALLMentorByEmployeeMentorId(id);
            //delete mentor as Traineer
            if (mentorList.Count > 0)
            {
                _MentorRepositeryObject.DeleteMentorPermanent(mentorList);
            }


            //get mentor 
            List<Mentor> mentorListNew = _MentorRepositeryObject.GetAllMentorByEmployeeId(id);
            //delete mentor as Traineer
            if (mentorListNew.Count > 0)
            {
                _MentorRepositeryObject.DeleteMentorPermanent(mentorListNew);
            }

            //get task 
            taskList = _TaskRepositeryObject.GetAllTaskByEmployeeId(id);


            if (taskList.Count > 0)
            {

                foreach (Task task in taskList)
                {
                    //get attachement
                    attachmentList = _AttachmentRepositeryObject.GetAttachmentsByTaskId(task.Id);
                    //Delete All attachement
                    if (attachmentList.Count > 0)
                    {
                        _AttachmentRepositeryObject.PermanentDeleteAttachement(attachmentList);
                    }

                }
                //delete task
                _TaskRepositeryObject.PermanentDeleteTask(taskList);
            }

            //get user
            user = _UsersRepositeryObject.GetUsersDetailUsingId(id);
            //delete user
            if (user != null)
            {
                _UsersRepositeryObject.DeletePermanent(user);
            }


            TempData["Message"] = "Employee Deleted Permanentliy";
            return RedirectToAction("Trash");
        }

        //=============================================================================================================================
        //=================================================Mentors=====================================================================
        //=============================================================================================================================

        // GET: Mentors====================================================================================================================
        [AdminAuthentication]
        public ActionResult MentorIndex()
        {

            //get mentor details            
            var mentor = _MentorRepositeryObject.GetMentorsForIndex();

            //get mentor deatis from users
            ViewBag.usersList = _UsersRepositeryObject.GetUserDetailByMentor(mentor.ToList());

            //get all users
            ViewBag.Users = _UsersRepositeryObject.GetAllUsers();

            return View(mentor.ToList());
        }

        // GET: Mentors/Create================================================================================================================
        [AdminAuthentication]
        public ActionResult CreateMentor()
        {
            ViewBag.Message = TempData["Message"];
            //get all users from user table
            usersList = _UsersRepositeryObject.GetAllUsers();
            //get all mentors and there trainee details
            mentorList = _MentorRepositeryObject.GetAllMentorDetails();
            //remove those users whose mentor is assigned
            usersList.RemoveAll(p => mentorList.Exists(x => p.Id == x.EmployeeId));
            //remove Admin from list
            usersList.RemoveAll(x => x.RoleId == 1);
            ViewBag.trainee = usersList;
            //now get all user detail to assign as mentor
            ViewBag.traineer = _UsersRepositeryObject.GetAllUsers();

            return View();
        }

        // POST: Mentors/Create===============================================================================================================
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthentication]
        public ActionResult CreateMentor([Bind(Include = "Id,EmployeeId,EmployeeMentorId,CreatedOn,ModifiedOn,DeletedOn,IsDeleted")] Mentor mentor)
        {
            if (ModelState.IsValid)
            {
                _MentorRepositeryObject.CreateMentor(mentor);
                return RedirectToAction("MentorIndex");
            }
            else
            {
                TempData["Message"] = "Please Select Mentor and Trainee";
                return RedirectToAction("CreateMentor");
            }

            ViewBag.EmployeeMentorId = new SelectList(db.Users, "Id", "Name", mentor.EmployeeMentorId);
            //ViewBag.EmployeeMentorId = mentorRepositeryObj.EmployeeMentorIdForCreate();
            return View(mentor);
        }

        // GET: Mentors/Edit/5=================================================================================================================
        [AdminAuthentication]
        public ActionResult EditMentor(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            mentor = _MentorRepositeryObject.GetMentorUsingId(id);
            if (mentor == null)
            {
                return HttpNotFound();
            }

            ViewBag.EmployeeMentorId = new SelectList(db.Users.Where(x => x.Id != mentor.EmployeeId && x.IsDeleted == false), "EmployeeId", "Name", "EmployeeId", mentor.EmployeeMentorId, mentor.Users.Name);


            return View(mentor);
        }

        // POST: Mentors/Edit/5===============================================================================================================
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthentication]
        public ActionResult EditMentor([Bind(Include = "Id,EmployeeId,EmployeeMentorId,CreatedOn,ModifiedOn,DeletedOn,IsDeleted")] Mentor mentor)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(mentor).State = EntityState.Modified;
                //db.SaveChanges();
                _MentorRepositeryObject.EditMentor(mentor);
                return RedirectToAction("MentorIndex");
            }
            ViewBag.EmployeeMentorId = new SelectList(db.Users, "Id", "Name", mentor.EmployeeMentorId);
            //ViewBag.EmployeeMentorId = mentorRepositeryObj.EmployeeMentorIdForPostEdit();
            return View(mentor);
        }

        // GET: Mentors/Delete/5==============================================================================================================
        [AdminAuthentication]
        public ActionResult DeleteMentor(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            mentor = _MentorRepositeryObject.GetMentorUsingId(id);
            if (mentor == null)
            {
                return HttpNotFound();
            }
            return View(mentor);
        }

        // POST: Mentors/Delete/5===============================================================================================================
        [AdminAuthentication]
        [HttpPost, ActionName("DeleteMentor")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMentorConfirmed(int id)
        {
            mentor = _MentorRepositeryObject.GetMentorUsingId(id);
            _MentorRepositeryObject.DeleteMentor(mentor);
            return RedirectToAction("MentorIndex");
        }

        //=============================================================================================================================
        //=================================================Roles=======================================================================
        //=============================================================================================================================

        // GET: Roles==================================================================================================================
        [AdminAuthentication]
        public ActionResult RoleIndex()
        {
            roleList = _RoleRepositeryObject.GetAllRolesDetail();
            return View(roleList);
        }

        // GET: Roles/Details/5=========================================================================================================
        [AdminAuthentication]
        public ActionResult RoleDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            role = _RoleRepositeryObject.GetRoleDetailUsingRoleId(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        // GET: Roles/Create=============================================================================================================
        [AdminAuthentication]
        public ActionResult CreateRole()
        {
            return View();
        }

        // POST: Roles/Create=============================================================================================================
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthentication]
        public ActionResult CreateRole([Bind(Include = "Id,Type,CreatedOn,ModifiedOn,DeletedOn,IsDeleted")] Role role)
        {
            if (ModelState.IsValid)
            {
                //db.Role.Add(role);
                //db.SaveChanges();
                _RoleRepositeryObject.CreateRole(role);
                return RedirectToAction("RoleIndex");
            }

            return View(role);
        }

        // GET: Roles/Edit/5===============================================================================================================
        [AdminAuthentication]
        public ActionResult EditRole(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            role = _RoleRepositeryObject.GetRoleDetailUsingRoleId(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        // POST: Roles/Edit/5==============================================================================================================
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthentication]
        public ActionResult EditRole([Bind(Include = "Id,Type,CreatedOn,ModifiedOn,DeletedOn,IsDeleted")] Role role)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(role).State = EntityState.Modified;
                //db.SaveChanges();
                _RoleRepositeryObject.EditRole(role);
                return RedirectToAction("RoleIndex");
            }
            return View(role);
        }

        // GET: Roles/Delete/5=-================================================================================================================
        [AdminAuthentication]
        public ActionResult DeleteRole(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            role = _RoleRepositeryObject.GetRoleDetailUsingRoleId(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        // POST: Roles/Delete/5=================================================================================================================
        [AdminAuthentication]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRoleConfirmed(int id)
        {
            role = _RoleRepositeryObject.GetRoleDetailUsingRoleId(id);
            _RoleRepositeryObject.DeleteRole(role);
            return RedirectToAction("RoleIndex");
        }

        //=============================================================================================================================
        //=================================================Task========================================================================
        //=============================================================================================================================

        // GET: Tasks==================================================================================================================
        [AdminAuthentication]
        public ActionResult TaskIndex(int? page)
        {
            var task = _TaskRepositeryObject.GetTaskForIndex();
            attachmentList = _AttachmentRepositeryObject.GetAllAttachments();
            ViewBag.attachement = attachmentList.ToList();
            ViewBag.Message = TempData["Message"];
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(task.ToList().ToPagedList(pageNumber, pageSize));
        }

        [AdminAuthentication]
        [HttpGet]
        public ActionResult AllEmployeesTask(int? page)
        {
            //Get all employee details
            usersList = _UsersRepositeryObject.GetAllUsers();

            //Remove Admin from list
            usersList.RemoveAll(x => x.RoleId == 1);
            ViewBag.Message = TempData["Message"];
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(usersList.ToList().ToPagedList(pageNumber, pageSize));
        }
        [AdminAuthentication]
        public ActionResult EmployeeTask(int id, int? page)
        {
            //Get all Task by employee Id
            taskList = _TaskRepositeryObject.GetAllTaskByEmployeeId(id);

            //Get all Attachment
            attachmentList = _AttachmentRepositeryObject.GetAllAttachments();
            ViewBag.attachement = attachmentList.ToList();
            ViewBag.attachementCount = attachmentList.Count;

            //Get Employee Detail by id
            ViewBag.EmployeeDetails = _UsersRepositeryObject.GetUsersDetailUsingId(id);
            ViewBag.Message = TempData["Message"];
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(taskList.ToPagedList(pageNumber, pageSize));
        }

        [AdminAuthentication]
        public ActionResult TaskDetails(int id)
        {
            //Get task by task id
            ViewBag.task = _TaskRepositeryObject.GetTaskdetailUsingId(id);

            //Get task attachement by task id
            ViewBag.attachement = _AttachmentRepositeryObject.GetAttachmentsByTaskId(id);

            return View();
        }
        //=============================================Trash===============================================================================
        //Get: //for trash to show the detail of deleted data
        [AdminAuthentication]
        public ActionResult Trash()
        {
            ViewBag.Message = TempData["Message"];
            //deleted users detail
            usersList = _UsersRepositeryObject.GetDeletedUsers();

            return View(usersList);
        }

        //Get: //restore Users
        [AdminAuthentication]
        public ActionResult RestoreUser(int id)
        {
            _UsersRepositeryObject.MakeIsdeletedFalse(id);
            TempData["Message"] = "Employee Restored Successfully";
            return RedirectToAction("Trash");
        }
        //Method for dropdown to add mentor
        public string Remove_Trainee(int id)
        {
            List<Users> ListOfUsers = _UsersRepositeryObject.GetAllUsers();
            ListOfUsers.RemoveAll(x => x.Id == id);
            List<Mentor> ListOfTrainee = _MentorRepositeryObject.ReadMentor_ByMentorId(id);

            ListOfUsers.RemoveAll(x => ListOfTrainee.Exists(y => y.EmployeeId == x.Id));
            //ListOfTrainee.RemoveAll(x=>x.EmployeeMentorId==id);

            //ListOfUsers.RemoveAll(x=>ListOfTrainee.Exists(y=>y.EmployeeId==x.Id));

            string html = "<option value=''>Please Select</option>";
            foreach (Users temp in ListOfUsers)
            {
                html += "<option value=" + temp.Id + ">" + temp.Name + "(" + temp.EmployeeId + ")" + "</option>";
            }
            return html;
        }

        //======================================================================Exception Logs======================================================================================
        [AdminAuthentication]
        public ActionResult ExceptionLogsDetail(int? page)
        {
            //Get all exceptions
            List<Utilities.ExceptionLogs> exceptionLogsList = new List<Utilities.ExceptionLogs>();
            ExceptionLogsRepositery _ExceptionLogsObject = new ExceptionLogsRepositery();

            exceptionLogsList = _ExceptionLogsRepositeryObject.GetAllExceptions();
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            exceptionLogsList.OrderByDescending(x => x.CreatedOn);
            return View(exceptionLogsList.ToList().ToPagedList(pageNumber, pageSize));

        }
        //======================================================================Email Logs======================================================================================
        [AdminAuthentication]
        public ActionResult EmailLogsDetail(int? page)
        {

            List<EmailLogs> emailLogsList = new List<EmailLogs>();
            EmailLogsRepositery _EmailLogsRepositeryObject = new EmailLogsRepositery();

            emailLogsList = _EmailLogsRepositeryObject.GetAllEmail();
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            emailLogsList.OrderByDescending(x => x.CreatedOn);
            return View(emailLogsList.ToList().ToPagedList(pageNumber, pageSize));

        }

        //======================================================================Email Queue ======================================================================================
        [AdminAuthentication]
        public ActionResult EmailQueueDetail(int? page)
        {

            List<EmailQueue> emailQueueList = new List<EmailQueue>();
            EmailQueueRepositery _EmailQueueRepositeryObject = new EmailQueueRepositery();

            emailQueueList = _EmailQueueRepositeryObject.GetEamilQueueDetail();
            int pageSize = 5;
            int pageNumber = (page ?? 1);


            emailQueueList.OrderByDescending(x => x.CreatedOn);
            return View(emailQueueList.ToList().ToPagedList(pageNumber, pageSize));

        }

        //======================================================================Email Timer======================================================================================
        [AdminAuthentication]
        public ActionResult TimerDetails()
        {
            timerSettingsList = _TimerSettingRepositeryObject.ReadAllTimerSetting();
            return View(timerSettingsList);
        }

        [AdminAuthentication]
        [HttpGet]
        public ActionResult EditTimer(int? id)
        {
            //_TimerSettingRepositeryObject.UpdateTimerSetting()
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TimerSetting timerSetting = _TimerSettingRepositeryObject.GetTimerById(id);
            if (timerSetting == null)
            {
                return HttpNotFound();
            }

            return View(timerSetting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminAuthentication]
        public ActionResult EditTimer([Bind(Include = "Id,StartTime,EndTime,CreatedOn,ModifiedOn,DeletedOn,IsDeleted")] TimerSetting timerSetting)
        {
            if (ModelState.IsValid)
            {

                _TimerSettingRepositeryObject.EditTimer(timerSetting);
                return RedirectToAction("TimerDetails");
            }
            //ViewBag.EmployeeMentorId = new SelectList(db.Users, "Id", "Name", mentor.EmployeeMentorId);
            //ViewBag.EmployeeMentorId = mentorRepositeryObj.EmployeeMentorIdForPostEdit();
            return View(timerSetting);
        }

        //======================================================Console Application==================================================================================================
        [HttpGet]
        [AdminAuthentication]
        public ActionResult EmailServiceStart(object sender, System.EventArgs e)
        {
            try
            {
                //Check if Email service is running
                foreach (Process clsProcess in Process.GetProcesses())
                {
                    if (clsProcess.ProcessName.StartsWith("EmailService"))
                    {
                        string temp = clsProcess.StartInfo.ToString();
                        if (temp != null)
                        {
                            TempData["Message"] = "Email Service is Allready Running";
                            return RedirectToAction("AdminDashboard");
                        }
                    }
                }
                using (Process p = new Process())
                {

                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.FileName = Server.MapPath(@"\EmailService\\EmailService.exe");
                    p.StartInfo.CreateNoWindow = false;
                    p.Start();
                    TempData["Message"] = "Email Service Started";
                    return RedirectToAction("AdminDashboard");
                }
            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = Request.Url.ToString();
                _ExceptionLogsRepositeryObject.InsertException(ex, url, site.Name);
                TempData["Message"] = "Exception occure in Starting Email Service ";
                return RedirectToAction("AdminDashboard");
            }
        }

        [HttpGet]
        [AdminAuthentication]
        public ActionResult EmailServiceStop(object sender, System.EventArgs e)
        {
            try
            {
                foreach (Process clsProcess in Process.GetProcesses())
                {
                    if (clsProcess.ProcessName.StartsWith("EmailService"))
                    {
                        clsProcess.Kill();
                        TempData["Message"] = "Email Service stoped";
                        return RedirectToAction("AdminDashboard");
                    }
                }


            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = Request.Url.ToString();
                _ExceptionLogsRepositeryObject.InsertException(ex, url, site.Name);
                TempData["Message"] = "Exception occure in Email Service stoping";
                return RedirectToAction("AdminDashboard");
            }
            TempData["Message"] = "Email Service not running";
            return RedirectToAction("AdminDashboard");
        }

        [HttpGet]
        [AdminAuthentication]
        public ActionResult EmailServiceStatus(object sender, System.EventArgs e)
        {
            try
            {
                foreach (Process clsProcess in Process.GetProcesses())
                {
                    if (clsProcess.ProcessName.StartsWith("EmailService"))
                    {
                        string temp = clsProcess.StartInfo.ToString();
                        if (temp != null)
                        {
                            TempData["Message"] = "Email Service is Running";
                            return RedirectToAction("AdminDashboard");
                        }
                        else
                        {
                            TempData["Message"] = "Email Service is not Running";
                            return RedirectToAction("AdminDashboard");
                        }

                    }
                }


            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = Request.Url.ToString();
                _ExceptionLogsRepositeryObject.InsertException(ex, url, site.Name);
                TempData["Message"] = "Exception occure Checking Email Service Status";
                return RedirectToAction("AdminDashboard");
            }
            TempData["Message"] = "Email Service Status not Running";
            return RedirectToAction("AdminDashboard");
        }

        //Today employees task status 
        [AdminAuthentication]
        public ActionResult TodayStatus()
        {
            //Read all employee
            List<Users> taskSubmittedUsersList = _UsersRepositeryObject.GetAllEmployee();
            List<Users> taskNotSubmittedUsersList = _UsersRepositeryObject.GetAllEmployee();

            //Get today task
            taskList = _TaskRepositeryObject.GetTaskDetailOfTodaySubmittedTask();
            ViewBag.taskList = taskList;
            //Employee who are submitted there task
            if (taskList.Count > 0)
            {
                taskSubmittedUsersList.RemoveAll(x => !taskList.Exists(i => i.EmployeeId == x.Id));
                ViewBag.taskSubmittedUsersList = taskSubmittedUsersList;
            }


            //Employee who are not submitted there task
            taskNotSubmittedUsersList.RemoveAll(x => taskList.Exists(i => i.EmployeeId == x.Id));
            ViewBag.taskNotSubmittedUsersList = taskNotSubmittedUsersList;
            return View(usersList);
        }

        //Task Details by Date
        [AdminAuthentication]
        [HttpGet]
        public ActionResult TaskStatusByDate()
        {
            DateTime start = (DateTime.Now);
            DateTime end = DateTime.Now;
            // = default(DateTime)=default(DateTime)
            if (start == end)
            {
                end = end.AddDays(-5);
            }

            //get Employee
            usersList = _UsersRepositeryObject.GetAllEmployee();
            ViewBag.usersList = usersList;
            //get users task by date 
            taskList = _TaskRepositeryObject.GetTaskByDate(start, end);
            ViewBag.taskList = taskList;
            ViewBag.start = start;
            ViewBag.end = end;

            return View(usersList);
        }

        //search task by date
        [AdminAuthentication]
        //[AcceptVerbs("Get", "Post")]
        [HttpPost]
        public ActionResult TaskStatusByDate(DateTime start, DateTime end)
        {



            //get Employee
            usersList = _UsersRepositeryObject.GetAllEmployee();

            //get users task by date 
            taskList = _TaskRepositeryObject.GetTaskByDate(start, end);
            ViewBag.taskList = taskList;
            ViewBag.start = start;
            ViewBag.end = end;

            return View(usersList);
        }


    }
}