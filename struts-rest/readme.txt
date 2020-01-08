Get* Struts REST API
  - References:
      https://struts.apache.org/plugins/rest/
      https://www.baeldung.com/struts-2-intro
      https://www.concretepage.com/struts-2/struts-2-rest-web-service-integration-example
      https://struts.apache.org/getting-started/hello-world-using-struts2.html
      https://struts.apache.org/getting-started/how-to-create-a-struts2-web-application.html
      https://struts.apache.org/plugins/junit/
      https://struts.apache.org/plugins/convention/
      http://learningprogramming.net/java/struts-2/read-data-from-database-with-spring-data-jpa-in-struts-2/

  - GAMEPLAN:
    - Server: Tomcat 8/Eclipse IDE
       DB: H2/file
       Maven/pom.xml
       Java 8
  
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
        contactId?: number;
      }

  - API:
    - GetContacts:   GET https://localhost:44362/api/Contacts
    - GetContact:    GET https://localhost:44362/api/Contacts/:id
    - UpdateContact: PUT https://localhost:44362/api/Contacts
    - AddContact:    POST https://localhost:44362/api/Contacts/:id
    - DeleteContact: DELETE https://localhost:44362/api/Contacts/:id

===================================================================================================
* Subprojects:
  - ContactsApp/winforms-ui:
      ContactsAppWinforms
      <= C#/.Net/NewtonSoft JSON UI

  - ContactsApp/struts-rest:
      soapui
      <= SoapUI REST requests/responses

===================================================================================================
* Start project:

1. Environment:
   - Project folder: 
       $PROJ/ContactsApp/struts-rest
       readme.txt (this file)
       struts-2-rest-web-service-integration-example.zip (reference example)
   - Git:
     - git pull --all; git status
       <= Verify everything's in sync
     - git  git checkout -b struts-rest; git status
       <= Verify we're on the new "struts-rest" branch, everything's clean
        
   - Update Eclipse:
     https://wiki.eclipse.org/FAQ_How_do_I_upgrade_Eclipse_IDE%3F
     - Eclipse > Help > About >
         <= Version: 2018-09 (4.9.0)
     - Window > Preferences > Available S/W sites > 
        REMOVE: Eclipse 2018-09
        ADD: https://download.eclipse.org/releases/2019-09/
     - Help > Check for Updates >
        <= Select options, [Update]
        <<Restart Eclipse>>

   - Eclipse > Servers > Tomcat 
       <= Eclipse update scrubbed previous Tomcat v8.0 config, needed to re-create (from D:\Tomcat)
     << Note: ultimately wound up using Tomcat 9 >>

