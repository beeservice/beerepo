using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeeCarService.Models
{
    public class ServiceRequestCriteriaDto
    {
        public int selectedTeamId { get; set; }

        public int currentPage { get; set; }

        public string selectedDate { get; set; }

    }
}