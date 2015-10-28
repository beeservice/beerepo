using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeeCarService.ClientModels
{
    public class ClientVehicleClass
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public int VehichleTypeID { get; set; }

        public int Status { get; set; }
    }
}