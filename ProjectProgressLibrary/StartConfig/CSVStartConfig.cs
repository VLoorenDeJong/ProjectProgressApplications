using Microsoft.Extensions.Configuration;
using ProjectProgressLibrary.DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectProgressLibrary.StartConfig
{
    public static class CSVStartConfig
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
    }
}
