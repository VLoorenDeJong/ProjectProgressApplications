using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ProjectProgressLibrary.Models.Options
{
    public class ApplicationOptions
    {
        public string CurrentDataStorage { get; set; }
        public bool HasPortfolio { get; set; }
    }


    public class PossibleDataStorage
    {
        public const string CSV = "CSV";
        public const string MySQL = "MySQL";
        public const string MsSQL = "MsSQL";
    }

    public class PredefinedGoals
    {
        public const string DemoGoal = "Demoing_the_app";
        public const string DevelopmentGoal = "!!!!_DEVELOPMENT_ENVIRONMENT_!!!!";
    }

    public class FileNames
    {
        public const string ProjectsFile = "ProjectsFile.CSV";
        public const string TimeUnitsFile = "TimeUnitsFile.CSV";

    }

    public class AppsettingsSections
    {
        public const string ApplicationSettings = "ApplicationSettings";
        public const string DataStorageSection = "DataStorage";
        public const string AppliactionInstances = "AppliactionInstances";
        public const string CurrentMainProjectGoal = "CurrentMainProjectGoal";
    }

}
