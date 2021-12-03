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
    public class testPageModel : PageModel
    {
        private readonly string _MainGoal;
        private readonly IDataAccess _db;
        public List<ProjectModel> AllProjects = new List<ProjectModel>();
        [BindProperty]
        public string MainGoal { get; private set; }

        [BindProperty(SupportsGet = true)]
        public string ProjectTitle { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool SearchEnabled { get; set; }

        [BindProperty(SupportsGet = true)]
        public ProjectStatus ProjectStatus { get; set; }

        public testPageModel(ILogger<IndexModel> logger, IDataAccess db, IConfiguration config)
        {

            (_db, _MainGoal) = GetDbConfig(config, db, "testPage");


            AllProjects = _db.ReadAllProjectRecords(_MainGoal);
            MainGoal = _MainGoal;
        }
        public void OnGet()
        {
            if (SearchEnabled == true)
            {
                Guid projectToChangeId = GetProjectIdByTitle(ProjectTitle, AllProjects);
                ProjectModel projectToChange = _db.GetProjectById(projectToChangeId, AllProjects);
                AllProjects = new List<ProjectModel> { projectToChange };
            }
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(new
            {
                SearchEnabled = true,
                ProjectTitile = ProjectTitle,
                ProjectStatus = ProjectStatus

            });
        }

        public void OnPostStartProject(string title)
        {
        }
    }
}
