using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProjectProgressLibrary.DataAccess;
using ProjectProgressLibrary.Models;
using static PortfolioMVP.Logic;
using static ProjectProgressLibrary.Enums;
using static ProjectProgressLibrary.ApplicationLogic;

namespace PortfolioMVP.Pages
{
    public class ProgressPageModel : PageModel
    {
        // Front end
        [BindProperty]
        public string DayOfWeek { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool SearchEnabled { get; set; } = false;
        [BindProperty(SupportsGet = true)]
        public DateTime SearchDate { get; set; }
        [BindProperty]
        public double TotalHours { get; private set; }
        [BindProperty]
        public double TheoreticalHours { get; private set; }
        [BindProperty]
        public double PracticalHours { get; private set; }
        [BindProperty]
        public double GeneralHours { get; private set; }
        [BindProperty]
        public List<ProjectModel> FinishedProjects { get; set; }
        [BindProperty]
        public List<ProjectModel> StartedProjects { get; set; }
        [BindProperty]
        public List<ProjectModel> AddedProjects { get; set; }


        // Back end
        private readonly ILogger _logger;
        private readonly IDataAccess _db;
        private readonly IConfiguration _config;
        private readonly string _mainGoal;
        private List<ProjectModel> AllProjects { get; set; } = new List<ProjectModel>();
        private ProjectModel MainProject { get; set; }

        public ProgressPageModel(ILogger<ProgressPageModel> logger, IConfiguration config, IDataAccess db)
        {

            _logger = logger;
            _config = config;
            (_db, _mainGoal) = GetDbConfig(config, db, "index");

            AllProjects = _db.ReadAllProjectRecords(_mainGoal);

            MainProject = _db.GetProjectByTitle(_mainGoal, AllProjects);

        }
        public void OnGet()
        {

            LoadPageSettings();
            SearchDate = CreateDateFromString(SearchDate.ToString());
            if (SearchEnabled == false || SearchDate.ToString() == "01/01/0001 12:00:00 AM"
                                       || SearchDate.ToString() == "01/01/0001 00:00:00")
            {
                SearchDate = GetDefaultDate();
            }

            if (SearchEnabled == true)
            {
                DayOfWeek = DecideDayOfWeek(SearchDate);
                LoadSelectionLists();
            }
        }

        private void LoadSelectionLists()
        {
            FinishedProjects = FilterListsByDate(FinishedProjects, SearchDate, ProjectStatus.Done);
            StartedProjects = FilterListsByDate(StartedProjects, SearchDate, ProjectStatus.Doing);
            AddedProjects = FilterListsByDate(AddedProjects, SearchDate, ProjectStatus.ToDo);
        }

        public IActionResult OnPostSearchDate()
        {
            return RedirectToPage(new
            {
                SearchDate = SearchDate,
                SearchEnabled = true
            });
        }

        private void LoadPageSettings()
        {
            LoadPageHours(MainProject);
            LoadProjectLists();
        }

        private void LoadProjectLists()
        {
            FinishedProjects = FillProjectList(ProjectStatus.Done, AllProjects);
            StartedProjects = FillProjectList(ProjectStatus.Doing, AllProjects);
            AddedProjects = FillProjectList(ProjectStatus.ToDo, AllProjects);
        }
        private void LoadPageHours(ProjectModel project)
        {
            TotalHours = project.TotalHours;
            GeneralHours = project.GeneralHours;
            TheoreticalHours = project.TheoreticalHours;
            PracticalHours = project.PracticalHours;
        }
    }
}
