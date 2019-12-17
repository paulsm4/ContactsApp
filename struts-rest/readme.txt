* Struts REST API
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

2. Create project:
   - File > New > New Maven Project >
       Folder= D:\paul\proj\ContactsApp\struts-rest\ContactsApp, 
       Archetype= maven-archetype-webapp
       GroupID= com.example, ArtifactId= ContactsApp, Package= com.example.contactsapp
       <= Creates struts-rest\ContactsApp Eclipse/Maven project
          NOTE: Strictly speaking, we *DIDN'T* need the webapp archetype... It just makes things easier...

   - pom.xml:
     -------
  <groupId>com.example</groupId>
  <artifactId>ContactsApp</artifactId>
  <packaging>war</packaging>
  <version>0.0.1-SNAPSHOT</version>
  <name>Contacts App</name>
  ...
  <properties>
	<project.build.sourceEncoding>UTF-8</project.build.sourceEncoding>
	<maven.compiler.source>1.8</maven.compiler.source>
	<maven.compiler.target>1.8</maven.compiler.target>
	<struts2.version>2.5.22</struts2.version>
  	<h2.version>1.4.200</h2.version>
  	<log4j2.version>2.12.1</log4j2.version>
  	<junit4.version>4.12</junit4.version>
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
    	<groupId>junit</groupId>
    	<artifactId>junit</artifactId>
    <= See https://mvnrepository.com/ for current versions of struts2-convention-plugin, struts2-rest-plugin, h2, junit
       Note: did *NOT* restrict h2 to "scope test"

  - Eclipse > Maven > Update Project >
      <= OK

  - Project > Build Path >
    - JRE System Library [J2SE-1.5]
      <= Eeek!  How did *that* fossil creep in???
      - JRE1.5 => [Remove]
      - [Add Library] => Workspace default (jdk1.8)
    - Targeted Runtime => Apache Tomcat v8.0

3. Scaffold app:
   - New > Class >
       com.example.contactsapp.models, Contact
       com.example.contactsapp.models, Note
       com.example.contactsapp.controllers, ContactsController, implements com.opensymphony.xwork2.ModelDriven<T>
       com.example.contactsapp.repositories, 
         Interface: ContactsRepository, Class: ContactsRepositoryImpl

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

