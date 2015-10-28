using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeeCarService.ClientModels
{
    public class ClientServiceType
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public int Duration { get; set; }

        public decimal Cost { get; set; }

        public int VehicleClassID { get; set; }

        public int Status { get; set; }

    }
}