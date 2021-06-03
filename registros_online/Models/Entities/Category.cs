using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace registros_online.Models
{
    public class Category
    {
        public virtual int id { get; set; }
        public virtual int id_user { get; set; }
        public virtual int id_tipo { get; set; }
        public virtual string descripcion { get; set; }
    }
}