using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Utilities;
using Utilities.Models;

namespace TaskTracker.Models
{
    public class UsersRepositery
    {
        private TaskTrackerEntities db = new TaskTrackerEntities();
        private ExceptionLogsRepositery exrepo = new ExceptionLogsRepositery();
        //UtilityClass _UtilityClassObject = new UtilityClass();     
        Users user = new Users();
        List<Users> usersList = new List<Users>();
       

        /// <summary>
        ////Get all users detail
        /// </summary>
        /// <returns></returns>
        public List<Users> GetAllUsers()
        {
            try
            {
                usersList = db.Users.Where(x=>x.IsDeleted==false).ToList();
            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = "";// HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exrepo.InsertException(ex, url, site.Name);
            }
            return usersList; 
        }

        /// <summary>
        /// To find the user detail using email
        /// </summary>
        /// <param name="email"></param>
        /// <returns>return the object of user</returns>
        public Users GetUserDetailUsingEmail(string email)
        {
            
            try
            {
                user = db.Users.Where(x => x.Email == email).FirstOrDefault();
                
            }
            catch(Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exrepo.InsertException(ex, url,site.Name);
            }
            return user;
        }

        /// <summary>
        /// To find the user detail using email
        /// </summary>
        /// <param name="email"></param>
        /// <returns>return the object of user</returns>
        public Users GetUserDetailUsingEmailAndEmployeeId(string email, int? employeeId)
        {

            try
            {
                user = db.Users.Where(x => x.Email == email || x.EmployeeId == employeeId).FirstOrDefault();

            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exrepo.InsertException(ex, url, site.Name);
            }
            return user;
        }

        /// <summary>
        /// To find the user detail using Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>users type object</returns>
        public Users GetUsersDetailUsingId(int? id)
        {
            try
            {
                user = db.Users.Find(id);
            }
            catch(Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exrepo.InsertException(ex, url, site.Name);
            }                      
            return user;
        }

        /// <summary>
        /// To create the user
        /// </summary>
        /// <param name="users"></param>
        public void CreateUser(Users users)
        {
            try
            {
                db.Configuration.ValidateOnSaveEnabled = false;
                db.Users.Add(users);
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exrepo.InsertException(ex, url,site.Name);
            }
        }

        /// <summary>
        /// To edit the existing user
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public Users EditUser(Users users)
        {
            try
            {
                db.Configuration.ValidateOnSaveEnabled = false;
                Users tempuser=db.Users.Single(x => x.Id == users.Id);
                tempuser.ModifiedOn = users.ModifiedOn;
                tempuser.Name = users.Name;
                tempuser.Email = users.Email;
                tempuser.Password = users.Password;
                tempuser.Phone = users.Phone;
                tempuser.RoleId = users.RoleId;
                tempuser.IsDeleted = users.IsDeleted;
                tempuser.EmployeeId = users.EmployeeId;
                db.SaveChanges();
                return user;
            }
            catch(Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exrepo.InsertException(ex,url,site.Name);
            }
            return user;
        }

        /// <summary>
        /// delete user from Users
        /// </summary>
        /// <param name="users"></param>
        public int DeleteUser(Users users)
        {
            int success = 0;
            try
            {
                db.Configuration.ValidateOnSaveEnabled = false;
                Users nUsers = db.Users.Single(x => x.Id == users.Id);
                db.Entry(nUsers).Property(x => x.Password).IsModified = false;
                nUsers.IsDeleted = true;
                nUsers.DeletedOn = DateTime.Now;
                //db.Entry(nUsers).Property(x => x.IsDeleted).IsModified = true;
                //db.Entry(nUsers).Property(x => x.DeletedOn).IsModified = true;

                //nUsers.Name = users.Name;
                //nUsers.Phone = users.Phone;
                //nUsers.Email = users.Email;
                //nUsers.Password = users.Password;
                //nUsers.RoleId = users.RoleId;
                db.SaveChanges();
                //DELETE MENTOR
                MentorRepositery mentorRepositery = new MentorRepositery();
                mentorRepositery.DeleteMentorByTraineeId(user.Id);
                mentorRepositery.DeleteMentorByTraineerId(user.Id);
                success = 1;
            }
            catch(Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                exrepo.InsertException(ex, url,site.Name);
            }
            return success;
        }

