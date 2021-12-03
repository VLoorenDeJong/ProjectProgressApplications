using Microsoft.Extensions.Configuration;
using ProjectProgressLibrary.DataAccess;
using ProjectProgressLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectProgressWebFront1
{
    public static class Logic
    {
        public static (IDataAccess database, string mainGoal) GetDbConfig(IConfiguration config, IDataAccess db, string pageName)
        {
            string backendProjectFilePath = "";
            string backendTimeUnitFilePath = "";
            string backendDatabaseFolderPath = "";

            string frondendProjectFilePath = "";
            string frontendTimeUnitFilePath = "";
            string frontEndDatabaseFolderPath = "";

            string backupProjectFilePath = "";
            string backupTimeUnitFilePath = "";
            string backupDatabaseFolderPath = "";

            // Get base setting from json file
            string connectionType = GetStringValueFromConfig(config, "DataStorageType", "Current");
            string environment = GetStringValueFromConfig(config, "Environment", "Current"); 
            string mainGoal = GetStringValueFromConfig(config, "MainProjectGoal", "Current");

            // Get pictureFolder path from json file
            string backendendPhotoFolderPath = GetPhotoFolderLocation("Backend", config);
            string frontendPhotoFolderPath = GetPhotoFolderLocation("Frontend", config);
            string backupPhotoFolderPath = GetPhotoFolderLocation("Backup", config);

            IDataAccess outputDb;

            if (environment.ToLower() == "demo")
            {
                mainGoal = "Demoing the app";
            }
            if (environment.ToLower() == "development")
            {
                mainGoal = "!!!! DEVELOPMENT ENVIRONMENT !!!!";
            }

            if (connectionType == "CSV" && mainGoal != "Demoing the app" && mainGoal != "!!!! DEVELOPMENT ENVIRONMENT !!!!")
            {
                backendDatabaseFolderPath = GetDatabaseLocation("Backend", config);
                backendProjectFilePath = $"{ backendDatabaseFolderPath }{ mainGoal }{ environment }ProjectsFile.CSV";
                backendTimeUnitFilePath = $"{ backendDatabaseFolderPath }{ mainGoal }{ environment }TimeUnitsFile.CSV";


                frontEndDatabaseFolderPath = GetDatabaseLocation("Frontend", config);
                if (string.IsNullOrEmpty(frontEndDatabaseFolderPath) == false)
                {
                    frondendProjectFilePath = $"{ frontEndDatabaseFolderPath }{ mainGoal }{ environment }ProjectsFile.CSV";
                    frontendTimeUnitFilePath = $"{ frontEndDatabaseFolderPath }{ mainGoal }{ environment }TimeUnitsFile.CSV";

                }

                backupDatabaseFolderPath = GetDatabaseLocation("Backup", config);
                if (string.IsNullOrEmpty(backupDatabaseFolderPath) == false)
                {
                    backupProjectFilePath = $"{ backupDatabaseFolderPath }{ mainGoal }{ environment }ProjectsFile.CSV";
                    backupTimeUnitFilePath = $"{ backupDatabaseFolderPath }{ mainGoal }{ environment }TimeUnitsFile.CSV";

                }
            }

            if (connectionType == "CSV" && mainGoal == "!!!! DEVELOPMENT ENVIRONMENT !!!!")
            {
                backendDatabaseFolderPath = GetDatabaseLocation("Backend", config);
                backendDatabaseFolderPath = @$"{backendDatabaseFolderPath}DevelopmentFolder\";
                backendendPhotoFolderPath = @$"{backendendPhotoFolderPath}DevelopmentFolder\";

                backendProjectFilePath = $"{ backendDatabaseFolderPath }{ mainGoal }{ environment }ProjectsFile.CSV";
                backendTimeUnitFilePath = $"{ backendDatabaseFolderPath }{ mainGoal }{ environment }TimeUnitsFile.CSV";


                frontEndDatabaseFolderPath = GetDatabaseLocation("Frontend", config);
                if (string.IsNullOrEmpty(frontEndDatabaseFolderPath) == false)
                {
                    frontEndDatabaseFolderPath = @$"{frontEndDatabaseFolderPath}DevelopmentFolder\";
                    frontendPhotoFolderPath = @$"{frontendPhotoFolderPath}DevelopmentFolder\";

                    frondendProjectFilePath = $"{ frontEndDatabaseFolderPath }{ mainGoal }{ environment }ProjectsFile.CSV";
                    frontendTimeUnitFilePath = $"{ frontEndDatabaseFolderPath }{ mainGoal }{ environment }TimeUnitsFile.CSV";
                }


                // Clear all not needed file paths
                backupProjectFilePath = "";
                backupTimeUnitFilePath = "";
                backupDatabaseFolderPath = "";
                backupPhotoFolderPath = "";

            }


            if (connectionType == "CSV" && mainGoal == "Demoing the app")
            {
                backendDatabaseFolderPath = GetDatabaseLocation("Backend", config);
                backendDatabaseFolderPath = @$"{backendDatabaseFolderPath}DemoFolder\";


                backendProjectFilePath = $"{ backendDatabaseFolderPath }{ mainGoal }{ environment }ProjectsFile.CSV";
                backendTimeUnitFilePath = $"{ backendDatabaseFolderPath }{ mainGoal }{ environment }TimeUnitsFile.CSV";

                // Clear all not needed file paths
                backendendPhotoFolderPath = "";

                frondendProjectFilePath = "";
                frontendTimeUnitFilePath = "";
                frontEndDatabaseFolderPath = "";
                frontendPhotoFolderPath = "";
                
                backupProjectFilePath = "";
                backupTimeUnitFilePath = "";
                backupDatabaseFolderPath = "";
                backupPhotoFolderPath = "";


            }
            if (db is CSVDataAccess)
            {
                outputDb = new CSVDataAccess(backendProjectFilePath, backendTimeUnitFilePath, backendDatabaseFolderPath, backendendPhotoFolderPath, frondendProjectFilePath, frontendTimeUnitFilePath, frontEndDatabaseFolderPath, frontendPhotoFolderPath, backupProjectFilePath, backupTimeUnitFilePath, backupDatabaseFolderPath, backupPhotoFolderPath);
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
        private static string GetDatabaseLocation(string endSelection , IConfiguration config )
        {
            string mainSection = "DataAccess";
            string value = "DatabaseFolderLocation";

            string output = GetStringValueFromConfig(mainSection, endSelection, value, config);

            return output;
        }

        private static string GetPhotoFolderLocation(string endSelection, IConfiguration config)
        {
            string mainSection = "DataAccess";
            string value = "PhotoFolderLocation";

            string output = GetStringValueFromConfig(mainSection, endSelection, value, config);

            return output;
        }
        private static string GetStringValueFromConfig(IConfiguration config, string section, string value)
        {
            string output = config.GetSection(section).GetValue<string>(value);

            return output;
        }

        private static string GetStringValueFromConfig(string mainSection, string subSection,  string value, IConfiguration config)
        {
            string output = config.GetSection(mainSection).GetSection(subSection).GetValue<string>(value);

            return output;
        }

        //ToDo look for changing this to data access
        public static Guid GetProjectIdByTitle(string projectTitle, List<ProjectModel> allprojects)
        {
            return allprojects.Where(x => x.Title == projectTitle).First().ProjectId;
        }

        internal static (ProjectModel projectToChange, Dictionary<string, List<string>> dictionaryToChange) LoadProjectDetails (string projectTitle, List<ProjectModel> allProjects, bool futureFeaturesLoaded, bool challengesLoaded, IDataAccess _db)
        {
            ProjectModel outputProject = new ProjectModel();
            Dictionary<string, List<string>> outputDictionary = new Dictionary<string, List<string>>();

            outputProject = _db.GetProjectByTitle(projectTitle, allProjects);

            if (futureFeaturesLoaded == true)
            {
                outputDictionary = outputProject.FutureAdditions;
            }

            if (challengesLoaded == true)
            {
                outputDictionary = outputProject.Challenges;
            }

            outputDictionary = outputDictionary.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

            return (outputProject, outputDictionary);
        }


        internal static ProjectModel OverrideDictionaryInProject(ProjectModel projectToChange, Dictionary<string, List<string>> updatedDictionary, bool futureFeaturesLoaded, bool challengesLoaded)
        {
            // 5refs
            if (futureFeaturesLoaded == true)
            {
                projectToChange.FutureAdditions = updatedDictionary;
            }

            if (challengesLoaded == true)
            {
                projectToChange.Challenges = updatedDictionary;
            }

            return projectToChange;
        }

        public static List<string> MakeListFromDictionaryItemValues(Dictionary<string, List<string>> inputList, string itemKey)
        {
            List<string> outputList = new List<string>();

            foreach (var key in inputList)
            {
                if (key.Key.ToLower() == itemKey.ToLower())
                {
                    foreach (string value in key.Value)
                    {
                        outputList.Add(value);
                    }
                }
            }

            outputList = outputList.OrderBy(x => x).ToList();

            return outputList;
        }

        public static List<string> SearchInCollection(this string stringToSeach,  List<string> collection)
        {
            List<string> outputlist = new List<string>();

            outputlist = collection.Where(x => x.ToLower().Contains(stringToSeach.ToLower())).ToList();

            return outputlist;
        }
    }
}
