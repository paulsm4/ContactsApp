﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ContactsApp.Models
{
    public class Note
    {
        public Note ()
        {
            this.Date = DateTime.Now; // Default value: local "now"
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NoteId { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        [ForeignKey("Contact")]
        public int ContactId { get; set; }
        public virtual Contact Contact { get; set; }
    }

 }