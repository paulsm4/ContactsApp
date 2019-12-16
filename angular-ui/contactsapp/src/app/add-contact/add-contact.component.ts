import { Component, OnInit } from '@angular/core';
import { ContactsService } from '../services/contacts.service';
import { Contact } from '../models/contact';
import { Router } from '@angular/router';
import { Note } from '../models/note';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-add-contact',
  templateUrl: './add-contact.component.html',
  styleUrls: ['./add-contact.component.css']
})
export class AddContactComponent implements OnInit {
  contact: Contact;
  newNote: string;

  constructor(
    private contactsService: ContactsService,
    private router: Router,
    private datePipe: DatePipe) {
      this.newNote = 'New contact added on ' + this.datePipe.transform(new Date(), 'MMM dd, yyyy HH:mm');
     }

  ngOnInit() {
    this.contact = new Contact();
  }

  createContact() {
    console.log(this.contact);

    // Add Initial comment
    this.contact.notes.push(new Note(this.newNote));

    // Call service to add new Contact record
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
