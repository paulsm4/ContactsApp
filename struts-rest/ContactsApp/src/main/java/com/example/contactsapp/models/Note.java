package com.example.contactsapp.models;

import java.text.SimpleDateFormat;
import java.util.Date;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.FetchType;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.JoinColumn;
import javax.persistence.ManyToOne;
import javax.persistence.Table;

import com.fasterxml.jackson.annotation.JsonBackReference;

@Entity
@Table(name = "notes")
public class Note {
    private int noteId;
    private String text;
    private Date date;
    private Contact contact;
    
    public Note () {
        this.date = new Date();
    }

    public Note (String text) {
        this.date = new Date();
        this.text = text;
    }

    @Override
    public String toString() {
        SimpleDateFormat sdf = new SimpleDateFormat ("MM/DD/yy HH:mm:ss.SSS");
        String s = "id: " + noteId + ", date: " + sdf.format(date) + ": " + text;
        return s;
    }
    
    /*
     * getters/setters
     */
    @Id
    @GeneratedValue(strategy=GenerationType.IDENTITY)
    @Column(unique=true, nullable=false)    
    public int getNoteId() { return noteId; }
    public void setNoteId(int noteId) { this.noteId = noteId; }
    
    public String getText() { return text; }
    public void setText(String text) { this.text = text; }
    
    public Date getDate() { return date; }
    public void setDate(Date date) { this.date = date; }
    
    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name="contactId", nullable=true)
    @JsonBackReference
    public Contact getContact() { return contact; }
    public void setContact(Contact contact) { this.contact = contact; }
}
