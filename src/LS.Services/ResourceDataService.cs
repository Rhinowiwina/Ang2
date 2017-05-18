using System;
using System.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LinqKit;
using LS.Core;
using LS.Domain;
using LS.Repositories;
using LS.Services;
using Microsoft.AspNet.Identity;

namespace LS.Services
{
   public class ResourceDataService : BaseDataService<Resources,string>
    {
        public override BaseRepository<Resources, string> GetDefaultRepository()
        {
            return new ResourcesRepository();
        }
      

    }
}
