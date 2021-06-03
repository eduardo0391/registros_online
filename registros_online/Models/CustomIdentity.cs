using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace registros_online.Models
{
    class CustomIdentity : IIdentity
    {
        public CustomIdentity(string name, string id, bool isSuperUser)
        {
            IsAuthenticated = true;
            Name = name;
            Id = Int32.Parse(id);
            AuthenticationType = "Forms";
            IsSuperUser = isSuperUser;
        }

        public string AuthenticationType { get; private set; }
        public bool IsAuthenticated { get; private set; }
        public string Name { get; private set; }
        public int Id { get; private set; }
        public bool IsSuperUser { get; private set; }

    }
}