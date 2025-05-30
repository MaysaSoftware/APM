using APM.Models.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace APM.Models.Database
{
    public class PublicJob
    {
        public string FullName { get; set; } 
        public string Query { get; set; } 
        public string StartDate { get; set; } 
        public string EndDate { get; set; } 
        public string StartTime { get; set; } 
        public string EndTime { get; set; }  
        public long CoreObjectID { get; set; } 
        public long RelatedDatasource { get; set; } 
        public int RepeatDay { get; set; } 
        public int RepeatClock { get; set; } 
        public CoreDefine.PublicJobType PublicJobType { get; set; } 
        public PublicJob()
        {
            PublicJobType = CoreDefine.PublicJobType.خالی;
            Query=String.Empty;
            StartDate = CDateTime.GetNowshamsiDate();
            EndDate = CDateTime.GetNowshamsiDate();
            StartTime = "00:00:00";
            EndTime = "23:59:59";
            RepeatDay=1;
            RepeatClock=1;
        } 
        public PublicJob(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(PublicJob));
            var PublicJobTypeInfo = serializer.Deserialize(stringReader) as PublicJob;
            this.FullName = _CoreObject.FullName;
            this.CoreObjectID = _CoreObject.CoreObjectID;
            PublicJobType = PublicJobTypeInfo.PublicJobType;
            RelatedDatasource = PublicJobTypeInfo.RelatedDatasource;
            Query = PublicJobTypeInfo.Query;
            RepeatDay = PublicJobTypeInfo.RepeatDay;
            RepeatClock = PublicJobTypeInfo.RepeatClock;
            StartDate = PublicJobTypeInfo.StartDate;
            EndDate = PublicJobTypeInfo.EndDate;
            StartTime = PublicJobTypeInfo.StartTime;
            EndTime = PublicJobTypeInfo.EndTime;
        }
    }
}