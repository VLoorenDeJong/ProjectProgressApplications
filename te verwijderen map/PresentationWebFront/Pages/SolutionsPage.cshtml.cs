using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PresentationWebFront.Models;
using ProjectProgressLibrary.DataAccess;
using ProjectProgressLibrary.Models;
using static PresentationWebFront.Enums.Enums;
using static PresentationWebFront.Logic;

namespace PresentationWebFront.Pages
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
        private readonly ILogger _logger;
        private readonly IDataAccess _db;
        private readonly IConfiguration _config;
        private readonly string _mainGoal;
        private List<ProjectModel> AllProjects { get; set; } = new List<ProjectModel>();
        private ProjectModel MainProject { get; set; }

        public SolutionsPageModel(ILogger<IndexModel> logger, IConfiguration config, IDataAccess db)
        {

            _logger = logger;
            _config = config;
            (_db, _mainGoal) = GetDbConfig(config, db, "index");

            AllProjects = _db.ReadAllProjectRecords(_mainGoal);

            MainProject = _db.GetProjectByTitle(_mainGoal, AllProjects);

        }
        public void OnGet()
        {
            LoadPageSettings();
            PageSolutions = LoadAllDictionaries(AllProjects, DictionaryClassification);
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
                case "Challenges":
                    return RedirectToPage( new { classification = "Challenges" });
                case "FutureAdditions":
                    return RedirectToPage( new { classification = "FutureAdditions" });
                case "All":
                    return RedirectToPage( new { classification = "All" });
                default:
                    return RedirectToPage();
            }
        }
        public IActionResult OnPostSearch()
        {
            return RedirectToPage(new { SearchTerm = SearchTerm,
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
                case "Challenges":
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