2. Create project:
   - File > New > New Maven Project >
       Folder= D:\paul\proj\ContactsApp\struts-rest\ContactsApp, 
       Archetype= maven-archetype-webapp
       GroupID= com.example, ArtifactId= ContactsApp, Package= com.example.contactsapp
       <= Creates struts-rest\ContactsApp Eclipse/Maven project
          NOTE: Strictly speaking, we *DIDN'T* need the webapp archetype... It just makes things easier...

   - pom.xml:
     -------
  <modelVersion>4.0.0</modelVersion>
  <groupId>com.example.contactsapp</groupId>
  <artifactId>ContactsApp</artifactId>
  <version>1.0</version>
  <name>Contacts App</name>
  <packaging>war</packaging>
  ...
  <properties>
    <project.build.sourceEncoding>UTF-8</project.build.sourceEncoding>
    <struts2.version>2.5.22</struts2.version>
    <hibernate.version>5.4.0.Final</hibernate.version>  
    <h2.version>1.4.197</h2.version>
    <log4j2.version>2.12.1</log4j2.version>
    <maven-war-plugin.version>3.2.2</maven-war-plugin.version>
    <maven-compiler-plugin.version>3.8.0</maven-compiler-plugin.version>
    <maven.compiler.target>1.8</maven.compiler.target>
    <maven.compiler.source>1.8</maven.compiler.source>
    ...
  <dependencies>
    <dependency>
        <groupId>org.apache.struts</groupId>
        <artifactId>struts2-core</artifactId>
        <version>${struts2.version}</version>
        ...
    <dependency>
        <groupId>org.apache.struts</groupId>
        <artifactId>struts2-convention-plugin</artifactId>
        ...
    <dependency>
        <groupId>org.apache.struts</groupId>
        <artifactId>struts2-rest-plugin</artifactId>
        ...
    <dependency>
        <groupId>com.h2database</groupId>
        <artifactId>h2</artifactId>
        ...
    <dependency>
        <groupId>org.apache.logging.log4j</groupId>
        <artifactId>log4j-core</artifactId>
        ...
    <dependency>
        <groupId>org.apache.logging.log4j</groupId>
        <artifactId>log4j-api</artifactId>
        ...
    <dependency>
        <groupId>com.h2database</groupId>
        <artifactId>h2</artifactId>
    <= See https://mvnrepository.com/ for current versions of struts2-convention-plugin, struts2-rest-plugin, h2, junit
       Note: did *NOT* restrict h2 to "scope test"
        ...
    <dependency>
        <groupId>org.hibernate</groupId>
        <artifactId>hibernate-core</artifactId>
    <= Persistence APIs and implementation
        ...
    <dependency>
        <groupId>org.apache.logging.log4j</groupId>
        <artifactId>log4j-api</artifactId>
        ...
    <dependency>
        <groupId>org.apache.logging.log4j</groupId>
        <artifactId>log4j-core</artifactId>
        ...
  <build>
    <finalName>StrutsContactsApp</finalName>
    <plugins>
        <plugin>
          <groupId>org.apache.maven.plugins</groupId>
          <artifactId>maven-compiler-plugin</artifactId>
          <version>${maven-compiler-plugin.version}</version>
          <configuration>
            <source>${maven.compiler.source}</source>
            <target>${maven.compiler.target}</target>
            ...
    <= NOTE: Explicit "maven-compiler-plugin" version info is necessary.
             See "default JRE 1.5" issue below...

   - Eclipse > Maven > Update Project >
       <= OK

   - Eclipse > Build Path >
      <= Add Tomcat runtime library

   - Eclipse > Project > Properties >
      - Facets > Java
        <= Should be JRE 1.8
      - Java Compiler > Settings
        <= Should be JRE 1.8
      - Build Path > Libraries > Add > Tomcat 8
        <= Servlet libs
      - Deployment Assembly
        <= Ensure /src/main/{java/, resources/, webapp/} all included

