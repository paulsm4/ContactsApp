* Angular client:
  - check s/w:
      npm -g update; npm --version: 6.13.2
      node --version: 10.13.0
      ng --version: v8.3.20

  - Create project:
      cd angular-ui
      ng new contactsapp >
        Angular routing= Y, Stylesheet= CSS
      ng generate class models/contact
      ng generate class models/note
      ng generate service services/contacts
      ng generate component contact/add-contact
      ng generate component contact/add-note
      ng generate component contact/list-contacts
      ng generate component contact/update-contact
      <= Components TBD...

