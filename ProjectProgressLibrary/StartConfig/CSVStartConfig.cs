﻿using Microsoft.Extensions.Configuration;
using ProjectProgressLibrary.DataAccess;
using System.Runtime.CompilerServices;
using ProjectProgressLibrary.Models.Options;
using System;
using System.IO;
using Microsoft.Extensions.Options;

namespace ProjectProgressLibrary.StartConfig
{
    public class CSVStartConfig : IStartConfig
    {
        private readonly IOptions<ApplicationOptions> _applicationOptions;
        private readonly IOptions<EnvironmentOptions> _environmentOptions;
        private readonly IOptions<PlatformOptions> _platformOptions;
        public CSVStartConfig(IOptions<ApplicationOptions> applicationOptions, IOptions<EnvironmentOptions> environmentOptions, IOptions<PlatformOptions> platformOptions)
        {
            _applicationOptions = applicationOptions;
            _environmentOptions = environmentOptions;
            _platformOptions = platformOptions;
        }

        //ToDo make this front end
        public (IDataAccess database, string mainGoal) GetDbConfig(IConfiguration config, IDataAccess db, string pageName)
        {
            string currentDirectory = Directory.GetCurrentDirectory();

            string projectFilePath = "";
            string timeUnitFilePath = "";

            string databseFolderPath = "";
            string projectPicturesFolderPath = "";

            // Get all base settings from json file
            string connectionType = _applicationOptions?.Value?.CurrentDataStorage;
            string environment = _environmentOptions?.Value?.CurrentEnvironment;
            string platform = _platformOptions?.Value?.CurrentPlatform;
            string mainGoal = "";

            switch (_applicationOptions?.Value?.CurrentDataStorage)
            {
                case PossibleDataStorage.CSV:
                    switch (_environmentOptions?.Value?.CurrentEnvironment)
                    {
                        case PossibleEvironments.Development:
                            mainGoal = PredefinedGoals.DevelopmentGoal;
                            projectFilePath = @$"{_applicationOptions?.Value?.BackendDatabaseFolderLocation}{mainGoal}{_environmentOptions?.Value?.CurrentEnvironment}{FileNames.ProjectsFile}";
                            timeUnitFilePath = @$"{_applicationOptions?.Value?.BackendDatabaseFolderLocation}{mainGoal}{_environmentOptions?.Value?.CurrentEnvironment}{FileNames.TimeUnitsFile}";
                            projectPicturesFolderPath = @$"{_applicationOptions?.Value?.BackendProjectPicturesFolderPath}";
                            databseFolderPath = _applicationOptions?.Value?.BackendDatabaseFolderLocation;

                            break;
                        case PossibleEvironments.Demo:
                            mainGoal = PredefinedGoals.DemoGoal;
                            projectFilePath = @$"{_applicationOptions?.Value?.BackendDatabaseFolderLocation}{mainGoal}{_environmentOptions?.Value?.CurrentEnvironment}{FileNames.ProjectsFile}";
                            timeUnitFilePath = @$"{_applicationOptions?.Value?.BackendDatabaseFolderLocation}{mainGoal}{_environmentOptions?.Value?.CurrentEnvironment}{FileNames.TimeUnitsFile}";
                            projectPicturesFolderPath = @$"{_applicationOptions?.Value?.BackendProjectPicturesFolderPath}";
                            databseFolderPath = _applicationOptions?.Value?.BackendDatabaseFolderLocation;
                            break;
                        default:
                            mainGoal = _applicationOptions?.Value?.CurrentMainProjectGoal;
                            projectFilePath = @$"{_applicationOptions?.Value?.BackendDatabaseFolderLocation}{mainGoal}{_environmentOptions?.Value?.CurrentEnvironment}{FileNames.ProjectsFile}";
                            timeUnitFilePath = @$"{_applicationOptions?.Value?.BackendDatabaseFolderLocation}{mainGoal}{_environmentOptions?.Value?.CurrentEnvironment}{FileNames.TimeUnitsFile}";
                            projectPicturesFolderPath = @$"{_applicationOptions?.Value?.BackendProjectPicturesFolderPath}";
                            databseFolderPath = _applicationOptions?.Value?.BackendDatabaseFolderLocation;
                            break;
                    }
                    break;
            }

            // Get all locations from json file
            IDataAccess outputDb;

            if (db is CSVDataAccess)
            {
                outputDb = new CSVDataAccess(projectFilePath, timeUnitFilePath, databseFolderPath, projectPicturesFolderPath);
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
        // ToDo combine front and backend to one
        public (IDataAccess database, string mainGoal) GetProgressDbConfig(IConfiguration config, IDataAccess db, string pageName)
        {
            string folderDelimiter = SetCurrentDelimiter(_platformOptions?.Value?.CurrentPlatform);

            string backendProjectFilePath = "";
            string backendTimeUnitFilePath = "";
            string backendDatabaseFolderPath = "";

            string frondendProjectFilePath = "";
            string frontendTimeUnitFilePath = "";
            string frontEndDatabaseFolderPath = "";

            string backupProjectFilePath = "";
            string backupTimeUnitFilePath = "";
            string backupDatabaseFolderPath = "";

            string mainGoal = "";

            // Get pictureFolder path from json file
            string backendendPhotoFolderPath = "";
            string frontendPhotoFolderPath = "";
            string backupPhotoFolderPath = "";

            IDataAccess outputDb;
            switch (_applicationOptions?.Value?.CurrentDataStorage)
            {
                case PossibleDataStorage.CSV:
                    switch (_environmentOptions?.Value?.CurrentEnvironment)
                    {
                        case PossibleEvironments.Production:
                            mainGoal = _applicationOptions.Value?.CurrentMainProjectGoal;
                            break;

                        case PossibleEvironments.Development:
                            mainGoal = PredefinedGoals.DevelopmentGoal;
                            break;

                        case PossibleEvironments.Demo:
                            mainGoal = PredefinedGoals.DemoGoal;
                            break;
                    }

                    backendProjectFilePath = @$"{_applicationOptions?.Value?.BackendDatabaseFolderLocation}{_environmentOptions?.Value?.CurrentEnvironment}_{_applicationOptions?.Value?.CurrentMainProjectGoal}{FileNames.ProjectsFile}";
                    backendTimeUnitFilePath = @$"{_applicationOptions?.Value?.BackendDatabaseFolderLocation}{_environmentOptions?.Value?.CurrentEnvironment}_{_applicationOptions?.Value?.CurrentMainProjectGoal}{FileNames.TimeUnitsFile}";
                    backendDatabaseFolderPath = @$"{_applicationOptions?.Value?.BackendDatabaseFolderLocation}";
                    backendendPhotoFolderPath = @$"{_applicationOptions?.Value?.BackendProjectPicturesFolderPath}";

                    if (!string.IsNullOrWhiteSpace(_applicationOptions?.Value?.FrontendDatabaseFolderLocation))
                    {
                        frondendProjectFilePath = @$"{_applicationOptions?.Value?.FrontendDatabaseFolderLocation}{_environmentOptions?.Value?.CurrentEnvironment}_{_environmentOptions?.Value?.CurrentEnvironment}{FileNames.ProjectsFile}";
                        frontendTimeUnitFilePath = @$"{_applicationOptions?.Value?.FrontendDatabaseFolderLocation}{_environmentOptions?.Value?.CurrentEnvironment}_{_environmentOptions?.Value?.CurrentEnvironment}{FileNames.TimeUnitsFile}";
                        frontEndDatabaseFolderPath = @$"{_applicationOptions?.Value?.FrontendDatabaseFolderLocation}";
                        frontendPhotoFolderPath = $@"{_applicationOptions?.Value?.FrontendProjectPicturesFolderPath}";
                    }

                    if (!string.IsNullOrWhiteSpace(_applicationOptions?.Value?.BackupDatabaseFolderLocation))
                    {
                        string date = DateTime.Now.Date.ToString("yyyy-MM-dd");
                        backupProjectFilePath = @$"{_applicationOptions?.Value?.BackupDatabaseFolderLocation}{date}_{_environmentOptions?.Value?.CurrentEnvironment}_{mainGoal}{_environmentOptions?.Value?.CurrentEnvironment}{FileNames.ProjectsFile}";
                        backupTimeUnitFilePath = @$"{_applicationOptions?.Value?.BackupDatabaseFolderLocation}{date}_{_environmentOptions?.Value?.CurrentEnvironment}_{mainGoal}{_environmentOptions?.Value?.CurrentEnvironment}{FileNames.TimeUnitsFile}";
                        backupDatabaseFolderPath = $@"{_applicationOptions?.Value?.BackupDatabaseFolderLocation}";
                        backupPhotoFolderPath = $@"{_applicationOptions.Value?.BackupProjectPicturesFolderPath}";
                    }
                    break;
            }

            if (db is CSVDataAccess)
            {
                outputDb = new CSVDataAccess(backendProjectFilePath, backendTimeUnitFilePath, backendDatabaseFolderPath, backendendPhotoFolderPath, frondendProjectFilePath, frontendTimeUnitFilePath, frontEndDatabaseFolderPath, frontendPhotoFolderPath, backupProjectFilePath, backupTimeUnitFilePath, backupDatabaseFolderPath, backupPhotoFolderPath);
            }
            else
            {
                outputDb = db;
            }

            if (pageName.ToLower() == "index" && string.Equals(_environmentOptions?.Value?.CurrentEnvironment, PossibleEvironments.Demo, StringComparison.CurrentCultureIgnoreCase))
            {
                outputDb.MakeFirstEntry(PredefinedGoals.DemoGoal);
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

                outputLocation = $"{currentDirectory}{placementFolderPath}{finalFolderPath}";
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

            string location = $"{currentDirectory}{placementFolderPath}{finalFolderPath}{developmentFolderPath}";

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
        private static string GetDatabaseLocation(string endSelection, IConfiguration config)
        {
            string mainSection = "DataAccess";
            string value = "DatabaseFolderLocation";

            string output = GetStringValueFromConfig(mainSection, endSelection, value, config);

            return output;
        }

        public string GetProjectPhotosFolderPath(IConfiguration config)
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
        private static string GetProjectPhotosFolderPath(string endSelection, IConfiguration config)
        {
            string mainSection = "DataAccess";
            string value = "PhotoFolderLocation";

            string output = GetStringValueFromConfig(mainSection, endSelection, value, config);

            return output;
        }
        public (string ProjectPictureFilePath, bool ShowPicture) SetUpPictureShowing(string title, IDataAccess db, string rootFolderPath)
        {
            bool showPicture = false;


            string pictureName = db.ExchangePunctuations(title);
            string projectPhotoName = $"{pictureName}.jpg";

            string fullFilePath = rootFolderPath + projectPhotoName;

            showPicture = DecideToShowPicture(fullFilePath);

            // Do not put in a full file path start from rootwww 
            string outputFilepath = @$"project_photos\{projectPhotoName}"; ;

            return (outputFilepath, showPicture);
        }
        public bool DecideToShowPicture(string projectPictureFilePath)
        {
            bool showPicture = false;

            if (System.IO.File.Exists($"{projectPictureFilePath}") == true)
            {
                showPicture = true;
            }

            return showPicture;
        }

        private string SetCurrentDelimiter(string currentPlatform)
        {
            string output = "";

            if (string.IsNullOrWhiteSpace(currentPlatform)) throw new Exception("No platform set in appsettings");
            else
            {
                switch (currentPlatform)
                {
                    case PossiblePlatforms.Windows:
                        return @"\";

                    case PossiblePlatforms.Ubuntu:
                        return @"/";
                }
            }
            return output;
        }
    }
}
