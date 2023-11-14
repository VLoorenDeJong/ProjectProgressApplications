using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProjectProgressLibrary.Enums;

namespace ProjectProgressLibrary.Models
{
    public class SolutionModel
    {
        public SolutionModel()
        {

        }
        public SolutionModel(DictionaryClassification classification)
        {

            switch (classification)
            {
                case DictionaryClassification.Challenges:
                    Mode = DictionaryMode.Challenges;
                    break;
                case DictionaryClassification.FutureAdditions:
                    Mode = DictionaryMode.Additions;
                    break;
            }
        }

        [StringLength(32)]
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Key { get; set; }
        public List<String> Values { get; set; }
        public string ProjectTitle { get; set; }
        public string Mode { get; private set; }

    }
}
