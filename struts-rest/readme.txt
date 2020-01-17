* Struts2:
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
       Hibernate
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
* INITIAL GAMEPLAN (struts2-rest-plugin-hibernate-h2):
  - Tried using struts2-rest-plugin + JPA API/H2 database
    <= No-go:
  - Saved work to "struts2-rest-plugin-hibernate-h2"

* NEW GAMEPLAN:
  - Scaffold "vanilla" Struts2 REST service, using...
  - "vanilla" Hibernate persistence, with... 
  - Derby embedded database.
===================================================================================================
* Scaffold "Vanilla" Struts2+Hibernate app:

1. pom.xml:
   -------
<project xmlns="http://maven.apache.org/POM/4.0.0" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
  xsi:schemaLocation="http://maven.apache.org/POM/4.0.0 http://maven.apache.org/maven-v4_0_0.xsd">
  <modelVersion>4.0.0</modelVersion>
  <groupId>com.example.contactsapp</groupId>
  <artifactId>ContactsApp</artifactId>
  <version>2.0</version>
  <name>Contacts App</name>
  <packaging>war</packaging>
  ...
  <properties>
	<project.build.sourceEncoding>UTF-8</project.build.sourceEncoding>
	<struts2.version>2.5.22</struts2.version>
    <hibernate.version>5.4.0.Final</hibernate.version>	
  	<derby.version>10.14.2.0</derby.version>
  	<log4j2.version>2.12.1</log4j2.version>
    <java.version>1.8</java.version>
    <maven-war-plugin.version>3.2.2</maven-war-plugin.version>
    <maven-compiler-plugin.version>3.8.0</maven-compiler-plugin.version>
  </properties>
  ...
  <dependencies>
	<dependency>
	    <groupId>org.apache.struts</groupId>
	    <artifactId>struts2-core</artifactId>
        ...
	<dependency>
	    <groupId>org.apache.struts</groupId>
	    <artifactId>struts2-json-plugin</artifactId>
        ...
    <dependency>
        <groupId>org.hibernate</groupId>
        <artifactId>hibernate-core</artifactId>
        ...
    <dependency>
        <groupId>org.apache.derby</groupId>
        <artifactId>derby</artifactId>
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
            <source>${java.version}</source>
            <target>${java.version}</target>
            ...
    <= NOTE: 
       Must specify Java version, and have a <build> stanza with explicit maven-compiler-plugin version, else defaults to JRE 1.5

