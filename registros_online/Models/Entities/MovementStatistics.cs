using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace registros_online.Models
{
    public class MovementStatistics
    {
        public virtual int id { get; set; }
        public virtual string movimiento {  get; set; }
        public virtual string fechaFormateada { get; set; }
        public virtual float total { get; set; }
    }
}