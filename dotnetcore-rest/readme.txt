* Create ASP.NET Core REST API
  ----------------------------
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
        public virtual ICollection<Note> Notes { get; set; }
    }
    ...

   - Note.cs:
    public class Note
    {
        public Note()
        {
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

3. Add EF DBContext and MVC  Controller:
   - MSVS > New Controller > API Controller with actions, using EF > 
         Model class > Contact, 
         Data context class > (+) > ContactsApp.Models.ContactsContext,
         Controller name= ContactsController

   - The MSVS "New Controller" template auto-generates:
     - Controllers\ContactsController.cs
       <= async Task<ActionResult> methods for all CRUD operations: GET, PUT (update), POST (add), DELETE
          "ContactsContext context" DI via ContactsController() constructor

   - Models\ContactsContext.cs:
    public class ContactsContext : DbContext
    {
        public ContactsContext(DbContextOptions<ContactsContext> options) : base(options)
        {
        }
        public DbSet<ContactsApp.Models.Contact> Contacts { get; set; }
        public DbSet<ContactsApp.Models.Note> Notes { get; set; }
        <= Manually exposed *both* "Contact" (main table) and "Notes" ("related data")

   - Default controller actions:
[Route("api/[controller]")]
[ApiController]
public class ContactsController : ControllerBase
    private readonly ContactsContext _context;
    public ContactsController(ContactsContext context) {
        _context = context;
        ...
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

    <<  Verified everything compiles cleanly at this point  >>
           
4. Deleted MSVS auto-generated "WeatherForecast" scaffolding:
   - { WeatherForecastController.cs, WeatherForecast.cs }
   
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

   - Add > Unit Test > IntegrationTests
     <= Let's focus on this for now...

   - Postman:
     - GET  http://localhost:53561/api/Contacts
       - Response: HTTP 200 (OK)
          []

     - POST http://localhost:53561/api/Contacts
         {
	       "Name":"Sy Snootles",
	       "EMail":"ss@abc.com"
         }
       - Response: HTTP 201 (Created)
         {
             "contactId": 1,
             "name": "Sy Snootlea",
             "eMail": "ss@abc.com",
             "phone1": null,
             "phone2": null,
             "address1": null,
             "address2": null,
             "city": null,
             "state": null,
             "zip": null,
             "notes": null
         }

     - GET  http://localhost:53561/api/Contacts
       - Response: HTTP 200 (OK)
         [
             {
               "contactId": 1,"name": "Sy Snootlea","eMail": "ss@abc.com","phone1": null,"phone2": null,
               "address1": null,"address2": null,"city": null,"state": null,"zip": null,"notes": null
             }
         ]

     - PUT http://localhost:53561/api/Contacts/1  // BAD
        { "Name":"Salacious Crumb" }
        <= Values passed to ContactsController.PutContract():
           id= 1 (OK), contact= {0, "Salacious Crumb", null, null, ...} > (id != contact.ContactId > return BadRequest()
       - Response: HTTP 400 (Bad Request)
         {
             "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
             "title": "Bad Request",
             "status": 400,
             "traceId": "|313754e1-456144caa750fef9."
         }

     - PUT http://localhost:53561/api/Contacts/1  // OK
        { "contactId": 1, "Name":"Salacious Crumb" }
       - Response: HTTP 204 (No Content)

6. Extended "GetContacts()" controller endpoint to allow for "Get by Name":
   - ContactsController.cs:
       [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> GetContacts()
        {
            var queryParams = HttpContext.Request.Query;
            if (queryParams.Count == 0)
            {
                return await _context.Contacts.ToListAsync();
            }
            var param = queryParams.First();
            if ("name" == param.Key)
            {
                string targetName = "%" + param.Value + "%";
                var query = from c in _context.Contacts
                            where EF.Functions.Like(c.Name, targetName)
                            select c;
                return query.ToList();
            }
            return BadRequest();
        }
    
    - PostMan:
      - GET  http://localhost:53561/api/Contacts
         - Response: HTTP 200 (OK), JSON= []
      - POST http://localhost:53561/api/Contacts
         { "Name":"Sy Snootle", "EMail":"ss@abc.com" }
         - Response: HTTP 201 (Created), JSON= { "contactId": 1,"name":"Sy Snootle", ...}
      - GET  http://localhost:53561/api/Contacts/?name=Snood
         <= Works perfectly!
        - Response: HTTP 200 (OK), JSON= { "contactId": 1,"name":"Sy Snootle", ...}

7. Reconfigure project for persistent LocalDB (vs. transient in-memory DB):
   - Startup.cs:
       public void ConfigureServices(IServiceCollection services) {
           services.AddDbContext<ContactsContext>(opt => opt.UseInMemoryDatabase(databaseName: "ContactsDB"));
           ...
       <= Change this!

     - appsettings.json:
         ...
         "ConnectionStrings": {
           "ContactsDB": "Server=(localdb)\\mssqllocaldb;Database=ContactsDB;Trusted_Connection=True;MultipleActiveResultSets=true"
         }

   - Startup.cs:
       public void ConfigureServices(IServiceCollection services) {
         services.AddDbContext<ContactsContext>(opt => 
           opt.UseSqlServer(Configuration.GetConnectionString("ContactsDB")));

   - Nuget > PM Console >
https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/migrations?view=aspnetcore-3.0
     - Add-Migration InitialCreate -Verbose 
        <= Auto-generates "Migrations\20191128012707_InityialCreate.cs", with Up() and Down() methods
           Creates/drops Contacts and Notes tables...
 
     - Update-Database -Verbose
       <= Database created %USERPROFILE%\ContactsDB.mdf

     - Ran Postman tests
       <= Verified data now persisted between sessions...

8. Integration tests (continued from Step 5 above):
   - run.bat:
       @rem Allows pre-starting ContactsApp REST service before running MSTest suite
	   cd bin\Debug\netcoreapp3.0
       start /B /WAIT ContactsApp.exe
	   
   - MSVS > Tools > Add Tool >
       Title= Run REST Service, Cmd= run.bat
   ----------------------------------------------------------------------------
   - PROBLEM: 
       Add/List/Update/Delete "Contacts" works OK...
       ... but unable to add or update any "notes" as part of adding/updating a "contact"
       <= Added EFSaving 

   - SOLUTION: Remove "cicular reference" from Note.cs:
       public class Note {
           public Note(){
               this.Date = DateTime.Now; // Default value: local "now"
           }
           [Key]
           [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
           public int NoteId { get; set; }
           public string Text { get; set; }
           public DateTime Date { get; set; }
           [ForeignKey("Contact")]
           public int ContactId { get; set; }
           //public Contact Contact { get; set; }  // <-- Removed this!
           <= Not sure why we needed it in the first place ... 
		      ... but it worked OK in ASP.Net/MVC, and we just carried it forward.

     <<See "nested-data.txt" for further details>>
   ----------------------------------------------------------------------------
  - public class IntegrationTests
        public  void TestContacts ()
            // Query for all existing test records: GET  http://localhost:5000/api/Contacts
               <= OK
            // Ensure Test Record does not exist
               <= PurgeTestContacts(): OK
                  "Test-Contact" record(s) and all corresponding Notes records DELETED
            // CREATE: Create new contact: POST  http://localhost:5000/api/Contacts
               <= OK
            // READ: verify contact created: GET  http://localhost:5000/api/Contacts/1
               <= OK
            // UPDATE: Change a field, Add some notes: PUT http://localhost:5000/api/Contacts/1
               <= OK: "Phone2" set correctly, two new fields added
            // DELETE Contact http://localhost:5000/api/Contacts/8
                <= OK
            // Re-read record (verify "delete")
                <= OK
    << All integration tests now passing >>
    - Updated Git...

===================================================================================================

* Current subprojects:
  - ContactsApp:  .Net Core REST service
  - ContactsApiTests: MSTest unit/integration tests
  - EFSaving: EF Core/Related data example
      https://docs.microsoft.com/en-us/ef/core/saving/related-data
  - ContosoUniversity: EF Core tutorials:
      https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/?view=aspnetcore-3.0

* Added "MinimalEF" subproject
  - MSVS > Add New > Project > Console App >
      Name= MinimalEF

  - Add > Class >
      Models\{Contact.cs, Note.cs, ContactsContext2.cs}

  - Models\Contact.cs:
   public class Contact
    {
        public string Name { get; set; }
        public string EMail { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public virtual ICollection<Note> Notes { get; set; }
    }

  - Models\Note.cs:
    public class Note
    {
        public Note()
        {
            this.Date = DateTime.Now; // Default value: local "now"
        }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public int ContactId { get; set; }
    }

  - Models\ContactsContext2.cs:
    class ContactsContext2 : DbContext {
        //// This constructor allows .Net Core runtume to inject DBContext options
        //public ContactsContext2(DbContextOptions<ContactsContext2> options) : base(options) { ; }

        // For standalone (console) app, we'll set options by overriding OnConfiguring()
        public ContactsContext2() { ; }

        public DbSet<MinimalEF.Models.Contact> Contacts { get; set; }
        public DbSet<MinimalEF.Models.Note> Notes { get; set; }

        // For standalone app, we'll hard-code connection string here, vs. reading from application.json
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
             optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=ContactsDB2;Trusted_Connection=True");
            ...

  - Nuget > Install MicrosoftEntityFrameworkCore
    <= NOTE: *ALSO* needed to manually install Microsoft.EntityFrameworkCore.SqlServer for LocalDB to work...

  - Add Contacts2.cs:
    <= Wraps functionality
    - Contacts2.cs:
    public class Contacts2 {
        private ContactsContext2 _context;
        public Contacts2 () {
            using (_context = new ContactsContext2()) {
                _context.Database.EnsureCreated();
            ... <= No-args constructor, call "EnsureCreated()", depends on "OnConfiguring()" in DBContext

    - FIRST PROBLEM:
      Error	CS1061	'DbSet<Contact>' does not contain a definition for 'ToList'...
      SOLUTION: add "using System.Linq;"

  - Update Program.cs:
    class Program {
        static void Main(string[] args) {
            Contacts2 contacts2 = new Contacts2();
            var contacts = contacts2.GetAllContacts();
            ...

  - Set bkpts in Program/Main, Contacts2, ContactsContext2 >
      MSVS > Debug >

    - NEXT PROBLEM:
      System.InvalidOperationException: The entity type 'Contact' requires a primary key to be defined.
      If you intended to use a keyless entity type call 'HasNoKey()'
      <= OK: So "MinimalEF" needs to be just a little less "minimal"...

    - Updated Contact.cs:
    public class Contact {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public virtual ICollection<Note> Notes { get; set; }
        <= "ID" automagically recognized as Primary key; DB generation creates an MSSQL clustered index on it...

    - Updated Note.cs:
    public class Note {
        [Key]
        public int ID { get; set; }
        public Note()
        {
            this.Date = DateTime.Now; // Default value: local "now"
        }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public int ContactID { get; set; }
        <= Note "ID" also automagically recognized as Primary key...
           "ContactID automagically recognized as a Foreign Key (no extra annotations required)

    - NOTE:
      - By wrapping _context in "using" (open/close) for each operation...
        ... OntactsContext2.OnConfiguring() also gets called over and over again for each operation...

  - Updated Contacts2.cs; added Get Contact Details:
        public Contact GetContact(int id) {
            Contact contact = null;
            using (_context = new ContactsContext2()) {
                contact =
                    _context.Contacts.Include(c => c.Notes).First(c => c.ID == id);
            }
            return contact;

    - NEXT PROBLEM:
      Error CS1061: 'DbSet<Contact>' does not contain a definition for 'Include'...
      SOLUTION: need all of the following assemblies:
using Microsoft.EntityFrameworkCore;
using MinimalEF.Models;
using System.Collections.Generic;
using System.Linq;
      <= OK

  - Contact.cs; print contact:
    public class Contact {
        ...
        public override string ToString () {
            string jsonText = JsonConvert.SerializeObject(this);
            return jsonText;
            ...

  - Contacts2.cs; add new contact:
        public int AddContact(Contact contact) {
            using (_context = new ContactsContext2()) {
                _context.Contacts.Add(contact);
                _context.SaveChanges();
                var c = _context.Entry(contact);
                return c.Entity.ID;
                ...

  - Contacts2.cs: update contact:
        public void UpdateContact(Contact contact) {
            using (_context = new ContactsContext2()) {
                // Fetch current version in DB
                var contactToUpdate =
                    _context.Contacts.Include(c => c.Notes).First(c => c.ID == contact.ID);

                // Update properties of the parent
                _context.Entry(contactToUpdate).CurrentValues.SetValues(contact);
                ...
                << "Update Contact record": So far, so good. But "Update Notes Records" leaves a bit to be desired... >>
                _context.SaveChanges();
                ...

  - Contacts2.cs: delete contact:
        public void DeleteContact(int id) {
            using (_context = new ContactsContext2()) {
                var contact =
                    _context.Contacts.Include(c => c.Notes).First(c => c.ID == id);
                _context.Contacts.Remove(contact);

