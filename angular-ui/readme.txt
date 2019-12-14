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

* TBD:
  1. Implement "Delete" and "Modify"
  2. Determine if "Add" and "Modify" can share the same component, or need to be separate components
  3. Implement "Notes" (Create/Read/Update/Delete)
  4. General cleanup

===================================================================================================
* Angular UI (continued): Implement "delete"
  - "Material" confirmation/alert boxes:
https://appdividend.com/2019/02/11/angular-modal-tutorial-with-example-angular-material-dialog/
https://blog.vanila.io/just-another-custom-alert-for-angular-c288bebc3c96
https://blog.thoughtram.io/angular/2017/11/13/easy-dialogs-with-angular-material.html
https://stackoverflow.com/questions/49472031/display-a-simple-alert-dialog-in-material-angular
https://blog.angular-university.io/angular-material-dialog/
https://firstclassjs.com/create-a-reusable-confirmation-dialog-in-angular-7-using-angular-material/
https://material.angular.io/components/dialog/api
  <= "Material" seems to be the most popular alternative for "alert boxes"...

  - npm install --save @angular/material @angular/cdk @angular/animations
+ @angular/material@8.2.3
+ @angular/cdk@8.2.3
added 2 packages from 1 contributor, updated 1 package and audited 18885 packages in 98.592s
    <= We already imported "material" above
       The  Component Dev Kit (CDK) is a set of Material tools that implement common interaction patterns

  - app.module.ts:
    -------------
      import { MatDialogModule } from '@angular/material';
      import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
      ...
      @NgModule({
        declarations: [
          AppComponent,
          ...
          ConfirmationDlgComponent
       imports: [
          BrowserModule,
          BrowserAnimationsModule,
          ...
          MatDialogModule
        ],
        providers: [],
        entryComponents: [ ConfirmationDlgComponent ],
        bootstrap: [AppComponent]
        ...
        <= NOTES:
           1. We need to declare entryComponents: [] because the dialog will be created dynamically...
           2. ConfirmationDlgComponent TBD in the next step...

  - styles.css:
    ----------
    @import "~@angular/material/prebuilt-themes/indigo-pink.css";
    <= Dialog *WILL NOT WORK* correctly without a material .css theme

  - mkdir src/app/common
    common/confirmation-dlg.ts:
    --------------------------
      import { Component, Inject } from '@angular/core';
      import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
      
      @Component({
        template: `
        <h3 mat-dialog-title>{{dlgTitle}}</h3>
        <mat-dialog-content>{{dlgMessage}}</mat-dialog-content>
        <mat-dialog-actions>
          <button mat-button mat-dialog-close>No</button>
          <button mat-button [mat-dialog-close]="true" cdkFocusInitial>Yes</button>
        </mat-dialog-actions>
        `
      })
      export class ConfirmationDlgComponent {
        dlgTitle: string;
        dlgMessage: string;
      
        constructor(
          public dialogRef: MatDialogRef<ConfirmationDlgComponent>,
          @Inject(MAT_DIALOG_DATA) public extraData) {
          console.log('ConfirmationDlgComponent(), extraData:', extraData);
          this.dlgTitle = extraData.dlgTitle;
          this.dlgMessage = extraData.dlgMessage;
        }
        ...
        <= Note that we're injecting a MatDialogRef, along with optional  MatDialogConfig and extraData, into the component

  - list-contacts/list-contacts.component.html:
    ------------------------------------------
      ...
      <button class="btn btn-primary" (click)="deleteContact(contact)"> Delete Contact </button>
      <= ListContactsComponent "deleteContact()" will invoke the confirmation dialog, then call the "Delete" service...

  - list-contacts/list-contacts.component.ts.
    ----------------------------------------
      export class ListContactsComponent implements OnInit, OnDestroy  {
        ...
        constructor(
          private contactsService: ContactsService,
          private router: Router,
          public dialog: MatDialog) { }
        ...
        deleteContact(contact: Contact) {
          const dialogRef = this.dialog.open(ConfirmationDlgComponent, {
            hasBackdrop: true,
            position: {top: '', bottom: '', left: '', right: ''},
            data: {
              dlgTitle: 'Delete (' + contact.name + ')',
              dlgMessage: 'Really delete this contact?'
            }
          });

          dialogRef.afterClosed().subscribe(result => {
            if(result) {
              console.log('deleteContact@afterClosed(): Yes clicked');
              this.contactsService.deleteContact(contact).subscribe(
                data => {
                  console.log('deleteContact', data);
                  //this.router.navigate(['/']); This doesn't help: we're *already* here...
                  this.loadContacts();  // This is what we need...
                },
                err => {
                  console.error('deleteContact', err);
                });
            }
          });
        }
        ...
        <= Notes:
           1. We need to inject "MatDialog" to get a "MatDialogRef<ConfirmationDlgComponent>" dialogRef
           2. dialog.open() parameters: a) component type, b) MatDialogConfig, which may optionally contain c) "extra data"
           3. We subscribe() to the Observable returned by dialogRef.afterClosed()
           4. ConfirmationDlgComponent template HTML:
                <button mat-button [mat-dialog-close]="true"
                <= If result is "true", then we know the user hit "Yes", and we can delete the contact

  - ALSO:
    - Consider refactoring the confirmation dialog?
    - Consider re-writing (e.g. in Bootstrap 4)?
    - SO question: https://stackoverflow.com/a/59293891/3135317

===================================================================================================

* Angular UI: Edit Contact:
  - ng g component EditContact
    <= Q: Do we really need this?  Or can we re-use "AddContactComponent?
       GAMEPLAN: Let's start off by creating it, and copy/paste from "AddContactComponent as needed..

  - Copied/pasted from add-contact.component.ts/html

  - list-contacts.component.html:
    ----------------------------
      <button class="btn btn-primary" (click)="editContact(contact.contactId)"> Edit Contact </button>

  - list-contacts.component.ts:
    --------------------------
      editContact(contactId: number) {
        this.router.navigate(['/edit-contact', contactId]);
      }

  - app-routing.module.ts:
    ---------------------]
      const routes: Routes = [
        { path: '', component: ListContactsComponent, pathMatch: 'full' },
        { path: 'add-contact', component: AddContactComponent },
        { path: 'edit-contact', component: EditContactComponent },
        { path: 'test', component: TestAPIComponent },
        { path: '**', redirectTo: '/' }
      ];
      
  - ng test =>
    - ERRORS:
ListContactsComponent > should create
NullInjectorError: StaticInjectorError(DynamicTestModule)[ListContactsComponent -> Router]: 
  StaticInjectorError(Platform: core)[ListContactsComponent -> Router]: 
    NullInjectorError: No provider for Router!
    <= Need to add HttpClientTestingModule, RouterTestingModule to Jasmine test:

ListContactsComponent > should create
Failed: Unexpected value 'undefined' imported by the module 'DynamicTestModule'
error properties: Object({ ngSyntaxError: true })
Error: Unexpected value 'undefined' imported by the module 'DynamicTestModule'

    - list-contacts.component.spec.ts:
      -------------------------------
        beforeEach(async(() => {
          TestBed.configureTestingModule({
            imports: [HttpClientTestingModule, RouterTestingModule, MatDialogModule],
            providers: [ContactsService, HttpTestingController],
            declarations: [ ListContactsComponent ]
          })
        <= ListContactsComponent doesn't have any direct dependency on "Material Dialog", so why the error?!?

    - NEXT ERROR:
EditContactComponent > should create
Failed: Template parse errors:
Can't bind to 'ngModel' since it isn't a known property of 'input'. ("lass="col-sm-2 col-form-label">Contact Name</label>
        <div class="col-sm-8">
          <input [ERROR ->][(ngModel)]="contact.name" type="text" name="name" class="form-control" id="name" placeholder="Enter "): ng:///DynamicTestModule/EditContactComponent.html@6:17
      <= Need to import "FormsModule".  Since we're going to need them sooner or later, just add all the imports now:'

    - edit-contacts.component.spec.ts:
      -------------------------------
        beforeEach(async(() => {
            TestBed.configureTestingModule({
              imports: [HttpClientTestingModule, FormsModule, RouterTestingModule, MatDialogModule],
              declarations: [ EditContactComponent ]
            })
        <= Fixed the problem ... but one more (new!) error..

    - NEXT ERROR:
EditContactComponent > should create
TypeError: Cannot read property 'name' of undefined
error properties: Object({ ngDebugContext: DebugContext_({ view: Object({ def: Object({ factory: Function, nodeFlags: 4869123, rootNodeFlags: 1, nodeMatchedQueries: 0, flags: 0, nodes: [ Object({ nodeIndex: 0, parent: null, renderParent: null, bindingIndex: 0, outputIndex: 0, checkIndex: 0, flags: 1, childFlags: 4869123, directChildFlags: 1, childMatchedQueries: 0, matchedQueries: Object({  }), matchedQueryIds: 0, references: Object({  }), ngContentIndex: null, childCount: 99, bindings: [  ], bindingFlags: 0, outputs: [  ], element: Object({ ns: '', name: 'div', attrs: [ Array, Array ], template: null, componentProvider: null, componentView: null, componentRendererType: null, publicProviders: null({  }), allProviders: null({  }), handleEvent: Function }), provider: null, text: null, query: null, ngContent: null }), Object({ nodeIndex: 1, parent: Object({ nodeIndex: 0, parent: null, renderParent: null, bindingIndex: 0, outputIndex: 0, checkIndex: 0, flags: 1, childFlags: 4869123, directChildFlags: 1,  ...
    at <Jasmine>
    at Object.eval [as updateDirectives] (ng:///DynamicTestModule/EditContactComponent.ngfactory.js:443:34)
    <= "contact" is initially null (until we read it from REST, in ngInit())

    - edit-contacts.component.ts:
      --------------------------
        export class EditContactComponent implements OnInit {
          contact: Contact;
          ...
          ngOnInit() {
            this.contact = new Contact();  // Temporary hack (to satisfy unit test)
            // Fetch contact from REST service
          <= OK: everything "green"!
    << Saved .bu10 >>

  - ng serve:
    - Set bkpts: ListContact@EditContact, EditContact@ngInit
      VSCode > Debug >
      - Hits ListContact@editContact(contactId: number) { this.router.navigate(['/edit-contact', contactId]);
        <= OK, contactId= 17... 
           ... but stays in same place (doesn't "navigate" to edit page...)
      - TROUBLESHOOTING:       
          this.router.navigate(['/edit-contact']  <= This works (no parameter)
      - PROBLEM:
          <= If I want to pass an argument ... then I need to declare it in the route!
      - app-routing.module.ts:
        ---------------------
          { path: 'edit-contact/:contactId', component: EditContactComponent },
      - list-contacts.component.ts:
        --------------------------
          editContact(contactId: number) {
            this.router.navigate(['/edit-contact', contactId]);
            <= This works!
               URL= http://localhost:4200/edit-contact/17

  - Working code:
    - app-routing.module.ts:
      ---------------------
const routes: Routes = [
  { path: '', component: ListContactsComponent, pathMatch: 'full' },
  { path: 'add-contact', component: AddContactComponent },
  { path: 'edit-contact/:contactId', component: EditContactComponent },
  { path: 'test', component: TestAPIComponent },
  { path: '**', redirectTo: '/' }
];

    - list-contacts.component.html:
      ----------------------------
        <td><button class="btn btn-primary" (click)="editContact(contact.contactId)"> Edit Contact </button>

    - list-contacts.component.ts:
      --------------------------
        editContact(contactId: number) {
            this.router.navigate(['/edit-contact', contactId]);
          }    

    - edit-contact.component.html:
      ---------------------------
        <div class="container" style="margin-top: 70px;">
          <h1>Edit {{contact.name}}:</h1>
            <form>
              <div class="form-group row">
                <label for="name" class="col-sm-2 col-form-label">Contact Name</label>
                <div class="col-sm-8">
                  <input [(ngModel)]="contact.name" type="text" name="name" class="form-control" id="name">
                  ...
            </form>
          <button class="btn btn-primary" (click)="updateContact()">Update contact</button>
        </div>

    - edit-contact.component.ts:
      -------------------------
        export class EditContactComponent implements OnInit {
          contact: Contact;
          contactId: number;
        
          constructor(
            private contactsService: ContactsService,
            private activatedRoute: ActivatedRoute,
            private router: Router,
            public dialog: MatDialog) {
              // tslint:disable-next-line:no-string-literal
              this.contactId = activatedRoute.snapshot.params['contactId'];
              this.contact = new Contact();  // Hack (to avoid "undefined" warning)
              ...
          ngOnInit() {
            // Fetch contact from REST service
            this.contactsService.getContact(this.contactId).subscribe(result => {
              this.contact = result;
            });
            ...
           updateContact() {
            // Confirm update
            const dialogRef = this.dialog.open(ConfirmationDlgComponent, {
              hasBackdrop: true,
              position: {top: '', bottom: '', left: '', right: ''},
              data: {
                dlgTitle: 'Update (' + this.contact.name + ')',
                dlgMessage: 'Update this contact?'
              }
            });
        
            dialogRef.afterClosed().subscribe(result => {
              if (result) {
                // Call REST service to update contact
                this.contactsService.updateContact(this.contact).subscribe(
                  data => {
                    this.router.navigate(['/']);
                  },
                  err => {
                    console.error('updateContact', err);
                  });
              }
            });
          ...

    - contacts.service.ts:
      -------------------
        updateContact(contact: Contact): Observable<Contact> {
          const url = this.mkUrl() + contact.contactId;
          console.log('ContactsService.updateContact(url=' + url + '):', contact);
          return this.http.put<Contact>(url, JSON.stringify(contact), this.httpOptions)
          .pipe(
            retry(1),
            catchError(this.errorHandler)
          );
          ...

===================================================================================================

* Angular UI: "Notes" functionality
  - GAMEPLAN: Get "Edit Contact" working first
    <= Display, add, modify, delete note(s)

  - Ensure there's a test record with some notes:
      Postman: POST https://localhost:44362/api/Contacts
{
  "name": "Sy Snootles",
  "eMail": "ss@abc.com",
  "phone1": "111-222-3333",
  "address1": "Emerald City",
  "state": "Oz",
  "zip": "00000",
  "Notes":[
    {"Text": "Now is the time for all good men"},
    {"Text": "How now brown cow"},
    {"Text": "Bibbidi Boppidi Boo"}
  ]
}  <= ContactId #18; NoteIds 24, 25, 26...

  - models/contact.ts:
    -----------------
      export class Contact {
        contactId?: number;
        name: string;
        eMail: string;
        ...
        notes: Note[];  // <-- Added this
      }

  - models/note.ts:
    --------------
      export class Note {
        noteId?: number;
        text: string;
        date: Date;
        contactId?: number;
        public constructor(text: string) { this.text = text; }  // <-- Added constructor to simplify creating new note
        ...

  - edit-contact.component.html:
    ---------------------------
      <div class="container" style="margin-top: 70px;">
        <h1>Edit {{contact.name}}:</h1>
          <form>
            <div class="form-group row">
              <label for="name" class="col-sm-2 col-form-label">Contact Name</label>
              <div class="col-sm-8">
                <input [(ngModel)]="contact.name" type="text" name="name" class="form-control" id="name">
                ...
            <div class="form-group row">
              <label for="eMail" class="col-sm-2 col-form-label">Contact Email</label>
              <div class="col-sm-8">
                <input [(ngModel)]="contact.eMail" type="text" name="eMail" class="form-control" id="eMail">
                ...
            <div class="card">
              <div class="card-body">
                <table border="0" cellpadding="2" width="100%">
                  <thead>
                    <tr>
                    <th>Note#</th><th>Date</th><th>Text</th><th colspan="2">Action</th>
                  </tr>
                  </thead>
                  <tr *ngFor="let note of contact.notes">
                    <td>{{ note.noteId }}</td>
                    <td> {{ note.date | date:'short' }}</td>
                    <td> {{ note.text }}</td>
                    <td style="float:right">
                      <button class="btn btn-primary btn-sm float-right" (click)="editNote(note)"> Edit </button>
                    </td>
                    <td style="float:right">
                      <button class="btn btn-primary btn-sm float-right" (click)="deleteNote(note)"> Delete </button>
                    </td>
                  </tr>
                </table>
              <button class="btn btn-primary btn-sm" (click)="addNote()"> Add Note </button>
              </div>
            </div>
          </form>
          ...  <= Added an HTML table within a Bootstrap card for the "notes" section
                  TBD: FIX THIS LAYOUT!!!

  - edit-contact.component.ts:
    -------------------------
export class EditContactComponent implements OnInit {
  contact: Contact;
  contactId: number;
  ...
  ngOnInit() {
    // Fetch contact from REST service
    this.contactsService.getContact(this.contactId).subscribe(result => {
      this.contact = result;
    });
  ...
  addNote() { /* TBD */ }
  ...
  editNote(note: Note) { /* TBD */ }
  ...
  deleteNote(note: Note) {
    // Delete specified note from contact "notes" array
    this.contact.notes = this.contact.notes.filter((e) => {
      return e !== note;
    });

    // Update REST service with the modified record
    this.updateContact();
  }
  <= So far, so good.  "Delete" works.  "Formatting" stinks...

  - Implement "Add":
    - edit-contact.component.html:
      ---------------------------
        <div class="card">
          <div class="card-body">
            ...
            <div class="form-group row col-sm-8">
              <input [(ngModel)]="newNote" type="text" name="newNote" class="form-control" id="newNote">
              <button class="btn btn-primary btn-sm" (click)="addNote()"> Add Note </button>

    - edit-contact.component.ts:
      -------------------------
        export class EditContactComponent implements OnInit {
          contact: Contact;
          contactId: number;
          newNote: string;  // <-- Added this
          ...
          addNote() {
            this.contact.notes.push(new Note(this.newNote));
            this.updateContact();
          }
        <= Ugly, but it works...

  - Implement "Modify":
    - edit-contact.component.html:
      ---------------------------
        <div class="card">
          <div class="card-body">
            <table border="0" cellpadding="2" width="100%">
              ...
              <tr *ngFor="let note of contact.notes">
                <td>{{ note.noteId }}</td>
                <td> {{ note.date | date:'short' }}</td>
                <td><input [(ngModel)]="note.text" type="text" name="text" class="form-control" id="note.noteId"></td>
      <= This does *NOT* work - it repeats the test from the LAST note...

    - EXPLANATION:
      https://stackoverflow.com/a/38368261/3135317
      - "Using @angular/forms when you use a <form> tag it automatically creates a FormGroup.
      -  For every contained ngModel tagged <input> it will create a FormControl and add it into
         the FormGroup created above; this FormControl will be named into the FormGroup using attribute name.
      - When you mark it as standalone: true this will not happen (it will not be added to the FormGroup).

    - SOLUTION:
      edit-contact.component.html:
      ---------------------------
       <td><input [(ngModel)]="note.text" type="text"  [ngModelOptions]="{standalone: true}" class="form-control"></td>

    - edit-contact.component.ts:
      -------------------------
       editNote() {
           this.updateContact();
         }

  - ng test:
ERROR in src/app/models/note.spec.ts:5:12 - error TS2554: Expected 1 arguments, but got 0.
5     expect(new Note()).toBeTruthy()
             ~~~~~~~~~~
  src/app/models/note.ts:6:22
    6   public constructor(text: string) { this.text = text; }
                           ~~~~~~~~~~~~
    An argument for 'text' was not provided.
    <= Test build failing: Now that we've specified a non-default constructor, "note" test failing...

    - WORKAROUND:
      note.spec.ts:
      ------------
        describe('Note', () => {
          it('should create an instance', () => {
            expect(new Note('abc')).toBeTruthy();
          });
        });


    - ng test:
        12 specs, 0 failures, randomized with seed 30902
        <= All "green"...

      

       
      