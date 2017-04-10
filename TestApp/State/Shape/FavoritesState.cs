using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.State.Shape
{
  public class FavoritesState
  {
    private readonly ImmutableList<int> contactIds;
    public ImmutableList<int> ContactIds => contactIds;

    public FavoritesState(ImmutableList<int> contactIds)
    {
      this.contactIds = contactIds;
    }
  }
}
