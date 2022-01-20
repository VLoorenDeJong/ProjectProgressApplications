using ProjectProgressLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectProgressLibrary.Validation
{
   public static class DataValidation
    {
        public static bool ValidateGuid(this string stringGuid)
        {
            bool output = false;

                if (stringGuid == "")
                {
                    throw new Exception($"This is a empty guid");
                }
                if (stringGuid != "" && stringGuid.Length != 36)
                {
                    throw new Exception($"This is not a valid id {stringGuid}");
                }
                if (stringGuid != "" && stringGuid.Length == 36)
                {
                    output = true;
                }
                // ToDo make a check for 4 dashes

            return output;
        }
        public static bool ValidateInt (this string stringInt)
        {
            bool output = false;

            output = int.TryParse(stringInt, out int result);

            return output;

        }
        public static bool ValidateDouble(this string stringDouble, string comesFrom)
        {
            bool output = false;

            output = double.TryParse(stringDouble, out double result);


            if (result < 0)
            {
                throw new Exception($"Comes from { comesFrom } value is { result }");
            }


            return output;
        }
        public static bool ValidateStringHasContent(this string stringToCheck)
        {
            bool output = false;

            if (string.IsNullOrEmpty(stringToCheck) ==  false)
            {
                output = true;
            }

            return output;
        }

        public static bool ValidateIfProjectTitleExsists(this string projectTitle, List<ProjectModel> allProjects)
        {

            bool output = true;

            if (string.IsNullOrEmpty(projectTitle) == true)
            {
                return false;
                // return output;
            }

            ProjectModel project;
            try
            {
                project = allProjects.Where(x => x.Title.ToLower() == projectTitle.ToLower()).First();

                //project = allProjects.Where(x => x.Title.ToLower().Contains(projectTitle.ToLower())).First();
            }
            catch (System.InvalidOperationException)
            {
                output = false;
            }

            return output;
        }
        public static bool ValidateIfSearchedProjectTitleExsists(this string projectTitle, List<ProjectModel> allProjects)
        {

            bool output = true;

            if (string.IsNullOrEmpty(projectTitle) == true)
            {
                return false;
                // return output;
            }

            ProjectModel project;
            try
            {
                //project = allProjects.Where(x => x.Title.ToLower() == projectTitle.ToLower()).First();

                project = allProjects.Where(x => x.Title.ToLower().Contains(projectTitle.ToLower())).First();
            }
            catch (System.InvalidOperationException)
            {
                output = false;
            }

            return output;
        }
        public static bool ValidateIfItemExists(this string itemToSearch, List<string> allItemKeys)
        {
            bool output = true;

            if (string.IsNullOrEmpty(itemToSearch) == true)
            {
                return false;
            }

            string item = null;
            try
            {
                item = allItemKeys.Where(x => x.ToLower().Contains(itemToSearch.ToLower())).First();
            }
            catch (Exception)
            {
                return false;
            }

            return output;


        }
       
    }
}
