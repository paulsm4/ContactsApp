* Contact app:
  - Purpose: simple full-stack app, ported to different environments
    <= Manage list of "contacts", along with associate "history" for each contact record

  - Potential variations:
    - ASP.Net/MVC5+EF+Razor UI (initial project)
    - Spring Boot+JPA+JSP UI
    - Spring Boot/REST+JPA, HTML5/Bootstrap UI
    - .Net Core/REST+EF, HTML5/Bootstrap UI, Swagger REST tests
    - Angular UI
    - Docker REST tests (NodJS/chai?)
    - Other?

  - .Net Core 3.0/ASP.Net Core examples:
      https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-3.0

  - Schema:
    - Contacts:
      { Id, Name, EMail, Phone1, Phone2, Addr1,Addr2, City, State, Zip }
    - Notes:
      { ContactId, NoteId, Text, Date }

  - UI:
    - Search screen
    - Edit Details screen
    - Edit Notes screen
