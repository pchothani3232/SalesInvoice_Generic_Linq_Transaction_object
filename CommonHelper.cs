using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Reflection;

namespace DatabaseConfiguration
{
    public class CommonHelper
    {
        //3. Automatically build the SqlParameter[] array so you don’t have to manually define parameters every time.
        public static SqlParameter[] GetParameter(string storedProcName, sqlparam obj)
        {
            DataTable dt = GetStoredProcedureParameters(storedProcName);

            SqlParameter[] parameters = new SqlParameter[dt.Rows.Count];

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string paramName = dt.Rows[i]["PARAMETER_NAME"].ToString();  // e.g. @CustomerName
                string paramMode = dt.Rows[i]["PARAMETER_MODE"].ToString();  // IN, OUT, INOUT
                string sqlTypeName = dt.Rows[i]["DATA_TYPE"].ToString();     // nvarchar, numeric, etc.

                // Match property in sqlparam object
                string propertyName = paramName.Substring(1); // remove '@'
                PropertyInfo property = obj.GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                object value = property != null ? property.GetValue(obj) ?? DBNull.Value : DBNull.Value;

                // Convert to SqlDbType
                SqlDbType sqlDbType;
                if (!Enum.TryParse(sqlTypeName, true, out sqlDbType))
                {
                    sqlDbType = SqlDbType.VarChar; // fallback
                }

                // Extra metadata
                int maxLength = dt.Columns.Contains("CHARACTER_MAXIMUM_LENGTH") && dt.Rows[i]["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value
                                ? Convert.ToInt32(dt.Rows[i]["CHARACTER_MAXIMUM_LENGTH"])
                                : -1;

                byte precision = dt.Columns.Contains("NUMERIC_PRECISION") && dt.Rows[i]["NUMERIC_PRECISION"] != DBNull.Value
                                ? Convert.ToByte(dt.Rows[i]["NUMERIC_PRECISION"])
                                : (byte)0;

                byte scale = dt.Columns.Contains("NUMERIC_SCALE") && dt.Rows[i]["NUMERIC_SCALE"] != DBNull.Value
                                ? Convert.ToByte(dt.Rows[i]["NUMERIC_SCALE"])
                                : (byte)0;

                SqlParameter param;

                // Handle string types
                if (sqlDbType == SqlDbType.NVarChar || sqlDbType == SqlDbType.VarChar ||
                    sqlDbType == SqlDbType.NChar || sqlDbType == SqlDbType.Char)
                {
                    // -1 means NVARCHAR(MAX) / VARCHAR(MAX)
                    param = new SqlParameter(paramName, sqlDbType, maxLength == -1 ? -1 : maxLength);
                }
                // Handle decimals/numerics
                else if (sqlDbType == SqlDbType.Decimal)
                {
                    param = new SqlParameter(paramName, sqlDbType)
                    {
                        Precision = precision,
                        Scale = scale
                    };
                }
                else
                {
                    param = new SqlParameter(paramName, sqlDbType);
                }

                // Assign value
                param.Value = value;

                // Direction (IN/OUT/INOUT)
                if (paramMode.Equals("OUT", StringComparison.OrdinalIgnoreCase) ||
                    paramMode.Equals("INOUT", StringComparison.OrdinalIgnoreCase))
                {
                    param.Direction = ParameterDirection.Output;
                }

                parameters[i] = param;
            }

            return parameters;
        }

        //4 : To execute any stored procedure dynamically and
        //return the result set as a DataTable, without hardcoding parameter definitions.
        public static DataTable GetDataTable(string storedProcedure, sqlparam obj)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(CommClass.Connection))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(storedProcedure, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter[] spParams = GetParameter(storedProcedure, obj);
                        cmd.Parameters.AddRange(spParams);

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                
                MessageBox.Show("Error loading combobox: " + ex.Message);
                return null;
            }
        }


        //2 :  It(GetStoredProcedureParameters) finds out what parameters exist in the given stored procedure.
        private static DataTable GetStoredProcedureParameters(string spName)
        {
            using (SqlConnection con = new SqlConnection(CommClass.Connection))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("USP_GetStoredProcedureParameters", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SPName", spName);

                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                    return dt;
                }
                con.Close();
            }
        }      
    }

    //1
    //You’ve written a helper class (CommonHelper) that is designed to dynamically 
    //fetch stored procedure parameters from SQL Server and map them automatically to your C# object properties.
    //sqlparam :your data bag whose properties map to SP parameters.
    public class sqlparam
    {
        public string status { get; set; }
        public string stateName { get; set; }
        public string CustomerName { get; set; }
        public string MobileNumber { get; set; }
        public string Product_Name { get; set; }
        //public string Product_Code { get; set; }
        public string MobileNo { get; set; }
        public string Customer_Address { get; set; }
        public int  Customer_State_id { get; set; }
        public string SalesInvoice_Date { get; set; }
        public string SalesInvoice_Number{get;set;}
        public decimal GrossAmount { get; set; }
        public decimal DiscountPer { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal Roundoff { get; set; }
        public decimal Total_Amount { get; set; }
        public int Sate_Id { get; set; }
        public int Customer_Id { get; set; }
        public int sid_SIH_id { get; set; }
        public int sid_mm_id { get; set; }
        public int sid_Qty { get; set; }
        public int sid_FreeQty { get; set; }
        public decimal sid_Discount { get; set; }
        public decimal sid_DiscountAmt { get; set; }
        public decimal sid_SubTotal { get; set; }
        public decimal sid_NetAmount { get; set; }
        public int ProductCode { get; set; }
        public int ProductQty { get; set; }
        public decimal SalesInvoiceNumber { get; set; }

    }
}
