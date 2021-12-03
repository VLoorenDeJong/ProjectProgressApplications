using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PortfolioMVP.Models;
using ProjectProgressLibrary.DataAccess;
using ProjectProgressLibrary.Models;
using static PortfolioMVP.Logic;
using static ProjectProgressLibrary.Enums;
using static ProjectProgressLibrary.ApplicationLogic;

namespace PortfolioMVP.Pages
{
    public class TimeUnitOverviewPageModel : PageModel
    {
        // Front end
        [BindProperty]
        public string DayOfWeek { get; set; }
        [BindProperty]
        public double SearchTotalHours { get; set; }
        [BindProperty]
        public double SearchGeneralHours { get; set; }
        [BindProperty]
        public double SearchPracticalHours { get; set; }
        [BindProperty]
        public double SearchTheoreticalHours { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime SearchDate { get; set; }
        [BindProperty]
        public List<ProjectModel> ProjectsToFind { get; set; }
        [BindProperty]
        public List<TimeUnitModel> TimeUnitsToShow { get; set; }
        [BindProperty]
        public List<TimeDisplayModel> WorkedOnProjects { get; set; } = new List<TimeDisplayModel>();
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool SearchProjectEnabled { get; set; } = false;
        [BindProperty(SupportsGet = true)]
        public bool SearchByDateEnabled { get; set; }
        [BindProperty]
        public bool IsFirstPage { get; set; } = false;
        [BindProperty]
        public double TotalHours { get; private set; }
        [BindProperty]
        public double TheoreticalHours { get; private set; }
        [BindProperty]
        public double PracticalHours { get; private set; }
        [BindProperty]
        public double GeneralHours { get; private set; }
        // Backend
        private readonly ILogger _logger;
        private readonly IDataAccess _db;
        private readonly IConfiguration _config;
        private readonly string _mainGoal;
        private List<TimeUnitModel> AllTimeUnits { get; set; } = new List<TimeUnitModel>();
        public List<ProjectModel> AllProjects { get; set; } = new List<ProjectModel>();
        private ProjectModel MainProject { get; set; }

        public TimeUnitOverviewPageModel(ILogger<TimeUnitOverviewPageModel> logger, IConfiguration config, IDataAccess db)
        {
            _logger = logger;
            _config = config;
            (_db, _mainGoal) = GetDbConfig(config, db, "index");

            (AllProjects, AllTimeUnits) = _db.ReadAllRecords(_mainGoal);

            MainProject = _db.GetProjectByTitle(_mainGoal, AllProjects);

        }
        public void OnGet()
        {
            LoadPageHours(MainProject);
            TimeUnitsToShow = AllTimeUnits.OrderByDescending(x => x.TimeStamp).ToList();            
            
            SearchDate = CreateDateFromString(SearchDate.ToString());


            if (SearchByDateEnabled == false || SearchDate.ToString() == "01/01/0001 12:00:00 AM"
                                             || SearchDate.ToString() == "01/01/0001 00:00:00")
            {
                SearchDate = GetDefaultDate();
            }


            if (SearchProjectEnabled == true && string.IsNullOrEmpty(SearchTerm) == false)
            {
                GetProjectSeachResults();
            }
            if (SearchByDateEnabled == true)
            {
                GetDateSearchResults();
            }

            if (SearchProjectEnabled == false && SearchByDateEnabled == false)
            {
                SearchByDateEnabled = true;
                IsFirstPage = true;
                GetDateSearchResults();
            }
        }

        public IActionResult OnPostSearchProject()
        {
            return RedirectToPage(new
            {
                SearchTerm = SearchTerm,
                SearchProjectEnabled = true
            });
        }

        public IActionResult OnPostSearchDate()
        {
            return RedirectToPage(new
            {
                SearchByDateEnabled = true,
                SearchDate = SearchDate
            });
        }
        private void GetProjectSeachResults()
        {
            TimeUnitsToShow = AllTimeUnits.Where(x => x.ProjectTitle.ToLower().Contains(SearchTerm.ToLower())).ToList();
            TimeUnitsToShow = TimeUnitsToShow.OrderByDescending(x => x.TimeStamp).ToList();

            ProjectsToFind = AllProjects.Where(x => x.Title.ToLower().Contains(SearchTerm.ToLower())).ToList();
            ProjectsToFind = ProjectsToFind.OrderBy(x => x.Title).ToList();
        }
        private void LoadPageHours(ProjectModel project)
        {
            TotalHours = project.TotalHours;
            GeneralHours = project.GeneralHours;
            TheoreticalHours = project.TheoreticalHours;
            PracticalHours = project.PracticalHours;
        }

        private void GetDateSearchResults()
        {
            SearchDate = SearchDate;
            DayOfWeek = DecideDayOfWeek(SearchDate);
            GetAllTimeUnitsToShow();
            GetAllHours();
            GenerateAllWorkedOnProjects();
        }

        private void GetAllTimeUnitsToShow()
        {
            TimeUnitsToShow = AllTimeUnits.Where(x => x.TimeStamp >= SearchDate).ToList();
        }
        private void GetAllHours()
        {
            foreach (TimeUnitModel timeUnit in TimeUnitsToShow)
            {

                SearchTotalHours = SearchTotalHours + timeUnit.HoursPutIn;
                switch (timeUnit.Classification)
                {
                    case HourClassification.Practical:
                        SearchPracticalHours = SearchPracticalHours + timeUnit.HoursPutIn;
                        break;
                    case HourClassification.Theoretical:
                        SearchTheoreticalHours = SearchTheoreticalHours + timeUnit.HoursPutIn;
                        break;
                    case HourClassification.General:
                        SearchGeneralHours = SearchGeneralHours + timeUnit.HoursPutIn;
                        break;
                    default:
                        break;
                }
            }
        }
        private void GenerateAllWorkedOnProjects()
        {
            // Sort out what projeccts are worked on
            var results = TimeUnitsToShow.GroupBy(x => x.ProjectTitle).Select(y => y.First()).ToList();

            //for eacht project calculate hours
            foreach (var result in results)
            {
                TimeDisplayModel displayModelToAdd = new TimeDisplayModel();

                // Get timeunits within time span
                List<TimeUnitModel> timeUnitsInTimeSpan = TimeUnitsToShow.Where(x => x.ProjectTitle == result.ProjectTitle).ToList();

                // Calculate hours within time span
                double hoursPutIn = GetHoursInTimeSpan(timeUnitsInTimeSpan);

                displayModelToAdd.ProjectTitle = result.ProjectTitle;
                displayModelToAdd.TotalHours = hoursPutIn;

                WorkedOnProjects.Add(displayModelToAdd);
                WorkedOnProjects = WorkedOnProjects.OrderByDescending(x => x.TotalHours).ToList();
            }
        }

        private double GetHoursInTimeSpan(List<TimeUnitModel> timeUnitsInTimeSpan)
        {
            double hoursPuIn = 0;

            foreach (TimeUnitModel timeUnit in timeUnitsInTimeSpan)
            {
                hoursPuIn = hoursPuIn + timeUnit.HoursPutIn;
            }
            return hoursPuIn;
        }
    }
}
