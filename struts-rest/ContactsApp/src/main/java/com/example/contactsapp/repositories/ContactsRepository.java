package com.example.contactsapp.repositories;

import java.util.List;

import com.example.contactsapp.models.Contact;

public interface ContactsRepository {
    public List<Contact> getContacts();
    // Mitigate Hibernate "failed to lazily initialize a collection" runtime error
    public List<Contact> getContactsFetchAll();
    public Contact getContact(int id);
    public int addContact(Contact contact);
    public int deleteContact(int id);
    public void updateContact(Contact contact);
}
