using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace scheduler
{
    /// <summary>
    /// Interaction logic for ReportsPage.xaml
    /// </summary>
    public partial class ReportsPage : Window
    {
        public ReportsPage()
        {
            InitializeComponent();
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
        private void SaveReportToCSV<T>(List<T> reportList, string reportName, string exportTime, string path)
        {
            string dashlinebreak = "----------------------------------------------------";
            var lines = new List<string>();
            IEnumerable<PropertyDescriptor> props = TypeDescriptor.GetProperties(typeof(T)).OfType<PropertyDescriptor>();
            lines.Add(dashlinebreak);
            string reportNameLine = "Report Name: " + reportName;
            lines.Add(reportNameLine);
            string exportTimeStamp = "Export Date/Time: " + exportTime;
            lines.Add(exportTimeStamp);
            lines.Add(dashlinebreak);
            var header = string.Join(",", props.ToList().Select(x => x.Name));
            lines.Add(header);
            var valueLines = reportList.Select(row => string.Join(",", header.Split(',').Select(a => row.GetType().GetProperty(a).GetValue(row, null))));
            lines.AddRange(valueLines);
            File.WriteAllLines(path, lines.ToArray());
        }
        private void exportReportButton_Click(object sender, RoutedEventArgs e)
        {
            string reportName;
            DateTime exportTime = DateTime.Now;
            string exportTimestamp = exportTime.ToString();
            mySQLDB mySQLDB = new mySQLDB();
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            save.DefaultExt = ".txt";
            save.RestoreDirectory = true;

            if ((string)reportDataGrid.Columns[0].Header == "Consultant")
            {
                reportName = "Sales Agent Schedules";
                save.FileName = reportName;
                List<ConsultantSchedule> reportList = mySQLDB.Report_ConsultantSchedules();
                foreach (var item in reportList)
                {
                    item.consultantName = mySQLDB.GetConsultantNameFromUserID(int.Parse(item.consultantName));
                    item.customerName = mySQLDB.GetCustomerNameFromCustomerID(int.Parse(item.customerName));
                    item.startTime.ToLocalTime();
                    item.endTime.ToLocalTime();
                }
                if (save.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SaveReportToCSV(reportList, reportName, exportTimestamp, save.FileName);
                }
            }
            if ((string)reportDataGrid.Columns[0].Header == "City Name")
            {
                reportName = "Number of Customers and Appointments by City";
                save.FileName = reportName;
                List<NumberCustomersAndAppointmentsByCity> reportList = mySQLDB.Report_NumberCustomersAndAppointmentsByCity();
                if (save.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SaveReportToCSV(reportList, reportName, exportTimestamp, save.FileName);
                }
            }
            if ((string)reportDataGrid.Columns[0].Header == "Year - Month")
            {
                reportName = "Number of Appointment Types by Month";
                save.FileName = reportName;
                List<NumberAppointmentTypesByMonth> reportList = mySQLDB.Report_NumAppointmentTypesByMonth();
                foreach (var item in reportList)
                {
                    int year = int.Parse(item.appointmentMonth.Substring(0, 4));
                    int convertMonth = int.Parse(item.appointmentMonth.Substring(4));
                    string longNameMonth = new DateTime(year, convertMonth, 1).ToString("MMMM");
                    string prettyYearMonth = year.ToString() + " - " + longNameMonth;
                    item.appointmentMonth = prettyYearMonth;
                }
                if (save.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SaveReportToCSV(reportList, reportName, exportTimestamp, save.FileName);
                }

            }
        }


        private void apptTypesByMonthButton_Click(object sender, RoutedEventArgs e)
        {
            noReportLabel.Visibility = Visibility.Hidden;
            noReportRectangle.Visibility = Visibility.Hidden;
            exportReportButton.Visibility = Visibility.Visible;
            mySQLDB mySQLDB = new mySQLDB();
            List<NumberAppointmentTypesByMonth> reportList = mySQLDB.Report_NumAppointmentTypesByMonth();

            foreach (var item in reportList)
            {
                int year = int.Parse(item.appointmentMonth.Substring(0, 4));
                int convertMonth = int.Parse(item.appointmentMonth.Substring(4));
                string longNameMonth = new DateTime(year, convertMonth, 1).ToString("MMMM");
                string prettyYearMonth = year.ToString() + " - " + longNameMonth;
                item.appointmentMonth = prettyYearMonth;
            }


            CollectionViewSource ReportsCollectionViewSource;
            ReportsCollectionViewSource = (CollectionViewSource)FindResource("ReportsCollectionViewSource");
            ReportsCollectionViewSource.Source = reportList;

            reportDataGrid.Columns[0].Header = "Year - Month";
            reportDataGrid.Columns[1].Header = "Appointment Type";
            reportDataGrid.Columns[2].Header = "Appointment Count";

        }

        private void customReportButton_Click(object sender, RoutedEventArgs e)
        {
            noReportLabel.Visibility = Visibility.Hidden;
            noReportRectangle.Visibility = Visibility.Hidden;
            exportReportButton.Visibility = Visibility.Visible;
            mySQLDB mySQLDB = new mySQLDB();
            List<NumberCustomersAndAppointmentsByCity> reportList = mySQLDB.Report_NumberCustomersAndAppointmentsByCity();

            CollectionViewSource ReportsCollectionViewSource;
            ReportsCollectionViewSource = (CollectionViewSource)FindResource("ReportsCollectionViewSource");
            ReportsCollectionViewSource.Source = reportList;

            reportDataGrid.Columns[0].Header = "City Name";
            reportDataGrid.Columns[1].Header = "Customer Count";
            reportDataGrid.Columns[2].Header = "Appointment Count";

        }

        private void consultantScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            noReportLabel.Visibility = Visibility.Hidden;
            noReportRectangle.Visibility = Visibility.Hidden;
            exportReportButton.Visibility = Visibility.Visible;
            mySQLDB mySQLDB = new mySQLDB();
            List<ConsultantSchedule> reportList = mySQLDB.Report_ConsultantSchedules();

            foreach (var item in reportList)
            {
                item.consultantName = mySQLDB.GetConsultantNameFromUserID(int.Parse(item.consultantName));
                item.customerName = mySQLDB.GetCustomerNameFromCustomerID(int.Parse(item.customerName));
                item.startTime.ToLocalTime();
                item.endTime.ToLocalTime();
            }


            CollectionViewSource ReportsCollectionViewSource;
            ReportsCollectionViewSource = (CollectionViewSource)FindResource("ReportsCollectionViewSource");
            ReportsCollectionViewSource.Source = reportList;

            reportDataGrid.Columns[0].Header = "Consultant";
            reportDataGrid.Columns[1].Header = "Customer Name";
            reportDataGrid.Columns[2].Header = "Appointment Title";
            reportDataGrid.Columns[3].Header = "Appointment Type";
            reportDataGrid.Columns[4].Header = "Start Date & Time";
            reportDataGrid.Columns[5].Header = "End Date & Time";
        }
    }
}
