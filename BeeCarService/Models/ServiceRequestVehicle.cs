using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeeCarService.Models
{
    public class ServiceRequestVehicle
    {
        public int VehicleTypeID { get; set; }
        public int VehicleClassID { get; set; }
        public int ServiceTypeID { get; set; }
        public List<int> VehicleAddOnIDs { get; set; }
    }
}