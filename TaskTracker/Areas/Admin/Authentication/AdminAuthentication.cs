using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;

namespace TaskTracker.Areas.Admin.Authentication
{
    public class AdminAuthenticationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (filterContext.HttpContext.Session.Contents["AdminId"] == null)
            {
                filterContext.HttpContext.Session.Contents.Add("controller", filterContext.Controller.ControllerContext.RouteData.Values["controller"].ToString());
                filterContext.HttpContext.Session.Contents.Add("action", filterContext.Controller.ControllerContext.RouteData.Values["action"].ToString());
                filterContext.Result = new RedirectToRouteResult("default", new RouteValueDictionary(new
                {
                    controller = "Home",
                    action = "Login",
                }));
            }
        }
    }
}