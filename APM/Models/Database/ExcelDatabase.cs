using APM.Models.Tools;
using APM.Models.APMObject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;

namespace APM.Models.Database
{ 
    public class ExcelDatabase
    {
        public ExcelDatabase()
        {
            this.ConnectionData.Source = string.Empty; 
            this.ConnectionData.DataBase = string.Empty;
        }
        public ExcelDatabase(string Source, string DataBase)
        {
            this.ConnectionData.Source = Source; 
            this.ConnectionData.DataBase = DataBase;
            this.ConnectionData.DBType=DataBase.EndsWith(".xlsx")?CoreDefine.ExcelTypes.xlsx:CoreDefine.ExcelTypes.xls;
            this.ConnectionData.DBConnection = new OleDbConnection(this.ConnectionData.ConnectionString);
        }

        public ConnectionDataClass ConnectionData = new ConnectionDataClass();
        public partial class ConnectionDataClass
        { 
            public string DataBase { get; set; }
            public string Source { get; set; }
            public CoreDefine.ExcelTypes DBType { get; set; }

            private bool _IsOpen;

            public OleDbConnection DBConnection { get; set; }

            public string ConnectionString
            {
                get
                {
                    return @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Source + "\\" + DataBase + ";" + @"Extended Properties=""Excel 12.0 Xml;HDR=Yes;""";
                }
            }


            public bool IsOpen
            {
                get
                {
                    return _IsOpen;
                }

                set
                {
                    _IsOpen = false;
                    if (value)
                    {
                        try
                        {
                            if (DBConnection.State != ConnectionState.Open)
                            {
                                DBConnection.Open();
                            }

                            _IsOpen = true;
                        }
                        catch (Exception ex)
                        {
                            _IsOpen = false;
                        }
                    }
                    else if (DBConnection.State != ConnectionState.Closed)
                    {
                        DBConnection.Close();
                    }
                }
            }



        }

        public bool Open()
        {
            ConnectionData.IsOpen = true;
            return ConnectionData.IsOpen;
        }

        public void Close()
        {
            ConnectionData.IsOpen = false;
        }

