using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeeCarService.ClientModels
{
    public class ClientAddon
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public decimal Cost { get; set; }

        public int Duration { get; set; }

        public int ServiceTypeID { get; set; }

        public int Status { get; set; }
    }
}