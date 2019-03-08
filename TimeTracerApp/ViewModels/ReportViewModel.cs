using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeTracker.Data.Models;

namespace TimeTracker.ViewModels
{
    public class ReportViewModel
    {
      public List<ReportElement> Report { get; set; }
    }

    public class ReportElement
    {
        public string NodeElementTitle { get; set; }
        public List<ReportElement> Children { get; set; }
        public long TotalSeconds { get; set; }
        public bool IsOpen { get; set; }
        public int Days { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }

    }
}
