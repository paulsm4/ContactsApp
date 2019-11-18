* ContactsApp: Asp.Net MVC5/Entity Framework 6/Razor UI

* Initial build:
  - MSVS > File > New Project > Asp.Net Web App >
      Name= ContactsApp, Create directory for solution= N,
	  Asp.Net Template= MVC, Add Unit Tests= Y, Azure= N, Authentication= None
  << Auto-generates scaffolding for an entire, runnable web site >>

  - Create model (POCOs):
    - MSVS > Models > Add Class > Contact.cs >
        namespace ContactsApp.Models {
             public class Contact {
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
	  <= NOTE: declared as "List" to ensure we can access notes by array[] index

    - MSVS > Models > Add Class > Note.cs >
        namespace ContactsApp.Models {
            public class Note {
                public Note () {
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
	  <= NOTE: You *CANNOT* use "[Default]" data annotation with GETDATE() in EF6; hence constructor...

    << This schema will automagically "join" "Notes" table on "Contacts" >>

  - Auto-generate MVC "Controller" and E/F "DB Context":
    - MSVS > Controllers > Add Controller >
	    Scaffolding= MVC5 Controller with views, using Entity Framework >
        Model class= Contact, Data context class= ContactsDB, Name= ContactsController,
        Use async controller actions= Y, Generate Views= Y, Reference script libraries= Y, Use a layout page= Y
        [Add]
      <= Auto-generates:
	       Controllers/ContactsController.cs
		   Models/ContactsDB.cs
		   Views/Contacts/{Create, Delete, Edit, Index}.cshtml

  << Auto-generated scaffolding is also completely runnable: http://localhost:54657/Contacts >>

  - Add a "DB Initializer" for a "Seed Database":
    - MSVS > Models > Add Class >
	    Name= ContactsDBIntializer
		Inherits from= System.Data.Entity.CreateDatabaseIfNotExists<ContactsDB>
		overrides protected override void Seed(ContactsDB context)

	  NOTE: OOTB EF provides three initializers
	  - CreateDatabaseIfNotExists
	  - DropCreateDatabaseWhenModelChanges
	  - DropCreateDatabaseAlways

    - Add "SetInitializer()" to Global.asax, Application_Start()
	  ... OR ...
	  Add entry to Web.Config:
	  ...
	    <entityFramework>
          <contexts>
            <context type="ContactsApp.Models.ContactsDB, ContactsApp">
              <databaseInitializer type="ContactsApp.Models.ContactsDBInitializer, ContactsApp" />

  << Auto-generates LocalDB "App_Data\ContactsDB-20191115123059.mdf >>

  - Add Unit Tests
    - MSVS > ContactsApp.Tests > Add Unit Test >
	    Name= ContactsDBTests
	- ContactsDBTests.cs:
namespace ContactsApp.Tests {
    [TestClass]
    public class ContactsDBTests  {
        [TestInitialize]
        public void SetupTest()
        {
            Database.SetInitializer(new ContactsApp.Models.ContactsDBInitializer());
            ...
        [TestMethod]
        public async Task TestInitializerSeed() {
            using (ContactsDB db = new ContactsDB()) {
                List<Contact> contactsList = await db.Contacts.ToListAsync();
                Assert.IsTrue(contactsList.Count > 0, "there will be one record in newly initialized database");
                Contact contact = contactsList[0];
                Assert.IsTrue(contact.Name == "abc", "initial contact Name \"abc\"");
                Assert.IsTrue(contact.EMail == "abc@xyz.com", "initial contact EMail \"abc@xyz.com\"");

	  NOTES: Must do the following:
	  - Explicitly install Entity Framework in Nuget
	    <= The EF install in the main project does *NOT* apply to ContactsApp.Tests subproject

	  - Add connection string for LocalDB, and explicit filepath:
	      App.config:
<configuration>
  ...
  <connectionStrings>
    <add name="ContactsDB" connectionString="Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=ContactsDB-20191115123059; Integrated Security=True; MultipleActiveResultSets=True; AttachDbFilename=D:\paul\proj\ContactsApp\aspnet-mvc5\ContactsApp\App_Data\ContactsDB-20191115123059.mdf" providerName="System.Data.SqlClient" />
  </connectionStrings>

	  - Add "Initializer" stanza to "entityFramework" stanza:
	      App.config:
<configuration>
  ...
  <contexts>
    <context type="ContactsApp.Models.ContactsDB, ContactsApp">
      <databaseInitializer type="ContactsApp.Models.ContactsDBInitializer, ContactsApp" />
  ...	  
	
  << At this point, we can execute any unit tests, including EF-based tests >>

* TBD:
  - Implement Add/Delete/Update "notes".
  - Implement Partial Views (eliminate duplicate "Notes" UI in Edit and Delete views).
  - Extend unit tests.
  - Update "readme.txt" with all relevant details.
