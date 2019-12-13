import { Component, OnInit } from '@angular/core';
import { ContactsService } from '../services/contacts.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Contact } from '../models/contact';
import { Note } from '../models/note';
import { ConfirmationDlgComponent } from '../common/confirmation-dlg.component';
import { MatDialog } from '@angular/material';

@Component({
  selector: 'app-edit-contact',
  templateUrl: './edit-contact.component.html',
  styleUrls: ['./edit-contact.component.css']
})
export class EditContactComponent implements OnInit {
  contact: Contact;
  contactId: number;

  constructor(
    private contactsService: ContactsService,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    public dialog: MatDialog) {
      // tslint:disable-next-line:no-string-literal
      this.contactId = activatedRoute.snapshot.params['contactId'];
      console.log('EditContactComponent(), contactId=' + this.contactId);
  }

  ngOnInit() {
    // Fetch contact from REST service
    this.contact = new Contact();  // Temporary hack (to satisfy unit test)
    this.contactsService.getContact(this.contactId).subscribe(result => {
      this.contact = result;
    });

  }

  updateContact() {
    // Confirm update
    const dialogRef = this.dialog.open(ConfirmationDlgComponent, {
      hasBackdrop: true,
      position: {top: '', bottom: '', left: '', right: ''},
      data: {
        dlgTitle: 'Update (' + this.contact.name + ')',
        dlgMessage: 'Update this contact?'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        // Call REST service to update contact
        console.log('updateContact@afterClosed(): Yes clicked');
        this.contactsService.updateContact(this.contact).subscribe(
          data => {
            console.log('updateContact', data);
            this.router.navigate(['/']);
          },
          err => {
            console.error('updateContact', err);
          });
      }
    });

  }

}
