using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContactsApp.Models;

namespace ContactsApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly ContactsContext _context;

        public ContactsController(ContactsContext context)
        {
            _context = context;
        }

        /*
         * GET: api/Contacts
         *
         * NOTE: extended the auto-generated code to accomodate "Get by Name"
         * EXAMPLES:
         * 1) GET  http://localhost:53561/api/Contacts  // Returns all contacts, including "Sy Snootles"
         * 2) GET  http://localhost:53561/api/Contacts/?name=Snoot  // Returns all names like "%Snoot%"
         */
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> GetContacts()
        {
            var queryParams = HttpContext.Request.Query;
            if (queryParams.Count == 0)
            {
                return await _context.Contacts.ToListAsync();
            }
            var param = queryParams.First();
            if ("name" == param.Key)
            {
                string targetName = "%" + param.Value + "%";
                var query = from c in _context.Contacts
                            where EF.Functions.Like(c.Name, targetName)
                            select c;
                return query.ToList();
            }
            return BadRequest();
        }

        // GET: api/Contacts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> GetContact(int id)
        {
            // var contact = await _context.Contacts.FindAsync(id);   Lazy-loading: fails to include related "Notes"
            var contact = await _context.Contacts
                .Include(c => c.Notes)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ContactId == id);

            if (contact == null)
            {
                return NotFound();
            }

            return contact;
        }

        // PUT: api/Contacts/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContact(int id, Contact contact)
        {
            if (id != contact.ContactId)
            {
                return BadRequest();
            }

            // Fetch entity as it exists in the DB...
            var contactToUpdate =  _context.Contacts
                .Include(c => c.Notes)
                .FirstOrDefault(c => c.ContactId == id);

            // Update properties of the parent
            _context.Entry(contactToUpdate).CurrentValues.SetValues(contact);

            // Remove or update child collection items
            List<Note> contactNotesClone = new List<Note>();
            foreach (Note n in contactToUpdate.Notes)
                contactNotesClone.Add(n);
            foreach (var contactNote in contactNotesClone)
            {
                // ... Fetch corresponding note from "contact" update
                var note = contact.Notes.SingleOrDefault(n => n.NoteId == contactNote.NoteId);
                if (note != null)
                {
                    // .. Present in "contact" update: write the updated "note" value to DB
                    _context.Entry(contactNote).CurrentValues.SetValues(note);
                } else
                {
                    // .. Not present in "contact" update: delete this note from DB
                    _context.Remove(contactNote);
                }
            }

            // Add the new items
            foreach (var note in contact.Notes)
            {
                if (contactNotesClone.All(n => n.NoteId != note.NoteId))
                {
                    // .. Not present in DB: add as a new "note"
                    contactToUpdate.Notes.Add(note);
                }
            }

            // OK: Refresh the database
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private void UpdateNotes (List<Note> notes, Contact contactToUpdate)
        {
            HashSet<int>currentNotes = new HashSet<int>(
                (contactToUpdate.Notes.Select(n => n.NoteId)));
            contactToUpdate.Notes = new List<Note>();
            foreach (var note in notes)
            {
                if (!currentNotes.Contains(note.NoteId))
                {
                    contactToUpdate.Notes.Add(note);
                } else
                {
                    _context.Notes.Update(note);
                }
            }


        }

        // POST: api/Contacts
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Contact>> PostContact(Contact contact)
        {
            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            //return CreatedAtAction("GetContact", new { id = contact.ContactId }, contact);
            return CreatedAtAction(nameof(GetContact), new { id = contact.ContactId }, contact);  // Better than hard-coding method name...
        }

        // DELETE: api/Contacts/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Contact>> DeleteContact(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            return contact;
        }

        private bool ContactExists(int id)
        {
            return _context.Contacts.Any(e => e.ContactId == id);
        }

        /** Added "Notes" APIs to auto-generated controller code */
        [HttpPut("Notes/{id}")]
        public async Task<IActionResult> PutNoteId(int noteId, Note note)
        {
            if (noteId != note.NoteId)
            {
                return BadRequest();
            }

            _context.Entry(note).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NoteExists(noteId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost("/Notes")]
        public async Task<ActionResult<Contact>> PostNote(Note note)
        {
            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            //return CreatedAtAction("GetContact", new { id = contact.ContactId }, contact);
            return null; // TBD
        }

        [HttpDelete("/Notes/{id}")]
        public async Task<ActionResult<Contact>> DeleteNote(int noteId)
        {
            var note = await _context.Notes.FindAsync(noteId);
            if (note == null)
            {
                return NotFound();
            }

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            return null;  // TBD
        }

        private bool NoteExists(int noteId)
        {
            return _context.Notes.Any(n => n.NoteId == noteId);
        }

    }
}
