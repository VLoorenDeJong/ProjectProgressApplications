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
using static ProjectProgressLibrary.Enums;
using static ProjectProgressWebFront1.Logic;

namespace ProjectProgressWebFront1.Pages
{
    // ToDo When project is automaticly started no startdate
    public class ProjectManagementPageModel : PageModel
    {
        private readonly string _MainGoal;
        private readonly IDataAccess _db;
        //See if private is ok
        public List<ProjectModel> AllProjects = new List<ProjectModel>();

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
        public ProjectManagementPageModel(ILogger<IndexModel> logger, IDataAccess db, IConfiguration config)
        {

            (_db, _MainGoal) = GetDbConfig(config, db, "projectManagement");


            (AllProjects, AllTimeUnits) = _db.ReadAllRecords(_MainGoal);
            
            AllProjects = AllProjects.OrderBy(x => x.SubProjectIds.Count).ToList();
            MainGoal = _MainGoal;
        }
        public void OnGet()
        {
            if (SearchEnabled == true)
            {
                List<string> allProjectTitles = AllProjects.Select(x => x.Title).ToList();                
                allProjectTitles = ProjectTitle.SearchInCollection(allProjectTitles);

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
        public void OnPostStartProject(string title)
        {
            ProjectStatus status = ProjectStatus.Doing;
            _db.ChangeProjectStatus(title, status, AllProjects);
        }
        public void OnPostStopProject(string title)
        {
            ProjectStatus status = ProjectStatus.ToDo;
            _db.ChangeProjectStatus(title, status, AllProjects);
        }
        public void OnPostFinishProject(string title)
        {
            ProjectStatus status = ProjectStatus.Done;
            _db.ChangeProjectStatus(title, status, AllProjects);
        }
        public void OnPostRestartProject(string title)
        {
            ProjectStatus status = ProjectStatus.Doing;
            _db.ChangeProjectStatus(title, status, AllProjects);
        }
        public void OnPostDeleteProject(string title)
        {
            _db.DeleteProject(title, AllProjects, AllTimeUnits);
        }
        public IActionResult OnPostSearchProject()
        {
            bool hasTitleToSearch = ProjectTitle.ValidateStringHasContent();
            bool projectExists = ProjectTitle.ValidateIfSearchedProjectTitleExsists(AllProjects);
            if (hasTitleToSearch == true && projectExists == true)
            {
                return RedirectToPage(new { SearchEnabled = true,
                                            ShowAll = true,
                                            ProjectTitle = ProjectTitle});
            }
            if (hasTitleToSearch == false)
            {
                return RedirectToPage(new { NoTitleEntered = true });
            }
            if (projectExists == false)
            {
                return RedirectToPage(new { ProjectNotFound = true, 
                                            ProjectTitle = ProjectTitle});

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
            return RedirectToPage("DictionaryManagement", new { FutureFeaturesLoaded = true, ProjectTitle = title});
        }

    }
}
