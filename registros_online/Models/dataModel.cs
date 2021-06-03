using System.Collections.Generic;


namespace registros_online.Models
{
    public class dataModel
    {
        public dataModel()
        {
            registros = null;
            cantidad = 0;
        }

        public IList<Register> registros;
        public int cantidad;
    }
}