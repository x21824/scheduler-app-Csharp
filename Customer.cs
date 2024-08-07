using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scheduler
{
    public class Customer : Base
    {
        public string customerName { get; set; }
        public int customerAddressID { get; set; }
        public byte customerActive { get; set; }
        public string address { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string postal { get; set; }
        public string country { get; set; }
        public string phone { get; set; }

        

    }
}
