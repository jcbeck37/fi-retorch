//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace fi.retorch.com.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class act_reminders
    {
        public int reminder_id { get; set; }
        public Nullable<int> account_id { get; set; }
        public Nullable<int> category_id { get; set; }
        public Nullable<int> schedule_id { get; set; }
        public Nullable<System.DateTime> start_date { get; set; }
        public Nullable<System.DateTime> end_date { get; set; }
        public string reminder_name { get; set; }
        public Nullable<int> amount { get; set; }
        public Nullable<decimal> interest_rate { get; set; }
        public byte positive { get; set; }
        public Nullable<System.DateTime> last_posted { get; set; }
    
        public virtual act_accounts act_accounts { get; set; }
        public virtual act_schedules act_schedules { get; set; }
    }
}