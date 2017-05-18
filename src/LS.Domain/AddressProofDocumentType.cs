using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LS.Core.Interfaces;

namespace LS.Domain {
    public class AddressProofDocumentType: IEntity<string> {
        public AddressProofDocumentType() {
            Id = Guid.NewGuid().ToString();
            }

        public string Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string LifelineSystem { get; set; }
        public string LifelineSystemId { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }

        }
    }
