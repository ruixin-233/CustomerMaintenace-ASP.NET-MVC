using System;
using MMABooksData.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Customer_aintenanceGUI
{
    public partial class frmAddModffyCustomer : Form
    {
        public frmAddModffyCustomer()
        {
            InitializeComponent();
        }
        // public fields (accessible from the main form)
        public bool isAdd; // main form sets it; to distinguish add from modify
        public Customers customer;  // main form sets it
        public List<States> states; // main form sets it

        private void frmAddModffyCustomer_Load(object sender, EventArgs e)
        {
            // populate the states combo box
            cboState.DataSource = this.states;
            cboState.DisplayMember = "StateName";
            cboState.ValueMember = "StateCode";

                if (this.isAdd) // is add
                {
                    this.Text = "Add Customer";
                }

                else    // is modify
                {
                    this.Text = "Modify Customer";
                    // display current customer data
                    if (customer == null)
                    {
                        MessageBox.Show("There is no current customer", "Modify Error");
                        this.Close();
                    }
                    txtAddress.Text = customer.Address;
                    txtCity.Text = customer.City;
                    txtName.Text = customer.Name;
                    cboState.SelectedValue = customer.State;    // state of current customer
                    txtZipCode.Text = customer.ZipCode;
                }
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {

            // Validator on the top, but Im too lazy

            if (isAdd)
            {
                customer = new Customers();
            }

            // if modify, object is already there
            // load data from form control into customer object
            customer.Address = txtAddress.Text;
            customer.City = txtCity.Text ;
            customer.Name = txtName.Text ;
            customer.State = cboState.SelectedValue.ToString() ;    // state of current customer
            customer.ZipCode = txtZipCode.Text;

            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
