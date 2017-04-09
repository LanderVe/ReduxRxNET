using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestApp.Api.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestApp.Api.Controllers
{
  [Route("api/[controller]")]
  public class ContactsController : Controller
  {
    // GET: api/values
    [HttpGet]
    public IEnumerable<Contact> Get()
    {
      return DummyDataSource.Contacts;
    }

    // GET api/values/5
    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
      var contact = DummyDataSource.Contacts.SingleOrDefault(c => c.Id == id);
      if (contact == null)
      {
        return NotFound();
      }
      return Ok(contact);
    }

    // POST api/values
    [HttpPost]
    public IActionResult Post([FromBody]Contact newContact)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest();
      }

      newContact.Id = DummyDataSource.Contacts.Max(c => c.Id) + 1;
      DummyDataSource.Contacts.Add(newContact);

      return CreatedAtAction(actionName: nameof(Get), controllerName: "Contacts", routeValues: new { id = newContact.Id }, value: newContact);
    }

    // PUT api/values/5
    [HttpPut("{id:int}")]
    public IActionResult Put(int id, [FromBody]Contact updatedContact)
    {
      if (id != updatedContact.Id || !ModelState.IsValid)
      {
        return BadRequest();
      }

      var contact = DummyDataSource.Contacts.SingleOrDefault(c => c.Id == id);
      if (contact == null)
      {
        return NotFound();
      }

      //silly left-right code
      contact.FirstName = updatedContact.FirstName;
      contact.LastName = updatedContact.LastName;
      contact.IsFavorite = updatedContact.IsFavorite;

      return NoContent();
    }

    // DELETE api/values/5
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
      var deletedContact = DummyDataSource.Contacts.SingleOrDefault(c => c.Id == id);
      if (deletedContact == null)
      {
        return NotFound();
      }
      DummyDataSource.Contacts.Remove(deletedContact);
      return NoContent();
    }

  }
}