2. Eclipse > Import Maven project > pom.xml >
   <= Auto-generates Dynamic web project, creates src/* and target project directories
   
   - Eclipse > ContactsApp > Java EE Tools > Generate deployment descriptor >
     <= pom.xml complaining "missing web.xml"

   - Eclipse >  ContactsApp > Properties >
       Facets: Dynamic Web Module 2.5, Java 1.8, Javascript 1.0: OK
       Java Compiler: JavaSE-1.8, 1.8 JDK compliance: OK
       Java Build Path > Libraries > {JRE System Library (JavaSE-1.8), Maven Dependencies > Add Library >
         Server Runtime > Tomcat 9 > [Apply & Close]
       <= OK: basic Eclipse project ready to go...

3. Scaffold project:
   - com.example.contactsapp.models
       Contact.java, Note.java
   - com.example.contactsapp.repositories
       ContactsRepository.java (interface), class ContactsRepositoryImpl.java (implements ContactsRepository)
   - com.example.contactsapp.actions
       ContactsAction (extends ActionSupport)

4. Data model:
   - JPA vs. Hibernate:
     - From JBoss Hibernate User Guide:
https://docs.jboss.org/hibernate/orm/5.4/userguide/html_single/Hibernate_User_Guide.html
    "Historically applications using Hibernate would have used its proprietary XML mapping
     file format for this purpose. With the coming of JPA, most of this information is now
     defined in a way that is portable across ORM/JPA providers using annotations (and/or
     standardized XML format). This chapter will focus on JPA mapping where possible."
     <= Not bad advice... but let's stick with old-school "hibernate.cfg.xml, *.hbm.xml" this go-around...

   - Links:
http://zetcode.com/db/hibernatederby/
https://www.mkyong.com/hibernate/hibernate-one-to-many-relationship-example/
https://www.mkyong.com/tutorials/hibernate-tutorials/

   - Note.java:
     ---------
public class Note implements Serializable {
    private static final long serialVersionUID = 1L;

    private int noteId;
    private String text;
    private Date date;
    private int contactId;
    
    public Note () {
        this.date = new Date();
    }

    public Note (String text) {
        this.date = new Date();
        this.text = text;
    }

    @Override
    public String toString() {
        SimpleDateFormat sdf = new SimpleDateFormat ("MM/DD/yy HH:mm:ss");
        String s = "id: " + noteId + ", date: " + sdf.format(date) + ": " + text;
        return s;
    }
    ... <= Auto-generated getters/setters
    <<No JPA or Hibernate annotations whatsoever.  We'll use *.hbm.xml instead >>

   - Contact.java:
     ------------
public class Contact implements Serializable {
    private static final long serialVersionUID = 1L;
    
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
    private Set<Note> notes = new HashSet<Note>();
    
    @Override
    public String toString() {...}
    <= NOTE: 
       Originally tried "List<Note> notes", but encountered problems.
       Punted, and a) defined as "Set<Note>", and b) initialized at declaration.

  - Eclipse > src > main > New Folder >
      <= Add "resources"
    - Eclipse > Build Path > Source >
      <= Add "resources" as a resource folder

  - src/main/resources/Note.hbm.xml:
    -------------------------------
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE hibernate-mapping PUBLIC "-//Hibernate/Hibernate Mapping DTD//EN" "http://www.hibernate.org/dtd/hibernate-mapping-3.0.dtd">
<hibernate-mapping>
    <class name="com.example.contactsapp.models.Note" table="notes" catalog="app">
        <id name="noteId" type="java.lang.Integer">
            <column name="noteId" />
            <generator class="identity" />
        </id>
        <property name="text" type="string">
            <column name="text" length="80"/>
        </property>
        <property name="date" type="date">
            <column name="date"/>
        </property>
    </class>
</hibernate-mapping>
    <= NOTE: 
       Did *NOT* need "<many-to-one>" stanza here

  - src/main/resources/Contact.hbm.xml:
    ----------------------------------
<hibernate-mapping>
    <class name="com.example.contactsapp.models.Contact" table="contacts" catalog="app">
        <id name="contactId" type="java.lang.Integer">
            <column name="contactId" />
            <generator class="identity" />
        </id>
        <property name="name" type="java.lang.String">
            <column name="name" length="40"/>
        </property>
        <property name="email" type="string">
            <column name="email" length="40"/>
        </property>
        <property name="phone1" type="string">
            <column name="phone1" length="20"/>
            ...
        <set name="notes" table="notes" inverse="true" lazy="true" fetch="select">
            <key column="contactId" not-null="true" />
            <one-to-many class="com.example.contactsapp.models.Note" />
        </set>
    </class>        
</hibernate-mapping>
    <= NOTES: 
       1. Defined as "notes" as a "set", with "lazy loading"
          Consequently, to access "notes" without "failed to lazily initialize a collection" error, must do one of:
          a) Read "notes" before closing the session
          b) Use a "JOIN FETCH" 
          c) call "Hibernate.initialize()" in the repository
       2. DOCTYPE must be 
          <!DOCTYPE hibernate-mapping PUBLIC "-//Hibernate/Hibernate Mapping DTD//EN" "http://www.hibernate.org/dtd/hibernate-mapping-3.0.dtd">

5. Manually create Derby database:
  - Eclipse > Perspective > DBeaver > 
      <= Open DBeaver perspective

  - Database > Driver Manager > 
      <= Verify "Derby Embedded" is present (download if missing)
    - Driver type= Generic, Class name= org.apache.derby.jdbc.EmbeddedDriver, URL template= jdbc:derby:{folder}, Embedded= Y
    
  - Database > New Connection >
      Derby Embedded= Y >
      Path= c:/temp/contactsdb;create=true >
     [Create] >
        <= Prompted to download drivers
           "Database creation successful"
     [Test Connection]
        <= OK
     - Edit connection >
        <= Delete ";create=true"
       Updated path= contactsdb
    [Test Connection]
       <= Still "OK"

    <= NOTES: 
       1. Use Path= "c:/temp/contactsdb;create=true", then change back to Path= "c:/temp/contactsdb" for subsequent runs
       2. JDBC URL= jdbc:derby:c:/temp/contactsdb
       3. Filepath= c:\temp\contactsdb\*

6. Persistence:
   - Hibernate SessionFactory vs. EntityManagerFactory:
https://stackoverflow.com/questions/5640778/hibernate-sessionfactory-vs-entitymanagerfactory
    <= In general, prefer EntityManager (JPA) over SessionFactory (Hibernate-specific)

  - src/main/resources/hibernate.cfg.xml:
    ------------------------------------
<?xml version="1.0" encoding="utf-8"?>
<!DOCTYPE hibernate-configuration PUBLIC
"-//Hibernate/Hibernate Configuration DTD 3.0//EN"
"http://www.hibernate.org/dtd/hibernate-configuration-3.0.dtd">
 
<hibernate-configuration>
<session-factory>
    <property name="hibernate.connection.driver_class">org.apache.derby.jdbc.EmbeddedDriver</property>
    <property name="hibernate.connection.username">app</property>
    <property name="hibernate.connection.password"></property>
    <property name="hibernate.connection.url">jdbc:derby:c:/temp/contactsdb</property>
    <property name="hibernate.dialect">org.hibernate.dialect.DerbyTenSevenDialect</property>
    <!-- Auto-generate tables: create|validate|update|create-drop
    <property name="hibernate.hbm2ddl.auto">create</property>
    -->
    <property name="show_sql">true</property>
    <property name="format_sql">true</property>
    <mapping resource="Contact.hbm.xml" />
    <mapping resource="Note.hbm.xml" />
</session-factory>
</hibernate-configuration>
    <= Points to newly created "contactsdb"; Can uncomment/comment out auto-generation

   - HibernateUtil.java:
     ------------------
/**
 * REFERENCE:
 * https://www.mkyong.com/tutorials/hibernate-tutorials/
 */
