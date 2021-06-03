using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JournalAPI.Models
{
    public class Journal
    {
        [Key]
        public int Id { get; set; }
        public string EntryBy { get; set; }
        public DateTime Date { get; set; }
        public string Comment { get; set; }
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
    }
}
