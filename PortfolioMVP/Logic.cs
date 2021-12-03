using Microsoft.Extensions.Configuration;
using PortfolioMVP.Models;
using ProjectProgressLibrary;
using ProjectProgressLibrary.DataAccess;
using ProjectProgressLibrary.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static PortfolioMVP.Enums.Enums;
using static ProjectProgressLibrary.Enums;
using static ProjectProgressLibrary.ApplicationLogic;

namespace PortfolioMVP
{
    public static class Logic
    {
        public static (IDataAccess database, string mainGoal) GetDbConfig(IConfiguration config, IDataAccess db, string pageName)
        {            
            string currentDirectory = Directory.GetCurrentDirectory();

            bool isUbuntu = false;
            bool isWindows = false;

            string projectFilePath = "";
            string timeUnitFilePath = "";

            string databseFolderPath = "";
            string projectPicturesFolderPath = "";
            string filesFolderPath = "";
            string picturesFolderPath = "";
            string randomItemsFolderPath = "";


            // Get all base settings from json file
            string connectionType = GetStringValueFromConfig(config, "DataStorageType", "Current");
            string environment = GetStringValueFromConfig(config, "Environment", "Current");
            string platform = GetStringValueFromConfig(config, "Platform", "Current");
            string mainGoal = GetStringValueFromConfig(config, "MainProjectGoal", "Current");

            // Get all locations from json file
            IDataAccess outputDb;

            if (environment.ToLower() == "demo")
            {
                mainGoal = "Demoing the app";
            }
            if (environment.ToLower() == "development")
            {
                mainGoal = "!!!! DEVELOPMENT ENVIRONMENT !!!!";
            }

            (isUbuntu, isWindows) = DecidePlatform(platform);

            if (connectionType.ToLower() == "csv" && mainGoal != "!!!! DEVELOPMENT ENVIRONMENT !!!!")
            {

                databseFolderPath = GetCSVFolderLocation("DatabaseFolderPath", currentDirectory, config, isUbuntu, isWindows);
                
                projectFilePath = $"{ databseFolderPath }{ mainGoal }{ environment }ProjectsFile.CSV";
                timeUnitFilePath = $"{ databseFolderPath }{ mainGoal }{ environment }TimeUnitsFile.CSV";

                projectPicturesFolderPath = GetFolderLocation("ProjectPicturesFolderPath", currentDirectory, config, isUbuntu, isWindows);
                filesFolderPath = GetFolderLocation("FilesFolderPath", currentDirectory, config, isUbuntu, isWindows);
                picturesFolderPath = GetFolderLocation("PicturesFolder", currentDirectory, config, isUbuntu, isWindows);
                randomItemsFolderPath = GetFolderLocation("RandomItemsFolder", currentDirectory, config, isUbuntu, isWindows);

            }

            if (connectionType == "CSV" && mainGoal == "!!!! DEVELOPMENT ENVIRONMENT !!!!")
            {
                databseFolderPath = GetCSVFolderLocation("DatabaseFolderPath", currentDirectory, config, isUbuntu, isWindows);
                projectFilePath = @$"{ databseFolderPath }DevelopmentFolder\{ mainGoal }{ environment }ProjectsFile.CSV";
                timeUnitFilePath = @$"{ databseFolderPath }DevelopmentFolder\{ mainGoal }{ environment }TimeUnitsFile.CSV";

                projectPicturesFolderPath = GetDevelopmentFolderLocation("ProjectPicturesFolderPath", currentDirectory, config);
                filesFolderPath = GetDevelopmentFolderLocation("FilesFolderPath", currentDirectory, config);
                picturesFolderPath = GetDevelopmentFolderLocation("PicturesFolder", currentDirectory, config);
                randomItemsFolderPath = GetDevelopmentFolderLocation("RandomItemsFolder", currentDirectory, config);

            }
            if (db is CSVDataAccess)
            {
                outputDb = new CSVDataAccess(projectFilePath, timeUnitFilePath, databseFolderPath, projectPicturesFolderPath, filesFolderPath, picturesFolderPath, randomItemsFolderPath);
            }
            else
            {
                outputDb = db;
            }

            if (pageName.ToLower() == "index" && environment.ToLower() == "demo")
            {
                outputDb.MakeFirstEntry("Demoing the app");
            }


            return (outputDb, mainGoal);
        }

