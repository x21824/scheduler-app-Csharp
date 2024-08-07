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
    /// Interaction logic for UpdateAppointment.xaml
    /// </summary>
    public partial class UpdateAppointment : Window
    {
        public UpdateAppointment(Appointment appt)
        {
            InitializeComponent();

            mySQLDB mySQLDB = new mySQLDB();
            List<Customer> allCustomersList = mySQLDB.SelectAllCustomers();
            List<string> custNamesList = new List<string>();
            foreach (var customer in allCustomersList)
            {
                custNamesList.Add(String.Format(customer.customerName));
            }
            updateApptCustomerComboBox.ItemsSource = custNamesList;

            updateApptTitleTextBox.Text = appt.apptTitle;
            updateApptDescriptionTextBox.Text = appt.apptDescription;
            updateApptLocationTextBox.Text = appt.apptLocation;
            updateApptContactTextBox.Text = appt.apptContact;
            updateApptTypeTextBox.Text = appt.apptType;
            updateApptURLTextBox.Text = appt.apptURL;
            updateApptStartDateBox.SelectedDate = appt.startDateTime.ToLocalTime().Date;
            int startHour = appt.startDateTime.ToLocalTime().Hour;
            int startMin = appt.startDateTime.ToLocalTime().Minute;
            updateApptEndDateBox.SelectedDate = appt.endDateTime.ToLocalTime().Date;
            int endHour = appt.endDateTime.ToLocalTime().Hour;
            int endMin = appt.endDateTime.ToLocalTime().Minute;
            updateApptCustomerComboBox.SelectedItem = mySQLDB.GetCustomerNameFromCustomerID(appt.apptCustID);
            mySQLDB.SetAppointmentID(appt.Id);

            if (startHour == 0 || startHour == 12)
            {
                updateApptStartHour.SelectedIndex = 11;
                if (startHour == 12)
                {
                    updateApptStartAMPM.SelectedIndex = 1;

                }
                else
                {
                    updateApptStartAMPM.SelectedIndex = 0;
                }

            }
            else if (startHour >= 1 && startHour <= 11)
            {
                updateApptStartAMPM.SelectedIndex = 0;
                updateApptStartHour.SelectedIndex = startHour - 1;
            }
            else if (startHour >= 13)
            {
                updateApptStartHour.SelectedIndex = (startHour - 12) - 1;
                updateApptStartAMPM.SelectedIndex = 1;
            }

            if (startMin == 0)
            {
                updateApptStartMin.SelectedIndex = startMin;
            }
            else if (startMin == 15)
            {
                updateApptStartMin.SelectedIndex = 1;
            }
            else if (startMin == 30)
            {
                updateApptStartMin.SelectedIndex = 2;
            }
            else if (startMin == 45)
            {
                updateApptStartMin.SelectedIndex = 3;
            }

            if (endHour == 0 || endHour == 12)
            {
                updateApptEndHour.SelectedIndex = 11;
                if (endHour == 12)
                {
                    updateApptEndAMPM.SelectedIndex = 1;

                }
                else
                {
                    updateApptEndAMPM.SelectedIndex = 0;
                }

            }
            else if (endHour >= 1 && endHour <= 11)
            {
                updateApptEndAMPM.SelectedIndex = 0;
                updateApptEndHour.SelectedIndex = endHour - 1;
            }
            else if (endHour >= 13)
            {
                updateApptEndHour.SelectedIndex = (endHour - 12) - 1;
                updateApptEndAMPM.SelectedIndex = 1;
            }

            if (endMin == 0)
            {
                updateApptEndMin.SelectedIndex = endMin;
            }
            else if (endMin == 15)
            {
                updateApptEndMin.SelectedIndex = 1;
            }
            else if (endMin == 30)
            {
                updateApptEndMin.SelectedIndex = 2;
            }
            else if (endMin == 45)
            {
                updateApptEndMin.SelectedIndex = 3;
            }

        }

        private void saveApptButton_Click(object sender, RoutedEventArgs e)
        {
            mySQLDB mySQLDB = new mySQLDB();
            DateTime now = DateTime.UtcNow;
            int apptID = mySQLDB.GetAppointmentID();
            int custID = mySQLDB.GetCustomerIDFromCustomerName(updateApptCustomerComboBox.SelectedItem.ToString());
            int userID = mySQLDB.GetLoggedInUID();
            string title = updateApptTitleTextBox.Text;
            string desc = updateApptDescriptionTextBox.Text;
            string loc = updateApptLocationTextBox.Text;
            string contact = updateApptContactTextBox.Text;
            string type = updateApptTypeTextBox.Text;
            string url = updateApptURLTextBox.Text;

            string startDateSelectedValue = updateApptStartDateBox.SelectedDate.ToString();
            DateTime startDateExpl = DateTime.Parse(startDateSelectedValue);
            DateTime startTimeExpl = new DateTime();
            if (updateApptStartAMPM.SelectedIndex == 1 && updateApptStartHour.SelectedIndex != 11)
            {
                startTimeExpl = DateTime.Today.AddHours((updateApptStartHour.SelectedIndex) + 13).AddMinutes(int.Parse(updateApptStartMin.Text));
            }
            if (updateApptStartAMPM.SelectedIndex == 1 && updateApptStartHour.SelectedIndex == 11)
            {
                startTimeExpl = DateTime.Today.AddHours(12).AddMinutes(int.Parse(updateApptStartMin.Text));
            }
            if (updateApptStartAMPM.SelectedIndex == 0 && updateApptStartHour.SelectedIndex == 11)
            {
                startTimeExpl = DateTime.Today.AddMinutes(int.Parse(updateApptStartMin.Text));
            }
            if (updateApptStartAMPM.SelectedIndex == 0 && updateApptStartHour.SelectedIndex != 11)
            {
                startTimeExpl = DateTime.Today.AddHours((updateApptStartHour.SelectedIndex) + 1).AddMinutes(int.Parse(updateApptStartMin.Text));
            }
            string startDate = startDateExpl.ToString("yyyy-MM-dd HH:mm:ss");
            string startTime = startTimeExpl.ToString("yyyy-MM-dd HH:mm:ss");
            string startCombo = startDate.Substring(0, 10) + " " + startTime.Substring(10);
            DateTime start = DateTime.Parse(startCombo).ToUniversalTime();

            string endDateSelectedValue = updateApptEndDateBox.SelectedDate.ToString();
            DateTime endDateExpl = DateTime.Parse(endDateSelectedValue);
            DateTime endTimeExpl = new DateTime();
            if (updateApptEndAMPM.SelectedIndex == 1 && updateApptEndHour.SelectedIndex != 11)
            {
                endTimeExpl = DateTime.Today.AddHours((updateApptEndHour.SelectedIndex) + 13).AddMinutes(int.Parse(updateApptEndMin.Text));
            }
            if (updateApptEndAMPM.SelectedIndex == 1 && updateApptEndHour.SelectedIndex == 11)
            {
                endTimeExpl = DateTime.Today.AddHours(12).AddMinutes(int.Parse(updateApptEndMin.Text));
            }
            if (updateApptEndAMPM.SelectedIndex == 0 && updateApptEndHour.SelectedIndex == 11)
            {
                endTimeExpl = DateTime.Today.AddMinutes(int.Parse(updateApptEndMin.Text));
            }
            if (updateApptEndAMPM.SelectedIndex == 0 && updateApptEndHour.SelectedIndex != 11)
            {
                endTimeExpl = DateTime.Today.AddHours(updateApptEndHour.SelectedIndex + 1).AddMinutes(int.Parse(updateApptEndMin.Text));
            }
            string endDate = endDateExpl.ToString("yyyy-MM-dd HH:mm:ss");
            string endTime = endTimeExpl.ToString("yyyy-MM-dd HH:mm:ss");
            string endCombo = endDate.Substring(0, 10) + " " + endTime.Substring(10);
            DateTime end = DateTime.Parse(endCombo).ToUniversalTime();

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
                        if (CheckApptOutsideBusHours(startTimeExpl, endTimeExpl))
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
                                        if (!mySQLDB.UpdateAppointment(apptID, custID, title, desc, loc, contact, type, url, start, end, now, username))
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

        private static bool CheckApptHasConflict(DateTime start, DateTime end)
        {
            mySQLDB mySQLDB = new mySQLDB();
            int countApptsAtTime = mySQLDB.CheckForApptsAtTime(start, end);
            List<Appointment> apptList = mySQLDB.SelectAllAppointments();
            if (countApptsAtTime > 0)
            {
                return true;
            }
            foreach (var appt in apptList)
            {
                if (start <= appt.startDateTime && end >= appt.endDateTime)
                {
                    return true;
                }
                if (start >= appt.startDateTime && start < appt.endDateTime)
                {
                    return true;
                }
                if (end > appt.startDateTime && end <= appt.startDateTime)
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
            DateTime apptStart = start;
            DateTime apptEnd = end;

            if (apptStart >= bizEndTime || apptStart < bizStartTime)
            {
                return true;
            }
            if (apptEnd > bizEndTime || apptEnd <= bizStartTime)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
