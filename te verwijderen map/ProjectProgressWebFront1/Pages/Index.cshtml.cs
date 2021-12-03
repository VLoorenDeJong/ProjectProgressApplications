using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProjectProgressLibrary.DataAccess;
using ProjectProgressLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ProjectProgressWebFront1.Logic;

namespace ProjectProgressWebFront1.Pages
{
    public class IndexModel : PageModel
    {
        private readonly string _MainGoal;
        private readonly string _ProjectFilePath = "";
        private readonly string _TimeUnitFilePath = "";
        private readonly IDataAccess _db;
        private readonly ILogger<IndexModel> _logger;
        private List<ProjectModel> allProjects;
        private List<TimeUnitModel> allTimeUnits;

        public IndexModel(ILogger<IndexModel> logger, IDataAccess db, IConfiguration config)
        {
            (_db, _MainGoal) = GetDbConfig(config, db, "index");

        }

        public void OnGet()
        {            
        }
    }
}
