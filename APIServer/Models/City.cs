//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace APIServer.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class City
    {
        public City()
        {
            this.User = new HashSet<User>();
        }
    
        public long Id { get; set; }
        public string Name { get; set; }
        public Nullable<long> Country_id { get; set; }
    
        public virtual Country Country { get; set; }
        public virtual ICollection<User> User { get; set; }
    }
}
