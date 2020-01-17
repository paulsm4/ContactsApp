package com.example.contactsapp.actions;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

import javax.servlet.http.HttpServletResponse;

import org.apache.struts2.ServletActionContext;

import com.example.contactsapp.models.Contact;
import com.example.contactsapp.repositories.ContactsRepository;
import com.example.contactsapp.repositories.ContactsRepositoryImpl;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.opensymphony.xwork2.Action;
import com.opensymphony.xwork2.ActionSupport;

public class ContactsAction extends ActionSupport {

	private static final long serialVersionUID = 1L;
	private String jsonString;
	private ContactsRepository contactsRepository = new ContactsRepositoryImpl();
	
	public String addContact () {
		return null;  // TBD
	}
	
	public String deleteContact () {
		return null;  // TBD
	}
	
	public String getContact (int id) {
		Contact contact = contactsRepository.getContact(id);
		return Action.SUCCESS;
	}
	
	public String getContacts () throws Exception {
		List<Contact> contacts = contactsRepository.getContactsFetchAll();
		ObjectMapper mapper = new ObjectMapper();
		jsonString = mapper.writeValueAsString(contacts);
		HttpServletResponse response = ServletActionContext.getResponse();
		response.setContentType("application/json");
	    response.getWriter().write(jsonString );
	    response.flushBuffer();
		return Action.SUCCESS;
	}

	public String updateContact () {
		return null;  // TBD
	}
	
	//public String getJsonString() { return jsonString; }
	//public void setJsonString(String s) { jsonString = s; }
	
}
