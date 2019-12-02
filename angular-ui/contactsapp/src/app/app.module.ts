import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AddContactComponent } from './contacts/add-contact/add-contact.component';
import { UpdateContactComponent } from './contacts/update-contact/update-contact.component';
import { ListContactsComponent } from './contacts/list-contacts/list-contacts.component';
import { AddNoteComponent } from './contacts/add-note/add-note.component';

@NgModule({
  declarations: [
    AppComponent,
    AddContactComponent,
    UpdateContactComponent,
    ListContactsComponent,
    AddNoteComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }