import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TestAPIComponent } from './test-api/test-api.component';
import { ListContactsComponent } from './list-contacts/list-contacts.component';
import { AddContactComponent } from './add-contact/add-contact.component';
import { EditContactComponent } from './edit-contact/edit-contact.component';


const routes: Routes = [
  { path: '', component: ListContactsComponent, pathMatch: 'full' },
  { path: 'add-contact', component: AddContactComponent },
  { path: 'edit-contact/:contactId', component: EditContactComponent },
  { path: 'test', component: TestAPIComponent },
  { path: '**', redirectTo: '/' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
