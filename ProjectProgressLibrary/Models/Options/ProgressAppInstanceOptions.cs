using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectProgressLibrary.Models.Options
{
    public class ProgressAppInstanceOptions
    {
        public string CurrentMainProjectGoal { get; set; }

        public string BackendDatabaseFolderLocation { get; set; }
        public string BackendProjectPicturesFolderPath { get; set; }

        public string FrontendDatabaseFolderLocation { get; set; }
        public string FrontendProjectPicturesFolderPath { get; set; }

        public string BackupDatabaseFolderLocation { get; set; }
        public string BackupProjectPicturesFolderPath { get; set; }
    }
}
