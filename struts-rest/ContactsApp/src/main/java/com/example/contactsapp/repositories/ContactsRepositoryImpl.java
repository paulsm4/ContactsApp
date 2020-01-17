package com.example.contactsapp.repositories;

import java.util.HashSet;
import java.util.List;
import java.util.Set;

import javax.persistence.Entity;

import org.hibernate.Hibernate;
import org.hibernate.Session;
import org.hibernate.Transaction;
import org.hibernate.query.Query;

import com.example.contactsapp.models.Contact;
import com.example.contactsapp.models.Note;
import com.example.contactsapp.util.HibernateUtil;

public class ContactsRepositoryImpl implements ContactsRepository {

	public void shutdown () {
		HibernateUtil.shutdown ();
	}
	
	@Override
	public List<Contact> getContacts() {
		//Note: Hibernate 5++ supports Java try-with-resource blocks
		try (Session session = HibernateUtil.openSession()) {
			// This will fetch all contacts... but "Notes" aren't accessible outside this session
			List<Contact> contacts = session.createQuery("FROM Contact").list();
			return contacts;
		}
	}

	@Override
	public List<Contact> getContactsFetchAll() {
		// Mitigate Hibernate "failed to lazily initialize a collection" runtime error
		try (Session session = HibernateUtil.openSession()) {
			// Jackson mapper.writeValueAsString() => "failed to lazily initialize a collection" 
			// List<Contact> contacts = session.createQuery("FROM Contact").list();
			
			// Plan A: Causes same "failed to lazily initialize a collection" runtime error
			// List<Contact> contacts = session.createQuery("FROM Contact").list();
			// Hibernate.initialize(contacts);
			
			// Plan B: Still no-go: returns [] empty set
			// List<Contact> contacts = session.createQuery("SELECT c FROM Contact c JOIN FETCH c.notes n").list();

			// Plan C: Same: "failed to lazily initialize a collection of role..."
			// Query query = session.createQuery("FROM Contact");
			// Hibernate.initialize(query);
			// List<Contact> contacts = query.list(); 
			
			// Plan D: Same: "ERROR: failed to lazily initialize a collection of role"
			// List<Contact> contacts = session.createQuery("FROM Contact").list();
			// for (Contact c : contacts) {
			//    Set<Note> n = c.getNotes();
			// }
			
			// Plan E: Same as Plan B: returns [] empty set...
			// REFERENCE:
			//   https://vladmihalcea.com/hibernate-facts-multi-level-fetching/
			// List<Contact> contacts = session.createQuery(
			// 	"SELECT c FROM Contact c " +
			// 	"JOIN FETCH c.notes n", Contact.class)
			// 	.list();
			
			// Plan F: Bypass HQL and just use SQL
			//   Error: java.lang.ClassCastException: class [Ljava.lang.Object; cannot be cast to class com.example.contactsapp.models.Contact
			// REFERENCE:
			//   https://www.journaldev.com/3422/hibernate-native-sql-query-example#hibernate-native-sql-entity-and-join
			//List<Contact> contacts = session.createSQLQuery(
			//	"select c.*, n.noteId, n.text, n.date from app.contacts c " +
			//	"left outer join app.notes n on c.contactId = n.contactId").list();
			
			// Plan 9 From Outer Space
			// a) "Notes" are accessible outside this session...
			// b) ... but this won't return any contacts that happen to have 0 notes
			List<Contact> contacts = session.createQuery("SELECT c FROM Contact c INNER JOIN FETCH c.notes").list();
			return contacts;
		}
	}
	
	@Override
	public Contact getContact(int id) {
		try (Session session = HibernateUtil.openSession()) {
			Contact contact = (Contact)session.get(Contact.class, id);
			// Mitigate "failed to lazily initialize a collection" error
			Hibernate.initialize(contact.getNotes());
			return contact;
		}
	}

