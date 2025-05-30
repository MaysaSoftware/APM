using System.Data.SqlClient;

namespace TaxApi
{

    public partial class Record
    {
        public string[] Columns = new string[0];
        public object[] Values = new object[0]; 
        public bool IsHasData;

        public Record()
        {
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
            int FindIndex = Tools.IsInArrayIndex(_ColumnName, Columns, false, false);
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
