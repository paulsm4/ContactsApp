package com.example.contactsapp.actions;

import java.util.HashMap;
import java.util.Map;

import com.opensymphony.xwork2.Action;
import com.opensymphony.xwork2.ActionSupport;

public class ContactsAction extends ActionSupport {

	private static final long serialVersionUID = 1L;
	private Map<String, String> jsonString;
	
	public String getContacts () {
		jsonString = new HashMap<String, String>();
		jsonString.put("status", "getContacts was successful!");
		jsonString.put("data", "Dummy Contact");
		
		return Action.SUCCESS;
	}
	
	public Map<String, String> getJsonString() { return jsonString; }
	public void setJsonString(Map<String, String> m) { jsonString = m; }
	
}
