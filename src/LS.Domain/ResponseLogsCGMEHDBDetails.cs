using System;
using System.Collections.Generic;
using LS.Core.Interfaces;
using LS.Domain.ExternalApiIntegration.Nlad;


namespace LS.Domain
{
    public class ResponseLogsCGMEHDBDetails : IEntity<string>
    {
       
     
        public ResponseLogsCGMEHDBDetails()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
       
        public string ResponseLogsCGMEHDBId { get; set; }
        public string Type { get; set; }
        public int PeriodDays{ get; set; }
        public int? Matches { get; set; } 
        public Boolean? IsMatched { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}
