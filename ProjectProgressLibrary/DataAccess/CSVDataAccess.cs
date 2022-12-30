using Microsoft.AspNetCore.Http;
using ProjectProgressLibrary.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ProjectProgressLibrary.Enums;
using static ProjectProgressLibrary.Modifications.TekstModifications;
using static ProjectProgressLibrary.Validation.DateTimeValidation;
using static ProjectProgressLibrary.Validation.DataValidation;
using System.Security.Cryptography.X509Certificates;

namespace ProjectProgressLibrary.DataAccess
{
    public class CSVDataAccess : IDataAccess
    {

        public bool FrontEndEnabled = false;
        public bool BackupEnabled = false;

        private string propertyDilimiter = ";|=*?*=|";
        private string DictionaryKeyDelimiter = "|=*&*=|";
        private string DictionaryItemDelimiter = "|=*$*=|";
        private string listItemDelimiter = "|=*!*=|";

        private readonly string _backendProjectTextFilePath;
        private readonly string _backendTimeUniTextFilePath;
        public readonly string _backendDatabaseFolderPath;
        public readonly string _backendPhotoFolderPath;

        private readonly string _frontendProjectTextFilePath;
        private readonly string _frontendTimeUniTextFilePath;
        public readonly string _frontendDatabaseFolderPath;
        public readonly string _frontendPhotoFolderPath;

        private readonly string _backupProjectTextFilePath;
        private readonly string _backupTimeUniTextFilePath;
        public readonly string _backupDatabaseFolderPath;
        public readonly string _backupPhotoFolderPath;

        public CSVDataAccess()
        {
        }
        public CSVDataAccess(string projectFilePath, string timeUnitFilePath, string databaseFileFolderPath, string projectPicturesFolderPath)
        {
            _backendProjectTextFilePath = projectFilePath;
            _backendTimeUniTextFilePath = timeUnitFilePath;

            if (string.IsNullOrWhiteSpace(databaseFileFolderPath)) throw new Exception("ID: 1 databaseFileFolderPath not specified");
            if (string.IsNullOrWhiteSpace(projectPicturesFolderPath)) throw new Exception("ID: 3projectPicturesFolderPath not specified");
            _backendDatabaseFolderPath = MakeSureTheDirectoryIsThereAsync(databaseFileFolderPath, true).Result;
            _backendPhotoFolderPath = MakeSureTheDirectoryIsThereAsync(projectPicturesFolderPath, true).Result;
        }
        public CSVDataAccess(string backendProjectTextFilePath, string backendTimeUniTextFilePath, string backendDatabaseFolderPath, string backendendPhotoFolderPath, string frontendProjectTextFilePath, string frontendTimeUnitTextFilePath, string frontendDatabaseFolderPath, string frontendPhotoFolderPath, string backupProjectTextFilePath, string backupTimeUnitTextFilePath, string backupDatabaseFolderPath, string backupPhotoFolderPath)
        {
            _backendProjectTextFilePath = backendProjectTextFilePath;
            _backendTimeUniTextFilePath = backendTimeUniTextFilePath;

            if (string.IsNullOrWhiteSpace(backendDatabaseFolderPath)) throw new Exception("ID: 4 backendDatabaseFolderPath not specified");
            _backendDatabaseFolderPath = MakeSureTheDirectoryIsThereAsync(backendDatabaseFolderPath, true).Result;

            if (string.IsNullOrEmpty(backendendPhotoFolderPath) == false)
            {
                if (string.IsNullOrWhiteSpace(backendendPhotoFolderPath)) throw new Exception("ID: 5 backendendPhotoFolderPath not specified");
                _backendPhotoFolderPath = Task.Run(() => MakeSureTheDirectoryIsThereAsync(backendendPhotoFolderPath, true)).Result;
            }

            FrontEndEnabled = CheckIfEnabled(frontendProjectTextFilePath);
            BackupEnabled = CheckIfEnabled(backupProjectTextFilePath);

            if (FrontEndEnabled == true)
            {
                _frontendProjectTextFilePath = frontendProjectTextFilePath;
                _frontendTimeUniTextFilePath = frontendTimeUnitTextFilePath;

                if (string.IsNullOrWhiteSpace(frontendDatabaseFolderPath)) throw new Exception("ID: 6 frontendDatabaseFolderPath not specified");
                if (string.IsNullOrWhiteSpace(frontendPhotoFolderPath)) throw new Exception("ID: 7 frontendPhotoFolderPath not specified");
                _frontendDatabaseFolderPath = Task.Run(() => MakeSureTheDirectoryIsThereAsync(frontendDatabaseFolderPath, false)).Result;
                _frontendPhotoFolderPath = Task.Run(() => MakeSureTheDirectoryIsThereAsync(frontendPhotoFolderPath, false)).Result;
            }

            if (BackupEnabled == true)
            {
                _backupProjectTextFilePath = backupProjectTextFilePath;
                _backupTimeUniTextFilePath = backupTimeUnitTextFilePath;

                if (string.IsNullOrWhiteSpace(backupDatabaseFolderPath)) throw new Exception("ID: 8 backupDatabaseFolderPath not specified");
                if (string.IsNullOrWhiteSpace(backupPhotoFolderPath)) throw new Exception("ID: 9 backupPhotoFolderPath not specified");
                _backupDatabaseFolderPath = Task.Run(() => MakeSureTheDirectoryIsThereAsync(backupDatabaseFolderPath, false)).Result;
                _backupPhotoFolderPath = Task.Run(() => MakeSureTheDirectoryIsThereAsync(backupPhotoFolderPath, false)).Result;
            }
        }

        private bool CheckIfEnabled(string projectTextFilePath)
        {
            bool output = false;
            if (string.IsNullOrEmpty(projectTextFilePath) == false)
            {
                output = true;
            }

            return output;
        }
        public string GetFrontEndPhotoFolder()
        {
            return _frontendPhotoFolderPath;
        }
        public (List<ProjectModel> projectList, List<TimeUnitModel> timeUnitsList) ReadAllRecords(string mainGoal)
        {
            List<ProjectModel> projectsList = new List<ProjectModel>();
            List<TimeUnitModel> timeUnitsList = new List<TimeUnitModel>();

            projectsList = ReadAllProjectRecords(mainGoal);
            timeUnitsList = ReadAllTimeUnits(mainGoal);

            return (projectsList, timeUnitsList);
        }

        private void MakeSureThereIsAnEntry<T>(List<T> entryList, string mainGoal)
        {
            if (entryList is List<TimeUnitModel> && entryList.Count == 0)
            {
                CreateFirstTimeUnitEntry(mainGoal);
            }
            if (entryList is List<ProjectModel> && entryList.Count == 0)
            {
                CreateFirstProjectEntry(mainGoal);
            }
        }

        public void MakeFirstEntry(string mainGoal)
        {
            CreateFirstProjectEntry(mainGoal);
            CreateFirstTimeUnitEntry(mainGoal);
        }

