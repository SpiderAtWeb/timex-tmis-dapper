using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.SMIS
{
    public class IconData
    {
        public int CountNew { get; set; }
        public int CountIdle { get; set; }
        public int CountRequested { get; set; }
        public int CountInTransit { get; set; }
        public int CountRepair { get; set; }
        public int CountRunning { get; set; }
        public int CountDisposed { get; set; }
        public int CountTerminate { get; set; }

    }
}
