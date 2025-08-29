using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace DatabaseConfiguration
{
    public partial class DatabaseConfigurationForm : Form
    {
        public DatabaseConfigurationForm()
        {
            InitializeComponent();
        }
       
        string filePath = "D://PRIYA.CHOTHANI//dbconfig4.txt";

        private void Form1_Load(object sender, EventArgs e)
        {
          
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Step 1: Get user input
            string server = CmbServerName.Text;
            string user = txtUnm.Text;
            string pass = txtPwd.Text;
            string db = txtDb.Text;

            // Step 2: Validate inputs
            if (server == "" || user == "" || pass == "" || db == "")
            {
                MessageBox.Show("Please fill in all fields.", "Warning");
            }
            else
            {
                // Step 3: Build connection string
                string connStr = $"Data Source={server};Initial Catalog={db};User ID={user};Password={pass}";

                SqlConnection con = new SqlConnection(connStr);
                con.Open();

                // Step 4: Check if connection is open
                if (con.State == ConnectionState.Open)
                {
                    if(!File.Exists(CommClass.filePath))
                    {
                        File.WriteAllText(CommClass.filePath, connStr);
                        MessageBox.Show("Connection successful and saved!", "Success");
                    }
                    else
                    {
                        File.WriteAllText(CommClass.filePath, connStr); // overwrite if exists
                        MessageBox.Show("File already exists. Connection updated!", "Updated");
                    }
                    con.Close();
                    CommClass.Connection = connStr;
                    this.DialogResult = DialogResult.OK;
                    this.Close();                   
                }
                else
                {
                    MessageBox.Show("Connection failed.", "Error");
                }       
            }
        }
    }
}