        private static (bool isUbuntu, bool isWindows) DecidePlatform(string platform)
        {
            bool isUbuntu = false;
            bool isWindows = false;

            if (platform.ToLower().Contains("ubuntu"))
            {                
                isUbuntu = true;
            }

            if (platform.ToLower().Contains("windows"))
            {
                isWindows = true;
            }


            return (isUbuntu, isWindows);

        }

        private static string GetCSVFolderLocation(string value, string currentDirectory, IConfiguration config, bool isUbuntu, bool isWindows)
        {
            string outputLocation = "";
            
            string mainSection = "DataAccess";
            string subSection = "CSV";
            string finalFolderPath = GetStringValueFromConfig(mainSection, subSection, value, config);

            if (isUbuntu == true)
            {
                outputLocation = finalFolderPath;
            }

            if (isWindows == true)
            {
                string placementFolderPath = @"\wwwroot\";

                outputLocation = $"{ currentDirectory }{ placementFolderPath }{ finalFolderPath }";
            }


            return outputLocation;
        }

        private static string GetFolderLocation(string value, string currentDirectory, IConfiguration config, bool isUbuntu, bool isWindows)
        {
            string outputLocation = "";            
            string mainSection = "DataAccess";
            string subSection = "Folders";
            string finalFolderPath = GetStringValueFromConfig(mainSection, subSection, value, config);

            if (isUbuntu == true)
            {
                outputLocation = finalFolderPath;
            }

            if (isWindows == true)
            {
                string placementFolderPath = @"\wwwroot\";

                outputLocation = $"{ currentDirectory }{ placementFolderPath }{ finalFolderPath }";
            }


            return outputLocation;

        }
        private static string GetDevelopmentFolderLocation(string value, string currentDirectory, IConfiguration config)
        {           
            string mainSection = "DataAccess";
            string subSection = "Folders";

            string developmentFolderPath = @"DevelopmentFolder\";

            string placementFolderPath = @"\wwwroot\";

            string finalFolderPath = GetStringValueFromConfig(mainSection, subSection, value, config);

            string location = $"{ currentDirectory }{ placementFolderPath }{ finalFolderPath }{ developmentFolderPath }";

            return location;

        }
        private static string GetStringValueFromConfig(IConfiguration config, string section, string value)
        {
            string output = config.GetSection(section).GetValue<string>(value);

            return output;
        }
        private static string GetStringValueFromConfig(string mainSection, string subSection, string value, IConfiguration config)
        {
            string output = config.GetSection(mainSection).GetSection(subSection).GetValue<string>(value);

            return output;
        }
        public static Guid ConvertStringToGuid(string stringGuid)
        {
            Guid outputGuid = Guid.NewGuid();

            bool isGuid = stringGuid.ValidateGuid();
            if (isGuid == true)
            {
                outputGuid = new Guid(stringGuid);
            }

            return outputGuid;
        }
        public static List<Guid> CreateGuidListForSubProjects(ProjectModel projectToConvert, List<ProjectModel> allProjects, IDataAccess db)
        {
            List<Guid> outputList = new List<Guid>();
            if (projectToConvert.SubProjectIds.Count > 0)
            {
                foreach (var subProjectId in projectToConvert.SubProjectIds)
                {
                    Guid guidToAdd = ConvertStringToGuid(subProjectId.ToString());
                    ProjectModel subProjectToAdd = db.GetProjectById(guidToAdd, allProjects);
                    if (subProjectToAdd.ShowItem == true)
                    {
                        outputList.Add(guidToAdd);
                    }
                }
            }

            return outputList;
        }
        public static string GetMainProjectTitle(ProjectModel projectToConvert, List<ProjectModel> allProjects, IDataAccess db)
        {
            string outputProjectTitle = null;

            if (projectToConvert.MainProjectId != null)
            {
                Guid mainProjectId = ConvertStringToGuid(projectToConvert.MainProjectId.ToString());
                ProjectModel mainProject = db.GetProjectById(mainProjectId, allProjects);

                outputProjectTitle = mainProject.Title;
            }

            return outputProjectTitle;
        }
        public static PresentaionProjectModel CreatePresentationProject(ProjectModel projectToConvert, List<ProjectModel> allProjects, IDataAccess db)
        {
            PresentaionProjectModel outputModel = new PresentaionProjectModel();

            // Set title
            outputModel.ProjectTitle = projectToConvert.Title;

            // Set MainProjectTitle
            outputModel.MainProjectTitle = GetMainProjectTitle(projectToConvert, allProjects, db);

            // Set priority number
            outputModel.Priority = projectToConvert.PriorityNumber;

            // Set MainProjectId
            if (projectToConvert.MainProjectId != null)
            {
                outputModel.MainProjectId = new Guid(projectToConvert.MainProjectId.ToString());
            }
            // Set project status
            outputModel.ProjectStatus = projectToConvert.ProjectStatus;

            // Set show progress boolean
            outputModel.ShowProgressBar = projectToConvert.ShowProgressBar;

            // Set progress percentage
            outputModel.ProgressPercentage = projectToConvert.Progress;

            // Set project id
            outputModel.ProjectId = projectToConvert.ProjectId;

            // Set the outcome
            outputModel.Outcome = projectToConvert.Outcome;

            // Set the description
            outputModel.ShortDescription = projectToConvert.ShortDescription;

            // Get all future additions
            outputModel.FutureAdditions = GetAllKeysFromDictionary(projectToConvert.FutureAdditions);

            // Get all challenges
            outputModel.Challenges = GetAllKeysFromDictionary(projectToConvert.Challenges);

            // Create a developernames list
            outputModel.DeveloperNames = CreateDeveloperNamesList(projectToConvert.DeveloperName);

            // Create a list of presentationSubprojectModels
            outputModel.SubprojectPresentationModels = GetPresentationSubProjectList(projectToConvert, allProjects, db);



            return outputModel;

        }

