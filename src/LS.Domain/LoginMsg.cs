using System;
using System.Collections.Generic;
using LS.Core.Interfaces;
using LS.Domain.ExternalApiIntegration.Nlad;


namespace LS.Domain
{
    public class LoginMsg : IEntity<string>
    {
       

        public LoginMsg()
        {
            Id = Guid.NewGuid().ToString();
            
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public string Msg { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool Active { get; set; }
        public int MsgLevel { get; set; } 
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
