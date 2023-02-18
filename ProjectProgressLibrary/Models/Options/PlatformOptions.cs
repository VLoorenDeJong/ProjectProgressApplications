using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectProgressLibrary.Models.Options
{
    public class PlatformOptions
    {
        public const string AppSettingsSection = "Platform";
        public string CurrentPlatform { get; set; }       
    }

    public class PossiblePlatforms
    {
        public const string Windows = "Windows";
        public const string Ubuntu = "Ubuntu";
    }
}