        private static List<string> GetAllKeysFromDictionary(Dictionary<string, List<string>> dictionaryCollection)
        {
            List<string> outputList = new List<string>();
            foreach (var dictionary in dictionaryCollection)
            {
                outputList.Add(dictionary.Key);
            }

            return outputList;
        }

        private static List<PresentationSubProjectModel> GetPresentationSubProjectList(ProjectModel projectToConvert, List<ProjectModel> allProjects, IDataAccess db)
        {
            List<PresentationSubProjectModel> outputPresentationSubProjectModels = new List<PresentationSubProjectModel>();

            // Get Guids from sub project list
            List<Guid> subProjectIds = CreateGuidListForSubProjects(projectToConvert, allProjects, db);

            // Make PresentationSubProjectModels from subProject is
            outputPresentationSubProjectModels = CreatePresentationSubProjectList(subProjectIds, allProjects, db);

            return outputPresentationSubProjectModels;
        }
        private static List<PresentationSubProjectModel> CreatePresentationSubProjectList(List<Guid> subProjectIds, List<ProjectModel> allProjects, IDataAccess db)
        {
            List<PresentationSubProjectModel> outputList = new List<PresentationSubProjectModel>();


            if (subProjectIds.Count > 0)
            {
                foreach (Guid subProjectId in subProjectIds)
                {
                    // Get the project details
                    ProjectModel subProjectToAdd = db.GetProjectById(subProjectId, allProjects);

                    // Create presentation model from project model to add
                    PresentationSubProjectModel modelToAdd = new PresentationSubProjectModel();
                    modelToAdd.Title = subProjectToAdd.Title;
                    modelToAdd.Priority = subProjectToAdd.PriorityNumber;
                    modelToAdd.ShowProgressBar = subProjectToAdd.ShowProgressBar;
                    modelToAdd.ProgressPercentage = subProjectToAdd.Progress;

                    outputList.Add(modelToAdd);
                }
            }

            return outputList;
        }
        public static List<TimeUnitModel> GetAllRelevantTimeUnits(ProjectModel mainProject, List<TimeUnitModel> allTimeUnits, List<ProjectModel> allProjects, IDataAccess db)
        {
            // Create a list of relevant project id's           
            List<Guid> relevantProjectIds = CreateRelevantGuidList(mainProject, allProjects, db);

            List<TimeUnitModel> allRelevantTimeUnits = GetTimeUnits(relevantProjectIds, allTimeUnits);

            return allRelevantTimeUnits;
        }
        private static List<TimeUnitModel> GetTimeUnits(List<Guid> relevantProjectIds, List<TimeUnitModel> allTimeUnits)
        {
            // Set up list to output
            List<TimeUnitModel> outputList = new List<TimeUnitModel>();

            foreach (Guid guid in relevantProjectIds)
            {
                // Get the timeunits for the project
                List<TimeUnitModel> projectTimeUnits = allTimeUnits.Where(x => x.ProjectId == guid).ToList();

                foreach (TimeUnitModel timeUnitModel in projectTimeUnits)
                {
                    outputList.Add(timeUnitModel);
                }

            }

            return outputList;
        }
        private static List<Guid> CreateRelevantGuidList(ProjectModel mainProject, List<ProjectModel> allProjects, IDataAccess db)
        {
            List<Guid> relevantProjectIds = new List<Guid>();

            // if the item can be shown
            if (mainProject.ShowItem == true)
            {
                // Create a list of guids for the next level down
                List<Guid> nextLevelGuids = new List<Guid>();

                // Create a list of sub project guids
                List<Guid> subProjectIds = new List<Guid>();

                // if the item can be shown get the subproject ids to the list
                subProjectIds = CreateGuidListForSubProjects(mainProject, allProjects, db);

                // Add base ids
                relevantProjectIds = GetBaseIds(mainProject.ProjectId, subProjectIds);

                // Create exit bool for loop
                bool moreSubprojects = true;

                // Create loop counter
                int counter = 0;

                do
                {
                    // Get all the next level down guids
                    nextLevelGuids = GetAllSubSubProjectids(subProjectIds, allProjects, db);

                    // Check for availability of more sub projects
                    moreSubprojects = CheckForSubprojects(nextLevelGuids);


                    subProjectIds = nextLevelGuids;

                    foreach (Guid guid in nextLevelGuids)
                    {
                        relevantProjectIds.Add(guid);
                    }

                    // add one to loop counter
                    counter = counter + 1;

                    CheckForInfiniteLoop(counter);

                } while (moreSubprojects == true);
            }

            return relevantProjectIds;
        }
        private static List<Guid> GetBaseIds(Guid mainProjectId, List<Guid> subProjectIds)
        {
            List<Guid> outputList = new List<Guid>();

            outputList.Add(mainProjectId);

            foreach (Guid guid in subProjectIds)
            {
                outputList.Add(guid);
            }

            return outputList;
        }
        private static void CheckForInfiniteLoop(int counter)
        {
            if (counter == 2000)
            {
                throw new Exception("there are 2000 subprojects in this catagory is that right?");
            }
        }
        private static bool CheckForSubprojects(List<Guid> nextLevelGuids)
        {
            bool output = true;


            if (nextLevelGuids.Count == 0)
            {
                output = false;
            }

            return output;
        }
        private static List<Guid> GetAllSubSubProjectids(List<Guid> subProjectIds, List<ProjectModel> allProjects, IDataAccess db)
        {
            List<Guid> outputList = new List<Guid>();

            foreach (Guid subProjectId in subProjectIds)
            {
                ProjectModel subProject = db.GetProjectById(subProjectId, allProjects);

                if (subProject.ShowItem == true && subProject.SubProjectIds.Count > 0)
                {
                    List<Guid> subSubProjectIds = CreateGuidListForSubProjects(subProject, allProjects, db);
                    foreach (Guid guid in subSubProjectIds)
                    {
                        outputList.Add(guid);
                    }
                }
            }

            return outputList;
        }
        //public static Dictionary<string, List<string>> LoadAllDictionaries(List<ProjectModel> allProjects, DictionaryClassification classification)
        public static List<SolutionModel> LoadAllDictionaries(List<ProjectModel> allProjects, DictionaryClassification classification)
        {
            List<SolutionModel> outputList = new List<SolutionModel>();

            foreach (ProjectModel project in allProjects)
            {
                List<SolutionModel> challengesSolutions = new List<SolutionModel>();
                List<SolutionModel> futureAdditionSolutions = new List<SolutionModel>();

                // Select the right dictionaries
                switch (classification)
                {
                    case DictionaryClassification.Challenges:
                        challengesSolutions = GetDictionaryItems(project.Challenges);
                        break;
                    case DictionaryClassification.FutureAdditions:
                        futureAdditionSolutions = GetDictionaryItems(project.FutureAdditions);
                        break;
                    case DictionaryClassification.All:
                        challengesSolutions = GetDictionaryItems(project.Challenges);
                        futureAdditionSolutions = GetDictionaryItems(project.FutureAdditions);
                        break;
                    default:
                        break;
                }


                // if there is a item in the dictionary add it to the list to return
                if (challengesSolutions.Count > 0)
                {
                    foreach (SolutionModel solution in challengesSolutions)
                    {
                        outputList.Add(solution);
                    }
                }
                if (futureAdditionSolutions.Count > 0)
                {
                    foreach (SolutionModel solution in futureAdditionSolutions)
                    {
                        outputList.Add(solution);
                    }

                }
            }

            return outputList;
        }

