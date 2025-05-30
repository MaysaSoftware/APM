using APM.Models.Tools;
using APM.Models.APMObject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Data.Common;

namespace APM.Models.Database
{ 
    public class AccessDatabase
    {
        public AccessDatabase()
        {
            this.ConnectionData.Source = string.Empty;
            this.ConnectionData.DataBase = string.Empty;
        }
        public AccessDatabase(string Source, string DataBase,string Password)
        {
            this.ConnectionData.Source = Source;
            this.ConnectionData.DataBase = DataBase;
            this.ConnectionData.Password = Password;
            this.ConnectionData.DBType = DataBase.EndsWith(".accdb") ? CoreDefine.AccessTypes.accdb : CoreDefine.AccessTypes.mdb;
            this.ConnectionData.DBConnection = new OleDbConnection(this.ConnectionData.ConnectionString);
        }

        public ConnectionDataClass ConnectionData = new ConnectionDataClass();
        public partial class ConnectionDataClass
        {
            public string DataBase { get; set; }
            public string Source { get; set; }
            public string Password { get; set; }
            public CoreDefine.AccessTypes DBType { get; set; }

            private bool _IsOpen;

            public OleDbConnection DBConnection { get; set; }

            public string ConnectionString
            {
                get
                {
                    if(string.IsNullOrEmpty(Password))
                    return @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Source + "\\" + DataBase + ";Persist Security Info=False;";
                    else
                    return @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Source + "\\" + DataBase + ";Jet OLEDB:Database Password="+Password+";";
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

        public bool CreateTableWithoutRegitry(string TableName,string[] ColumnName,string[] ColumnType )
        {
            try
            {
                string Query = "CREATE TABLE "+ TableName + "(";
                for(int i = 0; i < ColumnName.Length; i++)
                {
                    Query += "[" + ColumnName[i] + "] " + ColumnType[i] + ",";
                }
                Query+= "CONSTRAINT RegA_PK PRIMARY KEY(["+ColumnName[0]+"]))";
                if(Execute(Query)) 
                    return true; 
                else 
                   return false;
            }
            catch(Exception ex)
            {
                return false;
            }
        } 
        public bool Execute(string _Query, bool _CanShowError = true)
        {
            Open();
            var TheCommand = new OleDbCommand(_Query, ConnectionData.DBConnection);
            TheCommand.CommandType = CommandType.Text;
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
                //// Microsoft Access provider factory
                //DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.OleDb");

                //DataTable userTables = null;
                //using (OleDbConnection connection = new OleDbConnection())
                //{
                //    // c:\test\test.mdb
                //    connection.ConnectionString = ConnectionData.DBConnection.ConnectionString;
                //    // We only want user tables, not system tables
                //    //string[] restrictions = new string[4];
                //    //restrictions[3] = "Table";

                //    connection.Open();

                //    // Get list of user tables
                //    userTables = connection.GetSchema("Columns");
                //}

                //List<string> tableNames = new List<string>();
                //for (int i = 0; i < userTables.Rows.Count; i++)
                //    tableNames.Add(userTables.Rows[i][2].ToString());

                Open();
                OleDbCommand TheCommand = new OleDbCommand();
                TheCommand.Connection = ConnectionData.DBConnection;
                DataTable DataTable = ConnectionData.DBConnection.GetSchema("Columns");
                return DataTable;

            }
            catch (Exception ex)
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
            OleDbCommand TheCommand = new OleDbCommand(_Query, ConnectionData.DBConnection);
            DataTable Data = new DataTable();
            TheCommand.CommandTimeout = 300;
            TheCommand.CommandType = CommandType.Text;

            TheCommand.Parameters.Add("@شناسه_پرسنل", OleDbType.BigInt).Value = Referral.UserAccount.PersonnelID;
            TheCommand.Parameters.Add("@شناسه_کاربر", OleDbType.BigInt).Value = Referral.UserAccount.UsersID;
            TheCommand.Parameters.Add("@شناسه_نقش_کاربر", OleDbType.BigInt).Value = Referral.UserAccount.RoleTypeID;
            TheCommand.Parameters.Add("@شناسه_واحد_سازمانی_پرسنل", OleDbType.BigInt).Value = Referral.UserAccount.PersonnelUnitID;
            TheCommand.Parameters.Add("@شناسه_سمت_سازمانی_پرسنل", OleDbType.BigInt).Value = Referral.UserAccount.PersonnelPostID;
            TheCommand.Parameters.Add("@نقش_کاربر", OleDbType.VarChar).Value = Referral.UserAccount.RoleName.ToString();
            TheCommand.Parameters.Add("@نام_کاربر", OleDbType.VarChar).Value = Referral.UserAccount.UserName.ToString();
            TheCommand.Parameters.Add("@تاریخ_امروز", OleDbType.VarChar).Value = CDateTime.GetNowshamsiDate();


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

        public string DefineVariablesQuery(long TableID, long RowID, string[] ColumnNames, object[] _Values)
        {
            Table Table=new Table(CoreObject.Find(TableID));
            string PrimeryKey = Table.IDField().FieldName;
            string DeclareQuery = "PARAMETERS @"+ PrimeryKey+" Number="+RowID;

            int Counter=0;
            if (Desktop.TableDataFields[TableID.ToString()] == null)
                Desktop.StartupSettingTableDataFields(TableID);

            foreach (Field Item in Desktop.TableDataFields[TableID.ToString()])
            { 
                int FindIndex = Array.IndexOf(ColumnNames, Item.FieldName);
                string Value="";
                if(FindIndex > -1)
                    Value=_Values[FindIndex].ToString();

                if (!Item.IsIdentity && !Item.IsVirtual)
                {
                    DeclareQuery += "@" + Item.FieldName + " ";
                    switch (Item.FieldNature)
                    {
                        case "Bigint":
                            {
                                Value = Value == "" ? "0" : Value;
                                DeclareQuery += "Number = "+ Value ;
                                break;
                            }
                        case "Binary(800)":
                            {
                                Value = Value == "" ? "0" : Value;
                                DeclareQuery += "Binary = " + Value  ;
                                break;
                            }
                        case "Binary(MAX)":
                            {
                                Value = Value == "" ? "0" : Value;
                                DeclareQuery += "Binary = " + Value  ;
                                break;
                            }
                        case "Float":
                            {
                                Value = Value == "" ? "0" : Value;
                                DeclareQuery += "Float = " + Value  ;
                                break;
                            }
                        case "Bit":
                            {
                                Value = Value == "" ? "false" : Value;
                                Value = _Values[FindIndex].ToString() == "true" ? "1" : "0";
                                DeclareQuery += "Bit = " + Value  ;
                                break;
                            }
                        default:
                            {
                                DeclareQuery += "Text = '" + Value + "'"  ;
                                break;
                            }
                    }
                }
                Counter++;
                if(Desktop.TableDataFields[TableID.ToString()].Count ==Counter)
                    DeclareQuery += ";" + Tools.Tools.NewLine;
                else 
                    DeclareQuery += "," + Tools.Tools.NewLine;
            } 
            return DeclareQuery;
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
            string Query = "Insert Into " + TableCore.FullName + " (" + TableParams + ") Values (" + TableValues + ");";
            var TheCommand = new OleDbCommand(Query, ConnectionData.DBConnection);
            OleDbCommand TheCommand2 = new OleDbCommand("Select @@Identity", ConnectionData.DBConnection);
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
                TheCommand.ExecuteReader();
                TheReader = TheCommand2.ExecuteReader();
                if (TheReader.Read())
                {
                    Output = Convert.ToInt32(TheReader[0].ToString());
                }

                TheReader.Close();

                var Rec = new  Record(new AccessDatabase(ConnectionData.Source, ConnectionData.DataBase, ConnectionData.Password), TableCore.CoreObjectID, Output);
                string[] ColumnName = { "RegistryDate", "RegistryTime", "UserAccountID", "TableName", "RecordID", "Value", "IP","ServerName", "DatabaseName", "PCName", "Version", "BrowserType", "BrowserVersion" };
                object[] Values = new object[] { CDateTime.GetNowshamsiDate(), CDateTime.GetNowTime(), Referral.UserAccount.UsersID, TableCore.FullName, Output, Tools.Tools.ToXML(new RecordData(Rec)), Referral.UserAccount.IP ,ConnectionData.Source, ConnectionData.DataBase, Referral.UserAccount.PCName, Referral.AppVersion, Referral.UserAccount.BrowserType, Referral.UserAccount.BrowserVersion };
                Referral.DBData.Insert("Insert_APMRegistry", ColumnName, Values);


                if (TableCore.CoreObjectID > 0 && Output > 0)
                {

                    List<CoreObject> TableEventList = CoreObject.FindChilds(TableCore.CoreObjectID, CoreDefine.Entities.رویداد_جدول);

                    foreach (CoreObject ParameterCore in TableEventList)
                    {
                        TableEvent Event = new TableEvent(ParameterCore);
                        if (Event.EventType == CoreDefine.TableEvents.دستور_بعد_از_درج.ToString())
                        {
                            Execute(DefineVariablesQuery(TableCore.CoreObjectID, Output, ColumnNames, _Values) + "\n" + Tools.Tools.CheckQuery(Event.Query));
                            if (Event.RelatedTable > 0)
                            {
                                CoreObject TableName = CoreObject.Find(Event.RelatedTable);
                                string[] ColumnNameR = { "RegistryDate", "RegistryTime", "UserAccountID", "TableName", "IP", "ServerName", "DatabaseName", "PCName", "Version", "BrowserType", "BrowserVersion" };
                                object[] ValuesR = new object[] { CDateTime.GetNowshamsiDate(), CDateTime.GetNowTime(), Referral.UserAccount.UsersID, TableName.FullName, Referral.UserAccount.IP, ConnectionData.Source, ConnectionData.DataBase, Referral.UserAccount.PCName, Referral.AppVersion, Referral.UserAccount.BrowserType, Referral.UserAccount.BrowserVersion };
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
             
            Table Table=new Table(CoreObject.Find(TableID));

            for (int Index = 0; Index < TableColumnCount; Index++)
            {
                if (Index > 0)
                    TableParams += ",";
                TableParams += ColumnNames[Index] + " = @P" + Index;
            }
            string Whare = " Where ";

            Whare += Table.IDField().FieldName + " = " + _RowID;

            string Query = "Update " + Table.FullName + " Set " + TableParams + Whare;
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
                object[] PreviousValues = SelectRecord("Select " + String.Join(",", ColumnNames) + " From " + Table.FullName + Whare);
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
                        Referral.DBData.Insert("Update_APMRegistry", new string[] { "RegistryDate", "RegistryTime", "UserAccountID", "TableName", "RecordID", "PreviousValue", "NewValue", "IP", "ServerName", "DatabaseName", "PCName", "Version", "BrowserType", "BrowserVersion" },
                                                              new object[] { CDateTime.GetNowshamsiDate(), CDateTime.GetNowTime(), Referral.UserAccount.UsersID, Table.FullName, _RowID, PreviousValues[Index], ParameterValue, Referral.UserAccount.IP, ConnectionData.Source, ConnectionData.DataBase, Referral.UserAccount.PCName, Referral.AppVersion, Referral.UserAccount.BrowserType, Referral.UserAccount.BrowserVersion });
                    }
                }


                if (Table.CoreObjectID > 0 && Output)
                {

                    List<CoreObject> TableEventList = CoreObject.FindChilds(Table.CoreObjectID, CoreDefine.Entities.رویداد_جدول);

                    foreach (CoreObject ParameterCore in TableEventList)
                    {
                        TableEvent Event = new TableEvent(ParameterCore);
                        if (Event.EventType == CoreDefine.TableEvents.دستور_بعد_از_ویرایش.ToString())
                        {
                            Execute(DefineVariablesQuery(Table.CoreObjectID, _RowID, ColumnNames, _Values) + "\n" + Tools.Tools.CheckQuery(Event.Query));
                            if (Event.RelatedTable > 0)
                            {
                                CoreObject TableName = CoreObject.Find(Event.RelatedTable);
                                string[] ColumnName = { "RegistryDate", "RegistryTime", "UserAccountID", "TableName", "IP", "ServerName", "DatabaseName", "PCName", "Version", "BrowserType", "BrowserVersion" };
                                object[] Values = new object[] { CDateTime.GetNowshamsiDate(), CDateTime.GetNowTime(), Referral.UserAccount.UsersID, TableName.FullName, Referral.UserAccount.IP, ConnectionData.Source, ConnectionData.DataBase, Referral.UserAccount.PCName, Referral.AppVersion, Referral.UserAccount.BrowserType, Referral.UserAccount.BrowserVersion };
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
            Table Table=new Table(CoreObject.Find(TableID));
            var Rec = new Record(new AccessDatabase(ConnectionData.Source, ConnectionData.DataBase, ConnectionData.Password), Table.CoreObjectID, _ID); 

            string[] ColumnName = { "RegistryDate", "RegistryTime", "UserAccountID", "TableName", "RecordID", "Value", "IP", "ServerName", "DatabaseName", "PCName", "Version", "BrowserType", "BrowserVersion" };
            object[] Values = new object[] { CDateTime.GetNowshamsiDate(), CDateTime.GetNowTime(), Referral.UserAccount.UsersID, Table.FullName, _ID, Tools.Tools.ToXML(new RecordData(Rec)), Referral.UserAccount.IP, ConnectionData.Source, ConnectionData.DataBase, Referral.UserAccount.PCName, Referral.AppVersion, Referral.UserAccount.BrowserType, Referral.UserAccount.BrowserVersion };
            Referral.DBData.Insert("Delete_APMRegistry", ColumnName, Values);

            string Whare = " Where ";

            Whare += Table.IDField().FieldName + " = " + _ID;

            Execute("Delete From " + Table.FullName + Whare);
            return true;
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