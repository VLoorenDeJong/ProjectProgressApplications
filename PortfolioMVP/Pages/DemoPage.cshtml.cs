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

        private readonly IStartConfig _startConfig;


        // Back end
        private readonly ILogger<DemoPageModel> _logger;
        private readonly IDataAccess _db;
        private readonly IConfiguration _config;
        private readonly string _mainGoal;
        private readonly string _rootFolderPath;

        public DemoPageModel(ILogger<DemoPageModel> logger, IConfiguration config, IDataAccess db, IStartConfig startConfig)
        {
            _startConfig = startConfig;
            _logger = logger;
            _config = config;
            
            (_db, _mainGoal) = _startConfig.GetDbConfig(config, db, "index");

            AllProjects = Task.Run(() => _db.ReadAllProjectRecordsAsync(_mainGoal)).Result;
            _rootFolderPath = _startConfig.GetProjectPhotosFolderPath(config);

        }

        private List<ProjectModel> AllProjects = new List<ProjectModel>();
        public void OnGet()
        {
            LoadPageDetails();
        }

        private void LoadPageDetails()
        {
            AllDemoModels = _db.CreateAllDemoModels(AllProjects);

            if (string.IsNullOrEmpty(PageTitle) == false)
            {
                DemoModelToShow = AllDemoModels.Where(x => x.Title == PageTitle).First();
                (ProjectPictureFilePath, ShowPicture) = _startConfig.SetUpPictureShowing(DemoModelToShow.Title, _db, _rootFolderPath);
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
            DemoModelToShow.ShortDescription = "In het zijmenu staan de links naar de pagina's van mijn demo's.";
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
