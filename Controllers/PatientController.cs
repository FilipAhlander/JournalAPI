using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JournalAPI.Data;
using JournalAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JournalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PatientController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public PatientController(ApplicationDbContext db)
        {
            _db = db;
        }


        // GET: api/Patient/5
        [HttpGet("{id}", Name = "Get")] // TODO lägga till include på journaler
        [ProducesResponseType(200, Type=typeof(Patient))]
        [ProducesResponseType(404)]
        public IActionResult Get(int id)
        {
            var patient = _db.Patients.Include(x => x.Journals)
                .FirstOrDefault(x => x.Id == id);

            if (patient == null)
            {
                return NotFound();
            }
            return Ok(patient);
        }

        // POST: api/Patient
        [HttpPost]
        [ProducesResponseType(201, Type=typeof(Patient))]
        [ProducesResponseType(400)]
        public IActionResult Post(Patient patient)
        {

            if (
                String.IsNullOrEmpty(patient.FirstName) ||
                String.IsNullOrEmpty(patient.LastName) ||
                String.IsNullOrEmpty(patient.SocialSecurityNumber))
            {
                return BadRequest("Cant leave fields empty");
            }
            else if(_db.Patients.Any(x => x.SocialSecurityNumber.ToUpper() == patient.SocialSecurityNumber.ToUpper()))
            {
                return BadRequest("Patient med personnr finns redan");
            }

            var patientToDb = new Patient
            {
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                SocialSecurityNumber = patient.SocialSecurityNumber
            };
            _db.Patients.Add(patientToDb);
            _db.SaveChanges();
            return CreatedAtAction("Post", patientToDb);
        }

        // PUT: api/Patient/5
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public IActionResult Put(int id, Patient patient)
        {
            var patientFromDb = _db.Patients.FirstOrDefault(x => x.Id == id);
            if(patientFromDb == null)
            {
                return NotFound();
            }
            else if(
                String.IsNullOrEmpty(patient.FirstName) ||
                String.IsNullOrEmpty(patient.LastName) ||
                String.IsNullOrEmpty(patient.SocialSecurityNumber))
            {
                return BadRequest("INGA TOMMA FÄLT");
            }
            patientFromDb.FirstName = patient.FirstName;
            patientFromDb.LastName = patient.LastName;
            patientFromDb.SocialSecurityNumber = patient.SocialSecurityNumber;
            _db.SaveChanges();
            return NoContent();
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult Delete(int id)
        {
            var patientFromDb = _db.Patients.FirstOrDefault(x => x.Id == id);
            if (patientFromDb == null)
            {
                return NotFound();
            }
            _db.Patients.Remove(patientFromDb);
            try
            {
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
            
            return NoContent();
        }
    }
}
