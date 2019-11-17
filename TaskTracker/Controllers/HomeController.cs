using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using TaskTracker.Models;
using Utilities;
using Utilities.Models;

namespace TaskTracker.Controllers
{
    [OutputCache(NoStore = true, Duration = 0)]
    public class HomeController : Controller
    {

        private Email _EmailObject = new Email();
        HashPassword _HashPasswordObject = new HashPassword();
        UsersRepositery _UsersRepositeryObject = new UsersRepositery();
        EmailLogsRepositery _EmailLogsRepositeryObject = new EmailLogsRepositery();
        EmailQueueRepositery _EmailQueueRepositeryObject = new EmailQueueRepositery();
        ExceptionLogsRepositery _ExceptionLogsRepositeryObject = new ExceptionLogsRepositery();
        RoleRepositery _RoleRepositeryObject = new RoleRepositery();
        MentorRepositery _MentorRepositeryObject = new MentorRepositery();

        Role role = new Role();

        Users user = new Users();
        List<Users> usersList = new List<Users>();

        List<EmailLogs> emailLogsList = new List<EmailLogs>();

        EmailQueue item = new EmailQueue();
        List<EmailQueue> emailQueueList = new List<EmailQueue>(); 

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //To send the email from email queue data
        public static void SendEmailFromQueue()
        {
            EmailQueueRepositery _EmailQueueRepositeryObject = new EmailQueueRepositery();
            EmailLogsRepositery _EmailLogsRepositeryObject = new EmailLogsRepositery();
            ExceptionLogsRepositery _ExceptionLogsRepositeryObject = new ExceptionLogsRepositery();
            Email _EmailObject = new Email();
            EmailQueue item = new EmailQueue();
            string hostname = Dns.GetHostName();
            IPAddress[] iPAddress = Dns.GetHostAddresses(hostname);
            string ip = iPAddress[1].ToString();
            int success = 0;

            try
            {

                //read emails details from emailqueue             
                item = _EmailQueueRepositeryObject.GetTopEmailQueueDetail();

                if (item != null)    //emailQueueList.Count() != 0
                {

                    //set deleted is 1 in emailqueue                    
                    //_EmailQueueRepositeryObject.UpdateIsDeletedTrue(item);

                    //read tries from EmailQueue table
                    //int tries = _EmailQueueRepositeryObject.GetTries(item);
                    int tries = 0;

                    while (tries <= 5)
                    {
                        //send email 
                        success = _EmailObject.SendEmail(item.FromAddress, item.ToAddress, item.Subject, item.Body);

                        if (success == 1)
                        {

                            //insert email details into emaillogs status as success
                            _EmailLogsRepositeryObject.InsertEmaillogs(item.FromAddress, item.ToAddress, item.Subject, item.Body, "mail.clanstech.com", ip, "Success");
                            //delete email detail from email queue
                            _EmailQueueRepositeryObject.DeletedFromEmailQueue(item);
                            break;
                        }
                        else
                        {
                            //set deleted is 0 in emailqueue
                            //_EmailQueueRepositeryObject.UpdateIsDeletedFalse(item);
                            //increment tries into emailqueue 
                            _EmailQueueRepositeryObject.IncrementTries(item);
                        }
                        //read tries from EmailQueue table
                        tries = _EmailQueueRepositeryObject.GetTries(item);
                    }
                    if (tries > 6)
                    {
                        //insert email details into emaillogs status as fail
                        _EmailLogsRepositeryObject.InsertEmaillogs(item.FromAddress, item.ToAddress, item.Subject, item.Body, "mail.clanstech.com", ip, "Fail");
                        //delete email detail from email queue
                        _EmailQueueRepositeryObject.DeletedFromEmailQueue(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = "";// Request.Url.ToString();
                _ExceptionLogsRepositeryObject.InsertException(ex, url, site.Name);
            }

        }

        
        //============================================================================================================================================
        //===================================================Users====================================================================================
        //============================================================================================================================================

        //==============Login==============================================================================================================================================
        [HttpGet]
        public ActionResult Login()
        {
            if (Session["UserName"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Message = TempData["Message"];
            return View("Login");
        }
        [HttpPost]
        public ActionResult Login(Login login)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    string email = login.Email;
                    Users us = _UsersRepositeryObject.GetUserDetailUsingEmail(email);//email
                    if (us == null || us.IsDeleted == true)
                    {
                        TempData["Message"] = "Your account deleted or does not exist:Please contact to admin";
                        return RedirectToAction("Login");
                    }
                    //hashing of password
                    login.Password = _HashPasswordObject.GetHashPassword(login.Password);

                    if (login.Email == us.Email && login.Password == us.Password)
                    {
                        role = _RoleRepositeryObject.GetRoleDetailUsingRoleId(us.RoleId);

                        if (role.Type == "Admin")//need to change after project completion and all table truncate
                        {
                            Session["AdminId"] = us.Id;
                            Session["UserId"] = us.Id;
                            Session["UserName"] = us.Name;
                            Session["UserEmail"] = us.Email;
                            Session["RoleId"] = us.RoleId;

                            return RedirectToAction("AdminDashboard", "Home", new { area = "Admin" });
                        }
                        else
                        {
                            Session["UserId"] = us.Id;
                            Session["UserName"] = us.Name;
                            Session["UserEmail"] = us.Email;
                            Session["RoleId"] = us.RoleId;

                            Mentor mentor = _MentorRepositeryObject.GetMentorDetailsUsingEmployeeId(us.Id);

                            if (mentor != null)
                            {
                                Users user1 = _UsersRepositeryObject.GetUsersDetailUsingId(mentor.EmployeeMentorId);
                                Session["MentorEmail"] = user1.Email;
                            }



                            return RedirectToAction("EmployeeDashboard", "Home", new { area = "Employee" });
                        }

                    }
                    else
                    {
                        TempData["Message"] = "Invalid Credentials:Please Try Again";
                        return RedirectToAction("Login");
                    }
                }
            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = Request.Url.ToString();
                _ExceptionLogsRepositeryObject.InsertException(ex, url, site.Name);
            }

            return View("Login");
        }


        //=================Logout=======================================================================================================================
        public ActionResult LogOut()
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            return RedirectToAction("Login");
        }
        //============================================Forgot & Reset Password=================================================================================
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            ViewBag.Message = TempData["Message"];
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(Login login)
        {
            Guid guid = Guid.NewGuid();
            user = _UsersRepositeryObject.GetUserDetailUsingEmail(login.Email);
            if(user==null)
            {
                TempData["Message"] = "Invalid Email Id: Resend password reset link";
                return RedirectToAction("ForgotPassword");
            }
            string from = "abhishek.keshari@clanstech.com";
            string to = login.Email;
            string subject = "Password Reset";
            string body = string.Format("<html>Dear User,<br/><br/><br/> please click the following link to reset your password.<br/><br/> <a href = http://" + Request.Url.Host + ":" + Request.Url.Port + Url.Action("ResetPassword", "Home", new { id = user.Password }) + ">" + "Click Here to Reset" + "</a><br/><br/>Thanks & regards<br/><br/><img width=196 height=153 src=" + "https://" + Request.Url.Authority + "/Content/Image/Logo.jpg" + "></b><br/><br/><address><b>Adderss:</b><br/>G-282 , Sector-63<br/>   Noida -201301 ,<br/>Gautam Buddh Nagar<br/>U.P, India<br><br><b>Website: </b><a href=" + "https://clanstech.com" + ">www.clanstech.com</a><br/><b>Phone:</b> <phone>+91-7042.615.658</phone><br/><b>Maps: </b><a href=" + "https://www.google.com/maps/place/Clanstech+%7C+Providing+Online+Presence/@28.614499,77.3887193,17z/data=!3m1!4b1!4m5!3m4!1s0x390cef928e84423b:0xde8604eb59d84354!8m2!3d28.614499!4d77.390908" + ">Google Maps</a></address></html>");
            //_EmailObject.SendEmail(from, to, subject, body);
            _EmailQueueRepositeryObject.InsertEmailDeatailsInEmailQueue(from, to, subject, body);
            TempData["Message"] = "Password reset link has been successfully send to your Email Id";
            return RedirectToAction("ForgotPassword");
        }
        [HttpGet]
        public ActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ResetPassword(string id, string email, string password)
        {
            user = _UsersRepositeryObject.GetUserDetailUsingEmail(email);
            if(user==null)
            {
                TempData["Message"] = "Invalid Credentials: Resend password reset link";
                return RedirectToAction("ForgotPassword");
            }
            if (id == user.Password)
            {
                password = _HashPasswordObject.GetHashPassword(password);
                _UsersRepositeryObject.UpdatePassword(email, password);
            }
            else
            {
                TempData["Message"] = "Invalid Credentials: Resend password reset link";
                return RedirectToAction("ForgotPassword");
            }
            TempData["message"] = "Password has been successfully changed: Use your new Credentials to login";
            return RedirectToAction("Login");
        }


    }
}
