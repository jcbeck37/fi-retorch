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
    
    public partial class act_accounts
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public act_accounts()
        {
            this.act_reminders = new HashSet<act_reminders>();
            this.act_transactions = new HashSet<act_transactions>();
        }
    
        public int account_id { get; set; }
        public Nullable<int> user_id { get; set; }
        public Nullable<int> type_id { get; set; }
        public string account_name { get; set; }
        public Nullable<System.DateTime> start_date { get; set; }
        public Nullable<int> initial_balance { get; set; }
        public byte isActive { get; set; }
        public Nullable<int> dispOrder { get; set; }
    
        public virtual act_types act_types { get; set; }
        public virtual ng_users ng_users { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<act_reminders> act_reminders { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<act_transactions> act_transactions { get; set; }
    }
}
