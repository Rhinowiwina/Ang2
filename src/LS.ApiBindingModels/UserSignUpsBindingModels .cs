using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LS.Domain;
using LS.Core.Interfaces;
using System.ComponentModel.DataAnnotations;
//Relates to ApiCall only has nothing to do with LS Company
namespace LS.ApiBindingModels
{
    public class UserSignUpsView
    {
        [Required(ErrorMessage = "Subcontractor name is required.")]
        public string Subcontractor { get; set; }
        public string Gov64 { get; set; }
        public string Disclosure64 { get; set; }
        public string Authorization64 { get; set; }
        public string Training64 { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "DOB is required.")]
        public string DOB { get; set; }
        [Required(ErrorMessage = "Social Security is required.")]
        public string Ssn { get; set; }
        [Required(ErrorMessage = "Address is required.")]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; }
        [Required(ErrorMessage = "State is required.")]
        public string State { get; set; }
        [Required(ErrorMessage = "Zipcode is required.")]
        public string Zipcode { get; set; }
        [Required(ErrorMessage = "Phone number is required.")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }
       
        public string BackGroundCertificateID { get; set; }
        public string DrugScreenCertificateID { get; set; }
        public string GovernmentDocFilename { get; set; }
        public string DisclosureFilename { get; set; }
        public string AuthorizationFilename { get; set; }
        public string TrainingCertFilename { get; set; }
    }
}
