using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ContactsApiTests
{
    [TestClass]
    public class IntegrationTests
    {
        public const string REST_URL = "http://localhost:53561";
        public const string TEST_CONTACT_NAME = "Test-Contact";

        [TestMethod]
        public  void TestContacts ()
        {
            // Ensure Test Record does not exist
            Assert.IsTrue(PurgeTestRecord(), "PurgeTestRecord unsuccessful");

            //// Query current database
            //int initialCount = contactsList.Count;
            //List<Contact> contactsList = null;

            //// CREATE: Create new contact
            //Assert.IsTrue(CreateTestContact(db), "CreateTestContact unsuccessful");

            //// READ: verify contact created
            //contactsList = await db.Contacts.ToListAsync();
            //int currentCount = contactsList.Count;
            ////Assert.Equals(currentCount, (initialCount + 1)); // FAILS
            //Assert.IsTrue(currentCount == (initialCount + 1));

            //var resultSet = db.Contacts.Where(c => c.Name == TEST_NAME);
            //currentCount = resultSet.Count();
            ////Assert.Equals(currentCount, 1);  // FAILS
            //Assert.IsTrue(currentCount == 1);

            //Contact contact = resultSet.First();
            //Assert.AreEqual(contact.EMail, TEST_NAME + "@xyz.com");
            //Assert.AreEqual(contact.Phone1, "999-888-7777");

            //// UPDATE: verify record modification
            //contact.Phone1 = "111-222-3333";
            //db.SaveChanges();
            //contactsList = await db.Contacts.ToListAsync();
            //resultSet = db.Contacts
            //             .Where(c => c.Name == TEST_NAME);
            //contact = resultSet.First();
            //currentCount = resultSet.Count();
            //Assert.IsTrue(currentCount == 1);
            //Assert.AreEqual(contact.Phone1, "111-222-3333");

            //// DELETE: verify record delection
            //db.Contacts.Remove(contact);
            //db.SaveChanges();

            //contactsList = await db.Contacts.ToListAsync();
            //currentCount = contactsList.Count;
            //Assert.IsTrue(currentCount == initialCount);

        }

        private bool PurgeTestRecord ()
        {
            return false;
        }
    }
}