        /// <summary>
        /// get all users detail for Index where Isdeleted is false
        /// </summary>
        /// <returns></returns>
        public IQueryable<Users> GetUsers()
        {
            var users = db.Users.OrderBy(x=>x.Name).Where(x => x.IsDeleted == false).Include(u => u.Role);
            return users;
        }

        /// <summary>
        /// to get the deleted users
        /// </summary>
        /// <returns>list of users</returns>
        public List<Users> GetDeletedUsers()
        {
            try
            {
                usersList = db.Users.Where(x => x.IsDeleted == true).ToList();
            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = "";// request.Url.AbsoluteUri;
                exrepo.InsertException(ex, url, site.Name);
            }
            return usersList;
        }

        /// <summary>
        /// Make Isdeleted as 1 
        /// </summary>
        /// <param name="id"></param>
        public void MakeIsdeletedFalse(int id)
        {
            try
            {
                db.Configuration.ValidateOnSaveEnabled = false;
                user = db.Users.Where(x => x.Id == id).First();
                user.IsDeleted = false;
                db.SaveChanges();
              

            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = "";// request.Url.AbsoluteUri;
                exrepo.InsertException(ex, url, site.Name);
            }
        }

        /// <summary>
        /// Gets Users List in Task Controller
        /// </summary>
        /// <returns></returns>
        public SelectList Users_By_Task()
        {
            var users = new SelectList(db.Users, "Id", "Name");
            return users;
        }
        /// <summary>
        /// Gets Users in task controller for uploading task
        /// </summary>
        /// <returns></returns>
        public SelectList Users_By_EmployeeId()
        {
            Task task = new Task();
            var users = new SelectList(db.Users, "Id", "Name", task.EmployeeId);
            return users;
        }

        /// <summary>
        /// To update password in ser table
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        public void UpdatePassword(string email, string password)
        {
            try
            {
                db.Configuration.ValidateOnSaveEnabled = false;
                user = db.Users.Where(x => x.Email == email).First();
                user.Password =password;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = "";// request.Url.AbsoluteUri;
                exrepo.InsertException(ex, url, site.Name);
            }
        }

        /// <summary>
        /// get email id allready exist in database for edit 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Users GetUserByEmailForEdit(string email, int id)
        {
            try
            {
                user = db.Users.Where(x => x.Email == email && x.Id != id).SingleOrDefault();
            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = "";// request.Url.AbsoluteUri;
                exrepo.InsertException(ex, url, site.Name);
            }
            return user;
        }

        public Users GetUserByEmployeeIdForEdit(int? employeeId, int id)
        {
            try
            {
                user = db.Users.Where(x => x.EmployeeId == employeeId && x.Id != id).SingleOrDefault();
            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = "";// request.Url.AbsoluteUri;
                exrepo.InsertException(ex, url, site.Name);
            }
            return user;
        }

        /// <summary>
        /// Get User Detail By Mentor For mentor trainee List
        /// </summary>
        /// <param name="mentorList"></param>
        /// <returns></returns>
        public List<Users> GetUserDetailByMentor(List<Mentor> mentorList)
        {
            try
            {
                foreach (Mentor item in mentorList)
                {
                    usersList.Add(db.Users.Where(x => x.Id == item.EmployeeId).First());
                }
            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = "";// request.Url.AbsoluteUri;
                exrepo.InsertException(ex, url, site.Name);
            }

            return usersList;
        }

        /// <summary>
        /// To delete a user permanent
        /// </summary>
        /// <param name="user"></param>
        public void DeletePermanent(Users user)
        {
            try
            {    if(user.IsDeleted==true)
                {
                 Users deletedUser=db.Users.Remove(user);
                    db.SaveChanges();
                }
                
            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = "";// request.Url.AbsoluteUri;
                exrepo.InsertException(ex, url, site.Name);
            }
        }

        /// <summary>
        /// to get only employee
        /// </summary>
        /// <returns></returns>
        public List<Users> GetAllEmployee()
        {
            try
            {
                usersList = db.Users.Where(x => x.IsDeleted == false && x.RoleId == 2).ToList();

            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = "";// request.Url.AbsoluteUri;
                exrepo.InsertException(ex, url, site.Name);
            }
            return usersList;
        }
    }
}