namespace TaxApi
{
    public class Tools
    {
        public static string NewLine = "\n";

  
        public static string N(string Text)
        {
            return "N'" + Text + "'";
        } 
        public static string[] VString(params object[] _Values)
        {
            var Output = new string[_Values.Length];
            for (int Index = 0, loopTo = _Values.Length - 1; Index <= loopTo; Index++)
            {
                if (_Values[Index] is object)
                {
                    Output[Index] = _Values[Index].ToString();
                }
                else
                {
                    Output[Index] = "";
                }
            }

            return Output;
        }
        public static int IsInArrayIndex(object _Key, object[] _Array, bool _TypeCompare = false, bool _CaseSensitive = true)
        {
            var Index = default(int);
            if (_TypeCompare)
            {
                foreach (Type Item in _Array)
                {
                    if (ReferenceEquals(_Key, Item))
                    {
                        return Index;
                    }

                    Index += 1;
                }
            }
            else
            {
                foreach (object Item in _Array)
                {
                    if (Item != null)
                        if (_CaseSensitive)
                        {
                            if (_Key == Item)
                            {
                                return Index;
                            }
                        }
                        else if ((_Key.ToString().ToLower() ?? "") == (Item.ToString().ToLower() ?? ""))
                        {
                            return Index;
                        }

                    Index += 1;
                }
            }

            return -1;
        }

        public static string UnSafeTitle(string _Text)
        {
            return _Text.Replace("_", " ");
        }

        public static string SafeTitle(string _Text)
        {
            return _Text.Replace(" ", "_").Replace("\n", "").Replace("+", "_").Replace("-", "_").Replace("%", "_").Replace("$", "_").Replace(":", "_");
        }
         

        public static string ConvertToSQLQuery(string Query)
        {
            if (string.IsNullOrEmpty(Query))
            {
                Query = "";
            }
            else
            {
                Query = Query.ToUpper();
                Query = Query.Replace("در.غیر.اینصورت", "ELSE");
                Query = Query.Replace("ارتباط.مستقیم.با", "INNER JOIN");
                Query = Query.Replace("با.توجه.به", "ON");
                Query = Query.Replace("اگر.تهی.هست", " ISNULL ");
                Query = Query.Replace("اگر.تهی.است", "ISNULL");
                Query = Query.Replace("دستور.ویرایش", "UPDATE");
                Query = Query.Replace("مقداردهی", "SET");
                Query = Query.Replace("،", ",");
                Query = Query.Replace("نمایش ", "SELECT ");
                Query = Query.Replace("نمایش\n", "SELECT\n");
                Query = Query.Replace("حداکثر", "MAX");
                Query = Query.Replace("تبدیل", "CAST");
                Query = Query.Replace("بعنوان", "AS");
                Query = Query.Replace("تعریف ", "DECLARE ");
                Query = Query.Replace("نوع.صحیح", "BIGINT");
                Query = Query.Replace("نوع.رشته", "NVARCHAR(400)");
                Query = Query.Replace("رشته.انتخاب", "SUBSTRING");
                Query = Query.Replace("از.جدول", "FROM");
                Query = Query.Replace("انتخاب", "CASE");
                Query = Query.Replace(" مجموع", " SUM");
                Query = Query.Replace("زمانیکه", "WHEN");
                Query = Query.Replace(" هست ", " IS ");
                Query = Query.Replace("مخالف", "NOT");
                Query = Query.Replace(" تهی ", " NULL ");
                Query = Query.Replace(" پس ", " THEN ");
                Query = Query.Replace("ترتیب ", "ORDER BY ");
                Query = Query.Replace(" صعودی", " ASC ");
                Query = Query.Replace(" نزولی", " DESC ");
                Query = Query.Replace("در.صورتی.که", "WHERE");
                Query = Query.Replace("پایان.دستورات", "END");
                Query = Query.Replace(" اگر ", " IF ");
                Query = Query.Replace("اگر(", "IF(");
                Query = Query.Replace("اگر (", "IF (");
                Query = Query.Replace("تعداد.کل", "COUNT(1)");
                Query = Query.Replace("&gt;", "<");
                Query = Query.Replace("شروع.دستورات", "BEGIN");
                Query = Query.Replace("{", "N'");
                Query = Query.Replace("}", "'");
                Query = Query.Replace(" و ", " AND ");
                Query = Query.Replace(" یا ", " OR ");
                Query = Query.Replace(" سطرها.به.تعداد ", " Top ");
                Query = Query.Replace("دستور.درج ", "INSERT ");
                Query = Query.Replace(" در ", " INTO ");
                Query = Query.Replace(" مقادیر ", "VALUES ");
                Query = Query.Replace(" تهی", " NULL");
            }
            return Query;
        }
 
 

        public static string GetExcelFormat(string FileName)
        {
            return FileName.EndsWith(".xlsx") ? "" : FileName.EndsWith(".xls") ? "" : ".xls";
        }
        public static string GetAccessFormat(string FileName)
        {
            return FileName.EndsWith(".mdb") ? "" : FileName.EndsWith(".accdb") ? "" : ".accdb";
        } 
  

    }
}
