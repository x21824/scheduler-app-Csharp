using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scheduler
{
    public class ConsultantSchedule
    {
        public string consultantName { get; set; }
        public string customerName { get; set; }
        public string apptTitle { get; set; }
        public string apptType { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
    }
}
