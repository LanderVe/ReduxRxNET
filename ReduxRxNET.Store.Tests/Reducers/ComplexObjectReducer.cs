using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace ReduxRxNET.Store.Tests.Reducers
{
  class ComplexObjectReducer : Reducer<ComplexObjectReducer.ApplicationState>
  {
    public static readonly ApplicationState initialState = new ApplicationState(
      ui: new UIState(isVisible: false),
      data: new DataState(
         entities1: ImmutableSortedDictionary<int, Entitiy1>.Empty,
         entities2: ImmutableSortedDictionary<int, Entitiy2>.Empty
      )
    );

    public override ApplicationState Reduce(ApplicationState state = null, object action = null)
    {
      if (state == null)
        return initialState;

      var toggleVisibilityAction = action as ToggleVisibilityAction;
      if (toggleVisibilityAction != null)
      {
        return new ApplicationState(
          ui: new UIState(isVisible: !state.UI.IsVisible),
          data: state.Data
        );
      }

      var addEntity1Action = action as AddEntity1Action;
      if (addEntity1Action != null)
      {
        var newEntity1 = new Entitiy1(addEntity1Action.Id, addEntity1Action.Value);
        return new ApplicationState(
            ui: state.UI,
            data: new DataState(
              entities1: state.Data.Entities1.Add(addEntity1Action.Id, newEntity1),
              entities2: state.Data.Entities2
            )
          );
      }

      var addEntity2Action = action as AddEntity2Action;
      if (addEntity2Action != null)
      {
        var newEntity2 = new Entitiy2(addEntity2Action.Id, addEntity2Action.Value);
        return new ApplicationState(
            ui: state.UI,
            data: new DataState(
              entities1: state.Data.Entities1,
              entities2: state.Data.Entities2.Add(addEntity2Action.Id, newEntity2)
            )
          );
      }

      return state;
    }

    //actions
    internal class ToggleVisibilityAction
    {
    }

    internal class AddEntity1Action
    {
      private readonly int id;
      public int Id => id;
      private readonly string value;
      public string Value => value;

      public AddEntity1Action(int id, string value)
      {
        this.id = id;
        this.value = value;
      }
    }

    /// <summary>
    /// payload is Enetity2
    /// </summary>
    internal class AddEntity2Action
    {
      private readonly int id;
      public int Id => id;
      private readonly string value;
      public string Value => value;

      public AddEntity2Action(int id, string value)
      {
        this.id = id;
        this.value = value;
      }
    }

    //object
    internal class ApplicationState
    {
      public UIState UI { get; private set; }
      public DataState Data { get; private set; }

      public ApplicationState(UIState ui, DataState data)
      {
        UI = ui;
        Data = data;
      }
    }

    internal class UIState
    {
      public bool IsVisible { get; private set; }

      public UIState(bool isVisible)
      {
        IsVisible = isVisible;
      }
    }

    internal class DataState
    {
      public ImmutableSortedDictionary<int, Entitiy1> Entities1 { get; set; }
      public ImmutableSortedDictionary<int, Entitiy2> Entities2 { get; set; }

      public DataState(ImmutableSortedDictionary<int, Entitiy1> entities1, ImmutableSortedDictionary<int, Entitiy2> entities2)
      {
        Entities1 = entities1;
        Entities2 = entities2;
      }
    }

    internal class Entitiy1
    {
      public int Id { get; private set; }
      public string Value { get; private set; }

      public Entitiy1(int id, string value)
      {
        Id = id;
        Value = value;
      }
    }

    internal class Entitiy2
    {
      public int Id { get; private set; }
      public string Value { get; private set; }

      public Entitiy2(int id, string value)
      {
        Id = id;
        Value = value;
      }
    }

  }
}
