using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeeCarService.Models
{
    public class MasterData
    {
        public int ID { get; set; }

        public string Type { get; set; }

        public List<VechicleClass> Classes { get; set; }

    }
}