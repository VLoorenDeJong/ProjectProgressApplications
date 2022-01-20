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
using static ProjectProgressLibrary.Validation.DataValidation;

namespace ProgressApplicationMVP.Pages
{
    public class DictionaryValuesManagementPageModel : PageModel
    {
        // Front end
        [BindProperty(SupportsGet = true)]
        public bool IsNewValue { get; set; } = true;
        [BindProperty(SupportsGet = true)]
        public bool FutureFeaturesLoaded { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool ChallengesLoaded { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool EditingEnabled { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool IsEditedItemNew { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Key { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Mode { get; set; }
        [BindProperty]
        public string ValueItemPlaceHolderText { get; set; }
        [BindProperty]
        public string ValueTableHead { get; set; }
        [BindProperty(SupportsGet = true)]
        public string ValueToAdd { get; set; }
        [BindProperty]
        public string StringToRemove { get; set; }


        [BindProperty(SupportsGet = true)]
        public string ItemTitle { get; set; }
        [BindProperty(SupportsGet = true)]
        public string NewValue { get; set; }
        [BindProperty]
        public List<string> ItemValuesList { get; set; } = new List<string>();

        // Back end
        private readonly IDataAccess _db;
        private readonly ILogger<DictionaryValuesManagementPageModel> _logger;
        private readonly string _MainGoal;
        private List<ProjectModel> AllProjects = new List<ProjectModel>();
        private ProjectModel ProjectToChange { get; set; }
        [BindProperty(SupportsGet = true)]
        public Dictionary<string, List<string>> DictionaryToChange { get; private set; } = new Dictionary<string, List<string>>();
        [BindProperty(SupportsGet = true)]
        public string OldValue { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ProjectTitle { get; set; }

        public DictionaryValuesManagementPageModel(ILogger<DictionaryValuesManagementPageModel> logger, IDataAccess db, IConfiguration config)
        {
            (_db, _MainGoal) = GetDbConfig(config, db, "dictionaryManagement");

            AllProjects = _db.ReadAllProjectRecords(_MainGoal);
        }
        public void OnGet()
        {
            (ProjectToChange, DictionaryToChange) = _db.LoadProjectDetails(ProjectTitle, AllProjects, FutureFeaturesLoaded, ChallengesLoaded, _db);

            ItemValuesList = _db.MakeListFromDictionaryItemValues(DictionaryToChange, ItemTitle);

            LoadTheRightPageValues();
        }
        public IActionResult OnPostBackToItem(string item)
        {
            LoadTheRightPageValues();

            //GetValuesFromButton(item);

            return RedirectToPage("DictionaryManagement", new
            {
                SearchEnabled = true,
                ProjectTitle = ProjectTitle,
                Mode = Mode,
                FromValuePage = true
            });
        }
        public IActionResult OnPostBackToProjectManagement()
        {
            return RedirectToPage("ProjectManagementPage");
        }
        public IActionResult OnPostAddValue(string item)
        {
            // Get the send values from page en make the right page settings
            //GetValuesFromButton(item);
            LoadTheRightPageValues();

            // Load the project and the correct dictionary
            (ProjectToChange, DictionaryToChange) = _db.LoadProjectDetails(ProjectTitle, AllProjects, FutureFeaturesLoaded, ChallengesLoaded, _db);


            // Check the entered value for content
            bool itHasContent = ValueToAdd.ValidateStringHasContent();

            if (itHasContent == true)
            {
                // Extract the dictionary item values to a list
                ItemValuesList = _db.MakeListFromDictionaryItemValues(DictionaryToChange, ItemTitle);

                // Check for entry in list
                IsNewValue = false;
                IsNewValue = IsItANewValue(ValueToAdd, ItemValuesList);


                if (IsNewValue == true)
                {
                    string stringToRemove = $"No { ValueItemPlaceHolderText.ToLower() } yet.";
                    string firstListValue = ItemValuesList.First();
                    // Remove default entry
                    if (firstListValue == stringToRemove)
                    {
                        ItemValuesList = new List<string>();
                        // No solution yet.
                        DictionaryToChange.Where(x => x.Key.ToLower() == ItemTitle.ToLower()).First().Value.Remove(stringToRemove);
                    }


                    // Add new value to dictionary
                    DictionaryToChange.Where(x => x.Key.ToLower() == ItemTitle.ToLower()).First().Value.Add(ValueToAdd);


                    // Put the update dictionary in the project
                    ProjectToChange = _db.OverrideDictionaryInProject(ProjectToChange, DictionaryToChange, FutureFeaturesLoaded, ChallengesLoaded);
                                      

                    // save the project
                    _db.SaveProject(ProjectToChange, AllProjects);
                }

            }

            if (IsNewValue == false)
            {
                return RedirectToPage(new
                {
                    ValueToAdd = ValueToAdd,
                    ProjectTitle = ProjectTitle,
                    FutureFeaturesLoaded = FutureFeaturesLoaded,
                    ChallengesLoaded = ChallengesLoaded,
                    Mode = Mode,
                    ItemTitle = ItemTitle,
                    IsNewValue = IsNewValue
                });

            }

            return RedirectToPage(new
            {
                ProjectTitle = ProjectTitle,
                FutureFeaturesLoaded = FutureFeaturesLoaded,
                ChallengesLoaded = ChallengesLoaded,
                Mode = Mode,
                ItemTitle = ItemTitle
            });
        }
        public IActionResult OnPostEditValue(string item)
        {
            // Get the send values from page en make the right page settings
            //GetValuesFromButton(item);
            LoadTheRightPageValues();

            // Load the project and the correct dictionary
            (ProjectToChange, DictionaryToChange) = _db.LoadProjectDetails(ProjectTitle, AllProjects, FutureFeaturesLoaded, ChallengesLoaded, _db);



            return RedirectToPage(new
            {
                ProjectTitle = ProjectTitle,
                FutureFeaturesLoaded = FutureFeaturesLoaded,
                ChallengesLoaded = ChallengesLoaded,
                Mode = Mode,
                ItemTitle = ItemTitle,
                EditingEnabled = true,
                OldValue = OldValue,
                IsEditedItemNew = true

            }); ;
        }
        public IActionResult OnPostSaveEditedValue()
        {

            LoadTheRightPageValues();
            // Load the project and the correct dictionary
            (ProjectToChange, DictionaryToChange) = _db.LoadProjectDetails(ProjectTitle, AllProjects, FutureFeaturesLoaded, ChallengesLoaded, _db);

            bool itHasContent = NewValue.ValidateStringHasContent();
            if (itHasContent == true)
            {

                // Extract the dictionary item values to a list
                ItemValuesList = _db.MakeListFromDictionaryItemValues(DictionaryToChange, ItemTitle);

                // Check for entry in list
                IsNewValue = false;
                IsNewValue = IsItANewValue(NewValue, ItemValuesList);
                if (IsNewValue == true)
                {

                    // Load the page values
                    LoadTheRightPageValues();

                    // Remove old value
                    DictionaryToChange.Where(x => x.Key.ToLower() == ItemTitle.ToLower()).First().Value.Remove(OldValue);

                    // Add new value
                    DictionaryToChange.Where(x => x.Key.ToLower() == ItemTitle.ToLower()).First().Value.Add(NewValue);

                    // Put the updated dictionary in the project
                    ProjectToChange = _db.OverrideDictionaryInProject(ProjectToChange, DictionaryToChange, FutureFeaturesLoaded, ChallengesLoaded);

                    // save the project
                    _db.SaveProject(ProjectToChange, AllProjects);
                }
            }
            return RedirectToPage(new
            {
                ProjectTitle = ProjectTitle,
                FutureFeaturesLoaded = FutureFeaturesLoaded,
                ChallengesLoaded = ChallengesLoaded,
                Mode = Mode,
                ItemTitle = ItemTitle,
                IsNewValue = IsNewValue
            });
        }
        public IActionResult OnPostRemoveValue(string item)
        {
            // Get the send values from page en make the right page settings
            //GetValuesFromButton(item);
            LoadTheRightPageValues();

            // Load the project and the correct dictionary
            (ProjectToChange, DictionaryToChange) = _db.LoadProjectDetails(ProjectTitle, AllProjects, FutureFeaturesLoaded, ChallengesLoaded, _db);

            // Remove value from dictionary
            DictionaryToChange.Where(x => x.Key.ToLower() == ItemTitle.ToLower()).First().Value.Remove(OldValue);

            // Add default value to dictionary
            if (DictionaryToChange.Where(x => x.Key.ToLower() == ItemTitle.ToLower()).First().Value.Count == 0)
            {
                string stringToAdd = $"No { ValueItemPlaceHolderText.ToLower() } yet.";
                DictionaryToChange.Where(x => x.Key.ToLower() == ItemTitle.ToLower()).First().Value.Add(stringToAdd);
            }

            //string stringToAdd = $"No { ValueItemPlaceHolderText.ToLower() } yet.";
            //DictionaryToChange.Where(x => x.Key.ToLower() == ItemTitle.ToLower()).First().Value.Add(stringToAdd);



            // Put the update dictionary in the project
            ProjectToChange = _db.OverrideDictionaryInProject(ProjectToChange, DictionaryToChange, FutureFeaturesLoaded, ChallengesLoaded);

            // save the project
            _db.SaveProject(ProjectToChange, AllProjects);



            return RedirectToPage(new
            {
                ProjectTitle = ProjectTitle,
                FutureFeaturesLoaded = FutureFeaturesLoaded,
                ChallengesLoaded = ChallengesLoaded,
                Mode = Mode,
                ItemTitle = ItemTitle
            });
        }
        private bool IsItANewValue(string valueToAdd, List<string> itemValuesList)
        {
            bool output = true;

            foreach (string value in itemValuesList)
            {
                // Case sensitive

                if (valueToAdd == value)
                {
                    output = false;
                }
                // Case Insensitive

                //if (valueToAdd.ToLower() == value.ToLower())
                //{
                //    output = false;
                //}
            }

            return output;
        }
        private void LoadTheRightPageValues()
        {

            if (Mode == "Future")
            {
                LoadFutureValues();
            }

            if (Mode == "Challenges")
            {
                LoadChallengesValues();
            }
        }
        private void LoadFutureValues()
        {
            ValueTableHead = "Runway items";
            ValueItemPlaceHolderText = "Runway suggestion";
            FutureFeaturesLoaded = true;
            Mode = "Future";
        }
        private void LoadChallengesValues()
        {
            ValueTableHead = "Solutions";
            ValueItemPlaceHolderText = "Solution suggestion";
            ChallengesLoaded = true;
            Mode = "Challenges";
        }
    }
}
