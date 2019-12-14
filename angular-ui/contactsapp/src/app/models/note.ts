export class Note {
  noteId?: number;
  text: string;
  date: Date;
  contactId?: number;
  public constructor(text: string) { this.text = text; }
}
