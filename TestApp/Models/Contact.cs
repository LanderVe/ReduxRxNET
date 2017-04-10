using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestApp.Models
{
  public class Contact
  {
    private readonly int id;
    public int Id => id;

    private readonly string firstName;
    public string FirstName => firstName;

    private readonly string lastName;
    public string LastName => lastName;

    private readonly bool isFavorite;
    public bool IsFavorite => isFavorite;

    public Contact(int id, string firstName, string lastName, bool isFavorite)
    {
      this.id = id;
      this.firstName = firstName;
      this.lastName = lastName;
      this.isFavorite = isFavorite;
    }


  }
}
