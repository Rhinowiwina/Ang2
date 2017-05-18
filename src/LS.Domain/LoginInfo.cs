using System;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public class LoginInfo : IEntity<string>
    {
        public LoginInfo()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }

        public string UserId { get; set; }
        public bool UserIsLoggedIn { get; set; }
        public string SessionId { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}
