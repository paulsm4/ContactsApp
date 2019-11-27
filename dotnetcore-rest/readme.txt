* Create ASP.NET Core REST API:
1. Create project:
   - MSVS > Create project > 
      Language= C#, Template= ASP.Net Core Web Application >
      Project name= ContactsApp, [Create]
      .Net Core= Y, ASP.Net Core= 3.0, Angular= N, [Create]

2. Add models:
   - New folder > Models >
       New class > { Contact.cs, Note.cs }

   - Contact.cs:
    public class Contact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContactId { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public virtual List<Note> Notes { get; set; }
    }
    ...

   - Note.cs:
public class Note
    {
        public Note()
        {
            this.Date = DateTime.Now; // Default value: local "now"
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NoteId { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        [ForeignKey("Contact")]
        public int ContactId { get; set; }
        public virtual Contact Contact { get; set; }
    }

3. Add EF DBContext and MVC  Controller:
       New Controller > API Controller with actions, using EF > 
         Model class > Contact, 
         Data context class > (+) > ContactsApp.Models.ContactsContext,
         Controller name= ContactsController
   - The MSVS "New Controller" template auto-generates:
     - Controllers\ContactsController.cs
       <= async Task<ActionResult> methods for all CRUD operations: GET, PUT (update), POST (add), DELETE
          "ContactsContext context" DI via ContactsController() constructor
     - Models\ContactsContext.cs

   - Default controller actions:
[Route("api/[controller]")]
[ApiController]
public class ContactsController : ControllerBase

[HttpGet]             // List all
public async Task<ActionResult<IEnumerable<Contact>>> GetContacts()
[HttpGet("{id}")]     // Get one
public async Task<ActionResult<Contact>> GetContact(int id)
[HttpPut("{id}")]     // Update
public async Task<IActionResult> PutContact(int id, Contact contact) 
[HttpPost]            // Add new
public async Task<ActionResult<Contact>> PostContact(Contact contact)  
[HttpDelete("{id}")]  // Delete
public async Task<ActionResult<Contact>> DeleteContact(int id)
private bool ContactExists(int id)

   - Manually added "notes" APIs:
[HttpPut("Notes/{id}")]
public async Task<IActionResult> PutNoteId(int noteId, Note note)
[HttpPost("/Notes")]
public async Task<ActionResult<Contact>> PostNote(Note note)
[HttpDelete("/Notes/{id}")]
public async Task<ActionResult<Contact>> DeleteNote(int noteId)

   - Manually added "Notes" to Models\ContactsContext.cs for this to work:
    public class ContactsContext : DbContext {
        public ContactsContext(DbContextOptions<ContactsContext> options) : base(options) {}

        public DbSet<ContactsApp.Models.Contact> Contacts { get; set; }
        public DbSet<ContactsApp.Models.Note> Notes { get; set; }
    }
    <<  Verified everything compiles cleanly at this point  >>
           
4. Deleted MSVS auto-generated "WeatherForecast" scaffolding:
   - { WeatherForecastController.cs, WeatherForecat.cs }
   
   - Properties\launchSettings.json:
{
  "$schema": "http://json.schemastore.org/launchsettings.json",
  "iisSettings": {
    "windowsAuthentication": false,
    "anonymousAuthentication": true,
    "iisExpress": {
      "applicationUrl": "http://localhost:53561",
      "sslPort": 0
    }
  },
  "profiles": {
    "IIS Express": {
      "commandName": "IISExpress",
      "launchBrowser": true,
      "launchUrl": "",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "ContactsApp": {
      "commandName": "Project",
      "launchBrowser": true,
      "launchUrl": "",
      "applicationUrl": "http://localhost:5000",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}

5. Created a unit test subproject:
   - MSVS > File > New > Project >
       Template= MSTest Test Project (.Net Core)
       Name= ContactsApiTests, Solution= Add to solution
       [Create]

   - ContactsApiTests > Add > Reference >
       Projects > Add ContactsApp

   - Rename UnitTest1 => ContactTests

   - Add > Unit Test > NoteTests
     <= Unable to "Add unit test" per se.  
        WORKAROUND: Added class (NoteTests); manually added "using Microsoft.VisualStudio.TestTools.UnitTesting;", [TestClass] and  [TestMethod]
    << File > Save All; Build > Rebuild All => Verified OK >>

   - Test > Run >
     <= Success ... but only found "ContactTests" (not "NoteTests")
     <= SOLUTION: Needed to make "class NoteTests" *public*!!!

       
