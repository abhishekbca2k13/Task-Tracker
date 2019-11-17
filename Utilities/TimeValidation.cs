using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Reflection;
using Utilities.Models;

namespace Utilities
{
    public class TimeValidation : ValidationAttribute
    {
        private ExceptionLogsRepositery _ExceptionLogRepositeryObject = new ExceptionLogsRepositery();
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                DateTime dt = (DateTime)value;
                TimeSpan ts = dt.TimeOfDay;
                TimeSpan start = new TimeSpan(9, 00, 0);
                TimeSpan end = new TimeSpan(20, 00, 0);
                if (ts >= start && ts <= end)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Your task status should be submitted with in the openning hours, which is from 9:00 am to 8:00 pm.");
                }
            }
            catch (Exception ex)
            {
                MethodBase site = MethodBase.GetCurrentMethod();
                string url = "";// request.Url.AbsoluteUri;
                _ExceptionLogRepositeryObject.InsertException(ex, url, site.Name);
                return new ValidationResult("Invalid time input!");
            }
        }
    }
}