	@Override
	public int addContact(Contact contact) {
		try (Session session = HibernateUtil.openSession()) {
			Transaction tx = session.beginTransaction(); 
			try {
				// "save()" returns contactId immediately; persist() doesn't
				int id = (int)session.save(contact);
				
				// Ensure every contact has at least one note (INNER JOIN FETCH)
				Note initialNote = new Note("Creating new contact");
				initialNote.setContact(contact);
				contact.getNotes().add(initialNote);
				session.save(initialNote);
				for (Note n : contact.getNotes()) {
					n.setContact(contact);
					session.save(n);
				}
				tx.commit();
				return id;
			} catch (Exception e) {
				tx.rollback();
				throw e;
			}
		}
	}

	@Override
	public int deleteContact(int id) {
		try (Session session = HibernateUtil.openSession()) {
			Transaction tx = session.beginTransaction(); 
			try {
				// Use HQL (vs. SQL)
				// OBSOLETE: query.setInteger(0, id) et al: deprecated since Hibernate 5.2
				String hql = "delete from Contact where contactId  = ?1";
				Query query = session.createQuery(hql)
						.setParameter(1,  id);
				int result = query.executeUpdate(); 
				tx.commit ();
				return result;
			} catch (Exception e) {
				tx.rollback();
				throw e;
			}
		}
	}

	@Override
	public void updateContact(Contact contact) {
		try (Session session = HibernateUtil.openSession()) {
			Transaction tx = session.beginTransaction(); 
			try {
				session.update(contact);
				for (Note n : contact.getNotes()) {
					if (n.getContact() == null) {
						n.setContact(contact);
					}
					session.saveOrUpdate(n);
				}
				tx.commit ();
			} catch (Exception e) {
				tx.rollback();
				throw e;
			}
		}
			
	}

	/**
	 * Scaffolding test driver
	 */
	public static void main (String[] args) {
		ContactsRepository contactsRepository = null;
		try {
			// Connect to database
			System.out.println(">> new ContactsRepositoryImpl()...");			
			contactsRepository = new ContactsRepositoryImpl();		

//			// Fetch record (DEBUG)
//			Contact tmpContact = contactsRepository.getContact(101);
//			System.out.println("getContact(1): " + tmpContact.toString());

			// Query records 
			System.out.println(">> contactsRepository.getContacts()...");			
			List<Contact> contacts = contactsRepository.getContacts();
			System.out.println("<< getContacts(): ct=" + contacts.size() + ".");
			
			// Add new record
			System.out.println(">> addContact()...");
			Contact newContact = new Contact();
			newContact.setName("Mickey Mouse"); newContact.setEmail("mm@abc.com");
			newContact.getNotes().add(new Note("This is a test note"));
			int	newId = contactsRepository.addContact(newContact);
			System.out.println("<< addContact(), new contactId=" + newId + ".");
			contacts = contactsRepository.getContacts(); 
			System.out.println("<< getContacts(): ct=" + contacts.size() + ".");
			
			// Fetch record
			System.out.println(">> getContact(" + newId + ")...");
			newContact = null;
			newContact = contactsRepository.getContact(newId);
			System.out.println("<< getContact()@newId=" + newContact.toString() + ".");

			// Update record
			System.out.println(">> updateContact(" + newId + ")...");
			newContact.setCity("Emerald City");
			newContact.setState("Oz");
			newContact.setZip("00000");
			contactsRepository.updateContact(newContact);
			System.out.println("<< updateContact(): added City, State, Zip and test note.");

			// FetchAll
			System.out.println(">> getContactsFetchAll()...");
			contacts = contactsRepository.getContactsFetchAll();
			System.out.println("<< getContactsFetchAll(): #/contacts=" + contacts.size() + ".");
			for (Contact c : contacts) {
				System.out.println("  contact: " + c.toString());
			}
			
			// Delete record
			System.out.println(">> deleteContact(" + newId + ")...");
			int iret = contactsRepository.deleteContact(newId);
			System.out.println("<< deleteContact(), iret="+ iret + ".");
			contacts = contactsRepository.getContacts();
			System.out.println("<< getContacts(): ct=" + contacts.size() + ".");

		} catch (Exception e) {
			System.out.println("ERROR: " + e.getMessage());
			e.printStackTrace();
		} finally {
			if (contactsRepository != null)
				((ContactsRepositoryImpl)contactsRepository).shutdown();
		}		
	}
}
