using System;
using LS.Core.Interfaces;
using System.Runtime.Serialization;

namespace LS.Domain
{
    public class UserSignUps : IEntity<string>
    {
        public UserSignUps()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string Subcontractor { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DOB { get; set; }
        public string Ssn { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string BackGroundCertificateID { get; set; }
        public string DrugScreenCertificateID { get; set; }
        public string GovernmentDocFilename { get; set; }
        public string DisclosureFilename { get; set; }
        public string AuthorizationFileName { get; set; }
        public string TrainingCertFileName { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}
