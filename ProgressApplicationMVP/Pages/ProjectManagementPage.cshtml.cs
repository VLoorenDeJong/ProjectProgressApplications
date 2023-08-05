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
using static ProgressApplicationMVP.Logic;
using static ProjectProgressLibrary.Validation.DataValidation;
using ProjectProgressLibrary.Interfaces;

namespace ProgressApplicationMVP.Pages
{
    public class ProjectManagementPageModel : PageModel
    {
        private readonly string _MainGoal;
        private readonly IDataAccess _db;
        private readonly ILogger<ProjectManagementPageModel> _logger;
        //See if private is ok
        public List<ProjectModel> AllProjects = new List<ProjectModel>();
        private readonly IStartConfig _startConfig;

        [BindProperty(SupportsGet = true)]
        public string ProjectTitle { get; set; }
        [BindProperty]
        public string MainGoal { get; private set; }

        [BindProperty(SupportsGet = true)]
        public bool SearchEnabled { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool NoTitleEntered { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool ProjectNotFound { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool ShowAll { get; set; } = false;

        [BindProperty(SupportsGet = true)]
        public ProjectStatus ProjectStatus { get; set; }

        // Backend
        private List<TimeUnitModel> AllTimeUnits { get; set; }
        public ProjectManagementPageModel(ILogger<ProjectManagementPageModel> logger, IDataAccess db, IConfiguration config, IStartConfig startConfig)
        {
            _startConfig = startConfig;
            (_db, _MainGoal) = _startConfig.GetProgressDbConfig(config, db, "projectManagement");
            MainGoal = _MainGoal;
        }
        public async Task OnGet()
        {
            (AllProjects, AllTimeUnits) = await _db.ReadAllRecordsAsync(_MainGoal);
            AllProjects = AllProjects.OrderBy(x => x.SubProjectIds.Count).ToList();

            if (SearchEnabled == true)
            {
                List<string> allProjectTitles = AllProjects.Select(x => x.Title).ToList();
                allProjectTitles = _db.SearchInCollection(ProjectTitle, allProjectTitles);

                List<ProjectModel> foundProjects = new List<ProjectModel>();
                foreach (string projectTitle in allProjectTitles)
                {
                    ProjectModel projectToAdd = _db.GetProjectByTitle(projectTitle, AllProjects);
                    foundProjects.Add(projectToAdd);
                }

                AllProjects = foundProjects;
            }
        }

        public IActionResult OnPost(string title)
        {
            return RedirectToPage();
        }
        public IActionResult OnPostShowAll()
        {
            return RedirectToPage(new { ShowAll = true });
        }
        public async Task OnPostStartProject(string title)
        {
            (AllProjects, AllTimeUnits) = await _db.ReadAllRecordsAsync(_MainGoal);
            ProjectStatus status = ProjectStatus.Doing;
            _db.ChangeProjectStatus(title, status, AllProjects);
        }
        public async Task OnPostStopProject(string title)
        {
            (AllProjects, AllTimeUnits) = await _db.ReadAllRecordsAsync(_MainGoal);
            ProjectStatus status = ProjectStatus.ToDo;
            _db.ChangeProjectStatus(title, status, AllProjects);
        }
        public async Task OnPostFinishProject(string title)
        {
            (AllProjects, AllTimeUnits) = await _db.ReadAllRecordsAsync(_MainGoal);

            ProjectStatus status = ProjectStatus.Done;
            _db.ChangeProjectStatus(title, status, AllProjects);
        }
        public async Task OnPostRestartProject(string title)
        {
            (AllProjects, AllTimeUnits) = await _db.ReadAllRecordsAsync(_MainGoal);

            ProjectStatus status = ProjectStatus.Doing;
            _db.ChangeProjectStatus(title, status, AllProjects);
        }
        public async Task OnPostDeleteProject(string title)
        {
            (AllProjects, AllTimeUnits) = await _db.ReadAllRecordsAsync(_MainGoal);
            _db.DeleteProject(title, AllProjects, AllTimeUnits);
        }
        public async Task<IActionResult> OnPostSearchProject()
        {
            (AllProjects, AllTimeUnits) = await _db.ReadAllRecordsAsync(_MainGoal);

            bool hasTitleToSearch = ProjectTitle.ValidateStringHasContent();
            bool projectExists = ProjectTitle.ValidateIfSearchedProjectTitleExsists(AllProjects);
            if (hasTitleToSearch == true && projectExists == true)
            {
                return RedirectToPage(new
                {
                    SearchEnabled = true,
                    ShowAll = true,
                    ProjectTitle = ProjectTitle
                });
            }
            if (hasTitleToSearch == false)
            {
                return RedirectToPage(new { NoTitleEntered = true });
            }
            if (projectExists == false)
            {
                return RedirectToPage(new
                {
                    ProjectNotFound = true,
                    ProjectTitle = ProjectTitle
                });

            }
            return RedirectToPage();

        }
        public IActionResult OnPostEdit(string title)
        {
            return RedirectToPage("ProjectPage", new
            {
                ProjectTitle = title,
                EditingProject = true
            }
                                 );
        }
        public IActionResult OnPostAddProject(string title)
        {
            return RedirectToPage("ProjectPage", new
            {
                ProjectTitle = title,
                AddingProject = true
            }
                                 );
        }
        public IActionResult OnPostChallenge(string title)
        {
            return RedirectToPage("DictionaryManagement", new { ChallengesLoaded = true, ProjectTitle = title });
        }
        public IActionResult OnPostFuture(string title)
        {
            return RedirectToPage("DictionaryManagement", new { FutureFeaturesLoaded = true, ProjectTitle = title });
        }
    }
}
