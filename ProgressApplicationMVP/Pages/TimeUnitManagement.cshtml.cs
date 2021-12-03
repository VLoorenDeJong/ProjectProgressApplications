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
using static ProgressApplicationMVP.Logic;

namespace ProgressApplicationMVP.Pages
{
    public class TimeUnitManagementModel : PageModel
    {
        private readonly IDataAccess _db;
        private readonly ILogger<TimeUnitManagementModel> _logger;
        private readonly string _MainGoal;
        public List<TimeUnitModel> AllTimeUnits;
        public List<ProjectModel> AllProjects;

        [BindProperty(SupportsGet = true)]
        public Guid TimeUnitId { get; set; }
        [BindProperty(SupportsGet = true)]
        public TimeUnitModel TimeUnit { get; set; }
        public TimeUnitManagementModel(ILogger<TimeUnitManagementModel> logger, IDataAccess db, IConfiguration config)
        {
            _logger = logger;


            (_db, _MainGoal) = GetDbConfig(config, db, "timeUnitManagement");

            AllProjects = _db.ReadAllProjectRecords(_MainGoal);
            AllTimeUnits = _db.ReadAllTimeUnits(_MainGoal);

            AllTimeUnits = AllTimeUnits.OrderByDescending(x => x.TimeStamp).ToList();

        }
        public void OnGet()
        {
        }
        public IActionResult OnPostEdit(string timeUnit)
        {
            return RedirectToPage("TimeUnitPage", new { timeUnit });
        }
        public IActionResult OnPostRemove(string timeUnit)
        {
            Guid timeUnitGuid = new Guid(timeUnit);
            TimeUnitModel timeUnitToDelete = _db.GetTimeUnitById(timeUnitGuid, AllTimeUnits);
            ProjectModel projectWithTime = _db.GetProjectById(timeUnitToDelete.ProjectId, AllProjects);


            _db.RemoveTime(projectWithTime, timeUnitToDelete, AllProjects, AllTimeUnits);
            return RedirectToPage();
        }
    }
}