public class HibernateUtil {

    private static SessionFactory sessionFactory = buildSessionFactory();

    private static SessionFactory buildSessionFactory() {
        try {
            // load from different directory
            SessionFactory sessionFactory = 
                new Configuration()
                    .configure("hibernate.cfg.xml")
                    .buildSessionFactory();

            return sessionFactory;

        } catch (Throwable ex) {
            // Make sure you log the exception, as it might be swallowed
            System.err.println("Initial SessionFactory creation failed." + ex);
            throw new ExceptionInInitializerError(ex);
        }
    }

    public static SessionFactory getSessionFactory() {
        return sessionFactory;
    }

    public static void shutdown() {
        // Close caches and connection pools
        if (sessionFactory != null)
            sessionFactory.close();
        sessionFactory = null;
    }
    
    public static Session openSession() {
        if (sessionFactory == null)
            sessionFactory = buildSessionFactory();
        return sessionFactory.openSession();        
    }
   
   - ContactsRepository.java:
     -----------------------
public interface ContactsRepository {
	public List<Contact> getContacts();
	public Contact getContact(int id);
	public int addContact(Contact contact);
	public void deleteContact(int id);
	public void updateContact(Contact contact);
    ...

   - ContactsRepositoryImpl.java:
     ---------------------------
public class ContactsRepositoryImpl implements ContactsRepository {
	private Session session;
	
	public ContactsRepositoryImpl() {
		session = HibernateUtil.getSessionFactory().openSession();
	}
	
