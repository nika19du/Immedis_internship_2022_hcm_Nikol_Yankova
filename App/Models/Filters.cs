using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Models
{
    public class Filters
    {
        public Filters(string filterstring)
        {
            this.FilterString = filterstring ?? "all-all";
            if(FilterString.Contains('-'))
            {
                string[] filters = this.FilterString.Split('-');
                PositionId =filters[0];
                RoleId=filters[1];
            }
        }
        public string FilterString { get; set; }

        public string PositionId { get; set; }
        public string RoleId { get; set; }

        public bool HasPosition => PositionId.ToLower() != "all"; 
    }
}
