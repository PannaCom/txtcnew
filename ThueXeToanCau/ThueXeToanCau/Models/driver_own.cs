//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ThueXeToanCau.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class driver_own
    {
        public long id { get; set; }
        public string car_number { get; set; }
        public string driver_name { get; set; }
        public Nullable<double> money_month { get; set; }
        public Nullable<System.DateTime> date_month { get; set; }
        public Nullable<double> money_period { get; set; }
        public Nullable<System.DateTime> date_period { get; set; }
        public Nullable<double> money_year { get; set; }
        public Nullable<System.DateTime> date_year { get; set; }
        public Nullable<System.DateTime> date_time { get; set; }
    }
}