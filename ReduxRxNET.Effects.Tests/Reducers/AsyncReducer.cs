using ReduxRxNET.Store;
using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Text;

namespace ReduxRxNET.SideEffects.Tests.Reducers
{
  class AsyncReducer : Reducer<AsyncReducer.ApplicationState>
  {
    private static readonly ApplicationState initialValue = new ApplicationState(
        loading: false,
        data: ImmutableList<int>.Empty
      );

    public override ApplicationState Reduce(ApplicationState state = null, object action = null)
    {
      if (state == null)
      {
        state = initialValue;
      }

      if (action is LoadAction)
      {
        return new ApplicationState(
          loading: true,
          data: state.Data
        );
      }

      var successAction = action as SuccessAction;
      if (successAction != null)
      {
        return new ApplicationState(
          loading: false,
          data: successAction.Data
        );
      }

      if (action is LoadAction)
      {
        return new ApplicationState(
          loading: false,
          data: ImmutableList<int>.Empty
        );
      }

      return state;
    }

    //actions
    internal class LoadAction { }
    internal class SuccessAction {
      private readonly ImmutableList<int> data;
      public ImmutableList<int> Data => data;

      public SuccessAction(IEnumerable<int> data)
      {
        this.data = ImmutableList.CreateRange(data);
      }
    }
    internal class FailAction { }

    //state
    internal class ApplicationState
    {
      private readonly ImmutableList<int> data;
      public ImmutableList<int> Data => data;
      private readonly bool loading;
      public bool Loading => loading;

      public ApplicationState(ImmutableList<int> data, bool loading)
      {
        this.loading = loading;
        this.data = data;
      }
    }

  }
}
