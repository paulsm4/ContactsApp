package com.example.contactsapp.repositories;

import java.util.HashSet;
import java.util.List;

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
			List<Contact> contacts = session.createQuery("FROM Contact").list();
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
				// "save()" returns contatId immediately; persist() doesn't
				int id = (int)session.save(contact);
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
			contactsRepository = new ContactsRepositoryImpl();		

			// Fetch record (DEBUG)
			Contact tmpContact = contactsRepository.getContact(101);
			System.out.println("getContact(1): " + tmpContact.toString());

			// Query records 
			List<Contact> contacts = contactsRepository.getContacts();
			System.out.println("getContacts(): ct=" + contacts.size());
			
			// Add new record
			Contact newContact = new Contact();
			newContact.setName("Mickey Mouse"); newContact.setEmail("mm@abc.com");
			int	newId = contactsRepository.addContact(newContact);
			System.out.println("addContact(), new contactId=" + newId);
			contacts = contactsRepository.getContacts(); 
			System.out.println("getContacts(): ct=" + contacts.size());
			Contact c = contactsRepository.getContact(newId);
			System.out.println("getContact()@newId=" + c.toString());
			
			// Update record
			newContact.setCity("Emerald City");
			newContact.setState("Oz");
			newContact.setZip("00000");
			newContact.setNotes(new HashSet<Note>());
			newContact.getNotes().add(new Note("This is a test note"));
			contactsRepository.updateContact(newContact);
			System.out.println("updateContact(): added City, State, Zip and test note...");

			// Fetch record
			Contact updatedContact = contactsRepository.getContact(newId);
			System.out.println("getContact(" + newId + "): " + updatedContact.toString());

			// Delete record
			int iret = contactsRepository.deleteContact(newId);
			System.out.println("deleteContact()="+ iret + "...");
			contacts = contactsRepository.getContacts();
			System.out.println("getContacts(): ct=" + contacts.size());

		} catch (Exception e) {
			System.out.println("ERROR: " + e.getMessage());
			e.printStackTrace();
		} finally {
			if (contactsRepository != null)
				((ContactsRepositoryImpl)contactsRepository).shutdown();
		}		
	}
}
