using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectProgressLibrary;
using ProjectProgressLibrary.Interfaces;
using ProjectProgressLibrary.Models;
using ProjectProgressLibrary.Models.Options;
using static ProgressApplicationMVP.Logic;
using static ProjectProgressLibrary.Enums;
using static ProjectProgressLibrary.Validation.DataValidation;


namespace ProgressApplicationMVP.Pages
{
    public class ProjectPageModel : PageModel
    {
        public readonly string _mainGoal;
        private readonly IDataAccess _db;
        private readonly ILogger<ProjectPageModel> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IStartConfig _startConfig;

        [BindProperty]
        public string BackendPhotoFilePath { get; set; } = null;
        public string FrontendPhotoFilePath { get; set; } = null;

        public List<ProjectModel> AllProjects = new List<ProjectModel>();
        private List<TimeUnitModel> AllTimeUnits = new List<TimeUnitModel>();
        [BindProperty]
        public bool IsDemo { get; set; } = false;
        [BindProperty(SupportsGet = true)]
        public bool IsCircularReference { get; set; } = false;
        [BindProperty(SupportsGet = true)]
        public string ProblemProjectTitle { get; set; }
        [BindProperty]
        public string mainGoal { get; private set; }
        [BindProperty(SupportsGet = true)]
        public string ProjectTitle { get; set; }
        [BindProperty(SupportsGet = true)]
        public string OldProjectTitle { get; set; }
        [BindProperty(SupportsGet = true)]
        public string MainProjectTitle { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Outcome { get; set; }
        [BindProperty(SupportsGet = true)]
        public Nullable<Guid> MainProjectId { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool NoProjectTitle { get; set; } = false;
        [BindProperty(SupportsGet = true)]
        public bool EditingProject { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool AddingProject { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool NewProjectWithSameTitle { get; set; } = false;
        [BindProperty(SupportsGet = true)]
        public bool CannotAddToExistsingProject { get; set; }
        [BindProperty(SupportsGet = true)]
        public ProjectModel Project { get; set; } = new ProjectModel();
        [BindProperty]
        public string ProjectPictureFilePath { get; set; }    
        
        [BindProperty(SupportsGet =true)]
        public DateTime DateCreate { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime DateDoing { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime DateDone { get; set; }
        [BindProperty(SupportsGet =true)]
        public ProjectStatus NewStatus { get; set; }
        [BindProperty]
        public bool? HasPortfolio { get; set; }

        [BindProperty]
        public IFormFile Photo { get; set; }
        public ProjectPageModel(ILogger<ProjectPageModel> logger, IDataAccess db, IConfiguration config, IWebHostEnvironment webHostEnvironment, IStartConfig startConfig, IOptions<ApplicationOptions> applicationOptions)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _startConfig = startConfig;
            (_db, _mainGoal) = _startConfig.GetProgressDbConfig(config, db, "projectPage");
            mainGoal = _mainGoal;
            HasPortfolio = applicationOptions?.Value?.HasPortfolio;
        }
        public async Task OnGet()
        {
            (AllProjects, AllTimeUnits) = await _db.ReadAllRecordsAsync(_mainGoal);

            if (mainGoal == "Demoing the app")
            {
                IsDemo = true;
            }


            if (EditingProject == true)
            {
                // Get the project to edit
                Project = _db.GetProjectByTitle(ProjectTitle, AllProjects);

                // Get the mainproject title for radio pre selection
                string mainProjectIdStringGuid = Project.MainProjectId.ToString();
                Guid mainProjectId = new Guid(mainProjectIdStringGuid);

                MainProjectTitle = _db.GetProjectById(mainProjectId, AllProjects).Title;
                OldProjectTitle = ProjectTitle;
            }


            if (AddingProject == true)
            {
                MainProjectTitle = ProjectTitle;
                Project.Title = "";
            }
            if (NewProjectWithSameTitle == true)
            {
                Project.Title = ProjectTitle;
            }

            LoadPhotoPath(Project.Title);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostSave()
        {
            (AllProjects, AllTimeUnits) = await _db.ReadAllRecordsAsync(_mainGoal);

            if (string.IsNullOrEmpty(Project.Title) == true)
            {

                return RedirectToPage(new
                {
                    NoProjectTitle = true,
                    MainProjectTitle = MainProjectTitle,
                    ProjectTitle = Project.Title,
                    ShortDescription = Project.ShortDescription,
                    Outcome = Project.Outcome,
                    Impact = Project.Impact,
                    Urgency = Project.Urgency,
                    EaseOffSuccess = Project.EaseOffSuccess,
                    PersonalPreference = Project.PersonalPreference,
                    ShowItem = Project.ShowItem,
                    ShowProgressBar = Project.ShowProgressBar,
                    PriorityCalculation = Project.PriorityCalculation,
                    MainProjectId = MainProjectId,
                    Photo = Photo

                });
            }
            bool doesExsist = Project.Title.ValidateIfProjectTitleExsists(AllProjects);
            if (doesExsist == false)
            {

                if (_mainGoal == "Demoing the app")
                {
                    Project.DeveloperName = "Demo developer";
                }
                // ToDo test case
                //AllProjects priorities on main project on 10
                //add sub project mainproject priority = -999

                //This is for using it after if statement
                Guid mainProjectId = new Guid();

                // If mainproject exsists get the id else return to page with CannotAddToExistsingProject = true 
                if (string.IsNullOrEmpty(MainProjectTitle) == false)
                {
                    // Checks for same name in list main 
                    bool projectExists = MainProjectTitle.ValidateIfProjectTitleExsists(AllProjects);

                    if (projectExists == true)
                    {
                        mainProjectId = _db.GetProjectIdByTitle(MainProjectTitle, AllProjects);
                    }
                    else
                    {
                        CannotAddToExistsingProject = true;
                    }
                }

                // Failsafe if empty get main project id
                if (string.IsNullOrEmpty(MainProjectTitle) == true)
                {
                    mainProjectId = _db.GetProjectIdByTitle(_mainGoal, AllProjects);
                }

                Project.SetMainProjectId(mainProjectId);

                if (CannotAddToExistsingProject == false)
                {
                    ProjectModel mainProject = _db.GetProjectById(mainProjectId, AllProjects);

                    mainProject.AddSubProject(Project.ProjectId.ToString());

                    _db.CalculateProjectProgress(Project, AllProjects);

                    AllProjects.Add(Project);

                    _db.SaveAllProjects(AllProjects);

                    AllProjects = await _db.ReadAllProjectRecordsAsync(_mainGoal);

                    // Photo saving
                    _db.ProcessPicture(Photo, Project.Title);
                    //ProcessPicture();


                    return RedirectToPage("ProjectManagementPage");

                }
            }
            if (doesExsist == true && string.IsNullOrEmpty(Project.Title) == false)
            {
                return RedirectToPage(new
                {
                    NewProjectWithSameTitle = true,
                    MainProjectTitle = MainProjectTitle,
                    ProjectTitle = Project.Title,
                    ShortDescription = Project.ShortDescription,
                    Outcome = Project.Outcome,
                    Impact = Project.Impact,
                    Urgency = Project.Urgency,
                    EaseOffSuccess = Project.EaseOffSuccess,
                    PersonalPreference = Project.PersonalPreference,
                    ShowItem = Project.ShowItem,
                    ShowProgressBar = Project.ShowProgressBar,
                    PriorityCalculation = Project.PriorityCalculation,
                    MainProjectId = MainProjectId
                });
            }
            if (CannotAddToExistsingProject == true)
            {
                return RedirectToPage(new
                {
                    CannotAddToExistsingProject = true,
                    MainProjectTitle = MainProjectTitle,
                    ProjectTitle = Project.Title,
                    ShortDescription = Project.ShortDescription,
                    Outcome = Project.Outcome,
                    Impact = Project.Impact,
                    Urgency = Project.Urgency,
                    EaseOffSuccess = Project.EaseOffSuccess,
                    PersonalPreference = Project.PersonalPreference,
                    ShowItem = Project.ShowItem,
                    ShowProgressBar = Project.ShowProgressBar,
                    PriorityCalculation = Project.PriorityCalculation,
                    MainProjectId = MainProjectId
                });

            }

            return RedirectToPage(new
            {
                AllProjects,
                NoProjectTitle = true
            });
        }
        public async Task<IActionResult> OnPostSaveEditedProject()
        {
            (AllProjects, AllTimeUnits) = await _db.ReadAllRecordsAsync(_mainGoal);

            if (string.IsNullOrEmpty(Project.Title) == false)
            {
                string title = "";
                ProjectModel oldProject = _db.GetProjectByTitle(OldProjectTitle, AllProjects);
                Nullable<Guid> guidToCHeck = _db.FindProblemId(MainProjectTitle, oldProject.SubProjectIds, AllProjects);
                if (guidToCHeck == null)
                {
                    AllProjects = Task.Run(() => _db.ReadAllProjectRecordsAsync(_mainGoal)).Result;

                    title = OldProjectTitle;

                    Project.SetProjectId(oldProject.ProjectId.ToString());
                    Project.SetMainProjectId(_db.GetProjectByTitle(MainProjectTitle, AllProjects).ProjectId);
                    Project.SetProgress(oldProject.Progress.ToString());

                    Project.SetTotalHours(oldProject.TotalHours.ToString());
                    Project.SetTheoreticalHours(oldProject.TheoreticalHours.ToString());
                    Project.SetPracticalHours(oldProject.PracticalHours.ToString());
                    Project.SetGeneralHours(oldProject.GeneralHours.ToString());
                    Project.SetProjectStatus(oldProject.ProjectStatus.GetHashCode().ToString());

                    if (Project.ProjectStatus != NewStatus && IsDemo == false)
                    {
                        int projectStatus = GetProjectStatus(NewStatus);
                        Project.SetProjectStatus(projectStatus.ToString());
                    }

                    if (IsDemo == false)
                    {
                        Project.SetDateCreated(DateCreate.ToString());
                        Project.SetDateDoing(DateDoing.ToString());
                        Project.SetDateDone(DateDone.ToString());
                    }

                    if (IsDemo == true)
                    {
                        Project.SetDateCreated(oldProject.DateCreated.ToString());
                        Project.SetDateDoing(oldProject.DateDoing.ToString());
                        Project.SetDateDone(oldProject.DateDone.ToString());
                    }


                    foreach (var addition in oldProject.FutureAdditions)
                    {
                        Project.AddFutureAddition(addition.Key, addition.Value);
                    }
                    foreach (var challenge in oldProject.Challenges)
                    {
                        Project.AddChallenge(challenge.Key, challenge.Value);
                    }

                    foreach (Nullable<Guid> timeUnitId in oldProject.TimeUnitsPutIn)
                    {
                        Project.AddTimeUnitIdToProject(timeUnitId.ToString());
                    }

                    foreach (Nullable<Guid> subProjectId in oldProject.SubProjectIds)
                    {
                        Project.AddSubProject(subProjectId.ToString());
                    }

                    // Change all timeunit project titles for project if title changed

                    if (OldProjectTitle != Project.Title)
                    {
                        // Get all time units
                        List<TimeUnitModel> allTimeUnits = Task.Run(() => _db.ReadAllTimeUnits(_mainGoal)).Result;

                        // Select units for this project
                        List<TimeUnitModel> timeunitsForProject = allTimeUnits.Where(x => x.ProjectId == Project.ProjectId).ToList();

                        // Maak wijzigigen
                        foreach (TimeUnitModel timeUnit in timeunitsForProject)
                        {
                            // Change title
                            timeUnit.SetProjectTitle(Project.Title);
                            // Remove time unit from list
                            allTimeUnits.Remove(timeUnit);
                            // Add timeunit to list
                            allTimeUnits.Add(timeUnit);
                        }

                        _db.SaveAllTimeUnits(allTimeUnits);
                    }


                    _db.SaveProject(Project, AllProjects);

                    _db.ProcessPicture(Photo, Project.Title);

                    return RedirectToPage("ProjectManagementPage");
                }
                if (guidToCHeck != null)
                {
                    Guid problemProjectGuid = new Guid(guidToCHeck.ToString());
                    ProjectModel problemProject = _db.GetProjectById(problemProjectGuid, AllProjects);
                    return RedirectToPage(new
                    {
                        ProjectTitle = OldProjectTitle,
                        EditingProject = true,
                        IsCircularReference = true,
                        ProblemProjectTitle = problemProject.Title,


                    });

                }

            }

            return RedirectToPage("ProjectManagementPage");

        }

        private int GetProjectStatus(Enums.ProjectStatus projectStatus)
        {
            int outputStatus = 0;

            switch (projectStatus)
            {
                case Enums.ProjectStatus.ToDo:
                    outputStatus = 0;
                    break;
                case Enums.ProjectStatus.Doing:
                    outputStatus = 1;
                    break;
                case Enums.ProjectStatus.Done:
                    outputStatus = 2;
                    break;
                default:
                    break;
            }

            return outputStatus;
        }

        public IActionResult OnPostAddChallenge(string title)
        {

            return RedirectToPage();
        }
        public void LoadPhotoPath(string projectTitle)
        {
            if (string.IsNullOrEmpty(projectTitle) == false)
            {
                string fileName = _db.ExchangePunctuations(projectTitle);
                string extrafolder = "";
                string filePath = $"{ fileName }.jpg";

                //filePath = _db.ExchangeCharacters(filePath);

                if (mainGoal == "!!!! DEVELOPMENT ENVIRONMENT !!!!")
                {
                    extrafolder = @"DevelopmentFolder\";
                }
                ProjectPictureFilePath = $"{ extrafolder }{ filePath }";
            }
        }
    }
}
