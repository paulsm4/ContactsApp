package com.example.contactsapp.models;

import java.io.Serializable;
import java.text.SimpleDateFormat;
import java.util.HashSet;
import java.util.Set;

public class Contact implements Serializable {
	private static final long serialVersionUID = 1L;
	
	private int contactId;
    private String name;
    private String email;
    private String phone1;
    private String phone2;
    private String address1;
    private String address2;
    private String city;
    private String state;
    private String zip;
    private Set<Note> notes = new HashSet<Note>();
    
    @Override
    public String toString() {
    	StringBuilder sb = new StringBuilder ();
    	sb.append("id: " + contactId + "\n");
    	sb.append("name: " + name + "\n");
    	sb.append("email: " + email + "\n");
    	if (notes == null || notes.size() == 0) {
    		sb.append("notes: []");
    	} else {
    		sb.append("notes: [\n");
    		SimpleDateFormat sdf = new SimpleDateFormat ("MM/DD/yy HH:mm:ss.SSS");
    		for (Note n : notes) {
    			String sDate = sdf.format(n.getDate());
    			sb.append(sDate + ": " + n.getText());
  				sb.append("\n");
    		}
    		sb.append("]");
    	}
    	return sb.toString();
    }

    /*
     * getters/setters
     */    
	public int getContactId() {
		return contactId;
	}

	public void setContactId(int contactId) {
		this.contactId = contactId;
	}

	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}

	public String getEmail() {
		return email;
	}

	public void setEmail(String email) {
		this.email = email;
	}

	public String getPhone1() {
		return phone1;
	}

	public void setPhone1(String phone1) {
		this.phone1 = phone1;
	}

	public String getPhone2() {
		return phone2;
	}

	public void setPhone2(String phone2) {
		this.phone2 = phone2;
	}

	public String getAddress1() {
		return address1;
	}

	public void setAddress1(String address1) {
		this.address1 = address1;
	}

	public String getAddress2() {
		return address2;
	}

	public void setAddress2(String address2) {
		this.address2 = address2;
	}

	public String getCity() {
		return city;
	}

	public void setCity(String city) {
		this.city = city;
	}

	public String getState() {
		return state;
	}

	public void setState(String state) {
		this.state = state;
	}

	public String getZip() {
		return zip;
	}

	public void setZip(String zip) {
		this.zip = zip;
	}

	public Set<Note> getNotes() {
		return notes;
	}

	public void setNotes(Set<Note> notes) {
		this.notes = notes;
	}
    	
}
