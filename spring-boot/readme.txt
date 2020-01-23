* Spring Boot REST server
  - Models:
    - Contact {
        contactId?: number;
        name: string;
        eMail: string;
        phone1: string;
        phone2: string;
        address1: string;
        address2: string;
        city: string;
        state: string;
        zip: string;
        notes: Note[];
      }

    - Note {
        noteId?: number;
        text: string;
        date: Date;
        Contact: contact;
      }  
      
  - API:
    - GetContacts:   GET https://localhost:44362/api/Contacts
    - GetContact:    GET https://localhost:44362/api/Contacts/:id
    - UpdateContact: PUT https://localhost:44362/api/Contacts
    - AddContact:    POST https://localhost:44362/api/Contacts/:id
    - DeleteContact: DELETE https://localhost:44362/api/Contacts/:id

  - Architecture:
    - Model: JPA domain model, Hibernate persistence layer
    - View: NONE: Headless REST service only (allowing any REST client UI)
    - Controller: Combined Spring-based ServiceController 
   
1. Create project:
   - Spring Initializr: https://start.spring.io/
       Project= Maven, Language= Java, Spring Boot= 2.2.4, 
       Group= com.example, Artifact= contactsapp,
       Name= SpringContactsApp, Packaging= .war, Java= 8, 
       Dependencies= {Spring Web, Spring Data JPA, H2 Database, Spring Rest Docs}
       [Generate] => contactsapp.zip
       <= Extract contactsapp/ to $PROJ/ContactsApp/spring-boot

   - Eclipse > File > Import > Existing Maven Projects >
       C:\paul\proj\ContactsApp\spring-boot\contactsapp
       <= ERROR: A resource exists with a different case: '/ContactsApp'
     WORKAROUND: Changed directory name, artifactId and name to "SpringContactsApp":
	 - pom.xml:
        <groupId>com.example</groupId>
	    <artifactId>SpringContactsApp</artifactId>
	    <version>0.0.1-SNAPSHOT</version>
	    <packaging>war</packaging>
	    <name>SpringContactsApp</name>

    - Initial project:
      +--SpringContactsApp[boot]
        +--src/main/java/com/example/contactsapp
        |   ServletInitializer.java, SpringContactsAppApplication.java
        +--src/main/resources
        |   application.properties
        +--src/main/webapp
        +--src/test
        pom.xml

2. Create initial scaffolding for API Service:
   - Create Test (initial draft):
       Eclipse > SpringContactsApp > src > main > java > com.example.contactsapp > New class >
@RunWith(SpringRunner.class)
@WebMvcTest(ContactsAppController.class)
class ContactsAppControllerTest {

	@Test
	void test() {
		fail("Not yet implemented");
	}
    <= NO-GO: Need "ContactsAppController" 

   - Create ContactsAppController skeleton:
       Eclipse > SpringContactsApp > src > main > java > com.example.contactsapp > New Java JUnit Test Case >
         Package= com.example.contactsapp.controllers, Name= ContactsAppControllerTest >

         Package= com.example.contactsapp.controllers, Name= ContactsAppController >
@RestController
public class ContactsAppController {
	
	private final ContactsAppRepository contactsAppRepository;
	
	@Autowired
	public ContactsAppController(final ContactsAppRepository contactsAppRepository) {
		this.contactsAppRepository = contactsAppRepository;
	}
    <= NO-GO: Need "ContactsAppRepository" 

  - Create ContactsAppRepository skeleton:
       Eclipse > SpringContactsApp > src > main > java > com.example.contactsapp > New interface >
         Package= com.example.contactsapp.repositories, Name= ContactsAppRepository >
public interface ContactsAppRepository {
	; // TBD
}         
    <= OK: At this point, everything compiles

   - Implement ContactsAppController (first cut):
@RunWith(SpringRunner.class)
@WebMvcTest(ContactsAppController.class)
class ContactsAppControllerTest {

	@MockBean
	private ContactsAppController contactsAppController;
	
	@BeforeAll
	public static void setup () {
		; // TBD
	}

	@Test
	void test() {
		fail("Not yet implemented");
	}

  - Eclipse > Run > Debug Configurations > JUnit > New > 
      Project= SpringContactsApp, Test class= com.example.contactsapp.controllers.ContactsAppControllerTest, All Methods= Y >
      [Run]
      <= Invokes Spring runtime > Test Fails >
          FAILURE: org.opentest4j.AssertionFailedError: Not yet implemented: 
          PERFECT!  Exactly what we want!





      