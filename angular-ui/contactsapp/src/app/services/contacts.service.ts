import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { retry, catchError } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Contact } from '../models/Contact';
import { Note } from '../models/Note';

@Injectable({
  providedIn: 'root'
})
export class ContactsService {

  myAppUrl: string;
  myApiUrl: string;
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json; charset=utf-8'
    })
  };

  constructor(private http: HttpClient) {
    //debugger;
    // EXAMPLE (environment.ts) appUrl: 'https://localhost:44362/'
    this.myAppUrl = environment.appUrl;
    this.myApiUrl = 'api/Contacts/';
  }

  createContact(contact: Contact): Observable<any> {
    const url = this.mkUrl();
    console.log('ContactsService.createContact(url=' + url + '):', contact);
    return this.http.post<Contact[]>(url, contact)
    .pipe(
      retry(1),
      catchError(this.errorHandler)
    );
  }

  deleteContact(contact: Contact): Observable<any> {
    const url = this.mkUrl() + contact.contactId;
    console.log('ContactsService.deleteContact(url=' + url + '):', contact);
    return this.http.delete<Contact[]>(url)
    .pipe(
      retry(1),
      catchError(this.errorHandler)
    );
  }

  getContact(contactId: number): Observable<Contact> {
      return this.http.get<Contact>(this.myAppUrl + this.myApiUrl +contactId)
      .pipe(
        retry(1),
        catchError(this.errorHandler)
      );
  }

  getContacts(): Observable<Contact[]> {
    const url = this.mkUrl();
    //const url = 'http://localhost:52774/api/Contacts/';  // CORS Error
    //const url = 'https://localhost:44362/api/Contacts/';  // Works if Startup.cs/app.AddCors() *above* MVC

    console.log('ContactsService.getContacts(url=' + url + ')...');
    return this.http.get<Contact[]>(url)
    .pipe(
      retry(1),
      catchError(this.errorHandler)
    );
  }

  updateContact(contact: Contact): Observable<Contact> {
    const url = this.mkUrl() + contact.contactId;
    console.log('ContactsService.updateContact(url=' + url + '):', contact);
    return this.http.put<Contact>(url, JSON.stringify(contact), this.httpOptions)
    .pipe(
      retry(1),
      catchError(this.errorHandler)
    );
  }

  errorHandler(error) {
    //debugger;
    let errorMessage = '';
    if (error.error instanceof ErrorEvent) {
      // Get client-side error
      errorMessage = error.error.message;
    } else {
      // Get server-side error
      errorMessage = `Error Code: ${error.status}\n>>Message: ${error.message}`;
    }
    console.error(errorMessage);
    return throwError(errorMessage);
  }

  mkUrl() : string {
   return this.myAppUrl + this.myApiUrl;
  }
}
