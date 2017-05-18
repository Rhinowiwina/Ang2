using System;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public class LifelineProgram : IEntity<string>
    {
        public LifelineProgram()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string ProgramName { get; set; }
        public string StateCode { get; set; }
        public bool RequiresAccountNumber { get; set; }
        public string RequiredStateProgramId { get; set; }
        public StateProgram RequiredStateProgram { get; set; }
        public string RequiredSecondaryStateProgramId { get; set; }
        public StateProgram RequiredSecondaryStateProgram { get; set; }
        public DateTime DateEnd { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }

        public string NladEligibilityCode { get; set; }
    }
}
