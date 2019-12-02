using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MinimalEF.Models
{
    public class ContactsContext2 : DbContext
    {
        //// This constructor allows .Net Core runtume to inject DBContext options
        //public ContactsContext2(DbContextOptions<ContactsContext2> options) : base(options)
        //{
        //}

        // For standalone (console) app, we'll set options by overriding OnConfiguring()
        public ContactsContext2()
        {
            ;
        }
        public DbSet<MinimalEF.Models.Contact> Contacts { get; set; }
        public DbSet<MinimalEF.Models.Note> Notes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // For standalone app, we'll hard-code connection string here, vs. reading from application.json
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=ContactsDB2;Trusted_Connection=True");
        }
    }
}