3. Scaffold app:
   - New > Class >
       com.example.contactsapp.models, Contact
       com.example.contactsapp.models, Note
       com.example.contactsapp.controllers, ContactsController
          public class ContactsController implements ModelDriven<Contact> { ... }
          @Override
          public Contact getModel() { ... }
       com.example.contactsapp.repositories, ContactsRepository
          public interface ContactsRepository { ... }
       com.example.contactsapp.repositories, ContactsRepositoryImpl
          public class ContactsRepositoryImpl implements ContactsRepository { ... }

    - src > main > resources > struts.xml:
      -----------------------------------
        <?xml version="1.0" encoding="UTF-8" ?>
        <!DOCTYPE struts PUBLIC
            "-//Apache Software Foundation//DTD Struts Configuration 2.3//EN"
            "http://struts.apache.org/dtds/struts-2.3.dtd">
        <struts>
            <constant name="struts.convention.action.suffix" value="Controller"/>
            <constant name="struts.convention.action.mapAllMatches" value="true"/>
            <constant name="struts.convention.default.parent.package" value="rest-default"/>
            <constant name="struts.convention.package.locators" value="controllers"/>
        </struts>

    - src > main > webapp > WEB-INF > web.xml:
      ---------------------------------------
        <web-app>
          <display-name>Struts Contacts App</display-name>
          <filter>
                <filter-name>struts2</filter-name>
                <filter-class>org.apache.struts2.dispatcher.filter.StrutsPrepareAndExecuteFilter</filter-class>
            </filter>
          <filter-mapping>
            <filter-name>struts2</filter-name>
            <url-pattern>/*</url-pattern>
          </filter-mapping>
        </web-app>
          <= NOTE: 
               OLD:          org.apache.struts2.dispatcher.ng.filter.StrutsPrepareAndExecuteFilter
               Struts 2.5++: org.apache.struts2.dispatcher.filter.StrutsPrepareAndExecuteFilter

   - Test Build:
       Eclipse > pom.xml > Run as > Maven Build >
        Goals: clean install
        <= BUILD SUCCESS

   << saved backup, updated Git >>
===================================================================================================
4. ContactsApp/Persistence (JPA-Hibernate):
https://www.javaguides.net/2019/11/hibernate-h2-database-example-tutorial.html
https://thoughts-on-java.org/implementing-the-repository-pattern-with-jpa-and-hibernate/

  - Contact.java:
    ------------
    @Entity
    @Table(name = "contacts")
    public class Contact {
    
        @Id
        @GeneratedValue(strategy=GenerationType.AUTO)
        private int contactId;
        private String name;
        private String email;
        private String phone1;
        private String phone2;
        private String address1;
        private String address2;
        private String city;
        private String state;
        private String zip;
        @Column
        @OneToMany(cascade = CascadeType.ALL, orphanRemoval = true, fetch = FetchType.EAGER)
        @JoinColumn(name="contactId")
        private List<Note> notes;
        ... <= Added getters/setters
               Note @OneToMany, @JoinColumn annotations for "Notes" table

  - Note.java:
    ---------
    Entity
    @Table(name = "notes")
    public class Note {
        @Id
        @GeneratedValue(strategy=GenerationType.AUTO)
        private int noteId;
        private String text;
        private Date date;
        private int contactId;
    ... <= Added getters/setters

  - pom.xml:
    -------
    <properties>
      <hibernate.version>5.4.10.Final</hibernate.version>   
        <h2.version>1.4.200</h2.version>
      ...
      <dependency>
        <groupId>com.h2database</groupId>
        <artifactId>h2</artifactId>
        <version>${h2.version}</version>
          ...
        <dependency>
            <groupId>org.hibernate</groupId>
            <artifactId>hibernate-core</artifactId>
            <version>${hibernate.version}</version>
          ...
      <= NOTE: "hibernate-entitymanager" is deprecated; use hibernate-core instead

   - src/main/resources/META-INF/persistence.xml
     -------------------------------------------
      <?xml version="1.0" encoding="UTF-8"?>
      <!DOCTYPE xml>
      <persistence version="2.0" xmlns="http://java.sun.com/xml/ns/persistence"
         xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
         xsi:schemaLocation="http://java.sun.com/xml/ns/persistence 
         http://java.sun.com/xml/ns/persistence/persistence_2_0.xsd">
         
         <persistence-unit name="ContactsApp_JPA" transaction-type="RESOURCE_LOCAL"> 
            <class>com.example.contactsapp.models.Contact</class>
            <properties>
               <property name="javax.persistence.jdbc.driver" value="org.h2.Driver"/>
               <property name="javax.persistence.jdbc.url" value="jdbc:h2:file:~/data/contactsapp"/>
               <property name="javax.persistence.jdbc.user" value="sa"/>
               <property name="javax.persistence.jdbc.password" value=""/>
               <property name="hibernate.dialect" value="org.hibernate.dialect.H2Dialect"/>
               <!-- validate|update|create|create-drop 
               <property name="javax.persistence.schema-generation.database.action" value="create"/>
               -->
               <property name="hibernate.hbm2ddl.auto" value="create" />
               <property name="show_sql" value="true"/>
            </properties>
         </persistence-unit>
      </persistence>

   - NOTES:
    - JPA runtime needs persistence.xml in META-INF.
      Copying to Eclipse Dynamic web apps project folder src > main > resources > META-INF will do this.
    - Maven project dependencies for Struts web app sufficient for standalone Java app (e.g. for static main scaffolding)

  - ContactsRepository.java:
    -----------------------
    public interface ContactsRepository {
      public List<Contact> getContacts();
      public Contact getContact(int id);
      public int addContact(Contact contact);
      public void deleteContact(int id);
      public void updateContact(Contact contact);
   }

  - ContactsRepositoryImpl.java (initial draft):
    -------------------------------------------
public class ContactsRepositoryImpl implements ContactsRepository {
    private static final String PERSISTENCE_UNIT = "ContactsApp_JPA";
    private EntityManagerFactory emf = Persistence.createEntityManagerFactory(PERSISTENCE_UNIT);
    
    public List<Contact> getContacts() {
        EntityManager em = null;
        List<Contact> contacts = null;
        try {
            em = emf.createEntityManager();
            em.getTransaction().begin();
            Query query = em.createQuery("FROM Contact");
            contacts = (List<Contact>)query.getResultList();
            em.getTransaction().commit();
        } finally {
            if (em != null)
                em.close();
        }
        return contacts;
    }

    public int addContact(Contact contact) {
        EntityManager em = null;
        Contact newContact = null;
        try {
            em = emf.createEntityManager();
            em.getTransaction().begin();
            em.persist(contact);
            em.getTransaction().commit();
            return contact.getContactId();
        } finally {
            if (em != null)
                em.close();
        }
    }
    ...

  - ContactsRepositoryImpl.java (standalone test scaffolding):
    ---------------------------------------------------------
    public static void main(String[] args) {
        try {
            // Connect to database
            ContactsRepository contactsRepository = new ContactsRepositoryImpl();

            // Query records
            List<Contact> contacts = contactsRepository.getContacts();
            System.out.println("getContacts: ct=" + contacts.size());

            // Add new record
            Contact newContact = new Contact();
            newContact.setName("Mickey Mouse");
            newContact.setEmail("mm@abc.com");
            int newId = contactsRepository.addContact(newContact);
            System.out.println("addContact(), new contactId=" + newId);
            contacts = contactsRepository.getContacts();
            System.out.println("getContacts: ct=" + contacts.size());

            // Update record
            newContact.setCity("Emerald City");
            newContact.setState("Oz");
            newContact.setZip("00000");
            ...
            // Fetch record
            Contact updatedContact = contactsRepository.getContact(newId);
            System.out.println("getContact(" + newId + "): " + updatedContact.toString());

            // Delete record
            contactsRepository.deleteContact(newId);
            contacts = contactsRepository.getContacts();
            System.out.println("getContacts: ct=" + contacts.size());
            for (Contact contact: contacts) {
                System.out.println(contact.toString());
            }

        } catch (Exception e) {
            System.out.println("ERROR: " + e.getMessage());
            e.printStackTrace();
        }
    }
}  <= Verified OK

===================================================================================================
5. Implement controller (aka "Action")

   - ContactsApp/ContactsController.java:
     -----------------------------------
/**
 * Default URL mappings (https://struts.apache.org/plugins/rest/):
 * - index:   GET request with no id parameter.
 * - show:    GET request with an id parameter.
 * - create:  POST request with no id parameter and JSON/XML body
 * - update:  PUT request with an id parameter and JSON/XML body. 
 * - destroy: DELETE request with an id parameter. 
 * - edit:    GET  request with an id parameter and the edit view specified. 
 * - editNew: GET  request with no id parameter and the new view specified.
 */
