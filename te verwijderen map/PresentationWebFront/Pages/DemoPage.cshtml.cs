using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PresentationWebFront.Models;
using ProjectProgressLibrary.DataAccess;
using ProjectProgressLibrary.Models;
using static PresentationWebFront.Logic;

namespace PresentationWebFront.Pages
{
    public class DemoPageModel : PageModel
    {
        // Front end
        [BindProperty(SupportsGet = true)]
        public string PageTitle { get; set; }
        [BindProperty]
        public string ProjectPictureFilePath { get; set; }
        [BindProperty]
        public double TotalHours { get; private set; }
        [BindProperty]
        public double TheoreticalHours { get; private set; }
        [BindProperty]
        public double PracticalHours { get; private set; }
        [BindProperty]
        public double GeneralHours { get; private set; }
        [BindProperty]
        public List<PresentationDemoModel> AllDemoModels { get; set; }
        [BindProperty]
        public PresentationDemoModel DemoModelToShow { get; set; } = new PresentationDemoModel();
        [BindProperty]
        public bool ShowPicture { get; set; } = false;


        // Back end
        private readonly ILogger<IndexModel> _logger;
        private readonly IDataAccess _db;
        private readonly IConfiguration _config;
        private readonly string _mainGoal;


        private List<ProjectModel> AllProjects = new List<ProjectModel>();

        public DemoPageModel(ILogger<IndexModel> logger, IConfiguration config, IDataAccess db)
        {
            _logger = logger;
            _config = config;
            (_db, _mainGoal) = GetDbConfig(config, db, "index");

            AllProjects  = _db.ReadAllProjectRecords(_mainGoal);

        }
        public void OnGet()
        {
            LoadPageDetails();            
        }

        private void LoadPageDetails()
        {
            AllDemoModels = CreateAllDemoModels(AllProjects);

            if (string.IsNullOrEmpty(PageTitle) == false)
            {
                DemoModelToShow = AllDemoModels.Where(x => x.Title == PageTitle).First();
                (ProjectPictureFilePath, ShowPicture) = SetUpPictureShowing(DemoModelToShow.Title);
            }

            if (string.IsNullOrEmpty(PageTitle) == true)
            {
                PageTitle = _mainGoal;
                LoadPageModel();
            }

            ProjectModel PageProjectModel = _db.GetProjectByTitle(PageTitle, AllProjects);
            LoadPageHours(PageProjectModel);
        }

        private void LoadPageModel()
        {
  
            
            DemoModelToShow.Title = "Demo pagina";
            DemoModelToShow.ShortDescription = "Op deze pagina kunnen mijn demo applicaties gevonden worden";
            DemoModelToShow.DesiredOutcome = "Ga ze vooral proberen";
            DemoModelToShow.ProjectLink = "Hier is een link";
            DemoModelToShow.TotalHours = 10;
            DemoModelToShow.DeveloperNames = new List<string> { "Victor" };
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
