using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeeCarService.Models
{
    public class ServiceEvent
    {
        public int ServiceRequestID { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public int Duration { get; set; }

    }
}