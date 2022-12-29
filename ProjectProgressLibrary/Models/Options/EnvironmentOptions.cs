using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectProgressLibrary.Models.Options
{
    public class EnvironmentOptions
    {
        public string CurrentEnvironment { get; set; }   
    }
    public class PossibleEvironments
    {
        public const string Production = "Production";
        public const string Development = "Development";
        public const string Demo = "Demo";
    }

   
}
