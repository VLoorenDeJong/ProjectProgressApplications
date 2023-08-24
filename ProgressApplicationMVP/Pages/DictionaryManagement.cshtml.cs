using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProjectProgressLibrary.Interfaces;
using ProjectProgressLibrary.Models;
using ProjectProgressLibrary.Validation;


namespace ProgressApplicationMVP.Pages
{
    public class DictionaryManagementModel : PageModel
    {
        public readonly string Delimiter = "!*&";
        private readonly string _MainGoal;
        // FrontEnd
        [BindProperty(SupportsGet = true)]
        public bool FutureFeaturesLoaded { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool ChallengesLoaded { get; set; }
        [BindProperty(SupportsGet = true)]
        public string ProjectTitle { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Mode { get; set; }
        [BindProperty(SupportsGet = true)]
        public string ItemToSearch { get; set; }
        [BindProperty]
        public string KeyItemPlaceholderText { get; set; }
        [BindProperty]
        public string ValueItemPlaceHolderText { get; set; }
        [BindProperty]
        public string KeyTableHead { get; set; }
        [BindProperty]
        public string ValueTableHead { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool NoKeyValueEnterred { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool NoValueEntered { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool ItemKeyFound { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool ValueFound { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool SearchEnabled { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool EditEnabled { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool FromValuePage { get; set; }


        // BackEnd
        private IStartConfig _startConfig;
        private readonly IDataAccess _db;
        //See if private is ok
        private List<ProjectModel> AllProjects = new List<ProjectModel>();

        private ProjectModel ProjectToChange { get; set; }
        [BindProperty(SupportsGet = true)]
        public Dictionary<string, List<string>> DictionaryToChange { get; private set; } = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> SearchDictionary { get; private set; } = new Dictionary<string, List<string>>();
        [BindProperty(SupportsGet = true)]
        public string EnteredKey { get; set; }
        [BindProperty(SupportsGet = true)]
        public string EnteredValue { get; set; }
        [BindProperty(SupportsGet = true)]
        public string OldKey { get; set; }
        [BindProperty(SupportsGet = true)]
        public string NewKeyValue { get; set; }

        public DictionaryManagementModel(IDataAccess db, IConfiguration config, IStartConfig startConfig)
        {
            _startConfig = startConfig;
            (_db, _MainGoal) = _startConfig.GetProgressDbConfig(config, db, "dictionaryManagement");
        }

        public async Task OnGet()
        {

            AllProjects = await _db.ReadAllProjectRecordsAsync(_MainGoal);

            if (FromValuePage == true)
            {
                if (Mode == "Challenges")
                {
                    ChallengesLoaded = true;
                }
                if (Mode == "Additions")
                {
                    FutureFeaturesLoaded = true;
                }
            }
            LoadTheRightPageValues();
            if (SearchEnabled == true)
            {
                (ProjectToChange, DictionaryToChange) = _db.LoadProjectDetails(ProjectTitle, AllProjects, FutureFeaturesLoaded, ChallengesLoaded, _db);

                if (string.IsNullOrEmpty(ItemToSearch) == false)
                {
                    List<string> allDictionaryKeys = DictionaryToChange.Select(x => x.Key).ToList();
                    List<string> dictionaryKeysToFind = _db.SearchInCollection(ItemToSearch, allDictionaryKeys);

                    foreach (string key in dictionaryKeysToFind)
                    {
                        string itemFound = DictionaryToChange.Where(x => x.Key == key).Select(x => x.Key).First();
                        List<string> valuesFound = _db.MakeListFromDictionaryItemValues(DictionaryToChange, key);
                        SearchDictionary.Add(key, valuesFound);
                    }

                    DictionaryToChange = SearchDictionary;
                }

                DictionaryToChange = DictionaryToChange;
            }
            if (SearchEnabled == false)
            {
                (ProjectToChange, DictionaryToChange) = _db.LoadProjectDetails(ProjectTitle, AllProjects, FutureFeaturesLoaded, ChallengesLoaded, _db);
            }
        }

        public IActionResult OnPost()
        {
            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostSearchItem(string item)
        {
            AllProjects = await _db.ReadAllProjectRecordsAsync(_MainGoal);

            GetValuesFromButton(item);

            bool hasSomethingToSearch = ItemToSearch.ValidateStringHasContent();
            ProjectModel currentProject = _db.GetProjectByTitle(ProjectTitle, AllProjects);

            List<string> itemKeys = currentProject.Challenges.Select(x => x.Key).ToList();

            bool itemExists = ItemToSearch.ValidateIfItemExists(itemKeys);

            if (hasSomethingToSearch == true && itemExists == true)
            {
                return RedirectToPage(new
                {
                    ProjectTitle = ProjectTitle,
                    Mode = Mode,
                    FutureFeaturesLoaded = FutureFeaturesLoaded,
                    ChallengesLoaded = ChallengesLoaded,
                    ItemToSearch = ItemToSearch,
                    SearchEnabled = true
                });

            }

            return RedirectToPage(new
            {
                ProjectTitle = ProjectTitle,
                Mode = Mode,
                FutureFeaturesLoaded = FutureFeaturesLoaded,
                ChallengesLoaded = ChallengesLoaded,
                ItemToSearch = ItemToSearch
            });
        }
        public async Task<IActionResult> OnPostAddItem(string item)
        {
            AllProjects = await _db.ReadAllProjectRecordsAsync(_MainGoal);

            GetValuesFromButton(item);

            // Entery check key
            if (string.IsNullOrEmpty(EnteredKey) == true)
            {
                return RedirectToPage(new
                {
                    ProjectTitle = ProjectTitle,
                    Mode = Mode,
                    NoKeyValueEnterred = true,
                    EnteredValue = EnteredValue,
                    FutureFeaturesLoaded = FutureFeaturesLoaded,
                    ChallengesLoaded = ChallengesLoaded

                }); ;
            }


            (ProjectToChange, DictionaryToChange) = _db.LoadProjectDetails(ProjectTitle, AllProjects, FutureFeaturesLoaded, ChallengesLoaded, _db);

            foreach (var key in DictionaryToChange)
            {
                // Same item Key Check
                if (key.Key.ToLower() == EnteredKey.ToLower())
                {
                    return RedirectToPage(new
                    {
                        ProjectTitle,
                        ItemKeyFound = true,
                        Mode = Mode,
                        EnteredKey = EnteredKey,
                        EnteredValue = EnteredValue,
                        FutureFeaturesLoaded = FutureFeaturesLoaded,
                        ChallengesLoaded = ChallengesLoaded

                    });

                }
            }



            string firstValue = $"No { ValueItemPlaceHolderText.ToLower()} yet.";
            DictionaryToChange.Add(EnteredKey, new List<string> { firstValue });

            ProjectToChange = _db.OverrideDictionaryInProject(ProjectToChange, DictionaryToChange, FutureFeaturesLoaded, ChallengesLoaded);

            _db.SaveProject(ProjectToChange, AllProjects);
            return RedirectToPage(new
            {
                ProjectTitle = ProjectTitle,
                Mode = Mode,
                FutureFeaturesLoaded = FutureFeaturesLoaded,
                ChallengesLoaded = ChallengesLoaded
            });
        }
        public IActionResult OnPostEditItems(string item)
        {
            GetValuesFromButton(item);

            return RedirectToPage(new
            {
                ProjectTitle = ProjectTitle,
                Mode = Mode,
                FutureFeaturesLoaded = FutureFeaturesLoaded,
                ChallengesLoaded = ChallengesLoaded,
                EditEnabled = true
            });
        }
        public IActionResult OnPostEditKey(string item)
        {
            GetValuesFromButton(item);

            return RedirectToPage(new
            {
                ProjectTitle = ProjectTitle,
                Mode = Mode,
                FutureFeaturesLoaded = FutureFeaturesLoaded,
                ChallengesLoaded = ChallengesLoaded,
                OldKey = OldKey,
                EditEnabled = true
            }

                );
        }
        public async Task<IActionResult> OnPostRemoveItem(string item)
        {

            AllProjects = await _db.ReadAllProjectRecordsAsync(_MainGoal);

            GetValuesFromButton(item);

            (ProjectToChange, DictionaryToChange) = _db.LoadProjectDetails(ProjectTitle, AllProjects, FutureFeaturesLoaded, ChallengesLoaded, _db);

            DictionaryToChange.Remove(OldKey);

            ProjectToChange = _db.OverrideDictionaryInProject(ProjectToChange, DictionaryToChange, FutureFeaturesLoaded, ChallengesLoaded);

            _db.SaveProject(ProjectToChange, AllProjects);

            return RedirectToPage(new
            {
                ProjectTitle = ProjectTitle,
                FutureFeaturesLoaded = FutureFeaturesLoaded,
                ChallengesLoaded = ChallengesLoaded,
                Mode = Mode
            });
        }
        public async Task<IActionResult> OnPostSaveKey(string item)
        {
            AllProjects = await _db.ReadAllProjectRecordsAsync(_MainGoal);

            // ToDo test wit values
            GetValuesFromButton(item);

            LoadTheRightPageValues();

            (ProjectToChange, DictionaryToChange) = _db.LoadProjectDetails(ProjectTitle, AllProjects, FutureFeaturesLoaded, ChallengesLoaded, _db);

            bool doubleKey = IsKeyDouble();

            if (doubleKey == false)
            {
                List<string> oldValuesItems = new List<string>();

                foreach (var key in DictionaryToChange)
                {
                    if (key.Key == OldKey)
                    {
                        foreach (string value in key.Value)
                        {
                            oldValuesItems.Add(value);
                        }
                    }
                }

                DictionaryToChange.Add(NewKeyValue, oldValuesItems);
                DictionaryToChange.Remove(OldKey);

                ProjectToChange = _db.OverrideDictionaryInProject(ProjectToChange, DictionaryToChange, FutureFeaturesLoaded, ChallengesLoaded);

                _db.SaveProject(ProjectToChange, AllProjects);

                return RedirectToPage(new
                {
                    ProjectTitle = ProjectTitle,
                    FutureFeaturesLoaded = FutureFeaturesLoaded,
                    ChallengesLoaded = ChallengesLoaded,
                    Mode = Mode
                });
            }
            return RedirectToPage(new
            {
                ProjectTitle = ProjectTitle,
                FutureFeaturesLoaded = FutureFeaturesLoaded,
                ChallengesLoaded = ChallengesLoaded,
                Mode = Mode
            });
        }
        public IActionResult OnPostEditValues(string item)
        {
            GetValuesFromButton(item);

            LoadTheRightPageValues();

            return RedirectToPage("DictionaryValuesManagementPage", new
            {
                ProjectTitle = ProjectTitle,
                FutureFeaturesLoaded = FutureFeaturesLoaded,
                ChallengesLoaded = ChallengesLoaded,
                Mode = Mode,
                ItemTitle = OldKey

            });
        }
        public async Task<IActionResult> OnPostAddValue(string key)
        {
            AllProjects = await _db.ReadAllProjectRecordsAsync(_MainGoal);

            if (string.IsNullOrEmpty(EnteredValue) == true)
            {

                return RedirectToPage(new
                {
                    ProjectTitle = ProjectTitle,
                    FutureFeaturesLoaded = FutureFeaturesLoaded,
                    ChallengesLoaded = ChallengesLoaded,
                    Mode = Mode,
                    NoValueEntered = true
                });

            }

            (ProjectToChange, DictionaryToChange) = _db.LoadProjectDetails(ProjectTitle, AllProjects, FutureFeaturesLoaded, ChallengesLoaded, _db);

            bool isNewValue = IsValueDouble(key);

            if (isNewValue == true)
            {
                foreach (var dictionaryItem in DictionaryToChange)
                {
                    if (dictionaryItem.Key == key)
                    {
                        dictionaryItem.Value.Add(EnteredValue);
                    }
                }

            }

            if (isNewValue == false)
            {
                return RedirectToPage(new
                {
                    ProjectTitle,
                    FutureFeaturesLoaded = FutureFeaturesLoaded,
                    ChallengesLoaded = ChallengesLoaded,
                    ValueFound = true,
                    EnteredValue = EnteredValue
                });
            }

            ProjectToChange = _db.OverrideDictionaryInProject(ProjectToChange, DictionaryToChange, FutureFeaturesLoaded, ChallengesLoaded);

            _db.SaveProject(ProjectToChange, AllProjects);

            return RedirectToPage(new
            {
                ProjectTitle,
                FutureFeaturesLoaded = FutureFeaturesLoaded,
                ChallengesLoaded = ChallengesLoaded
            });
        }
        public async Task<IActionResult> OnPostRemoveValue(string value)
        {
            AllProjects = await _db.ReadAllProjectRecordsAsync(_MainGoal);

            (ProjectToChange, DictionaryToChange) = _db.LoadProjectDetails(ProjectTitle, AllProjects, FutureFeaturesLoaded, ChallengesLoaded, _db);

            foreach (var item in DictionaryToChange)
            {
                foreach (var stringValue in item.Value)
                {
                    if (value == stringValue)
                    {
                        item.Value.Remove(value);
                    }

                    if (item.Value.Count == 0)
                    {
                        break;
                    }
                }
            }

            ProjectToChange = _db.OverrideDictionaryInProject(ProjectToChange, DictionaryToChange, FutureFeaturesLoaded, ChallengesLoaded);
            _db.SaveProject(ProjectToChange, AllProjects);

            return RedirectToPage(new
            {
                ProjectTitle,
                FutureFeaturesLoaded = FutureFeaturesLoaded,
                ChallengesLoaded = ChallengesLoaded
            });
        }
        public IActionResult OnPostGoBackToProjectManagement()
        {
            return RedirectToPage("ProjectManagementPage");
        }
        private bool IsValueDouble(string key)
        {
            bool output = false;

            foreach (var dictionaryItem in DictionaryToChange)
            {
                if (dictionaryItem.Key.ToLower() == key.ToLower())
                {
                    foreach (string value in dictionaryItem.Value)
                    {
                        if (value.ToLower() == EnteredValue.ToLower())
                        {
                            output = true;
                            break;
                        }
                    }
                }
            }

            return output;
        }
        private void LoadTheRightPageValues()
        {

            if (FutureFeaturesLoaded == true)
            {
                LoadFutureValues();
            }

            if (ChallengesLoaded == true)
            {
                LoadChallengesValues();
            }
        }
        private void LoadFutureValues()
        {
            KeyTableHead = "Additions";
            ValueTableHead = "Runway items";
            KeyItemPlaceholderText = "Addition";
            ValueItemPlaceHolderText = "Runway suggestion";
            FutureFeaturesLoaded = true;
            Mode = "Additions";
        }
        private void LoadChallengesValues()
        {
            KeyTableHead = "Challenges";
            ValueTableHead = "Solutions";
            KeyItemPlaceholderText = "Challenge";
            ValueItemPlaceHolderText = "Solution suggestion";
            ChallengesLoaded = true;
            Mode = "Challenges";
        }
        private bool IsKeyDouble()
        {
            bool output = false;

            foreach (var key in DictionaryToChange)
            {
                if (key.Key.ToLower() == NewKeyValue.ToLower())
                {
                    output = true;
                }
            }
            return output;
        }
        private void GetValuesFromButton(string item)
        {
            string[] passedValues = item.Split(Delimiter);

            ProjectTitle = passedValues[0];

            if (passedValues[1] == "Challenges")
            {
                LoadChallengesValues();
            }
            if (passedValues[1] == "Additions")
            {
                LoadFutureValues();
            }
            if (passedValues.Count() == 3)
            {
                OldKey = passedValues[2];
            }

        }
    }
}