public class ContactsController implements ModelDriven<Object> {

    private static final long serialVersionUID = 1L;
    private String id;
    private Object model;
    private ContactsRepository contactsRepository = new ContactsRepositoryImpl();

    @Override
    public Object getModel() {
        return model;
    }
    
    public HttpHeaders index () {
        model = contactsRepository.getContacts();
        return new DefaultHttpHeaders("index").disableCaching();
    }
    
    public HttpHeaders show() {
        int contactId = Integer.parseInt(id);
        model = (Object)contactsRepository.getContact(contactId);
        return new DefaultHttpHeaders("show");
    }
    
    public HttpHeaders create() {
        Contact contact = (Contact)model;
        contactsRepository.addContact(contact);
        return new DefaultHttpHeaders("success");
    }
    ...

===================================================================================================
6. Initial test drive:

  - Eclipse > Servers > Tomcat > Debug >
    - http://localhost:8080/StrutsContactsApp => "Hello world!"
      <= Web app works!
    - http://localhost:8080/StrutsContactsApp/contacts => HTTP 400
    - http://localhost:8080/StrutsContactsApp/contacts.json => []
    - http://localhost:8080/StrutsContactsApp/contacts..xml => <list/>
    <= Simple requests working in browser...

  - Installed SoapUI 5.5.0
      SoapUI > New REST Project >
        Project Name= StrutsContactsApp, endpoint= http://localhost:8080

