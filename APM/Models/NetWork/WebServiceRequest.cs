using APM.Models.APMObject;
using APM.Models.Database;
using APM.Models.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

namespace APM.Models.NetWork
{
    public class WebServiceRequest
    {
        public string Url { get; set; }
        public string Method { get; set; }
        public string PostData { get; set; }

        public void SendRequest()
        {
             
            try
            {
                WebRequest request = WebRequest.Create(Url);
                request.Method = Method;
                byte[] byteArray = Encoding.UTF8.GetBytes(PostData);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                WebResponse response2 = request.GetResponse();
                Console.WriteLine(((HttpWebResponse)response2).StatusDescription);
                dataStream = response2.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                Console.WriteLine(responseFromServer);
                reader.Close();
                dataStream.Close();
                response2.Close();
                System.Diagnostics.Debug.WriteLine(responseFromServer);

            }
            catch(Exception ex)
            {

                using (var client = new HttpClient())
                {
                    try
                    { 
                        var response = client.GetAsync(Url + "?" + PostData);
                        response.Wait();
                        var result = response.Result;
                        //string textResult = response.Content.ReadAsStringAsync(); 
                        if (result.IsSuccessStatusCode)
                        {
                            var jsonResponse = result.Content.ReadAsStringAsync().Result;
                            string Massage = jsonResponse.ToString();
                            //readTask.Wait();
                        }
                        else
                        {

                        }
                    }
                    catch (Exception ex2)
                    {

                    }
                }
            }

        }

        public void GenarateUrlFromWebService(long WebServiceID,string DeclareQuery,string[] ColumnNames,object[] ColumnValues , ref string postData)
        {
            CoreObject WebServiceCore = CoreObject.Find(WebServiceID);
            WebService webService = new WebService(WebServiceCore);
            List<CoreObject> PapameterCoreList = CoreObject.FindChilds(WebServiceCore.CoreObjectID, CoreDefine.Entities.پارامتر_وب_سرویس);
            int Counter = 1; 
            foreach (CoreObject Papameter in PapameterCoreList)
            {
                WebServiceParameter webServiceParameter = new WebServiceParameter(Papameter);
                for (int Index = 0; Index < ColumnNames.Length; Index++)
                {
                    string[] ParameterValueArr = webServiceParameter.Value.Split(' ');
                    for (int ParameterValueIndex = 0; ParameterValueIndex < ParameterValueArr.Length; ParameterValueIndex++)
                        if (ParameterValueArr[ParameterValueIndex] == "@" + ColumnNames[Index])
                            ParameterValueArr[ParameterValueIndex] = ColumnValues[Index].ToString();
                    webServiceParameter.Value =string.Join(" ",ParameterValueArr); 

                    if (webServiceParameter.QueryValue !="")
                        webServiceParameter.QueryValue = Referral.DBData.SelectField(DeclareQuery+"\n"+ webServiceParameter.QueryValue).ToString();

                }
                if (webServiceParameter.ConvertToJsonArr)
                {
                    string[] JsobArr = new string[0];

                    if (!string.IsNullOrEmpty(webServiceParameter.QueryValue))
                    {
                        string[] Arr = Referral.DBData.SelectColumn(DeclareQuery + "\n" +Tools.Tools.CheckQuery(webServiceParameter.QueryValue)).OfType<string>().ToArray();
                        Array.Resize(ref JsobArr, Arr.Length);
                        JsobArr = Arr;
                    }

                    if (!string.IsNullOrEmpty(webServiceParameter.Value))
                    {
                        Array.Resize(ref JsobArr, JsobArr.Length + 1);
                        JsobArr[JsobArr.Length - 1] = webServiceParameter.Value;
                    }
                    string json = JsonConvert.SerializeObject(JsobArr);
                    postData += webServiceParameter.Name + "=" + json;
                }
                else
                {
                    postData += webServiceParameter.Name + "=" + webServiceParameter.Value;
                    if (!string.IsNullOrEmpty(webServiceParameter.QueryValue))
                        postData += Referral.DBData.SelectField(DeclareQuery + "\n" + Tools.Tools.CheckQuery(webServiceParameter.QueryValue));
                }

                if (Counter < PapameterCoreList.Count)
                {
                    Counter++;
                    postData += "&";
                }
            }

            if (string.IsNullOrEmpty(this.Url))
                this.Url = webService.URL;

            if (string.IsNullOrEmpty(this.PostData))
                this.PostData = postData;

        }
    }

    public class WebServiceMethod
    {
        public string Post = "POST";
        public string GET = "GET";
    }
}