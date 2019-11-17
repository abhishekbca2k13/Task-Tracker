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
    public class TimerSettingRepositery
    {
        TaskTrackerEntities db = new TaskTrackerEntities();
        List<TimerSetting> timerSettingsList = new List<TimerSetting>();
        ExceptionLogsRepositery _ExceptionLogsRepositeryObject = new ExceptionLogsRepositery();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<TimerSetting> ReadAllTimerSetting()
        {
            try
            {
                timerSettingsList = db.TimerSetting.ToList();
                db.SaveChanges();
            }
            catch (Exception ex)
            {

                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                _ExceptionLogsRepositeryObject.InsertException(ex, url, site.Name);
            }
            return timerSettingsList;
        }

        public void UpdateTimerSetting(TimerSetting timerSetting)
        {
            try
            {
                TimerSetting temp = new TimerSetting();
                temp = db.TimerSetting.SingleOrDefault(x => x.Id == timerSetting.Id);
                temp.StartTime = timerSetting.StartTime;
                temp.EndTime = timerSetting.EndTime;
                db.SaveChanges();
            }
            catch (Exception ex)
            {

                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                _ExceptionLogsRepositeryObject.InsertException(ex, url, site.Name);
            }

        }

        public TimerSetting GetTimerById(int? id)
        {
            TimerSetting timerSetting = new TimerSetting();
            try
            {

                timerSetting = db.TimerSetting.Find(id);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                _ExceptionLogsRepositeryObject.InsertException(ex, url, site.Name);
            }
            return timerSetting;

        }

        public void EditTimer(TimerSetting timerSetting)
        {
            try
            {
                timerSetting.CreatedOn = DateTime.Now;
                timerSetting.ModifiedOn = DateTime.Now;
                db.Entry(timerSetting).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {

                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                _ExceptionLogsRepositeryObject.InsertException(ex, url, site.Name);
            }
        }
    }
}