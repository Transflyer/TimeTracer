using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTracker.ViewModels
{
    public class ElementViewModel
    {
        #region Constructor
        public ElementViewModel()
        {
        }
        #endregion

        #region Properties
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Text { get; set; }
        public string Notes { get; set; }
        public int Type { get; set; }
        public int Flags { get; set; }
        public string UserId { get; set; }
        public long ParentId { get; set; }
        public bool IsStarted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        #endregion
    }
}