        private void CreateFirstTimeUnitEntry(string mainGoal)
        {
            // Setting up all needed parameters
            List<TimeUnitModel> emptyList = new List<TimeUnitModel>();
            TimeUnitModel first = new TimeUnitModel();
            List<ProjectModel> allProjects = ReadAllProjectRecords(mainGoal);
            ProjectModel mainProject = GetProjectByTitle(mainGoal, allProjects);

            // Set first timeunit details
            first.SetProjectTitle(mainGoal);
            first.SetClassification(HourClassification.General);
            first.SetHoursPutIn("1");
            first.SetProjectId(mainProject.ProjectId.ToString());

            // add time to the project
            mainProject.AddHoursToProject(first);
            SaveProject(mainProject, allProjects);

            SaveTimeUnit(first, emptyList);
        }
        private void CreateFirstProjectEntry(string mainGoal)
        {
            List<ProjectModel> emptyList = new List<ProjectModel>();
            ProjectModel mainProject = new ProjectModel();
            mainProject.Title = mainGoal;
            mainProject.ShowProgressBar = false;

            SaveProject(mainProject, emptyList);
        }
        public void SaveTimeUnit(TimeUnitModel timeUnitToSave, List<TimeUnitModel> timeUnits)
        {
            try
            {
                TimeUnitModel toRemove = timeUnits.Where(x => x.TimeUnitId == timeUnitToSave.TimeUnitId).First();
                timeUnits.Remove(toRemove);
            }
            catch (System.InvalidOperationException)
            {
            }

            timeUnits.Add(timeUnitToSave);
            SaveAllTimeUnits(timeUnits);
        }
        // ToDo check if project to save is needed it might be changed in the list all ready
        public void SaveProject(ProjectModel projectToSave, List<ProjectModel> allProjects)
        {
            try
            {
                // Get old project
                ProjectModel oldProject = allProjects.Where(x => x.ProjectId == projectToSave.ProjectId).First();

                // Get old main project
                string oldMainProjectIdString = oldProject.MainProjectId.ToString();
                Guid mainProjectId = new Guid(oldMainProjectIdString);
                ProjectModel oldMainProject = GetProjectById(mainProjectId, allProjects);

                if (oldProject.MainProjectId != projectToSave.MainProjectId && projectToSave.MainProjectId != null)
                {
                    // Remove subproject id from main project
                    oldMainProject.SubProjectIds.Remove(oldProject.ProjectId);
                    //List<Nullable<Guid>> mainProjectSubprojectIds = GetProjectById(mainProjectId, allProjects).SubProjectIds;

                    //Get old governing projectIds
                    List<Guid> oldGoverningProjectIds = GetAllGoverningProjectIds(oldMainProjectIdString, allProjects);

                    // subtract hours from old governing projects
                    foreach (Guid guid in oldGoverningProjectIds)
                    {
                        ProjectModel oldGoverningProject = GetProjectById(guid, allProjects);

                        // Get all hours fom project
                        double oldMainTotalHours = oldGoverningProject.TotalHours;
                        double oldMainPracticalHours = oldGoverningProject.PracticalHours;
                        double oldMainTheoryHours = oldGoverningProject.TheoreticalHours;
                        double oldMainGeneralHours = oldGoverningProject.GeneralHours;

                        // Remove hours from main project
                        oldMainTotalHours = oldMainTotalHours - projectToSave.TotalHours;
                        oldMainPracticalHours = oldMainPracticalHours - projectToSave.PracticalHours;
                        oldMainTheoryHours = oldMainTheoryHours - projectToSave.TheoreticalHours;
                        oldMainGeneralHours = oldMainGeneralHours - projectToSave.GeneralHours;

                        if (oldMainTotalHours < 0 ||
                            oldMainPracticalHours < 0 ||
                            oldMainTheoryHours < 0 ||
                            oldMainGeneralHours < 0)
                        {
                            throw new Exception($"Tot{oldMainTotalHours} " +
                                                $"Pra{oldMainPracticalHours}" +
                                                $"The{oldMainTheoryHours}" +
                                                $"Gen{oldMainGeneralHours}");
                        }

                        // Set new hour values for project
                        oldGoverningProject.SetTotalHours(oldMainTotalHours.ToString());
                        oldGoverningProject.SetPracticalHours(oldMainPracticalHours.ToString());
                        oldGoverningProject.SetTheoreticalHours(oldMainTheoryHours.ToString());
                        oldGoverningProject.SetGeneralHours(oldMainGeneralHours.ToString());

                        if (oldGoverningProject.TotalHours == 0)
                        {
                            oldGoverningProject.StopProject();
                        }

                        CalculateProjectProgress(oldGoverningProject, allProjects);

                        if (oldGoverningProject.TotalHours == 0)
                        {
                            oldGoverningProject.SetProjectStatus("0");
                        }
                    }

                    // Get new Main project
                    string newMainProjectIdString = projectToSave.MainProjectId.ToString();
                    Guid newMainProjectId = new Guid(newMainProjectIdString);
                    ProjectModel newMainProject = GetProjectById(newMainProjectId, allProjects);

                    // Add project id to new Main project
                    newMainProject.AddSubProject(projectToSave.ProjectId.ToString());

                    //Get governing projectIds
                    List<Guid> newGoverningProjectIds = GetAllGoverningProjectIds(newMainProjectIdString, allProjects);

                    foreach (Guid guid in newGoverningProjectIds)
                    {
                        ProjectModel newGoverningProject = GetProjectById(guid, allProjects);


                        // Get all hours fom project
                        double newMainProjectTotalHours = newGoverningProject.TotalHours;
                        double newMainProjectTheoreticalHours = newGoverningProject.TheoreticalHours;
                        double newMainProjectPracticalHours = newGoverningProject.PracticalHours;
                        double newMainProjectGeneralHours = newGoverningProject.GeneralHours;

                        // Add hours to the project
                        newMainProjectTotalHours = newMainProjectTotalHours + projectToSave.TotalHours;
                        newMainProjectTheoreticalHours = newMainProjectTheoreticalHours + projectToSave.TheoreticalHours;
                        newMainProjectPracticalHours = newMainProjectPracticalHours + projectToSave.PracticalHours;
                        newMainProjectGeneralHours = newMainProjectGeneralHours + projectToSave.GeneralHours;

                        // Set new hour values for project
                        newGoverningProject.SetTotalHours(newMainProjectTotalHours.ToString());
                        newGoverningProject.SetTheoreticalHours(newMainProjectTheoreticalHours.ToString());
                        newGoverningProject.SetPracticalHours(newMainProjectPracticalHours.ToString());
                        newGoverningProject.SetGeneralHours(newMainProjectGeneralHours.ToString());

                        if (newGoverningProject.TotalHours > 0)
                        {
                            newGoverningProject.StartProject();
                        }

                        CalculateProjectProgress(newGoverningProject, allProjects);

                    }
                }

                // Remove project fom list
                allProjects.Remove(oldProject);
            }
            catch (System.InvalidOperationException)
            {
            }

            allProjects.Add(projectToSave);
            SaveAllProjects(allProjects);
        }
        public ProjectModel GetProjectById(Guid projectId, List<ProjectModel> projectsList)
        {

            if (projectId.ToString() == "00000000-0000-0000-0000-000000000000")
            {
                ProjectModel emptyModel = new ProjectModel();
                return emptyModel;
            }


            ProjectModel projectToFind = projectsList.Where(x => x.ProjectId == projectId).First();

            return projectToFind;
        }
        public void SaveAllRecords(List<ProjectModel> projects, List<TimeUnitModel> timeUnits)
        {
            SaveAllProjects(projects);
            SaveAllTimeUnits(timeUnits);
        }
        public List<ProjectModel> ReadAllProjectRecords(string mainGoal)
        {
            List<ProjectModel> output = new List<ProjectModel>();

            if (File.Exists(_backendProjectTextFilePath))
            {
                output = GetAllProjectRecords();

                MakeSureThereIsAnEntry(output, mainGoal);
            }

            else
            {
                try
                {
                    using (File.Create(_backendProjectTextFilePath))
                    {
                    }

                }
                catch (DirectoryNotFoundException)
                {
                    MakeSureTheDirectoryIsThereAsync(_backendDatabaseFolderPath, true);

                    using (File.Create(_backendProjectTextFilePath))
                    {
                    }
                }

                CreateFirstProjectEntry(mainGoal);

                output = GetAllProjectRecords();


            }
            return output;
        }
        private async Task<string> MakeSureTheDirectoryIsThereAsync(string filePath, bool isBackend)
        {
            if (isBackend == true)
            {
                await Task.Run(() => Directory.CreateDirectory(filePath));
            }
            if (isBackend == false)
            {
                Directory.CreateDirectory(filePath);
            }

            return filePath;
        }
        private List<ProjectModel> GetAllProjectRecords()
        {
            List<ProjectModel> output = new List<ProjectModel>();
            var lines = File.ReadAllLines(_backendProjectTextFilePath);
            lines = lines.Skip(1).ToArray();

            foreach (var line in lines)
            {

                ProjectModel project = new ProjectModel();
                var values = line.Split(propertyDilimiter);

                if (values.Length != 30)
                {
                    if (line != "")
                    {
                        throw new Exception($"Invalid row of data (Your file might be corrupted or empty){line}");
                    }
                }

                project.Title = values[0];
                project.SetProjectId(values[1]);
                project.SetMainProjectId(new Guid(values[2]));
                project.ShortDescription = values[3];
                project.Outcome = values[4];
                project.DeveloperName = values[5];
                project.DemoLink = values[6];
                project.GitHubLink = values[7];
                project.ShowItem = bool.Parse(values[8]);
                project.ShowProgressBar = bool.Parse(values[9]);
                project.PriorityCalculation = bool.Parse(values[10]);
                project.HasDemo = bool.Parse(values[11]);
                project.SetPriorityNumber(values[12]);
                project.Impact = int.Parse(values[13]);
                project.Urgency = int.Parse(values[14]);
                project.EaseOffSuccess = int.Parse(values[15]);
                project.PersonalPreference = int.Parse(values[16]);
                project.SetProgress(values[17]);
                project.SetTotalHours(values[18]);
                project.SetTheoreticalHours(values[19]);
                project.SetPracticalHours(values[20]);
                project.SetGeneralHours(values[21]);
                project.SetDateCreated(values[22]);
                project.SetDateDoing(values[23]);
                project.SetDateDone(values[24]);
                project.SetProjectStatus(values[25]);

                string listOfChallenges = values[26];
                string[] challenges = listOfChallenges.Split(DictionaryItemDelimiter);

                foreach (string challenge in challenges)
                {
                    string[] challengeSolutionArray = challenge.Split(DictionaryKeyDelimiter);

                    if (string.IsNullOrEmpty(challengeSolutionArray[0]) == false)
                    {

                        //string challengeTitle = challengeSolutionArray[0];
                        //string[] solutionArray = challengeSolutionArray[1].Split(listItemDelimiter);
                        //List<string> groomedSolutionList = solutionArray.Skip(1).ToList();

                        (string challengeTitle, List<string> groomedSolutionList) = PrepareDictionary(challengeSolutionArray);
                        project.AddChallenge(challengeTitle, groomedSolutionList);
                    }
                }
                string listOfAdditions = values[27];
                string[] additions = listOfAdditions.Split(DictionaryItemDelimiter);

                foreach (string addition in additions)
                {
                    string[] additionRunwayArray = addition.Split(DictionaryKeyDelimiter);
                    if (string.IsNullOrEmpty(additions[0]) == false)
                    {

                        //string additionTitle = additionRunwayArray[0];
                        //string[] runwayArray = additionRunwayArray[1].Split(listItemDelimiter);
                        //List<string> groomedSolutionList = runwayArray.Skip(1).ToList();

                        (string additionTitle, List<string> groomedSolutionList) = PrepareDictionary(additionRunwayArray);

                        project.AddFutureAddition(additionTitle, groomedSolutionList);
                    }
                }

                string listOfTimeUnitIds = values[28];
                string[] timeUnitIds = listOfTimeUnitIds.Split(listItemDelimiter);

                bool hasTimeUnits = timeUnitIds[0].ToString().ValidateStringHasContent();

                if (hasTimeUnits == true)
                {
                    foreach (var timeUnitId in timeUnitIds)
                    {
                        project.AddTimeUnitIdToProject(timeUnitId);
                    }
                }

                var listOfSubProjectIds = values[29];
                var subProjectIds = listOfSubProjectIds.Split(listItemDelimiter);

                bool hasSubProjects = subProjectIds[0].ToString().ValidateStringHasContent();

                if (hasSubProjects == true)
                {
                    foreach (var subProjectId in subProjectIds)
                    {
                        project.AddSubProject(subProjectId);
                    }
                }

                project.CalculatePriority();
                output.Add(project);
            }
            return output;
        }

