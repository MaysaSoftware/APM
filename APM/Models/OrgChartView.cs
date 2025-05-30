using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APM.Models
{
    public class OrgChartView
    {
        public long Id { get; set; }
        public long ParentId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Avatar { get; set; }
        public string Group { get; set; }
        public bool Expanded { get; set; }      
    }
}