using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProjectProgressLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ProgressApplicationMVP.Logic;

namespace ProgressApplicationMVP.Pages
{
    public class IndexModel : PageModel
    {

        //ToDo Make all pages have a IsDemo bool
        private readonly IStartConfig _startConfig;
        private readonly ILogger<IndexModel> _logger;
        public readonly string _MainGoal;
        private readonly IDataAccess _db;

        public IndexModel(ILogger<IndexModel> logger, IDataAccess db, IConfiguration config, IStartConfig startConfig)
        {
            _startConfig = startConfig;
            _logger = logger;
            (_db, _MainGoal) = _startConfig.GetProgressDbConfig(config, db, "index");
        }

        public void OnGet()
        {

        }
    }
}
