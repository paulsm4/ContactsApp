* Angular client:
  - check s/w:
      npm -g update; npm --version: 6.13.2
      node --version: 10.13.0
      ng --version: v8.3.20

  - Create project:
      cd angular-ui
      ng new contactsapp >
        Angular routing= Y, Stylesheet= CSS
      ng generate class models/Contact
      ng generate class models/Note
      ng generate service services/Contacts
      <= Components TBD...

  - Models (based on .Net Core REST API, C#/Asp.Net Core project "dotnetcore-rest\ContactsApp"):
    - cd angular-ui/contactsapp
      code .
      <= Start VSCode

    - src\app\models\contact.ts =>
        export class Contact {
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
        }
        <= NOTE: originally declared fields in PascalCase (e,g, "ContactId", etc.)
                 But the return JSON was camelCase (e.g. "contactId"); named the fields appropriately 

    - src\app\models\note.ts =>
        export class Note {
          noteId?: number;
          text: string;
          date: Date;
          contactId?: number;
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

    - Update app.module.ts to provide HttpClient et al:
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
            const url = this.myAppUrl + this.myApiUrl;
            //const url = 'http://localhost:52774/api/Contacts/';  // CORS Error
            //const url = 'https://localhost:44362/api/Contacts/';  // Works if Startup.cs/app.AddCors() *above* MVC
        
            console.log('ContactsService.getContacts(url=' + url + ')...');
            return this.http.get<Contact[]>(url)
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
              errorMessage = `Error Code: ${error.status}\n>>Message: ${error.message}`;
            }
            console.error(errorMessage);
            return throwError(errorMessage);
          ...

    - contacts.service.spec.ts (run against live REST service, vs. HttpTestModule/mocks):
        import { TestBed, inject } from '@angular/core/testing';
        import { HttpClientModule, HttpClient } from '@angular/common/http';
        import { ContactsService } from './contacts.service';
        
        describe('ContactsService', () => {
        
          let service: ContactsService;
        
          beforeEach(() => {
            TestBed.configureTestingModule({
              imports: [HttpClientModule],
              providers: [ContactsService, HttpClient]
            });
            service = TestBed.get(ContactsService);
          });
          ...
          it('should retrieve all contacts', (done) => {
            expect(service).toBeTruthy();
            // Call the service
            service.getContacts().subscribe(data => {
              console.log('it(should be created)@getContacts(): live data=', data);
              expect(data).toBeTruthy();
              done();  // 'expect' was used when there was no current spec Jasmine error unless "done()"
            });
          });
          ...

    - .Net Core/Startup.cs:
         public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            ...
            app.UseCors(CORS_POLICY);
            app.UseRouting();
            app.UseAuthorization();
            app.UseHttpsRedirection();
            ...
            // *MUST* specify CORS before this...
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            <= *MUST* configure CORS, *MUST* configure HTTPS redirect, and *MUST* call "app.UseCors()" before any MVC!!!
               Else "Access to XMLHttpRequest at 'http://localhost:52774/api/Contacts/' from origin 'http://localhost:4200' has been blocked by CORS policy"...

    - ng test (live .Net Core REST service running, https://localhost:44362/api/Contacts/)
ContactsService.getContacts(url=https://localhost:44362/api/Contacts/)...
it(should be created)@getContacts(): live data= 
[{…}]
  0: {contactId: 7, name: "Test-Contact", eMail: "Test-Contact@abc.com", phone1: "111-222-3333", phone2: null, …}
  length: 1
  __proto__: Array(0)
    <= All tests pass!

    - ng test (.Net Core REST service OFF):
      - Chrome developer tools:
          GET https://localhost:44362/api/Contacts/ net::ERR_CONNECTION_REFUSED
          <= Good error message!
      - HttpClient/Jasmine error handler:
HttpErrorResponse {headers: HttpHeaders, status: 0, statusText: "Unknown Error", url: "https://localhost:44362/api/Contacts/", ok: false, …}
  error: ProgressEvent {isTrusted: true, lengthComputable: false, loaded: 0, total: 0, type: "error", …}
  headers: HttpHeaders {normalizedNames: Map(0), lazyUpdate: null, headers: Map(0)}
  message: "Http failure response for https://localhost:44362/api/Contacts/: 0 Unknown Error"
  name: "HttpErrorResponse"
  ok: false
  status: 0
  statusText: "Unknown Error"
  url: "https://localhost:44362/api/Contacts/"
  __proto__: HttpResponseBase
     <= Pretty useless error object...
        ... but at least we're correctly hitting the error handler and logging whatever information we have...

  - Built test component 
    <= It would have been nice if Jasmine unit test provided all the "development scaffolding" we need.
       Unfortunately, it doesn't.

   - ng g component TestAPI
     <= Creates test-api\test-api.component{.ts, .html, .spec}.ts

   - test-api/test-api.component.ts:
     ------------------------------
       ...
       @Component({
         selector: 'app-test-api',
         templateUrl: './test-api.component.html',
         styleUrls: ['./test-api.component.css']
       })
       export class TestAPIComponent implements OnInit, OnDestroy {
       
         contactsList: Contact[];
         private contactsSubscription$: Subscription;
       
         constructor(private contactsService: ContactsService) { }
       
         ngOnInit() {
           this.loadContacts();
         }
       
         ngOnDestroy() {
           this.contactsSubscription$.unsubscribe();
         }
       
         loadContacts() {
             // // tslint:disable-next-line: deprecation
             // this.contacts$ = this.contactsService.getContacts().pipe(tap(console.log));
             this.contactsSubscription$ = this.contactsService.getContacts().subscribe(
             data => {
               this.contactsList = data;
               console.log('loadContacts', data);
             },
             err => {
               console.error('loadContacts', err);
             });
         }
       
}
   - test-api/test-api.component.html:
     --------------------------------
       <h1>Contacts</h1>
       <p *ngIf="!(contactsList)"><em>Loading...</em></p>
       <table class="table table-sm table-hover" *ngIf="(contactsList)">
         <thead>
         ...
       <tbody>
           <tr *ngFor="let contact of (contactsList)">
             <td>{{ contact.contactId }}</td>
             <td>{{ contact.name }}</td>
             <td>{{ contact.eMail }}</td>
           </tr>
         ...

   - test-api/test-api.componentspec.ts:
     ----------------------------------
       describe('TestAPIComponent', () => {
         let component: TestAPIComponent;
         let fixture: ComponentFixture<TestAPIComponent>;
       
         beforeEach(async(() => {
           console.log('TestAPIComponent@beforeEach(async)')
           TestBed.configureTestingModule({
             imports: [HttpClientTestingModule],
             providers: [ContactsService, HttpTestingController],
             declarations: [ TestAPIComponent ]
           })
           .compileComponents();
         }));
         ...
         <= NOTE: we're using HttpClientTestingModule/mock HTTP here (vs. "live" in contacts.service text)

* SO questions:
  - https://stackoverflow.com/a/59240795/3135317
  - https://stackoverflow.com/questions/59204306

===================================================================================================

* Implement UI:
  - Cleanup; generate UI components:
    - cd $PROJ\ContactsApp\angular-ui\contactsapp
      del /s/q src\app\contacts
      <= Nuke these
      ng g component ListContacts
      ng g component EditContact
      ng g component EditNote
      <= Create these

  - ListContacts screen:
    - Angular and Bootstrap:
https://www.smashingmagazine.com/2019/02/angular-application-bootstrap/

    - npm install --save bootstrap jquery =>
+ jquery@3.4.1
+ bootstrap@4.4.1
    <= NOTE: Going forward, use *bootstrap* (with jQuery); *NOT* "ng-bootstrap"!

    - angular.json:
      ------------
{
   ...
   "projects": {
   "contactsapp": {
     ...
      "architect": {
        "build": {
          ...
           "styles": [
              "src/styles.css",
               "node_modules/bootstrap/dist/css/bootstrap.min.css"
              ...

    - code .
      list-contacts/list-contants.component.ts:
      ----------------------------------------
@Component({
  selector: 'app-list-contacts',
  templateUrl: './list-contacts.component.html',
  styleUrls: ['./list-contacts.component.css']
})
export class ListContactsComponent implements OnInit, OnDestroy  {

  contactsList: Contact[];
  selectedContact: Contact;
  private contactsSubscription$: Subscription;

  constructor(private contactsService: ContactsService) { }

  ngOnInit() {
    this.loadContacts();
  }

  ngOnDestroy() {
    this.contactsSubscription$.unsubscribe();
  }

  addContact(contact: Contact) {
    ; // TBD
  }

  deleteContact(contact: Contact) {
    ; // TBD
  }

  editContact(contact: Contact) {
    ; // TBD
  }

    - list-contacts/list-contants.component.html:
      ------------------------------------------
<div class="container" style="margin-top: 70px;">
  <h1>Contacts</h1>
  <p>
    <button class="btn btn-primary" (click)="addContact()"> Add Contact</button>
  </p>
  <table class="table table-hover">
    <thead>
      <tr>
        <th>#</th>
        <th>Name</th>
        <th>Email</th>
        <th colspan="2">Actions</th>
      </tr>
    </thead>
    <tbody>
      <tr *ngFor="let contact of contactsList">
        <td>{{ contact.contactId }}</td>
        <td> {{ contact.name }}</td>
        <td> {{ contact.eMail }}</td>
        <td>
          <button class="btn btn-primary" (click)="editContact(contact)"> Contact Details </button>
        </td>
        <td>
          <button class="btn btn-primary" (click)="deleteContact(contact)"> Delete Contact </button>
        </td>
      </tr>
    </tbody>
  </table>
  ...

    - ng test =>
      - 10 specs, 1 failure, randomized with seed 71656
      - ListContactsComponent > should create
          NullInjectorError: StaticInjectorError(DynamicTestModule)[ContactsService -> HttpClient]: 
          StaticInjectorError(Platform: core)[ContactsService -> HttpClient]: 
          NullInjectorError: No provider for HttpClient!
      - FIX: 
       list-contacts/list-contacts.component.spec.ts:
  ...
  describe('ListContactsComponent', () => {
    let component: ListContactsComponent;
    let fixture: ComponentFixture<ListContactsComponent>;

    beforeEach(async(() => {
      TestBed.configureTestingModule({
        imports: [HttpClientTestingModule],
        providers: [ContactsService, HttpTestingController],
        declarations: [ ListContactsComponent ]
      })
      .compileComponents();
      ...
      <= Works OK!

    - ng serve => OK
      Debugger:
      <= OK ... but still running "TestAPIComponent"...
      - app-routing.module.ts:
        ---------------------
...
const routes: Routes = [
  { path: '', component: ListContactsComponent, pathMatch: 'full' },
  { path: 'test', component: TestAPIComponent, pathMatch: 'full' },
  { path: '**', redirectTo: '/' }
];
  <= OK: both "http://localhost:4200" and "http://localhost:4200/test" work
  <<Saved .bu6, Updated Git>>

===================================================================================================
* Angular UI (continued): "Add Contact" component:
  - ng -g component add-contact
C:\paul\proj\ContactsApp\angular-ui\contactsapp>ng g component add-contact
CREATE src/app/add-contact/add-contact.component.html (26 bytes)
CREATE src/app/add-contact/add-contact.component.spec.ts (657 bytes)
CREATE src/app/add-contact/add-contact.component.ts (288 bytes)
CREATE src/app/add-contact/add-contact.component.css (0 bytes)
UPDATE src/app/app.module.ts (1074 bytes)

  - app.module.ts:
    -------------
@NgModule({
  declarations: [
    AppComponent,
    ListContactsComponent,
    EditContactComponent,
    EditNoteComponent,
    TestAPIComponent,
    AddContactComponent  <-- Automatically added
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule   <= Got rid of "ReactiveFormsModule", substituted vanilla "FormsModule" instead...
  ],
  ...

  - add-contact/add-contact.component.ts:
    -------------------------------------
@Component({
  selector: 'app-add-contact',
  templateUrl: './add-contact.component.html',
  styleUrls: ['./add-contact.component.css']
})
export class AddContactComponent implements OnInit {
  contact: Contact;

  constructor(private contactsService: ContactsService, private router: Router) { }

  ngOnInit() {
    this.contact = new Contact();
  }

  createContact() {
    console.log(this.contact);
    this.contactsService.createContact(this.contact).subscribe(
      data => {
        console.log('createContact,data=', data);
        this.router.navigate(['/']);
      },
      error => {
        console.error('ERROR@createContact()', error);
      });
  }
  <= route back to start page after successful "Add"

  - add-contact/add-contact.component.html:
    --------------------------------------
<div class="container" style="margin-top: 70px;">
  <div class="row">
    <div class="col-sm-8 offset-sm-2">
      <div>
        <form class="container" [formGroup]="contactForm" (ngSubmit)="createContact()">
          <div class="form-group">
            <label for="id">ID</label>
            <input [(ngModel)]="contact.contactId" type="text" name="id" class="form-control" id="id" aria-describedby="idHelp" placeholder="Enter ID">
            <small id="idHelp" class="form-text text-muted">Enter your contact’s ID</small>

            <label for="name">Contact Name</label>
            <input [(ngModel)]="contact.name" type="text" name="name" class="form-control" id="name" aria-describedby="nameHelp" placeholder="Enter your name">
            <small id="nameHelp" class="form-text text-muted">Enter your contact’s name</small>
            ...
          </div>
        </form>
        <button class="btn btn-primary" (click)="createContact()">Create contact</button>
      </div>
    </div>
  </div>
</div>
f
  - list-contacts/list-contacts.component.html:
    ------------------------------------------
      <button class="btn btn-primary" routerLink="/add-contact"> Add Contact</button>
        <= Add Contact" button will route here...

  - app-routing.module.ts:
    ---------------------
const routes: Routes = [
  { path: '', component: ListContactsComponent, pathMatch: 'full' },
  { path: 'add-contact', component: AddContactComponent },
  { path: 'test', component: TestAPIComponent },
  { path: '**', redirectTo: '/' }
];
   <= New "add-contact" route...

  - ng serve
    Debug > ERROR:
null                                                                           add-contact.component.ts:23
ContactsService.createContact(url=https://localhost:44362/api/Contacts/)...    contacts.service.ts:31
ERROR                                                                          AddContactComponent.html:3
  message:"Cannot read property 'contactId' of null"
TypeError: Cannot read property 'contactId' of null                            AddContactComponent.html:3
ERROR CONTEXT                                                                  AddContactComponent.html:3
...  <= PROBLEM: We had an "ID" text input entry.
        SOLUTION: Eliminated the field. The *SYSTEM* assigns contact ID (the REST service), *NOT* the user!

  - ng test => ERROR:
AddContactComponent > should create
Failed: Template parse errors:
Can't bind to 'ngModel' since it isn't a known property of 'input'. ("
    - SOLUTION:
        beforeEach(async(() => {
          TestBed.configureTestingModule({
            imports: [HttpClientTestingModule, FormsModule],
            providers: [ContactsService, HttpTestingController],
            declarations: [ AddContactComponent ]
            ...

    - NEXT ERROR:
NullInjectorError: StaticInjectorError(DynamicTestModule)[AddContactComponent -> Router]: 
  StaticInjectorError(Platform: core)[AddContactComponent -> Router]: 
    NullInjectorError: No provider for Router!
    - SOLUTION:
        import { RouterTestingModule } from '@angular/router/testing';
        ...
          beforeEach(async(() => {
            TestBed.configureTestingModule({
              imports: [HttpClientTestingModule, FormsModule, RouterTestingModule],
              providers: [ContactsService, HttpTestingController],
              declarations: [ AddContactComponent ]
              ...
              <= Clean compile!
    << Unit tests all compile/run cleanly >>

    - Formatting problem, add-contact.component.html:
      
    - FORMATTING PROBLEM: <label> and <input> for each item on SEPARATE LINES
      <= Too tall vertically...
    - SOLUTION:
https://stackoverflow.com/questions/48907760/label-and-input-in-same-line-on-form-group
      add-contact/add-contact.component.html:
      --------------------------------------
<div class="container" style="margin-top: 70px;">
  <h1>Contacts</h1>
    <form>
      <div class="form-group row">
        <label for="name" class="col-sm-2 col-form-label">Contact Name</label>
        <div class="col-sm-8">
          <input [(ngModel)]="contact.name" type="text" name="name" class="form-control" id="name" placeholder="Enter your name">
        </div>
      </div>

      <div class="form-group row">
        <label for="email" class="col-sm-2 col-form-label">Contact Email</label>
        <div class="col-sm-8">
          <input [(ngModel)]="contact.eMail" type="text" name="eMail" class="form-control" id="eMail" placeholder="Enter your email">
        </div>
      </div>
      ...
      <div class="form-group row">
        <label for="phone2" class="col-sm-2 col-form-label">Alternate Phone</label>
        <div class="col-sm-8">
          <input [(ngModel)]="contact.phone2" type="text" name="phone2" class="form-control" id="phone2" placeholder="Enter your alternate phone#">
        </div>
      </div>
  </form>
  <button class="btn btn-primary" (click)="createContact()">Create contact</button>
</div>

===================================================================================================