        private static List<SolutionModel> GetDictionaryItems(Dictionary<string, List<string>> dictionaryToConvert)
        {
            List<SolutionModel> outputList = new List<SolutionModel>();

            foreach (var item in dictionaryToConvert)
            {
                SolutionModel solutionToAdd = new SolutionModel();
                solutionToAdd.Key = item.Key;
                solutionToAdd.Values = item.Value;


                outputList.Add(solutionToAdd);
            }

            return outputList;
        }

        public static List<ProjectModel> FillProjectList(ProjectStatus status, List<ProjectModel> allProjects)
        {
            List<ProjectModel> outputList = new List<ProjectModel>();

            // Remove all projects with more then one subproject
            outputList = allProjects.Where(x => x.SubProjectIds.Count == 0).ToList();

            // Select items per status
            switch (status)
            {
                case ProjectStatus.ToDo:
                    outputList = outputList.Where(x => x.ProjectStatus == ProjectStatus.ToDo).ToList();
                    outputList = outputList.OrderByDescending(x => x.DateCreated).ToList();
                    break;
                case ProjectStatus.Doing:
                    outputList = outputList.Where(x => x.ProjectStatus == ProjectStatus.Doing).ToList();
                    outputList = outputList.OrderByDescending(x => x.DateDoing).ToList();
                    break;
                case ProjectStatus.Done:
                    outputList = outputList.Where(x => x.ProjectStatus == ProjectStatus.Done).ToList();
                    outputList = outputList.OrderByDescending(x => x.DateDone).ToList();
                    break;
                default:
                    break;
            }
            return outputList;
        }
        public static List<ProjectModel> FilterListsByDate(List<ProjectModel> projectList, DateTime selectionDate, ProjectStatus status)
        {
            List<ProjectModel> outputList = new List<ProjectModel>();

            switch (status)
            {
                case ProjectStatus.ToDo:
                    // Create list of ToDo projects
                    outputList = projectList.Where(x => x.DateCreated >= selectionDate).ToList();
                    // Order by date Created
                    outputList = outputList.OrderBy(x => x.DateCreated).ToList();
                    break;
                case ProjectStatus.Doing:
                    // Create list of Doing projects
                    outputList = projectList.Where(x => x.DateDoing >= selectionDate).ToList();
                    // Order by date Doing
                    outputList = outputList.OrderBy(x => x.DateDoing).ToList();
                    break;
                case ProjectStatus.Done:
                    // Create list of Done projects
                    outputList = projectList.Where(x => x.DateDone >= selectionDate).ToList();
                    // Order by date Done
                    outputList = outputList.OrderBy(x => x.DateDone).ToList();
                    break;
                default:
                    break;
            }


            return outputList;
        }
        public static List<PresentationDemoModel> CreateAllDemoModels(List<ProjectModel> allProjects)
        {
            List<PresentationDemoModel> outputDemoModelList = new List<PresentationDemoModel>();

            List<ProjectModel> allDemoProjects = allProjects.Where(x => x.HasDemo == true).ToList();

            foreach (ProjectModel project in allDemoProjects)
            {
                if (project.ShowItem == true)
                {
                    PresentationDemoModel modelToAdd = new PresentationDemoModel();
                    modelToAdd.Title = project.Title;
                    modelToAdd.DesiredOutcome = project.Outcome;
                    modelToAdd.ShortDescription = project.ShortDescription;
                    modelToAdd.ProjectLink = project.DemoLink;
                    modelToAdd.GitHubLink = project.GitHubLink;
                    modelToAdd.TotalHours = project.TotalHours;
                    modelToAdd.DeveloperNames = CreateDeveloperNamesList(project.DeveloperName);

                    outputDemoModelList.Add(modelToAdd);
                }
            }

            return outputDemoModelList;
        }

