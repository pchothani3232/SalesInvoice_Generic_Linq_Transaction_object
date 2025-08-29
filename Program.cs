using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Data;

namespace DatabaseConfiguration
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            CommClass.filePath = "D://PRIYA.CHOTHANI//dbconfig4.txt";

            if (!File.Exists(CommClass.filePath))
            {
                DatabaseConfigurationForm f1 = new DatabaseConfigurationForm();
                f1.ShowDialog();
                if (f1.DialogResult == DialogResult.Cancel)
                {
                    return;
                }
            }
            else
            {
                CommClass.Connection = File.ReadAllText(CommClass.filePath);
                SqlConnection con = new SqlConnection(CommClass.Connection);
                try
                {
                    con.Open();
                }
                catch (SqlException ex)
                {
                    DatabaseConfigurationForm f1 = new DatabaseConfigurationForm();
                    f1.ShowDialog();
                    if (f1.DialogResult == DialogResult.Cancel)
                    {
                        return;
                    }
                }
                finally
                {
                    con.Close();
                }
            }

            Application.Run(new SalesInvoice());
            //Application.Run(new SalesInvoiceCustomerListPage());
            //Application.Run(new Form1_test());
            //Application.Run(new LoginForm());           
        }

    }
}
