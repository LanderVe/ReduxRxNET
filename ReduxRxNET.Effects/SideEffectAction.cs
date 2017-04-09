using System;
using System.Collections.Generic;
using System.Text;

namespace ReduxRxNET.SideEffects
{
  public delegate IObservable<object> SideEffectAction(IObservable<object> actions);
  //public delegate IObservable<object> SideEffectAction<in S>(IObservable<S> actions);
}
