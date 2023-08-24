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
using static ProjectProgressLibrary.Enums;

namespace PortfolioMVP.Pages
{
    public class SolutionsPageModel : PageModel
    {
        // Front end
        [BindProperty]
        public DictionaryClassification DictionaryClassification { get; private set; } = DictionaryClassification.All;
        [BindProperty]
        public double TotalHours { get; private set; }
        [BindProperty]
        public double TheoreticalHours { get; private set; }
        [BindProperty]
        public double PracticalHours { get; private set; }
        [BindProperty]
        public double GeneralHours { get; private set; }
        [BindProperty(SupportsGet = true)]
        public string classification { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool SearchEnabled { get; set; } = false;
        [BindProperty]
        public List<SolutionModel> PageSolutions { get; set; }



        // Backend
        private readonly IStartConfig _startConfig;
        private readonly ILogger _logger;
        private readonly IDataAccess _db;
        private readonly IConfiguration _config;
        private readonly string _mainGoal;
        private List<ProjectModel> AllProjects { get; set; } = new List<ProjectModel>();
        private ProjectModel MainProject { get; set; }


        public SolutionsPageModel(ILogger<SolutionsPageModel> logger, IConfiguration config, IDataAccess db, IStartConfig startConfig)
        {
            _startConfig = startConfig;
            _logger = logger;
            _config = config;

            (_db, _mainGoal) = _startConfig.GetProgressDbConfig(config, db, "solutions");

            MainProject = _db.GetProjectByTitle(_mainGoal, AllProjects);
        }

        public async Task OnGet()
        {
            AllProjects = await _db.ReadAllProjectRecordsAsync(_mainGoal);

            LoadPageSettings();
            PageSolutions = _db.LoadAllDictionaries(AllProjects, DictionaryClassification);
            if (SearchEnabled == true && string.IsNullOrEmpty(SearchTerm) == false)
            {
                PageSolutions = PageSolutions.Where(x => x.Key.ToLower().Contains(SearchTerm.ToLower())).ToList();
               

            }
            PageSolutions = PageSolutions.OrderBy(x => x.Key).ToList();
            
        }

        public IActionResult OnPostClassification()
        {
            switch (classification)
            {
                case DictionaryMode.Challenges:
                    return RedirectToPage(new { classification = DictionaryMode.Challenges });
                case "FutureAdditions":
                    return RedirectToPage(new { classification = "FutureAdditions" });
                case "All":
                    return RedirectToPage(new { classification = "All" });
                default:
                    return RedirectToPage();
            }
        }
        public IActionResult OnPostSearch()
        {
            return RedirectToPage(new
            {
                SearchTerm = SearchTerm,
                SearchEnabled = true,
                Classification = "All"
            });
        }

        private void LoadPageSettings()
        {
            LoadPageHours(MainProject);
            LoadPageDictionary();
        }

        private void LoadPageDictionary()
        {
            switch (classification)
            {
                case DictionaryMode.Challenges:
                    DictionaryClassification = DictionaryClassification.Challenges;
                    break;
                case "FutureAdditions":
                    DictionaryClassification = DictionaryClassification.FutureAdditions;
                    break;
                case "All":
                    DictionaryClassification = DictionaryClassification.All;
                    break;
                default:
                    break;
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
