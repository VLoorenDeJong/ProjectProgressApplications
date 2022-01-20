using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProjectProgressLibrary.DataAccess;
using ProjectProgressLibrary.Models;
using ProjectProgressLibrary.StartConfig;

namespace PortfolioMVP.Pages
{
    public class TestPageModel : PageModel
    {
        // Front end
        [BindProperty]
        public ProjectModel FakeModel { get; set; } = new ProjectModel();
        [BindProperty]
        public double TotalHours { get; private set; }
        [BindProperty]
        public double TheoreticalHours { get; private set; }
        [BindProperty]
        public double PracticalHours { get; private set; }
        [BindProperty]
        public double GeneralHours { get; private set; }

        // Backend
        public readonly string _mainGoal;
        private readonly IDataAccess _db;
        private readonly IStartConfig _startConfig;
        private readonly ILogger<TestPageModel> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public List<ProjectModel> AllProjects = new List<ProjectModel>();
        private List<TimeUnitModel> AllTimeUnits = new List<TimeUnitModel>();

        public TestPageModel(ILogger<TestPageModel> logger, IDataAccess db, IConfiguration config, IWebHostEnvironment webHostEnvironment, IStartConfig startConfig)
        {
            _startConfig = startConfig;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;

            (_db, _mainGoal) = startConfig.GetDbConfig(config, db, "projectPage");

            (AllProjects, AllTimeUnits) = _db.ReadAllRecords(_mainGoal);

        }
        public void OnGet()
        {
            CreateFakePageModel();
            LoadPageHours();
        }

        private void CreateFakePageModel()
        {

            FakeModel.Title = "Fake Model";
            FakeModel.ShortDescription = "Korte omschrijving van wat een nep model is en wat hij kan en mischien wel wil?";
            FakeModel.Outcome = "nee het is toch een ander vak waar je in moet vullen wat je wil maar ja dat is dan ook weer klaar";
            FakeModel.DemoLink = "Fake demo link hier";
            FakeModel.GitHubLink = "Fake GitHub link hier";
            FakeModel.ShowItem = true;
            FakeModel.HasDemo = true;

            FakeModel.SetTotalHours(1.27.ToString());
            FakeModel.SetTheoreticalHours(1.05.ToString());
            FakeModel.SetPracticalHours(1.57.ToString());
            FakeModel.SetGeneralHours(58.68.ToString());

            FakeModel.AddChallenge("Help1 help help help help help1 help help help help help1", new List<string> { });
            FakeModel.AddChallenge("Help2 help help help help help2 help help help help help2", new List<string> { });
            FakeModel.AddChallenge("Help3 help help help help help3 help help help help help3", new List<string> { });
            FakeModel.AddChallenge("Help4 help help help help help4 help help help help help4", new List<string> { });
            FakeModel.AddChallenge("Help5 help help help help help5 help help help help help5", new List<string> { });

            FakeModel.AddFutureAddition("Toekomst1, toekomst, toekomst, toekomst, toekomst, toekomst, toekomst1", new List<string> { });
            FakeModel.AddFutureAddition("Toekomst2, toekomst, toekomst, toekomst, toekomst, toekomst, toekomst2", new List<string> { });
            FakeModel.AddFutureAddition("Toekomst3, toekomst, toekomst, toekomst, toekomst, toekomst, toekomst3", new List<string> { });
            FakeModel.AddFutureAddition("Toekomst4, toekomst, toekomst, toekomst, toekomst, toekomst, toekomst4", new List<string> { });
            FakeModel.AddFutureAddition("Toekomst5, toekomst, toekomst, toekomst, toekomst, toekomst, toekomst5", new List<string> { });


        }

        private void LoadPageHours()
        {
            TotalHours = FakeModel.TotalHours;
            TheoreticalHours = FakeModel.TheoreticalHours;
            PracticalHours = FakeModel.PracticalHours;
            GeneralHours = FakeModel.GeneralHours;
        }
    }
}
