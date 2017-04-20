using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.State.Shape
{
  public class ApplicationState
  {
    private readonly DataState data;
    public DataState Data => data;

    private readonly ListState list;
    public ListState List => list;

    private readonly FavoritesState favorites;
    public FavoritesState Favorites => favorites;

    private readonly MainState main;
    public MainState Main => main;

    public ApplicationState(DataState data, ListState list, FavoritesState favorites, MainState main)
    {
      this.data = data;
      this.list = list;
      this.favorites = favorites;
      this.main = main;
    }
  }
}