        private static List<string> CreateDeveloperNamesList(string developerName)
        {

            string delimiter = @", ";
            string[] developers = developerName.Split(delimiter);
            List<string> outputList = developers.ToList();

            return outputList;
        }

        internal static string GetProjectPhotosFolderPath(IConfiguration config)
        {
            string outputFilePath = "";
            bool isUbuntu = false;
            bool isWindows = false;


            string platform = GetStringValueFromConfig(config, "Platform", "Current");
            (isUbuntu, isWindows) = DecidePlatform(platform);


            string currentDirectory = Directory.GetCurrentDirectory();

            outputFilePath = GetFolderLocation("ProjectPicturesFolderPath", currentDirectory, config, isUbuntu, isWindows);


            return outputFilePath;
        }

        public static (string ProjectPictureFilePath, bool ShowPicture) SetUpPictureShowing(string title, IDataAccess db, string rootFolderPath)
        {
            bool showPicture = false;


            string pictureName = db.ExchangePunctuations(title);
            string projectPhotoName = $"{ pictureName }.jpg";

            string fullFilePath = rootFolderPath + projectPhotoName;

            showPicture = DecideToShowPicture(fullFilePath);

            // Do not put in a full file path start from rootwww 
            string outputFilepath = @$"project_photos\{ projectPhotoName }"; ;

            return (outputFilepath, showPicture);
        }


