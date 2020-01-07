package com.example.contactsapp.controllers;

import org.apache.struts2.rest.DefaultHttpHeaders;
import org.apache.struts2.rest.HttpHeaders;

import com.example.contactsapp.models.Contact;
import com.example.contactsapp.repositories.ContactsRepository;
import com.example.contactsapp.repositories.ContactsRepositoryImpl;
import com.opensymphony.xwork2.ModelDriven;

/**
 * Default URL mappings (https://struts.apache.org/plugins/rest/):
 * - index:   GET request with no id parameter.
 * - show:    GET request with an id parameter.
 * - create:  POST request with no id parameter and JSON/XML body
 * - update:  PUT request with an id parameter and JSON/XML body. 
 * - destroy: DELETE request with an id parameter. 
 * - edit:    GET  request with an id parameter and the edit view specified. 
 * - editNew: GET  request with no id parameter and the new view specified.
 */
public class ContactsController implements ModelDriven<Object> {

	private static final long serialVersionUID = 1L;
	private String id;
	private Object model;
	private ContactsRepository contactsRepository = new ContactsRepositoryImpl();

	@Override
	public Object getModel() {
		return model;
	}
	
    public HttpHeaders index () {
        model = contactsRepository.getContacts();
        return new DefaultHttpHeaders("index").disableCaching();
    }
    
    public HttpHeaders show() {
    	int contactId = Integer.parseInt(id);
        model = (Object)contactsRepository.getContact(contactId);
        return new DefaultHttpHeaders("show");
    }
    
    public HttpHeaders create() {
    	Contact contact = (Contact)model;
    	contactsRepository.addContact(contact);
        return new DefaultHttpHeaders("success");
    }

    public HttpHeaders update() {
    	Contact contact = (Contact)model;
    	contactsRepository.addContact(contact);
        return new DefaultHttpHeaders("success");
    }

    public HttpHeaders destroy() {
    	int contactId = Integer.parseInt(id);
        contactsRepository.deleteContact(contactId);
        return new DefaultHttpHeaders("success");
    }



}
