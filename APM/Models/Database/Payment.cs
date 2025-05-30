using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace APM.Models.Database
{
    public class Payment
    {
        public string MerchantID { get; set; }
        public string CallBackUrl { get; set; }
        public string PaymentGatewaytype { get; set; }  
        public Payment()
        {
            this.MerchantID = "";
            this.CallBackUrl = "";
            this.PaymentGatewaytype = ""; 
        }

        public Payment(string MerchantID, string CallBackUrl, string PaymentGatewaytype)
        {
            this.MerchantID = MerchantID;
            this.CallBackUrl = CallBackUrl;
            this.PaymentGatewaytype = PaymentGatewaytype; 
        }
        public Payment(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(Payment));
            var EmailInfo = serializer.Deserialize(stringReader) as Payment;
            this.MerchantID = EmailInfo.MerchantID;
            this.CallBackUrl = EmailInfo.CallBackUrl;
            this.PaymentGatewaytype = EmailInfo.PaymentGatewaytype; 
        }
    }
}