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
using static PresentationWebFront.Logic;
using static ProjectProgressLibrary.Enums;

namespace PresentationWebFront.Pages
{
    public class ProgressPageModel : PageModel
    {
        // Front end
        [BindProperty(SupportsGet = true)]
        public bool SearchEnabled { get; set; } = false;
        [BindProperty(SupportsGet =true)]
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


        public ProgressPageModel(ILogger<IndexModel> logger, IConfiguration config, IDataAccess db)
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

            if (SearchEnabled == true)
            {
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
