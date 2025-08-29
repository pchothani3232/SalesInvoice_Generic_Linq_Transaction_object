using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;

namespace DatabaseConfiguration
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();

        }

        //string filePath = "D://PRIYA.CHOTHANI//dbconfig2.txt";
      
        //Login -Button
        private void btnLogin_Click(object sender, EventArgs e)       
        {
            //Encrpt/Decrypted data
            string plainPassword = txtPwd.Text;

            // Encrypt
            string encrypted = EncryptionHelper.Encrypt(plainPassword);

            // Decrypt
            string decrypted = EncryptionHelper.Decrypt(encrypted);

            //MessageBox.Show($"Encrypted: {encrypted}\nDecrypted: {decrypted}");




            if (string.IsNullOrWhiteSpace(txtUnm.Text) || string.IsNullOrWhiteSpace(txtPwd.Text))
            {
                MessageBox.Show("Please enter both Username and Password");
                return;
            }



            using (SqlConnection con = new SqlConnection(CommClass.Connection))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("LoginCredential", con);  //LoginCredential = stored procedure name
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Username", txtUnm.Text);
                    cmd.Parameters.AddWithValue("@Password", txtPwd.Text);
                    cmd.Parameters.AddWithValue("@status", "Select");
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    con.Close();

                    if (dt.Rows.Count > 0)
                    {
                        //MessageBox.Show("Login Successful");
                        this.Hide();
                        SalesInvoiceCustomerListPage l1 = new SalesInvoiceCustomerListPage();
                        l1.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Login not Successful");
                        txtUnm.Clear();
                        txtPwd.Clear();
                      
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Connection Failed:\n" + ex.Message);
                    return;
                }
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            btnShowPwd.Text = "🙈";
        }

        private void btnShowPwd_Click(object sender, EventArgs e)
        {
            txtPwd.UseSystemPasswordChar = !txtPwd.UseSystemPasswordChar;

            if (txtPwd.UseSystemPasswordChar)
                btnShowPwd.Text = "🙈";  // Hide mode
            else
                btnShowPwd.Text = "👁";  // Show mode
        }
    }
}
 
