using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProjectProgressLibrary.DataAccess;
using ProjectProgressLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PresentationWebFront.Logic;

namespace PresentationWebFront.Pages
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



        // Back end
        private readonly ILogger<IndexModel> _logger;
        IDataAccess _db;
        private readonly string mainGoal;

        private List<ProjectModel> allProjects = new List<ProjectModel>();

        public IndexModel(ILogger<IndexModel> logger, IConfiguration config, IDataAccess db)
        {
            _logger = logger;
            (_db, mainGoal) = GetDbConfig(config, db, "index");

            allProjects = _db.ReadAllProjectRecords(mainGoal);

            MainProject = _db.GetProjectByTitle(mainGoal, allProjects);

            TotalHours = MainProject.TotalHours;
            GeneralHours = MainProject.GeneralHours;
            TheoreticalHours = MainProject.TheoreticalHours;
            PracticalHours = MainProject.PracticalHours;

        }

        public void OnGet()
        {

        }
    }
}
