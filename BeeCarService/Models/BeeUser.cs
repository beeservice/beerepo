using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeeCarService.Models
{
    public class BeeUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public System.DateTime RegDate { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Landmark { get; set; }
        public string Message { get; set; }
        public short ContactPreference { get; set; }
        public bool TextNotifications { get; set; }
        public short PaymentMode { get; set; }

    }
}