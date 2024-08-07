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
    /// Interaction logic for AddCustomer.xaml
    /// </summary>
    public partial class AddCustomer : Window
    {
        public AddCustomer()
        {
            InitializeComponent();
            mySQLDB mySQLDB = new mySQLDB();
            List<Address> allAddressList = mySQLDB.SelectAllAddresses();
            List<string> addressList = new List<string>();
            foreach (var address in allAddressList)
            {
                addressList.Add((String.Format(address.address)) + ", " + (String.Format(address.address2)) + ", " + (mySQLDB.GetCityNameFromCityID(address.cityID)));
            }

            addCustAddressComboBox.ItemsSource = addressList;
        }

        private void saveCustButton_Click(object sender, RoutedEventArgs e)
        {
            mySQLDB mySQLDB = new mySQLDB();
            bool proceed = false;
            try
            {
                if (addCustAddressComboBox.SelectedItem == null || addCustFullNameTextBox.Text.Length == 0)
                {
                    proceed = false;
                    throw new ApptException();
                }
                else
                {
                    proceed = true;
                    string customerName = addCustFullNameTextBox.Text.Trim();
                    int addressId = int.Parse(addCustAddressIDTextBox.Text);

                    // Insert customer
                    mySQLDB.InsertCustomer(customerName, addressId);
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



        private void newAddressButton_Click(object sender, RoutedEventArgs e)
        {
            new AddAddress().ShowDialog();
            this.Close();
        }

        private void addCustAddressComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mySQLDB mySQLDB = new mySQLDB();
            char[] separators = new char[] { ',' };
            string[] addressData = addCustAddressComboBox.SelectedItem.ToString().Split(separators, StringSplitOptions.RemoveEmptyEntries);
            int cityID = mySQLDB.GetCityIDFromCityName(addressData[2].Substring(1));
            if (addCustAddressIDTextBox.Text == "")
            {
                addCustAddressIDTextBox.Text = mySQLDB.GetAddressIDFromAddress(addressData[0], addressData[1].Substring(1), cityID).ToString();
            }
            else
            {
                addCustAddressIDTextBox.Text = "";
                addCustAddressIDTextBox.Text = mySQLDB.GetAddressIDFromAddress(addressData[0], addressData[1], cityID).ToString();
            }
        }
    }
}
