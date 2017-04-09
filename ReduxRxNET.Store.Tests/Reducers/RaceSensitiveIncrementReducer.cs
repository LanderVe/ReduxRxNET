using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ReduxRxNET.Store.Tests.Reducers
{
  //reducer
  class RaceSensitiveIncrementReducer : Reducer<int>
  {
    public override int Reduce(int state, object action)
    {
      var incrementAction = action as IncrementAction;
      if (incrementAction != null)
      {
        var temp = state;
        Thread.Sleep(200);
        temp += 1;
        return temp;
      }

      var decrementAction = action as DecrementAction;
      if (decrementAction != null)
      {
        var temp = state;
        Thread.Sleep(200);
        temp -= 1;
        return temp;
      }

      return state;
    }


    //actions
    internal class IncrementAction { }
    internal class DecrementAction { }
    internal class NonExistingAction { }
  }
}