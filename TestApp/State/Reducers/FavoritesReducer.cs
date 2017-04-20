using System;
using ReduxRxNET.Store;
using TestApp.State.Shape;

namespace TestApp.State.Reducers
{
  internal class FavoritesReducer : Reducer<FavoritesState>
  {
    public override FavoritesState Reduce(FavoritesState state = null, object action = null)
    {
      throw new NotImplementedException();
    }
  }
}