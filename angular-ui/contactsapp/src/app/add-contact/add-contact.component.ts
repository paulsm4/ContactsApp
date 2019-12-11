import { Component, OnInit } from '@angular/core';
import { ContactsService } from '../services/contacts.service';
import { Contact } from '../models/contact';
import { Router } from '@angular/router';

@Component({
  selector: 'app-add-contact',
  templateUrl: './add-contact.component.html',
  styleUrls: ['./add-contact.component.css']
})
export class AddContactComponent implements OnInit {
  contact: Contact;

  constructor(private contactsService: ContactsService, private router: Router) { }

  ngOnInit() {
    this.contact = new Contact();
  }

  createContact() {
    console.log(this.contact);
    this.contactsService.createContact(this.contact).subscribe(
      data => {
        console.log('createContact,data=', data);
        this.router.navigate(['/']);
      },
      error => {
        console.error('ERROR@createContact()', error);
      });
  }

}
