using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProjectProgressLibrary.DataAccess;
using ProjectProgressLibrary.Models;
using static PresentationWebFront.Logic;

namespace PresentationWebFront.Pages
{
    public class TestPageModel : PageModel
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

        // Backend
        public readonly string _mainGoal;
        private readonly IDataAccess _db;
        private readonly ILogger<IndexModel> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public List<ProjectModel> AllProjects = new List<ProjectModel>();
        private List<TimeUnitModel> AllTimeUnits = new List<TimeUnitModel>();
        public TestPageModel(ILogger<IndexModel> logger, IDataAccess db, IConfiguration config, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;

            (_db, _mainGoal) = GetDbConfig(config, db, "projectPage");

            (AllProjects, AllTimeUnits) = _db.ReadAllRecords(_mainGoal);

        }
        public void OnGet()
        {
            LoadPageHours();
        }

        private void LoadPageHours()
        {
            TotalHours = 1.5;
            TheoreticalHours = 1.05;
            PracticalHours = 1.57;
            GeneralHours = 58.68;
        }
    }
}
