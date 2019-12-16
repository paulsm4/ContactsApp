using Microsoft.EntityFrameworkCore;
using MinimalEF.Models;
using System.Collections.Generic;
using System.Linq;

namespace MinimalEF
{
    public class Contacts2
    {
        private ContactsContext2 _context;

        public Contacts2()
        {
            using (_context = new ContactsContext2())
            {
                //context.Database.EnsureDeleted();
                // Create database, if needed
                _context.Database.EnsureCreated();
            }
        }

        public List<Contact> GetAllContacts()
        {
            using (_context = new ContactsContext2())
            {
                return _context.Contacts.ToList();
            }
        }

        public Contact GetContact(int id)
        {
            Contact contact = null;
            using (_context = new ContactsContext2())
            {
                contact =
                    _context.Contacts.Include(c => c.Notes).First(c => c.ID == id);
            }
            return contact;
        }

        public int AddContact(Contact contact)
        {
            using (_context = new ContactsContext2())
            {
                _context.Contacts.Add(contact);
                _context.SaveChanges();
                var c = _context.Entry(contact);
                return c.Entity.ID;
            }
        }

        public void UpdateContact(Contact contact)
        {
            using (_context = new ContactsContext2())
            {
                // Fetch current version in DB
                var contactToUpdate =
                    _context.Contacts.Include(c => c.Notes).First(c => c.ID == contact.ID);

                // Update properties of the parent
                _context.Entry(contactToUpdate).CurrentValues.SetValues(contact);


                // Clone contactNotes to mitigate:
                //   "System.InvalidOperationException: Collection was modified after the enumerator was instantiated."
                List<Note> contactNotesClone = new List<Note>();
                foreach (Note n in contactToUpdate.Notes)
                    contactNotesClone.Add(n);

                // Remove or update child collection items
                foreach (var contactNote in contactNotesClone)
                {
                    // ... Fetch corresponding note from "contact" update
                    var note = contact.Notes.SingleOrDefault(n => n.ID == contactNote.ID);
                    if (note != null)
                    {
                        // .. Present in "contact" update: write the updated "note" value to DB
                        _context.Entry(contactNote).CurrentValues.SetValues(note);
                    }
                    else
                    {
                        // .. Not present in "contact" update: delete this note from DB
                        _context.Remove(contactNote);
                    }
                }

                // Add the new items
                foreach (var note in contact.Notes)
                {
                    if (contactNotesClone.All(n => n.ID != note.ID))
                    {
                        // .. Not present in DB: add as a new "note"
                        contactToUpdate.Notes.Add(note);
                    }
                }

                // OK: Refresh the database
                _context.SaveChanges();
            }
        }

        public void DeleteContact(int id)
        {
            using (_context = new ContactsContext2())
            {
                var contact =
                    _context.Contacts.Include(c => c.Notes).First(c => c.ID == id);
                _context.Contacts.Remove(contact);
                _context.SaveChanges();
            }

        }
    }
}
