using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace registros_online.Models
{
    public class MovementStatisticsViewModel
    {
        public IList<MovementStatistics> top10 { get; set; }
        public IList<MovementStatistics> full { get; set; }
    }
}