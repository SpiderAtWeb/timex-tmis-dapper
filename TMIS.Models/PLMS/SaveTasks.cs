using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMIS.Models.PLMS
{
    public class SaveTasks
    {
        [FromForm]
        public List<CompletedTask> MainTasks { get; set; }

        [FromForm]
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

        [FromForm]
        public IFormFile? ZipFile { get; set; }

    }
}
