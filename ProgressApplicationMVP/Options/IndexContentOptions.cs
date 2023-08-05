using System.Collections.Generic;

namespace ProgressApplicationMVP.Options
{
    public class IndexContentOptions
    {
        public List<string> CurrentFeatures { get; set; }
        public List<string> FutureFeatures { get; set; }
        public List<string> KnownBugs { get; set; }
        public List<string> BugfixesUnderReview { get; set; }
    }
}
