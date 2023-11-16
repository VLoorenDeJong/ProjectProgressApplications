using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProjectProgressLibrary.Models;
using static ProjectProgressLibrary.Enums;
using static ProjectProgressLibrary.Validation.DateTimeValidation;
using static ProjectProgressLibrary.Modifications.TekstModifications;
using ProjectProgressLibrary.Interfaces;

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
        private readonly IStartConfig _startConfig;
        private readonly ILogger _logger;
        private readonly IDataAccess _db;
        private readonly IConfiguration _config;
        private readonly string _mainGoal;
        private List<ProjectModel> AllProjects { get; set; } = new List<ProjectModel>();
        private ProjectModel MainProject { get; set; }

        public ProgressPageModel(ILogger<ProgressPageModel> logger, IConfiguration config, IDataAccess db, IStartConfig startConfig)
        {
            _startConfig = startConfig;
            _logger = logger;
            _config = config;
            (_db, _mainGoal) = _startConfig.GetDbConfig(config, db, "index");


            MainProject = _db.GetProjectByTitle(_mainGoal, AllProjects);

        }
        public async Task OnGet()
        {

            await LoadPageSettings();
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
            FinishedProjects = _db.FilterListsByDate(FinishedProjects, SearchDate, ProjectStatus.Done);
            StartedProjects = _db.FilterListsByDate(StartedProjects, SearchDate, ProjectStatus.Doing);
            AddedProjects = _db.FilterListsByDate(AddedProjects, SearchDate, ProjectStatus.ToDo);
        }

        public IActionResult OnPostSearchDate()
        {
            return RedirectToPage(new
            {
                SearchDate = SearchDate,
                SearchEnabled = true
            });
        }

        private async Task LoadPageSettings()
        {
            AllProjects = await _db.ReadAllProjectRecordsAsync(_mainGoal);
            LoadPageHours(MainProject);
            await LoadProjectLists();
        }

        private async Task LoadProjectLists()
        {

            FinishedProjects = await _db.FillProjectList(ProjectStatus.Done, _mainGoal);
            StartedProjects = await _db.FillProjectList(ProjectStatus.Doing, _mainGoal);
            AddedProjects = await _db.FillProjectList(ProjectStatus.ToDo, _mainGoal);
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
