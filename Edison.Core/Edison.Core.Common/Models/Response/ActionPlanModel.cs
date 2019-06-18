﻿using Edison.Core.Common.Interfaces;
using System;
using System.Collections.Generic;

namespace Edison.Core.Common.Models
{
    public class ActionPlanModel : IActionPlanModel
    {
        public Guid ActionPlanId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool IsActive { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
        public double PrimaryRadius { get; set; }
        public double SecondaryRadius { get; set; }
        public bool AcceptSafeStatus { get; set; }
        public virtual IEnumerable<ActionModel> OpenActions { get; set; }
        public virtual IEnumerable<ActionModel> CloseActions { get; set; }
    }
}
