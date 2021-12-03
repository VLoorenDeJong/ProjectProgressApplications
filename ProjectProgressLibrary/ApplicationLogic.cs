using ProjectProgressLibrary.DataAccess;
using ProjectProgressLibrary.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProjectProgressLibrary.Enums;

namespace ProjectProgressLibrary
{
   public static class ApplicationLogic
    {


        public static DateTime CreateDateFromString(string dateTimeStringToParse)
        {
            DateTime outputDate = DateTime.Now;

            if (string.IsNullOrEmpty(dateTimeStringToParse) == false)
            {
                CultureInfo culture = new CultureInfo("en-US");
                
                try
                {
                    outputDate = DateTime.ParseExact(dateTimeStringToParse, "dd/MM/yyyy hh:mm:ss tt", culture);
                }
                catch (Exception)
                {
                    DateTime dateTimeToModify = DateTime.Parse(dateTimeStringToParse);
                    dateTimeStringToParse = CreateSavableTimeString(dateTimeToModify);
                    outputDate = DateTime.ParseExact(dateTimeStringToParse, "dd/MM/yyyy hh:mm:ss tt", culture);
                }
            }
            return outputDate;
        }


        internal static string CreateSavableTimeString(DateTime inputTime)
        {
            string output = "";

            string inputTimeString = inputTime.ToString("dd/MM/yyyy hh:mm:ss tt");
            output = inputTimeString.ToUpper();

            return output;
        }

        internal static DateTime CreateDateTimeFromString(string inputDateTime)
        {
            DateTime outputDateTime = CreateDateFromString(inputDateTime);

            return outputDateTime;
        }
    }
}
