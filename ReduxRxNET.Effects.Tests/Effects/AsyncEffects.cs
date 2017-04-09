using ReduxRxNET.SideEffects.Tests.Reducers;
using System;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;
using System.Reactive.Threading.Tasks;

namespace ReduxRxNET.SideEffects.Tests.Effects
{
  public static class AsyncEffects
  {
    //[SideEffect(typeof(AsyncReducer.LoadAction))]
    //internal static IObservable<object> OnLoadSuccess(IObservable<object> actions)
    //{
    //  return actions
    //   .Select(loadAction => GetDataAsync(shouldFail: false))
    //   .Switch()
    //   .Select<IEnumerable<int>, object>(data => new AsyncReducer.SuccessAction(data))
    //   .Catch(Observable.Return(new AsyncReducer.FailAction()));
    //}

    [SideEffect(typeof(AsyncReducer.LoadAction))]
    internal static IObservable<object> OnLoadSuccess(IObservable<object> actions)
    {
      return actions
       .Select(loadAction => GetDataAsync(shouldFail: false).ToObservable()
         .Select<IEnumerable<int>, object>(data => new AsyncReducer.SuccessAction(data))
         .Catch(Observable.Return(new AsyncReducer.FailAction())))
       .Switch();
    }

    //[SideEffect(typeof(AsyncReducer.LoadAction))]
    internal static IObservable<object> OnLoadFail(IObservable<object> actions)
    {
      return actions
       .Select(loadAction => GetDataAsync(shouldFail: true))
       .Switch()
       .Select<IEnumerable<int>, object>(data => new AsyncReducer.SuccessAction(data))
       .Catch(Observable.Return(new AsyncReducer.FailAction()));
      //.Catch<object, Exception>( ex => Observable.Return(new AsyncReducer.FailAction()) );
    }

    private static async Task<IEnumerable<int>> GetDataAsync(bool shouldFail)
    {
      if (shouldFail)
      {
        throw new Exception("GetDataAsync failed as requested");
      }

      await Task.Delay(TimeSpan.FromMilliseconds(100));
      return new List<int> { 1, 8, 2 };
    }

    internal class AsyncReducerApplicationStateComparer : IComparer
    {
      public int Compare(AsyncReducer.ApplicationState x, AsyncReducer.ApplicationState y)
      {
        if (x.Loading != y.Loading)
          return -1;

        if (!Enumerable.SequenceEqual(x.Data, y.Data))
          return -1;

        return 0;
      }

      public int Compare(object first, object second)
      {
        var x = first as AsyncReducer.ApplicationState;
        var y = second as AsyncReducer.ApplicationState;

        if (x == null || y == null)
          return -1;

        if (x.Loading != y.Loading)
          return -1;

        if (!Enumerable.SequenceEqual(x.Data, y.Data))
          return -1;

        return 0;
      }
    }

  }
}
