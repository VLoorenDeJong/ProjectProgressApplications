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
using ProjectProgressLibrary.StartConfig;

namespace PortfolioMVP.Pages
{
    public class ProjectsModel : PageModel
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
        public ProjectModel MostRecentProject { get; set; }
        [BindProperty]
        public string ProjectPictureFilePath { get; set; }
        [BindProperty]
        public string _PageTitle { get; set; }
        [BindProperty(SupportsGet = true)]
        public string BasePage { get; set; }
        [BindProperty(SupportsGet = true)]
        public string BasePageTitle { get; set; }
        [BindProperty]
        public bool IsDeperPage { get; set; } = true;
        public ProjectModel PageProject { get; set; }
        [BindProperty]
        public bool ShowPicture { get; set; } = false;
        [BindProperty]
        public bool WorkedOn { get; set; } = false;
        [BindProperty(SupportsGet = true)]
        public bool SubProjectClicked { get; set; } = true;
        [BindProperty(SupportsGet = true)]
        public bool MainProjectClicked { get; set; } = false;
        [BindProperty]
        public List<string> SubProjectTitles { get; set; } = new List<string>();
        public PresentaionProjectModel PagePresentationProject { get; set; }
        public List<PresentaionProjectModel> AllPresentationModels { get; set; } = new List<PresentaionProjectModel>();




        // Back end
        private readonly IStartConfig _startConfig;
        private readonly ILogger<ProjectsModel> _logger;
        private readonly IDataAccess _db;
        private readonly IConfiguration _config;
        private readonly string _MainGoal;
        private readonly string _projectPhotosFolderPath;

        private List<ProjectModel> AllProjects = new List<ProjectModel>();
        private List<TimeUnitModel> AllTimeUnits = new List<TimeUnitModel>();
        private List<TimeUnitModel> AllProjectTimeUnits = new List<TimeUnitModel>();


        private List<Guid> SubProjectIds = new List<Guid>();

        public ProjectsModel(ILogger<ProjectsModel> logger, IConfiguration config, IDataAccess db, IStartConfig startConfig)
        {
            _startConfig = startConfig;
            _logger = logger;
            _config = config;
            (_db, _MainGoal) = _startConfig.GetDbConfig(config, db, "index");

            (AllProjects, AllTimeUnits) = _db.ReadAllRecords(_MainGoal);
            _projectPhotosFolderPath = _startConfig.GetProjectPhotosFolderPath(config);

        }


        public void OnGet(string pageTitle)
        {
            LoadPageDetails(pageTitle);

            bool isFirstProjectLayer = checkForFirstLayerProject(pageTitle);

            if (isFirstProjectLayer == true)
            {
                LoadPageSettings(pageTitle);
                PagePresentationProject.MainProjectTitle = null;
                SelectMostRecentWorkedOnProject();
                IsDeperPage = false;
            }


        }

        private bool checkForFirstLayerProject(string pageTitle)
        {
            bool isFirstLayer = false;

            if (pageTitle == "Leerpad" ||
                pageTitle == "Projecten" ||
                pageTitle == "Contributies")
            {
                isFirstLayer = true;
            }

            return isFirstLayer;
        }

        private void LoadPageDetails(string pageTitle)
        {

            LoadPageSettings(pageTitle);

            PageProject = _db.GetProjectByTitle(pageTitle, AllProjects);
            // ToDo test if this is needed?
            SubProjectIds = _db.GetSubprojectIds(PageProject);
            PagePresentationProject = _db.CreatePresentationProject(PageProject, AllProjects, _db);

            LoadSubProjectTitles();
            LoadAllNeededPresentationModels();

            LoadPageHours(PageProject);


            (ProjectPictureFilePath, ShowPicture) = _startConfig.SetUpPictureShowing(pageTitle, _db, _projectPhotosFolderPath);

        }
        private void LoadPageSettings(string pageTitle)
        {

            switch (pageTitle)
            {
                case "Leerpad":
                    _PageTitle = pageTitle;
                    BasePageTitle = _PageTitle;
                    BasePage = pageTitle;
                    break;
                case "Projecten":
                    _PageTitle = pageTitle;
                    BasePageTitle = _PageTitle;
                    BasePage = pageTitle;
                    break;
                case "Contibuties":
                    _PageTitle = pageTitle;
                    BasePageTitle = _PageTitle;
                    BasePage = pageTitle;
                    break;
                default:
                    _PageTitle = pageTitle;
                    break;
            }

        }
        private void LoadAllNeededPresentationModels()
        {
            foreach (PresentationSubProjectModel subProject in PagePresentationProject.SubprojectPresentationModels)
            {
                // Get the project
                ProjectModel subProjectToAdd = _db.GetProjectByTitle(subProject.Title, AllProjects);

                // Create a presentation model from project
                PresentaionProjectModel subPresentationProjectToAdd = _db.CreatePresentationProject(subProjectToAdd, AllProjects, _db);

                // Order the sub project list by priority
                subPresentationProjectToAdd.SubprojectPresentationModels = subPresentationProjectToAdd.SubprojectPresentationModels.OrderBy(x => x.Priority).ToList();

                // Add project presentation model to te page list
                AllPresentationModels.Add(subPresentationProjectToAdd);
            }

            // Order the page project list by priority
            AllPresentationModels = AllPresentationModels.OrderBy(x => x.Priority).ToList();
        }
        private void LoadSubProjectTitles()
        {
            if (PagePresentationProject.SubprojectPresentationModels.Count > 0)
            {
                PagePresentationProject.SubprojectPresentationModels = PagePresentationProject.SubprojectPresentationModels.OrderBy(x => x.Priority).ToList(); foreach (PresentationSubProjectModel subProjectModel in PagePresentationProject.SubprojectPresentationModels)
                {
                    SubProjectTitles.Add(subProjectModel.Title);
                }
            }
        }
        private void SelectMostRecentWorkedOnProject()
        {
            AllProjectTimeUnits = _db.GetAllRelevantTimeUnits(PageProject, AllTimeUnits, AllProjects, _db);

            if (AllProjectTimeUnits.Count > 0)
            {
                WorkedOn = true;

                AllProjectTimeUnits = AllProjectTimeUnits.OrderBy(x => x.TimeStamp).ToList();

                TimeUnitModel mostRecentTimeUnit = AllProjectTimeUnits.Last();

                MostRecentProject = AllProjects.Where(x => x.ProjectId == mostRecentTimeUnit.ProjectId).First();

                (ProjectPictureFilePath, ShowPicture) = _startConfig.SetUpPictureShowing(MostRecentProject.Title, _db, _projectPhotosFolderPath);
            }
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
