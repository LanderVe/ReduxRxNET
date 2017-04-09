using ReduxRxNET.SideEffects.Tests.Reducers;
using System;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace ReduxRxNET.SideEffects.Tests.Effects
{
  public static class IncrementEffects
  {
    //[SideEffect(typeof(IncrementReducer.IncrementAction))]
    [SideEffect]
    internal static IObservable<object> OnIncrement(IObservable<object> actions)
    {
      return Observable.Merge(
        actions
        .OfType<IncrementReducer.IncrementAction>()
        .SelectMany(incrAction => Observable.Return(new IncrementReducer.DecrementAction()))
      );
    }

  }
}
