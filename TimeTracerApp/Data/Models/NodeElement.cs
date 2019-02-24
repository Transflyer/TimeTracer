﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeTracker.Data.Models
{
    public class NodeElement
    {
        #region Constructor

        public NodeElement()
        {
        }

        #endregion Constructor

        #region Properties

        [Key]
        [Required]
        public long Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }
        public string Text { get; set; }
        public string Notes { get; set; }

        [DefaultValue(0)]
        public int Type { get; set; }

        [DefaultValue(0)]
        public int Flags { get; set; }

        public string UserId { get; set; }
        public long? ParentId { get; set; }
        public bool? Deleted { get; set; }
        public string DeletedUserId { get; set; }
        public long? DeletedParentId { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public DateTime LastModifiedDate { get; set; }

        #endregion Properties

        #region Lazy-Load Properties

        /// <summary>
        /// The owner of this element
        /// </summary>
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        /// <summary>
        /// The owner of this element
        /// </summary>
        [ForeignKey("ParentId")]
        public virtual NodeElement ParentNode { get; set; }

        /// <summary>
        /// A list containing all the chield nodes.
        /// </summary>
        public virtual List<NodeElement> NodeElements { get; set; }

        /// <summary>
        /// A list containing all TimeSpents
        /// </summary>
        public virtual List<TimeSpent> TimeSpents { get; set; }

        #endregion Lazy-Load Properties
    }
}