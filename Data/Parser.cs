using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Data.OleDb;
using Language;

namespace Data
{
    public static class Parser
    {
        public static Frame CSV(string filename)
        {
            // Parse CSV to DataTable
            if (!File.Exists(filename))
                return null;

            string full = Path.GetFullPath(filename);
            string file = Path.GetFileName(full);
            string dir = Path.GetDirectoryName(full);

            //create the "database" connection string 
            string connString = "Provider=Microsoft.Jet.OLEDB.4.0;"
              + "Data Source=\"" + dir + "\\\";"
              + "Extended Properties=\"text;HDR=Yes;FMT=Delimited\"";

            //create the database query
            string query = "SELECT * FROM " + file;

            //create a DataTable to hold the query results
            DataTable dTable = new DataTable();

            //create an OleDbDataAdapter to execute the query
            OleDbDataAdapter dAdapter = new OleDbDataAdapter(query, connString);

            try
            {
                //fill the DataTable
                dAdapter.Fill(dTable);
            }
            catch (InvalidOperationException /*e*/)
            { }

            dAdapter.Dispose();

            // Spit out to Frame
            Frame result = new Frame();

            foreach (DataColumn column in dTable.Columns)
            {
                result = Frame.Cbind(result, new Frame(InitializeColumn(column, dTable), new Symbol(column.ColumnName)));
            }

            return result;
        }

        static bool IsNumeric(DataColumn col)
        {
            if (col == null)
                return false;
            // Make this const
            var numericTypes = new[] { typeof(Byte), typeof(Decimal), typeof(Double),
                typeof(Int16), typeof(Int32), typeof(Int64), typeof(SByte),
                typeof(Single), typeof(UInt16), typeof(UInt32), typeof(UInt64)};
            return numericTypes.Contains(col.DataType);
        }

        static bool IsInteger(DataColumn col)
        {
            if (col == null)
                return false;
            // Make this const
            var integerTypes = new[] { typeof(Byte),
                typeof(Int16), typeof(Int32), typeof(Int64), typeof(SByte),
                typeof(UInt16), typeof(UInt32), typeof(UInt64)};
            return integerTypes.Contains(col.DataType);
        }

        static Vector InitializeColumn(DataColumn column, DataTable table)
        {
            if (IsNumeric(column))
            {
                int n = table.AsEnumerable().Select(row => row[column.Ordinal]).Distinct().Count();

                if (IsInteger(column) && n <= 7)
                {
                    IList<string> levels = table.AsEnumerable().Select(row => row[column.Ordinal]).Distinct().OrderBy(d => d).Select(d => d.ToString()).ToList();
                    List<int> v = table.AsEnumerable().Select(row => levels.IndexOf(row[column.Ordinal].ToString())).ToList();
                    return new Factor(v, true, levels);                    
                }
                else
                {
                    List<double> d = (from row in table.AsEnumerable() select (double)Convert.ChangeType(row[column.Ordinal], typeof(double))).ToList();
                    double min = d.Min();
                    double max = d.Max();

                    Language.Numeric.NumericDomain domain;

                    if (IsInteger(column) && min >= 0)
                        domain = Language.Numeric.NumericDomain.COUNT;
                    else if (min >= 0 && max <= 1)
                        domain = Language.Numeric.NumericDomain.RATIO;
                    else if (min >= 0)
                        domain = Language.Numeric.NumericDomain.AMOUNT;
                    else
                        domain = Language.Numeric.NumericDomain.BALANCE;

                    return new Numeric(d, domain);
                }
            }
            else
            {
                List<string> s = (from row in table.AsEnumerable() select row[column.Ordinal].ToString()).ToList();

                IList<string> levels = s.Distinct().OrderBy(d => d).ToList();
                List<int> v = s.Select(x => levels.IndexOf(x)).ToList();
                return new Factor(v, false, levels);
            }
        }
    }
}