    - SoapUI project organization:
Projects
  +-- StrutsContactsApp          // Project
     +-- http://localhost:8080   // Service (e.g. from WSDL, WADL, Swagger, etc)
        +-- JSONEndpoints        // Resource
          +-- GetContacts        // Method
          |  +-- GetContactsReq  // GET Endpoint= http://localhost:8080, Resource= /StrutsContactsApp/contacts.json
          +-- AddContacts        // Method
             +-- AddContactReq   // POST Endpoint= http://localhost:8080, Resource= /StrutsContactsApp/contacts.json
             |                      Request body=
             |                        {
             |                          "name":"Jason Bourne",
             |                          "email":"jb@abc.com"
             |                        }
             +-- GetContactsReq   // GET Endpoint= http://localhost:8080, Resource= /StrutsContactsApp/contacts.json
       <= Saved to $PROJ/ContactsApp/struts-rest/soapui/


    - SoapUI > StrutsContactsApp > GetContacts > GetContactsReq > Run >
        <= OK: Response= HTTP 200, Content-Type: application/json;charset=UTF-8, Body= [] (no contacts)

    - SoapUI > StrutsContactsApp > AddContacts > AddContactReq > Run >
HTTP Status 500: Internal Server Error
java.lang.NullPointerException
        com.fasterxml.jackson.databind.ObjectMapper.readerForUpdating(ObjectMapper.java:3691)
        org.apache.struts2.rest.handler.JacksonLibHandler.toObject(JacksonLibHandler.java:43)
        ...
        <= TBD: DEBUG!

===================================================================================================
* Build Problems:
1. pom.xml import defaults to JRE 1.5
  - Project > Build Path >
    - JRE System Library [J2SE-1.5]
      <= Eeek!  How did *that* fossil creep in???
    SOLUTION:
    - DELETE .settings, .target, .classpath, .project (i.e. delete Eclipse project artifacts)
    - Explicitly specify maven-compiler-plugin and JRE versions in pom.xml:
pom.xml:
-------
...
  <properties>
    ...
    <maven-compiler-plugin.version>3.8.0</maven-compiler-plugin.version>
    <maven.compiler.target>1.8</maven.compiler.target>
    <maven.compiler.source>1.8</maven.compiler.source>
    ...
  <build>
    ...
    <plugins>
        <plugin>
          <groupId>org.apache.maven.plugins</groupId>
          <artifactId>maven-compiler-plugin</artifactId>
          <version>${maven-compiler-plugin.version}</version>
          <configuration>
            <source>${maven.compiler.source}</source>
            <target>${maven.compiler.target}</target>
            ...
    - Eclipse > Import > Existing Maven Project >
       <= Imports project

