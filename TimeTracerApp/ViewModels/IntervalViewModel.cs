using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTracker.ViewModels
{
    public class IntervalViewModel
    {

        #region Constructor
        public IntervalViewModel()
        {
        }
        #endregion

        #region Properties
        public long Id { get; set; }
        public long? ElementId { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool IsOpen { get; set; }
        public long? TotalSecond { get; set; }
        public int Days { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }

        #endregion
    }
}

