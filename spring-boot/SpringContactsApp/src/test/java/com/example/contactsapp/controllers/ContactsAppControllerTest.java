package com.example.contactsapp.controllers;

import static org.junit.jupiter.api.Assertions.*;

import org.junit.jupiter.api.BeforeAll;
import org.junit.jupiter.api.Test;
import org.junit.runner.RunWith;
import org.springframework.boot.test.autoconfigure.web.servlet.WebMvcTest;
import org.springframework.boot.test.mock.mockito.MockBean;
import org.springframework.test.context.junit4.SpringRunner;

@RunWith(SpringRunner.class)
@WebMvcTest(ContactsAppController.class)
class ContactsAppControllerTest {

	@MockBean
	private ContactsAppController contactsAppController;
	
	@BeforeAll
	public static void setup () {
		; // TBD
	}

	@Test
	void test() {
		fail("Not yet implemented");
	}
	
}
