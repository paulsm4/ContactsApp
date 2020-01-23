package com.example.contactsapp.controllers;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.RestController;

import com.example.contactsapp.repositories.ContactsAppRepository;

@RestController
public class ContactsAppController {
	
	private final ContactsAppRepository contactsAppRepository;
	
	@Autowired
	public ContactsAppController(final ContactsAppRepository contactsAppRepository) {
		this.contactsAppRepository = contactsAppRepository;
	}

}
