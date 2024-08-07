using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace scheduler
{
    /// <summary>
    /// Interaction logic for UpdateCustomer.xaml
    /// </summary>
    public partial class UpdateCustomer : Window
    {
        public UpdateCustomer(Customer customer)
        {
            InitializeComponent();
            mySQLDB mySQLDB = new mySQLDB();
            List<Address> allAddressList = mySQLDB.SelectAllAddresses();
            List<string> addressList = new List<string>();
            foreach (var address in allAddressList)
            {
                addressList.Add((String.Format(address.address)) + ", " + (String.Format(address.address2)) + ", " + (mySQLDB.GetCityNameFromCityID(address.cityID)));
            }

            updateCustAddressComboBox.ItemsSource = addressList;
            updateCustCustIDTextBox.Text = customer.Id.ToString();
            updateCustFullNameTextBox.Text = customer.customerName;
            updateCustAddressComboBox.SelectedIndex = (customer.customerAddressID) - 1;
        }

        private void updateCustAddressComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mySQLDB mySQLDB = new mySQLDB();
            char[] separators = new char[] { ',' };
            string[] addressData = updateCustAddressComboBox.SelectedItem.ToString().Split(separators, StringSplitOptions.RemoveEmptyEntries);
            int cityID = mySQLDB.GetCityIDFromCityName(addressData[2].Trim());
            if (updateCustAddressIDTextBox.Text == "")
            {
                updateCustAddressIDTextBox.Text = mySQLDB.GetAddressIDFromAddress(addressData[0], addressData[1].Trim(), cityID).ToString();
            }
            else
            {
                updateCustAddressIDTextBox.Text = "";
                updateCustAddressIDTextBox.Text = mySQLDB.GetAddressIDFromAddress(addressData[0], addressData[1].Trim(), cityID).ToString();
            }
        }

        private void newAddressButton_Click(object sender, RoutedEventArgs e)
        {
            new AddAddress().ShowDialog();
        }

        private void saveCustButton_Click(object sender, RoutedEventArgs e)
        {
            mySQLDB mySQLDB = new mySQLDB();
            bool proceed = false;
            try
            {
                if (updateCustAddressComboBox.SelectedItem == null || updateCustFullNameTextBox.Text.Length == 0)
                {
                    proceed = false;
                    throw new ApptException();
                }
                else
                {
                    proceed = true;
                    int customerId = int.Parse(updateCustCustIDTextBox.Text);
                    string customerName = updateCustFullNameTextBox.Text.Trim();
                    int addressId = int.Parse(updateCustAddressIDTextBox.Text);
                    string address = updateCustAddressComboBox.SelectedItem.ToString().Trim();
                    char[] separators = new char[] { ',' };
                    string[] addressData = address.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    string address1 = addressData[0].Trim();
                    string phone = updateCustPhoneTextBox.Text.Trim();

                    if (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^[0-9-]+$"))
                    {
                        MessageBox.Show("Invalid phone number format. Only digits and dashes are allowed.");
                        return;
                    }

                    mySQLDB.UpdateCustomer(customerId, customerName, addressId, address1, phone);
                }
            }
            catch (ApptException exception)
            {
                exception.CustomerDataException();
            }
            if (proceed)
            {
                new CustomersPage().Show();
                this.Close();
            }
        }
    }
}