        public bool Execute(string _Query, bool _CanShowError = true)
        {
            Open();
            var TheCommand = new OleDbCommand(_Query, ConnectionData.DBConnection);
            TheCommand.CommandType =CommandType.Text;
            try
            {
                TheCommand.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        
        public DataTable GetSchemaTable()
        {
            try
            {
                Open();
                OleDbCommand TheCommand = new OleDbCommand();
                TheCommand.Connection = ConnectionData.DBConnection;
                DataTable DataTable = ConnectionData.DBConnection.GetSchema("Tables");
                return DataTable;

            }
            catch(Exception ex)
            {
                return new DataTable();
            }
        }

        public object SelectField(string _Query, object _DefaultValue = null)
        {
            Open();
            var TheCommand = new OleDbCommand(_Query, ConnectionData.DBConnection);
            OleDbDataReader TheReader;
            object Output = null;

            try
            {
                TheReader = TheCommand.ExecuteReader();
                if (TheReader.Read())
                {
                    Output = TheReader[0];
                }

                TheReader.Close();
            }
            catch (Exception ex)
            {
            }

            if ((Output is DBNull | Output is null) & _DefaultValue is object)
            {
                Output = _DefaultValue;
            }

            return Output;
        }

 
        public object[] SelectRecord(string _Query)
        {
            Open();
            var TheCommand = new OleDbCommand(_Query, ConnectionData.DBConnection);
            OleDbDataReader TheReader;
            TheCommand.CommandType = CommandType.Text;
            var Output = Array.Empty<object>();
            try
            {
                TheReader = TheCommand.ExecuteReader();
                if (TheReader.Read())
                {
                    Output = new object[TheReader.FieldCount];
                    for (int Index = 0, loopTo = TheReader.FieldCount - 1; Index <= loopTo; Index++)
                        Output[Index] = TheReader[Index];
                }

                TheReader.Close();
            }
            catch (Exception ex)
            {
            }

            return Output;
        }

        public object[] SelectColumn(string _Query)
        {
            Open();
            var TheCommand = new OleDbCommand(_Query, ConnectionData.DBConnection);
            OleDbDataReader TheReader;
            var Output = new object[0];
            try
            {
                TheReader = TheCommand.ExecuteReader();
                int Index = 0;
                while (TheReader.Read())
                {
                    Array.Resize(ref Output, Index + 1);
                    Output[Index] = TheReader[0];
                    Index += 1;
                }

                TheReader.Close();
            }
            catch (Exception ex)
            {
            }

            return Output;
        }

        public DataTable SelectDataTable(string _Query)
        {
            Open();
            var TheCommand = new OleDbCommand(_Query, ConnectionData.DBConnection);
            DataTable Data = new DataTable();
            TheCommand.CommandTimeout = 300;
            TheCommand.CommandType = CommandType.Text;
            try
            {
                OleDbDataAdapter DataAdapter = new OleDbDataAdapter(TheCommand);
                DataAdapter.Fill(Data);

                return Data;
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        public string DefineVariablesQuery(string _TableName, long RowID, string[] ColumnNames, object[] _Values)
        {
            string DeclareQuery = "Declare @شناسه as Bigint=" + RowID.ToString() + Tools.Tools.NewLine;

            DataTable ColumnData = GetAllColumn(_TableName);

            foreach (DataRow Row in ColumnData.Rows)
            {
                if (Row["COLUMN_NAME"].ToString() != "شناسه")
                {
                    int IndexFind = Array.IndexOf(ColumnNames, Row["COLUMN_NAME"].ToString());
                    if (IndexFind != -1)
                    {
                        DeclareQuery += "DECLARE @" + Row["COLUMN_NAME"].ToString() + " as " + (Row["DATA_TYPE"].ToString() == "nvarchar" ? "NVARCHAR(" + Row["CHARACTER_MAXIMUM_LENGTH"].ToString() + ")" : Row["DATA_TYPE"].ToString()) + " = ";

                        if (Row["DATA_TYPE"].ToString() == "nvarchar")
                            DeclareQuery += "N'" + _Values[IndexFind].ToString() + "'";
                        else if (_Values[IndexFind].ToString() == "")
                            DeclareQuery += "null";
                        else if (Row["DATA_TYPE"].ToString() == "bit")
                            DeclareQuery += _Values[IndexFind].ToString() == "true" ? 1 : 0;
                        else
                            DeclareQuery += _Values[IndexFind].ToString() == "" ? "null" : _Values[IndexFind].ToString();

                        DeclareQuery += Tools.Tools.NewLine;
                    }
                }
            }

            return DeclareQuery;
        }

        public string GetIdentityTable(long TableID)
        {
            return "";
        }

        public int Insert(long TableID, string[] ColumnNames, object[] _Values)
        {
            Open();
            int TableColumnCount = ColumnNames.Length;
            int TableValuesCount = _Values.Length;
            string TableParams = "";
            string TableValues = "";

            CoreObject TableCore = CoreObject.Find(TableID);

            for (int Index = 0, loopTo = TableColumnCount - 1; Index <= loopTo; Index++)
            {
                if (Index > 0)
                {
                    TableParams += ",";
                    TableValues += ",";
                }
                TableParams += ColumnNames[Index];
                TableValues += "@P" + Index;
            }

            OleDbDataReader TheReader;
            string Query = "Insert Into " + TableCore.FullName + " (" + TableParams + ") Values (" + TableValues + "); Select @@Identity";
            var TheCommand = new OleDbCommand(Query, ConnectionData.DBConnection);
            TheCommand.CommandType = CommandType.Text;

            for (int Index = 0; Index < TableColumnCount; Index++)
            {
                object ParameterValue;
                string ValueType = "";
                if (_Values[Index] != null)
                {
                    ValueType = _Values[Index].GetType().Name;
                    if (ValueType == "Object[]" || ValueType == "String[]")
                    {
                        object[] _val = (object[])_Values[Index];
                        ParameterValue = _val[0];
                    }
                    else
                        ParameterValue = _Values[Index] is DBNull ? "" : _Values[Index];

                }
                else
                {
                    ParameterValue = "";
                }
                if (ValueType == "Byte[]")
                {
                    byte[] sr = (byte[])ParameterValue;
                    TheCommand.Parameters.Add("@P" + Index, OleDbType.VarBinary, -1).Value = sr;

                }
                else
                    TheCommand.Parameters.AddWithValue("@P" + Index, ParameterValue);
            }

            try
            {
                int Output = 0;
                TheReader = TheCommand.ExecuteReader();
                if (TheReader.Read())
                {
                    Output = Convert.ToInt32(TheReader[0].ToString());

                }

                TheReader.Close();

                var Rec = new Record(Referral.DBData, TableCore.FullName, Output);
                string[] ColumnName = { "RegistryDate", "RegistryTime", "UserAccountID", "TableName", "RecordID", "Value", "IP" };
                object[] Values = new object[] { CDateTime.GetNowshamsiDate(), CDateTime.GetNowTime(), Referral.UserAccount.UsersID, TableCore.FullName, Output, Tools.Tools.ToXML(new RecordData(Rec)), Referral.UserAccount.IP };
                Referral.DBData.Insert("Insert_APMRegistry", ColumnName, Values);


                if (TableCore.CoreObjectID > 0 && Output > 0)
                {

                    List<CoreObject> TableEventList = CoreObject.FindChilds(TableCore.CoreObjectID, CoreDefine.Entities.رویداد_جدول);


                    foreach (CoreObject ParameterCore in TableEventList)
                    {
                        TableEvent Event = new TableEvent(ParameterCore);
                        if (Event.EventType == CoreDefine.TableEvents.دستور_بعد_از_درج.ToString())
                        {
                            Execute(DefineVariablesQuery(TableCore.FullName, Output, ColumnNames, _Values) + "\n" + Tools.Tools.CheckQuery(Event.Query));
                            if (Event.RelatedTable > 0)
                            {
                                CoreObject TableName = CoreObject.Find(Event.RelatedTable);
                                string[] ColumnNameR = { "RegistryDate", "RegistryTime", "UserAccountID", "TableName", "IP" };
                                object[] ValuesR = new object[] { CDateTime.GetNowshamsiDate(), CDateTime.GetNowTime(), Referral.UserAccount.UsersID, TableName.FullName, Referral.UserAccount.IP };
                                Referral.DBData.Insert("Insert_APMRegistry", ColumnNameR, ValuesR);
                            }
                        }
                    }

                }

                return Output;
            }
            catch (Exception ex)
            {
            }

            return default;
        }

        public bool UpdateRow(long _RowID, long TableID, string[] ColumnNames, object[] _Values)
        {
            Open();

            int TableColumnCount = ColumnNames.Length;
            int TableValuesCount = _Values.Length;
            string TableParams = "";

            CoreObject TableCore = CoreObject.Find(TableID);

            for (int Index = 0; Index < TableColumnCount; Index++)
            {
                if (Index > 0)
                    TableParams += ",";
                TableParams += ColumnNames[Index] + " = @P" + Index;
            }
            string Whare = " Where ";

            Whare += GetIdentityTable(TableID) + " = " + _RowID;

            string Query = "Update " + TableCore.FullName + " Set " + TableParams + Whare;
            var TheCommand = new OleDbCommand(Query, ConnectionData.DBConnection);
            TheCommand.CommandType = CommandType.Text;
            for (int Index = 0; Index < TableColumnCount; Index++)
            {
                object ParameterValue;
                if (_Values[Index] is null)
                {
                    ParameterValue = "";
                }
                else
                {
                    string ValueType = _Values[Index].GetType().Name;
                    if (ValueType == "Object[]" || ValueType == "String[]")
                    {
                        object[] _val = (object[])_Values[Index];
                        ParameterValue = _val[0];
                    }
                    else
                        ParameterValue = _Values[Index] is DBNull ? "" : _Values[Index];
                }

                TheCommand.Parameters.AddWithValue("@P" + Index, ParameterValue);
            }
            try
            {
                bool Output = false;
                object[] PreviousValues = SelectRecord("Select " + String.Join(",", ColumnNames) + " From " + TableCore.FullName + Whare);
                TheCommand.ExecuteNonQuery();
                Output = true;
                for (int Index = 0; Index < ColumnNames.Length; Index++)
                {
                    object ParameterValue;
                    if (_Values[Index] is null)
                    {
                        ParameterValue = "";
                    }
                    else
                    {
                        string ValueType = _Values[Index].GetType().Name;
                        if (ValueType == "Object[]" || ValueType == "String[]")
                        {
                            object[] _val = (object[])_Values[Index];
                            ParameterValue = _val[0];
                        }
                        else
                            ParameterValue = _Values[Index] is DBNull ? "" : _Values[Index];
                    }
                    if (IsDifference(PreviousValues[Index], ParameterValue))
                    {
                        Referral.DBData.Insert("Update_APMRegistry", new string[] { "RegistryDate", "RegistryTime", "UserAccountID", "TableName", "RecordID", "PreviousValue", "NewValue", "IP" },
                                                              new object[] { CDateTime.GetNowshamsiDate(), CDateTime.GetNowTime(), Referral.UserAccount.UsersID, TableCore.FullName, _RowID, PreviousValues[Index], ParameterValue, Referral.UserAccount.IP });
                    }
                }


                if (TableCore.CoreObjectID > 0 && Output)
                {

                    List<CoreObject> TableEventList = CoreObject.FindChilds(TableCore.CoreObjectID, CoreDefine.Entities.رویداد_جدول);

                    foreach (CoreObject ParameterCore in TableEventList)
                    {
                        TableEvent Event = new TableEvent(ParameterCore);
                        if (Event.EventType == CoreDefine.TableEvents.دستور_بعد_از_ویرایش.ToString())
                        {
                            Execute(DefineVariablesQuery(TableCore.FullName, _RowID, ColumnNames, _Values) + "\n" + Tools.Tools.CheckQuery(Event.Query));
                            if (Event.RelatedTable > 0)
                            {
                                CoreObject TableName = CoreObject.Find(Event.RelatedTable);
                                string[] ColumnName = { "RegistryDate", "RegistryTime", "UserAccountID", "TableName", "IP" };
                                object[] Values = new object[] { CDateTime.GetNowshamsiDate(), CDateTime.GetNowTime(), Referral.UserAccount.UsersID, TableName.FullName, Referral.UserAccount.IP };
                                Referral.DBData.Insert("Insert_APMRegistry", ColumnName, Values);
                            }
                        }
                    }

                }

                return Output;
            }
            catch (Exception ex)
            {
            }

            return default;
        }

        public bool Delete(long TableID, long _ID)
        {
            CoreObject TableCore = CoreObject.Find(TableID);
            var Rec = new Record(Referral.DBData, TableCore.FullName, _ID, TableCore.CoreObjectID);

            string[] ColumnName = { "RegistryDate", "RegistryTime", "UserAccountID", "TableName", "RecordID", "Value", "IP" };
            object[] Values = new object[] { CDateTime.GetNowshamsiDate(), CDateTime.GetNowTime(), Referral.UserAccount.UsersID, TableCore.FullName, _ID, Tools.Tools.ToXML(new RecordData(Rec)), Referral.UserAccount.IP };
            Referral.DBData.Insert("Delete_APMRegistry", ColumnName, Values);

            string Whare = " Where ";

            Whare += GetIdentityTable(TableID) + " = " + _ID;

            Execute("Delete " + TableCore.FullName + Whare);
            return true;
        }


        public string[] Columns(string _ObjectName, bool _IsOnlyNormalColumns = false)
        {
            var Output = new object[0];
            string QueryWhere = "";
            //switch (ConnectionData.Version)
            //{
            //    case var @case when @case == SQLVersions.SQL2000:
            //        {
            //            if (_IsOnlyNormalColumns)
            //                QueryWhere = "And colstat = 0";
            //            Output = SelectColumn("Select syscolumns.name From sysobjects inner join syscolumns on sysobjects.id = syscolumns.id Where sysobjects.name = '" + _ObjectName + "' " + QueryWhere + " order by syscolumns.colid");
            //            break;
            //        }

            //    case var case1 when case1 == SQLVersions.SQL2008:
            //        {
            if (_IsOnlyNormalColumns)
                QueryWhere = "And is_identity = 0 And is_computed = 0";
            Output = SelectColumn("Select sys.all_columns.name From sys.all_objects inner join sys.all_columns on sys.all_objects.object_id = sys.all_columns.object_id Where sys.all_objects.name = N'" + _ObjectName + "' " + QueryWhere + " order by sys.all_columns.column_id");
            //            break;
            //        }
            //}

            return Tools.Tools.VString(Output);
        }

        public DataTable GetAllColumn(string TableName)
        {
            string Query = "SELECT COLUMN_NAME,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'" + TableName + "'";
            return SelectDataTable(Query);
        }
        public string GetDatabasePath()
        {
            string Query = "select physical_name from sys.database_files where type = 0";
            string FilePath = SelectField(Query).ToString();
            return FilePath;
        }

        public static bool IsDifference(object _PreviousValue, object _NewValue)
        {
            if (_PreviousValue is null)
            {
                if (_NewValue is null)
                {
                    return false;
                }
            }
            else if (_NewValue is string)
            {
                if (ReferenceEquals(_PreviousValue.GetType(), _NewValue.GetType()))
                {
                    if (_NewValue == _PreviousValue)
                    {
                        return false;
                    }
                }
                else if (_PreviousValue is long)
                {
                    if (_PreviousValue == _NewValue)
                    {
                        return false;
                    }
                }
            }
            //else if (Information.IsNumeric(_NewValue))
            //{
            //    if (Information.IsNumeric(_PreviousValue))
            //    {
            //        if (Operators.ConditionalCompareObjectEqual(_NewValue, _PreviousValue, false))
            //        {
            //            return false;
            //        }
            //    }
            //    else if (_PreviousValue is string)
            //    {
            //        if (_PreviousValue== _NewValue.ToString())
            //        {
            //            return false;
            //        }
            //    }
            //}
            else if (_NewValue is DBNull)
            {
                if (ReferenceEquals(_PreviousValue.GetType(), _NewValue.GetType()))
                {
                    return false;
                }
            }

            return true;
        }

    }
}