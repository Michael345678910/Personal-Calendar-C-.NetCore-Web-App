# TimeGrid — Your Personal Calendar App (ASP.NET Core + EF Core):


# Overview:


TimeGrid is a full-stack + C# .NET web application that lets authenticated users manage a personal calendar with events and add optional locations to the events. It uses ASP.NET Core MVC (net9.0), Entity Framework Core 9 (SQL Server), ASP.NET Core Identity for auth, and FullCalendar for a smooth UI with a month/week/list view. You can add, edit, and delete events, assign a location to the events (e.g., “Office”), and view your schedule on an interactive calendar.

While developing this program, I was designing it to showcase my skills and knowledge by using the following different concepts. I built this project to showcase my skills in full-stack web development, along with C#, SQL, EF Core, Identity, and clean architecture using dependency injection.

# Features:

- **Interactive calendar UI:** This calendar application is powered by FullCalendar (https://fullcalendar.io/) to provide an interactive and aesthetically pleasing user interface (month, week, and list views).
- **Event management (CRUD):** The application practices CRUD to allow the user the ability to create, update, delete, and save different calendar events, with names, optional descriptions, optional locations, and optional start/end times.
- **Durable Data Persistence:** Events and locations are stored in SQL Server via EF Core; the schema is versioned with migrations, so data survives app restarts and upgrades.
- **Authentication & Authorization:** ASP.NET Core Identity; allows users to log in and log out of an account they register with the calendar application. This allows the calendar application to hold onto and remember/load the current logged in owners calendar, which allows the user to view/manage their own events.
- **Bootstrap 5 UI:** The Bootstrap allows the user to add events directly from the calendar via the modal form.
- **Lazy loading:** This allows the users to navigate calendar properties via EF Core proxies.\
- **Clean separation:** Controllers, view models, data access layer (IDAL), and the rest of the project are all cleanly separated from one another to allow for seamless future improvements and upcoming features to be added and developed easily.

## Technologies Used:
- .NET 9 (ASP.NET Core MVC) — TargetFramework: net9.0
- Entity Framework Core 9 — SQL Server provider + LazyLoading proxies
- ASP.NET Core Identity — authentication & user management
- FullCalendar 5.x (bundled under wwwroot/js/main.js)
- Bootstrap 5.3 (loaded via CDN in _Layout.cshtml) + Font icons

# Usage Instructions:

# File Pathway Tree/ File Directory:
Please check the File Pathway Tree Document File found in this project's repository.

# Installation & Setup:
**1) Prerequisites:**
- 1.1.1) .NET SDK 9.0
- 1.2.1) SQL Server Express LocalDB (Windows) or access to a SQL Server instance
- 1.2.2) Default dev connection uses LocalDB, configured in appsettings.json
- 1.3.1) Make sure an IDE is installed; for this tutorial, Visual Studio 2022 Community Edition will be the one utilized.
**2) Clone And Restore:**
- 2.1.1) Download or clone this repository to your local machine.
- 2.2.1) After, open the project folder and open the Developer Command Prompt in the IDE via the Tools drop-down menu.
- 2.3.1) In the Developer Command Prompt Menu, to restore the packages for the program, type in the following prompt.
- 2.3.2) *dotnet restore* (Type this into the terminal)
**3) Configure The Connection String:**
- 3.1.1) By default, the app reads ConnectionStrings:DefaultConnection from the configuration.
- 3.2.1) You can either keep LocalDB (Windows) as it currently is in appsettings.json, to have it connect to a local database. OR you can do the following in 3.3.1.
- 3.3.1) Set a secure connection string via the User Secrets from within development:
- 3.3.2) Once in the IDE and while still being inside the project folder, type in the following. 
- 3.3.3) *dotnet user-secrets init* (Type this into the terminal)
- 3.3.4) In 3.3.5 below, there is an example of what the next Developer Command Prompt Menu would be to get the application to connect to your server/DB (edit the connection address to be your server/DB).
- 3.3.5) dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\\mssqllocaldb;Database=TimeGrid;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True" (Type this into the terminal)
**4) Apply EF Core Migrations:**
- 4.1.1) If it has not been installed previously, or if you need the EF CLI package installed, use the following Developer Command Prompt Menu command.
- 4.1.2) dotnet tool install --global dotnet-ef (If not already installed | Type this into the terminal)
- 4.2.1) Then use the next Developer Command Prompt Menu command to update the database.
- 4.2.2) dotnet ef database update (Type this into the terminal)
- 4.3.1) These commands will create the identity tables and app tables (Events & Locations) in the configured database.
**5) Run the app:**
- 5.1.1) To run the calendar application, type the following command into the Command Prompt Menu.
- 5.1.2) dotnet run (Type this into the terminal)
- This should then open a new tab on your device's preferred web surfing application (Chrome, Firefox, Edge, etc.) with the calendar application up, running, and ready to go. 
- 5.1.3) If it did not open automatically, open a new tab in your preferred web surfing application and open the URL being shown in the console. (Currently: https://localhost:5001).


## Troubleshooting:
If any of the following issues occur, please try the following troubleshooting solutions.
- **1) dotnet ef not found:**
- 1.1.1) Reinstall or install the following tool via the Developer Command Prompt Menu.
- 1.1.2) dotnet tool install --global dotnet-ef (Type this into the terminal).
- 1.2.1) Afterwards, run the following prompt into the Developer Command Prompt Menu.
- 1.2.2) dotnet ef database update (Type this into the terminal).


