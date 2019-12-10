/**
 * NOTE:
 * TestAPIComponent uses HttpClientTestingModule (mocking; "standard" for unit testing).
 * ContactsService test, in contrast, uses HttpClientModule to invoke the "live" REST service.
 */
import { TestBed, inject } from '@angular/core/testing';
// import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { HttpClientModule, HttpClient } from '@angular/common/http';
import { ContactsService } from './contacts.service';

describe('ContactsService', () => {

  let service: ContactsService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientModule],
      providers: [ContactsService, HttpClient]
    });
    service = TestBed.get(ContactsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should retrieve all contacts', (done) => {
    expect(service).toBeTruthy();
    // Call the service
    service.getContacts().subscribe(data => {
      console.log('it(should be created)@getContacts(): live data=', data);
      expect(data).toBeTruthy();
      done();  // 'expect' was used when there was no current spec Jasmine error unless "done()"
    });
  });

});
