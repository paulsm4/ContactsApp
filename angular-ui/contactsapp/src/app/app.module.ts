import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AddContactComponent } from './contacts/add-contact/add-contact.component';
import { UpdateContactComponent } from './contacts/update-contact/update-contact.component';
import { ListContactsComponent } from './contacts/list-contacts/list-contacts.component';
import { AddNoteComponent } from './contacts/add-note/add-note.component';
import { TestAPIComponent } from './test-api/test-api.component';

@NgModule({
  declarations: [
    AppComponent,
    AddContactComponent,
    UpdateContactComponent,
    ListContactsComponent,
    AddNoteComponent,
    TestAPIComponent
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
