using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JournalAPI.Data;
using JournalAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JournalAPI.Controllers
{
    [Route("api/patient/{patientId:int}/[controller]")]
    [ApiController]
    [Authorize]
    public class JournalController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public JournalController(ApplicationDbContext db)
        {
            _db = db;
        }
        
        // POST: api/Journal
        [HttpPost]
        [ProducesResponseType(201, Type=typeof(Journal))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult Post([FromRoute]int patientId, Journal journal)
        {
            var patientFromDb = _db.Patients.FirstOrDefault(x => x.Id == patientId);
            
            if (patientFromDb == null)
            {
                return NotFound();
            }
            else if (
                String.IsNullOrEmpty(journal.EntryBy) ||
                (journal.Date == DateTime.MinValue) ||
                String.IsNullOrEmpty(journal.Comment)) 
            {
                return BadRequest();
            }

            var journalToDb = new Journal
            {
                EntryBy = journal.EntryBy,
                Date = journal.Date,
                Comment = journal.Comment
            };
            journalToDb.PatientId = patientId;
            journalToDb.Patient = patientFromDb;

            _db.Journals.Add(journalToDb);
            _db.SaveChanges();
            
            patientFromDb.Journals.Add(journalToDb);
            _db.SaveChanges();

            return CreatedAtAction("Post", journalToDb);
        }

        
    }
}
