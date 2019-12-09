import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
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
    this.myAppUrl = environment.appUrl;
    this.myApiUrl = 'api/Contacts/';
  }

  getContacts(): Observable<Contact[]> {
    //const url = this.myAppUrl + this.myApiUrl;
    //const url = 'http://localhost:52774/api/Contacts/';  // CORS Error
    //const url = 'https://localhost:44333/api/Contacts/';
    //const url = 'https://localhost:53561/api/Contacts/';  // net::ERR_SSL_PROTOCOL_ERROR
    const url = 'https://localhost:44362/api/Contacts/';  // net::ERR_CONNECTION_REFUSED

    console.log('ContactsService.getContacts(url=' + url + ')...');
    return this.http.get<Contact[]>(url)
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

  errorHandler(error) {
    let errorMessage = '';
    if (error.error instanceof ErrorEvent) {
      // Get client-side error
      errorMessage = error.error.message;
    } else {
      // Get server-side error
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    console.error(errorMessage);
    return throwError(errorMessage);
  }

}
