using Microsoft.Extensions.Configuration;
using ProjectProgressLibrary.DataAccess;
using ProjectProgressLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgressApplicationMVP
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
                    string date = DateTime.Now.Date.ToString("yyyy-MM-dd");
                    backupProjectFilePath = $"{ backupDatabaseFolderPath }{ date }_{ mainGoal }{ environment }ProjectsFile.CSV";
                    backupTimeUnitFilePath = $"{ backupDatabaseFolderPath }{ date }_{ mainGoal }{ environment }TimeUnitsFile.CSV";
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
        private static string GetDatabaseLocation(string endSelection, IConfiguration config)
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

        private static string GetStringValueFromConfig(string mainSection, string subSection, string value, IConfiguration config)
        {
            string output = config.GetSection(mainSection).GetSection(subSection).GetValue<string>(value);

            return output;
        }

      

       




       



    }
}
