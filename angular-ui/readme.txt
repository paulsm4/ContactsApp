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

  - Models (based on .Net Core REST API, C#/Asp.Net Core project "dotnetcore-rest\ContactsApp"):
    - cd angular-ui/contactsapp
      code .
      <= Start VSCode

    - src\app\models\contact.ts =>
        export class Contact {
          ContactId?: number;
          Name: string;
          EMail: string;
          Phone1: string;
          Phone2: string;
          Address1: string;
          Address2: string;
          City: string;
          State: string;
          Zip: string;
        }

    - src\app\models\note.ts =>
        export class Note {
          NoteId?: number;
          Text: string;
          Date: Date;
          ContactId?: number;
        }

  - Service:
    - cd angular-ui/contactsapp
      npm update

    - VSCode > src\app\services\contacts.service.ts =>
mport { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { retry, catchError } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Contact } from '../models/Contact';
import { Note } from '../models/Note';

@Injectable({
  providedIn: 'root'
})
export class ContactsService {

  myAppUrl: string;
  myApiUrl: string;
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json; charset=utf-8'
    })
  };
  constructor(private http: HttpClient) {
      this.myAppUrl = environment.appUrl;
      this.myApiUrl = 'api/Contacts/';
  }
}
  <= Minimal skeleton; no operations defined yet...

    - src\environments\environment.ts
export const environment = {
  production: false,
  appUrl: 'http://localhost:53561/' // MSVS 2019/IIS Express
};

    - Debug > Add Configuration > Chrome launch >
        "configurations": [{
            "type": "chrome",
            "request": "launch",
            "name": "Launch Chrome against localhost",
            "url": "http://localhost:4200",
            "webRoot": "${workspaceFolder}"
        }]
        <= this presumes REST running in MSVS, Angular running in "npm start"

  - Initial test:
    - npm start => OK
      <= Angular app running on httpP://localhost:4200
    - VSCode > Debug > Start Debugging => OK: brings up default page

  - Added "Material"
    - ng add @angular/material
    - Links:
https://auth0.com/blog/creating-beautiful-apps-with-angular-material/
https://material.angular.io/
https://material.io/design/

