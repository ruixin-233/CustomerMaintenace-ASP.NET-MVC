using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MMABooksData.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Customer_aintenanceGUI
{
    public partial class frmCustomerMaintenance : Form
    {
        private Customers selectedCustomer = null;
        private List<Invoices> invoices = null;
        private MMABooksContext db = new MMABooksContext();
        

        public frmCustomerMaintenance()
        {
            InitializeComponent();
        }

        private void frmCustomerMaintenance_Load(object sender, EventArgs e)
        {
            //ClearControls();
            dgvInvoices.DataSource = invoices;
        }

        // retrieve data of customer with provided ID
        private void btnGetCustomer_Click(object sender, EventArgs e)
        {
            if (Validator.IsPresent(txtCustomerID) &&
                Validator.IsNonNegativeInt(txtCustomerID))
            {
                int customerID = Convert.ToInt32(txtCustomerID.Text);

                try
                {
                    selectedCustomer = db.Customers.Find(customerID);   // LinQ: search by PK value

                    if(selectedCustomer !=null)
                    {
                        invoices = selectedCustomer.Invoices.ToList();  // lazy loding get invoices from this customer
                        // dgvInvoices.DataSource = invoices;
                        DisplayCustomer();
                    }
                    else
                    {
                        MessageBox.Show("No customer found with ID" + customerID, 
                            "Customer not found");
                        
                        ClearControls();
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error when getting customer data: " + ex.Message,
                        ex.GetType().ToString());
                }
            }
        }

        // display current customer data
        private void DisplayCustomer()
        {
            txtCustomerID.Text = selectedCustomer.CustomerId.ToString();
            txtName.Text = selectedCustomer.Name;
            txtAddress.Text = selectedCustomer.Address;
            txtCity.Text = selectedCustomer.City;
            txtState.Text = selectedCustomer.State;
            txtZipCode.Text = selectedCustomer.ZipCode;
            
            // display invoices
            dgvInvoices.DataSource = invoices;

            // enable modify and delete button
            btnModify.Enabled = true;
            btnDelete.Enabled = true;

            // focus on customer ID text box
            txtCustomerID.Focus();
        }

        private void ClearControls()
        {
            // erase text boxes
            txtCustomerID.Text = "";
            txtName.Text = "";
            txtAddress.Text = "";
            txtCity.Text = "";
            txtState.Text = "";
            txtZipCode.Text = "";
            dgvInvoices.DataSource = null;

            // display modify and delete buttons
            btnModify.Enabled = false;
            btnDelete.Enabled = false;

            // focus on customer ID text box
            txtCustomerID.Focus();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedCustomer != null)
            {
                // get the confirmation from the user
                DialogResult answer = MessageBox.Show($"Do you want to delete {selectedCustomer.Name}?",
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if(answer == DialogResult.Yes)  // user confirmed
                {
                    try
                    {
                        db.Customers.Remove(selectedCustomer);
                        db.SaveChanges();
                        ClearControls(); // no selected customer
                    }

                    catch (DbUpdateException ex)    // database error
                    {
                        // get inner exception with potentially multiple errors
                        SqlException innerException = (SqlException)ex.InnerException;
                        string message = "";
                        foreach(SqlError err in innerException.Errors)
                        {
                            message += $"Error code: {err.Number} - {err.Message} \n";
                        }

                        MessageBox.Show(message, "Datbase error(s) when deleting customer");
                    }

                    catch (Exception ex)
                    {
                        MessageBox.Show("Other error when deleting customer: " + ex.Message,
                            ex.GetType().ToString());
                    }
                }
            }
            else // no selected customer
            {
                MessageBox.Show("You need to customer first", "Delete Aborted");
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            frmAddModffyCustomer secondForm = new frmAddModffyCustomer();
            secondForm.isAdd = true;
            secondForm.customer = null;

            // get the states
            try
            {
                secondForm.states = db.States.OrderBy(s => s.StateName).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Other error when retrieving states: " + ex.Message,
                    ex.GetType().ToString());
            }

            DialogResult result = secondForm.ShowDialog();

            if(result == DialogResult.OK)
            {
                selectedCustomer = secondForm.customer;
                try
                {
                    db.Customers.Add(selectedCustomer);
                    db.SaveChanges();
                    DisplayCustomer();
                }

                catch (Exception ex)
                {
                    MessageBox.Show("Other error when adding customer: " + ex.Message,
                        ex.GetType().ToString());
                }
            }
        }   // add method

        private void btnModify_Click(object sender, EventArgs e)
        {
            frmAddModffyCustomer secondForm = new frmAddModffyCustomer();
            secondForm.isAdd = false;
            secondForm.customer = selectedCustomer;

            // get the states
            try
            {
                secondForm.states = db.States.OrderBy(s => s.StateName).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Other error when retrieving states: " + ex.Message,
                    ex.GetType().ToString());
            }

            // 
            DialogResult result = secondForm.ShowDialog();

            if (result == DialogResult.OK)  // second form has customer object with data
            {
                selectedCustomer = secondForm.customer;
                try
                {
                    // no need to call update - selected customer is in db.Customers
                    db.SaveChanges();
                    DisplayCustomer();
                }

                catch (Exception ex)
                {
                    MessageBox.Show("Other error when adding customer: " + ex.Message,
                        ex.GetType().ToString());
                }
            }
        }
    }
}
