import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ListContactsComponent } from './list-contacts.component';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ContactsService } from '../services/contacts.service';

describe('ListContactsComponent', () => {
  let component: ListContactsComponent;
  let fixture: ComponentFixture<ListContactsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ContactsService, HttpTestingController],
      declarations: [ ListContactsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ListContactsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
