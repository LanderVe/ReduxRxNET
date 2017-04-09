using System;
using System.Collections.Generic;
using System.Text;

namespace ReduxRxNET.Store
{
  public abstract class Reducer<TState>
  {
    public abstract TState Reduce(TState state = default(TState), object action = default(object));
  }
}
