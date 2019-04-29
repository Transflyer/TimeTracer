using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeTracker.Data.Models
{
    public class Interval
    {
        #region Constructor
        public Interval()
        {
        }
        #endregion Constructor

        #region Properties

        [Key]
        [Required]
        public long Id { get; set; }

        [Required]
        public long ElementId { get; set; }

        [Required]
        public DateTime Start { get; set; }

        [Required]
        public DateTime End { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public DateTime LastModifiedDate { get; set; }

        [Required]
        public bool IsOpen { get; set; }

        public long? TotalSecond { get; set; }

        public string UserId { get; set; }
        
        #endregion

        #region Lazy-Load Properties
        /// <summary>
        /// The owner of this element
        /// </summary>
        [ForeignKey("ElementId")]
        public virtual NodeElement NodeElement { get; set; }
        #endregion
    }
}
