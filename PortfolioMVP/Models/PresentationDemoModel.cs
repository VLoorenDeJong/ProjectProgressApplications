using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioMVP.Models
{
    public class PresentationDemoModel
    {
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string DesiredOutcome { get; set; }
        public string ProjectLink { get; set; }
        public string GitHubLink { get; set; }
        public double TotalHours { get; set; }
        public List<string> DeveloperNames { get; set; }
    }
}
