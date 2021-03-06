//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BeeCarService.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class BeeUser
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BeeUser()
        {
            this.ServiceRequests = new HashSet<ServiceRequest>();
        }
    
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public System.DateTime RegDate { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public Nullable<int> LandmarkID { get; set; }
        public string Message { get; set; }
        public short ContactPreference { get; set; }
        public bool TextNotifications { get; set; }
        public short PaymentMode { get; set; }
    
        public virtual Landmark Landmark { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ServiceRequest> ServiceRequests { get; set; }
    }
}
