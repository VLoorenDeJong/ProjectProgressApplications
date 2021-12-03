using ProjectProgressLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static ProjectProgressLibrary.Enums;

namespace ProjectProgressLibrary.Tests
{
    public class PriorityNrCalculationTests
    {
        // ToDo Create priority number tests (they are subtracting from 1011)
        // ToDo make the class validate number under 10 first
        [Fact]
        public void ShouldReturn() 
        {
            // Arrange
            ProjectModel project = new ProjectModel();

            project.CalculatePriority();

            int expected = 0;

            // Act
            int actual = 0;


            // Assert

            Assert.Equal(expected, actual);

        }
        


    }
}
