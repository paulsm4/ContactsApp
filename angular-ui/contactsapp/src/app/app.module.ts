import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ListContactsComponent } from './list-contacts/list-contacts.component';
import { EditContactComponent } from './edit-contact/edit-contact.component';
import { EditNoteComponent } from './edit-note/edit-note.component';
import { TestAPIComponent } from './test-api/test-api.component';

@NgModule({
  declarations: [
    AppComponent,
    ListContactsComponent,
    EditContactComponent,
    EditNoteComponent,
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
