using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectProgressLibrary.Models
{
    public class RelevantProjectModel
    {
        public string ProjectTitle { get; set; }
        public Guid ProjectId { get; set; }
        public bool ShowItem { get; set; }
    }
}
