import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { MatDialogModule } from '@angular/material';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ListContactsComponent } from './list-contacts/list-contacts.component';
import { EditContactComponent } from './edit-contact/edit-contact.component';
import { EditNoteComponent } from './edit-note/edit-note.component';
import { TestAPIComponent } from './test-api/test-api.component';
import { AddContactComponent } from './add-contact/add-contact.component';
import { ConfirmationDlgComponent } from './common/confirmation-dlg.component';

@NgModule({
  declarations: [
    AppComponent,
    ListContactsComponent,
    EditContactComponent,
    EditNoteComponent,
    TestAPIComponent,
    AddContactComponent,
    ConfirmationDlgComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    MatDialogModule
  ],
  providers: [],
  entryComponents: [ ConfirmationDlgComponent ],
  bootstrap: [AppComponent]
})
export class AppModule { }
