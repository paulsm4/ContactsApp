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
  appUrl: 'https://localhost:53561/' // MSVS 2019/IIS Express
};
    <= NOTES:
       1. .Net Core server *MUST* be configured for CORS
       2. URL *MUST* be "https"; 'http://localhost:53561/' will result in CORS error

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

===================================================================================================
* Build service
  <= services/contacts.service.ts: allows Angular app to communicate with .Net Core/REST API

  - Update contacts.service.ts to support HttpClient:
    cd $PROJ/ContactsApp/angular-ui/contactsapp
    code .
    <= Start VSCode...

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

    - Update app.module.gs to provide HttpClient et al:
...
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
...
@NgModule({
  declarations: [
    AppComponent,
    ...
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    ReactiveFormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }

    - Verify build:
      ng test => FAILS:
NullInjectorError: StaticInjectorError(DynamicTestModule)[HttpClient]: 
  StaticInjectorError(Platform: core)[HttpClient]: 
    NullInjectorError: No provider for HttpClient!
    <= PROBLEM: Need to update contacts.service.spec.ts, too!

    - contacts.service.spec.ts:
import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import { ContactsService } from './contacts.service';

describe('ContactsService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [HttpClientTestingModule],
    providers: [ContactsService]
  }));

  it('should be created', () => {
    const service: ContactsService = TestBed.get(ContactsService);
    expect(service).toBeTruthy();
  });
});
  <= Only need "HttpClientTestingModule"

  << "ng test" now OK, Chrome/localhost port 9867... >>
     Also need to explicitly provide "ContactsService"

  - Configure debugger for ng test:
https://studiolacosanostra.github.io/2019/05/16/How-to-debug-Angular-tests-in-VSCode/
    <= Tried this (create a special VSCode debugger configuration: I
       It didn't work: "Cannot connect to the runtime process... Can't find a valid target that matches..."

    - PLAN B: simply:
      a) ng test  // Start Karma tests
      b) F12 > Chrome Developer Tools
      c) Add "debug" to Javascript (contacts.service.spec.ts, contacts.service.ts, or any other JS code of interest...)
      d) <<Refresh>> page and debug from breakpoint
      <= Voila!  It works...

  - Unit testing HttpClient
https://blog.knoldus.com/unit-testing-of-angular-service-with-httpclient/
https://skryvets.com/blog/2018/02/18/unit-testing-angular-service-with-httpclient/
https://medium.com/netscape/testing-with-the-angular-httpclient-api-648203820712
https://blog.angulartraining.com/how-to-write-unit-tests-for-angular-code-that-uses-the-httpclient-429fa782eb15
  <= All of these use "HttpClientTestingModule" (vs. "HttpClient")
     Default: talk to "live" REST service (vs. mockm using "HttpTestingController")

  - Implement getContacts():
    - contacts.service.cs:
export class ContactsService {
  ...
  getContacts(): Observable<Contact[]> {
    return this.http.get<Contact[]>(this.myAppUrl + this.myApiUrl)
    .pipe(
      retry(1),
      catchError(this.errorHandler)
    );
  }
  ...
  errorHandler(error) {
    let errorMessage = '';
    if (error.error instanceof ErrorEvent) {
      // Get client-side error
      errorMessage = error.error.message;
    } else {
      // Get server-side error
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    console.error(errorMessage);
    return throwError(errorMessage);
  }
  ...

    - contacts.service.spec.ts:
describe('ContactsService', () => {
  ...
  it('should retrieve all contacts', () => {
    const contactsService: ContactsService = TestBed.get(ContactsService);
    let contacts = contactsService.getContacts();
    debugger;  // <-- Will stop here
    expect(contacts).toBeTruthy();
  });

    - Leave MSVS/.Net Core REST API *OFF*

    - ng test
      <= Karma test UI displays: http://localhost:9876/?id=77187353, Chrome browser
      <F12> Developer Tools
      <<Refresh>>

    - Bkpt@test:
      - contacts:
Observable {_isScalar: false, source: Observable, operator: CatchOperator}
operator: CatchOperator {caught: Observable, selector: ƒ}
source: Observable {_isScalar: false, source: Observable, operator: RetryOperator}
_isScalar: false
__proto__: Object
      <= Test does *NOT* fail, do *NOT* see any kind of error... despite REST API offline...
  
    - Added additional instrumentation
      Started MSVS
      <= REST API: http://localhost:53561/api/Contacts/
      <<Refresh Chrome>>
      <= No-go: "ContactsService" *STILL* passes ... *WITHOUT* invoking web service...

