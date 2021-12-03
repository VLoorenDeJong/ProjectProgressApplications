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
//    public class ProgressCalculationTests
//    {
//        [Fact]
//        public void ToDoProgressShouldreturn0()
//        {

//            // Arrange
//            ProjectModel project = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            int expected = 0;

//            // Act
//          //  project = project.CalculateProgress(project);
//            int actual = project.Progress;

//            // Assert
//            Assert.Equal(expected, actual);
//        }
//        [Fact]
//        public void FinishedProjectProgressShouldreturn100()
//        {

//            // Arrange
//            ProjectModel project = new ProjectModel(true, true, true, ProjectStatus.Done);


//            int expected = 100;

//            // Act
//           // project = project.CalculateProgress(project);
//            int actual = project.Progress;

//            // Assert
//            Assert.Equal(expected, actual);
//        }
//        [Fact]
//        public void StartedProgressShouldReturn25()
//        {
//            // Arange
//            ProjectModel project = new ProjectModel(true, true, true, ProjectStatus.Busy);

//            ProjectModel subproject1 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject2 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            ProjectModel subproject3 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            ProjectModel subproject4 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
           
//            //project.SubProjects.Add(subproject3);
//            //project.SubProjects.Add(subproject2);
//            //project.SubProjects.Add(subproject1);
//            //project.SubProjects.Add(subproject4);

//            int expected = 25;

//            // Act
//        //    project = project.CalculateProgress(project);
//            int actual = project.Progress;

//            //Assert
//            Assert.Equal(expected, actual);

//        }
//        [Fact]
//        public void StartedProgressShouldReturn75()
//        {
//            // Arange
//            ProjectModel project = new ProjectModel(true, true, true, ProjectStatus.Busy);

//            ProjectModel subproject1 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject2 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject3 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject4 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
           
//            //project.SubProjects.Add(subproject1);
//            //project.SubProjects.Add(subproject2);
//            //project.SubProjects.Add(subproject3);
//            //project.SubProjects.Add(subproject4);


//            int expected = 75;

//            // Act
//            // project = project.CalculateProgress(project);
//            int actual = project.Progress;

//            //Assert
//            Assert.Equal(expected, actual);

//        }
//        [Fact]
//        public void StartedProgressShouldReturn100()
//        {
//            // Arange
//            List<ProjectModel> subProjects = new List<ProjectModel>();


//            ProjectModel subproject1 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject2 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject3 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject4 = new ProjectModel(true, true, true, ProjectStatus.Done);
            
//            subProjects.Add(subproject1);
//            subProjects.Add(subproject2);
//            subProjects.Add(subproject3);
//            subProjects.Add(subproject4);

//            ProjectModel project = new ProjectModel(true, true, true, ProjectStatus.Busy, subProjects);


//            subproject4.ChangeProjectStatus(ProjectStatus.Done);

//            int expected = 100;

//            // Act
//           // project = project.CalculateProgress(project);
//            int actual = project.Progress;

//            //Assert
//            Assert.Equal(expected, actual);

//        }
//        [Fact]
//        public void StartedProgressShouldReturn16()
//        {
//            // Arange
//            ProjectModel project = new ProjectModel(true, true, true, ProjectStatus.Busy);

//            ProjectModel subproject1 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject2 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject3 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            ProjectModel subproject4 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            ProjectModel subproject5 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            ProjectModel subproject6 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            ProjectModel subproject7 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            ProjectModel subproject8 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            ProjectModel subproject9 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            ProjectModel subproject10 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            ProjectModel subproject11 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            ProjectModel subproject12 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
           
//            //project.SubProjects.Add(subproject1);
//            //project.SubProjects.Add(subproject2);
//            //project.SubProjects.Add(subproject3);
//            //project.SubProjects.Add(subproject4);
//            //project.SubProjects.Add(subproject5);
//            //project.SubProjects.Add(subproject6);
//            //project.SubProjects.Add(subproject7);
//            //project.SubProjects.Add(subproject8);
//            //project.SubProjects.Add(subproject9);
//            //project.SubProjects.Add(subproject10);
//            //project.SubProjects.Add(subproject11);
//            //project.SubProjects.Add(subproject12);

//            int expected = 16;

//            // Act
//           // project = project.CalculateProgress(project);
//            int actual = project.Progress;

//            //Assert
//            Assert.Equal(expected, actual);

//        }
//        [Fact]
//        public void ProgressShouldReturn48()
//        {
//            // Arange
//            ProjectModel project = new ProjectModel(true, true, true, ProjectStatus.Busy);

//            ProjectModel subproject1 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject2 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject3 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject4 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject5 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject6 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject7 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            ProjectModel subproject8 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            ProjectModel subproject9 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            ProjectModel subproject10 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            ProjectModel subproject11 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            ProjectModel subproject12 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
            
