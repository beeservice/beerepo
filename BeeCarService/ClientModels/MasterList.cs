using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeeCarService.ClientModels
{
    public class MasterList
    {
        public List<ClientVehicleType> VehicleTypes { get; set; }
        public List<ClientVehicleClass> VehicleClasses { get; set; }
        public List<ClientServiceType> ServiceTypes { get; set; }
        public List<ClientAddon> AddOns { get; set; }

    }
}