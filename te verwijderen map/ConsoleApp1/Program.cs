using Microsoft.Extensions.Configuration;
using ProjectProgressLibrary;
using ProjectProgressLibrary.DataAccess;
using ProjectProgressLibrary.Models;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using static ProjectProgressLibrary.Enums;

namespace ConsoleApp1
{
    class Program
    {
        private static IConfiguration _config;
        private static string _csvProjectsFile = "E:\\C#\\TextFileData\\Projects0.csv";
        private static string _csvTimeUnitsFile = "E:\\C#\\TextFileData\\TimeUnits0.csv";

        static void Main(string[] args)
        {
            //CSVDataAccess projectDb = new CSVDataAccess();

            //ProjectModel project1 = new ProjectModel();
            //ProjectModel project2 = new ProjectModel();
            //ProjectModel project3 = new ProjectModel();
            //ProjectModel project4 = new ProjectModel();
            //ProjectModel project5 = new ProjectModel();
            //ProjectModel project6 = new ProjectModel();

            //TimeUnitModel time2 = new TimeUnitModel("10", HourClassification.Theoretical, project1.ProjectId);
            //TimeUnitModel time1 = new TimeUnitModel("10", HourClassification.InCode, project1.ProjectId);
            //TimeUnitModel time3 = new TimeUnitModel("10", HourClassification.General, project1.ProjectId);

            //project1.Title = "testProject1";

            //project2.Title = "testProject2";
            //project2.SetMainProjectId(project1.ProjectId);
            //project1.AddSubProject(project2.ProjectId.ToString());

            //project3.Title = "testProject3";
            //project3.SetMainProjectId(project2.ProjectId);
            //project2.AddSubProject(project3.ProjectId.ToString());

            //project4.Title = "testProject4";
            //project4.SetMainProjectId(project2.ProjectId);
            //project3.AddSubProject(project3.ProjectId.ToString());

            //project5.Title = "testProject5";
            //project5.SetMainProjectId(project3.ProjectId);
            //project3.AddSubProject(project5.ProjectId.ToString());

            //project6.Title = "testProject6";
            //project6.SetMainProjectId(project3.ProjectId);
            //project3.AddSubProject(project6.ProjectId.ToString());



            //List<ProjectModel> exportProjectList = new List<ProjectModel> { project1, project2 };
            //List<TimeUnitModel> exportTimeUnitList = new List<TimeUnitModel> { time1, time2 };

            //project3.AddTimeToProject(time1, exportProjectList);
            //project2.AddTimeToProject(time2, exportProjectList);
            //project1.AddTimeToProject(time3, exportProjectList);

            //project4.StartProject();
            //project5.StartProject();
            //project6.FinishProject();

            //exportProjectList.Add(project1);
            //exportProjectList.Add(project2);
            //exportProjectList.Add(project3);
            //exportProjectList.Add(project4);
            //exportProjectList.Add(project5);
            //exportProjectList.Add(project6);


            //project6.CalculateProjectProgress(exportProjectList);





            //projectDb.SaveAllRecords(exportProjectList, exportTimeUnitList);

            //(List<ProjectModel> importProjectList, List<TimeUnitModel> importedTimeUnits) = projectDb.ReadAllProjects();

            Console.ReadLine();

        }

        private static void GetSetting()
        {
            var builder = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile("appsettings.json");

            _config = builder.Build();
        }
    }
    
}
