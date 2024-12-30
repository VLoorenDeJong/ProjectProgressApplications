using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProjectProgressLibrary.Interfaces;
using ProjectProgressLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioMVP.Pages
{
    public class IndexModel : PageModel
    {
        // Front end
        [BindProperty]
        public double TotalHours { get; private set; }
        [BindProperty]
        public double TheoreticalHours { get; private set; }
        [BindProperty]
        public double PracticalHours { get; private set; }
        [BindProperty]
        public double GeneralHours { get; private set; }
        [BindProperty]
        public ProjectModel MainProject { get; set; }
        [BindProperty(SupportsGet =true)]
        public bool FromDemoLink { get; set; }




        // Back end
        private readonly IStartConfig _startConfig;
        private readonly ILogger<IndexModel> _logger;
        private readonly IDataAccess _db;
        private readonly string mainGoal;

        private List<ProjectModel> AllProjects = new List<ProjectModel>();

        public IndexModel(ILogger<IndexModel> logger, IConfiguration config, IDataAccess db, IStartConfig startConfig)
        {
         
            _startConfig = startConfig;
            _logger = logger;
            (_db, mainGoal) = _startConfig.GetDbConfig(config, db, "index");
            

        }

        public async Task OnGet()
        {
            AllProjects = await _db.ReadAllProjectRecordsAsync(mainGoal);


            MainProject = _db.GetProjectByTitle(mainGoal, AllProjects);

            TotalHours = MainProject.TotalHours;
            GeneralHours = MainProject.GeneralHours;
            TheoreticalHours = MainProject.TheoreticalHours;
            PracticalHours = MainProject.PracticalHours;
        }
    }
}
