using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProjectProgressLibrary.Interfaces;
using ProjectProgressLibrary.Models;

namespace PortfolioMVP.Pages
{
    public class AboutMePageModel : PageModel
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



        // Back end
        private readonly ILogger _logger;
        private readonly IStartConfig _startConfig;
        private readonly IDataAccess _db;
        private readonly string mainGoal;

        private List<ProjectModel> AllProjects = new List<ProjectModel>();


        public AboutMePageModel(ILogger<IndexModel> logger, IConfiguration config, IDataAccess db, IStartConfig startConfig)
        {
            _logger = logger;
            _startConfig = startConfig;
            
            (_db, mainGoal) = _startConfig.GetDbConfig(config, db, "index");


            MainProject = _db.GetProjectByTitle(mainGoal, AllProjects);

            TotalHours = MainProject.TotalHours;
            GeneralHours = MainProject.GeneralHours;
            TheoreticalHours = MainProject.TheoreticalHours;
            PracticalHours = MainProject.PracticalHours;

        }
        public async Task OnGet()
        {
            AllProjects = await _db.ReadAllProjectRecordsAsync(mainGoal);
        }
    }
}
