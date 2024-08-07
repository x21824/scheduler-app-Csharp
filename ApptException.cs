using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace scheduler
{
    //Exceptions
    public class ApptException : ApplicationException
    {
        public void ApptOutOfBusHoursException()
        {
            Console.WriteLine("Exception has occurred. The appointment is outside of business hours, please schedule your appointment within standard business hours.");
            System.Windows.Forms.MessageBox.Show("Exception has occurred. The appointment is outside of business hours, please schedule your appointment within standard business hours.", "scheduler - Appointment Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public void ApptConflictException()
        {
            Console.WriteLine("Exception has occurred. The appointment is conflicting with an already scheduled appointment. Please schedule your appointment for a free time.");
            System.Windows.Forms.MessageBox.Show("Exception has occurred. The appointment is conflicting with an already scheduled appointment. Please schedule your appointment for a free time.", "scheduler - Appointment Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public void ApptGeneralException()
        {
            Console.WriteLine("Exception has occurred. Unable to add Appointment, a field has likely been left blank. All fields are required.");
            System.Windows.Forms.MessageBox.Show("Exception has occurred. Unable to add Appointment, a field has likely been left blank. All fields are required.", "scheduler - Appointment Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public void CustomerDataException()
        {
            Console.WriteLine("Exception has occurred. Unable to add customer, a field has likely been left blank or the data is invalid. All fields are required.");
            System.Windows.Forms.MessageBox.Show("Exception has occurred. Unable to add customer, a field has likely been left blank or the data is invalid. All fields are required.", "scheduler - Customer Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public void AddressDataException()
        {
            Console.WriteLine("Exception has occurred. Unable to add address, a field has likely been left blank or the data is invalid. All fields except Address 2 are required.");
            System.Windows.Forms.MessageBox.Show("Exception has occurred. Unable to add address, a field has likely been left blank or the data is invalid. All fields except Address 2 are required.", "scheduler - Address Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

    }
}
