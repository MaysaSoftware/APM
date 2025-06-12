
using APM.Models.APMObject;
using APM.Models.Database;
using APM.Models.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

namespace APM.Models.Tools
{
    public class SQLDataBase
    {
        public SQLDataBase()
        {
            this.ConnectionData.Source = string.Empty;
            this.ConnectionData.DBUser = string.Empty;
            this.ConnectionData.DBPassword = string.Empty;
            this.ConnectionData.DataBase = string.Empty;
            this.ConnectionData.Version = SQLVersions.SQL2008;
        }
        public SQLDataBase(string Source, string DataBase, string DBPassword, string DBUser, SQLVersions versions)
        {
            this.ConnectionData.Source = Source;
            this.ConnectionData.DBUser = DBUser;
            this.ConnectionData.DBPassword = DBPassword;
            this.ConnectionData.DataBase = DataBase;
            this.ConnectionData.Version = versions;
            this.ConnectionData.DBConnection = new SqlConnection(this.ConnectionData.ConnectionString);
        }

        public enum SQLVersions
        {
            SQL2000,
            SQL2008
        }


        public partial class ConnectionDataClass
        {
            public string DBUser;
            public string DBPassword;
            public string DataBase;
            public string Source;
            public SQLVersions Version;
            private bool _IsOpen;

            public SqlConnection DBConnection { get; set; }

