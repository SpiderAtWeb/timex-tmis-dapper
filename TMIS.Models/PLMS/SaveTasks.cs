using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.PLMS
{
    public class SaveTasks
    {
        public List<CompletedTask> MainTasks { get; set; }
        public List<CompletedTask> SubTasks { get; set; }

        public SaveTasks()
        {
            MainTasks = [];
            SubTasks = [];
        }
    }

    public class CompletedTask
    {
        public int TaskId { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
