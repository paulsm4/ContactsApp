package com.example.contactsapp.models;

import java.io.Serializable;
import java.text.SimpleDateFormat;
import java.util.Date;

public class Note implements Serializable {
	private static final long serialVersionUID = 1L;

	private int noteId;
    private String text;
    private Date date;
    private int contactId;
    
	public Note () {
		this.date = new Date();
	}

	public Note (String text) {
		this.date = new Date();
		this.text = text;
	}

	@Override
    public String toString() {
		SimpleDateFormat sdf = new SimpleDateFormat ("MM/DD/yy HH:mm:ss");
		String s = "id: " + noteId + ", date: " + sdf.format(date) + ": " + text;
		return s;
	}
	
    /*
     * getters/setters
     */	
	public int getNoteId() {
		return noteId;
	}

	public void setNoteId(int noteId) {
		this.noteId = noteId;
	}

	public String getText() {
		return text;
	}

	public void setText(String text) {
		this.text = text;
	}

	public Date getDate() {
		return date;
	}

	public void setDate(Date date) {
		this.date = date;
	}

	public int getContactId() {
		return contactId;
	}

	public void setContactId(int contactId) {
		this.contactId = contactId;
	}
	
}
