using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTracker.ViewModels
{
    public class ElementSpanViewModel
    {
        public long NodeElementId { get; set; }
        public int Days { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
        public long IsOpenIntervalId { get; set; }
    }
}

