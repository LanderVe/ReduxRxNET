using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ReduxRxNET.Store
{
  public class Store<TApplicationState>
  {
    private Subject<object> dispatcher;
    private IConnectableObservable<TApplicationState> storeStateStream;
    private object dispatchLock = new object();

    public Store(Reducer<TApplicationState> reducer, TApplicationState initialState = default(TApplicationState), params Interceptor<TApplicationState>[] interceptors)
    {
      dispatcher = new Subject<object>();

      storeStateStream = GetStateObservable(reducer, initialState);
      storeStateStream.Connect();

      //for initialState, might be set in reducer
      Dispatch<object>(null);

      foreach (var interceptor in interceptors)
      {
        interceptor.Initialize(this, dispatcher);
      }
    }

    public void Dispatch<TAction>(TAction action)
    {
      lock (dispatchLock)
      {
        dispatcher.OnNext(action);
      }
    }

    private IConnectableObservable<TApplicationState> GetStateObservable(Reducer<TApplicationState> reducer, TApplicationState initialState)
    {
      return dispatcher
      .Scan(initialState, (state, action) => reducer.Reduce(state, action))
      .Replay(1);
    }

    public IObservable<TSlice> Select<TSlice>(Func<TApplicationState, TSlice> selector)
    {
      return storeStateStream.Select(state => selector(state)).DistinctUntilChanged();
    }

    public IObservable<TApplicationState> GetState()
    {
      return storeStateStream.DistinctUntilChanged();
    }

    public async Task<TApplicationState> GetStateSnapshotAsync()
    {
      return await storeStateStream.FirstAsync();
    }


  }
}
