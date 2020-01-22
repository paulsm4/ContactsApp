package com.example.contactsapp.controllers;

import java.util.Collection;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

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

    private static final Logger log = LogManager.getLogger(ContactsController.class);
    private String id;
    private Contact model = new Contact();
    private Collection<Contact> list;
    private ContactsRepository contactsRepository = new ContactsRepositoryImpl();

    // Returns single "Contact" (model) or Collection<Contact> (list)
    @Override
    public Object getModel() {
        return (list != null ? list : model);
    }
    
    // "Id" managed by struts-rest-plugin runtime (extracted from URI)
    public String getId () {
        return id;
    }
    
    public void setId(String id) {
        if (id != null) {
            int contactId = Integer.parseInt(id);
            this.model = contactsRepository.getContact(contactId);
        }
        this.id = id;
    }
    
    // EX: GET http://localhost:8080/StrutsContactsApp/contacts.json
    public HttpHeaders index () {
        log.debug("Reading all contacts...");
        list = contactsRepository.getContactsFetchAll();
        return new DefaultHttpHeaders("index").disableCaching();
    }
    
    // EX: GET http://localhost:8080/StrutsContactsApp/contacts/1.json (fetch contactId=1)
    public HttpHeaders show() {
        log.debug("Reading contact(" + id + ")...");
        int contactId = Integer.parseInt(id);
        model = (Contact)contactsRepository.getContact(contactId);
        return new DefaultHttpHeaders("show");
    }
    
    // EX: POST http://localhost:8080/StrutsContactsApp/contacts.json
    public HttpHeaders create() {
        log.debug("Creating new contact...", model);
        contactsRepository.addContact(model);
        return new DefaultHttpHeaders("show");
    }

    // EX: PUT http://localhost:8080/StrutsContactsApp/contacts/65.json (update contactId=65)
    public String update() {
        log.debug("Updating existing contact(" + id + ")...", model);
        contactsRepository.updateContact(model);
        return "update";
    }
//
//    // GET /orders/1/deleteConfirm
//    public String deleteConfirm() {
//      log.debug("Confirming delete(" + id + ")...");
//        return "deleteConfirm";
//    }

    // EX: DELETE http://localhost:8080/StrutsContactsApp/contacts/33.json (delete contactId=33)
    public HttpHeaders destroy() {
        log.debug("Deleting contact(" + id + ")...");
        int contactId = Integer.parseInt(id);
        contactsRepository.deleteContact(contactId);
        return new DefaultHttpHeaders("sucshowcess");
    }

}
