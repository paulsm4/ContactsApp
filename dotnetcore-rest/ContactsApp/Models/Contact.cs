using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContactsApp.Models
{
    public class Contact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContactId { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        //public List<Note> Notes { get; set; }  // POST new Contact + Notes OK, but fails otherwise
        public virtual ICollection<Note> Notes { get; set; }
    }
}
