import { Component, OnInit, OnDestroy } from '@angular/core';
import { MatDialog } from '@angular/material';
import { Subscription } from 'rxjs';
import { Contact } from '../models/contact';
import { ContactsService } from '../services/contacts.service';
import { ConfirmationDlgComponent } from '../common/confirmation-dlg.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-list-contacts',
  templateUrl: './list-contacts.component.html',
  styleUrls: ['./list-contacts.component.css']
})
export class ListContactsComponent implements OnInit, OnDestroy  {
  contactsList: Contact[];
  selectedContact: Contact;
  private contactsSubscription$: Subscription;

  constructor(
    private contactsService: ContactsService,
    private router: Router,
    public dialog: MatDialog) { }

  ngOnInit() {
    this.loadContacts();
  }

  ngOnDestroy() {
    this.contactsSubscription$.unsubscribe();
  }

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

  editContact(contact: Contact) {
    ; // TBD
  }

  loadContacts() {
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