        private static bool DecideToShowPicture(string projectPictureFilePath)
        {
            bool showPicture = false;

            if (System.IO.File.Exists($"{ projectPictureFilePath }") == true)
            {
                showPicture = true;
            }

            return showPicture;
        }



        internal static string DecideDayOfWeek(DateTime searchDate)
        {
            string outputDay = "";
            switch (searchDate.DayOfWeek)
            {
                case System.DayOfWeek.Sunday:
                    outputDay = "Zondag";
                    break;
                case System.DayOfWeek.Monday:
                    outputDay = "Maandag";
                    break;
                case System.DayOfWeek.Tuesday:
                    outputDay = "Dinsdag";
                    break;
                case System.DayOfWeek.Wednesday:
                    outputDay = "Woensdag";
                    break;
                case System.DayOfWeek.Thursday:
                    outputDay = "Donderdag";
                    break;
                case System.DayOfWeek.Friday:
                    outputDay = "Vrijdag";
                    break;
                case System.DayOfWeek.Saturday:
                    outputDay = "Zaterdag";
                    break;
                default:
                    break;
            }

            return outputDay;
        }

        internal static DateTime GetDefaultDate()
        {
            string defaultDate = "13/05/2020 00:00:00 AM";

            DateTime outputDate = CreateDateFromString(defaultDate);

            return outputDate;
        }

    }
}
