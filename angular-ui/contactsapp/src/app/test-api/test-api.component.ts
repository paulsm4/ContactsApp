import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { Contact } from '../models/contact';
import { ContactsService } from '../services/contacts.service';
import { tap } from 'rxjs/operators';

@Component({
  selector: 'app-test-api',
  templateUrl: './test-api.component.html',
  styleUrls: ['./test-api.component.css']
})
export class TestAPIComponent implements OnInit {

  //// "$" convention denotes this as an "Observable"
  // contacts$: Observable<Contact[]>;
  contactsList: Contact[];

  constructor(private contactsService: ContactsService) { }

  ngOnInit() {
      // // tslint:disable-next-line: deprecation
      // this.contacts$ = this.contactsService.getContacts().pipe(tap(console.log));

      this.contactsService.getContacts().subscribe(
        data => {
          this.contactsList = data;
          console.log('data', data);
        },
        err => {
          console.log('err', err);
        });
  }

  loadContacts() {

  }
}
