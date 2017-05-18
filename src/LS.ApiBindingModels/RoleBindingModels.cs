using LS.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LS.ApiBindingModels
{
    public class RoleSimpleViewBindingModel {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Rank { get; set; }
    }
}
