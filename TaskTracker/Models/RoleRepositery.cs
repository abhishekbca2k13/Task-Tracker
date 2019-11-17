using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Utilities;
using Utilities.Models;

namespace TaskTracker.Models
{
    public class RoleRepositery
    {
        ExceptionLogsRepositery exceptionLogsRepositery = new ExceptionLogsRepositery();
        Role role = new Role();
        List<Role> roleList = new List<Role>();
        TaskTrackerEntities db = new TaskTrackerEntities();

        /// <summary>
        /// create role 
        /// </summary>
        /// <param name="role"></param>
        public void CreateRole(Role role)
        {
            try
            {
                db.Role.Add(role);
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
        /// edit a particular role
        /// </summary>
        /// <param name="role"></param>
        public void EditRole(Role role)
        {
            try
            {
                db.Entry(role).State = EntityState.Modified;
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
        /// delete a particular role
        /// </summary>
        /// <param name="role"></param>
        public void DeleteRole(Role role)
        {
            try
            {
                db.Role.Remove(role);
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
        /// get role all details 
        /// </summary>
        /// <returns></returns>
        public List<Role> GetAllRolesDetail()
        {
            try
            {                
                roleList = db.Role.ToList();
            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }
            return roleList;

        }
        /// <summary>
        /// get role details using roleId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Role GetRoleDetailUsingRoleId(int? id)
        {
            try
            {
                role=db.Role.Where(x => x.Id == id).First();

            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exceptionLogsRepositery.InsertException(ex, url, site.Name);
            }
            return role;
        }
        /// <summary>
        /// Get All Role with reference of Users Table(VAISHALI GOEL)
        /// </summary>
        /// <returns></returns>
        public SelectList GetRole()
        {
            var Role = new SelectList(db.Role, "Id", "Type");
            return Role;
        }

        public SelectList Role_by_RoleId(Users users)
        {
            var roles = new SelectList(db.Role, "Id", "Type", users.RoleId);
            return roles;
        }

    }
}