- **2) Build failed while running migrations:**
- Each of the following prompts should be entered into the Developer Command Prompt Menu to attempt the solution.


- *2.1) Quick Fix:*
- 2.1.1) Clean and rebuild the project, once in your project folder with .csproj.
- 2.1.2) dotnet clean (Type this into the terminal) then | dotnet restore (Type this into the terminal) then | dotnet build (Type this into the terminal) then | dotnet ef database update (Type this into the terminal).


- *2.2) Dotnet ef Is Missing:*
- 2.2.1) dotnet tool install --global dotnet-ef (Type this into the terminal).


- *2.3) If You Have Multiple Projects That Are Getting In The Way (Set Project & Startup):*
- 2.3.1 dotnet ef database update -p DotNetCoreCalendar.csproj -s DotNetCoreCalendar.csproj (Type this into the terminal).


- 2.4) If The Last Migration Is Broken & Not Applied:


- 2.4.1) dotnet ef migrations remove (Type this into the terminal) then | dotnet ef migrations add FixBuild (Type this into the terminal) then | dotnet ef database update (Type this into the terminal).


- **3) Connection issues:**
- 3.1.1) If you run into database connection issues, verify your connection string is correct. This will be either via *User Secrets* if you’re connecting to your personal database or via *appsettings.json* if you’re connecting to a local database.
- 3.2.1) If you are not using a Windows operating system computer, it is highly suggested to switch to a SQL Server instance/container or update EF to use SQLite. 
- 3.2.2) If you do step 3.2.1, you will also need to change the provider in the following files: Program.cs and ApplicationDbContext.cs.

## How It Works:
1. Try the “Your Personal Calendar” application/webpage without logging in to get a good grasp of the page and the application. Or, register and sign in to use the built-in Identity user interface to create an account and log in to have a personalized calendar experience.
2. My calendar is the landing page for the Your Personal Calendar” application/webpage. In the My Calendar view, once logged in (or as a guest/not logged in), you will be able to view your personal schedule.
- Click the “Create” button, or click on a date/time slot to open the Add Event modal (Bootstrap). This will then allow the user to enter an event Name, an optional Location, an optional Start/End time, (and a optional date if you selected the Create button).
3. Use the Events page to view the entered calendar entries in a list format, edit, or delete any entered calendar events from the logged-in user's calendar.
4. The Locations page is used to create/edit/delete location records. When initially creating or when editing an event, you can assign a location to the calendar event; howeve,r this is optional.
5. The privacy page is utilized to share a fake/demonstration version of a website privacy page. Going over numerous data privacy/overall fake terms and conditions for the fake application/site.
6. Ownership is enforced through the custom UserAccessOnly filter; this ensures only the event owner can access/modify their own calendar items.


## How It Works (Architecture Version):
- **ASP.NET Core MVC with minimal hosting (Program.cs):** wires up DI, EF Core, Identity, Razor Pages, and MVC routes. 
- **EF Core 9 + SQL Server:** ApplicationDbContext manages Events and Locations plus Identity tables; lazy loading proxies are enabled as well. 
- **Data Access Layer:** IDAL interface and DAL implementation wrap allow for common queries and persistence logic. 
- **View Models:** The EventViewModel supplies event data and location dropdowns for the project forms. 
- **User-Interface:** Layout.cshtml loads Bootstrap; Home/MyCalendar.cshtml initializes FullCalendar and posts form data to EventController.Create. 

# Contributing To the Codebase:

Contributions are welcome! Please fork this repository, and create a feature branch: git checkout -b feature/your-idea . Then add your changes, fixes, or new features to the project under this branch, and then commit the changes with a clear description. Afterwards, open a Pull Request describing the changes/fixes and the rationale behind them.

# Acknowledgments:

- FullCalendar for the robust calendar UI 
- Bootstrap for the responsive styling 
- ASP.NET Core Identity for authentication scaffolding 

# Still Working On/Upcoming Features:

- Data persistence/enabling the database to save events added to the database/calendar application.
- Have edits to the calendar save after being edited, and the save button is clicked.
- Deleting a Calendar Event does not crash the entire application.
