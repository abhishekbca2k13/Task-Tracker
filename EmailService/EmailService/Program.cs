using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using TaskTracker;
using TaskTracker.Models;
using Utilities;
using Utilities.Models;

namespace EmailService
{
    class Program
    {

        static class ReminderState
        {
            public static bool FirstReminder = false;
            public static bool SecondReminder = false;
            public static bool ThirdReminder = false;
            public static DateTime date = System.DateTime.Now.ToLocalTime();
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Email Service Application is running.....");
            //for send mail from email queue
            System.Timers.Timer emailService = new System.Timers.Timer();
            emailService.Start();
            emailService.Interval = 10000;
            emailService.Elapsed += emailService_elapsed;
            //emailService.AutoReset = false;

            //for email alert email
            System.Timers.Timer emailAlertService = new System.Timers.Timer();
            emailAlertService.Start();
            emailAlertService.Interval = 6000;
                                                 //Skip sunday
            if (DateTime.Today.DayOfWeek.ToString() != "Sunday")
            {
                emailAlertService.Elapsed += emailAlertService_elapsed;
            }
                
            emailAlertService.AutoReset = true;

            Console.ReadLine();
        }

        /// <summary>
        /// To send mail from email queue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void emailService_elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Send Email
            Console.WriteLine("Sending Email from Email Queue..... ");
            try
            {
                SendEmailFromQueue();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// To send first reminder email
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void emailAlertService_elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if(ReminderState.date.Date<System.DateTime.Now.ToLocalTime().Date)
            {
                ReminderState.date = System.DateTime.Now.ToLocalTime();
                ReminderState.FirstReminder = false;
                ReminderState.SecondReminder = false;
                ReminderState.ThirdReminder = false;
            }
            TaskTrackerEntities db = new TaskTrackerEntities();

            TimerSetting timerSettingFirstReminder = new TimerSetting();
            TimerSetting timerSettingSecondReminder = new TimerSetting();
            TimerSetting timerSettingThirdReminder = new TimerSetting();
            TimerSetting timerSettingWarning = new TimerSetting();

            DateTime dt = DateTime.Now;
            TimeSpan time = dt.TimeOfDay;
            //Send First Reminder Mail
            //TimeSpan FRStart = new TimeSpan(17, 34, 0);
            //TimeSpan FREnd = new TimeSpan(17, 35, 0);
            timerSettingFirstReminder = db.TimerSetting.Where(x => x.Id == 1).FirstOrDefault();
            TimeSpan FRStart = timerSettingFirstReminder.StartTime;
            TimeSpan FREnd = timerSettingFirstReminder.EndTime;
            if (time >= FRStart && time <= FREnd && ReminderState.FirstReminder==false)
            {
                Console.WriteLine("Inserting first reminder Email in Email Queue");
                SendReminderEmail();
                ReminderState.FirstReminder = true;
            }


            //Send Second Reminder Mail
            //TimeSpan SRStart = new TimeSpan(18, 30, 0);
            //TimeSpan SREnd = new TimeSpan(18, 35, 0);
            timerSettingSecondReminder = db.TimerSetting.Where(x => x.Id == 2).FirstOrDefault();
            TimeSpan SRStart = timerSettingSecondReminder.StartTime;
            TimeSpan SREnd = timerSettingSecondReminder.EndTime;
            if (time >= SRStart && time <= SREnd && ReminderState.SecondReminder == false)
            {
                Console.WriteLine("Inserting second reminder Email in Email Queue");
                SendReminderEmail();
                ReminderState.SecondReminder = true;
            }

            //Send third Reminder Mail
            //TimeSpan TRStart = new TimeSpan(19, 30, 0);
            //TimeSpan TREnd = new TimeSpan(19, 35, 0);
            timerSettingThirdReminder = db.TimerSetting.Where(x => x.Id == 3).FirstOrDefault();
            TimeSpan TRStart = timerSettingThirdReminder.StartTime;
            TimeSpan TREnd = timerSettingThirdReminder.EndTime;
            if (time >= SRStart && time <= SREnd && ReminderState.ThirdReminder == false)
            {
                Console.WriteLine("Inserting third reminder Email in Email Queue");
                SendReminderEmail();
                ReminderState.ThirdReminder = true;
            }

            //Send Warning Mail
            //TimeSpan WStart = new TimeSpan(20, 00, 0);
            //TimeSpan WEnd = new TimeSpan(20, 5, 0);
            timerSettingWarning = db.TimerSetting.Where(x => x.Id == 4).FirstOrDefault();
            TimeSpan WStart = timerSettingWarning.StartTime;
            TimeSpan WEnd = timerSettingWarning.EndTime;
            if (time >= WStart && time <= WEnd)
            {
                Console.WriteLine("Inserting warning  Email in Email Queue");
                SendWarningEmail();
            }

        }


