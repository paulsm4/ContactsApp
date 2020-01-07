package com.example.contactsapp.repositories;

import java.util.ArrayList;
import java.util.List;

import javax.persistence.EntityManager;
import javax.persistence.EntityManagerFactory;
import javax.persistence.Persistence;
import javax.persistence.Query;

import com.example.contactsapp.models.Contact;
import com.example.contactsapp.models.Note;

public class ContactsRepositoryImpl implements ContactsRepository {
	private static final String PERSISTENCE_UNIT = "ContactsApp_JPA";
	private EntityManagerFactory emf;

	public ContactsRepositoryImpl() {
		emf = Persistence.createEntityManagerFactory(PERSISTENCE_UNIT);
	}

	public List<Contact> getContacts() {
		EntityManager em = null;
		List<Contact> contacts = null;
		try {
			em = emf.createEntityManager();
			em.getTransaction().begin();
			Query query = em.createQuery("FROM Contact");
			contacts = (List<Contact>) query.getResultList();
			em.getTransaction().commit();
			return contacts;
		} finally {
			if (em != null)
				em.close();
		}
	}

	public Contact getContact(int contactId) {
		EntityManager em = null;
		try {
			em = emf.createEntityManager();
			em.getTransaction().begin();
			Contact contact = em.find(Contact.class, contactId);
			em.getTransaction().commit();
			return contact;
		} finally {
			if (em != null)
				em.close();
		}
	}

	public int addContact(Contact contact) {
		EntityManager em = null;
		try {
			em = emf.createEntityManager();
			em.getTransaction().begin();
			em.persist(contact);
			em.getTransaction().commit();
			return contact.getContactId();
		} finally {
			if (em != null)
				em.close();
		}
	}

	public void deleteContact(int contactId) {
		EntityManager em = null;
		try {
			em = emf.createEntityManager();
			em.getTransaction().begin();
			Contact contact = em.find(Contact.class, contactId);
			em.remove(contact);
			em.getTransaction().commit();
		} finally {
			if (em != null)
				em.close();
		}
	}

	public void updateContact(Contact contact) {
		EntityManager em = null;
		try {
			em = emf.createEntityManager();
			em.getTransaction().begin();
			Contact dbRecord = em.find(Contact.class, contact.getContactId());
			dbRecord.setName(contact.getName());
			dbRecord.setEmail(contact.getEmail());
			dbRecord.setAddress1(contact.getAddress1());
			dbRecord.setAddress2(contact.getAddress2());
			dbRecord.setCity(contact.getCity());
			dbRecord.setState(contact.getState());
			dbRecord.setZip(contact.getZip());
			dbRecord.setPhone1(contact.getPhone1());
			dbRecord.setPhone2(contact.getPhone2());
			//dbRecord.setNotes(contact.getNotes());  // TBD: accomodate child elements
			em.getTransaction().commit();
		} finally {
			if (em != null)
				em.close();
		}
	}

	/**
	 * Test driver
	 */
	public static void main(String[] args) {
		try {
			// Connect to database
			ContactsRepository contactsRepository = new ContactsRepositoryImpl();

			// Query records
			List<Contact> contacts = contactsRepository.getContacts();
			System.out.println("getContacts: ct=" + contacts.size());

			// Add new record
			Contact newContact = new Contact();
			newContact.setName("Mickey Mouse");
			newContact.setEmail("mm@abc.com");
			int newId = contactsRepository.addContact(newContact);
			System.out.println("addContact(), new contactId=" + newId);
			contacts = contactsRepository.getContacts();
			System.out.println("getContacts: ct=" + contacts.size());

			// Update record
			newContact.setCity("Emerald City");
			newContact.setState("Oz");
			newContact.setZip("00000"); 
			/*
			 * // ... Mitigate "contact.notes == null". // TBD:
			 * come up with a better solution... List<Note> updatedNotes =
			 * newContact.getNotes(); if (updatedNotes == null) updatedNotes = new
			 * ArrayList<Note>(); updatedNotes.add(new Note("New Note"));
			 * newContact.setNotes(updatedNotes);
			 */			
			contactsRepository.updateContact(newContact);

			// Fetch record
			Contact updatedContact = contactsRepository.getContact(newId);
			System.out.println("getContact(" + newId + "): " + updatedContact.toString());

			// Delete record
			contactsRepository.deleteContact(newId);
			contacts = contactsRepository.getContacts();
			System.out.println("getContacts: ct=" + contacts.size());
			for (Contact contact: contacts) {
				System.out.println(contact.toString());
			}

		} catch (Exception e) {
			System.out.println("ERROR: " + e.getMessage());
			e.printStackTrace();
		}
	}
}
