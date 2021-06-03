using System;



namespace registros_online.Models
{
    public class User
    {
        public virtual string name { get; set; }

        public virtual int? id_user { get; set; }

        public virtual string user { get; set; }

        public virtual string email { get; set; }

        public virtual string password { get; set; }

        public virtual DateTime? creationDate { get; set; }

        public virtual bool remember { get; set; }

        public virtual string confirmPassword { get; set; }

        public virtual string codActivation { get; set; }

        public virtual DateTime? shipmentDate { get; set; }

        public virtual DateTime? ExpirationDate { get; set; }
        //0 no. 1 si
        public virtual bool IsConfirm { get; set; }

        public virtual bool IsSuperUser { get; set; }
    }
    }