	public void close () {
		HibernateUtil.shutdown ();
		session = null;
        ...
	@Override
	public List<Contact> getContacts() { ... }
	@Override
	public Contact getContact(int id) { ... }
	@Override
	public int addContact(Contact contact) { ... }
	@Override
	public void deleteContact(int id) { ... }
	@Override
	public void updateContact(Contact contact) { ... }
	public static void main (String[] args) {
		ContactsRepository contactsRepository = null;
		try {
			// Connect to database
			contactsRepository = new ContactsRepositoryImpl();
			...
		} catch (Exception e) {
			System.out.println("ERROR: " + e.getMessage());
			e.printStackTrace();
		} finally {
			if (contactsRepository != null)
				((ContactsRepositoryImpl)contactsRepository).close();
		}
  - Eclipse > Debug Configurations > New > Java Application >
      ContactsRepositoryImpl > Stop in main= Y > [Debug] >
ERROR StatusLogger No Log4j 2 configuration file found. Using default configuration (logging only errors to the console), or user programmatically provided configurations. Set system property 'log4j2.debug' to show Log4j 2 internal initialization logging. See https://logging.apache.org/log4j/2.x/manual/configuration.html for instructions on how to configure Log4j 2

    - SOLUTION: Added log4j2.xml
      src > main > resources > log4j2.xml:
      -----------------------------------
<?xml version="1.0" encoding="UTF-8"?>
<Configuration status="INFO">
  <Appenders>
    <Console name="Console" target="SYSTEM_OUT">
      <PatternLayout pattern="%d{HH:mm:ss.SSS} [%t] %-5level %logger{36} - %msg%n"/>
    </Console>
  </Appenders>
  <Loggers>
    <Root level="debug">
      <AppenderRef ref="Console"/>
    </Root>
  </Loggers>
</Configuration>

  - Eclipse > Debug > ContactsRepositoryImpl >
    - Console.log:
20:57:51.595 [main] DEBUG org.jboss.logging - Logging Provider: org.jboss.logging.Log4j2LoggerProvider
20:57:51.607 [main] DEBUG org.hibernate.integrator.internal.IntegratorServiceImpl - Adding Integrator [org.hibernate.cfg.beanvalidation.BeanValidationIntegrator].
20:57:51.607 [main] DEBUG org.hibernate.integrator.internal.IntegratorServiceImpl - Adding Integrator [org.hibernate.secure.spi.JaccIntegrator].
20:57:51.607 [main] DEBUG org.hibernate.integrator.internal.IntegratorServiceImpl - Adding Integrator [org.hibernate.cache.internal.CollectionCacheInvalidator].
20:57:51.689 [main] INFO  org.hibernate.Version - HHH000412: Hibernate Core {5.4.0.Final}
... <= OK: Hibernate runtime successfully invoked...
20:57:52.220 [main] DEBUG org.hibernate.boot.jaxb.internal.MappingBinder - Performing JAXB binding of hbm.xml document : Origin(name=Contact.hbm.xml,type=RESOURCE)
20:57:52.876 [main] DEBUG org.hibernate.boot.jaxb.internal.stax.LocalXmlResourceResolver - Interpreting public/system identifier : [-//Hibernate/Hibernate Mapping DTD//EN] - [http://www.hibernate.org/dtd/hibernate-mapping-3.0.dtd]
20:57:52.876 [main] DEBUG org.hibernate.boot.jaxb.internal.stax.LocalXmlResourceResolver - Recognized hibernate-mapping identifier; attempting to resolve on classpath under org/hibernate/
20:57:52.893 [main] DEBUG org.hibernate.boot.jaxb.internal.MappingBinder - Performing JAXB binding of hbm.xml document : Origin(name=Note.hbm.xml,type=RESOURCE)
... <= OK:  We're reading the correct hibernate.cfg.xml file, and picking up our *.hbm.xml definitions
20:57:53.033 [main] INFO  org.hibernate.orm.connections.pooling - HHH10001005: using driver [org.apache.derby.jdbc.EmbeddedDriver] at URL [jdbc:derby:c:/temp/contactsdb]
20:57:53.033 [main] INFO  org.hibernate.orm.connections.pooling - HHH10001001: Connection properties: {password=, user=sa}
... <=OK:  We're reading the correct hibernate.cfg.xml file, and picking up the correct Derby driver...
20:57:53.033 [main] INFO  org.hibernate.orm.connections.pooling - HHH10001005: using driver [org.apache.derby.jdbc.EmbeddedDriver] at URL [jdbc:derby:c:/temp/contactsdb]
20:57:53.033 [main] INFO  org.hibernate.orm.connections.pooling - HHH10001001: Connection properties: {password=, user=sa}
20:57:53.033 [main] INFO  org.hibernate.orm.connections.pooling - HHH10001003: Autocommit mode: false
...
20:57:53.392 [main] DEBUG org.hibernate.engine.jdbc.env.internal.JdbcEnvironmentInitiator - JDBC version : 4.2
20:57:53.392 [main] INFO  org.hibernate.dialect.Dialect - HHH000400: Using dialect: org.hibernate.dialect.DerbyTenSevenDialect
20:57:53.783 [main] DEBUG org.hibernate.boot.model.relational.Namespace - Created database namespace [logicalName=Name{catalog=null, schema=null}, physicalName=Name{catalog=null, schema=null}]
20:57:53.783 [main] DEBUG org.hibernate.type.spi.TypeConfiguration$Scope - Scoping TypeConfiguration [org.hibernate.type.spi.TypeConfiguration@6ff0b1cc] to MetadataBuildingContext [org.hibernate.boot.internal.MetadataBuildingContextRootImpl@3b55dd15]
20:57:53.845 [main] DEBUG org.hibernate.boot.model.relational.Namespace - Created database namespace [logicalName=Name{catalog=app, schema=null}, physicalName=Name{catalog=app, schema=null}]
... <= We've got the right driver, and opening the correct database...
20:57:53.861 [main] DEBUG org.hibernate.boot.model.source.internal.hbm.ModelBinder - Mapping class: com.example.contactsapp.models.Contact -> contacts
20:57:53.861 [main] DEBUG org.hibernate.boot.model.source.internal.hbm.ModelBinder - Mapped property: contactId -> [contactId]
20:57:53.861 [main] DEBUG org.hibernate.mapping.PrimaryKey - Forcing column [contactid] to be non-null as it is part of the primary key for table [contacts]
20:57:53.861 [main] DEBUG org.hibernate.boot.model.source.internal.hbm.ModelBinder - Mapped property: name -> [name]
...  <= Binding "Contact" entities
20:57:53.876 [main] DEBUG org.hibernate.boot.model.source.internal.hbm.ModelBinder - Mapping class: com.example.contactsapp.models.Note -> notes
20:57:53.876 [main] DEBUG org.hibernate.boot.model.source.internal.hbm.ModelBinder - Mapped property: noteId -> [noteId]
20:57:53.876 [main] DEBUG org.hibernate.mapping.PrimaryKey - Forcing column [noteid] to be non-null as it is part of the primary key for table [notes]
20:57:53.876 [main] DEBUG org.hibernate.boot.model.source.internal.hbm.ModelBinder - Mapped property: text -> [text]
20:57:53.876 [main] DEBUG org.hibernate.boot.model.source.internal.hbm.ModelBinder - Mapped property: date -> [date]
20:57:53.876 [main] DEBUG org.hibernate.boot.model.source.internal.hbm.ModelBinder - Mapping collection: com.example.contactsapp.models.Contact.notes -> notes
20:57:53.876 [main] DEBUG org.hibernate.boot.model.source.internal.hbm.ModelBinder - Binding [ONE_TO_MANY] element type for a [SET]
20:57:53.876 [main] DEBUG org.hibernate.boot.model.source.internal.hbm.ModelBinder - Mapped collection : com.example.contactsapp.models.Contact.notes
20:57:53.876 [main] DEBUG org.hibernate.boot.model.source.internal.hbm.ModelBinder -    + table -> contacts
20:57:53.876 [main] DEBUG org.hibernate.boot.model.source.internal.hbm.ModelBinder -    + key -> contactId
20:57:53.876 [main] DEBUG org.hibernate.boot.model.source.internal.hbm.ModelBinder -    + one-to-many -> com.example.contactsapp.models.Note
20:57:53.876 [main] DEBUG org.hibernate.boot.internal.InFlightMetadataCollectorImpl - Resolving reference to class: com.example.contactsapp.models.Contact
20:57:53.876 [main] DEBUG org.hibernate.boot.internal.InFlightMetadataCollectorImpl - Resolving reference to class: com.example.contactsapp.models.Contact
...  <= Binding "Note" entities, and establishing Note's child relationship to "Contact"

  - Used static main() driver in ContactsRepositoryImpl.java to verify:
    - getContacts()
    - getContact(id)
    - addContact(contact)
    - updateContact(contact)
    - deleteContact(id)
    - Contact.toString()

6. Struts2 actions (preliminary):
   -----------------------------
   - src > main > webapp > WEB-INF > web.xml:
     ---------------------------------------
<?xml version="1.0" encoding="UTF-8"?>
<web-app 
  xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
  xmlns="http://java.sun.com/xml/ns/javaee" 
  xsi:schemaLocation="http://java.sun.com/xml/ns/javaee http://java.sun.com/xml/ns/javaee/web-app_2_5.xsd" 
  version="2.5">
  <display-name>Struts Contacts App</display-name>
  <welcome-file-list>
    <welcome-file>index.html</welcome-file>
    <welcome-file>index.jsp</welcome-file>
  </welcome-file-list>
  <filter>
    <filter-name>struts2</filter-name>
    <filter-class>org.apache.struts2.dispatcher.filter.StrutsPrepareAndExecuteFilter</filter-class>
  </filter>
  <filter-mapping>
	<filter-name>struts2</filter-name>
	<url-pattern>/*</url-pattern>
  </filter-mapping>  
</web-app>
  <= NOTES:
     - Web-App 2.5 xsd, configure Strut2 as a servlet filter
     - CURRENT: class= org.apache.struts2.dispatcher.filter.StrutsPrepareAndExecuteFilter
     - OLD: org.apache.struts2.dispatcher.FilterDispatcher (deprecated in Struts 2.1.3)

   - src > main > resources > struts.xml:
     -----------------------------------
<?xml version="1.0" encoding="UTF-8" ?>
<!DOCTYPE struts PUBLIC
        "-//Apache Software Foundation//DTD Struts Configuration 2.5//EN"
        "http://struts.apache.org/dtds/struts-2.5.dtd">
<struts>
   <constant name = "struts.devMode" value = "true" />
   <package name = "default" extends = "json-default">
      <action name = "getContacts" 
         class = "com.example.contactsapp.actions.ContactsAction" 
         method = "getContacts">
         <result name = "success" type="json">
             <param name="noCache">true</param>
             <param name="excludeNullProperties">true</param>
             <param name="root">jsonString</param>
         </result>
      </action>
   </package>
</struts>
  <= NOTES:
     1. For now, just one action: "getContacts"
     2. Using struts2-json-plugin, hence "json-default" 

7. ContactsActions.java:
   --------------------
public class ContactsAction extends ActionSupport {
    private static final long serialVersionUID = 1L;
    private Map<String, String> jsonString;
    
    public String getContacts () {
        jsonString = new HashMap<String, String>();
        jsonString.put("status", "getContacts was successful!");
        jsonString.put("data", "Dummy Contact");        
        return Action.SUCCESS;
    }
    
    public Map<String, String> getJsonString() { return jsonString; }
    public void setJsonString(Map<String, String> m) { jsonString = m; }
    ...

  <= NOTES:
     1. Action.SUCCESS is the return value to Struts2
     2. Per struts.xml, "success" action will return root parameter "jsonString()"
     3. jsonString is a Map<> object in the controller, accessed via getter/setters getJsonString()/setJsonString()

8. Test Struts2 app:
   ----------------
   - Eclipse > Tomcat > Add/Remove > ContactsApp
     <= Add "ContactsApp" servlet
   - Eclipse > Tomcat > Debug >
     <= Run ContactsApp in Eclipse debugger
   - http://localhost:8080/StrutsContactsApp/getContacts.action
     <= Value "{"data":"Dummy Contact","status":"getContacts was successful!"}" returned to browser

===================================================================================================
* Further Hibernate-related changes:
  - Contact.java:
    ------------
public class Contact implements Serializable {
	private static final long serialVersionUID = 1L;
	
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
    private Set<Note> notes = new HashSet<Note>();
    ... <= "Notes" should be defined as a Java Set<>; it should be initialized for each new Contact object

  - Note.java:
    ---------
public class Note implements Serializable {
	private static final long serialVersionUID = 1L;

	private int noteId;
    private String text;
    private Date date;
    private Contact contact;
    ... <= We don't reference other entities by their ID, but by a direct reference to the entity.

  - Contact.hbm.xml:
    ---------------
<hibernate-mapping>
    <class name="com.example.contactsapp.models.Contact" table="contacts" catalog="app">
        <id name="contactId" type="java.lang.Integer">
            <column name="contactId" />
            <generator class="identity" />
            ...
        <set name="notes" table="notes" cascade="delete" inverse="true" lazy="true" fetch="select">
            <key column="contactId" not-null="true" on-delete="cascade" />
            <one-to-many class="com.example.contactsapp.models.Note" />
        <= Needed to define Cascade=Delete
           There should be a *BI-MODAL* relationship between "Contact" and "Note"

  - Note.hbm.xml:
    ------------
<hibernate-mapping>
    <class name="com.example.contactsapp.models.Note" table="notes" catalog="app">
        <id name="noteId" type="java.lang.Integer">
            <column name="noteId" />
            <generator class="identity" />
            ...
        <many-to-one name="Contact" class="com.example.contactsapp.models.Contact" fetch="select">
            <column name="contactId" not-null="true"/>
            ...
        <property name="date" type="timestamp">
            <column name="date"/>
            ...  <= Hibernate XML time/date options: "time", "date" or "timestamp"

  - ContactsRepositoryImpl.java:
    ---------------------------
public class ContactsRepositoryImpl implements ContactsRepository {

	public void shutdown () {
		HibernateUtil.shutdown ();
		...
    @Override
	public List<Contact> getContacts() {
		//Note: Hibernate 5++ supports Java try-with-resource blocks
		try (Session session = HibernateUtil.openSession()) {
			// This will fetch all contacts... but "Notes" aren't accessible outside this session
			List<Contact> contacts = session.createQuery("FROM Contact").list();
			return contacts;
		...
	@Override
	public List<Contact> getContactsFetchAll() {
		// Mitigate Hibernate "failed to lazily initialize a collection" runtime error
		try (Session session = HibernateUtil.openSession()) {
			List<Contact> contacts = session.createQuery("SELECT c FROM Contact c INNER JOIN FETCH c.notes").list();
			return contacts;
		...
	@Override
	public Contact getContact(int id) {
		try (Session session = HibernateUtil.openSession()) {
			Contact contact = (Contact)session.get(Contact.class, id);
			// Mitigate "failed to lazily initialize a collection" error
			Hibernate.initialize(contact.getNotes());
			return contact;
        ...
	@Override
	public int addContact(Contact contact) {
		try (Session session = HibernateUtil.openSession()) {
			Transaction tx = session.beginTransaction(); 
			try {
				// "save()" returns contactId immediately; persist() doesn't
				int id = (int)session.save(contact);
				
				// Ensure every contact has at least one note (INNER JOIN FETCH)
				Note initialNote = new Note("Creating new contact");
				initialNote.setContact(contact);
				contact.getNotes().add(initialNote);
				session.save(initialNote);
				for (Note n : contact.getNotes()) {
					n.setContact(contact);
					session.save(n);
				}
				tx.commit();
				return id;
			} catch (Exception e) {
				tx.rollback();
				throw e;
			}
        ...
	@Override
	public int deleteContact(int id) {
		try (Session session = HibernateUtil.openSession()) {
			Transaction tx = session.beginTransaction(); 
			try {
				// Use HQL (vs. SQL)
				// OBSOLETE: query.setInteger(0, id) et al: deprecated since Hibernate 5.2
				String hql = "delete from Contact where contactId  = ?1";
				Query query = session.createQuery(hql)
						.setParameter(1,  id);
				int result = query.executeUpdate(); 
				tx.commit ();
				return result;
			} catch (Exception e) {
				tx.rollback();
				throw e;
        ...
	@Override
	public void updateContact(Contact contact) {
		try (Session session = HibernateUtil.openSession()) {
			Transaction tx = session.beginTransaction(); 
			try {
				session.update(contact);
				for (Note n : contact.getNotes()) {
					if (n.getContact() == null) {
						n.setContact(contact);
					}
					session.saveOrUpdate(n);
				}
				tx.commit ();
			} catch (Exception e) {
				tx.rollback();
				throw e;
			}

* Hibernate Pearls of Wisdom:
  Best Practices for Many-To-One and One-To-Many Association Mappings,  Thorben Janssen:
https://thoughts-on-java.org/best-practices-many-one-one-many-associations-mappings/
    1. Don’t use unidirectional one-to-many associations
    2. Avoid the mapping of huge to-many associations
       <= For large #/associated entities, it’s better to use a JPQL query with pagination. 
    3. Think twice before using CascadeType.Remove
    4. Use orphanRemoval when modeling parent-child associations
       <= EX: @OneToMany(mappedBy = "order", orphanRemoval = true)
    5. Implement helper methods to update bi-directional associations
    6. Define FetchType.LAZY for @ManyToOne association
       <= Default= FetchType.EAGER.  Don't do this!

===================================================================================================
* Problems getting "vanilla" Struts2 actions to work with HTTP "PUT" or "DELETE" methods
  <= Changed gears: reverted back to "struts2-rest-plugin" baseline

  - git branch -a =>
  aspnet-mvc5
  dotcore-angular
  dotnetcore-angular/angular
  master
* struts-rest
  struts2-rest-plugin-jpa-hibernate-h2
  remotes/origin/aspnet-mvc5
  remotes/origin/dependabot/nuget/aspnet-mvc5/ContactsApp/bootstrap-3.4.1
  remotes/origin/dotcore-angular
  remotes/origin/dotnetcore-angular/angular
  remotes/origin/master
  remotes/origin/struts-rest
  remotes/origin/struts2-rest-plugin-jpa-hibernate-h2

  - git merge struts2-rest-plugin-jpa-hibernate-h2
Already up to date.
    <= No-go on explicit merge

  - git checkout struts2-rest-plugin-jpa-hibernate-h2
    <= Copy to temp directory>>

  - git checkout struts-rest
    <= Manually reconciled with old "struts2-rest-plugin" code

  - JPA vs. Hibernate:
    <= Quickly discovered the "Hibernate dialect" ("Session" vs. EntityManager, etc.) far better than JPA
       Reverted from JPA back to Hibernate

  - CURRENT STATUS:
                 Previous                                  Current
                 struts-rest      struts2-rest-plugin      struts-rest
                 ---------------  -------------------      -------------------
Persistence API  Hibernate/XML    JPA/Annotations          Hibernate/Annotations
Database         Derby            H2                       H2 
Struts API       Struts2/XML      struts2-rest-plugin/XML  struts2-rest-plugin/XML


===================================================================================================
      


        
  
