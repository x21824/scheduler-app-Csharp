using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
namespace scheduler
{
    ///
    /// Interaction logic for CustomersPage.xaml
    ///
    public partial class CustomersPage : Window
    {
        private bool isDataGridLoaded = false;

        public CustomersPage()
        {
            InitializeComponent();
            Loaded += Page_Loaded;
            customerDataGrid.Loaded += customerDataGrid_Loaded;
            mySQLDB mySQLDB = new mySQLDB();
            System.Collections.Generic.List<Customer> customersList = mySQLDB.SelectAllCustomersAndAddressData();

            CollectionViewSource CustomerCollectionViewSource;
            CustomerCollectionViewSource = (CollectionViewSource)(FindResource("CustomerCollectionViewSource"));
            CustomerCollectionViewSource.Source = customersList;

        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            customerDataGrid.SelectedItem = null;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text.Count() > 0)
            {
                mySQLDB mySQLDB = new mySQLDB();
                List<Customer> customersList = mySQLDB.SelectAllCustomersAndAddressData();
                List<Customer> filteredCustomersList = new List<Customer>();

                foreach (Customer customer in customersList)
                {
                    if (customer.customerName.ToLower().Contains(SearchBox.Text.ToLower()) || customer.address.ToLower().Contains(SearchBox.Text.ToLower()) || customer.address2.ToLower().Contains(SearchBox.Text.ToLower()) || customer.city.ToLower().Contains(SearchBox.Text.ToLower()) || customer.postal.ToLower().Contains(SearchBox.Text.ToLower()) || customer.country.ToLower().Contains(SearchBox.Text.ToLower()) || customer.phone.ToLower().Contains(SearchBox.Text.ToLower()))
                    {
                        filteredCustomersList.Add(customer);
                    }
                }

                CollectionViewSource CustomerCollectionViewSource;
                CustomerCollectionViewSource = (CollectionViewSource)(FindResource("CustomerCollectionViewSource"));
                CustomerCollectionViewSource.Source = filteredCustomersList;
            }
            else
            {
                mySQLDB mySQLDB = new mySQLDB();
                List<Customer> customersList = mySQLDB.SelectAllCustomersAndAddressData();

                CollectionViewSource CustomerCollectionViewSource;
                CustomerCollectionViewSource = (CollectionViewSource)(FindResource("CustomerCollectionViewSource"));
                CustomerCollectionViewSource.Source = customersList;
            }
            SearchBox.Text = "";
        }

        private void SearchBox_EnterKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchButton_Click(sender, e);
            }
        }

        private void apptButton_Click(object sender, RoutedEventArgs e)
        {
            new AppointmentsPage().Show();
            this.Close();
        }

        private void customerButton_Click(object sender, RoutedEventArgs e)
        {
            new CustomersPage().Show();
            this.Close();
        }

        private void calendarButton_Click(object sender, RoutedEventArgs e)
        {
            new CalendarPage().Show();
            this.Close();
        }

        private void homeButton_Click(object sender, RoutedEventArgs e)
        {
            new HomePage().Show();
            this.Close();
        }
        private void reportsButton_Click(object sender, RoutedEventArgs e)
        {
            new ReportsPage().Show();
            this.Close();
        }

        private void customerAddButton_Click(object sender, RoutedEventArgs e)
        {
            new AddCustomer().Show();
            this.Close();
        }

        private void customerUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = (Customer)customerDataGrid.SelectedItem;
            new UpdateCustomer(customer).Show();
            this.Close();
        }

        private void customerDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (customerDataGrid.SelectedItem == null || !(customerDataGrid.SelectedItem is Customer))
            {
                System.Windows.MessageBox.Show("Please select a customer to delete.");
                return;
            }

            try
            {
                var warning = System.Windows.Forms.MessageBox.Show("Are you sure you want to delete the selected Customer and any appointments linked to this Customer? This is permanent!", "scheduler - Confirm Delete?", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (warning == System.Windows.Forms.DialogResult.OK)
                {
                    mySQLDB mySQLDB = new mySQLDB();
                    Customer cust = (Customer)customerDataGrid.SelectedItem;
                    mySQLDB.DeleteCustomerAppointments(cust.Id);
                    mySQLDB.DeleteCustomer(cust.Id);
                    new CustomersPage().Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("An error occurred while deleting the customer record: " + ex.Message);
            }

        }


        private void customerDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            isDataGridLoaded = true;
            customerDataGrid.Columns[1].Visibility = Visibility.Hidden;
            customerDataGrid.Columns[2].Visibility = Visibility.Hidden;
            customerDataGrid.Columns[9].Visibility = Visibility.Hidden;
            customerDataGrid.Columns[10].Visibility = Visibility.Hidden;
            customerDataGrid.Columns[11].Visibility = Visibility.Hidden;
            customerDataGrid.Columns[12].Visibility = Visibility.Hidden;
            customerDataGrid.Columns[13].Visibility = Visibility.Hidden;
            customerDataGrid.Columns[0].Header = "Customer Name";
            customerDataGrid.Columns[3].Header = "Address";
            customerDataGrid.Columns[4].Header = "Address 2";
            customerDataGrid.Columns[5].Header = "City";
            customerDataGrid.Columns[6].Header = "Postal Code";
            customerDataGrid.Columns[7].Header = "Country";
            customerDataGrid.Columns[8].Header = "Phone Number";
        }
    }
}