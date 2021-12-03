using ProjectProgressLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectProgressWebFront1
{
    public static class Validation
    {
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
        public static bool ValidateStringHasContent(this string stringToCheck)
        {
            bool output = false;

            if (string.IsNullOrEmpty(stringToCheck) == false)
            {
                output = true;
            }

            return output;
        }
        public static bool ValidateGuid(this string stringGuid)
        {
            bool output = false;

            if (stringGuid != "" && stringGuid.Length == 36)
            {
                output = true;
            }
            // ToDo make a check for 4 dashes

            return output;
        }
    }


}
