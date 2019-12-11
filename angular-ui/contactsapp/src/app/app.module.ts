import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ListContactsComponent } from './list-contacts/list-contacts.component';
import { EditContactComponent } from './edit-contact/edit-contact.component';
import { EditNoteComponent } from './edit-note/edit-note.component';
import { TestAPIComponent } from './test-api/test-api.component';
import { AddContactComponent } from './add-contact/add-contact.component';

@NgModule({
  declarations: [
    AppComponent,
    ListContactsComponent,
    EditContactComponent,
    EditNoteComponent,
    TestAPIComponent,
    AddContactComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
