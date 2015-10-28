using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BeeCarService.Data;

namespace BeeCarService.Models
{
    public class ServiceRequest
    {

        public int ID { get; set; }

        public DateTime StartTime { get; set; }

        public string FormattedStartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int CustomerID { get; set; }

        public string CustomerName { get; set; }

        public string CustomerAddress { get; set; }

        public string CustomerPhone { get; set; }

        public string CustomerEmail { get; set; }

        public int ServiceTeamId { get; set; }

        public int VehicleCount { get; set; }

        public int ServiceDuration { get; set; }

        public short Status { get; set; }

        public decimal Cost { get; set; }

        public BeeUser BeeUser { get; set; }

        public List<ServiceRequestVehicle> ServiceRequestVehicles { get; set; }
    }
}