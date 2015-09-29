using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeeCarService.Models
{
    public class TeamCalendar
    {
        public int ServiceTeamID { get; set; }

        public string ServiceTeamName { get; set; }

        public List<ServiceEvent> ServiceEvents { get; set; }

    }
}