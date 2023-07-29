using Microsoft.Extensions.Configuration;
using ProjectProgressLibrary.DataAccess;
using System.Runtime.CompilerServices;
using ProjectProgressLibrary.Models.Options;
using System;
using System.IO;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using ProjectProgressLibrary.Interfaces;

namespace ProjectProgressLibrary.StartConfig
{
    public class CSVStartConfig : IStartConfig
    {
        private readonly IOptions<ApplicationOptions> _applicationOptions;
        private readonly IOptions<EnvironmentOptions> _environmentOptions;
        private readonly IOptions<PlatformOptions> _platformOptions;
        private readonly IOptions<ProgressAppInstanceOptions> _progressAppInstanceOptions;
        private readonly ILogger<CSVStartConfig> _logger;

        public CSVStartConfig(IOptions<ApplicationOptions> applicationOptions, IOptions<EnvironmentOptions> environmentOptions, IOptions<PlatformOptions> platformOptions, IOptions<ProgressAppInstanceOptions> progressAppInstanceOptions, ILogger<CSVStartConfig> logger)
        {
            _applicationOptions = applicationOptions;
            _environmentOptions = environmentOptions;
            _platformOptions = platformOptions;
            _progressAppInstanceOptions = progressAppInstanceOptions;
            _logger = logger;

            _logger.LogInformation("CSVStartConfig => CTOR => CSV Enabled");
            ConfigCheck();
        }

