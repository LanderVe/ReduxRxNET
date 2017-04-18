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
using System.Threading;

namespace ReduxRxNET.SideEffects.Tests.Effects
{
  public class AsyncEffects
  {
    //[SideEffect(typeof(AsyncReducer.LoadAction))]
    //[SideEffect]
    //internal IObservable<object> OnLoadSuccess(IObservable<object> actions)
    //{
    //  return actions
    //   .OfType<AsyncReducer.LoadAction>()
    //   .Select(loadAction => GetDataAsync(loadAction.ShouldFail, loadAction.Flag).ToObservable()
    //     .Select<IEnumerable<int>, object>(data => new AsyncReducer.SuccessAction(data))
    //     .Catch(Observable.Return(new AsyncReducer.FailAction())))
    //   .Switch();
    //}

    //[SideEffect(typeof(AsyncReducer.LoadAction))]
    //[SideEffect]
    //internal IObservable<object> OnLoadSuccess(IObservable<object> actions)
    //{
    //  return actions
    //   .OfType<AsyncReducer.LoadAction>()
    //   .Select(loadAction => Observable.FromAsync(token => GetDataSupportsCancelAsync(loadAction.ShouldFail, loadAction.Flag, token))
    //     .Select<IEnumerable<int>, object>(data => new AsyncReducer.SuccessAction(data))
    //     .Catch(Observable.Return(new AsyncReducer.FailAction())))
    //   .Switch();
    //}

    //[SideEffect(typeof(AsyncReducer.LoadAction))]
    [SideEffect]
    internal IObservable<object> OnLoadSuccess(IObservable<object> actions)
    {
      return actions
       .OfType<AsyncReducer.LoadAction>()
       .Select(loadAction => GetDataSupportsCancel(loadAction.ShouldFail, loadAction.Flag)
         .Select<IEnumerable<int>, object>(data => new AsyncReducer.SuccessAction(data))
         .Catch(Observable.Return(new AsyncReducer.FailAction())))
       .Switch();
    }

    /// <summary>
    /// cannot be cancelled, switch will not emit the result if a new call is made, but the async task will still continue
    /// </summary>
    /// <param name="shouldFail"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    private async Task<IEnumerable<int>> GetDataAsync(bool shouldFail, int flag)
    {
      await Task.Delay(TimeSpan.FromMilliseconds(100));
      Trace.WriteLine($"async task {flag} done");
      if (shouldFail)
      {
        throw new Exception("GetDataAsync failed as requested");
      }
      return new List<int> { flag, 1, 8, 2 };
    }

    /// <summary>
    /// can be cancelled, switch will not emit the result if a new call is made, and the async task will be stopped
    /// </summary>
    /// <param name="shouldFail"></param>
    /// <param name="flag">indicates which call is going on</param>
    /// <param name="cancellationToken"></param>
    /// <returns>task that can be cancelled</returns>
    private async Task<IEnumerable<int>> GetDataSupportsCancelAsync(bool shouldFail, int flag, CancellationToken cancellationToken)
    {
      await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
      Trace.WriteLine($"async task {flag} done");
      if (shouldFail)
      {
        throw new Exception("GetDataAsync failed as requested");
      }
      return new List<int> { flag, 1, 8, 2 };
    }

    /// <summary>
    /// This one returns an observable directly
    /// </summary>
    /// <param name="shouldFail"></param>
    /// <param name="flag">indicates which call is going on</param>
    /// <returns>observable</returns>
    private IObservable<IEnumerable<int>> GetDataSupportsCancel(bool shouldFail, int flag)
    {
      return Observable.FromAsync(async token =>
      {
        await Task.Delay(TimeSpan.FromMilliseconds(100), token);
        Trace.WriteLine($"async task {flag} done");
        if (shouldFail)
        {
          throw new Exception("GetDataAsync failed as requested");
        }
        return new List<int> { flag, 1, 8, 2 };
      });
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
