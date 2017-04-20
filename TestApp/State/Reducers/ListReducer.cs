using System;
using ReduxRxNET.Store;
using TestApp.State.Shape;
using System.Collections.Immutable;
using TestApp.State.Actions;

namespace TestApp.State.Reducers
{
  public class ListReducer : Reducer<ListState>
  {
    private ListSearchReducer dataReducer = new ListSearchReducer();
    private ListSelectedItemReducer listReducer = new ListSelectedItemReducer();

    public override ListState Reduce(ListState state = null, object action = null)
    {
      var newState = new ListState(
        search: dataReducer.Reduce(state?.Search, action),
        selectedItem: listReducer.Reduce(state?.SelectedItem, action)
      );

      var hasChanged = state == null
                       || state.Search != newState.Search
                       || state.SelectedItem != newState.SelectedItem;

      return hasChanged ? newState : state;
    }
  }

  public class ListSearchReducer : Reducer<ListSearchState>
  {
    private static readonly ListSearchState initialValue = new ListSearchState(
        searchTerm: "",
        isSearching: false,
        contactIds: ImmutableList<int>.Empty
      );

    public override ListSearchState Reduce(ListSearchState state = null, object action = null)
    {
      if (state == null)
      {
        state = initialValue;
      }

      var searchAction = action as SearchListAction;
      if (searchAction != null)
      {
        return new ListSearchState(
                  searchTerm: searchAction.Term,
                  isSearching: true,
                  contactIds: ImmutableList<int>.Empty //clear previous result
                );
      }

      var successAction = action as SearchListSuccessAction;
      if (successAction != null)
      {
        return new ListSearchState(
                  searchTerm: state.SearchTerm,
                  isSearching: false,
                  contactIds: successAction.Data
                );
      }

      if (action is SearchListFailAction)
      {
        return new ListSearchState(
                  searchTerm: state.SearchTerm,
                  isSearching: false,
                  contactIds: ImmutableList<int>.Empty //clear previous result
                );
      }

      return state;
    }

  }

  public class ListSelectedItemReducer : Reducer<ListSelectedItemState>
  {
    private static readonly ListSelectedItemState initialValue = new ListSelectedItemState(
        selectedId: -1,
        isSelectedSaving: false,
        isSelectedNew: false
    );

    public override ListSelectedItemState Reduce(ListSelectedItemState state = null, object action = null)
    {
      if (state == null)
      {
        state = initialValue;
      }

      //select existing or new
      var selectAction = action as SelectListItemAction;
      if (selectAction != null)
      {
        return new ListSelectedItemState(
                  selectedId: selectAction.SelectedId,
                  isSelectedSaving: false,
                  isSelectedNew: selectAction.IsNew
                );
      }

      //start new, update, delete
      if (action is SaveNewContactAction || action is UpdateContactAction || action is DeleteContactAction)
      {
        return new ListSelectedItemState(
                  selectedId: state.SelectedId,
                  isSelectedSaving: true,
                  isSelectedNew: state.IsSelectedNew
                );
      }

      //success new, update, delete
      if (action is SaveNewContactSuccessAction || action is UpdateContactSuccessAction || action is DeleteContactSuccessAction)
      {
        return new ListSelectedItemState(
                  selectedId: state.SelectedId,
                  isSelectedSaving: false,
                  isSelectedNew: state.IsSelectedNew
                );
      }

      //fail new, update, delete
      if (action is SaveNewContactFailAction || action is UpdateContactFailAction || action is DeleteContactFailAction)
      {
        return new ListSelectedItemState(
                  selectedId: state.SelectedId,
                  isSelectedSaving: false,
                  isSelectedNew: state.IsSelectedNew,
                  error: GetErrorForType(action)
                );
      }


      return state;
    }

    private string GetErrorForType(object action)
    {
      if (action is SaveNewContactFailAction) return "could not add contact";
      if (action is UpdateContactFailAction) return "could not update contact";
      if (action is DeleteContactFailAction) return "could not delete contact";
      else return null;
    }

  }

}