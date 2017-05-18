using System;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public class CompanyTranslations : IEntity<string>
    {
        public CompanyTranslations()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string CompanyID { get;set; }
     
        public string LSID { get; set; }
        public int TranslatedID { get; set; }
        public string Type { get; set; }
    
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}
