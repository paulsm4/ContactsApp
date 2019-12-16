import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { Contact } from '../models/contact';
import { ContactsService } from '../services/contacts.service';

@Component({
  selector: 'app-test-api',
  templateUrl: './test-api.component.html',
  styleUrls: ['./test-api.component.css']
})
export class TestAPIComponent implements OnInit, OnDestroy {

  //// "$" convention denotes this as an "Observable"
  // contacts$: Observable<Contact[]>;
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
