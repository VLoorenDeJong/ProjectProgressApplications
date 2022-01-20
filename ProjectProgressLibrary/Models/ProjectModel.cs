using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProjectProgressLibrary.Enums;
using static ProjectProgressLibrary.Validation.DataValidation;
using static ProjectProgressLibrary.Validation.DateTimeValidation;

namespace ProjectProgressLibrary.Models
{
    public class ProjectModel
    {
        public ProjectModel()
        {
        }
        public ProjectModel(bool showItemOn, bool priorityCalculationOn, bool showProgressBarOn, ProjectStatus projectStatus, List<ProjectModel> subProjects)
        {
        }

        private int priorityPoints = 10001;

        public Guid ProjectId { get; private set; } = Guid.NewGuid();
        // ToDo make this private
        public Nullable<Guid> MainProjectId { get; private set; } = new Guid();
        public string Title { get; set; }       
        public string ShortDescription { get; set; }
        public string Outcome { get; set; }
        public string DeveloperName { get; set; }
        public string DemoLink { get; set; }
        public string GitHubLink { get; set; }
        public bool ShowItem { get; set; } = false;
        public bool ShowProgressBar { get; set; } = false;
        public bool PriorityCalculation { get; set; } = true;
        public bool HasDemo { get; set; } = false;
        public int PriorityNumber { get; private set; } = 10001;
        public int Impact { get; set; } = 1;
        public int Urgency { get; set; } = 1;
        public int EaseOffSuccess { get; set; } = 1;
        public int PersonalPreference { get; set; } = 1;
        public double Progress { get; private set; } 
        public double TotalHours { get; private set; } 
        public double TheoreticalHours { get; private set; }
        public double PracticalHours { get; private set; }
        public double GeneralHours { get; private set; }
        public DateTime DateCreated { get; private set; } = DateTime.Now;
        public DateTime DateDoing { get; private set; }
        public DateTime DateDone { get; private set; } 
        public ProjectStatus ProjectStatus { get; private set; } = ProjectStatus.ToDo;
        public Dictionary<string, List<string>> Challenges { get; set; } = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> FutureAdditions { get; set; } = new Dictionary<string, List<string>>();
        public List<Nullable<Guid>> TimeUnitsPutIn { get; private set; } = new List<Nullable<Guid>>();
        public List<Nullable<Guid>> SubProjectIds { get; private set; } = new List<Nullable<Guid>>();
        public void ChangeProjectStatus(ProjectStatus newStatus)
        {
            switch (newStatus)
            {
                case ProjectStatus.ToDo:
                    break;
                case ProjectStatus.Doing:
                    SetDateDoing(DateTime.Now.ToString());
                    break;
                case ProjectStatus.Done:
                    Progress = 100;
                    SetDateDone(DateTime.Now.ToString());
                    break;
                default:
                    break;
            }
            ProjectStatus = newStatus;
        }
        public void SetMainProjectId(Nullable<Guid> mainProjectId)
        {
            bool isGuid = mainProjectId.ToString().ValidateGuid();

            if (isGuid == true)
            {                
                MainProjectId = mainProjectId;
            }
        }
        public void CalculatePriority()
        {
                PriorityNumber = priorityPoints - (Impact * Urgency * EaseOffSuccess * PersonalPreference);
        }                                         
        public void AddTimeUnitIdToProject(string stringGuid)
        {
            bool isGuid = stringGuid.ValidateGuid();

            if (isGuid == true)
            {
                TimeUnitsPutIn.Add(new Guid(stringGuid));
            }
        }
        public void AddSubProject(string stringGuid)
        {
            bool isGuid = stringGuid.ValidateGuid();

            if (isGuid == true)
            {
                SubProjectIds.Add(new Guid(stringGuid));
            }
        }
        public void SetProjectId(string stringGuid)
        {
            bool isGuid = stringGuid.ValidateGuid();

            if (isGuid == true)
            {
                ProjectId = new Guid(stringGuid);
            }
        }
        internal void SetPriorityNumber(string priorityNumberString)
        {

            int priorityNumber = int.Parse(priorityNumberString);

            if (priorityNumber > 0 && priorityNumber < 10)
            {
                PriorityNumber = priorityNumber;
            }
        }
        public void SetTotalHours(string totalHours)
        {
            bool isDouble = totalHours.ValidateDouble("TotalHours");
            if (isDouble == true)
            {
                TotalHours = double.Parse(totalHours);
            }
        }
        public void SetTheoreticalHours(string theoreticalHours = "0")
        {

            bool isDouble = theoreticalHours.ValidateDouble("TheoreticalHours");
            if (isDouble == true)
            {
                TheoreticalHours = double.Parse(theoreticalHours);
            }
        }
        public void SetPracticalHours(string practicalHours = "0")
        {

            bool isDouble = practicalHours.ValidateDouble("TheoreticalHours");
            if (isDouble == true)
            {
                PracticalHours = double.Parse(practicalHours);
            }
        }
        public void SetGeneralHours(string generalHours = "0")
        {
            bool isDouble = generalHours.ValidateDouble("GeneralHours");
            if (isDouble == true)
            {
                GeneralHours = double.Parse(generalHours);
            }
        }
        public void SetDateCreated(string creatationDateTime = null)
        {
            DateCreated = CreateDateFromString(creatationDateTime);
        }
        public void SetDateDoing(string startedDateTime = null)
        {
            DateDoing = CreateDateFromString(startedDateTime);
        }
        public void SetDateDone(string finishedDateTime = null)
        {
            DateDone = CreateDateFromString(finishedDateTime);
        }
        public void SetProjectStatus(string status)
        {
            bool hasContent = status.ValidateStringHasContent();

            if (hasContent == true)
            {
                int enumNumber = int.Parse(status);

                switch (enumNumber)
                {
                    case 0:
                        ProjectStatus = ProjectStatus.ToDo;
                        break;
                    case 1:
                        ProjectStatus = ProjectStatus.Doing;
                        break;
                    case 2:
                        ProjectStatus = ProjectStatus.Done;
                        break;
                    default:
                        break;
                }
            }
        }
        public void  AddChallenge(string challenge, List<string> solutions)
        {
            if (string.IsNullOrWhiteSpace(challenge) == false )
            {
                Challenges.Add(challenge, solutions);
            }
        }
        public void AddFutureAddition(string addition, List<string> suggestions)
        {
            if (string.IsNullOrWhiteSpace(addition) == false)
            {
                FutureAdditions.Add(addition, suggestions);
            }
        }
        public void SetProgress(string stringDouble)
        {
            bool isDouble = stringDouble.ValidateDouble("Progress persentage");

            if (isDouble == true)
            {
                Progress = double.Parse(stringDouble);
            }
        }
        internal void AddHoursToProject(TimeUnitModel timeUnitToAdd)
        {
            switch (timeUnitToAdd.Classification)
            {
                case HourClassification.Practical:
                    PracticalHours = PracticalHours + timeUnitToAdd.HoursPutIn;
                    break;
                case HourClassification.Theoretical:
                    TheoreticalHours = TheoreticalHours + timeUnitToAdd.HoursPutIn;
                    break;
                case HourClassification.General:
                    GeneralHours = GeneralHours + timeUnitToAdd.HoursPutIn;
                    break;
                default:
                    break;
            }

            TotalHours = PracticalHours + TheoreticalHours + GeneralHours;

            if (timeUnitToAdd.ProjectId == ProjectId)
            {
                AddTimeUnitIdToProject(timeUnitToAdd.TimeUnitId.ToString());
            }
        }
        internal void SubtractHoursFromProject(TimeUnitModel timeUnitToSubtract)
        {
            switch (timeUnitToSubtract.Classification)
            {
                case HourClassification.Practical:
                    PracticalHours = PracticalHours - timeUnitToSubtract.HoursPutIn;
                    break;
                case HourClassification.Theoretical:
                    TheoreticalHours = TheoreticalHours - timeUnitToSubtract.HoursPutIn;
                    break;
                case HourClassification.General:
                    GeneralHours = GeneralHours - timeUnitToSubtract.HoursPutIn;
                    break;
                default:
                    break;
            }

            TotalHours = PracticalHours + TheoreticalHours + GeneralHours;

            if (PracticalHours < 0 || TheoreticalHours < 0 || GeneralHours < 0 || TotalHours < 0)
            {
                throw new Exception($"practical={ PracticalHours } Theoretical={ TheoreticalHours } General={ GeneralHours } Total={ TotalHours }");
            }

            if (timeUnitToSubtract.ProjectId == ProjectId)
            {

                RemoveTimeUnitIdFromProject(timeUnitToSubtract.TimeUnitId.ToString());
            }

        }
        private void RemoveTimeUnitIdFromProject(string stringGuid)
        {

            bool isGuid = stringGuid.ValidateGuid();

            if (isGuid == true)
            {
                TimeUnitsPutIn.Remove(new Guid(stringGuid));
            }
        }
        public string GetProjectSatusInString()
        {
            ProjectStatus status = ProjectStatus;

            int output = 0;

            switch (status)
            {
                case ProjectStatus.ToDo:
                    output = ProjectStatus.ToDo.GetHashCode();
                    break;
                case ProjectStatus.Doing:
                    output = ProjectStatus.Doing.GetHashCode();
                    break;
                case ProjectStatus.Done:
                    output = ProjectStatus.Done.GetHashCode();
                    break;
                default:
                    break;
            }

            return output.ToString();
        }
        //private double CalculateSubprojectProgress(List<ProjectStatus> subProjectStatusList, int projectPercentage)
        //{
        //    double outputPercentage = 0;

