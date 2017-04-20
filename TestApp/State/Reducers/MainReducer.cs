using System;
using ReduxRxNET.Store;
using TestApp.State.Shape;

namespace TestApp.State.Reducers
{
  internal class MainReducer : Reducer<MainState>
  {
    public override MainState Reduce(MainState state = null, object action = null)
    {
      throw new NotImplementedException();
    }
  }
}