using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MinimalEF.Models
{
    public class Contact
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public virtual ICollection<Note> Notes { get; set; }

        public override string ToString ()
        {
            string jsonText = JsonConvert.SerializeObject(this);
            return jsonText;
        }
    }
}
