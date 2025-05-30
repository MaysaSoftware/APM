//using APM.Models.Database;
//using APM.Models.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Xml.Serialization;

namespace TaxApi
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
        public SQLDataBase(string Source, string DataBase, string DBPassword, string DBUser,SQLVersions versions)
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
                    return "Password=" + DBPassword + ";Persist Security Info=True;Data Source=" + Source + ";Integrated Security=False; Initial Catalog=" + DataBase + ";User ID=" + DBUser; // Connect timeout=15;
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
            try
            {
                
                ConnectionData.IsOpen = true;
                if(!ConnectionData.IsOpen)
                    ConnectionData.DBConnection.Open();
                return ConnectionData.IsOpen;
            }
            catch (Exception ex)
            { 
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
            Open();
            var TheCommand = new SqlCommand(_Query, ConnectionData.DBConnection);
            TheCommand.CommandType = System.Data.CommandType.Text;
            try
            {
                TheCommand.ExecuteNonQuery();
                Close();     
                return true;
            }
            catch (Exception ex)
            {
                if (ex.Message == "ExecuteNonQuery requires an open and available Connection. The connection's current state is closed.")
                    Execute(_Query);
                Close();
                return false;
            }
        }
        
        public string GetPrimeryKey(string TableName)
        { 
            string Query= "SELECT name FROM sys.key_constraints WHERE type = 'PK' and parent_object_id = (SELECT OBJECT_ID(N'"+ TableName + "') )";
            object Result= SelectField(Query);
            return Result==null?"": Result.ToString();
        }
        public bool SetPrimeryKey(string TableName, string FieldName)
        { 
            string Query= "ALTER TABLE "+ TableName + "  ADD CONSTRAINT PK_"+ TableName + "HistoryArchive_"+ FieldName + " PRIMARY KEY CLUSTERED(["+ FieldName + "]); "; 
            if(Execute(Query)) 
                return true;
            else
                return false; 
        }
        public bool RemovePrimeryKey(string TableName)
        {  
            string Query= "ALTER TABLE "+ TableName + " DROP CONSTRAINT "+GetPrimeryKey(TableName)+"; "; 
            if (Execute(Query)) 
                return true;
            else
                return false; 
        }

        public object SelectField(string _Query, object _DefaultValue = null)
        { 
            Open();
            var TheCommand = new SqlCommand(_Query, ConnectionData.DBConnection);
            SqlDataReader TheReader;
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
             
            Close();
            return Output==null?"":Output;
        }

        public object[] SelectRecord(string _Query)
        { 
            Open();
            var TheCommand = new SqlCommand(_Query, ConnectionData.DBConnection);
            SqlDataReader TheReader;
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

            Close();
            return Output;
        }

        public object[] SelectColumn(string _Query)
        { 
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
                    }


                }
                DBConnection.Close();

            } 
            return Output; 
 
        }
        
        public object[] GetAllTableName()
        {
            string Query = "SELECT TABLE_NAME  FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
            return SelectColumn(Query);

        }

        public DataTable SelectDataTable(string _Query)
        { 
            Open();
            var TheCommand = new SqlCommand(_Query, ConnectionData.DBConnection); 
            DataTable Data = new DataTable();
            TheCommand.CommandTimeout = 300;
            TheCommand.CommandType = CommandType.Text;
            try
            {
                SqlDataAdapter DataAdapter = new SqlDataAdapter(TheCommand);
                DataAdapter.Fill(Data); 
                Close();
                return Data;
            }
            catch (Exception ex)
            { 
            } 

            Close();
            return null;
        }

        public string DefineVariablesQuery(string _TableName,long RowID, string[] ColumnNames, object[] _Values)
        {
            string IdentityField = GetIdentityTable(_TableName);
            string DeclareQuery = "Declare @" + IdentityField + " as Bigint=" + RowID.ToString() + "\n";

            DataTable ColumnData = GetAllColumn(_TableName);

            foreach (DataRow Row in ColumnData.Rows)
            {
                if (Row["COLUMN_NAME"].ToString() != IdentityField)
                {
                    int IndexFind = Array.IndexOf(ColumnNames, Row["COLUMN_NAME"].ToString());
                    DeclareQuery += "DECLARE @" + Row["COLUMN_NAME"].ToString() + " as " + (Row["DATA_TYPE"].ToString() == "nvarchar" ? "NVARCHAR(" + (Row["CHARACTER_MAXIMUM_LENGTH"].ToString() == "-1" ? "MAX" : Row["CHARACTER_MAXIMUM_LENGTH"].ToString()) + ")" : Row["DATA_TYPE"].ToString()) + " = ";

                    if (IndexFind != -1)
                    {
                      
                        if (Row["DATA_TYPE"].ToString() == "nvarchar")
                            DeclareQuery += "N'" + _Values[IndexFind].ToString() + "'";
                        else if (_Values[IndexFind].ToString() == "")
                            DeclareQuery += "null";
                        else if (Row["DATA_TYPE"].ToString() == "bit")
                            DeclareQuery += _Values[IndexFind].ToString() == "true" ? 1 : 0;
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
                            DeclareQuery +=  "0";
                    }
                    DeclareQuery += "\n";
                }
            }

            return DeclareQuery;
        }

        
        public string DefineVariablesQuery(string _TableName,long RowID)
        {
            string IdentityField = GetIdentityTable(_TableName);
            string DeclareQuery = "Declare @"+ IdentityField + " as Bigint=" + RowID.ToString()+ "\n";

            DataTable ColumnData = GetAllColumn(_TableName);
            DataTable Data = SelectDataTable("Select * From " + _TableName + " Where " + IdentityField + "=" + RowID);
            string[] ColumnNames = new string[ColumnData.Rows.Count];
            object[] _Values = new object[ColumnData.Rows.Count];
            int Counter = 0;
            foreach(DataColumn col in Data.Columns)
            {
                ColumnNames[Counter]=col.ColumnName;    
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
                        DeclareQuery += "DECLARE @" + Row["COLUMN_NAME"].ToString() + " as " + (Row["DATA_TYPE"].ToString() == "nvarchar" ? "NVARCHAR(" + (Row["CHARACTER_MAXIMUM_LENGTH"].ToString()=="-1"?"MAX": Row["CHARACTER_MAXIMUM_LENGTH"].ToString()) + ")" : Row["DATA_TYPE"].ToString()) + " = ";

                        if (Row["DATA_TYPE"].ToString() == "nvarchar")
                            DeclareQuery += "N'" + _Values[IndexFind].ToString() + "'";
                        else if (_Values[IndexFind].ToString() == "")
                            DeclareQuery += "null";
                        else if (Row["DATA_TYPE"].ToString() == "bit")
                            DeclareQuery += _Values[IndexFind].ToString() == "true" ? 1 : 0;
                        else
                            DeclareQuery += _Values[IndexFind].ToString() == "" ? "null" : _Values[IndexFind].ToString();

                        DeclareQuery += "\n";
                    }
                }
            }

            return DeclareQuery;
        }


        public string GetIdentityTable(string TableName, object _DefaultValue = null)
        { 
            string Query = "select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where COLUMNPROPERTY(object_id(TABLE_SCHEMA + '.' + TABLE_NAME), COLUMN_NAME, 'IsIdentity') = 1 and TABLE_NAME = N'" + TableName + "' ";
            object Field= SelectField(Query);
            if (Field == null)
                return "";
            else
            return Field.ToString();
        }

        public int Insert(string _TableName,  string[] ColumnNames,object[] _Values, long TableID=0)
        { 
            Open();
            int TableColumnCount = ColumnNames.Length;
            int TableValuesCount = _Values.Length;
            string TableParams = "";
            string TableValues = ""; 
              
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

            SqlDataReader TheReader;
            string Query = "Insert Into " + _TableName.Replace(" ","_") + "("+ TableParams + ") Values (" + TableValues + "); Select @@Identity";
            var TheCommand = new SqlCommand(Query, ConnectionData.DBConnection);
            TheCommand.CommandType = CommandType.Text; 
            for (int Index = 0; Index < TableColumnCount; Index++)
            {
                object ParameterValue;
                string ValueType="";
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
                if (ValueType== "Byte[]")
                {
                    byte[] sr = (byte[]) ParameterValue;
                    TheCommand.Parameters.Add("@P" + Index, SqlDbType.VarBinary,-1).Value = sr;

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
                Close();
                return Output;
            }
            catch (Exception ex)
            { 
            }

            Close();
            return default;
        } 
         

        public bool UpdateRow(long _RowID,long TableID, string _TableName, string[] ColumnNames, object[] _Values)
        { 

            int TableColumnCount = ColumnNames.Length;
            int TableValuesCount = _Values.Length;
            string TableParams = ""; 
             

            for (int Index = 0; Index < TableColumnCount; Index++)
            {
                if (Index > 0)
                    TableParams += ",";
                TableParams += ColumnNames[Index] + " = @P" + Index;
            }
            string Whare = " Where "+ GetIdentityTable(_TableName) +" = " + _RowID;

            string Query = "Update " + _TableName.Replace(" ", "_") + " Set " + TableParams + Whare;
            var TheCommand = new SqlCommand(Query, ConnectionData.DBConnection);
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
                    if (ValueType == "Object[]" || ValueType == "String[]" )
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
                Open();
                TheCommand.ExecuteNonQuery(); 
                Output = true;  
                Close();
                return Output;
            }
            catch (Exception ex)
            { 

            }

            Close();
            return default;
        } 
  
        public bool Delete(string TableName, long _ID)
        {  
            string Whare = " Where ";
             
               Whare += GetIdentityTable(TableName) +" = " + _ID;

            TableName = "[" + TableName + "]";
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

        public DataTable GetAllColumn(string TableName)
        {
            string Query = "SELECT COLUMN_NAME,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,IS_NULLABLE,ORDINAL_POSITION FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'" + TableName+"'";
            return SelectDataTable(Query);
        }
        public bool CreateTableWithIdentityField(string TableName,string FieldName="شناسه")
        {
            try
            {
                Execute("CREATE TABLE dbo." + Tools.SafeTitle(TableName) + " ("+ FieldName + " bigint IDENTITY(1,1) NOT NULL , PRIMARY KEY ("+ FieldName + "))");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool CreateIdentityField(string TableName,string FieldName = "شناسه")
        {
            try
            {
                Execute("ALTER TABLE "+ Tools.SafeTitle(TableName) + " DROP COLUMN "+FieldName+ "; ALTER TABLE " + Tools.SafeTitle(TableName) + " ADD " + FieldName + " bigint identity not null;");
                return true;
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
        public  bool CreateDataBase(string FilePath,string DatabaseName)
        {
            try
            {
                string Query = "USE master " + Tools.NewLine+ Tools.NewLine +"CREATE DATABASE " + DatabaseName + " " + Tools.NewLine +"CONTAINMENT = NONE " + Tools.NewLine +"ON PRIMARY " + Tools.NewLine +
                                "(NAME = N'" + DatabaseName + "', FILENAME = N'" + FilePath + "" + DatabaseName + ".mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )" + Tools.NewLine +
                                " LOG ON " + Tools.NewLine +"(NAME = N'" + DatabaseName + "_log', FILENAME = N'" + FilePath + "" + DatabaseName + "_log.ldf' , SIZE = 139264KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )" + Tools.NewLine +
                                " WITH CATALOG_COLLATION = DATABASE_DEFAULT " + Tools.NewLine + Tools.NewLine +
                                "IF(1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled')) " + Tools.NewLine +
                                "begin " + Tools.NewLine +
                                "EXEC " + DatabaseName + ".dbo.sp_fulltext_database @action = 'enable' " + Tools.NewLine +
                                "end " + Tools.NewLine +
                                 Tools.NewLine +"ALTER DATABASE " + DatabaseName + " SET ANSI_NULL_DEFAULT OFF " + Tools.NewLine +
                                 Tools.NewLine +"ALTER DATABASE " + DatabaseName + " SET ANSI_NULLS OFF " + Tools.NewLine +
                                 Tools.NewLine +"ALTER DATABASE " + DatabaseName + " SET ANSI_PADDING OFF " + Tools.NewLine +
                                 Tools.NewLine +"ALTER DATABASE " + DatabaseName + " SET ANSI_WARNINGS OFF " + Tools.NewLine +
                                 Tools.NewLine +"ALTER DATABASE " + DatabaseName + " SET ARITHABORT OFF " + Tools.NewLine +
                                 Tools.NewLine +"ALTER DATABASE " + DatabaseName + " SET AUTO_CLOSE OFF " + Tools.NewLine +
                                 Tools.NewLine +"ALTER DATABASE " + DatabaseName + " SET AUTO_SHRINK OFF " + Tools.NewLine +
                                 Tools.NewLine +"ALTER DATABASE " + DatabaseName + " SET AUTO_UPDATE_STATISTICS ON " + Tools.NewLine +
                                 Tools.NewLine +"ALTER DATABASE " + DatabaseName + " SET CURSOR_CLOSE_ON_COMMIT OFF " + Tools.NewLine +
                                 Tools.NewLine +"ALTER DATABASE " + DatabaseName + " SET CURSOR_DEFAULT  GLOBAL " + Tools.NewLine +
                                 Tools.NewLine +"ALTER DATABASE " + DatabaseName + " SET CONCAT_NULL_YIELDS_NULL OFF " + Tools.NewLine +
                                 Tools.NewLine +"ALTER DATABASE " + DatabaseName + " SET NUMERIC_ROUNDABORT OFF " + Tools.NewLine +
                                 Tools.NewLine +"ALTER DATABASE " + DatabaseName + " SET QUOTED_IDENTIFIER OFF " + Tools.NewLine +
                                 Tools.NewLine +"ALTER DATABASE " + DatabaseName + " SET RECURSIVE_TRIGGERS OFF " + Tools.NewLine +
                                 Tools.NewLine +"ALTER DATABASE " + DatabaseName + " SET  DISABLE_BROKER " + Tools.NewLine +
                                 Tools.NewLine +"ALTER DATABASE " + DatabaseName + " SET AUTO_UPDATE_STATISTICS_ASYNC OFF " + Tools.NewLine +
                                 Tools.NewLine +"ALTER DATABASE " + DatabaseName + " SET DATE_CORRELATION_OPTIMIZATION OFF " + Tools.NewLine +
                                 Tools.NewLine +"ALTER DATABASE " + DatabaseName + " SET TRUSTWORTHY OFF " + Tools.NewLine +
                                 Tools.NewLine +"ALTER DATABASE " + DatabaseName + " SET ALLOW_SNAPSHOT_ISOLATION OFF " + Tools.NewLine +
                                 Tools.NewLine +"ALTER DATABASE " + DatabaseName + " SET PARAMETERIZATION SIMPLE " + Tools.NewLine +
                                 Tools.NewLine +"ALTER DATABASE " + DatabaseName + " SET READ_COMMITTED_SNAPSHOT OFF " + Tools.NewLine +
                                 Tools.NewLine +"ALTER DATABASE " + DatabaseName + " SET HONOR_BROKER_PRIORITY OFF " + Tools.NewLine +
                                 Tools.NewLine +"ALTER DATABASE " + DatabaseName + " SET RECOVERY FULL " + Tools.NewLine +
                                 Tools.NewLine +"ALTER DATABASE " + DatabaseName + " SET  MULTI_USER " + Tools.NewLine +
                                 Tools.NewLine +"ALTER DATABASE " + DatabaseName + " SET PAGE_VERIFY CHECKSUM " + Tools.NewLine +
                                 Tools.NewLine +"ALTER DATABASE " + DatabaseName + " SET DB_CHAINING OFF " + Tools.NewLine +
                                 Tools.NewLine + "ALTER DATABASE " + DatabaseName + " SET FILESTREAM(NON_TRANSACTED_ACCESS = OFF) " + Tools.NewLine +
                                 Tools.NewLine + "ALTER DATABASE " + DatabaseName + " SET TARGET_RECOVERY_TIME = 60 SECONDS " + Tools.NewLine +
                                 Tools.NewLine +"ALTER DATABASE " + DatabaseName + " SET DELAYED_DURABILITY = DISABLED " + Tools.NewLine +
                                 Tools.NewLine +"ALTER DATABASE " + DatabaseName + " SET QUERY_STORE = OFF " + Tools.NewLine +
                                 Tools.NewLine +"ALTER DATABASE " + DatabaseName + " SET READ_WRITE " + Tools.NewLine;

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
                    if (_PreviousValue.ToString()==_NewValue.ToString())
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




     
    public partial class FieldData
    {
        public string Name;
        public string Value;
        public long CoreObjectID;

        public FieldData()
        {
        }

        public FieldData(string _Name, string _Value,long _CoreObjectID)
        {
            Name = _Name;
            Value = _Value;
            CoreObjectID = _CoreObjectID;
        }
    }
}
