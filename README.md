# ProjectProgressApplications
This is a combination of solutions, a Progress tracker and my Portfolio application. The Progress application is also set up as a CMS for parts of the portfolio.

## Progress application 
An application set to monitor your progress and display it in a Portfolio
There is a demo available @ https://demo.victorloorendejong.nl/

### Features:
* Create/Read/Update/Delete projects
* Add challenges and solutions to the project
* Add future plans and runways to the project
* Add time to the project in three catagories (Theoretical/Practical/General)
* Create a project tree with sub projects
* Set the projetc status(ToDo/Doing/Done)

### Future plans
* Statistic reports of a corporate level
* Adding comments to a time unit
* User sign in
* Multiple users
* Add a picture to the project
* Indication why a project can not be removed
* Character limit on text fields
* Adding a SQL database option

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
* Check the location of the save files in the appsettings.json before use



## Portfolio application
This application is my portfolio you can visit it @ www.victorloorendejong.nl 

### Features
* Displaying my current knowledge and understanding of C# projects
* Displaying project details
* Find your runways and solutions of all the projects based on the Challenge or Addition title
* Displaying the hours per project and catagory
* Displaying certificates and contact info
* More then 90% of the styling is done with css

### Future additions
* Rewrite front end code 
* Clean up the urls 
* More funtional menu's
* Create a blog section
* Add a contributions section
* Add a hobby section

### Known bugs
* List marker moves to last line when multiple lines
* Long project tilte goes outside the menu
* Does not scale well to small screens

#### Build notes
*To start the application the foloing prjects are mendatory in the CSV file: Projecten, Leerpad

# If there are any problems or suggestions for the app please contact me @ contact@victorloorendejong.nl
 
