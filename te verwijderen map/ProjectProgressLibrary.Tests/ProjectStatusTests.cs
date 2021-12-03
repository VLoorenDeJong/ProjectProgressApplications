//using ProjectProgressLibrary.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Xunit;
//using static ProjectProgressLibrary.Enums;

//namespace ProjectProgressLibrary.Tests
//{
//    public class ProjectStatusTests
//    {
//        [Fact]
//        public void ShoudReturnToDo()
//        {
//            // Arange
//            List<ProjectModel> subProjects = new List<ProjectModel>();

//           // ProjectModel project = new ProjectModel(true, true, true, ProjectStatus.ToDo, subProjects);

//            ProjectModel subproject1 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            ProjectModel subproject2 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            ProjectModel subproject3 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            ProjectModel subproject4 = new ProjectModel(true, true, true, ProjectStatus.ToDo);

//            //project.SubProjects.Add(subproject1);
//            //project.SubProjects.Add(subproject2);
//            //project.SubProjects.Add(subproject3);
//            //project.SubProjects.Add(subproject4);

//            ProjectStatus expected = ProjectStatus.ToDo;

//            // Act
//           // project = project.CalculateProgress(project);
//            // ProjectStatus actual = project.ProjectStatus;

//            //Assert
//           // Assert.Equal(expected, actual);

//        }
//        [Fact]
//        public void ShoudReturnStarted()
//        {
//            // Arange
//            ProjectModel project = new ProjectModel(true, true, true, ProjectStatus.ToDo);

//            ProjectModel subproject1 = new ProjectModel(true, true, true, ProjectStatus.Busy);
//            ProjectModel subproject2 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            ProjectModel subproject3 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            ProjectModel subproject4 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            //project.SubProjects.Add(subproject1);
//            //project.SubProjects.Add(subproject2);
//            //project.SubProjects.Add(subproject3);
//            //project.SubProjects.Add(subproject4);

//            ProjectStatus expected = ProjectStatus.Busy;

//            // Act
//           // project = project.CalculateProgress(project);
//            ProjectStatus actual = project.ProjectStatus;

//            //Assert
//            Assert.Equal(expected, actual);
//        }
//        [Fact]
//        public void ShoudReturnFinished()
//        {
//            // Arange
//            List<ProjectModel> subProjects = new List<ProjectModel>();

//            ProjectModel project = new ProjectModel(true, true, true, ProjectStatus.ToDo, subProjects);

//            ProjectModel subproject1 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject2 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject3 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject4 = new ProjectModel(true, true, true, ProjectStatus.Done);

//            //project.SubProjects.Add(subproject1);
//            //project.SubProjects.Add(subproject2);
//            //project.SubProjects.Add(subproject3);
//            //project.SubProjects.Add(subproject4);

//            ProjectStatus expected = ProjectStatus.Done;

//            // Act
//           // project = project.CalculateProgress(project);
//            ProjectStatus actual = project.ProjectStatus;

//            //Assert
//            Assert.Equal(expected, actual);
//        }
//    }
//}
