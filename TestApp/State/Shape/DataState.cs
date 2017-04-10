using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApp.Models;

namespace TestApp.State.Shape
{
  public class DataState
  {
    private readonly bool isLoading;
    public bool IsLoading => isLoading;

    private readonly ImmutableSortedDictionary<int, Contact> contacts;
    public ImmutableSortedDictionary<int, Contact> Contacts => contacts;

    public DataState(bool isLoading, ImmutableSortedDictionary<int, Contact> contacts)
    {
      this.isLoading = isLoading;
      this.contacts = contacts;
    }
  }
}
