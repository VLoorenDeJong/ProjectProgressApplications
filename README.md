# ProjectProgressApplications
This is a combination of solutions, a Progress tracker and my Portfolio application. The Progress application is also set up as a CMS for parts of the portfolio.

## Progress application 
An application set to monitor your progress and display it in a Portfolio

### Features:
* Create/Read/Update/Delete projects
* Add challenges and solutions to the project
* Add future plans and runways to the project
* Add time to the project in three catagories (Theoretical/Practical/General)
* Create a project tree with sub projects
* Set the projetc status(ToDo/Doing/Done)

### Known bugs
* Beginner front end code
* Timestamps are in UTC time
* Demo version removes entries upon homepage load new demo use
* Adding time to subproject does not start main project
* Numeric counters in challenges and additions order numbers like text 1, 10, 2

### Bugfixes under review
* When shifting projects around a negative hour count can be calculated
* Subtract subproject hours from all projects above

#### Build notes
* check the location of the save files in the appsettings.json

## Portfolio application
This application is my portfolio you can visit it @ www.victorloorendejong.nl 

### Features
* Displaying project details
* Repository of your progress application solutions and runways
* Displaying the hours per project and catagory
* 
