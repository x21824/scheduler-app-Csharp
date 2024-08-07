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
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Window
    {
        private static DateTime _loginTime;
        public HomePage()
        {
            InitializeComponent();
            DateTime loginTime = DateTime.Now;
            _loginTime = loginTime;
            DateTime reminderTime = loginTime.AddMinutes(15);

            mySQLDB mySQLDB = new mySQLDB();
            List<Appointment> allAppointmentsList = mySQLDB.SelectAllAppointments();
            List<Appointment> upcomingAppointmentsList = new List<Appointment>();
            foreach (Appointment appointment in allAppointmentsList)
            {
                appointment.startDateTime = appointment.startDateTime.ToLocalTime();
                appointment.endDateTime = appointment.endDateTime.ToLocalTime();
                appointment.Created = appointment.Created.ToLocalTime();
                appointment.Modified = appointment.Modified.ToLocalTime();
                if (appointment.startDateTime <= reminderTime && appointment.startDateTime >= loginTime)
                {
                    upcomingAppointmentsList.Add(appointment);
                }
            }

            CollectionViewSource AppointmentCollectionViewSource;
            AppointmentCollectionViewSource = (CollectionViewSource)(FindResource("AppointmentCollectionViewSource"));
            AppointmentCollectionViewSource.Source = upcomingAppointmentsList;

            if (upcomingAppointmentsList.Count == 0)
            {
                homeDataGrid.Visibility = Visibility.Hidden;
                noAppointmentsLabel.Visibility = Visibility.Visible;
                noAppointmentsRectangle.Visibility = Visibility.Visible;
            }
        }
        private void homeDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            homeDataGrid.Columns[0].Visibility = Visibility.Hidden;
            homeDataGrid.Columns[1].Visibility = Visibility.Hidden;
            homeDataGrid.Columns[10].Visibility = Visibility.Hidden;
            homeDataGrid.Columns[11].Visibility = Visibility.Hidden;
            homeDataGrid.Columns[12].Visibility = Visibility.Hidden;
            homeDataGrid.Columns[13].Visibility = Visibility.Hidden;
            homeDataGrid.Columns[14].Visibility = Visibility.Hidden;

            homeDataGrid.Columns[2].Header = "Title";
            homeDataGrid.Columns[3].Header = "Description";
            homeDataGrid.Columns[4].Header = "Location";
            homeDataGrid.Columns[5].Header = "Contact";
            homeDataGrid.Columns[6].Header = "Type";
            homeDataGrid.Columns[7].Header = "URL";
            homeDataGrid.Columns[8].Header = "Start Date & Time";
            homeDataGrid.Columns[9].Header = "End Date & Time";
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

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text.Count() > 0)
            {
                DateTime reminderTime = _loginTime.AddMinutes(15);
                mySQLDB mySQLDB = new mySQLDB();
                List<Appointment> allAppointmentsList = mySQLDB.SelectAllAppointments();
                List<Appointment> upcomingAppointmentsList = new List<Appointment>();
                List<Appointment> filteredAppointmentsList = new List<Appointment>();
                foreach (Appointment appointment in allAppointmentsList)
                {
                    appointment.startDateTime = appointment.startDateTime.ToLocalTime();
                    appointment.endDateTime = appointment.endDateTime.ToLocalTime();
                    appointment.Created = appointment.Created.ToLocalTime();
                    appointment.Modified = appointment.Modified.ToLocalTime();
                    if (appointment.startDateTime <= reminderTime && appointment.startDateTime >= _loginTime)
                    {
                        upcomingAppointmentsList.Add(appointment);
                    }
                }
                foreach (Appointment appointment in upcomingAppointmentsList)
                {
                    if (appointment.apptTitle.ToLower().Contains(SearchBox.Text.ToLower()) || appointment.apptDescription.ToLower().Contains(SearchBox.Text.ToLower()) || appointment.apptContact.ToLower().Contains(SearchBox.Text.ToLower()) || mySQLDB.GetCustomerNameFromCustomerID(appointment.apptCustID).ToLower().Contains(SearchBox.Text.ToLower()) || appointment.apptLocation.ToLower().Contains(SearchBox.Text.ToLower()) || appointment.apptType.ToLower().Contains(SearchBox.Text.ToLower()) || appointment.apptURL.ToLower().Contains(SearchBox.Text.ToLower()))
                    {
                        filteredAppointmentsList.Add(appointment);
                    }
                }

                CollectionViewSource AppointmentCollectionViewSource;
                AppointmentCollectionViewSource = (CollectionViewSource)(FindResource("AppointmentCollectionViewSource"));
                AppointmentCollectionViewSource.Source = filteredAppointmentsList;

                if (filteredAppointmentsList.Count == 0)
                {
                    homeDataGrid.Visibility = Visibility.Hidden;
                    noAppointmentsLabel.Content = "No Upcoming Appointments found matching search!";
                    noAppointmentsLabel.Visibility = Visibility.Visible;
                    noAppointmentsRectangle.Visibility = Visibility.Visible;
                }
                if (filteredAppointmentsList.Count != 0)
                {
                    homeDataGrid.Visibility = Visibility.Visible;
                    noAppointmentsLabel.Visibility = Visibility.Hidden;
                    noAppointmentsRectangle.Visibility = Visibility.Hidden;
                }
                homeDataGrid.Columns[0].Visibility = Visibility.Hidden;
                homeDataGrid.Columns[1].Visibility = Visibility.Hidden;
                homeDataGrid.Columns[10].Visibility = Visibility.Hidden;
                homeDataGrid.Columns[11].Visibility = Visibility.Hidden;
                homeDataGrid.Columns[12].Visibility = Visibility.Hidden;
                homeDataGrid.Columns[13].Visibility = Visibility.Hidden;
                homeDataGrid.Columns[14].Visibility = Visibility.Hidden;

                homeDataGrid.Columns[2].Header = "Title";
                homeDataGrid.Columns[3].Header = "Description";
                homeDataGrid.Columns[4].Header = "Location";
                homeDataGrid.Columns[5].Header = "Contact";
                homeDataGrid.Columns[6].Header = "Type";
                homeDataGrid.Columns[7].Header = "URL";
                homeDataGrid.Columns[8].Header = "Start Date & Time";
                homeDataGrid.Columns[9].Header = "End Date & Time";

                SearchBox.Text = "";
            }
        }

        private void SearchBox_EnterKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchButton_Click(sender, e);
            }
        }
    }
}
