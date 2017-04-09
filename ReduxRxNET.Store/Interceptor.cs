using System;
using System.Collections.Generic;
using System.Text;

namespace ReduxRxNET.Store
{
  public abstract class Interceptor<TApplicationState>
  {
    public abstract void Initialize(Store<TApplicationState> store, IObservable<object> dispatcher);
  }
}
