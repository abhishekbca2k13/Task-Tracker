using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TaskTracker.Models;

namespace TaskTracker
{
    public class MvcApplication : System.Web.HttpApplication
    {        
       //UtilityClass _UtilityClassObject = new UtilityClass();
        protected void Application_Start()
        {

                AreaRegistration.RegisterAllAreas();
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                BundleConfig.RegisterBundles(BundleTable.Bundles);

                ////for send mail from email queue
                //System.Timers.Timer emailService = new System.Timers.Timer();
                //emailService.Start();
                //emailService.Interval = 30000;
                //emailService.Elapsed += emailService_elapsed;

                ////for email alert email
                //System.Timers.Timer emailAlertService = new System.Timers.Timer();
                //emailAlertService.Start();
                //emailAlertService.Interval = 120000;
                //emailAlertService.Elapsed += emailAlertService_elapsed;
               
        }

        ///// <summary>
        ///// To send mail from email queue
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //public void emailService_elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //        //Send Email
        //        TaskTracker.Controllers.HomeController.SendEmailFromQueue();
        //}

        ///// <summary>
        ///// To send first reminder email
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //public void emailAlertService_elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{

        //        DateTime dt = DateTime.Now;
        //        TimeSpan time = dt.TimeOfDay;
        //        //Send First Reminder Mail
        //        TimeSpan FRStart = new TimeSpan(17, 10, 0);
        //        TimeSpan FREnd = new TimeSpan(17,15, 0);
        //        if (time >= FRStart && time <= FREnd)
        //        {
        //             _UtilityClassObject.SendReminderEmail();
        //        }


        //        //Send Second Reminder Mail
        //        TimeSpan SRStart = new TimeSpan(17, 16, 0);
        //        TimeSpan SREnd = new TimeSpan(17, 21, 0);
        //        if (time >= SRStart && time <= SREnd)
        //        {
        //            _UtilityClassObject.SendReminderEmail();
        //        }

        //        //Send third Reminder Mail
        //        TimeSpan TRStart = new TimeSpan(17, 22, 0);
        //        TimeSpan TREnd = new TimeSpan(17, 27, 0);
        //        if (time >= SRStart && time <= SREnd)
        //        {
        //            _UtilityClassObject.SendReminderEmail();
        //        }

        //        //Send Warning Mail
        //        TimeSpan WStart = new TimeSpan(17, 28, 0);
        //        TimeSpan WEnd = new TimeSpan(17, 33, 0);
        //        if (time >= WStart && time <= WEnd)
        //        {
        //            _UtilityClassObject.SendWarningEmail();
        //        }

        //}        

    }
}

