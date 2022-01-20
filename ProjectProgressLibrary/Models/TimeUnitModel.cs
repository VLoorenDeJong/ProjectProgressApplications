using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProjectProgressLibrary.Enums;
using static ProjectProgressLibrary.Validation.DataValidation;
using static ProjectProgressLibrary.Validation.DateTimeValidation;

namespace ProjectProgressLibrary.Models
{
    public class TimeUnitModel
    {
        public TimeUnitModel()
        {
            CreateTimeStamp();
        }
        public string ProjectTitle { get; private set; }
        public Guid TimeUnitId { get; private set; } = Guid.NewGuid();
        public double HoursPutIn { get; private set; }
        public HourClassification Classification { get; private set; }
        public Guid ProjectId { get; private set; }
        public DateTime TimeStamp { get; private set; }
        public string Description { get; private set; }
        public void SetTimeUnitId(string stringGuid)
        {
            bool guidValidated = stringGuid.ValidateGuid();
            
            if (guidValidated == true)
            {
                TimeUnitId = new Guid(stringGuid);
            }
        }
        public void SetHoursPutIn(string hours)
        {
            if (hours == "")
            {
                HoursPutIn = 0;
            }
            if (hours != "")
            {
                HoursPutIn = double.Parse(hours);
            }
        }        
        internal string GetClassification(HourClassification classification)
        {
            // ToDo make it on GetHashCode
            string output = "";           

            switch (classification)
            {
                case HourClassification.Practical:
                    output = "1";
                    break;
                case HourClassification.General:
                    output = "2";
                    break;
                case HourClassification.Theoretical:
                    output = "3";
                    break;
                default:
                    break;
            }

            return output;
        }
        public void SetClassification(string classification)
        {
            // ToDo make it on GetHashCode
            if (classification == "")
            {
                Classification = HourClassification.General;
            }
            if (classification != "")
            {
                switch (classification)
                {
                    case "1":
                        Classification = HourClassification.Practical;
                        break;
                    case "2":
                        Classification = HourClassification.General;
                        break;
                    case "3":
                        Classification = HourClassification.Theoretical;
                        break;
                    default:
                        break;
                }
            }
        }
        public void SetClassification(HourClassification classification)
        {
            Classification = classification;
        }
        public void SetProjectId(string stringGuid)
        {
            bool guidValidated = stringGuid.ValidateGuid();

            if (guidValidated== true)
            {
                ProjectId = new Guid(stringGuid);
            }
        }
        public void SetProjectTitle(string projectTitle)
        {
            bool stringHasContent = projectTitle.ValidateStringHasContent();
            if (stringHasContent == true)
            {
                ProjectTitle = projectTitle;
            }
        }
        public void SetTimeStamp(string stringDateTime)
        {

            TimeStamp = CreateDateFromString(stringDateTime);
        }  
        public void SetDescription(string description)
        {
            Description = description;
        }
        private void CreateTimeStamp()
        {
            TimeStamp = DateTime.Now;
        }
        private string ChangeCharacterForCharacter(string input, string characterToReplace, string replacementCharacter)
        {
            bool containscharacterToReplace = input.Contains(characterToReplace);

            if (containscharacterToReplace == true)
            {
                input = input.Replace(characterToReplace, replacementCharacter);
            }

            return input;
        }
    }
}
