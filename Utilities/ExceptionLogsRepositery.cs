using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.EntityClient;
using System.Configuration;
using System.Data;
using System.Reflection;

namespace Utilities.Models
{
    public class ExceptionLogsRepositery
    {
        UtilitieEntities db = new UtilitieEntities();

        //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ConnectionString);

        /// <summary>
        /// insert exception details in exceptionLogs
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="url"></param>
        /// <param name="method"></param>
        public void InsertException(Exception ex, string url, string method)
        {
            try
            {
                ExceptionLogs exceptionLogs = new ExceptionLogs();

                exceptionLogs.Url = url;
                exceptionLogs.Method = method;
                exceptionLogs.Message = ex.Message.ToString();
                exceptionLogs.CreatedOn = DateTime.Now;
                exceptionLogs.ModifiedOn = DateTime.Now;
                exceptionLogs.ExceptionNumber = ex.HResult;
                db.ExceptionLogs.Add(exceptionLogs);
                db.SaveChanges();
                //SqlCommand cmd = new SqlCommand("Insert_Exception", con);
                //cmd.CommandType = CommandType.StoredProcedure;

                //cmd.Parameters.AddWithValue("@Method", method);
                //cmd.Parameters.AddWithValue("@Message", "Message : "+ex.Message.ToString()+"\nSource : "+ex.Source+"\nStackTrace : "+ex.StackTrace+"\nInner Exception : "+ex.InnerException);
                //cmd.Parameters.AddWithValue("@ExceptionNumber", ex.GetHashCode());
                //cmd.Parameters.AddWithValue("@Url", url.ToString());
                //con.Open();
                //int i = cmd.ExecuteNonQuery();
                //con.Close();

            }
            catch (Exception exc)
            {
                InsertExceptionIntoExceptionLogFile(exc, url, method);
            }

        }

        /// <summary>
        /// Writing exception into exception file
        /// </summary>
        /// <param name="exc"></param>
        /// <param name="url"></param>
        /// <param name="method"></param>
        public void InsertExceptionIntoExceptionLogFile(Exception exc, string url, string method)
        {
            try
            {
                var path = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/"));
                path = path + "\\ExceptionLogs.txt";

                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine();
                    writer.Write("Date = " + DateTime.Now);
                    writer.WriteLine();
                    writer.Write("Error occured at = " + url);
                    writer.WriteLine();
                    writer.Write("Number for the Exception is = " + exc.HResult.ToString());
                    writer.WriteLine();
                    writer.Write("Message for the exception = " + exc.Message.ToString());
                    writer.WriteLine();
                    writer.Write("Method where exception occure= " + method);
                    writer.WriteLine();
                }
            }
            catch (Exception exe)
            {
                string error = exe.Message;
            }
        }

        List<ExceptionLogs> exceptionLogsList = new List<ExceptionLogs>();

        public List<ExceptionLogs> GetAllExceptions()
        {
            try
            {
                exceptionLogsList = db.ExceptionLogs.OrderByDescending(x => x.CreatedOn).ToList();
            }
            catch (Exception exc)
            {

                MethodBase site = MethodBase.GetCurrentMethod();
                string url = HttpContext.Current.Request.Url.AbsoluteUri;// request.Url.AbsoluteUri;
                InsertExceptionIntoExceptionLogFile(exc, url, site.Name);
            }
            return exceptionLogsList;
        }
    }
}
