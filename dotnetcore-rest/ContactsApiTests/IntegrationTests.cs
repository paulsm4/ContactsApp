/**
 * IntegrationTests: Run against "live" REST service and ContactsDB
 * 
 * NOTES:
 * - Use "run.bat" to start REST service, then "MSVS > Test" to execute the test suite
 * - Experiments with different APIs: WebClient, HttpRequest (both available since .Net 1.0) and HttpClient (.Net 4.5++)
 */
using ContactsApp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ContactsApiTests
{
    [TestClass]
    public class IntegrationTests
    {
        //public const string REST_URL = "http://localhost:53561/api/Contacts";  // From MSVS: port 53561; from cmd-line: port 5000
        public const string REST_URL = "http://localhost:5000/api/Contacts";
        public const string TEST_CONTACT_NAME = "Test-Contact";

        [TestMethod]
        public  void TestContacts ()
        {
            // 
            // Query for all existing test records: GET  http://localhost:5000/api/Contacts
            //
            // EXPECTED RESPONSE: [] (if no records); else JSON array of contact records
            string jsonResponse = HttpGet(REST_URL);
            Assert.IsFalse(String.IsNullOrEmpty(jsonResponse), "Query ContactsDB failed");

            // Ensure Test Record does not exist
            Assert.IsTrue(PurgeTestContacts(), "PurgeTestContacts unsuccessful");

            // CREATE: Create new contact: POST  http://localhost:5000/api/Contacts
            string jsonText = "{" +
                "\"name\": \"" + TEST_CONTACT_NAME + "\"," +
                "\"eMail\": \"" + TEST_CONTACT_NAME + "@abc.com\"," +
                "\"phone1\": \"111-222-3333\"," +
                "\"address1\": \"2001 Country Lane\"," +
                "\"city\": \"Los Angeles\"," +
                "\"state\": \"CA\"," +
                "\"zip\": \"90021\"," +
                "\"Notes\":[" +
                "  {\"Text\": \"abc\"}," +
                "  {\"Text\": \"def\"}," +
                "  {\"Text\": \"ghi\"}]" +
                "}";
            Contact contact = CreateTestContact(jsonText);
            Assert.IsFalse(contact == null, "CreateTestContact unsuccessful");
            Assert.IsTrue(contact.ContactId > 0, "CreateTestContact: valid ContactId");
            Assert.AreEqual(TEST_CONTACT_NAME, contact.Name);
            Assert.AreEqual(TEST_CONTACT_NAME + "@abc.com", contact.EMail);
            Assert.AreEqual("2001 Country Lane", contact.Address1);
            Assert.AreEqual("Los Angeles", contact.City);
            Assert.AreEqual("CA", contact.State);
            Assert.AreEqual("90021", contact.Zip);
            Assert.AreEqual("111-222-3333", contact.Phone1);
            Assert.IsNotNull(contact.Notes);
            Assert.AreEqual(3, contact.Notes.Count);

            // READ: verify contact created: GET  http://localhost:5000/api/Contacts/1
            Contact tmpContact = GetContactById(contact.ContactId);
            Assert.IsNotNull(tmpContact, "Verify contact failed");
            Assert.AreEqual(tmpContact.ContactId, contact.ContactId);
            Assert.AreEqual(tmpContact.Name, contact.Name);
            Assert.AreEqual(tmpContact.EMail, contact.EMail);
            Assert.AreEqual(tmpContact.Phone1, contact.Phone1);
            Assert.AreEqual(tmpContact.Phone2, contact.Phone2);
            Assert.AreEqual(tmpContact.Address1, contact.Address1);
            Assert.AreEqual(tmpContact.Address2, contact.Address2);
            Assert.AreEqual(tmpContact.City, contact.City);
            Assert.AreEqual(tmpContact.State, contact.State);
            Assert.AreEqual(tmpContact.Zip, contact.Zip);
            Assert.IsNotNull(contact.Notes);
            Assert.AreEqual(3, contact.Notes.Count);

            // UPDATE: Change a field, Add some notes: PUT http://localhost:5000/api/Contacts/1

            // Change an attribute...
            contact.Phone2 = "999-999-9999";

            // Add some notes
            List<Note> notes = new List<Note>();
            Note note1 = new Note();
            note1.Text = "This is a new note";
            notes.Add(note1);
            Note note2 = new Note();
            note2.Text = "This is another new note";
            notes.Add(note2);
            contact.Notes = notes;

            // Make the change
            int httpStatus = UpdateTestContact(contact);
            Assert.AreEqual(httpStatus, 204);

            // Re-read record
            tmpContact = GetContactById(contact.ContactId);
            Assert.AreEqual("999-999-9999", tmpContact.Phone2);
            Assert.IsNotNull(tmpContact.Notes);
            Assert.IsTrue(tmpContact.Notes.Count == 2);

            // Add note by modifying record

            // Add note directly to valid ID

            // Add note to invalid ID

            // Modify valid note

            // Modify invalid note

            // Delete valid note

            // Delete invalid note

            // DELETE Contact http://localhost:5000/api/Contacts/8
            string url = REST_URL + "/" + contact.ContactId;
            httpStatus = HttpDelete(url);
            Assert.AreEqual(httpStatus, 200);

            // Re-read record
            tmpContact = GetContactById(contact.ContactId);
            Assert.IsNull(tmpContact, "Verifying deleted record failed");

        }

        /**
         * Create new test record
         */
        private Contact CreateTestContact(string jsonText)
        {
            Contact contact = null;

            // Create new test record
            string url = REST_URL;
            string jsonResponse = HttpPost(url, jsonText);

            // Convert to "Contact"
            contact = JsonConvert.DeserializeObject<Contact>(jsonResponse);
            return contact;
        }

        /**
         * Create new test record
         */
        private Contact CreateTestContactWithNotes()
        {
            Contact contact = new Contact
            {
                Name = TEST_CONTACT_NAME,
                EMail = TEST_CONTACT_NAME + "@xyz.com",
                Phone1 = "999-999-9999",
                Notes = new List<Note>
                {
                    new Note { Text = "abc" },
                    new Note { Text = "def" },
                    new Note { Text = "ghi" }
                }
            };

            // Create new test record
            string jsonText = JsonConvert.SerializeObject(contact);
            string url = REST_URL;
            string jsonResponse = HttpPost(url, jsonText);

            // Convert to "Contact"
            contact = JsonConvert.DeserializeObject<Contact>(jsonResponse);
            return contact;
        }

        /**
         * Query specific contact record
         */
        private Contact GetContactById(int id) {
            Contact contact = null;
            string url = REST_URL + "/" + id;
            string jsonResponse = HttpGet(url);
            if (!String.IsNullOrEmpty(jsonResponse))
            {
                contact = JsonConvert.DeserializeObject<Contact>(jsonResponse);
            }
            return contact;
        }

        /**
         * HTTP DELETE return HTTP status code
         */
        private int HttpDelete(string url)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage result = httpClient.DeleteAsync(url).Result;
                System.Diagnostics.Trace.WriteLine("HttpDelete(" + url + "): HTTP " + result.StatusCode);
                return (int)result.StatusCode;
            }
        }

        /**
         * HTTP GET request, return response string
         */
        private string HttpGet(string url)
        {
            string response = "";
            WebClient webClient = new WebClient();
            try
            {
                response = webClient.DownloadString(url);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("ERROR(HttpGet): " + ex.Message);
            }
            return response;
        }

        /**
         * HTTP POST request, return response string
         */
        private string HttpPost(string url, string jsonText)
        {
            string response = "";
            try
            {
                // Post request
                HttpWebRequest http = (HttpWebRequest)WebRequest.Create(new Uri(url));
                http.Accept = "application/json";
                http.ContentType = "application/json";
                http.Method = "POST";

                ASCIIEncoding encoding = new ASCIIEncoding();
                Byte[] bytes = encoding.GetBytes(jsonText);

                Stream newStream = http.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();

                // Read response
                HttpWebResponse httpResponse = (HttpWebResponse)http.GetResponse();
                Stream stream = httpResponse.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                response = sr.ReadToEnd();
                System.Diagnostics.Trace.WriteLine("HttpPost(" + url + "): HTTP " + httpResponse.StatusCode);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("ERROR(HttpPost): " + ex.Message);
            }
            return response;
        }

        /**
          * HTTP PUT return HTTP status code
          */
        private int HttpPut(string url, string jsonText)
        {
            try
            {
                // Post request
                HttpWebRequest http = (HttpWebRequest)WebRequest.Create(new Uri(url));
                http.Accept = "application/json";
                http.ContentType = "application/json";
                http.Method = "PUT";

                ASCIIEncoding encoding = new ASCIIEncoding();
                Byte[] bytes = encoding.GetBytes(jsonText);

                Stream newStream = http.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();

                // Read response
                HttpWebResponse httpResponse = (HttpWebResponse)http.GetResponse();
                System.Diagnostics.Trace.WriteLine("HttpPut(" + url + "): HTTP " + httpResponse.StatusCode);
                return (int)httpResponse.StatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("ERROR(HttpPut): " + ex.Message);
            }
            return -1;
        }

        /**
         * Find and delete any "test records"
         */
        private bool PurgeTestContacts()
        {
            // Query for any existing test records
            string url = REST_URL + "/?name=" + TEST_CONTACT_NAME;
            string jsonResponse = HttpGet(url);

            // Sanity check
            if (String.IsNullOrEmpty(jsonResponse))
                return false;

            // Valid JSON; empty list - we're good to go!
            if ("[]" == jsonResponse)
            {
                return true;
            }

            // Let's parse the list...
            Contact[] contactsList = JsonConvert.DeserializeObject<Contact[]>(jsonResponse);

            // ... and delete each object
            foreach (Contact contact in contactsList)
            {
                string deleteUrl = REST_URL + "/" + contact.ContactId;
                int httpStatus = HttpDelete(deleteUrl);
            }

            // Done
            return true;
        }

        /**
         * Create new test record
         */
        private int UpdateTestContact(Contact contact)
        {
            // Update existing test record
            string url = REST_URL + "/" + contact.ContactId;
            string jsonText = JsonConvert.SerializeObject(contact);
            int httpStatus = HttpPut(url, jsonText);
            return httpStatus;
        }

    }
}
