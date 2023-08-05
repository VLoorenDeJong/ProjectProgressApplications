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
using static ProgressApplicationMVP.Logic;

namespace ProgressApplicationMVP.Pages
{
    public class TimeUnitManagementModel : PageModel
    {
        private readonly IDataAccess _db;
        private readonly ILogger<TimeUnitManagementModel> _logger;
        private readonly IStartConfig _startConfig;
        private readonly string _MainGoal;
        public List<TimeUnitModel> AllTimeUnits;
        public List<ProjectModel> AllProjects;

        [BindProperty(SupportsGet = true)]
        public Guid TimeUnitId { get; set; }
        [BindProperty(SupportsGet = true)]
        public TimeUnitModel TimeUnit { get; set; }
        public TimeUnitManagementModel(ILogger<TimeUnitManagementModel> logger, IDataAccess db, IConfiguration config, IStartConfig startConfig)
        {
            _logger = logger;
            _startConfig = startConfig;

            (_db, _MainGoal) = _startConfig.GetProgressDbConfig(config, db, "timeUnitManagement");
        }
        public async Task OnGet()
        {
            (AllProjects, AllTimeUnits) = await _db.ReadAllRecordsAsync(_MainGoal);

            AllTimeUnits = AllTimeUnits.OrderByDescending(x => x.TimeStamp).ToList();
        }
        public IActionResult OnPostEdit(string timeUnit)
        {
            return RedirectToPage("TimeUnitPage", new { timeUnit });
        }
        public async Task<IActionResult> OnPostRemove(string timeUnit)
        {
            (AllProjects, AllTimeUnits) = await _db.ReadAllRecordsAsync(_MainGoal);

            Guid timeUnitGuid = new Guid(timeUnit);
            TimeUnitModel timeUnitToDelete = _db.GetTimeUnitById(timeUnitGuid, AllTimeUnits);
            ProjectModel projectWithTime = _db.GetProjectById(timeUnitToDelete.ProjectId, AllProjects);


            _db.RemoveTime(projectWithTime, timeUnitToDelete, AllProjects, AllTimeUnits);
            return RedirectToPage();
        }
    }
}
