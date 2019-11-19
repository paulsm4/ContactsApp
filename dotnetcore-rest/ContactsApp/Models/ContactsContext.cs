using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
