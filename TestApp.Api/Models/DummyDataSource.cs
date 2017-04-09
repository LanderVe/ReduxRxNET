using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestApp.Api.Models
{
  public static class DummyDataSource
  {
    public static List<Contact> Contacts { get; } = new List<Contact> {
      new Contact {Id=1, FirstName="Chuck", LastName="Norris", IsFavorite = true },
      new Contact {Id=2, FirstName="Arnold", LastName="Schwarzenegger", IsFavorite = true },
      new Contact {Id=3, FirstName="Steven", LastName="Segal", IsFavorite = false },
      new Contact {Id=4, FirstName="Chuckie", LastName="Norrissy", IsFavorite = false }
    };
  }
}
