using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProjectProgressLibrary.Validation.DataValidation;

namespace ProjectProgressLibrary.Modifications
{
    public static class TekstModifications
    {
        public static Guid ConvertStringToGuid(string stringGuid)
        {
            Guid outputGuid = Guid.NewGuid();

            bool isGuid = stringGuid.ValidateGuid();
            if (isGuid == true)
            {
                outputGuid = new Guid(stringGuid);
            }

            return outputGuid;
        }
        internal static List<string> CreateDeveloperNamesList(string developerName)
        {

            string delimiter = @", ";
            string[] developers = developerName.Split(delimiter);
            List<string> outputList = developers.ToList();

            return outputList;
        }
        public static string DecideDayOfWeek(DateTime searchDate)
        {
            string outputDay = "";
            switch (searchDate.DayOfWeek)
            {
                case System.DayOfWeek.Sunday:
                    outputDay = "Zondag";
                    break;
                case System.DayOfWeek.Monday:
                    outputDay = "Maandag";
                    break;
                case System.DayOfWeek.Tuesday:
                    outputDay = "Dinsdag";
                    break;
                case System.DayOfWeek.Wednesday:
                    outputDay = "Woensdag";
                    break;
                case System.DayOfWeek.Thursday:
                    outputDay = "Donderdag";
                    break;
                case System.DayOfWeek.Friday:
                    outputDay = "Vrijdag";
                    break;
                case System.DayOfWeek.Saturday:
                    outputDay = "Zaterdag";
                    break;
                default:
                    break;
            }

            return outputDay;
        }
    }
}
