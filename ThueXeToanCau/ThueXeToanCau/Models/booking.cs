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
    
    public partial class booking
    {
        public long id { get; set; }
        public string car_from { get; set; }
        public string car_to { get; set; }
        public Nullable<int> car_type { get; set; }
        public string car_hire_type { get; set; }
        public string car_who_hire { get; set; }
        public Nullable<System.DateTime> from_datetime { get; set; }
        public Nullable<System.DateTime> to_datetime { get; set; }
        public System.Data.Entity.Spatial.DbGeography geo1 { get; set; }
        public Nullable<double> lon1 { get; set; }
        public Nullable<double> lat1 { get; set; }
        public System.Data.Entity.Spatial.DbGeography geo2 { get; set; }
        public Nullable<double> lon2 { get; set; }
        public Nullable<double> lat2 { get; set; }
        public Nullable<int> book_price { get; set; }
        public Nullable<System.DateTime> datebook { get; set; }
    }
}
