/**
 * NOTE:
 * TestAPIComponent uses HttpClientTestingModule (mocking; "standard" for unit testing).
 * ContactsService test, in contrast, uses HttpClientModule to invoke the "live" REST service.
 */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import { TestAPIComponent } from './test-api.component';
import { ContactsService } from '../services/contacts.service';

describe('TestAPIComponent', () => {
  let component: TestAPIComponent;
  let fixture: ComponentFixture<TestAPIComponent>;

  beforeEach(async(() => {
    console.log('TestAPIComponent@beforeEach(async)')
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ContactsService, HttpTestingController],
      declarations: [ TestAPIComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    console.log('TestAPIComponent@beforeEach()')
    fixture = TestBed.createComponent(TestAPIComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    console.log('TestAPIComponent@it(should create)');
    expect(component).toBeTruthy();
  });
});
