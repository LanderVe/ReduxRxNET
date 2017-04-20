using System;
using ReduxRxNET.Store;
using TestApp.State.Shape;
using System.Collections.Immutable;
using TestApp.Models;
using TestApp.State.Actions;

namespace TestApp.State.Reducers
{
  public class DataReducer : Reducer<DataState>
  {
    private static readonly DataState initialValue = new DataState(
        isLoading: false,
        contacts: ImmutableSortedDictionary<int, Contact>.Empty
      );
    public override DataState Reduce(DataState state = null, object action = null)
    {
      if (state == null)
      {
        state = initialValue;
      }

      #region LoadContacts
      if (action is LoadContactsAction)
      {
        return new DataState(
          isLoading: true,
          contacts: state.Contacts
        );
      }

      var successAction = action as LoadContactsSuccessAction;
      if (successAction != null)
      {
        return new DataState(
          isLoading: false,
          contacts: successAction.Data.ToImmutableSortedDictionary(c => c.Id, c => c)
        );
      }

      if (action is LoadContactsFailAction)
      {
        return new DataState(
          isLoading: false,
          contacts: ImmutableSortedDictionary<int, Contact>.Empty
        );
      }
      #endregion

      var saveSuccessAction = action as SaveNewContactSuccessAction;
      if (saveSuccessAction != null)
      {
        return new DataState(
          isLoading: false,
          contacts: state.Contacts.Add(saveSuccessAction.Contact.Id, saveSuccessAction.Contact)
        );
      }

      var updateSuccessAction = action as SaveNewContactSuccessAction;
      if (updateSuccessAction != null)
      {
        var updatedContacts = state.Contacts.Remove(updateSuccessAction.Contact.Id);
        updatedContacts = updatedContacts.Add(updateSuccessAction.Contact.Id, updateSuccessAction.Contact);
        return new DataState(
          isLoading: false,
          contacts: updatedContacts
        );
      }

      var deleteSuccessAction = action as SaveNewContactSuccessAction;
      if (deleteSuccessAction != null)
      {
        return new DataState(
          isLoading: false,
          contacts: state.Contacts.Remove(deleteSuccessAction.Contact.Id)
        );
      }

      return state;
    }

  }
}