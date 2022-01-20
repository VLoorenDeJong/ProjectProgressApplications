using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProjectProgressLibrary.DataAccess;
using static ProjectProgressLibrary.Enums;
using static ProgressApplicationMVP.Logic;
using ProjectProgressLibrary.Models;
using static ProjectProgressLibrary.Validation.DataValidation;
using ProjectProgressLibrary.StartConfig;

namespace ProgressApplicationMVP.Pages
{
    public class TimeUnitPageModel : PageModel
    {
        private readonly string _MainGoal;
        private readonly IDataAccess _db;
        private readonly ILogger<TimeUnitPageModel> _logger;
        private readonly IStartConfig _startConfig;

        //ToDo check if lists can be private
        public List<TimeUnitModel> AllTimeUnits;
        private List<ProjectModel> allProjects;
        [BindProperty(SupportsGet = true)]
        public bool CanAddToExistsingProject { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool FromManagementPage { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool ForgotProjectSelection { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool ForgotHours { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool TimeUnitLoaded { get; set; }
        [BindProperty]
        public TimeUnitModel TimeUnitToAdd { get; set; } = new TimeUnitModel();
        [BindProperty(SupportsGet = true)]
        public double HoursPutIn { get; set; }
        [BindProperty(SupportsGet = true)]
        public Guid ProjectId { get; set; }
        [BindProperty(SupportsGet = true)]
        public Guid TimeUnitId { get; set; }
        [BindProperty(SupportsGet = true)]
        public HourClassification Classification { get; set; } = HourClassification.General;
        [BindProperty(SupportsGet = true)]
        public string ProjectTitle { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime TimeStamp { get; set; } = DateTime.Now;
        public List<ProjectModel> AllProjects { get => allProjects; set => allProjects = value; }

        public TimeUnitPageModel(ILogger<TimeUnitPageModel> logger, IDataAccess db, IConfiguration config, IStartConfig startConfig)
        {
            _logger = logger;
            _startConfig = startConfig;

            (_db, _MainGoal) = _startConfig.GetProgressDbConfig(config, db, "timeUnitPage");

            AllProjects = _db.ReadAllProjectRecords(_MainGoal);
            AllTimeUnits = _db.ReadAllTimeUnits(_MainGoal);

            AllProjects = AllProjects.OrderBy(x => x.Title).ToList();

        }
        public void OnGet(string timeUnit)
        {
            if (ProjectTitle.ValidateStringHasContent() == true)
            {
                FromManagementPage = true;
            }

            if (string.IsNullOrEmpty(timeUnit) == false)
            {
                Guid timeUnitId = new Guid(timeUnit);
                TimeUnitToAdd = _db.GetTimeUnitById(timeUnitId, AllTimeUnits);
                ProjectTitle = TimeUnitToAdd.ProjectTitle;
                TimeUnitId = TimeUnitToAdd.TimeUnitId;
                HoursPutIn = TimeUnitToAdd.HoursPutIn;
                Classification = TimeUnitToAdd.Classification;
                ProjectId = TimeUnitToAdd.ProjectId;
                TimeStamp = TimeUnitToAdd.TimeStamp;

                TimeUnitLoaded = true;
            }
        }
        public IActionResult OnPost(string title)
        {
            // Check if title is loaded
            bool hasContent = title.ValidateStringHasContent();
            if (hasContent == true)
            {
                ProjectTitle = title;
            }
            // If title is loaded redirects to page with title
            if (hasContent == true)
            {
                return RedirectToPage(new
                {
                    ProjectTitle = ProjectTitle,
                    FromManagementPage = true
                });
            }
            // If no title is loaded reloads page with entered data and sets ForgotProjectSelection = true 
            if (hasContent == false)
            {
                return RedirectToPage(new
                {
                    Classification = Classification,
                    HoursPutIn = HoursPutIn,
                    ForgotProjectSelection = true
                }); ;
            }
            return RedirectToPage();
        }



        public IActionResult OnPostSave()
        {
            if (string.IsNullOrEmpty(ProjectTitle) == true)
            {
                return RedirectToPage(new
                {
                    HoursPutIn = HoursPutIn,
                    Classification = Classification,
                    ForgotProjectSelection = true
                });

            }
            if (HoursPutIn == 0)
            {
                return RedirectToPage(new
                {
                    ProjectTitle = ProjectTitle,
                    Classification = Classification,
                    ForgotHours = true
                });
            }
            SaveTimeUnit();
            return RedirectToPage();
        }


        public IActionResult OnPostSaveChangeProject(string timeUnit)
        {
            if (HoursPutIn == 0)
            {
                return RedirectToPage(new
                {
                    ProjectTitle = ProjectTitle,
                    Classification = Classification,
                    ForgotHours = true
                });
            }
            HandelChangedTimeUnit(timeUnit, true);
            return RedirectToPage("ProjectManagementPage");
        }
        public IActionResult OnPostSaveChangedTimeUnit(string timeUnit)
        {
            HandelChangedTimeUnit(timeUnit, false);
            return RedirectToPage("TimeUnitManagement");
        }

        private void HandelChangedTimeUnit(string stringFromFrontEnd, bool isNewTimeUnit)
        {
            ProjectModel projectToEdit = new ProjectModel();

            if (isNewTimeUnit == false)
            {

                Guid timeGuid = new Guid(stringFromFrontEnd);
                TimeUnitModel timeUnitToChange = _db.GetTimeUnitById(timeGuid, AllTimeUnits);
                projectToEdit = _db.GetProjectById(timeUnitToChange.ProjectId, AllProjects);

                _db.RemoveTime(projectToEdit, timeUnitToChange, AllProjects, AllTimeUnits);

                // Get new project loaded and input loaded for saving
                projectToEdit = _db.GetProjectByTitle(ProjectTitle, AllProjects);
                SaveInputToTimeUnitToAdd(projectToEdit.ProjectId, stringFromFrontEnd);
            }
            if (isNewTimeUnit == true)
            {
                projectToEdit = _db.GetProjectByTitle(stringFromFrontEnd, AllProjects);
                SaveInputToTimeUnitToAdd(projectToEdit.ProjectId);
            }

            _db.AddTime(projectToEdit, TimeUnitToAdd, AllProjects, AllTimeUnits);

        }

        private void SaveInputToTimeUnitToAdd(Guid projectId, string stringGuid = "")
        {
            if (stringGuid == "")
            {
                stringGuid = Guid.NewGuid().ToString();
            }
            string projectGuidString = projectId.ToString();

            TimeUnitToAdd.SetProjectTitle(ProjectTitle);
            TimeUnitToAdd.SetProjectId(projectGuidString);
            TimeUnitToAdd.SetHoursPutIn(HoursPutIn.ToString());
            TimeUnitToAdd.SetClassification(Classification);
            TimeUnitToAdd.SetTimeUnitId(stringGuid);
            TimeUnitToAdd.SetTimeStamp(TimeStamp.ToString());
        }

        private void SaveTimeUnit()
        {
            (AllProjects, AllTimeUnits) = _db.ReadAllRecords(_MainGoal);

            ProjectModel projectToAddHoursTo = _db.GetProjectByTitle(ProjectTitle, AllProjects);

            SaveInputToTimeUnitToAdd(projectToAddHoursTo.ProjectId);


            if (projectToAddHoursTo.ProjectStatus == ProjectStatus.ToDo)
            {
                _db.ChangeProjectStatus(ProjectTitle, ProjectStatus.Doing, AllProjects);
            }

            _db.AddTime(projectToAddHoursTo, TimeUnitToAdd, AllProjects, AllTimeUnits);
        }
    }
}
