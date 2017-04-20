using ReduxRxNET.SideEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApp.Services;
using TestApp.State.Actions;

namespace TestApp.State.Effects
{
  class ListSideEffects
  {
    private LocalSearchService searchService;

    public ListSideEffects(LocalSearchService searchService)
    {
      this.searchService = searchService;
    }

    /// <summary>
    /// runs on background thread, could take long
    /// </summary>
    /// <param name="actions"></param>
    /// <returns></returns>
    [SideEffect]
    public IObservable<object> LoadContacts(IObservable<object> actions)
    {
      return actions
       .OfType<SearchListAction>()
       .SubscribeOn(TaskPoolScheduler.Default) //potential long synchrnous task
       .Select<SearchListAction, object>(action =>
       {
         try
         {
           var contactIds = searchService.SearchContacts(action.Term);
           return new SearchListSuccessAction(contactIds);
         }
         catch
         {
           return new SearchListFailAction();
         }
       });
    }

  }
}
