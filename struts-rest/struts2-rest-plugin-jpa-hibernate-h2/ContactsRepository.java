package com.example.contactsapp.repositories;

import java.util.List;

import com.example.contactsapp.models.Contact;

public interface ContactsRepository {
	public List<Contact> getContacts();
	public Contact getContact(int id);
	public int addContact(Contact contact);
	public void deleteContact(int id);
	public void updateContact(Contact contact);
}