            public string ConnectionString
            {
                get
                {
                     return $"Server={Source};Database={DataBase};User Id={DBUser};Password={DBPassword};Integrated Security=False;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"; 
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

        public ConnectionDataClass ConnectionData = new ConnectionDataClass();
        public bool Open()
        {
            Log.LogFunction("DataBase.Open", true);
            try
            {
                //ConnectionData.DBConnection.Open();
                ConnectionData.IsOpen = true;
                return ConnectionData.IsOpen;
            }
            catch (Exception ex)
            {
                Log.Error("DataBase.Open", ex.Message);
                return false;
            }
        }

        public void Close()
        {
            ConnectionData.IsOpen = false;
            //ConnectionData.DBConnection.Close();
        }

        public bool Execute(string _Query, bool _CanShowError = true)
        {
            Log.LogFunction("DataBase.Execute");

            bool Output = false;
            using (SqlConnection DBConnection = new SqlConnection(ConnectionData.ConnectionString))
            {
                DBConnection.Open();
                using (SqlCommand TheCommand = new SqlCommand(_Query, DBConnection))
                {
                    TheCommand.CommandTimeout = 300;
                    TheCommand.CommandType = System.Data.CommandType.Text;
                    try
                    {
                        TheCommand.ExecuteNonQuery();
                        Output = true;
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Database.Execute", "Query : \n" + _Query + "\n\n" + ex.Message);
                        Output = false;
                    }

                }
                DBConnection.Close();
            }
            Log.LogFunction("DataBase.Execute", false);
            return Output;

        }

        public async Task<bool> ExecuteAsync(string query, bool canShowError = true)
        {
            Log.LogFunction("DataBase.ExecuteAsync");

            bool output = false;

            try
            {
                using (SqlConnection dbConnection = new SqlConnection(ConnectionData.ConnectionString))
                {
                    await dbConnection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(query, dbConnection))
                    {
                        command.CommandTimeout = 300;
                        command.CommandType = CommandType.Text;

                        try
                        {
                            await command.ExecuteNonQueryAsync();
                            output = true;
                        }
                        catch (Exception ex)
                        {
                            if (canShowError)
                            {
                                Log.Error("DataBase.ExecuteAsync", $"Query:\n{query}\n\n{ex.Message}");
                            }
                            output = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (canShowError)
                {
                    Log.Error("DataBase.ExecuteAsync", $"Connection Error:\n{query}\n\n{ex.Message}");
                }
                output = false;
            }

            Log.LogFunction("DataBase.ExecuteAsync", false);
            return output;
        }


        public string GetPrimeryKey(string TableName)
        {
            string Query = "SELECT name FROM sys.key_constraints WHERE type = 'PK' and parent_object_id = (SELECT OBJECT_ID(N'" + TableName + "') )";
            object Result = SelectField(Query);
            return Result == null ? "" : Result.ToString();
        }

        public async Task<string> GetPrimeryKeyAsync(string TableName)
        {
            string Query = "SELECT name FROM sys.key_constraints WHERE type = 'PK' and parent_object_id = (SELECT OBJECT_ID(N'" + TableName + "') )";
            object Result = await SelectFieldAsync(Query);
            return Result == null ? "" : Result.ToString();
        }

        public async Task<bool> SetPrimeryKey(string TableName, string FieldName)
        {
            string Query = "ALTER TABLE " + TableName + "  ADD CONSTRAINT PK_" + TableName + "HistoryArchive_" + FieldName + " PRIMARY KEY CLUSTERED([" + FieldName + "]); ";
            if (await ExecuteAsync(Query))
                return true;
            else
                return false;
        }
        public bool RemovePrimeryKey(string TableName)
        {
            string Query = "ALTER TABLE " + TableName + " DROP CONSTRAINT " + GetPrimeryKey(TableName) + "; ";
            if (Execute(Query))
                return true;
            else
                return false;
        }
        public async Task<bool> RemovePrimeryKeyAsync(string TableName)
        {
            string Query = "ALTER TABLE " + TableName + " DROP CONSTRAINT " + await GetPrimeryKeyAsync(TableName) + "; ";
            if (await ExecuteAsync(Query))
                return true;
            else
                return false;
        }

        public object SelectField(string _Query, object _DefaultValue = null)
        {
            Log.LogFunction("DataBase.SelectField");
            object Output = null;

            using (SqlConnection DBConnection = new SqlConnection(ConnectionData.ConnectionString))
            {
                DBConnection.Open();
                using (SqlCommand TheCommand = new SqlCommand(_Query, DBConnection))
                {
                    TheCommand.CommandTimeout = 300;

                    try
                    {
                        using (SqlDataReader TheReader = TheCommand.ExecuteReader())
                        {
                            if (TheReader.Read())
                                Output = TheReader[0];

                            TheReader.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Database.SelectField", "Query : \n" + _Query + "\n\n" + ex.Message);
                    }

                    if ((Output is DBNull | Output is null) & _DefaultValue is object)
                    {
                        Output = _DefaultValue;
                    }

                }
                DBConnection.Close();

            }

            Log.LogFunction("DataBase.SelectField", false);
            return Output == null ? "" : Output;

        }

        public async Task<object> SelectFieldAsync(string query, object defaultValue = null)
        {
            Log.LogFunction("DataBase.SelectFieldAsync");

            object output = null;

            try
            {
                using (var dbConnection = new SqlConnection(ConnectionData.ConnectionString))
                {
                    await dbConnection.OpenAsync();

                    using (var command = new SqlCommand(query, dbConnection))
                    {
                        command.CommandTimeout = 300;

                        try
                        {
                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    output = reader[0];
                                }
                                // No need to call reader.CloseAsync() here
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error("DataBase.SelectFieldAsync", $"Query:\n{query}\n\n{ex.Message}");
                        }

                        if ((output is DBNull || output is null) && defaultValue is object)
                        {
                            output = defaultValue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("DataBase.SelectFieldAsync", $"Connection Error:\n{query}\n\n{ex.Message}");
            }

            Log.LogFunction("DataBase.SelectFieldAsync", false);
            return output ?? "";
        }

        public object[] SelectRecord(string _Query)
        {
            Log.LogFunction("DataBase.SelectRecord");
            var Output = Array.Empty<object>();

            using (SqlConnection DBConnection = new SqlConnection(ConnectionData.ConnectionString))
            {
                DBConnection.Open();
                using (SqlCommand TheCommand = new SqlCommand(_Query, DBConnection))
                {
                    TheCommand.CommandTimeout = 300;
                    TheCommand.CommandType = CommandType.Text;

                    try
                    {
                        using (SqlDataReader TheReader = TheCommand.ExecuteReader())
                        {

                            if (TheReader.Read())
                            {
                                Output = new object[TheReader.FieldCount];
                                for (int Index = 0, loopTo = TheReader.FieldCount - 1; Index <= loopTo; Index++)
                                    Output[Index] = TheReader[Index];
                            }

                            TheReader.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Database.SelectRecord", "Query : \n" + _Query + "\n\n" + ex.Message);
                    }


                }
                DBConnection.Close();

            }

            Log.LogFunction("DataBase.SelectRecord", false);
            return Output;
        }

        public async Task<object[]> SelectRecordAsync(string query)
        {
            Log.LogFunction("DataBase.SelectRecordAsync");

            object[] output = Array.Empty<object>();

            try
            {
                using (var dbConnection = new SqlConnection(ConnectionData.ConnectionString))
                {
                    await dbConnection.OpenAsync();

                    using (var command = new SqlCommand(query, dbConnection))
                    {
                        command.CommandTimeout = 300;
                        command.CommandType = CommandType.Text;

                        try
                        {
                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    output = new object[reader.FieldCount];
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        output[i] = reader[i];
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error("DataBase.SelectRecordAsync", $"Query:\n{query}\n\n{ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("DataBase.SelectRecordAsync", $"Connection Error:\n{query}\n\n{ex.Message}");
            }

            Log.LogFunction("DataBase.SelectRecordAsync", false);
            return output;
        }


        public object[] SelectColumn(string _Query)
        {
            Log.LogFunction("DataBase.SelectColumn");
            var Output = new object[0];
            using (SqlConnection DBConnection = new SqlConnection(ConnectionData.ConnectionString))
            {
                DBConnection.Open();
                using (SqlCommand TheCommand = new SqlCommand(_Query, DBConnection))
                {
                    TheCommand.CommandTimeout = 300;

                    try
                    {
                        using (SqlDataReader TheReader = TheCommand.ExecuteReader())
                        {
                            int Index = 0;
                            while (TheReader.Read())
                            {
                                Array.Resize(ref Output, Index + 1);
                                Output[Index] = TheReader[0];
                                Index += 1;
                            }

                            TheReader.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Database.SelectColumn", "Query : \n" + _Query + "\n\n" + ex.Message);
                    }


                }
                DBConnection.Close();

            }
            Log.LogFunction("DataBase.SelectColumn", false);
            return Output;
        }

        public async Task<object[]> SelectColumnAsync(string query)
        {
            Log.LogFunction("DataBase.SelectColumnAsync");

            var output = new List<object>();

            try
            {
                using (var dbConnection = new SqlConnection(ConnectionData.ConnectionString))
                {
                    await dbConnection.OpenAsync();

                    using (var command = new SqlCommand(query, dbConnection))
                    {
                        command.CommandTimeout = 300;

                        try
                        {
                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    output.Add(reader[0]);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error("DataBase.SelectColumnAsync", $"Query:\n{query}\n\n{ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("DataBase.SelectColumnAsync", $"Connection Error:\n{query}\n\n{ex.Message}");
            }

            Log.LogFunction("DataBase.SelectColumnAsync", false);
            return output.ToArray();
        }


        public DataTable GetAllTableName()
        {
            string Query = "SELECT  TABLE_SCHEMA,TABLE_NAME   FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
            return SelectDataTable(Query);
        }

        public async Task<DataTable> GetAllTableNameAsync()
        {
            string Query = "SELECT  TABLE_SCHEMA,TABLE_NAME   FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
            return await SelectDataTableAsync(Query);
        }

        public DataTable SelectDataTable(string _Query)
        {
            Log.LogFunction("DataBase.SelectDataTable", true);
            DataTable Data = new DataTable();

            using (SqlConnection DBConnection = new SqlConnection(ConnectionData.ConnectionString))
            {
                DBConnection.Open();
                using (SqlCommand TheCommand = new SqlCommand(_Query, DBConnection))
                {
                    TheCommand.CommandTimeout = 300;
                    TheCommand.CommandType = CommandType.Text;

                    try
                    {
                        SqlDataAdapter DataAdapter = new SqlDataAdapter(TheCommand);

                        DataAdapter.Fill(Data);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("DataBase.SelectDataTable", "Query : \n" + _Query + "\n\n" + ex.Message);
                    }
                }
                DBConnection.Close();

            }

            Log.LogFunction("DataBase.SelectDataTable", false);
            return Data;
        }

        public async Task<DataTable> SelectDataTableAsync(string query)
        {
            Log.LogFunction("DataBase.SelectDataTableAsync", true);
            var data = new DataTable();

            try
            {
                using (var dbConnection = new SqlConnection(ConnectionData.ConnectionString))
                {
                    await dbConnection.OpenAsync();

                    using (var command = new SqlCommand(query, dbConnection))
                    {
                        command.CommandTimeout = 300;
                        command.CommandType = CommandType.Text;

                        try
                        {
                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                data.Load(reader);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error("DataBase.SelectDataTableAsync", $"Query:\n{query}\n\n{ex}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("DataBase.SelectDataTableAsync", $"General Error:\n{ex}");
            }

            Log.LogFunction("DataBase.SelectDataTableAsync", false);
            return data;
        }


        public string DefineVariablesQuery(string _TableName, long RowID, string[] ColumnNames, object[] _Values, string PrefixStr = "")
        {
            string IdentityField = GetIdentityTable(_TableName);
            string DeclareQuery = "Declare @" + (PrefixStr != "" ? PrefixStr + "_" : "") + IdentityField + " as Bigint=" + RowID.ToString() + Tools.NewLine;

            DataTable ColumnData = GetAllColumn(_TableName);

            foreach (DataRow Row in ColumnData.Rows)
            {
                if (Row["COLUMN_NAME"].ToString() != IdentityField)
                {
                    int IndexFind = Array.IndexOf(ColumnNames, Row["COLUMN_NAME"].ToString());
                    DeclareQuery += "DECLARE @" + (PrefixStr != "" ? PrefixStr + "_" : "") + Row["COLUMN_NAME"].ToString() + " as " + (Row["DATA_TYPE"].ToString() == "nvarchar" ? "NVARCHAR(" + (Row["CHARACTER_MAXIMUM_LENGTH"].ToString() == "-1" ? "MAX" : Row["CHARACTER_MAXIMUM_LENGTH"].ToString()) + ")" : Row["DATA_TYPE"].ToString()) + " = ";

                    if (IndexFind != -1)
                    {
                        if (Row["DATA_TYPE"].ToString() == "nvarchar")
                        {
                            if (_Values[IndexFind] == null)
                                DeclareQuery += "null";
                            else
                                DeclareQuery += "N'" + _Values[IndexFind].ToString() + "'";
                        }
                        else if (_Values[IndexFind] == null)
                            DeclareQuery += "null";
                        else if (_Values[IndexFind].ToString() == "")
                            DeclareQuery += "null";
                        else if (Row["DATA_TYPE"].ToString() == "bit")
                            DeclareQuery += _Values[IndexFind].ToString().ToLower() == "true" ? 1 : 0;
                        else
                            DeclareQuery += _Values[IndexFind].ToString() == "" ? "null" : _Values[IndexFind].ToString();

                    }
                    else
                    {
                        if (Row["DATA_TYPE"].ToString() == "bigint" || Row["DATA_TYPE"].ToString() == "float" || Row["DATA_TYPE"].ToString() == "int")
                            DeclareQuery += "0";
                        else if (Row["DATA_TYPE"].ToString() == "nvarchar")
                            DeclareQuery += "null";
                        else if (Row["DATA_TYPE"].ToString() == "bit")
                            DeclareQuery += "0";
                    }
                    DeclareQuery += Tools.NewLine;
                }
            }

            return DeclareQuery;
        }


        public string DefineVariablesQuery(string _TableName, long RowID, ref string[] ColumnNames, ref object[] _Values)
        {
            string IdentityField = GetIdentityTable(_TableName);
            string DeclareQuery = "Declare @" + IdentityField + " as Bigint=" + RowID.ToString() + Tools.NewLine;

            DataTable ColumnData = GetAllColumn(_TableName);
            DataTable Data = SelectDataTable("Select * From " + _TableName + " Where " + IdentityField + "=" + RowID);
            ColumnNames = new string[ColumnData.Rows.Count];
            _Values = new object[ColumnData.Rows.Count];
            int Counter = 0;
            if (Data != null)
                if (Data.Rows.Count == 0)
                {
                    foreach (DataColumn col in Data.Columns)
                    {
                        ColumnNames[Counter] = col.ColumnName;
                        Counter++;
                    }


                    foreach (DataRow Row in ColumnData.Rows)
                    {
                        if (Row["COLUMN_NAME"].ToString() != IdentityField)
                        {
                            int IndexFind = Array.IndexOf(ColumnNames, Row["COLUMN_NAME"].ToString());
                            if (IndexFind != -1)
                            {
                                DeclareQuery += "DECLARE @" + Row["COLUMN_NAME"].ToString() + " as " + (Row["DATA_TYPE"].ToString().ToLower() == "nvarchar" ? "NVARCHAR(" + (Row["CHARACTER_MAXIMUM_LENGTH"].ToString() == "-1" ? "MAX" : Row["CHARACTER_MAXIMUM_LENGTH"].ToString()) + ")" : Row["DATA_TYPE"].ToString()) + " = null ";
                                DeclareQuery += Tools.NewLine;
                            }
                        }
                    }
                }
                else
                {
                    foreach (DataColumn col in Data.Columns)
                    {
                        ColumnNames[Counter] = col.ColumnName;
                        _Values[Counter] = Data.Rows[0][Counter];
                        Counter++;
                    }

                    foreach (DataRow Row in ColumnData.Rows)
                    {
                        if (Row["COLUMN_NAME"].ToString() != IdentityField)
                        {
                            int IndexFind = Array.IndexOf(ColumnNames, Row["COLUMN_NAME"].ToString());
                            if (IndexFind != -1)
                            {
                                DeclareQuery += "DECLARE @" + Row["COLUMN_NAME"].ToString() + " as " + (Row["DATA_TYPE"].ToString().ToLower() == "nvarchar" ? "NVARCHAR(" + (Row["CHARACTER_MAXIMUM_LENGTH"].ToString() == "-1" ? "MAX" : Row["CHARACTER_MAXIMUM_LENGTH"].ToString()) + ")" : Row["DATA_TYPE"].ToString()) + " = ";

                                if (Row["DATA_TYPE"].ToString().ToLower() == "nvarchar")
                                    DeclareQuery += "N'" + _Values[IndexFind].ToString() + "'";
                                else if (_Values[IndexFind].ToString() == "")
                                    DeclareQuery += "null";
                                else if (Row["DATA_TYPE"].ToString() == "bit")
                                    DeclareQuery += _Values[IndexFind].ToString().ToLower() == "true" ? 1 : 0;
                                else
                                    DeclareQuery += _Values[IndexFind].ToString() == "" ? "null" : _Values[IndexFind].ToString();

                                DeclareQuery += Tools.NewLine;
                            }
                        }
                    }
                }


            return DeclareQuery;
        }


        public string GetIdentityTable(string TableName, object _DefaultValue = null)
        {
            string Query = "select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where COLUMNPROPERTY(object_id(TABLE_SCHEMA + '.' + TABLE_NAME), COLUMN_NAME, 'IsIdentity') = 1 and TABLE_NAME = N'" + TableName + "' ";
            object Field = SelectField(Query);
            if (Field == null)
                return "";
            else
                return Field.ToString();
        }

        public async Task<string> GetIdentityTableAsync(string TableName, object _DefaultValue = null)
        {
            string Query = "select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where COLUMNPROPERTY(object_id(TABLE_SCHEMA + '.' + TABLE_NAME), COLUMN_NAME, 'IsIdentity') = 1 and TABLE_NAME = N'" + TableName + "' ";
            object Field = await SelectFieldAsync(Query);
            if (Field == null)
                return "";
            else
                return Field.ToString();
        }

        public int Insert(string _TableName, string[] ColumnNames, object[] _Values, long TableID = 0)
        {

            int Output = 0;
            Log.LogFunction("DataBase.Insert", true);
            int TableColumnCount = ColumnNames.Length;
            int TableValuesCount = _Values.Length;
            string TableParams = "";
            string TableValues = "";
            CoreObject TableCore = CoreObject.Find(TableID);
            List<CoreObject> list = new List<CoreObject>();

            Table TableInfo = new Table();
            if (TableCore.CoreObjectID > 0)
                TableInfo = new Table(TableCore);

            for (int Index = 0, loopTo = TableColumnCount - 1; Index <= loopTo; Index++)
            {
                if (Index > 0)
                {
                    TableParams += ",";
                    TableValues += ",";
                }
                TableParams += "[" + ColumnNames[Index] + "]";
                TableValues += "@P" + Index;
            }

            string Query = "Insert Into " + TableInfo.TABLESCHEMA + "." + _TableName.Replace(" ", "_") + "(" + TableParams + ") Values (" + TableValues + "); Select @@Identity";

            using (SqlConnection DBConnection = new SqlConnection(ConnectionData.ConnectionString))
            {
                DBConnection.Open();

                using (SqlCommand TheCommand = new SqlCommand(Query, DBConnection))
                {
                    TheCommand.CommandType = CommandType.Text;
                    TheCommand.CommandTimeout = 300;

                    if (!_TableName.EndsWith("_APMRegistry"))
                    {
                        list = CoreObject.FindChilds(TableID, CoreDefine.Entities.فیلد);
                        foreach (CoreObject obj in list)
                        {
                            Field field = new Field(obj);
                            if (field.FieldNature == "Bigint" || field.FieldNature == "Float")
                            {
                                int FindIndex = Array.IndexOf(ColumnNames, field.FieldName);
                                if (FindIndex > -1)
                                    _Values[FindIndex] = _Values[FindIndex].ToString() == "" || _Values[FindIndex] == null ? 0 : double.Parse(_Values[FindIndex].ToString().Replace('/', '.'), System.Globalization.CultureInfo.InvariantCulture);
                            }
                            //else if (field.FieldNature == "Bit")
                            //    FieldType = "دو مقدار (0 یا 1)";
                            //else
                            //    FieldType = "رشته";
                        }

                    }
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
                            TheCommand.Parameters.Add("@P" + Index, SqlDbType.VarBinary, -1).Value = sr;

                        }
                        else
                            TheCommand.Parameters.AddWithValue("@P" + Index, ParameterValue);
                    }

                    try
                    {
                        using (SqlDataReader TheReader = TheCommand.ExecuteReader())
                        {
                            if (TheReader.Read())
                            {
                                Output = Convert.ToInt32(TheReader[0].ToString());
                            }

                            TheReader.Close();
                        }
                        if (!_TableName.EndsWith("_APMRegistry") && !_TableName.EndsWith("CoreObjectAttachment"))
                        {
                            var Rec = new Record(new SQLDataBase(ConnectionData.Source, ConnectionData.DataBase, ConnectionData.DBPassword, ConnectionData.DBUser, ConnectionData.Version), _TableName, Output, TableCore.CoreObjectID);
                            string[] ColumnName = { "RegistryDate", "RegistryTime", "UserAccountID", "TableName", "CoreObjectID", "RecordID", "Value", "IP", "ServerName", "DatabaseName", "PCName", "Version", "BrowserType", "BrowserVersion" };
                            object[] Values = new object[] { CDateTime.GetNowshamsiDate(), CDateTime.GetNowTime(), Referral.UserAccount.UsersID, _TableName, TableCore.CoreObjectID, Output, Tools.ToXML(new RecordData(Rec)), Referral.UserAccount.IP, ConnectionData.Source, ConnectionData.DataBase, Referral.UserAccount.PCName, Referral.AppVersion, Referral.UserAccount.BrowserType, Referral.UserAccount.BrowserVersion };
                            Referral.DBRegistry.Insert("Insert_APMRegistry", ColumnName, Values);
                        }
                        Log.LogFunction("DataBase.Insert", false);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Database.Insert", "TableName : " + _TableName + "\n" + ex.Message);
                        Log.LogFunction("DataBase.Insert", false);
                    }

                }

                DBConnection.Close();

            }

            return Output;
        }


        public bool UpdateRow(long _RowID, long TableID, string _TableName, string[] ColumnNames, object[] _Values)
        {
            Log.LogFunction("DataBase.UpdateRow");

            bool Output = false;
            int TableColumnCount = ColumnNames.Length;
            int TableValuesCount = _Values.Length;
            string TableParams = "";

            CoreObject TableCore = new CoreObject();
            Table TableInfo = new Table();

            if (TableID > 0)
            {
                TableCore = CoreObject.Find(TableID);
                TableInfo = new Table(TableCore);
            }

            for (int Index = 0; Index < TableColumnCount; Index++)
            {
                if (Index > 0)
                    TableParams += ",";
                TableParams += "[" + ColumnNames[Index] + "] = @P" + Index;
            }
            string Whare = " Where " + GetIdentityTable(_TableName) + " = " + _RowID;

            string Query = "Update " + TableInfo.TABLESCHEMA + "." + _TableName.Replace(" ", "_") + " Set " + TableParams + Whare;



            using (SqlConnection DBConnection = new SqlConnection(ConnectionData.ConnectionString))
            {
                DBConnection.Open();
                using (SqlCommand TheCommand = new SqlCommand(Query, DBConnection))
                {
                    TheCommand.CommandTimeout = 300;
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

                        if (_TableName.StartsWith("CoreObject") && !_TableName.StartsWith("CoreObjectAttachment"))
                        {
                            TableCore = CoreObject.Find(_RowID);
                        }
                        if (!_TableName.EndsWith("APMRegistry") && !_TableName.EndsWith("CoreObject") && !_TableName.EndsWith("CoreObjectAttachment") && TableCore.Entity != CoreDefine.Entities.گزارش)
                        {
                            object[] PreviousValues = SelectRecord("Select " + String.Join(",", ColumnNames) + " From " + TableInfo.TABLESCHEMA + "." + _TableName.Replace(" ", "_") + Whare);

                            //Open(); 

                            TheCommand.CommandTimeout = 300;
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
                                    Referral.DBRegistry.Insert("Update_APMRegistry", new string[] { "RegistryDate", "RegistryTime", "UserAccountID", "TableName", "CoreObjectID", "RecordID", "FieldName", "PreviousValue", "NewValue", "IP", "ServerName", "DatabaseName", "PCName", "Version", "BrowserType", "BrowserVersion" },
                                                                        new object[] { CDateTime.GetNowshamsiDate(), CDateTime.GetNowTime(), Referral.UserAccount.UsersID, _TableName.Replace(" ", "_"), TableCore.CoreObjectID, _RowID, ColumnNames[Index], PreviousValues[Index], ParameterValue, Referral.UserAccount.IP, ConnectionData.Source, ConnectionData.DataBase, Referral.UserAccount.PCName, Referral.AppVersion, Referral.UserAccount.BrowserType, Referral.UserAccount.BrowserVersion });
                                }
                            }
                        }
                        else
                        {
                            //Open();
                            TheCommand.ExecuteNonQuery();
                            Referral.DBRegistry.Insert("Update_APMRegistry", new string[] { "RegistryDate", "RegistryTime", "UserAccountID", "TableName", "RecordID", "CoreObjectID", "IP", "ServerName", "DatabaseName", "PCName", "Version", "BrowserType", "BrowserVersion" },
                                                                new object[] { CDateTime.GetNowshamsiDate(), CDateTime.GetNowTime(), Referral.UserAccount.UsersID, _TableName.Replace(" ", "_"), _RowID, TableCore.CoreObjectID, Referral.UserAccount.IP, ConnectionData.Source, ConnectionData.DataBase, Referral.UserAccount.PCName, Referral.AppVersion, Referral.UserAccount.BrowserType, Referral.UserAccount.BrowserVersion });
                            Output = true;
                        }

                        Log.LogFunction("DataBase.UpdateRow", false);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Database.UpdateRow", "TableName : " + _TableName + "\n" + ex.Message);
                        Log.LogFunction("DataBase.UpdateRow", false);
                    }
                }
                DBConnection.Close();

            }

            return Output;
        }

        public bool Delete(string TableName, long _ID)
        {
            Log.LogFunction("DataBase.Delete");

            SQLDataBase DataBase = new SQLDataBase(ConnectionData.Source, ConnectionData.DataBase, ConnectionData.DBPassword, ConnectionData.DBUser, ConnectionData.Version);

            long DatabaseCoreID = 0;
            Table TableInfo = new Table();
            List<CoreObject> DatabaseObjects = CoreObject.FindChilds(CoreDefine.Entities.پایگاه_داده);
            foreach (CoreObject DatabaseObject in DatabaseObjects)
            {
                DataSourceInfo DataSourceInfo = new DataSourceInfo(DatabaseObject);
                if (DataSourceInfo.ServerName == ConnectionData.Source && DataSourceInfo.DataBase == ConnectionData.DataBase && DataSourceInfo.UserName == ConnectionData.DBUser)
                {
                    DatabaseCoreID = DatabaseObject.CoreObjectID;
                    break;
                }
            }


            CoreObject TableCore = CoreObject.Find(DatabaseCoreID, CoreDefine.Entities.جدول, TableName);
            if (TableCore.CoreObjectID > 0)
                TableInfo = new Table(TableCore);

            var Rec = new Record(DataBase, TableName, _ID, TableCore.CoreObjectID);

            if (!TableName.EndsWith("_APMRegistry"))
            {
                string[] ColumnName = { "RegistryDate", "RegistryTime", "UserAccountID", "TableName", "RecordID", "Value", "IP", "ServerName", "DatabaseName", "PCName", "Version", "BrowserType", "BrowserVersion" };
                object[] Values = new object[] { CDateTime.GetNowshamsiDate(), CDateTime.GetNowTime(), Referral.UserAccount.UsersID, TableName, _ID, Tools.ToXML(new RecordData(Rec)), Referral.UserAccount.IP, ConnectionData.Source, ConnectionData.DataBase, Referral.UserAccount.PCName, Referral.AppVersion, Referral.UserAccount.BrowserType, Referral.UserAccount.BrowserVersion };
                Referral.DBRegistry.Insert("Delete_APMRegistry", ColumnName, Values);
            }
            string Whare = " Where ";

            Whare += "[" + GetIdentityTable(TableName) + "] = " + _ID;

            TableName = "[" + TableInfo.TABLESCHEMA + "].[" + TableName + "]";
            return Execute("Delete " + TableName + Whare);
        }
        public string NowDateShamsi()
        {
            return SelectField("SELECT FORMAT(GETDATE(), 'yyyy/MM/dd', 'fa')").ToString();
        }

        public string NowDateMilady()
        {
            return SelectField("SELECT FORMAT(GETDATE(), 'yyyy/MM/dd', 'en')").ToString();
        }

        public string NowTime()
        {
            return SelectField("SELECT FORMAT(GETDATE(), 'HH:mm:ss', 'fa')").ToString();
        }

        public string[] Columns(string _ObjectName, bool _IsOnlyNormalColumns = false)
        {
            var Output = new object[0];
            string QueryWhere = "";
            switch (ConnectionData.Version)
            {
                case var @case when @case == SQLVersions.SQL2000:
                    {
                        if (_IsOnlyNormalColumns)
                            QueryWhere = "And colstat = 0";
                        Output = SelectColumn("Select syscolumns.name From sysobjects inner join syscolumns on sysobjects.id = syscolumns.id Where sysobjects.name = '" + _ObjectName + "' " + QueryWhere + " order by syscolumns.colid");
                        break;
                    }

                case var case1 when case1 == SQLVersions.SQL2008:
                    {
                        if (_IsOnlyNormalColumns)
                            QueryWhere = "And is_identity = 0 And is_computed = 0";
                        Output = SelectColumn("Select sys.all_columns.name From sys.all_objects inner join sys.all_columns on sys.all_objects.object_id = sys.all_columns.object_id Where sys.all_objects.name = N'" + _ObjectName + "' " + QueryWhere + " order by sys.all_columns.column_id");
                        break;
                    }
            }

            return Tools.VString(Output);
        }

        public async Task<string[]> ColumnsAsync(string _ObjectName, bool _IsOnlyNormalColumns = false)
        {
            var Output = new object[0];
            string QueryWhere = "";
            switch (ConnectionData.Version)
            {
                case var @case when @case == SQLVersions.SQL2000:
                    {
                        if (_IsOnlyNormalColumns)
                            QueryWhere = "And colstat = 0";
                        Output = await SelectColumnAsync("Select syscolumns.name From sysobjects inner join syscolumns on sysobjects.id = syscolumns.id Where sysobjects.name = '" + _ObjectName + "' " + QueryWhere + " order by syscolumns.colid");
                        break;
                    }

                case var case1 when case1 == SQLVersions.SQL2008:
                    {
                        if (_IsOnlyNormalColumns)
                            QueryWhere = "And is_identity = 0 And is_computed = 0";
                        Output = await SelectColumnAsync("Select sys.all_columns.name From sys.all_objects inner join sys.all_columns on sys.all_objects.object_id = sys.all_columns.object_id Where sys.all_objects.name = N'" + _ObjectName + "' " + QueryWhere + " order by sys.all_columns.column_id");
                        break;
                    }
            }

            return Tools.VString(Output);
        }

        public DataTable GetAllColumn(string TableName)
        {
            string Query = "SELECT COLUMN_NAME,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,IS_NULLABLE,ORDINAL_POSITION FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'" + TableName + "'";
            return SelectDataTable(Query);
        }

        public async Task<DataTable> GetAllColumnAsync(string TableName)
        {
            string Query = "SELECT COLUMN_NAME,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,IS_NULLABLE,ORDINAL_POSITION FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'" + TableName + "'";
            return await SelectDataTableAsync(Query);
        }
        public DataTable GetAllFrankeyColumn()
        {
            string Query = "SELECT    FK_Table = FK.TABLE_NAME,   " +
                " FK_Column = CU.COLUMN_NAME,    " +
                "PK_Table = PK.TABLE_NAME,    " +
                "PK_Column = PT.COLUMN_NAME,   " +
                " Constraint_Name = C.CONSTRAINT_NAME " +
                "FROM    INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C " +
                "INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK" +
                "    ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME " +
                "INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK" +
                "    ON C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME " +
                "INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU" +
                "    ON C.CONSTRAINT_NAME = CU.CONSTRAINT_NAME " +
                "INNER JOIN(" +
                "            SELECT" +
                "                i1.TABLE_NAME," +
                "                i2.COLUMN_NAME" +
                "            FROM" +
                "                INFORMATION_SCHEMA.TABLE_CONSTRAINTS i1" +
                "            INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE i2" +
                "                ON i1.CONSTRAINT_NAME = i2.CONSTRAINT_NAME" +
                "            WHERE" +
                "                i1.CONSTRAINT_TYPE = 'PRIMARY KEY'" +
                "           ) PT" +
                "    ON PT.TABLE_NAME = PK.TABLE_NAME";
            return SelectDataTable(Query);
        }

        public async Task<DataTable> GetAllFrankeyColumnAsync()
        {
            string Query = "SELECT    FK_Table = FK.TABLE_NAME,   " +
                " FK_Column = CU.COLUMN_NAME,    " +
                "PK_Table = PK.TABLE_NAME,    " +
                "PK_Column = PT.COLUMN_NAME,   " +
                " Constraint_Name = C.CONSTRAINT_NAME " +
                "FROM    INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C " +
                "INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK" +
                "    ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME " +
                "INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK" +
                "    ON C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME " +
                "INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU" +
                "    ON C.CONSTRAINT_NAME = CU.CONSTRAINT_NAME " +
                "INNER JOIN(" +
                "            SELECT" +
                "                i1.TABLE_NAME," +
                "                i2.COLUMN_NAME" +
                "            FROM" +
                "                INFORMATION_SCHEMA.TABLE_CONSTRAINTS i1" +
                "            INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE i2" +
                "                ON i1.CONSTRAINT_NAME = i2.CONSTRAINT_NAME" +
                "            WHERE" +
                "                i1.CONSTRAINT_TYPE = 'PRIMARY KEY'" +
                "           ) PT" +
                "    ON PT.TABLE_NAME = PK.TABLE_NAME";
            return await SelectDataTableAsync(Query);
        }
        public bool CreateTableWithIdentityField(string TableName, string FieldName = "شناسه")
        {
            try
            {
                Execute("CREATE TABLE dbo." + Tools.SafeTitle(TableName) + " (" + FieldName + " bigint IDENTITY(1,1) NOT NULL , PRIMARY KEY (" + FieldName + "))");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CreateTableWithIdentityFieldAsync(string TableName, string FieldName = "شناسه")
        {
            try
            {
                await ExecuteAsync("CREATE TABLE dbo." + Tools.SafeTitle(TableName) + " (" + FieldName + " bigint IDENTITY(1,1) NOT NULL , PRIMARY KEY (" + FieldName + "))");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool CreateIdentityField(string TableName, string FieldName = "شناسه")
        {
            try
            {
                return Execute("ALTER TABLE " + Tools.SafeTitle(TableName) + " DROP COLUMN " + FieldName + "; ALTER TABLE " + Tools.SafeTitle(TableName) + " ADD " + FieldName + " bigint identity not null;");

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CreateIdentityFieldAsync(string TableName, string FieldName = "شناسه")
        {
            try
            {
                return await ExecuteAsync("ALTER TABLE " + Tools.SafeTitle(TableName) + " DROP COLUMN " + FieldName + "; ALTER TABLE " + Tools.SafeTitle(TableName) + " ADD " + FieldName + " bigint identity not null;");

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string GetDatabasePath()
        {
            string Query = "select physical_name from sys.database_files where type = 0";
            string FilePath = SelectField(Query).ToString();
            return FilePath;
        }

        public async Task<string> GetDatabasePathAsync()
        {
            string Query = "select physical_name from sys.database_files where type = 0";
            var FilePath = await SelectFieldAsync(Query);
            return FilePath.ToString();
        }

        public bool CreateDataBase(string FilePath, string DatabaseName)
        {
            try
            {
                string Query = "USE master " + Tools.NewLine + Tools.NewLine + "CREATE DATABASE " + DatabaseName + " " + Tools.NewLine + "CONTAINMENT = NONE " + Tools.NewLine + "ON PRIMARY " + Tools.NewLine +
                                "(NAME = N'" + DatabaseName + "', FILENAME = N'" + FilePath + "" + DatabaseName + ".mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )" + Tools.NewLine +
                                " LOG ON " + Tools.NewLine + "(NAME = N'" + DatabaseName + "_log', FILENAME = N'" + FilePath + "" + DatabaseName + "_log.ldf' , SIZE = 139264KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )" + Tools.NewLine +
                                " WITH CATALOG_COLLATION = DATABASE_DEFAULT " + Tools.NewLine + Tools.NewLine +
                                "IF(1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled')) " + Tools.NewLine +
                                "begin " + Tools.NewLine +
                                "EXEC " + DatabaseName + ".dbo.sp_fulltext_database @action = 'enable' " + Tools.NewLine +
                                "end " + Tools.NewLine +
                                 Tools.NewLine + "ALTER DATABASE " + DatabaseName + " SET ANSI_NULL_DEFAULT OFF " + Tools.NewLine +
                                 Tools.NewLine + "ALTER DATABASE " + DatabaseName + " SET ANSI_NULLS OFF " + Tools.NewLine +
                                 Tools.NewLine + "ALTER DATABASE " + DatabaseName + " SET ANSI_PADDING OFF " + Tools.NewLine +
                                 Tools.NewLine + "ALTER DATABASE " + DatabaseName + " SET ANSI_WARNINGS OFF " + Tools.NewLine +
                                 Tools.NewLine + "ALTER DATABASE " + DatabaseName + " SET ARITHABORT OFF " + Tools.NewLine +
                                 Tools.NewLine + "ALTER DATABASE " + DatabaseName + " SET AUTO_CLOSE OFF " + Tools.NewLine +
                                 Tools.NewLine + "ALTER DATABASE " + DatabaseName + " SET AUTO_SHRINK OFF " + Tools.NewLine +
                                 Tools.NewLine + "ALTER DATABASE " + DatabaseName + " SET AUTO_UPDATE_STATISTICS ON " + Tools.NewLine +
                                 Tools.NewLine + "ALTER DATABASE " + DatabaseName + " SET CURSOR_CLOSE_ON_COMMIT OFF " + Tools.NewLine +
                                 Tools.NewLine + "ALTER DATABASE " + DatabaseName + " SET CURSOR_DEFAULT  GLOBAL " + Tools.NewLine +
                                 Tools.NewLine + "ALTER DATABASE " + DatabaseName + " SET CONCAT_NULL_YIELDS_NULL OFF " + Tools.NewLine +
                                 Tools.NewLine + "ALTER DATABASE " + DatabaseName + " SET NUMERIC_ROUNDABORT OFF " + Tools.NewLine +
                                 Tools.NewLine + "ALTER DATABASE " + DatabaseName + " SET QUOTED_IDENTIFIER OFF " + Tools.NewLine +
                                 Tools.NewLine + "ALTER DATABASE " + DatabaseName + " SET RECURSIVE_TRIGGERS OFF " + Tools.NewLine +
                                 Tools.NewLine + "ALTER DATABASE " + DatabaseName + " SET  DISABLE_BROKER " + Tools.NewLine +
                                 Tools.NewLine + "ALTER DATABASE " + DatabaseName + " SET AUTO_UPDATE_STATISTICS_ASYNC OFF " + Tools.NewLine +
                                 Tools.NewLine + "ALTER DATABASE " + DatabaseName + " SET DATE_CORRELATION_OPTIMIZATION OFF " + Tools.NewLine +
                                 Tools.NewLine + "ALTER DATABASE " + DatabaseName + " SET TRUSTWORTHY OFF " + Tools.NewLine +
                                 Tools.NewLine + "ALTER DATABASE " + DatabaseName + " SET ALLOW_SNAPSHOT_ISOLATION OFF " + Tools.NewLine +
                                 Tools.NewLine + "ALTER DATABASE " + DatabaseName + " SET PARAMETERIZATION SIMPLE " + Tools.NewLine +
                                 Tools.NewLine + "ALTER DATABASE " + DatabaseName + " SET READ_COMMITTED_SNAPSHOT OFF " + Tools.NewLine +
                                 Tools.NewLine + "ALTER DATABASE " + DatabaseName + " SET HONOR_BROKER_PRIORITY OFF " + Tools.NewLine +
                                 Tools.NewLine + "ALTER DATABASE " + DatabaseName + " SET RECOVERY FULL " + Tools.NewLine +
                                 Tools.NewLine + "ALTER DATABASE " + DatabaseName + " SET  MULTI_USER " + Tools.NewLine +
                                 Tools.NewLine + "ALTER DATABASE " + DatabaseName + " SET PAGE_VERIFY CHECKSUM " + Tools.NewLine +
                                 Tools.NewLine + "ALTER DATABASE " + DatabaseName + " SET DB_CHAINING OFF " + Tools.NewLine +
                                 Tools.NewLine + "ALTER DATABASE " + DatabaseName + " SET FILESTREAM(NON_TRANSACTED_ACCESS = OFF) " + Tools.NewLine +
                                 Tools.NewLine + "ALTER DATABASE " + DatabaseName + " SET TARGET_RECOVERY_TIME = 60 SECONDS " + Tools.NewLine +
                                 Tools.NewLine + "ALTER DATABASE " + DatabaseName + " SET DELAYED_DURABILITY = DISABLED " + Tools.NewLine +
                                 Tools.NewLine + "ALTER DATABASE " + DatabaseName + " SET QUERY_STORE = OFF " + Tools.NewLine +
                                 Tools.NewLine + "ALTER DATABASE " + DatabaseName + " SET READ_WRITE " + Tools.NewLine;

                return Execute(Query);
            }
            catch (Exception ex)
            {
                return false;
            }
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
                    if (_NewValue.ToString() == _PreviousValue.ToString())
                    {
                        return false;
                    }
                }
                else if (_PreviousValue is long)
                {
                    if (_PreviousValue.ToString() == _NewValue.ToString())
                    {
                        return false;
                    }
                }
                else if (_PreviousValue is bool)
                {
                    if (_PreviousValue.ToString().ToLower() == _NewValue.ToString().ToLower())
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



    [System.Xml.Serialization.XmlInclude(typeof(FieldData))]
    public partial class RecordData
    {

        public List<FieldData> Items;

        public RecordData()
        {
        }

        public RecordData(Record _Rec)
        {
            Items = new List<FieldData>();
            if (_Rec.FieldsObject.Count == 0 && _Rec.Columns.Length > 0)
                for (int Index = 0; Index < _Rec.Columns.Length; Index++)
                    Items.Add(new FieldData(_Rec.Columns[Index], (_Rec.Values[Index] == null ? "" : _Rec.Values[Index].ToString()), 0));
            else
                for (int Index = 0; Index < _Rec.FieldsObject.Count; Index++)
                    if (_Rec.FieldsObject.Count == 0)
                        Items.Add(new FieldData(_Rec.Columns[Index], _Rec.Values[Index].ToString(), 0));
                    else
                    {
                        if (_Rec.Columns[Index] == _Rec.FieldsObject[Index].FullName)
                            Items.Add(new FieldData(_Rec.Columns[Index], _Rec.Values[Index].ToString(), _Rec.FieldsObject[Index].CoreObjectID));
                        else
                        {
                            int FindIndex = Array.IndexOf(_Rec.Columns, _Rec.FieldsObject[Index].FullName);
                            Items.Add(new FieldData(_Rec.Columns[FindIndex], _Rec.Values[FindIndex].ToString(), _Rec.FieldsObject[Index].CoreObjectID));
                        }
                    }
        }
    }

    public partial class FieldData
    {
        public string Name;
        public string Value;
        public long CoreObjectID;

        public FieldData()
        {
        }

        public FieldData(string _Name, string _Value, long _CoreObjectID)
        {
            Name = _Name;
            Value = _Value;
            CoreObjectID = _CoreObjectID;
        }
    }
}
 