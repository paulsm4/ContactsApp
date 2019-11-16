using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using ContactsApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContactsApp.Tests
{
    [TestClass]
    public class ContactsDBTests
    {
        [TestInitialize]
        public void SetupTest()
        {
            Database.SetInitializer(new ContactsApp.Models.ContactsDBInitializer());
        }

        private async Task<List<Contact>> GetContacts(ContactsDB db)
        {
            List<Contact> contacts = await db.Contacts.ToListAsync();
            return contacts;
        }
        //
        // NOTE: DELETING ContactsApp database before running should invoke DB Initializer
        //
        [TestMethod]
        public async Task TestInitializerSeed()
        {
            using (ContactsDB db = new ContactsDB())
            {
                var contactsList = await GetContacts(db);
                Assert.IsTrue(contactsList.Count > 0, "there will be one record in newly initialized database");
                Contact contact = contactsList[0];
                Assert.IsTrue(contact.Name == "abc", "initial contact Name \"abc\"");
                Assert.IsTrue(contact.EMail == "abc@xyz.com", "initial contact EMail \"abc@xyz.com\"");
            }
        }

        [TestMethod]
        public void TestAddContact()
        {
            using (ContactsDB db = new ContactsDB())
            {
                // Create new contact
                Contact contact = new Contact();
                contact.Name = "test-add";
                contact.EMail = "test-add@xyz.com";
                contact.Phone1 = "999-888-7777";
                db.Contacts.Add(contact);
                db.SaveChanges();
            }
        }
    }
    }