//            //project.SubProjects.Add(subproject1);
//            //project.SubProjects.Add(subproject2);
//            //project.SubProjects.Add(subproject3);
//            //project.SubProjects.Add(subproject4);
//            //project.SubProjects.Add(subproject5);
//            //project.SubProjects.Add(subproject6);
//            //project.SubProjects.Add(subproject7);
//            //project.SubProjects.Add(subproject8);
//            //project.SubProjects.Add(subproject9);
//            //project.SubProjects.Add(subproject10);
//            //project.SubProjects.Add(subproject11);
//            //project.SubProjects.Add(subproject12);

//            int expected = 48;

//            // Act
//           // project = project.CalculateProgress(project);
//            int actual = project.Progress;

//            //Assert
//            Assert.Equal(expected, actual);

//        }
//        [Fact]
//        public void FourStartedFiveFinishedProgressShouldReturn48()
//        {

//            // Arange
//            List<ProjectModel> subProjects = new List<ProjectModel>();

//            ProjectModel subproject1 = new ProjectModel(true, true, true, ProjectStatus.Busy);
//            ProjectModel subproject2 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject3 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject4 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject5 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject6 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject7 = new ProjectModel(true, true, true, ProjectStatus.Busy);
//            ProjectModel subproject8 = new ProjectModel(true, true, true, ProjectStatus.Busy);
//            ProjectModel subproject9 = new ProjectModel(true, true, true, ProjectStatus.Busy);
//            ProjectModel subproject10 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            ProjectModel subproject11 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            ProjectModel subproject12 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
            
//            subProjects.Add(subproject1);
//            subProjects.Add(subproject2);
//            subProjects.Add(subproject3);
//            subProjects.Add(subproject4);
//            subProjects.Add(subproject5);
//            subProjects.Add(subproject6);
//            subProjects.Add(subproject7);
//            subProjects.Add(subproject8);
//            subProjects.Add(subproject9);
//            subProjects.Add(subproject10);
//            subProjects.Add(subproject11);
//            subProjects.Add(subproject12);



//           // ProjectModel project = new ProjectModel(true, true, true, ProjectStatus.Busy, subProjects);

//            int expected = 48;

//            // Act
//            //project = project.CalculateProgress(project);
//           // int actual = project.Progress;

//            // Assert

//           // Assert.Equal(expected, actual);
//        }
//        [Fact]
//        public void ProgressShouldReturn90()
//        {

//            // Arange
//            ProjectModel project = new ProjectModel(true, true, true, ProjectStatus.Busy);

//            ProjectModel subproject1 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject2 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject3 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject4 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject5 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject6 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject7 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject8 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject9 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject10 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject11 = new ProjectModel(true, true, true, ProjectStatus.Done);
//            ProjectModel subproject12 = new ProjectModel(true, true, true, ProjectStatus.Busy);
           
//            //project.SubProjects.Add(subproject1);
//            //project.SubProjects.Add(subproject2);
//            //project.SubProjects.Add(subproject3);
//            //project.SubProjects.Add(subproject4);
//            //project.SubProjects.Add(subproject5);
//            //project.SubProjects.Add(subproject6);
//            //project.SubProjects.Add(subproject7);
//            //project.SubProjects.Add(subproject8);
//            //project.SubProjects.Add(subproject9);
//            //project.SubProjects.Add(subproject10);
//            //project.SubProjects.Add(subproject11);
//            //project.SubProjects.Add(subproject12);

//            int expected = 90;

//            // Act
//           // project = project.CalculateProgress(project);
//            int actual = project.Progress;

//            // Assert

//            Assert.Equal(expected, actual);
//        }

//        [Fact]
//        public void ProgressShouldReturn2()
//        {
//            // Arange
//            List<ProjectModel> subProjects = new List<ProjectModel>();

//            ProjectModel subproject1 = new ProjectModel(true, true, true, ProjectStatus.Busy);
//            ProjectModel subproject2 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            ProjectModel subproject3 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            ProjectModel subproject4 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            ProjectModel subproject5 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            ProjectModel subproject6 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            ProjectModel subproject7 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            ProjectModel subproject8 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            ProjectModel subproject9 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            ProjectModel subproject10 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            ProjectModel subproject11 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
//            ProjectModel subproject12 = new ProjectModel(true, true, true, ProjectStatus.ToDo);
            
//            subProjects.Add(subproject1);
//            subProjects.Add(subproject2);
//            subProjects.Add(subproject3);
//            subProjects.Add(subproject4);
//            subProjects.Add(subproject5);
//            subProjects.Add(subproject6);
//            subProjects.Add(subproject7);
//            subProjects.Add(subproject8);
//            subProjects.Add(subproject9);
//            subProjects.Add(subproject10);
//            subProjects.Add(subproject11);
//            subProjects.Add(subproject12);

//          //  ProjectModel project = new ProjectModel(true, true, true, ProjectStatus.ToDo, subProjects);

//            int expected = 2;

//            // Act
//           // project = project.CalculateProgress(project);
//           // int actual = project.Progress;

//            // Assert

//            Assert.Equal(expected, actual);
//        }
//    }
//}
