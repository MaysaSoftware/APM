using APM.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APM.Models.APMObject
{
    public class SubEntities
    {
        public CoreDefine.Entities Entities { get; set; }
        public string Name { get; set; }  
        public string Text { get; set; }  
        public string Class { get; set; }  
        
        public SubEntities()
        {

        }
    }
}