//using ReduxRxNET.Store;
//using System;
//using System.Collections.Generic;
//using System.Reactive.Subjects;
//using System.Reactive.Linq;
//using System.Reflection;
//using System.Text;
//using System.Linq;

//namespace ReduxRxNET.Effects
//{
//  public class SideEffectsManager<TApplicationState> : Interceptor<TApplicationState>
//  {
//    private Store<TApplicationState> store;
//    private IObservable<object> actions;
//    private HashSet<SideEffectAction> effectActions = new HashSet<SideEffectAction>();
//    private List<IObservable<object>> effectStreams = new List<IObservable<object>>();

//    public override void Initialize(Store<TApplicationState> store, IObservable<object> dispatcher)
//    {
//      this.store = store;
//      this.actions = dispatcher;
//    }

//    public void AddEffectsClass(params Type[] types)
//    {
//      var effects = ExtractEffectsFromType(types);
//      foreach (var effect in effects)
//      {
//        var notInSetYet = this.effectActions.Add(effect);
//        if (notInSetYet)
//        {
//          var effectStream = effect(actions);
//          effectStreams.Add(effectStream);
//        }
//      }
//    }

//    private List<SideEffectAction> ExtractEffectsFromType(Type[] types)
//    {
//      var methods = from type in types
//                    from m in type.GetTypeInfo().DeclaredMethods
//                    where m.GetCustomAttributes<SideEffect>().Any()
//                    select m.CreateDelegate(typeof(SideEffectAction)) as SideEffectAction;
//      return methods.ToList();
//    }

//    public void Start()
//    {
//      //actions are the ones caused by side effects
//      Observable.Merge(effectStreams).Subscribe(action => store.Dispatch(action));
//    }
//  }
//}

using ReduxRxNET.Store;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Linq;

namespace ReduxRxNET.SideEffects
{
  public class SideEffectsManager<TApplicationState> : Interceptor<TApplicationState>
  {
    private Store<TApplicationState> store;
    private IObservable<object> actions;
    private HashSet<DelegateContainer> effectActions = new HashSet<DelegateContainer>();
    private List<IObservable<object>> effectStreams = new List<IObservable<object>>();

    public override void Initialize(Store<TApplicationState> store, IObservable<object> dispatcher)
    {
      this.store = store;
      this.actions = dispatcher;
    }

    public void AddEffectsClass(params object[] input)
    {
      if (actions == null)
      {
        throw new Exception($"{nameof(AddEffectsClass)} should be called after the store has been created");
      }

      var ofTypeMethod = typeof(Observable).GetTypeInfo().GetDeclaredMethod("OfType");

      var effects = ExtractEffectsFromType(input);
      foreach (var effect in effects)
      {
        var notInSetYet = this.effectActions.Add(effect);
        if (notInSetYet)
        {
          IObservable<object> effectStream;

          // [SideEffect(typeof(MyType))], if a type is specified use Observable.OfType<MyType>() to filter the list of actions
          if (effect.Type != null)
          {
            var ofType = ofTypeMethod.MakeGenericMethod(effect.Type);
            effectStream = effect.Delegate((IObservable<object>)ofType.Invoke(null, new object[] { actions }));
          }
          // [SideEffect], send all actions
          else
          {
            effectStream = effect.Delegate(actions);
          }
          if (effectStream != null)
          {
            effectStreams.Add(effectStream);
          }
        }
      }
    }

    private List<DelegateContainer> ExtractEffectsFromType(object[] inputs)
    {
      var methods = from input in inputs
                    from m in input.GetType().GetTypeInfo().DeclaredMethods
                    from a in m.GetCustomAttributes<SideEffect>()
                    select new DelegateContainer
                    {
                      Type = a.Type,
                      Delegate = m.CreateDelegate(typeof(SideEffectAction), input) as SideEffectAction //turn into delegate, faster than reflection's Invoke
                    };
      return methods.ToList();

    }

    public void Start()
    {
      if (actions == null)
      {
        throw new Exception($"{nameof(Start)} should be called after the store has been created");
      }

      //actions are the ones caused by side effects
      Observable.Merge(effectStreams).Subscribe(action => store.Dispatch(action));
    }

    class DelegateContainer
    {
      public Type Type { get; set; }
      public SideEffectAction Delegate { get; set; }
    }

  }
}
