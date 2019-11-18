using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using ContactsApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace ContactsApp.Tests
{
    [TestClass]
    public class ContactsDBTests
    {
        private const string TEST_NAME = "TestName";

        //[TestInitialize]
        //public void SetupTest()
        //{
        //    Database.SetInitializer(new ContactsApp.Models.ContactsDBInitializer());
        //}

        private bool PurgeTestRecord(ContactsDB db)
        {
            bool result = false;
            try
            {
                var resultSet = db.Contacts
                                    .Where(c => c.Name == TEST_NAME);
                int i = resultSet.Count();
                if (resultSet.Count() > 0)
                {
                    db.Contacts.RemoveRange(resultSet);
                    db.SaveChanges();
                }
                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        private bool CreateTestContact(ContactsDB db)
        {
            bool result = false;
            try
            {
                Contact contact = new Contact();
                contact.Name = TEST_NAME;
                contact.EMail = TEST_NAME + "@xyz.com";
                contact.Phone1 = "999-888-7777";
                db.Contacts.Add(contact);
                db.SaveChanges();
                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        //
        // NOTE: DELETING ContactsApp database before running should invoke DB Initializer
        //
        [TestMethod]
        public async Task TestInitializerSeed()
        {
            using (ContactsDB db = new ContactsDB())
            {
                List<Contact> contactsList = await db.Contacts.ToListAsync();
                Assert.IsTrue(contactsList.Count > 0, "there will be one record in newly initialized database");
                Contact contact = contactsList[0];
                Assert.IsTrue(contact.Name == "abc", "initial contact Name \"abc\"");
                Assert.IsTrue(contact.EMail == "abc@xyz.com", "initial contact EMail \"abc@xyz.com\"");
            }
        }

        [TestMethod]
        public async Task TestContacts()
        {
            using (ContactsDB db = new ContactsDB())
            {
                // Ensure Test Record does not exist
                Assert.IsTrue(PurgeTestRecord(db), "PurgeTestRecord unsuccessful");

                // Query current database
                List<Contact> contactsList = await db.Contacts.ToListAsync();
                int initialCount = contactsList.Count;

                // CREATE: Create new contact
                Assert.IsTrue(CreateTestContact(db), "CreateTestContact unsuccessful");

                // READ: verify contact created
                contactsList = await db.Contacts.ToListAsync();
                int currentCount = contactsList.Count;
                //Assert.Equals(currentCount, (initialCount + 1)); // FAILS
                Assert.IsTrue(currentCount == (initialCount + 1));

                var resultSet = db.Contacts.Where(c => c.Name == TEST_NAME);
                currentCount = resultSet.Count();
                //Assert.Equals(currentCount, 1);  // FAILS
                Assert.IsTrue(currentCount == 1);

                Contact contact = resultSet.First();
                Assert.AreEqual(contact.EMail, TEST_NAME + "@xyz.com");
                Assert.AreEqual(contact.Phone1, "999-888-7777");

                // UPDATE: verify record modification
                contact.Phone1 = "111-222-3333";
                db.SaveChanges();
                contactsList = await db.Contacts.ToListAsync();
                resultSet = db.Contacts
                             .Where(c => c.Name == TEST_NAME);
                contact = resultSet.First();
                currentCount = resultSet.Count();
                Assert.IsTrue(currentCount == 1);
                Assert.AreEqual(contact.Phone1, "111-222-3333");

                // DELETE: verify record delection
                db.Contacts.Remove(contact);
                db.SaveChanges();

                contactsList = await db.Contacts.ToListAsync();
                currentCount = contactsList.Count;
                Assert.IsTrue(currentCount == initialCount);
            }
        }

        [TestMethod]
        public void TestNotes()
        {
            using (ContactsDB db = new ContactsDB())
            {
                // Ensure Test Record does not exist
                Assert.IsTrue(PurgeTestRecord(db), "PurgeTestRecord unsuccessful");

                // Create test record
                Assert.IsTrue(CreateTestContact(db), "CreateTestContact unsuccessful");
                List<Contact> contactsList = db.Contacts.Where(c => c.Name == TEST_NAME).ToList();
                Assert.IsTrue(contactsList.Count == 1);

                // Verify there aren't any notes
                Contact contact = contactsList[0];
                Assert.IsNull(contact.Notes);

                // CREATE: Create new note

                // UPDATE: Modify note

                // DELETE: Delete note

            }
        }
    }
}    