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
    
    public partial class transaction
    {
        public long id { get; set; }
        public string type { get; set; }
        public System.DateTime date { get; set; }
        public string car_number { get; set; }
        public Nullable<double> money { get; set; }
        public string note { get; set; }
    }
}