        private (string challengeTitle, List<string> groomedSolutionList) PrepareDictionary(string[] challengeSolutionArray)
        {
            // Create output vaiables
            string outputTitle = "";
            List<string> outputList;

            // Get the output title
            outputTitle = challengeSolutionArray[0];

            // Get the output list
            string[] solutionArray = challengeSolutionArray[1].Split(listItemDelimiter);
            outputList = solutionArray.Skip(1).ToList();

            // Order the list
            outputList = outputList.OrderBy(x => x).ToList();


            return (outputTitle, outputList);
        }

        public List<TimeUnitModel> ReadAllTimeUnits(string mainGoal)
        {
            List<TimeUnitModel> output = new List<TimeUnitModel>();

            if (File.Exists(_backendTimeUniTextFilePath))
            {
                output = GetAllTimeUnitRecords();

                MakeSureThereIsAnEntry(output, mainGoal);
            }
            else
            {
                using (File.Create(_backendTimeUniTextFilePath))
                {
                }

                CreateFirstTimeUnitEntry(mainGoal);
            }


            return output;
        }
        private List<TimeUnitModel> GetAllTimeUnitRecords()
        {
            List<TimeUnitModel> output = new List<TimeUnitModel>();
            var lines = File.ReadAllLines(_backendTimeUniTextFilePath);
            lines = lines.Skip(1).ToArray();

            foreach (var line in lines)
            {
                TimeUnitModel timeUnit = new TimeUnitModel();
                var values = line.Split(propertyDilimiter);

                if (values.Length != 6)
                {
                    throw new Exception($"Invalid row of data {line}");
                }
                timeUnit.SetProjectTitle(values[0]);
                timeUnit.SetTimeUnitId(values[1]);
                timeUnit.SetHoursPutIn(values[2]);
                timeUnit.SetClassification(values[3]);
                timeUnit.SetProjectId(values[4]);
                timeUnit.SetTimeStamp(values[5]);
                output.Add(timeUnit);

            }
            return output;
        }
        public void SaveAllTimeUnits(List<TimeUnitModel> timeUnits)
        {
            timeUnits = timeUnits.OrderByDescending(x => x.TimeStamp).ToList();

            List<string> timeUnitRecords = new List<string>();
            TimeUnitModel example = new TimeUnitModel();
            timeUnitRecords.Add($"{nameof(example.ProjectTitle)}{propertyDilimiter}" +
                                $"{nameof(example.TimeUnitId)}{propertyDilimiter}" +
                                $"{nameof(example.HoursPutIn)}{propertyDilimiter}" +
                                $"{nameof(example.Classification)}{propertyDilimiter}" +
                                $"{nameof(example.ProjectId)}{propertyDilimiter}" +
                                $"{nameof(example.TimeStamp)}{propertyDilimiter}");

            foreach (var timeUnit in timeUnits)
            {
                string stringTimeStamp = CreateSavableTimeString(timeUnit.TimeStamp);

                timeUnitRecords.Add($"{timeUnit.ProjectTitle}{propertyDilimiter}" +
                                    $"{timeUnit.TimeUnitId}{propertyDilimiter}" +
                                    $"{timeUnit.HoursPutIn}{propertyDilimiter}" +
                                    $"{timeUnit.GetClassification(timeUnit.Classification)}{propertyDilimiter}" +
                                    $"{timeUnit.ProjectId}{propertyDilimiter}" +
                                    stringTimeStamp);
            }

            File.WriteAllLines(_backendTimeUniTextFilePath, timeUnitRecords);

            if (FrontEndEnabled == true)
            {
                File.WriteAllLines(_frontendTimeUniTextFilePath, timeUnitRecords);
                // ToDo make async
                //WriteToSecodaryLocationAsync(_frontendTimeUniTextFilePath, timeUnitRecords);
            }

            if (BackupEnabled == true)
            {
                File.WriteAllLines(_backupTimeUniTextFilePath, timeUnitRecords);

                // ToDo make async
                //WriteToSecodaryLocationAsync(_backupTimeUniTextFilePath, timeUnitRecords);
            }

        }
        public void SaveAllProjects(List<ProjectModel> projects)
        {
            projects = projects.OrderBy(x => x.Title).ToList();

            List<string> projectRecords = new List<string>();
            ProjectModel example = new ProjectModel();
            projectRecords.Add($"{nameof(example.Title)}{propertyDilimiter}" +
                               $"{nameof(example.ProjectId)}{propertyDilimiter}" +
                               $"{nameof(example.MainProjectId)}{propertyDilimiter}" +
                               $"{nameof(example.ShortDescription)}{propertyDilimiter}" +
                               $"{nameof(example.Outcome)}{propertyDilimiter}" +
                               $"{nameof(example.DeveloperName)}{propertyDilimiter}" +
                               $"{nameof(example.DemoLink)}{propertyDilimiter}" +
                               $"{nameof(example.GitHubLink)}{propertyDilimiter}" +
                               $"{nameof(example.ShowItem)}{propertyDilimiter}" +
                               $"{nameof(example.ShowProgressBar)}{propertyDilimiter}" +
                               $"{nameof(example.PriorityCalculation)}{propertyDilimiter}" +
                               $"{nameof(example.HasDemo)}{propertyDilimiter}" +
                               $"{nameof(example.PriorityNumber)}{propertyDilimiter}" +
                               $"{nameof(example.Impact)}{propertyDilimiter}" +
                               $"{nameof(example.Urgency)}{propertyDilimiter}" +
                               $"{nameof(example.EaseOffSuccess)}{propertyDilimiter}" +
                               $"{nameof(example.PersonalPreference)}{propertyDilimiter}" +
                               $"{nameof(example.Progress)}{propertyDilimiter}" +
                               $"{nameof(example.TotalHours)}{propertyDilimiter}" +
                               $"{nameof(example.TheoreticalHours)}{propertyDilimiter}" +
                               $"{nameof(example.PracticalHours)}{propertyDilimiter}" +
                               $"{nameof(example.GeneralHours)}{propertyDilimiter}" +
                               $"{nameof(example.DateCreated)}{propertyDilimiter}" +
                               $"{nameof(example.DateDoing)}{propertyDilimiter}" +
                               $"{nameof(example.DateDone)}{propertyDilimiter}" +
                               $"{nameof(example.ProjectStatus)}{propertyDilimiter}" +
                               $"{nameof(example.Challenges)}{propertyDilimiter}" +
                               $"{nameof(example.FutureAdditions)}{propertyDilimiter}" +
                               $"{nameof(example.TimeUnitsPutIn)}{propertyDilimiter}" +
                               $"{nameof(example.SubProjectIds)}");

            foreach (ProjectModel project in projects)
            {
                string timeUnitIds = string.Join(listItemDelimiter, project.TimeUnitsPutIn);
                string subProjectIds = string.Join(listItemDelimiter, project.SubProjectIds);
                string projectChalengesAndSolutions = MakeStringFromDictionary(project.Challenges);
                string projectFutureAdditionsAndRunway = MakeStringFromDictionary(project.FutureAdditions);
                string dateCreated = CreateSavableTimeString(project.DateCreated);
                string dateDoing = CreateSavableTimeString(project.DateDoing);
                string dateDone = CreateSavableTimeString(project.DateDone);

                project.CalculatePriority();

                projectRecords.Add($"{project.Title}{propertyDilimiter}" +
                                    $"{project.ProjectId}{propertyDilimiter}" +
                                    $"{project.MainProjectId}{propertyDilimiter}" +
                                    $"{project.ShortDescription}{propertyDilimiter}" +
                                    $"{project.Outcome}{propertyDilimiter}" +
                                    $"{project.DeveloperName}{propertyDilimiter}" +
                                    $"{project.DemoLink}{propertyDilimiter}" +
                                    $"{project.GitHubLink}{propertyDilimiter}" +
                                    $"{project.ShowItem}{propertyDilimiter}" +
                                    $"{project.ShowProgressBar}{propertyDilimiter}" +
                                    $"{project.PriorityCalculation}{propertyDilimiter}" +
                                    $"{project.HasDemo}{propertyDilimiter}" +
                                    $"{project.PriorityNumber}{propertyDilimiter}" +
                                    $"{project.Impact}{propertyDilimiter}" +
                                    $"{project.Urgency}{propertyDilimiter}" +
                                    $"{project.EaseOffSuccess}{propertyDilimiter}" +
                                    $"{project.PersonalPreference}{propertyDilimiter}" +
                                    $"{project.Progress}{propertyDilimiter}" +
                                    $"{project.TotalHours}{propertyDilimiter}" +
                                    $"{project.TheoreticalHours}{propertyDilimiter}" +
                                    $"{project.PracticalHours}{propertyDilimiter}" +
                                    $"{project.GeneralHours}{propertyDilimiter}" +
                                    $"{dateCreated}{propertyDilimiter}" +
                                    $"{dateDoing}{propertyDilimiter}" +
                                    $"{dateDone}{propertyDilimiter}" +
                                    $"{project.GetProjectSatusInString()}{propertyDilimiter}" +
                                    $"{projectChalengesAndSolutions}{propertyDilimiter}" +
                                    $"{projectFutureAdditionsAndRunway}{propertyDilimiter}" +
                                    $"{timeUnitIds}{propertyDilimiter}" +
                                    $"{subProjectIds}");

            }



            File.WriteAllLines(_backendProjectTextFilePath, projectRecords);

            if (FrontEndEnabled == true)
            {
                File.WriteAllLines(_frontendProjectTextFilePath, projectRecords);

                // ToDo make async
                //WriteToSecodaryLocationAsync(_frontendProjectTextFilePath, projectRecords);               
            }

            if (BackupEnabled == true)
            {
                File.WriteAllLines(_backupProjectTextFilePath, projectRecords);

                // ToDo make Async
                //WriteToSecodaryLocationAsync(_backupProjectTextFilePath, projectRecords);               
            }
        }
        private async Task WriteToSecodaryLocationAsync(string backupProjectTextFilePath, List<string> projectRecords)
        {
            await Task.Run(() => File.WriteAllLines(_frontendProjectTextFilePath, projectRecords));
        }
        private string MakeStringFromDictionary(Dictionary<string, List<string>> dictionaries)
        {

            dictionaries = ChangeOrder(dictionaries);

            string output = "";

            foreach (var dictionary in dictionaries)
            {
                if (string.IsNullOrEmpty(output) == false)
                {
                    output = output + DictionaryItemDelimiter + dictionary.Key + DictionaryKeyDelimiter;
                }
                if (string.IsNullOrEmpty(output) == true)
                {
                    output = dictionary.Key + DictionaryKeyDelimiter;
                }

                foreach (string solution in dictionary.Value)
                {
                    output = output + listItemDelimiter + solution;
                }

            }

            return output;
        }