        //    foreach (ProjectStatus subProjectStatus in subProjectStatusList)
        //    {
        //        if (subProjectStatus == ProjectStatus.Done)
        //        {
        //            outputPercentage = outputPercentage + projectPercentage;
        //        }
        //    }

        //    return outputPercentage;
        //}
        //private List<Guid> GetGoverningProjectIds(string mainProjectId, List<ProjectModel> allProjects)
        //{
        //    List<Guid> output = new List<Guid>();

        //    while (mainProjectId != "00000000-0000-0000-0000-000000000000")
        //    {
        //        output.Add(new Guid(mainProjectId));

        //        ProjectModel governingProject = allProjects.Where(x => x.ProjectId == output.Last()).Last();

        //        mainProjectId = governingProject.MainProjectId.ToString();
        //    }

        //    return output;
        //}
        public void StartProject()
        {
            ChangeProjectStatus(ProjectStatus.Doing);
        }
        public void StopProject()
        {
            ChangeProjectStatus(ProjectStatus.ToDo);
            
        }
        public void FinishProject()
        {
            ChangeProjectStatus(ProjectStatus.Done);
        }
        public Guid GetMainProjectId()
        {
            Guid outputGuid = Guid.NewGuid();

            bool isGuid = MainProjectId.ToString().ValidateGuid();

            if (isGuid == true)
            {
               string stringGuid = MainProjectId.ToString();
               outputGuid = Guid.Parse(stringGuid);
            }

            return outputGuid;
        }
    }
}
