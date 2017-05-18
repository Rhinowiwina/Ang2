using System;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public class BasePlan
    {
        public bool BasePlanDetailsAvailable { get; set; }
        public string PlanDescription { get; set; }
        public string Voice { get; set; }
        public string Text { get; set; }
        public string Data { get; set; }
    }
}
