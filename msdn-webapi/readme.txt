* Contact Manager Web API
  https://code.msdn.microsoft.com/Contact-Manager-Web-API-0e8e373d
  Daniel Roth,  MSFT
  <= WebAPI back-end, Windows 8 front-end...

* Description:
This sample uses ASP.NET Web API to build a simple contact manager application. The 
application consists of a contact manager web API that is used by an ASP.NET
 MVC application, a Windows Phone application, and a Windows 8 app to display 
and manage a list of contacts.

* Project:
D:.
+---.nuget
+---ContactManager
¦   +---App_Data
¦   +---App_Start
¦   +---Areas
¦   ¦   +---HelpPage
¦   ¦       +---App_Start
¦   ¦       +---Controllers
¦   ¦       +---Models
¦   ¦       +---SampleGeneratio
¦   ¦       +---Views
¦   ¦           +---Help
¦   ¦           ¦   +---Display
¦   ¦           +---Shared
¦   +---Content
¦   ¦   +---themes
¦   ¦       +---base
¦   ¦           +---images
¦   ¦           +---minified
¦   ¦               +---images
¦   +---Controllers
¦   ¦   +---Apis
¦   +---Filters
¦   +---Formatters
¦   +---Images
¦   +---Models
¦   +---Properties
¦   +---Scripts
¦   +---Views
¦       +---Account
¦       +---Home
¦       +---Shared
+---ContactManager.Phone
¦   +---Models
¦   +---Properties
¦   +---SampleData
¦   +---ViewModels
+---ContactManager.Tests
¦   +---Controllers
¦   +---Properties
+---ContactManager.WindowsStore
    +---Assets
    +---Common
    +---DataModel
    +---Properties

* Build:
  - Removed ContactManager.Phone, ContactManager.WindowsStore
  - Before build: 10.7MB
  - After build:  91.9MB
  - Packages: AspNetWebAPI.*, DotNetOpenAuth.*
  - Tests: {Delete, GetContact, GetContacts, Post}
    <= All pass
  - http://localhost:8081
    <= Beautifully laid out

* Projects: ContactManager (WebAPI + ASP.Net/MVC), ContactManager.Tests
    Removed: ContactManager.Phone, ContactManager.WindowsStore

* Models\*.cs: System.Data.Entity;
    Web.config: Data Source=(localdb)\v11.0
 Directory of D:\paul\proj\ContactsApp\msdn-webapi\Contact Manager Web API\C#\ContactManager\App_Data
11/05/2019  11:34 AM         3,211,264 aspnet-ContactManager-20120803164346.mdf
11/05/2019  11:26 AM         3,211,264 ContactManagerContext-20120803165354.mdf

* packages.config: 
AspNetWebApi.Core,
DotNetOpenAuth.AspNet,DotNetOpenAuth.Core",DotNetOpenAuth.OAuth.*,DotNetOpenAuth.OpenId.*,
EntityFramework,Microsoft.AspNet.Mvc,Microsoft.AspNet.Razor
Microsoft.AspNet.WebApi.*,
Microsoft.AspNet.WebPages,Microsoft.AspNet.WebPages.Data,Microsoft.AspNet.WebPages.OAuth,Microsoft.AspNet.WebPages.WebData,
jQuery,jQuery.UI.Combined,jQuery.Validation,
knockoutjs,
WebGrease

  
