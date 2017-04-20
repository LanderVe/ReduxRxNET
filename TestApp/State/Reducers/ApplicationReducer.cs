using ReduxRxNET.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApp.State.Shape;

namespace TestApp.State.Reducers
{
  class ApplicationReducer : Reducer<ApplicationState>
  {
    private DataReducer dataReducer = new DataReducer();
    private ListReducer listReducer = new ListReducer();
    private FavoritesReducer favoritesReducer = new FavoritesReducer();
    private MainReducer mainReducer = new MainReducer();

    public override ApplicationState Reduce(ApplicationState state = null, object action = null)
    {
      var newState = new ApplicationState(
        data: dataReducer.Reduce(state?.Data, action),
        list: listReducer.Reduce(state?.List, action),
        favorites: favoritesReducer.Reduce(state?.Favorites, action),
        main: mainReducer.Reduce(state?.Main, action)
      );

      var hasChanged = state == null
                       || state.Data != newState.Data
                       || state.List != newState.List
                       || state.Favorites != newState.Favorites
                       || state.Main != newState.Main;

      return hasChanged ? newState : state;
    }
  }
}
