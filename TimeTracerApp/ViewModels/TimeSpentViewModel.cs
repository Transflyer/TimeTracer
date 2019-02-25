using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTracker.ViewModels
{
    public class TimeSpentViewModel
    {

        #region Constructor
        public TimeSpentViewModel()
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
        #endregion
    }
}
