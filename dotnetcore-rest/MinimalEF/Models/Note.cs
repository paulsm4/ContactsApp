using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MinimalEF.Models
{
    public class Note
    {
        [Key]
        public int ID { get; set; }
        public Note()
        {
            this.Date = DateTime.Now; // Default value: local "now"
        }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public int ContactID { get; set; }
    }
}
