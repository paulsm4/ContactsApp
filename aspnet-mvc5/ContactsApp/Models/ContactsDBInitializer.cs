using ContactsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContactsApp.Models
{
    public class ContactsDBInitializer :
        //System.Data.Entity.DropCreateDatabaseAlways<ContactsDB>
        System.Data.Entity.CreateDatabaseIfNotExists<ContactsDB>
    {
        protected override void Seed(ContactsDB context)
        {
            // Create new contact
            Contact contact = new Contact();
            contact.Name = "abc";
            contact.EMail = "abc@xyz.com";
            contact.Phone1 = "111-222-3333";
            contact.Address1 = "2001 Country Lane";
            contact.City = "Cypress";
            contact.State = "CA";
            contact.Zip = "90630";

            // Create new notes; append to contact record
            List<Note> notes = new List<Note>();
            Note note = new Note();
            note.Text = "An example note...";
            notes.Add(note);
            note.Text = "Another example note...";
            notes.Add(note);
            contact.Notes = notes;

            // Add to DB
            context.Contacts.Add(contact);
            base.Seed(context);
        }
    }
}