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
    
    public partial class act_links
    {
        public int link_id { get; set; }
        public Nullable<int> user_id { get; set; }
        public string link_url { get; set; }
        public string link_title { get; set; }
        public byte link_inactive { get; set; }
    
        public virtual ng_users ng_users { get; set; }
    }
}
