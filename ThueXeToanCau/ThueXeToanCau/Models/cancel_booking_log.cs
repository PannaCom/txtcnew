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
    
    public partial class cancel_booking_log
    {
        public long id { get; set; }
        public Nullable<long> id_booking { get; set; }
        public string custom_phone { get; set; }
        public string driver_phone { get; set; }
        public Nullable<System.DateTime> date_time { get; set; }
        public Nullable<int> date_id { get; set; }
        public Nullable<int> type_cancel { get; set; }
        public Nullable<long> user_id { get; set; }
        public Nullable<long> driver_id { get; set; }
    }
}