    - Eclipse > Project > Properties >
      - Facets > Java
        <= Should be JRE 1.8
      - Java Compiler > Settings
        <= Should be JRE 1.8
      - Build Path > Libraries > Add > Tomcat 8
        <= Servlet libs
      - Deployment Assembly
        <= Ensure /src/main/{java/, resources/, webapp/} all included

2. Java version incompatibility:
    - Tomcat > Debug >
an 05, 2020 4:09:06 PM org.apache.catalina.startup.ContextConfig processAnnotationsJar
SEVERE: Unable to process Jar entry [module-info.class] from Jar [file:/C:/Users/paulsm/eclipse-workspace5/.metadata/.plugins/org.eclipse.wst.server.core/tmp0/wtpwebapps/ContactsApp/WEB-INF/lib/stax-ex-1.8.1.jar] for annotations
org.apache.tomcat.util.bcel.classfile.ClassFormatException: Invalid byte tag in constant pool: 19
    at org.apache.tomcat.util.bcel.classfile.Constant.readConstant(Constant.java:97)
    at org.apache.tomcat.util.bcel.classfile.ConstantPool.<init>(ConstantPool.java:55)
        ...
Jan 05, 2020 4:09:07 PM org.apache.catalina.startup.ContextConfig processAnnotationsJar
SEVERE: Unable to process Jar entry [module-info.class] from Jar [file:/C:/Users/paulsm/eclipse-workspace5/.metadata/.plugins/org.eclipse.wst.server.core/tmp0/wtpwebapps/ContactsApp/WEB-INF/lib/istack-commons-runtime-3.0.8.jar] for annotations
org.apache.tomcat.util.bcel.classfile.ClassFormatException: Invalid byte tag in constant pool: 19
    at org.apache.tomcat.util.bcel.classfile.Constant.readConstant(Constant.java:97)
    at org.apache.tomcat.util.bcel.classfile.ConstantPool.<init>(ConstantPool.java:55)
        ...
Jan 05, 2020 4:09:07 PM org.apache.catalina.startup.ContextConfig processAnnotationsJar
SEVERE: Unable to process Jar entry [module-info.class] from Jar [file:/C:/Users/paulsm/eclipse-workspace5/.metadata/.plugins/org.eclipse.wst.server.core/tmp0/wtpwebapps/ContactsApp/WEB-INF/lib/txw2-2.3.2.jar] for annotations
org.apache.tomcat.util.bcel.classfile.ClassFormatException: Invalid byte tag in constant pool: 19
    at org.apache.tomcat.util.bcel.classfile.Constant.readConstant(Constant.java:97)
    at org.apache.tomcat.util.bcel.classfile.ConstantPool.<init>(ConstantPool.java:55)
        ...
     <<Tried many things.  Finally wound up:
       1. Upgrading MSI to Tomcat9 (vs. Tomcat 8)
       2. Downgrading to slightly lower 3rd party library versions in pom.xml >>

    - Compared libraries (from old pom.xml vs. new pom.xml):
INCOMPATIBLE:                      OK:
------------                       --
stax-ex-1.8.1.jar                  stax-ex-1.8.jar
istack-commons-runtime-3.0.8.jar   istack-commons-runtime-3.0.7.jar
txw2-2.3.2.jar                     txw2-2.3.1.jar
jackson-databind-2.10.1.jar        jackson-databind-2.10.0.jar
...                                ...

===================================================================================================
* TBD:
  - Debug/finish controller (struts2-rest-plugin implementation)
  - Try Hibernate (vs. JPA-Hibernate) repository?
  - Try EclipseLink (vs. H2) DB?
  - Try "vanilla" struts2/struts.xml (vs. struts2-rest-plugin)?
  - Other?

===================================================================================================
  
* Double-checked Strut2 example code:
  - http://struts.apache.org/download.cgi#struts2316-SNAPSHOT >
      Example Applications > https://www-us.apache.org/dist/struts/2.5.22/struts-2.5.22-apps.zip
      
  - Explicitly defined controller methods in struts.xml:
    - struts2-rest-showcase-war > WEB-INF > classes > stuts.xml >
        <!DOCTYPE struts PUBLIC
            "-//Apache Software Foundation//DTD Struts Configuration 2.5//EN"
            "http://struts.apache.org/dtds/struts-2.5.dtd">
           ...
           <package name="contacts" extends="rest-default">
               <global-allowed-methods>index,show,create,update,destroy,deleteConfirm</global-allowed-methods>
           </package>
        <= Added this to my struts.xml
           Also needed to change schema from Struts 2.3 => Struts 2.5

