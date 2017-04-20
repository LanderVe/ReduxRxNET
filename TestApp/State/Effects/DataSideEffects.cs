using ReduxRxNET.SideEffects;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApp.Models;
using TestApp.Services;
using TestApp.State.Actions;

namespace TestApp.State.Effects
{
  class DataSideEffects
  {
    private ContactsApiClient client;

    public DataSideEffects(ContactsApiClient client)
    {
      this.client = client;
    }

    [SideEffect]
    public IObservable<object> LoadContacts(IObservable<object> actions)
    {
      return actions
       .OfType<LoadContactsAction>()
       .Select(loadAction => client.GetAllContacts()
         .Select(contacts => (object)new LoadContactsSuccessAction(contacts))
         .Catch(Observable.Return(new LoadContactsFailAction())))
       .Switch();
    }

    [SideEffect]
    public IObservable<object> SaveNewContact(IObservable<object> actions)
    {
      return actions
       .OfType<SaveNewContactAction>()
       .Select(newAction => client.PostContact(newAction.Contact)
         .Select(contact => (object)new SaveNewContactSuccessAction(contact))
         .Catch(Observable.Return(new SaveNewContactFailAction())))
       .Concat();
    }

    [SideEffect]
    public IObservable<object> UpdateContact(IObservable<object> actions)
    {
      return actions
       .OfType<UpdateContactAction>()
       .Select(updateAction => client.PutContact(updateAction.Contact)
         .Select(contact => (object)new UpdateContactSuccessAction(contact))
         .Catch(Observable.Return(new UpdateContactFailAction())))
       .Concat();
    }

    [SideEffect]
    public IObservable<object> DeleteContact(IObservable<object> actions)
    {
      return actions
       .OfType<DeleteContactAction>()
       .Select(deleteAction => client.DeleteContact(deleteAction.ContactId)
         .Select(id => (object)new DeleteContactSuccessAction(id))
         .Catch(Observable.Return(new DeleteContactFailAction())))
       .Concat();
    }

  }
}
