using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace registros_online.Models
{
    public class UserPermission
    {
        public virtual int Id_user { get; set; }
        public virtual int Id_permission { get; set; }
        public virtual string Description { get; set; }
        public virtual bool IsRequiredPay { get; set; }
    }
}