        public (IDataAccess database, string mainGoal) GetDbConfig(IConfiguration config, IDataAccess db, string pageName)
        {
            string frondendProjectFilePath = "";
            string frontendTimeUnitFilePath = "";

            string frontEndDatabaseFolderPath = "";
            string frontendPicturesFolderPath = "";

            // Get all base settings from json file
            string connectionType = _applicationOptions?.Value?.CurrentDataStorage;
            string environment = _environmentOptions?.Value?.CurrentEnvironment;
            string platform = _platformOptions?.Value?.CurrentPlatform;
            string mainGoal = "";

            _logger.LogInformation("CSVStartConfig => GetDbConfig => CurrentDataStorage: {stor}", _applicationOptions?.Value?.CurrentDataStorage);
            switch (_applicationOptions?.Value?.CurrentDataStorage)
            {
                case PossibleDataStorage.CSV:
                    switch (_environmentOptions?.Value?.CurrentEnvironment)
                    {
                        case PossibleEvironments.Development:
                            mainGoal = PredefinedGoals.DevelopmentGoal;

                            break;
                        case PossibleEvironments.Demo:
                            mainGoal = PredefinedGoals.DemoGoal;
                            break;
                        default:
                            mainGoal = _progressAppInstanceOptions?.Value?.CurrentMainProjectGoal;
                            break;
                    }

                    frondendProjectFilePath = @$"{_progressAppInstanceOptions?.Value?.FrontendDatabaseFolderLocation}{_environmentOptions?.Value?.CurrentEnvironment}_{FileNames.ProjectsFile}";
                    frontendTimeUnitFilePath = @$"{_progressAppInstanceOptions?.Value?.FrontendDatabaseFolderLocation}{_environmentOptions?.Value?.CurrentEnvironment}_{FileNames.TimeUnitsFile}";
                    frontEndDatabaseFolderPath = @$"{_progressAppInstanceOptions?.Value?.FrontendDatabaseFolderLocation}";
                    frontendPicturesFolderPath = $@"{_progressAppInstanceOptions?.Value?.FrontendProjectPicturesFolderPath}";

                    break;
            }

            // Get all locations from json file
            IDataAccess outputDb;

            if (db is CSVDataAccess)
            {
                outputDb = new CSVDataAccess(frondendProjectFilePath, frontendTimeUnitFilePath, frontEndDatabaseFolderPath, frontendPicturesFolderPath, _logger);
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
            string backendendPicturesFolderPath = "";
            string frontendPicturesFolderPath = "";
            string backupPicturesFolderPath = "";

            IDataAccess outputDb;

            _logger.LogInformation("CSVStartConfig => GetProgressDbConfig => CurrentDataStorage: {stor}", _applicationOptions?.Value?.CurrentDataStorage);

            switch (_applicationOptions?.Value?.CurrentDataStorage)
            {
                case PossibleDataStorage.CSV:
                    switch (_environmentOptions?.Value?.CurrentEnvironment)
                    {
                        case PossibleEvironments.Production:
                            mainGoal = _progressAppInstanceOptions.Value?.CurrentMainProjectGoal;
                            break;

                        case PossibleEvironments.Development:
                            mainGoal = PredefinedGoals.DevelopmentGoal;
                            break;

                        case PossibleEvironments.Demo:
                            mainGoal = PredefinedGoals.DemoGoal;
                            break;
                    }

                    _logger.LogInformation("CSVStartConfig => GetProgressDbConfig => MainGoal: {goal}", mainGoal);

                    if (!string.IsNullOrEmpty(_progressAppInstanceOptions?.Value?.BackendDatabaseFolderLocation))
                    {
                        _logger.LogInformation("CSVStartConfig => GetProgressDbConfig => BackendDatabaseFolderLocation: {loc}", _progressAppInstanceOptions?.Value?.BackendDatabaseFolderLocation);
                        backendProjectFilePath = @$"{_progressAppInstanceOptions?.Value?.BackendDatabaseFolderLocation}{_environmentOptions?.Value?.CurrentEnvironment}_{_progressAppInstanceOptions?.Value?.CurrentMainProjectGoal}_{FileNames.ProjectsFile}";
                        backendTimeUnitFilePath = @$"{_progressAppInstanceOptions?.Value?.BackendDatabaseFolderLocation}{_environmentOptions?.Value?.CurrentEnvironment}_{_progressAppInstanceOptions?.Value?.CurrentMainProjectGoal}_{FileNames.TimeUnitsFile}";
                        backendDatabaseFolderPath = @$"{_progressAppInstanceOptions?.Value?.BackendDatabaseFolderLocation}";
                        backendendPicturesFolderPath = @$"{_progressAppInstanceOptions?.Value?.BackendProjectPicturesFolderPath}";
                    }
                    else
                    {
                        _logger.LogError("CSVStartConfig => GetProgressDbConfig => BackendDatabaseFolderLocation empty!!: {loc}", _progressAppInstanceOptions?.Value?.BackendDatabaseFolderLocation);
                    }

                    if (!string.IsNullOrWhiteSpace(_progressAppInstanceOptions?.Value?.FrontendDatabaseFolderLocation))
                    {
                        _logger.LogInformation("CSVStartConfig => GetProgressDbConfig => FrontendDatabaseFolderLocation: {loc}", _progressAppInstanceOptions?.Value?.FrontendDatabaseFolderLocation);
                        frondendProjectFilePath = @$"{_progressAppInstanceOptions?.Value?.FrontendDatabaseFolderLocation}{_environmentOptions?.Value?.CurrentEnvironment}_{FileNames.ProjectsFile}";
                        frontendTimeUnitFilePath = @$"{_progressAppInstanceOptions?.Value?.FrontendDatabaseFolderLocation}{_environmentOptions?.Value?.CurrentEnvironment}_{FileNames.TimeUnitsFile}";
                        frontEndDatabaseFolderPath = @$"{_progressAppInstanceOptions?.Value?.FrontendDatabaseFolderLocation}";
                        frontendPicturesFolderPath = $@"{_progressAppInstanceOptions?.Value?.FrontendProjectPicturesFolderPath}";
                    }
                    else
                    {
                        _logger.LogError("CSVStartConfig => GetProgressDbConfig => FrontendDatabaseFolderLocation empty!!: {loc}", _progressAppInstanceOptions?.Value?.FrontendDatabaseFolderLocation);
                    }

                    if (!string.IsNullOrWhiteSpace(_progressAppInstanceOptions?.Value?.BackupDatabaseFolderLocation))
                    {
                        _logger.LogInformation("CSVStartConfig => GetProgressDbConfig => BackupDatabaseFolderLocation: {loc}", _progressAppInstanceOptions?.Value?.BackupDatabaseFolderLocation);
                        string date = DateTime.Now.Date.ToString("yyyy-MM-dd");
                        backupProjectFilePath = @$"{_progressAppInstanceOptions?.Value?.BackupDatabaseFolderLocation}{date}_{_environmentOptions?.Value?.CurrentEnvironment}_{mainGoal}{_environmentOptions?.Value?.CurrentEnvironment}_{FileNames.ProjectsFile}";
                        backupTimeUnitFilePath = @$"{_progressAppInstanceOptions?.Value?.BackupDatabaseFolderLocation}{date}_{_environmentOptions?.Value?.CurrentEnvironment}_{mainGoal}{_environmentOptions?.Value?.CurrentEnvironment}_{FileNames.TimeUnitsFile}";
                        backupDatabaseFolderPath = $@"{_progressAppInstanceOptions?.Value?.BackupDatabaseFolderLocation}";
                        backupPicturesFolderPath = $@"{_progressAppInstanceOptions.Value?.BackupProjectPicturesFolderPath}";
                    }
                    else
                    {
                        _logger.LogError("CSVStartConfig => GetProgressDbConfig => BackupDatabaseFolderLocation empty!!: {loc}", _progressAppInstanceOptions?.Value?.FrontendDatabaseFolderLocation);
                    }
                    break;
            }

            if (db is CSVDataAccess)
            {
                _logger.LogInformation("CSVtartConfig => GetProgressDbConfig => Starting CSVDataAccess CTOR 2");

                outputDb = new CSVDataAccess(backendProjectFilePath, backendTimeUnitFilePath, backendDatabaseFolderPath, backendendPicturesFolderPath, frondendProjectFilePath, frontendTimeUnitFilePath, frontEndDatabaseFolderPath, frontendPicturesFolderPath, backupProjectFilePath, backupTimeUnitFilePath, backupDatabaseFolderPath, backupPicturesFolderPath, _logger);
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

        public string GetProjectPhotosFolderPath(IConfiguration config)
        {
            string outputFilePath = "";

            outputFilePath = _progressAppInstanceOptions?.Value?.FrontendProjectPicturesFolderPath;

            return outputFilePath;
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

        private void ConfigCheck()
        {
            _logger.LogInformation("CSVStartConfig => CTOR => Start config check");

            if (_applicationOptions is null) _logger.LogError("CSVStartConfig => CTOR => ConfigCheck => _applicationOptions is null!!");
            else _logger.LogInformation("CSVStartConfig => CTOR => ConfigCheck => _applicationOptions is not null");


            if (_environmentOptions is null) _logger.LogError("CSVStartConfig => CTOR => ConfigCheck => _environmentOptions is null!!");
            else _logger.LogInformation("CSVStartConfig => CTOR => ConfigCheck => _environmentOptions.CurrentEnvironment: {env}", _environmentOptions.Value?.CurrentEnvironment);


            if (_platformOptions is null) _logger.LogError("CSVStartConfig => CTOR => ConfigCheck => _platformOptions is null!!");
            else _logger.LogInformation("CSVStartConfig => CTOR => ConfigCheck => _platformOptions.CurrentPlatform: {plat}", _platformOptions.Value?.CurrentPlatform);

            if (_progressAppInstanceOptions is null) _logger.LogError("CSVStartConfig => CTOR => ConfigCheck => _progressAppInstanceOptions is null!!");
            else 
            {
                _logger.LogInformation("CSVStartConfig => CTOR => ConfigCheck => _progressAppInstanceOptions.CurrentMainProjectGoal: {info}", _progressAppInstanceOptions.Value?.CurrentMainProjectGoal);
                _logger.LogInformation("CSVStartConfig => CTOR => ConfigCheck => _progressAppInstanceOptions.BackendDatabaseFolderLocation: {info}", _progressAppInstanceOptions.Value?.BackendDatabaseFolderLocation);
                _logger.LogInformation("CSVStartConfig => CTOR => ConfigCheck => _progressAppInstanceOptions.BackendProjectPicturesFolderPath: {info}", _progressAppInstanceOptions.Value?.BackendProjectPicturesFolderPath);
                _logger.LogInformation("CSVStartConfig => CTOR => ConfigCheck => _progressAppInstanceOptions.FrontendDatabaseFolderLocation: {info}", _progressAppInstanceOptions.Value?.FrontendDatabaseFolderLocation);
                _logger.LogInformation("CSVStartConfig => CTOR => ConfigCheck => _progressAppInstanceOptions.FrontendProjectPicturesFolderPath: {info}", _progressAppInstanceOptions.Value?.FrontendProjectPicturesFolderPath);
                _logger.LogInformation("CSVStartConfig => CTOR => ConfigCheck => _progressAppInstanceOptions.BackupDatabaseFolderLocation: {info}", _progressAppInstanceOptions.Value?.BackupDatabaseFolderLocation);
                _logger.LogInformation("CSVStartConfig => CTOR => ConfigCheck => _progressAppInstanceOptions.BackupProjectPicturesFolderPath: {info}", _progressAppInstanceOptions.Value?.BackupProjectPicturesFolderPath);
            }
        }
    }
}