        private Dictionary<string, List<string>> ChangeOrder(Dictionary<string, List<string>> dictionaries)
        {
            Dictionary<string, List<string>> outputDictionary = new Dictionary<string, List<string>>();

            var keysList = dictionaries.Keys.ToList();
            keysList = keysList.OrderBy(x => x).ToList();


            foreach (var key in keysList)
            {
                List<string> values = dictionaries.Where(x => x.Key == key).Select(x => x.Value).First();
                outputDictionary.Add(key, values);
            }



            return outputDictionary;
        }

        public ProjectModel GetProjectByTitle(string projectTitle, List<ProjectModel> projectsList)
        {
            ProjectModel projectToFind = new();

            if (projectsList is not null)
            {
                List<string> projectTitles = projectsList.Select(x => x.Title).ToList();
                if(projectTitles.Contains(projectTitle)) projectToFind = projectsList.Where(x => x.Title == projectTitle).First();
            }
            return projectToFind;
        }
        public void AddTime(ProjectModel projectToAddHousTo, TimeUnitModel timeUnitToAdd, List<ProjectModel> allProjects, List<TimeUnitModel> allTimeUnits)
        {
            if (projectToAddHousTo.TotalHours == 0)
            {
                projectToAddHousTo.SetProjectStatus("1");
            }
            projectToAddHousTo.AddHoursToProject(timeUnitToAdd);

            List<Guid> listOfGoverningProjectIds = GetAllGoverningProjectIds(projectToAddHousTo.MainProjectId.ToString(), allProjects);

            foreach (Guid guid in listOfGoverningProjectIds)
            {
                ProjectModel governingProject = allProjects.Where(x => x.ProjectId == guid).First();

                if (governingProject.TotalHours == 0)
                {
                    governingProject.SetProjectStatus("1");
                }

                governingProject.AddHoursToProject(timeUnitToAdd);
            }

            SaveTimeUnit(timeUnitToAdd, allTimeUnits);
            SaveAllProjects(allProjects);
        }
        public void RemoveTime(ProjectModel projectToAddHousTo, TimeUnitModel timeUnitToDelete, List<ProjectModel> allProjects, List<TimeUnitModel> allTimeUnits)
        {
            TimeUnitModel unitToDelete = GetTimeUnitById(timeUnitToDelete.TimeUnitId, allTimeUnits);
            ProjectModel projectToSubtractHoursFrom = GetProjectById(unitToDelete.ProjectId, allProjects);

            List<Guid> listOfGoverningProjectIds = GetAllGoverningProjectIds(projectToSubtractHoursFrom.MainProjectId.ToString(), allProjects);

            foreach (Guid guid in listOfGoverningProjectIds)
            {
                // ToDo remo time from project
                ProjectModel governingProject = GetProjectById(guid, allProjects);
                governingProject.SubtractHoursFromProject(unitToDelete);
                if (governingProject.TotalHours == 0)
                {
                    governingProject.SetProjectStatus("0");
                }
            }

            projectToSubtractHoursFrom.SubtractHoursFromProject(unitToDelete);

            if (projectToSubtractHoursFrom.TotalHours == 0)
            {
                projectToSubtractHoursFrom.SetProjectStatus("0");
            }

            CalculateProjectProgress(projectToSubtractHoursFrom, allProjects);

            allTimeUnits.Remove(unitToDelete);

            SaveAllRecords(allProjects, allTimeUnits);
        }
        public void ChangeProjectStatus(string projectTitle, ProjectStatus newStatus, List<ProjectModel> allProjects)
        {

            ProjectModel projectToChange = GetProjectByTitle(projectTitle, allProjects);

            switch (newStatus)
            {
                case ProjectStatus.ToDo:
                    projectToChange.StopProject();
                    break;
                case ProjectStatus.Doing:
                    projectToChange.StartProject();
                    break;
                case ProjectStatus.Done:
                    projectToChange.FinishProject();
                    break;
                default:
                    break;
            }


            CalculateProjectProgress(projectToChange, allProjects);
            SaveAllProjects(allProjects);
        }
        public void DeleteProject(string projectTitle, List<ProjectModel> allProjects, List<TimeUnitModel> allTimeUnits)
        {
            ProjectModel projectToRemove = allProjects.Where(x => x.Title == projectTitle).First();
            ProjectModel mainProject = allProjects.Where(x => x.ProjectId == projectToRemove.MainProjectId).First();

            try
            {
                if (projectToRemove.SubProjectIds.Count == 0 && projectToRemove.MainProjectId.ToString() != "00000000-0000-0000-0000-000000000000")
                {

                    mainProject.SubProjectIds.Remove(projectToRemove.ProjectId);

                    allProjects.Remove(projectToRemove);

                    List<TimeUnitModel> projectTimeUnits = allTimeUnits.Where(x => x.ProjectTitle == projectToRemove.Title).ToList();

                    foreach (TimeUnitModel timeUnit in projectTimeUnits)
                    {
                        allTimeUnits.Remove(timeUnit);
                    }
                }
            }
            catch (System.InvalidOperationException)
            {
                throw new Exception("no sutch record");
            }

            List<Guid> governingProjectIds = GetAllGoverningProjectIds(mainProject.ProjectId.ToString(), allProjects);
            foreach (Guid guid in governingProjectIds)
            {
                ProjectModel governingProject = allProjects.Where(x => x.ProjectId == guid).First();

                string newTotalHours = SubtractHours(governingProject.TotalHours, projectToRemove.TotalHours);
                string newTheoreticalHours = SubtractHours(governingProject.TheoreticalHours, projectToRemove.TheoreticalHours);
                string newPracticalHours = SubtractHours(governingProject.PracticalHours, projectToRemove.PracticalHours);
                string newGeneralHours = SubtractHours(governingProject.GeneralHours, projectToRemove.GeneralHours);

                governingProject.SetTotalHours(newTotalHours);
                governingProject.SetTheoreticalHours(newTheoreticalHours);
                governingProject.SetPracticalHours(newPracticalHours);
                governingProject.SetGeneralHours(newGeneralHours);
            }

            SaveAllProjects(allProjects);
            SaveAllTimeUnits(allTimeUnits);

        }
        private string SubtractHours(double governingHours, double removeHours)
        {
            string output = "";

            double calculated = governingHours - removeHours;

            if (calculated < 0 || calculated == null)
            {
                throw new Exception("the hous are negative or null in the hour subtraction while removing this project");
            }

            output = calculated.ToString();

            return output;
        }
        public void CalculateProjectProgress(ProjectModel projectToCalculate, List<ProjectModel> allProjects)
        {

            List<ProjectStatus> subProjectStatusList = allProjects.Where(X => X.MainProjectId == projectToCalculate.ProjectId).Select(x => x.ProjectStatus).ToList();

            CalculateProgressPercentage(projectToCalculate, subProjectStatusList);

            List<Guid> listOfGoverningProjectIds = GetAllGoverningProjectIds(projectToCalculate.MainProjectId.ToString(), allProjects);

            foreach (Guid guid in listOfGoverningProjectIds)
            {
                ProjectModel governingProject = allProjects.Where(x => x.ProjectId == guid).First();

                subProjectStatusList = allProjects.Where(X => X.MainProjectId == governingProject.ProjectId).Select(x => x.ProjectStatus).ToList();


                CalculateProgressPercentage(governingProject, subProjectStatusList);

                CalculateProjectProgress(governingProject, allProjects);
            }
        }
        private void CalculateProgressPercentage(ProjectModel projectToCalculate, List<ProjectStatus> subProjectStatusList)
        {

            //double projectProgress = 0;

            double finishedPercentage = 0;
            ProjectStatus subProjectStatus = ProjectStatus.ToDo;


            if (subProjectStatusList.Count == 0)
            {
                ChangeProjectProgress(projectToCalculate);
            }

            if (subProjectStatusList.Count > 0)
            {
                int projectPercentage = 100 / subProjectStatusList.Count;

                (subProjectStatus, finishedPercentage) = CheckSubprojectsForProgress(subProjectStatusList, projectPercentage);


                if (projectPercentage * subProjectStatusList.Count == finishedPercentage)
                {
                    finishedPercentage = 100;

                    projectToCalculate.FinishProject();

                }


                if (finishedPercentage >= 0 && finishedPercentage < 100 && subProjectStatus == ProjectStatus.Doing && projectToCalculate.ProjectStatus == ProjectStatus.ToDo)
                {
                    projectToCalculate.StartProject();
                }

                if (subProjectStatus == ProjectStatus.ToDo)
                {
                    projectToCalculate.StopProject();
                }

                if (subProjectStatus == ProjectStatus.Doing && projectToCalculate.ProjectStatus == ProjectStatus.Done)
                {
                    projectToCalculate.StartProject();
                }

                projectToCalculate.SetProgress(finishedPercentage.ToString());
            }

        }
        private void ChangeProjectProgress(ProjectModel projectToCalculate)
        {
            switch (projectToCalculate.ProjectStatus)
            {
                case ProjectStatus.ToDo:
                    projectToCalculate.SetProgress("0");
                    break;
                case ProjectStatus.Doing:
                    projectToCalculate.SetProgress("50");
                    break;
                case ProjectStatus.Done:
                    projectToCalculate.SetProgress("100");
                    break;
                default:
                    break;
            }
        }
        private (ProjectStatus, double) CheckSubprojectsForProgress(List<ProjectStatus> subProjectStatusList, int projectPercentage)
        {
            double outputPercentage = 0;
            ProjectStatus outputStatus = ProjectStatus.ToDo;

            int projectsToDo = 0;
            int projectsDoing = 0;
            int projectsDone = 0;

            foreach (ProjectStatus projectStatus in subProjectStatusList)
            {
                switch (projectStatus)
                {
                    case ProjectStatus.ToDo:
                        projectsToDo = projectsToDo + 1;
                        break;
                    case ProjectStatus.Doing:
                        projectsDoing = projectsDoing + 1;
                        break;
                    case ProjectStatus.Done:
                        outputPercentage = outputPercentage + projectPercentage;
                        projectsDone = projectsDone + 1;
                        break;
                    default:
                        break;
                }

            }

            outputStatus = DecideProjectStatus(projectsToDo, projectsDoing, projectsDone, subProjectStatusList.Count);


            return (outputStatus, outputPercentage);
        }
        private ProjectStatus DecideProjectStatus(int projectsToDo, int projectsDoing, int projectsDone, int subProjectCount)
        {
            ProjectStatus outputStatus = ProjectStatus.ToDo;

            if (projectsDone > 0 && projectsDone < subProjectCount)
            {
                return ProjectStatus.Doing;
            }

            if (projectsDoing > 0)
            {
                return ProjectStatus.Doing;
            }

            if (projectsDone == subProjectCount)
            {
                return ProjectStatus.Done;
            }

            return outputStatus;
        }
        public Nullable<Guid> FindProblemId(string mainProjectTitle, List<Nullable<Guid>> subProjectIds, List<ProjectModel> allProjects)
        {
            ProjectModel mainProject = GetProjectByTitle(mainProjectTitle, allProjects);

            List<Guid> governingProjectIds = GetAllGoverningProjectIds(mainProject.ProjectId.ToString(), allProjects);

            // Create a list of guids is the subproject count is bigger than 0
            List<Guid> subProjectGuids = new List<Guid>();
            if (subProjectIds.Count > 0)
            {
                subProjectGuids = CreateGuidListFromNullableGuid(subProjectIds);
            }

            List<Guid> allSubProjectIds = GetAllSubprojectIds(subProjectGuids, allProjects);

            Nullable<Guid> problemGuid = null;

            //bool isCircularReference = false;

            foreach (Guid governingId in governingProjectIds)
            {
                foreach (Guid subProjectId in allSubProjectIds)
                {
                    if (governingId == subProjectId)
                    {
                        //isCircularReference = true;
                        problemGuid = governingId;
                    }
                }
            }
            return problemGuid;
        }
        private List<Guid> CreateGuidListFromNullableGuid(List<Nullable<Guid>> subProjectIds)
        {
            List<Guid> outputGuids = new List<Guid>();

            foreach (Nullable<Guid> guid in subProjectIds)
            {
                Guid guidToAdd = new Guid(guid.ToString());
                outputGuids.Add(guidToAdd);
            }

            return outputGuids;
        }
        private List<Guid> GetAllGoverningProjectIds(string mainProjectId, List<ProjectModel> allProjects)
        {
            int counter = 0;
            List<Guid> output = new List<Guid>();

            while (mainProjectId != "00000000-0000-0000-0000-000000000000")
            {
                counter = counter + 1;
                if (counter == 1000)
                {
                    Guid projectGuid = new Guid(mainProjectId);
                    string projectTitle = GetProjectById(projectGuid, allProjects).Title;
                    throw new Exception($"There is a circular pointing in the project tree of project {projectTitle}");
                }

                output.Add(new Guid(mainProjectId));

                ProjectModel governingProject = allProjects.Where(x => x.ProjectId == output.Last()).Last();

                mainProjectId = governingProject.MainProjectId.ToString();
            }

            return output;
        }
        private List<Guid> GetAllSubprojectIds(List<Guid> subProjectIds, List<ProjectModel> allProjects)
        {
            List<Guid> relevantProjectIds = new List<Guid>();

            // Create a list of guids for the next level down
            List<Guid> nextLevelGuids = new List<Guid>();

            // Add base ids
            foreach (Guid subProjectId in subProjectIds)
            {
                relevantProjectIds.Add(subProjectId);
            }

            // Create exit bool for do while loop
            bool moreSubprojects = true;

            // Create loop counter
            int counter = 0;

            do
            {
                // Get all the next level down guids
                nextLevelGuids = GetAllSubSubProjectids(subProjectIds, allProjects);

                // Check for availability of more sub projects
                moreSubprojects = CheckForSubprojects(nextLevelGuids);


                subProjectIds = nextLevelGuids;

                foreach (Guid guid in nextLevelGuids)
                {
                    relevantProjectIds.Add(guid);
                }

                // add one to loop counter
                counter = counter + 1;

                CheckForInfiniteLoop(counter);

            } while (moreSubprojects == true);


            return relevantProjectIds;
        }
        private void CheckForInfiniteLoop(int counter)
        {
            if (counter == 2000)
            {
                throw new Exception("there are 2000 subprojects in this catagory is that right?");
            }
        }
        private bool CheckForSubprojects(List<Guid> nextLevelGuids)
        {
            bool output = true;


            if (nextLevelGuids.Count == 0)
            {
                output = false;
            }

            return output;
        }
        public List<Guid> GetSubprojectIds(ProjectModel mainProject)
        {
            List<Guid> outputList = new List<Guid>();
            if (mainProject.SubProjectIds.Count > 0)
            {
                foreach (var guid in mainProject.SubProjectIds)
                {
                    Guid guidToAdd = new Guid(guid.ToString());
                    outputList.Add(guidToAdd);
                }
            }

            return outputList;
        }
        private List<Guid> GetAllSubSubProjectids(List<Guid> subProjectIds, List<ProjectModel> allProjects)
        {
            List<Guid> outputList = new List<Guid>();

            foreach (Guid subProjectId in subProjectIds)
            {
                ProjectModel subProject = GetProjectById(subProjectId, allProjects);

                List<Guid> subSubProjectIds = GetSubprojectIds(subProject);
                foreach (Guid guid in subSubProjectIds)
                {
                    outputList.Add(guid);
                }
            }

            return outputList;
        }
        public TimeUnitModel GetTimeUnitById(Guid timeUnitId, List<TimeUnitModel> allTimeUnits)
        {
            return allTimeUnits.Where(x => x.TimeUnitId == timeUnitId).First();
        }
        public void ClearData()
        {
            throw new NotImplementedException();
        }
        public void ProcessPicture(IFormFile photo, string projectTitle)
        {
            if (string.IsNullOrEmpty(projectTitle) == false)
            {
                string backendFilePath = CreatePhotoPath(_backendPhotoFolderPath, projectTitle);
                string frontendFilePath = "";
                string backupFilePath = "";

                // look for exsisting photo and delete it
                if (photo != null)
                {
                    RemoveExsistingPhoto(backendFilePath);

                    if (FrontEndEnabled == true)
                    {
                        frontendFilePath = CreatePhotoPath(_frontendPhotoFolderPath, projectTitle);
                        RemoveExsistingPhoto(frontendFilePath);
                    }
                    if (BackupEnabled == true)
                    {
                        backupFilePath = CreatePhotoPath(_backupPhotoFolderPath, projectTitle);
                        RemoveExsistingPhoto(backupFilePath);
                    }
                }
                // save the picture
                ProcessUploadedFile(photo, backendFilePath, frontendFilePath, backupFilePath);
            }

        }
        private void RemoveExsistingPhoto(string filePath)
        {
            if (System.IO.File.Exists(filePath) == true)
            {
                System.IO.File.Delete(filePath);
            }
        }
        private void ProcessUploadedFile(IFormFile photo, string backendFilePath, string frontendFilePath, string backupFilePath)
        {
            if (photo != null)
            {
                using (var fileStream = new FileStream(backendFilePath, FileMode.Create))
                {
                    photo.CopyTo(fileStream);
                }
                if (string.IsNullOrEmpty(frontendFilePath) == false)
                {
                    using (var fileStream = new FileStream(frontendFilePath, FileMode.Create))
                    {
                        photo.CopyTo(fileStream);
                    }

                }
                if (string.IsNullOrEmpty(backupFilePath) == false)
                {
                    using (var fileStream = new FileStream(backupFilePath, FileMode.Create))
                    {
                        photo.CopyTo(fileStream);
                    }

                }
            }
        }
        private string CreatePhotoPath(string folderPath, string projectName)
        {
            string outputFilePath = "";

            projectName = ExchangePunctuations(projectName);

            string fileName = $"{projectName}.jpg";

            outputFilePath = @$"{folderPath}{fileName}";

            return outputFilePath;
            //string photoName = $"{ Project.Title }.jpg";
            //BackendPhotoFilePath = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot", "ProjectsPictures", photoName);
            //string frontEndFoldePath = _db.GetFrontEndPhotoFolder();
            //FrontendPhotoFilePath = Path.Combine(frontEndFoldePath, photoName);
        }

