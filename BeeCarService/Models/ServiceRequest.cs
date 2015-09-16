using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BeeCarService.Data;

namespace BeeCarService.Models
{
    public class ServiceRequest
    {

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int CustomerID { get; set; }

        public List<ServiceRequestVehicle> ServiceRequestVehicles { get; set; }
    }
}