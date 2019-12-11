import { Component, OnInit, OnDestroy } from '@angular/core';
import { Contact } from '../models/contact';
import { Subscription } from 'rxjs';
import { ContactsService } from '../services/contacts.service';

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

  deleteContact(contact: Contact) {
    ; // TBD
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
