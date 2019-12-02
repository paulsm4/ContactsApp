using MinimalEF.Models;
using System;
using System.Collections.Generic;

namespace MinimalEF
{
    class Program
    {
        static void Main(string[] args)
        {
            // Instantiate a Contacts2 object
            Console.WriteLine("Testing \"Minimal EF\" app...");
            Contacts2 contacts2 = new Contacts2();

            // Query current contacts
            var contacts = contacts2.GetAllContacts();
            int initialContacts = contacts.Count;
            Console.WriteLine("#/Contacts=" + initialContacts);

            // Add a new contact
            Contact c1 = new Contact();
            c1.Name = "Test-Contact";
            c1.EMail = "tc@abc.com";
            c1.Phone1 = "111-222-3333";
            c1.Phone2 = "444-555-6666";

            List<Note> notes = new List<Note>();
            Note n = new Note();
            n.Text = "1st note";
            notes.Add(n);
            n = new Note();
            n.Text = "2nd note";
            notes.Add(n);
            c1.Notes = notes;

            int contactId = contacts2.AddContact(c1);
            Console.WriteLine("Added new contact, ID=" + contactId);

            // Fetch contact details
            Contact c2 = contacts2.GetContact(contactId);
            Console.WriteLine("New contact details=" + c2.ToString());

            // Modify contact
            c2.Phone2 = "999-999-9999";
            contacts2.UpdateContact(c2);

            c2 = contacts2.GetContact(contactId);
            Console.WriteLine("Updated contact details=" + c2.ToString());

            // Delete contact
            contacts2.DeleteContact(contactId);

            // Query all one final time
            contacts = contacts2.GetAllContacts();
            Console.WriteLine("start #/Contacts=" + initialContacts + ", end #/Contacts=" + contacts.Count + ".");
        }
    }
}