        // ToDo Move punctuations stuff to modfications folder
        public string ExchangePunctuations(string projectName)
        {
            List<PunctuationsMarkModel> punctuations = CreatePuntuationmarkList();
            string output = projectName;



            int punctuationsCounter = CountPunctuations(projectName, punctuations);
            int punctuationsReplacedCounter = 0;

            {
                (output, punctuationsReplacedCounter) = ReplacePunctuaions(projectName, punctuationsReplacedCounter, punctuations);
            } while (punctuationsCounter != punctuationsReplacedCounter) ;

            return output;
        }
        private (string projectName, int punctuationsReplacedCounter) ReplacePunctuaions(string projectName, int punctuationsReplacedCounter, List<PunctuationsMarkModel> punctuations)
        {
            int outputCounter = punctuationsReplacedCounter;
            string outputProjectName = projectName;
            string[] oldProjectWords = projectName.Split();
            List<string> newProjectWords = new List<string>();


            foreach (string word in oldProjectWords)
            {
                string newWord = word;
                foreach (PunctuationsMarkModel punctuationMark in punctuations)
                {
                    if (word.Contains(punctuationMark.Punctuationmark))
                    {
                        newWord = word.Replace(punctuationMark.Punctuationmark, $"_{punctuationMark.VerbosePunctuationmark}_");
                        outputCounter = outputCounter + 1;
                    }
                }
                newProjectWords.Add(newWord);
            }


            outputProjectName = string.Join(" ", newProjectWords);



            return (outputProjectName, outputCounter);
        }
        private int CountPunctuations(string projectName, List<PunctuationsMarkModel> punctuations)
        {
            int outputCounter = 0;

            string[] words = projectName.Split();
            foreach (string word in words)
            {
                foreach (PunctuationsMarkModel punctuation in punctuations)
                {
                    if (word.Contains(punctuation.Punctuationmark))
                    {
                        outputCounter = outputCounter + 1;
                    }
                }
            }

            return outputCounter;
        }
        private List<PunctuationsMarkModel> CreatePuntuationmarkList()
        {
            List<PunctuationsMarkModel> outputList = new List<PunctuationsMarkModel> {
            new PunctuationsMarkModel{ Punctuationmark = "~", VerbosePunctuationmark = "tilde" },
            new PunctuationsMarkModel{ Punctuationmark = "`", VerbosePunctuationmark = "backQuote" },
            new PunctuationsMarkModel{ Punctuationmark = "!", VerbosePunctuationmark = "exclamationMark" },
            new PunctuationsMarkModel{ Punctuationmark = "@", VerbosePunctuationmark = "at" },
            new PunctuationsMarkModel{ Punctuationmark = "#", VerbosePunctuationmark = "sharp_pound" },
            new PunctuationsMarkModel{ Punctuationmark = "$", VerbosePunctuationmark = "dollar" },
            new PunctuationsMarkModel{ Punctuationmark = "%", VerbosePunctuationmark = "percent" },
            new PunctuationsMarkModel{ Punctuationmark = "^", VerbosePunctuationmark = "circumflexCaret" },
            new PunctuationsMarkModel{ Punctuationmark = "&", VerbosePunctuationmark = "ampersand" },
            new PunctuationsMarkModel{ Punctuationmark = "*", VerbosePunctuationmark = "asterisk" },
            new PunctuationsMarkModel{ Punctuationmark = "(", VerbosePunctuationmark = "openingRoundBracket" },
            new PunctuationsMarkModel{ Punctuationmark = ")", VerbosePunctuationmark = "closingRoundBracket" },
            new PunctuationsMarkModel{ Punctuationmark = "-", VerbosePunctuationmark = "hyphen" },
            new PunctuationsMarkModel{ Punctuationmark = "_", VerbosePunctuationmark = "underscore" },
            new PunctuationsMarkModel{ Punctuationmark = "=", VerbosePunctuationmark = "equals" },
            new PunctuationsMarkModel{ Punctuationmark = "+", VerbosePunctuationmark = "plus" },
            new PunctuationsMarkModel{ Punctuationmark = "{", VerbosePunctuationmark = "openingMustacheBracket" },
            new PunctuationsMarkModel{ Punctuationmark = "}", VerbosePunctuationmark = "closingMustacheBracket" },
            new PunctuationsMarkModel{ Punctuationmark = "[", VerbosePunctuationmark = "openingSquareBracket" },
            new PunctuationsMarkModel{ Punctuationmark = "]", VerbosePunctuationmark = "closingSquareBracket" },
            new PunctuationsMarkModel{ Punctuationmark = ":", VerbosePunctuationmark = "colon" },
            new PunctuationsMarkModel{ Punctuationmark = ";", VerbosePunctuationmark = "semiColon" },
            new PunctuationsMarkModel{ Punctuationmark = "\"", VerbosePunctuationmark = "doubleQuotes" },
            new PunctuationsMarkModel{ Punctuationmark = "'", VerbosePunctuationmark = "singleQuotes" },
            new PunctuationsMarkModel{ Punctuationmark = "|", VerbosePunctuationmark = "pipe" },
            new PunctuationsMarkModel{ Punctuationmark = @"\", VerbosePunctuationmark = "backslash" },
            new PunctuationsMarkModel{ Punctuationmark = "<", VerbosePunctuationmark = "smallerThen" },
            new PunctuationsMarkModel{ Punctuationmark = ",", VerbosePunctuationmark = "comma" },
            new PunctuationsMarkModel{ Punctuationmark = ">", VerbosePunctuationmark = "biggerThen" },
            new PunctuationsMarkModel{ Punctuationmark = ".", VerbosePunctuationmark = "fullStop" },
            new PunctuationsMarkModel{ Punctuationmark = "?", VerbosePunctuationmark = "questionmark" },
            new PunctuationsMarkModel{ Punctuationmark = "/", VerbosePunctuationmark = "forwardslash" }
            };

            return outputList;

        }

        public List<Guid> CreateGuidListForSubProjects(ProjectModel projectToConvert, List<ProjectModel> allProjects, IDataAccess db)
        {
            List<Guid> outputList = new List<Guid>();
            if (projectToConvert.SubProjectIds.Count > 0)
            {
                foreach (var subProjectId in projectToConvert.SubProjectIds)
                {
                    Guid guidToAdd = ConvertStringToGuid(subProjectId.ToString());
                    ProjectModel subProjectToAdd = db.GetProjectById(guidToAdd, allProjects);
                    if (subProjectToAdd.ShowItem == true)
                    {
                        outputList.Add(guidToAdd);
                    }
                }
            }

            return outputList;
        }

        public string GetMainProjectTitle(ProjectModel projectToConvert, List<ProjectModel> allProjects, IDataAccess db)
        {
            string outputProjectTitle = null;

            if (projectToConvert.MainProjectId != null)
            {
                Guid mainProjectId = ConvertStringToGuid(projectToConvert.MainProjectId.ToString());
                ProjectModel mainProject = db.GetProjectById(mainProjectId, allProjects);

                outputProjectTitle = mainProject.Title;
            }

            return outputProjectTitle;
        }

        public PresentaionProjectModel CreatePresentationProject(ProjectModel projectToConvert, List<ProjectModel> allProjects, IDataAccess db)
        {
            PresentaionProjectModel outputModel = new PresentaionProjectModel();

            // Set title
            outputModel.ProjectTitle = projectToConvert.Title;

            // Set MainProjectTitle
            outputModel.MainProjectTitle = GetMainProjectTitle(projectToConvert, allProjects, db);

            // Set priority number
            outputModel.Priority = projectToConvert.PriorityNumber;

            // Set MainProjectId
            if (projectToConvert.MainProjectId != null)
            {
                outputModel.MainProjectId = new Guid(projectToConvert.MainProjectId.ToString());
            }
            // Set project status
            outputModel.ProjectStatus = projectToConvert.ProjectStatus;

            // Set show progress boolean
            outputModel.ShowProgressBar = projectToConvert.ShowProgressBar;

            // Set progress percentage
            outputModel.ProgressPercentage = projectToConvert.Progress;

            // Set project id
            outputModel.ProjectId = projectToConvert.ProjectId;

            // Set the outcome
            outputModel.Outcome = projectToConvert.Outcome;

            // Set the description
            outputModel.ShortDescription = projectToConvert.ShortDescription;

            // Get all future additions
            outputModel.FutureAdditions = GetAllKeysFromDictionary(projectToConvert.FutureAdditions);

            // Get all challenges
            outputModel.Challenges = GetAllKeysFromDictionary(projectToConvert.Challenges);

            // Create a developernames list
            outputModel.DeveloperNames = CreateDeveloperNamesList(projectToConvert.DeveloperName);

            // Create a list of presentationSubprojectModels
            outputModel.SubprojectPresentationModels = GetPresentationSubProjectList(projectToConvert, allProjects, db);



            return outputModel;

        }

        private List<string> GetAllKeysFromDictionary(Dictionary<string, List<string>> dictionaryCollection)
        {
            List<string> outputList = new List<string>();
            foreach (var dictionary in dictionaryCollection)
            {
                outputList.Add(dictionary.Key);
            }

            return outputList;
        }

        private List<PresentationSubProjectModel> GetPresentationSubProjectList(ProjectModel projectToConvert, List<ProjectModel> allProjects, IDataAccess db)
        {
            List<PresentationSubProjectModel> outputPresentationSubProjectModels = new List<PresentationSubProjectModel>();

            // Get Guids from sub project list
            List<Guid> subProjectIds = CreateGuidListForSubProjects(projectToConvert, allProjects, db);

            // Make PresentationSubProjectModels from subProject is
            outputPresentationSubProjectModels = CreatePresentationSubProjectList(subProjectIds, allProjects, db);

            return outputPresentationSubProjectModels;
        }
        private List<PresentationSubProjectModel> CreatePresentationSubProjectList(List<Guid> subProjectIds, List<ProjectModel> allProjects, IDataAccess db)
        {
            List<PresentationSubProjectModel> outputList = new List<PresentationSubProjectModel>();


            if (subProjectIds.Count > 0)
            {
                foreach (Guid subProjectId in subProjectIds)
                {
                    // Get the project details
                    ProjectModel subProjectToAdd = db.GetProjectById(subProjectId, allProjects);

                    // Create presentation model from project model to add
                    PresentationSubProjectModel modelToAdd = new PresentationSubProjectModel();
                    modelToAdd.Title = subProjectToAdd.Title;
                    modelToAdd.Priority = subProjectToAdd.PriorityNumber;
                    modelToAdd.ShowProgressBar = subProjectToAdd.ShowProgressBar;
                    modelToAdd.ProgressPercentage = subProjectToAdd.Progress;

                    outputList.Add(modelToAdd);
                }
            }

            return outputList;
        }

        public List<TimeUnitModel> GetAllRelevantTimeUnits(ProjectModel mainProject, List<TimeUnitModel> allTimeUnits, List<ProjectModel> allProjects, IDataAccess db)
        {
            // Create a list of relevant project id's           
            List<Guid> relevantProjectIds = CreateRelevantGuidList(mainProject, allProjects, db);

            List<TimeUnitModel> allRelevantTimeUnits = GetTimeUnits(relevantProjectIds, allTimeUnits);

            return allRelevantTimeUnits;
        }
        private List<TimeUnitModel> GetTimeUnits(List<Guid> relevantProjectIds, List<TimeUnitModel> allTimeUnits)
        {
            // Set up list to output
            List<TimeUnitModel> outputList = new List<TimeUnitModel>();

            foreach (Guid guid in relevantProjectIds)
            {
                // Get the timeunits for the project
                List<TimeUnitModel> projectTimeUnits = allTimeUnits.Where(x => x.ProjectId == guid).ToList();

                foreach (TimeUnitModel timeUnitModel in projectTimeUnits)
                {
                    outputList.Add(timeUnitModel);
                }

            }

            return outputList;
        }
        private List<Guid> CreateRelevantGuidList(ProjectModel mainProject, List<ProjectModel> allProjects, IDataAccess db)
        {
            List<Guid> relevantProjectIds = new List<Guid>();

            // if the item can be shown
            if (mainProject.ShowItem == true)
            {
                // Create a list of guids for the next level down
                List<Guid> nextLevelGuids = new List<Guid>();

                // Create a list of sub project guids
                List<Guid> subProjectIds = new List<Guid>();

                // if the item can be shown get the subproject ids to the list
                subProjectIds = CreateGuidListForSubProjects(mainProject, allProjects, db);

                // Add base ids
                relevantProjectIds = GetBaseIds(mainProject.ProjectId, subProjectIds);

                // Create exit bool for loop
                bool moreSubprojects = true;

                // Create loop counter
                int counter = 0;

                do
                {
                    // Get all the next level down guids
                    nextLevelGuids = GetAllSubSubProjectids(subProjectIds, allProjects, db);

                    // Check for availability of more sub projects
                    moreSubprojects = CheckForSubprojects(nextLevelGuids);


                    subProjectIds = nextLevelGuids;

                    foreach (Guid guid in nextLevelGuids)
                    {
                        relevantProjectIds.Add(guid);
                    }

                    // add one to loop counter
                    counter = counter + 1;

                    CheckForInfiniteLoop(counter);

                } while (moreSubprojects == true);
            }

            return relevantProjectIds;
        }
        private List<Guid> GetBaseIds(Guid mainProjectId, List<Guid> subProjectIds)
        {
            List<Guid> outputList = new List<Guid>();

            outputList.Add(mainProjectId);

            foreach (Guid guid in subProjectIds)
            {
                outputList.Add(guid);
            }

            return outputList;
        }
        //private static void CheckForInfiniteLoop(int counter)
        //{
        //    if (counter == 2000)
        //    {
        //        throw new Exception("there are 2000 subprojects in this catagory is that right?");
        //    }
        //}
        //private bool CheckForSubprojects(List<Guid> nextLevelGuids)
        //{
        //    bool output = true;


        //    if (nextLevelGuids.Count == 0)
        //    {
        //        output = false;
        //    }

        //    return output;
        //}
        private List<Guid> GetAllSubSubProjectids(List<Guid> subProjectIds, List<ProjectModel> allProjects, IDataAccess db)
        {
            List<Guid> outputList = new List<Guid>();

            foreach (Guid subProjectId in subProjectIds)
            {
                ProjectModel subProject = db.GetProjectById(subProjectId, allProjects);

                if (subProject.ShowItem == true && subProject.SubProjectIds.Count > 0)
                {
                    List<Guid> subSubProjectIds = CreateGuidListForSubProjects(subProject, allProjects, db);
                    foreach (Guid guid in subSubProjectIds)
                    {
                        outputList.Add(guid);
                    }
                }
            }

            return outputList;
        }

        public List<SolutionModel> LoadAllDictionaries(List<ProjectModel> allProjects, DictionaryClassification classification)
        {
            List<SolutionModel> outputList = new List<SolutionModel>();

            foreach (ProjectModel project in allProjects)
            {
                List<SolutionModel> challengesSolutions = new List<SolutionModel>();
                List<SolutionModel> futureAdditionSolutions = new List<SolutionModel>();

                // Select the right dictionaries
                switch (classification)
                {
                    case DictionaryClassification.Challenges:
                        challengesSolutions = GetDictionaryItems(project.Challenges);
                        break;
                    case DictionaryClassification.FutureAdditions:
                        futureAdditionSolutions = GetDictionaryItems(project.FutureAdditions);
                        break;
                    case DictionaryClassification.All:
                        challengesSolutions = GetDictionaryItems(project.Challenges);
                        futureAdditionSolutions = GetDictionaryItems(project.FutureAdditions);
                        break;
                    default:
                        break;
                }


                // if there is a item in the dictionary add it to the list to return
                if (challengesSolutions.Count > 0)
                {
                    foreach (SolutionModel solution in challengesSolutions)
                    {
                        outputList.Add(solution);
                    }
                }
                if (futureAdditionSolutions.Count > 0)
                {
                    foreach (SolutionModel solution in futureAdditionSolutions)
                    {
                        outputList.Add(solution);
                    }

                }
            }

            return outputList;
        }

        private List<SolutionModel> GetDictionaryItems(Dictionary<string, List<string>> dictionaryToConvert)
        {
            List<SolutionModel> outputList = new List<SolutionModel>();

            foreach (var item in dictionaryToConvert)
            {
                SolutionModel solutionToAdd = new SolutionModel();
                solutionToAdd.Key = item.Key;
                solutionToAdd.Values = item.Value;


                outputList.Add(solutionToAdd);
            }

            return outputList;
        }

        public List<ProjectModel> FillProjectList(ProjectStatus status, string mainGoal)
        {
            // Get all projects
            List<ProjectModel> allProjects = ReadAllProjectRecords(mainGoal);

            // Create list to output
            List<ProjectModel> outputList = new List<ProjectModel>();

            // Remove all projects with more then one subproject
            outputList = allProjects.Where(x => x.SubProjectIds.Count == 0).ToList();

            // Select items per status
            switch (status)
            {
                case ProjectStatus.ToDo:
                    outputList = outputList.Where(x => x.ProjectStatus == ProjectStatus.ToDo).ToList();
                    outputList = outputList.OrderByDescending(x => x.DateCreated).ToList();
                    break;
                case ProjectStatus.Doing:
                    outputList = outputList.Where(x => x.ProjectStatus == ProjectStatus.Doing).ToList();
                    outputList = outputList.OrderByDescending(x => x.DateDoing).ToList();
                    break;
                case ProjectStatus.Done:
                    outputList = outputList.Where(x => x.ProjectStatus == ProjectStatus.Done).ToList();
                    outputList = outputList.OrderByDescending(x => x.DateDone).ToList();
                    break;
                default:
                    break;
            }
            return outputList;
        }
        public List<ProjectModel> FilterListsByDate(List<ProjectModel> projectList, DateTime selectionDate, ProjectStatus status)
        {
            List<ProjectModel> outputList = new List<ProjectModel>();

            switch (status)
            {
                case ProjectStatus.ToDo:
                    // Create list of ToDo projects
                    outputList = projectList.Where(x => x.DateCreated >= selectionDate).ToList();
                    // Order by date Created
                    outputList = outputList.OrderByDescending(x => x.DateCreated).ToList();
                    break;
                case ProjectStatus.Doing:
                    // Create list of Doing projects
                    outputList = projectList.Where(x => x.DateDoing >= selectionDate).ToList();
                    // Order by date Doing
                    outputList = outputList.OrderByDescending(x => x.DateDoing).ToList();
                    break;
                case ProjectStatus.Done:
                    // Create list of Done projects
                    outputList = projectList.Where(x => x.DateDone >= selectionDate).ToList();
                    // Order by date Done
                    outputList = outputList.OrderByDescending(x => x.DateDone).ToList();
                    break;
                default:
                    break;
            }


            return outputList;
        }

        public List<PresentationDemoModel> CreateAllDemoModels(List<ProjectModel> allProjects)
        {
            List<PresentationDemoModel> outputDemoModelList = new List<PresentationDemoModel>();

            List<ProjectModel> allDemoProjects = allProjects.Where(x => x.HasDemo == true).ToList();

            foreach (ProjectModel project in allDemoProjects)
            {
                if (project.ShowItem == true)
                {
                    PresentationDemoModel modelToAdd = new PresentationDemoModel();
                    modelToAdd.Title = project.Title;
                    modelToAdd.DesiredOutcome = project.Outcome;
                    modelToAdd.ShortDescription = project.ShortDescription;
                    modelToAdd.ProjectLink = project.DemoLink;
                    modelToAdd.GitHubLink = project.GitHubLink;
                    modelToAdd.TotalHours = project.TotalHours;
                    modelToAdd.DeveloperNames = CreateDeveloperNamesList(project.DeveloperName);

                    outputDemoModelList.Add(modelToAdd);
                }
            }

            return outputDemoModelList;
        }

        public Guid GetProjectIdByTitle(string projectTitle, List<ProjectModel> allprojects)
        {
            return allprojects.Where(x => x.Title == projectTitle).First().ProjectId;
        }

        public (ProjectModel projectToChange, Dictionary<string, List<string>> dictionaryToChange) LoadProjectDetails(string projectTitle, List<ProjectModel> allProjects, bool futureFeaturesLoaded, bool challengesLoaded, IDataAccess _db)
        {
            ProjectModel outputProject = new ProjectModel();
            Dictionary<string, List<string>> outputDictionary = new Dictionary<string, List<string>>();

            outputProject = _db.GetProjectByTitle(projectTitle, allProjects);

            if (futureFeaturesLoaded == true)
            {
                outputDictionary = outputProject.FutureAdditions;
            }

            if (challengesLoaded == true)
            {
                outputDictionary = outputProject.Challenges;
            }

            outputDictionary = outputDictionary.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

            return (outputProject, outputDictionary);
        }

        public ProjectModel OverrideDictionaryInProject(ProjectModel projectToChange, Dictionary<string, List<string>> updatedDictionary, bool futureFeaturesLoaded, bool challengesLoaded)
        {

            if (futureFeaturesLoaded == true)
            {
                projectToChange.FutureAdditions = updatedDictionary;
            }

            if (challengesLoaded == true)
            {
                projectToChange.Challenges = updatedDictionary;
            }

            return projectToChange;
        }

        public List<string> MakeListFromDictionaryItemValues(Dictionary<string, List<string>> inputList, string itemKey)
        {
            List<string> outputList = new List<string>();

            foreach (var key in inputList)
            {
                if (key.Key.ToLower() == itemKey.ToLower())
                {
                    foreach (string value in key.Value)
                    {
                        outputList.Add(value);
                    }
                }
            }

            outputList = outputList.OrderBy(x => x).ToList();

            return outputList;
        }
        public List<string> SearchInCollection(string stringToSeach, List<string> collection)
        {
            List<string> outputlist = new List<string>();

            outputlist = collection.Where(x => x.ToLower().Contains(stringToSeach.ToLower())).ToList();

            return outputlist;
        }


    }
}
