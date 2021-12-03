using ProjectProgressLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ProjectProgressLibrary.Enums;

namespace PresentationWebFront.Models
{
    public class PresentationSubProjectModel
    {
        public string Title { get; set; }
        public int Priority { get; set; }
        public ProjectStatus ProjectStatus { get; set; }
        public double ProgressPercentage { get; set; }
        public bool ShowProgressBar { get; set; }
    }
}
