using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections;
namespace scheduler
{
    ///
    /// Interaction logic for AddAppointment.xaml
    ///
    public partial class AddAppointment : Window
    {
        public AddAppointment()
        {
            InitializeComponent();
            mySQLDB mySQLDB = new mySQLDB();
            var customers = mySQLDB.SelectAllCustomers();
            ArrayList allCustomersList = new ArrayList(customers);
            ArrayList custNamesList = new ArrayList();
            foreach (var customer in allCustomersList)
            {
                custNamesList.Add(((scheduler.Customer)customer).customerName);
            }

            addApptCustomerComboBox.ItemsSource = custNamesList;
        }

        private void newCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            new AddCustomer().ShowDialog();
        }

        private static bool CheckApptHasConflict(DateTime start, DateTime end)
        {
            mySQLDB mySQLDB = new mySQLDB();
            int countApptsAtTime = mySQLDB.CheckForApptsAtTime(start, end);
            var appointments = mySQLDB.SelectCurrentUIDAppointments();
            ArrayList apptList = new ArrayList(appointments);
            if (countApptsAtTime > 0)
            {
                return true;
            }
            foreach (var appt in apptList)
            {
                if (start <= ((scheduler.Appointment)appt).startDateTime && end >= ((scheduler.Appointment)appt).endDateTime)
                {
                    return true;
                }
                if (start >= ((scheduler.Appointment)appt).startDateTime && start < ((scheduler.Appointment)appt).endDateTime)
                {
                    return true;
                }
                if (end > ((scheduler.Appointment)appt).startDateTime && end <= ((scheduler.Appointment)appt).endDateTime)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool CheckApptOutsideBusHours(DateTime start, DateTime end)
        {
            DateTime bizStartTime = DateTime.Today.AddHours(9);
            DateTime bizEndTime = DateTime.Today.AddHours(17);
            if (start.TimeOfDay < bizStartTime.TimeOfDay || end.TimeOfDay > bizEndTime.TimeOfDay)
            {
                return true;
            }
            if (start.DayOfWeek == DayOfWeek.Saturday || start.DayOfWeek == DayOfWeek.Sunday)
            {
                return true;
            }
            return false;
        }

        private void saveApptButton_Click(object sender, RoutedEventArgs e)
        {
            mySQLDB mySQLDB = new mySQLDB();
            DateTime now = DateTime.UtcNow;
            int custID = mySQLDB.GetCustomerIDFromCustomerName(addApptCustomerComboBox.SelectedItem.ToString());
            int userID = mySQLDB.GetLoggedInUID();
            string title = addApptTitleTextBox.Text;
            string desc = addApptDescriptionTextBox.Text;
            string loc = addApptLocationTextBox.Text;
            string contact = addApptContactTextBox.Text;
            string type = addApptTypeTextBox.Text;
            string url = addApptURLTextBox.Text;

            DateTime startDate = (DateTime)addApptStartDateBox.SelectedDate;
            DateTime startTime = DateTime.ParseExact(addApptStartHour.Text + ":" + addApptStartMin.Text + " " + addApptStartAMPM.Text, "h:mm tt", null);
            DateTime start = startDate.Date + startTime.TimeOfDay;

            DateTime endDate = (DateTime)addApptEndDateBox.SelectedDate;
            DateTime endTime = DateTime.ParseExact(addApptEndHour.Text + ":" + addApptEndMin.Text + " " + addApptEndAMPM.Text, "h:mm tt", null);
            DateTime end = endDate.Date + endTime.TimeOfDay;

            string username = mySQLDB.GetLoggedInUName();

            try
            {
                if (CheckApptHasConflict(start, end))
                {
                    throw new ApptException();
                }
                else
                {
                    try
                    {
                        if (CheckApptOutsideBusHours(start, end))
                        {
                            throw new ApptException();
                        }
                        else
                        {
                            try
                            {
                                if (type.Length == 0)
                                {
                                    throw new ApptException();
                                }
                                else
                                {
                                    try
                                    {
                                        if (!mySQLDB.InsertNewAppointment(custID, userID, title, desc, loc, contact, type, url, start, end, now, username, now, username))
                                        {
                                            throw new ApptException();
                                        }
                                        else
                                        {
                                            new AppointmentsPage().Show();
                                            this.Close();
                                        }
                                    }
                                    catch (ApptException exception)
                                    {
                                        exception.ApptGeneralException();
                                    }
                                }
                            }
                            catch (ApptException exception)
                            {
                                exception.ApptGeneralException();
                            }
                        }
                    }
                    catch (ApptException exception)
                    {
                        exception.ApptOutOfBusHoursException();
                    }
                }
            }
            catch (ApptException exception)
            {
                exception.ApptConflictException();
            }

        }
    }
}
