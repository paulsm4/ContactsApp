using Microsoft.EntityFrameworkCore;

namespace ContactsApp.Models
{
    public class ContactsContext : DbContext
    {
        public ContactsContext(DbContextOptions<ContactsContext> options) : base(options)
        {
        }

        public DbSet<ContactsApp.Models.Contact> Contacts { get; set; }
        public DbSet<ContactsApp.Models.Note> Notes { get; set; }
    }
   
}
