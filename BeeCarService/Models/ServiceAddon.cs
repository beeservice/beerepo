﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeeCarService.Models
{
    public class ServiceAddon
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public decimal Cost { get; set; }

        public int Duration { get; set; }
    }
}