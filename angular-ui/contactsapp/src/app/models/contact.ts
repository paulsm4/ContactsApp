import { Note } from './Note';

export class Contact {
  contactId?: number;
  name: string;
  eMail: string;
  phone1: string;
  phone2: string;
  address1: string;
  address2: string;
  city: string;
  state: string;
  zip: string;
  notes: Note[];
}
