using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LS.Core.Interfaces;
namespace LS.Domain {
    public class UserOnBoardData : IEntity<string> {
        public UserOnBoardData() {
            Id = Guid.NewGuid().ToString();
            }
        public string Id { get; set; }
        public string CompanyID { get; set; }
       
        public string StreetAddress1 { get; set; }
        public string StreetAddress2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string PictureID { get; set; }
        public string Ssn { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool Exported { get; set; }
        public bool IsDeleted { get; set; }   
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        }
    }
