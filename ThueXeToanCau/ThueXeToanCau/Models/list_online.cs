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
    
    public partial class list_online
    {
        public long id { get; set; }
        public Nullable<System.DateTime> date_time { get; set; }
        public Nullable<double> lon { get; set; }
        public Nullable<double> lat { get; set; }
        public System.Data.Entity.Spatial.DbGeography geo { get; set; }
        public string phone { get; set; }
        public string car_number { get; set; }
        public Nullable<int> status { get; set; }
        public Nullable<int> car_type { get; set; }
        public Nullable<int> os { get; set; }
        public string reg_id { get; set; }
        public Nullable<long> driver_id { get; set; }
        public Nullable<int> car_size { get; set; }
    }
}
