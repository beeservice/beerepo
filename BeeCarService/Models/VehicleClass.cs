using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeeCarService.Models
{
    public class VehicleClass
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public List<ServiceType> Services { get; set; }

    }
}