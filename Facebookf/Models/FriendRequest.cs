//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Facebookf.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class FriendRequest
    {
        public int SenderID { get; set; }
        public int RecieverID { get; set; }
        public Nullable<int> IsApproved { get; set; }
    
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}