  - Modified controller to support either "single record" or "list":
    - ContactsController.java:
      -----------------------
      public class ContactsController implements ModelDriven<Object> {
          ....
          private String id;
          private Contact model = new Contact();
          private Collection<Contact> list;
          private ContactsRepository contactsRepository = new ContactsRepositoryImpl();
          ....
          @Override
          public Object getModel() {
              return (list != null ? list : model);  // <-- return either single record, or list
          }
          ...
          public HttpHeaders index () {
              log.debug("Reading all contacts...");
              list = contactsRepository.getContacts();  // <-- Fetch list
              return new DefaultHttpHeaders("index").disableCaching();
          ...
          public HttpHeaders show() {
              log.debug("Reading contact(" + id + ")...");
              int contactId = Integer.parseInt(id);
              model = (Contact)contactsRepository.getContact(contactId);  // <-- Fetch item
              return new DefaultHttpHeaders("show");
           ...
           
  - SoapUI > AddContact > AddcontactRequest >
      POST Endpoint= http://localhost:8080, Resource= /StrutsContactsApp/contacts.json, Body=
        {
            "name": "Sy Snootles",
            "email": "ss@abc.com",
            "phone1": "111-111-1111",
            "phone2": null,
            "address1": "1 Yellow Brick Road",
            "address2": null,
            "city": "Emerald City",
            "state": "Oz",
            "zip": "00000",
            "notes": null
        }
        <= Successfully added new contact

  - Current status, both Postman and SoapUI:
    - GET Endpoint= http://localhost:8080, Resource= /StrutsContactsApp/contacts.json: OK

    - POST Endpoint= http://localhost:8080, Resource= /StrutsContactsApp/contacts.json + JSON body: OK

    - PUT Endpoint= http://localhost:8080, Resource= http://localhost:8080, + JSON body:
      <= Successfully calls ContactsController.create() and correctly creates the new "Contact" in the database:
    19:46:05.361 [http-nio-8080-exec-3] DEBUG com.opensymphony.xwork2.validator.ValidationInterceptor - Validating /contacts with method update.
    19:46:05.393 [http-nio-8080-exec-3] DEBUG com.opensymphony.xwork2.DefaultActionInvocation - Executing action method = update
    19:46:05.398 [http-nio-8080-exec-3] DEBUG com.opensymphony.xwork2.ognl.SecurityMemberAccess - Checking access for [target: com.example.contactsapp.controllers.ContactsController@7e69160c, member: public java.lang.String com.example.contactsapp.controllers.ContactsController.update(), property: null]
    19:46:07.862 [http-nio-8080-exec-3] DEBUG com.example.contactsapp.controllers.ContactsController - Updating existing contact(97)...

      ... but it then fails on "return":
    19:46:11.380 [http-nio-8080-exec-3] WARN  org.apache.struts2.dispatcher.Dispatcher - Could not find action or result: /StrutsContactsApp/contacts/97
    com.opensymphony.xwork2.config.ConfigurationException: : NNo result defined for action com.example.contactsapp.controllers.ContactsController and result update
    	at org.apache.struts2.rest.RestActionInvocation.findResult(RestActionInvocation.java:283) ~[struts2-rest-plugin-2.5.22.jar:2.5.22]
    	at org.apache.struts2.rest.RestActionInvocation.executeResult(RestActionInvocation.java:225) ~[struts2-rest-plugin-2.5.22.jar:2.5.22]
    	at org.apache.struts2.rest.RestActionInvocation.processResult(RestActionInvocation.java:189) ~[struts2-rest-plugin-2.5.22.jar:2.5.22]
    	at org.apache.struts2.rest.RestActionInvocation.invoke(RestActionInvocation.java:137) ~[struts2-rest-plugin-2.5.22.jar:2.5.22]
    	at com.opensymphony.xwork2.DefaultActionProxy.execute(DefaultActionProxy.java:157) ~[struts2-core-2.5.22.jar:2.5.22]
        ...
    - Postman > DELETE > http://localhost:8080/StrutsContactsApp/contacts/97
        <= Status 405 – Method Not Allowed; no console.log

    <<Tried many things: all no-go>>   
    
  - TBD: 
    - Save current
    - New Git branch
    - Rewrite for a) "vanilla" Struts2, b) "vanilla" Hibernate, c) EclipseL

  - Git update:
        
  
