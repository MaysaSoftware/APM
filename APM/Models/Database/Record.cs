using APM.Models.Tools;
using APM.Models.APMObject;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace APM.Models.Database
{

    public partial class Record
    {
        public string[] Columns = new string[0];
        public object[] Values = new object[0];
        public List<CoreObject> FieldsObject = new List<CoreObject>();
        public bool IsHasData;

        public Record()
        {
        }

        public Record(SQLDataBase _DataBase, string _TableName, long _ID, long _TableID=0)
        {
            try
            {
                if (1==1)
                {
                    Table TableInfo = new Table();

                    if(_TableID>0) 
                        TableInfo=new Table(CoreObject.Find(_TableID));

                    Columns = _DataBase.Columns(_TableName, true);
                    string PrimeryKey = _DataBase.GetIdentityTable(_TableName);
                    string Where = " Where [" + PrimeryKey + "] = " + _ID;

                    string ColumnQuery = string.Empty;
                    foreach (string ColumnItem in Columns)
                        ColumnQuery += "[" + ColumnItem + "]\n,";

                    ColumnQuery= ColumnQuery.Substring(0, ColumnQuery.Length - 1);  

                    Values = _DataBase.SelectRecord("Select " + ColumnQuery + " From [" + TableInfo.TABLESCHEMA +"].["+ _TableName+"] \n" + Where);

                    if (_TableName.EndsWith("CoreObjectAttachment"))
                    {
                        Values[Array.IndexOf(Columns, "Value")] = Values[Array.IndexOf(Columns, "Value")].ToString()==""?true : Values[Array.IndexOf(Columns, "Value")];
                        Values[Array.IndexOf(Columns, "Thumbnail")] = Values[Array.IndexOf(Columns, "Thumbnail")].ToString()==""?true : Values[Array.IndexOf(Columns, "Thumbnail")]; 
                    }

                    if (Values.Length > 0)
                    {
                        FieldsObject = CoreObject.FindChilds(_TableID, CoreDefine.Entities.فیلد);
                        FieldsObject.RemoveAll(item => item.FullName == PrimeryKey);
                        IsHasData = true;
                    }
                }
            }
            catch (Exception ex)
            {
                IsHasData = false;
            }
        }

        public Record(SQLDataBase Engine, string _Query)
        {
            Engine.Open();
            var TheCommand = new SqlCommand(_Query, Engine.ConnectionData.DBConnection);
            SqlDataReader TheReader;
            TheCommand.CommandType = System.Data.CommandType.Text;
            IsHasData = false;
            try
            {
                TheReader = TheCommand.ExecuteReader();
                Columns = new string[TheReader.FieldCount];
                Values = new object[TheReader.FieldCount];
                if (TheReader.Read())
                {
                    for (int Index = 0, loopTo = Columns.Length - 1; Index <= loopTo; Index++)
                    {
                        Columns[Index] = TheReader.GetName(Index);
                        Values[Index] = TheReader[Index];
                    }

                    IsHasData = true;
                }

                TheReader.Close();
            }
            catch (Exception ex)
            {

            }
        }

        public Record(MySqlDatabase _DataBase, string _TableName, long _ID)
        {
            try
            {
                Columns = _DataBase.Columns(_TableName, true);
                string PrimeryKey = _DataBase.GetIdentityTable(_TableName).ToString();
                string Where = " Where " + PrimeryKey + "=" + _ID;

                Values = _DataBase.SelectRecord("Select " + String.Join(",", Columns) + " From " + _TableName + Where);
                if (Values.Length > 0)
                {

                    if (!_TableName.EndsWith("CoreObjectAttachment"))
                    {
                        CoreObject _Table = CoreObject.Find(CoreDefine.Entities.جدول, _TableName);
                        FieldsObject = CoreObject.FindChilds(_Table.CoreObjectID, CoreDefine.Entities.فیلد);
                        FieldsObject.RemoveAll(item => item.FullName == PrimeryKey);
                    }
                    IsHasData = true;
                }
            }
            catch (Exception ex)
            {
                IsHasData = false;
            }
        }

        public Record(AccessDatabase _DataBase, long _TableID, long _ID)
        {
            try
            {
                CoreObject TabeleObject = CoreObject.Find(_TableID);
                Table Table = new Table(TabeleObject);
                foreach (Field Field in Desktop.DataFields[_TableID.ToString()])
                {
                    if (!Field.IsIdentity && !Field.IsVirtual)
                    {
                        Array.Resize<string>(ref Columns, Columns.Length + 1);
                        Columns[Columns.Length - 1] = Field.FieldName;
                    }
                }

                string PrimeryKey = Table.IDField().FieldName;
                string Where = " Where " + PrimeryKey + "=" + _ID;

                Values = _DataBase.SelectRecord("Select " + String.Join(",", Columns) + " From " + TabeleObject.FullName + Where);
                if (Values.Length > 0)
                {

                    if (!TabeleObject.FullName.EndsWith("CoreObjectAttachment"))
                    {
                        FieldsObject = CoreObject.FindChilds(_TableID, CoreDefine.Entities.فیلد);
                        FieldsObject.RemoveAll(item => item.FullName == PrimeryKey);
                    }
                    IsHasData = true;
                }
            }
            catch (Exception ex)
            {
                IsHasData = false;
            }
        }

        public Record(AccessDatabase Engine, string _Query)
        {
            Engine.Open();
            var TheCommand = new OleDbCommand(_Query, Engine.ConnectionData.DBConnection);
            OleDbDataReader TheReader;
            TheCommand.CommandType = System.Data.CommandType.Text;
            IsHasData = false;
            try
            {
                TheReader = TheCommand.ExecuteReader();
                Columns = new string[TheReader.FieldCount];
                Values = new object[TheReader.FieldCount];
                if (TheReader.Read())
                {
                    for (int Index = 0, loopTo = Columns.Length - 1; Index <= loopTo; Index++)
                    {
                        Columns[Index] = TheReader.GetName(Index);
                        Values[Index] = TheReader[Index];
                    }

                    IsHasData = true;
                }

                TheReader.Close();
            }
            catch (Exception ex)
            {

            }
        }

        public Record(ExcelDatabase _DataBase, long _TableID, long _ID)
        {
            try
            {
                CoreObject TabeleObject = CoreObject.Find(_TableID);
                Columns = _DataBase.Columns(TabeleObject.FullName, true);

                string PrimeryKey = _DataBase.GetIdentityTable(_TableID).ToString();
                string Where = " Where " + PrimeryKey + "=" + _ID;

                Values = _DataBase.SelectRecord("Select " + String.Join(",", Columns) + " From " + TabeleObject.FullName + Where);
                if (Values.Length > 0)
                {

                    if (!TabeleObject.FullName.EndsWith("CoreObjectAttachment"))
                    {
                        CoreObject _Table = CoreObject.Find(CoreDefine.Entities.جدول, TabeleObject.FullName);
                        FieldsObject = CoreObject.FindChilds(_Table.CoreObjectID, CoreDefine.Entities.فیلد);
                        FieldsObject.RemoveAll(item => item.FullName == "شناسه");
                    }
                    IsHasData = true;
                }
            }
            catch (Exception ex)
            {
                IsHasData = false;
            }
        }

        public Record(ExcelDatabase Engine, string _Query)
        {
            Engine.Open();
            var TheCommand = new OleDbCommand(_Query, Engine.ConnectionData.DBConnection);
            OleDbDataReader TheReader;
            TheCommand.CommandType = System.Data.CommandType.Text;
            IsHasData = false;
            try
            {
                TheReader = TheCommand.ExecuteReader();
                Columns = new string[TheReader.FieldCount];
                Values = new object[TheReader.FieldCount];
                if (TheReader.Read())
                {
                    for (int Index = 0, loopTo = Columns.Length - 1; Index <= loopTo; Index++)
                    {
                        Columns[Index] = TheReader.GetName(Index);
                        Values[Index] = TheReader[Index];
                    }

                    IsHasData = true;
                }

                TheReader.Close();
            }
            catch (Exception ex)
            {

            }
        }
        public object Field(int _Index, object _DefaultValue = null)
        {
            var Output = Values[_Index];
            if (Output is null)
            {
                return _DefaultValue;
            }
            else
            {
                return Output;
            }
        }

        public object Field(string _ColumnName, object _DefaultValue = null)
        {
            object Output = null;
            int FindIndex = Tools.Tools.IsInArrayIndex(_ColumnName, Columns, false, false);
            if (FindIndex >= 0)
                Output = Values[FindIndex];
            if (Output is null | ReferenceEquals(Output, DBNull.Value))
            {
                return _DefaultValue;
            }
            else
            {
                return Output;
            }
        }
    }
}