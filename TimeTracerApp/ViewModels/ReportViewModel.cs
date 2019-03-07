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
        public List<ReportElement> Childs { get; set; }
        public long TotalSeconds { get; set; }
        public bool IsOpen { get; set; }
    }
}
