using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectProgressLibrary
{
    public class Enums
    {
        public enum ProjectInstance
        {
            demo_application,
            victor_developer,
            de_kunst_van_het_hopen
        }
        public enum HourClassification
        {
            Practical,
            Theoretical,
            General
        }
        public enum ProjectStatus
        {
            ToDo,
            Doing,
            Done
        }
        public enum DictionaryClassification
        {
            Challenges,
            FutureAdditions,
            All
        }
    }
}
