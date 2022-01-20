using Microsoft.AspNetCore.Http;
using ProjectProgressLibrary.Models;
using System;
using System.Collections.Generic;

namespace ProjectProgressLibrary.DataAccess
{
    public interface IDataAccess
    {
        void AddTime(ProjectModel projectToAddHousTo, TimeUnitModel timeUnitToAdd, List<ProjectModel> allProjects, List<TimeUnitModel> allTimeUnits);
        void CalculateProjectProgress(ProjectModel projectToCalculate, List<ProjectModel> allProjects);
        void ChangeProjectStatus(string projectTitle, Enums.ProjectStatus newStatus, List<ProjectModel> allProjects);
        void ClearData();
        List<PresentationDemoModel> CreateAllDemoModels(List<ProjectModel> allProjects);
        List<Guid> CreateGuidListForSubProjects(ProjectModel projectToConvert, List<ProjectModel> allProjects, IDataAccess db);
        PresentaionProjectModel CreatePresentationProject(ProjectModel projectToConvert, List<ProjectModel> allProjects, IDataAccess db);
        void DeleteProject(string projectTitle, List<ProjectModel> allProjects, List<TimeUnitModel> allTimeUnits);
        string ExchangePunctuations(string projectName);
        Guid? FindProblemId(string mainProjectTitle, List<Guid?> subProjectIds, List<ProjectModel> allProjects);
        List<TimeUnitModel> GetAllRelevantTimeUnits(ProjectModel mainProject, List<TimeUnitModel> allTimeUnits, List<ProjectModel> allProjects, IDataAccess db);
        string GetFrontEndPhotoFolder();
        string GetMainProjectTitle(ProjectModel projectToConvert, List<ProjectModel> allProjects, IDataAccess db);
        ProjectModel GetProjectById(Guid projectId, List<ProjectModel> projectsList);
        ProjectModel GetProjectByTitle(string projectTitle, List<ProjectModel> projectsList);
        List<Guid> GetSubprojectIds(ProjectModel mainProject);
        TimeUnitModel GetTimeUnitById(Guid timeUnitId, List<TimeUnitModel> allTimeUnits);
        void MakeFirstEntry(string mainGoal);
        void ProcessPicture(IFormFile photo, string projectTitle);
        List<ProjectModel> ReadAllProjectRecords(string mainGoal);
        (List<ProjectModel> projectList, List<TimeUnitModel> timeUnitsList) ReadAllRecords(string mainGoal);
        List<TimeUnitModel> ReadAllTimeUnits(string mainGoal);
        void RemoveTime(ProjectModel projectToAddHousTo, TimeUnitModel timeUnitToDelete, List<ProjectModel> allProjects, List<TimeUnitModel> allTimeUnits);
        void SaveAllProjects(List<ProjectModel> projects);
        void SaveAllRecords(List<ProjectModel> projects, List<TimeUnitModel> timeUnits);
        void SaveAllTimeUnits(List<TimeUnitModel> timeUnits);
        void SaveProject(ProjectModel projectToSave, List<ProjectModel> allProjects);
        void SaveTimeUnit(TimeUnitModel timeUnitToSave, List<TimeUnitModel> timeUnits);
    }
}