//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Utilities
{
    using System;
    using System.Collections.Generic;
    
    public partial class ExceptionLogs
    {
        public int Id { get; set; }
        public string Method { get; set; }
        public string Message { get; set; }
        public string Url { get; set; }
        public int ExceptionNumber { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
