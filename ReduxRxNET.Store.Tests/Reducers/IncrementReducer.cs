using System;
using System.Collections.Generic;
using System.Text;

namespace ReduxRxNET.Store.Tests.Reducers
{
  //reducer
  class IncrementReducer : Reducer<int>
  {
    public override int Reduce(int state, object action)
    {
      var incrementAction = action as IncrementAction;
      if (incrementAction != null)
      {
        return ++state;
      }

      var decrementAction = action as DecrementAction;
      if (decrementAction != null)
      {
        return --state;
      }

      return state;
    }

    //actions
    internal class IncrementAction { }
    internal class DecrementAction { }
    internal class NonExistingAction { }

  }
}
