using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeeCarService.Models
{
    public class ServiceType
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public decimal Duration { get; set; }

        public decimal Cost { get; set; }

        public List<ServiceAddon> Addons { get; set; }

    }
}