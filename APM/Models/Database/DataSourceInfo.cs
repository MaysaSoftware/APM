using APM.Models.Tools;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Xml.Serialization; 

namespace APM.Models.Database
{
    public class DataSourceInfo
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DataBase { get; set; }
        public string ServerName { get; set; }
        public string FilePath { get; set; }
        public CoreDefine.DataSourceType DataSourceType { get; set; }
         
        public string ConnectionString
        {
            get
            {
                return "Password=" + Password + ";Persist Security Info=True;Data Source=" + ServerName + ";Integrated Security=False; Initial Catalog=" + DataBase + ";User ID=" + UserName;
            }
            set { }
        }

        public DataSourceInfo()
        { 
            this.UserName = ""; 
            this.Password = "";
            this.DataBase = "";
            this.ServerName = "";
            this.FilePath = "";
            this.DataSourceType = CoreDefine.DataSourceType.SQLSERVER; 
        }

        public DataSourceInfo(string Source, string DataBase, string DBPassword, string DBUser, CoreDefine.DataSourceType DataSourceName,string FilePath)
        {
            this.ServerName = Source;
            this.UserName = DBUser;
            this.Password = DBPassword;
            this.DataBase = DataBase;
            this.DataSourceType = DataSourceName; 
            this.FilePath = FilePath; 
        }

        public DataSourceInfo(CoreObject _CoreObject)
        {
            string ValueXml = _CoreObject.Value.ToString();
            var stringReader = new System.IO.StringReader(ValueXml);
            var serializer = new XmlSerializer(typeof(DataSourceInfo));
            var DBInfo = serializer.Deserialize(stringReader) as DataSourceInfo; 
            this.ServerName=DBInfo.ServerName;
            this.UserName=DBInfo.UserName;
            this.Password = DBInfo.Password; 
            this.DataBase = DBInfo.DataBase; 
            this.DataSourceType = DBInfo.DataSourceType; 
            this.FilePath = DBInfo.FilePath; 
        }
        public bool CheckConnected()
        {
            if (this.DataSourceType == CoreDefine.DataSourceType.SQLSERVER)
            {
                SqlConnection DBConnection = new SqlConnection(this.ConnectionString);
                try
                {
                    DBConnection.Open();
                    DBConnection.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else if (this.DataSourceType == CoreDefine.DataSourceType.MySql)
            {
                //MySqlConnection DBConnection = new MySqlConnection("Server="+this.ServerName+";Database="+this.DataBase+";Uid="+this.UserName+ ";Password=" + this.Password+";");
                MySqlConnection DBConnection = new MySqlConnection("SERVER=" + ServerName + ";" + "DATABASE=" + DataBase + ";" + "UID=" + UserName + ";" + "PASSWORD=" + Password + ";");
                try
                {
                    DBConnection.Open();
                    DBConnection.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            
            return false;
        }


    }
}