        //======================================================================================To send the email from email queue data=====================================================================
        /// <summary>
        /// To send the email from email queue data
        /// </summary>
        public static void SendEmailFromQueue()
        {
            Console.WriteLine("SendEmailFromQueue()");
            try
            {
                EmailQueueRepositery _EmailQueueRepositeryObject = new EmailQueueRepositery();
                EmailLogsRepositery _EmailLogsRepositeryObject = new EmailLogsRepositery();
                Email _EmailObject = new Email();
                EmailQueue item = new EmailQueue();
                string hostname = Dns.GetHostName();
                IPAddress[] iPAddress = Dns.GetHostAddresses(hostname);
                string ip = iPAddress[1].ToString();
                int success = 0;


                Console.WriteLine("Send email from email queue is executing...");
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
                            Console.WriteLine("Email sent successfull to : " + item.ToAddress);
                            break;
                        }
                        else
                        {
                            //set deleted is 0 in emailqueue
                            //_EmailQueueRepositeryObject.UpdateIsDeletedFalse(item);
                            //increment tries into emailqueue 
                            _EmailQueueRepositeryObject.IncrementTries(item);
                            Console.WriteLine("Email sending fail to : " + item.ToAddress);
                        }
                        //read tries from EmailQueue table
                        tries = _EmailQueueRepositeryObject.GetTries(item);
                        Console.WriteLine("Incrementing try");
                    }
                    if (tries >= 6)
                    {
                        //insert email details into emaillogs status as fail
                        _EmailLogsRepositeryObject.InsertEmaillogs(item.FromAddress, item.ToAddress, item.Subject, item.Body, "mail.clanstech.com", ip, "Fail");
                        //delete email detail from email queue
                        _EmailQueueRepositeryObject.DeletedFromEmailQueue(item);
                        Console.WriteLine("Email sending  fail to : " + item.ToAddress);
                    }
                }
                else
                {
                    Console.WriteLine("Email Queue is empty...");
                }
                
            }
            catch (Exception ex)
            {
                try
                {
                    ExceptionLogsRepositery _ExceptionLogsRepositeryObject = new ExceptionLogsRepositery();
                    MethodBase site = MethodBase.GetCurrentMethod();
                    string url = "";// Request.Url.ToString();
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

        }

        //===================================================Method to Send Reminder Email //Insert reminder email details in email Queue  =========================================================================
        /// <summary>
        /// Method to Send Reminder Email //Insert reminder email details in email Queue
        /// </summary>
        public static void SendReminderEmail()
        {
            ExceptionLogsRepositery exceptionLogsRepositery = new ExceptionLogsRepositery();
            UsersRepositery _UsersRepositeryObject = new UsersRepositery();
            EmailQueueRepositery _EmailQueueRepositeryObject = new EmailQueueRepositery();
            EmailLogsRepositery _EmailLogsRepositeryObject = new EmailLogsRepositery();
            TaskRepositery _TaskRepositertyObject = new TaskRepositery();

            List<Users> usersList = new List<Users>();
            List<EmailLogs> emailLogsList = new List<EmailLogs>();
            List<Task> taskList = new List<Task>();
            List<EmailLogs> emailLogsListReminder = new List<EmailLogs>();
            string hostname = Dns.GetHostName();

            try
            {
                //get all users detail int usersList
                usersList = _UsersRepositeryObject.GetAllUsers();

                //Remove admin details
                usersList.RemoveAll(x => x.RoleId == 1);

                //get detail of user who have submitted there task today from task table
                taskList = _TaskRepositertyObject.GetTaskDetailOfTodaySubmittedTask();

                //list of user who have not submitted there task today //remove the user who have submitted there task
                usersList.RemoveAll(x => taskList.Exists(i => x.Id == i.EmployeeId));

                string logo = "ftp://demo8@clanstech.com/Content/Image/Logo.jpg";
                if (usersList.Count > 0)
                {
                    //insert email detail into email Queue
                    foreach (Users item in usersList)
                    {
                        _EmailQueueRepositeryObject.InsertEmailDeatailsInEmailQueue("abhishek.Keshari@clanstech.com", item.Email, "Task Reminder", "<html>Dear Employee,<br/><br/><br/>You have not submitted your today's task detail. Don't forget to submit your task.<br/><br/><b>Thanks & regards<br/><br/><img width=196 height=153 src=" + "http://demo8.clanstech.com/Content/Image/logo.jpg" + "><br/><br/></b><br/><br><address><b>Adderss:</b><br/>G-282 , Sector-63<br/>   Noida -201301 ,<br/>Gautam Buddh Nagar<br/>U.P, India<br><br><b>Website: </b><a href=" + "https://clanstech.com" + ">www.clanstech.com</a><br/><b>Phone:</b> <phone>+91-7042.615.658</phone><br/><b>Maps: </b><a href=" + "https://www.google.com/maps/place/Clanstech+%7C+Providing+Online+Presence/@28.614499,77.3887193,17z/data=!3m1!4b1!4m5!3m4!1s0x390cef928e84423b:0xde8604eb59d84354!8m2!3d28.614499!4d77.390908" + ">Google Maps</a></address></html>");
                        Console.WriteLine("Email details inserting to Email Queue : " + item.Email);
                    }
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                MethodBase site = MethodBase.GetCurrentMethod();
                //HttpRequest request = HttpContext.Current.Request;
                string url = "";// request.Url.AbsoluteUri;
                Console.WriteLine("Exception occurred in inserting into email queue for reminder");
                Console.WriteLine("Message : " + ex.Message);
                Console.WriteLine("StackTrace : " + ex.StackTrace);
                Console.WriteLine("Data : " + ex.Data);
                Console.WriteLine("Source : " + ex.Source);
                Console.WriteLine("Method" + site.Name.ToString());
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }
        }

        //========================================================================================Send warning Email ===================================================================================================
        /// <summary>
        /// Send warning Email
        /// </summary>
        public static void SendWarningEmail()
        {
            ExceptionLogsRepositery exceptionLogsRepositery = new ExceptionLogsRepositery();
            UsersRepositery _UsersRepositeryObject = new UsersRepositery();
            EmailQueueRepositery _EmailQueueRepositeryObject = new EmailQueueRepositery();
            EmailLogsRepositery _EmailLogsRepositeryObject = new EmailLogsRepositery();
            TaskRepositery _TaskRepositertyObject = new TaskRepositery();

            List<Users> usersList = new List<Users>();
            List<EmailLogs> emailLogsList = new List<EmailLogs>();
            List<Task> taskList = new List<Task>();
            List<EmailLogs> emailLogsListReminder = new List<EmailLogs>();
            string hostname = Dns.GetHostName();

            try
            {

                //get all users detail int usersList
                usersList = _UsersRepositeryObject.GetAllUsers();

                //Remove admin details
                usersList.RemoveAll(x => x.RoleId == 1);

                //get detail of user who have submitted there task today from task table
                taskList = _TaskRepositertyObject.GetTaskDetailOfTodaySubmittedTask();

                //list of user who have not submitted there task today //remove the user who have submitted there task
                usersList.RemoveAll(x => taskList.Exists(i => x.Id == i.EmployeeId));

                if (usersList.Count > 0)
                {
                    //insert email detail into email Queue
                    foreach (Users item in usersList)
                    {
                        _EmailQueueRepositeryObject.InsertEmailDeatailsInEmailQueue("abhishek.Keshari@clanstech.com", item.Email, "Regarding Task Warning", "<html>Dear Employee,<br/><br/><br/>This is warning email.You have not submitted your today's task detail.<br/><br/><br/><b>Thanks & regards</b><br/><br/><img width=196 height=153 src=" + "http://demo8.clanstech.com/Content/Image/logo.jpg" + "><br/><br/><br/><address><b>Adderss:</b><br/>G-282 , Sector-63<br/>   Noida -201301 ,<br/>Gautam Buddh Nagar<br/>U.P, India<br><br><b>Website: </b><a href=" + "https://clanstech.com" + ">www.clanstech.com</a><br/><b>Phone:</b> <phone>+91-7042.615.658</phone><br/><b>Maps: </b><a href=" + "https://www.google.com/maps/place/Clanstech+%7C+Providing+Online+Presence/@28.614499,77.3887193,17z/data=!3m1!4b1!4m5!3m4!1s0x390cef928e84423b:0xde8604eb59d84354!8m2!3d28.614499!4d77.390908" + ">Google Maps</a></address></html>");
                        Console.WriteLine("Email details inserting to Email Queue : " + item.Email);
                    }
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                MethodBase site = MethodBase.GetCurrentMethod();
                //HttpRequest request = HttpContext.Current.Request;
                string url = "";// request.Url.AbsoluteUri;
                Console.WriteLine("Exception occurred in inserting into email queue for warning");
                Console.WriteLine("Message : " + ex.Message);
                Console.WriteLine("StackTrace : " + ex.StackTrace);
                Console.WriteLine("Data : " + ex.Data);
                Console.WriteLine("Source : " + ex.Source);
                Console.WriteLine("Method" + site.Name.ToString());
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }
        }

    }
}
