import { TestBed, inject } from '@angular/core/testing';
//import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { HttpClientModule, HttpHandler } from '@angular/common/http';

import { ContactsService } from './contacts.service';

describe('ContactsService', () => {

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientModule],
      providers: [ContactsService]
    });
  });

  it('should be created', () => {
    //debugger;
    inject(
      [HttpClientModule, ContactsService],
      (httpClient: HttpClientModule, service: ContactsService) => {
        console.log('it(should be created)@inject...');
        expect(service).toBeTruthy();

        // Call the service
        service.getContacts().subscribe(data => {
          console.log('it(should be created)@service.getContacts().subscribe()...');
          console.log('data:', data);
          expect(data).toBeTruthy();
        });

        // Set expectations

        // Set data to be returned

      });
      expect("xyz").toBeTruthy();
    });

  it('should retrieve all contacts', () => {
    const contactsService: ContactsService = TestBed.get(ContactsService);
    let observable = contactsService.getContacts();
    expect(observable).toBeTruthy();
    //debugger;
    observable.subscribe(data => {
      debugger;
      console.log("Done");
    },
    error => {
      //debugger;
      console.error("observable error");
    });
  });

});
