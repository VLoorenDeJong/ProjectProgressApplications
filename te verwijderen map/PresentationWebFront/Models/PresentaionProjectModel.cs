using ProjectProgressLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ProjectProgressLibrary.Enums;

namespace PresentationWebFront.Models
{
    public class PresentaionProjectModel
    {
        public string ProjectTitle { get; set; }
        public string MainProjectTitle { get; set; }
        public string Outcome { get; set; }
        public string ShortDescription { get; set; }
        public int Priority { get; set; }
        public Guid MainProjectId { get; set; }
        public Guid ProjectId { get; set; }
        public ProjectStatus ProjectStatus { get; set; }
        public double ProgressPercentage { get; set; }
        public List<PresentationSubProjectModel> SubprojectPresentationModels { get; set; }
        public List<string> FutureAdditions { get; set; }
        public List<string> Challenges { get; set; }
        public bool ShowProgressBar { get; set; }
        public List<string> DeveloperNames { get; set; }

    }
}
