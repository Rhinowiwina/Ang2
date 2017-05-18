using System;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public class BaseIncomeLevels : IEntity<string>
    {
        public BaseIncomeLevels()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string StateCode { get; set; }
        public int Base1Person { get; set; }
        public int Base2Person { get; set; }
        public int Base3Person { get; set; }
        public int Base4Person { get; set; }
        public int Base5Person { get; set; }
        public int Base6Person { get; set; }
        public int Base7Person { get; set; }
        public int Base8Person { get; set; }
        public int BaseAdditional { get; set; }
        public int IncomeLevel { get; set; }
        public DateTime DateActive { get; set; }
        
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}
