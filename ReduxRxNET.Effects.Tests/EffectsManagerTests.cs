using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReduxRxNET.SideEffects.Tests.Reducers;
using System.Collections.Generic;
using ReduxRxNET.Store;
using ReduxRxNET.SideEffects.Tests.Effects;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Concurrency;
using System.Diagnostics;
using System.Threading;

namespace ReduxRxNET.SideEffects.Tests
{
  [TestClass]
  public class EffectsManagerTests
  {
    [TestMethod]
    public void SideEffect_Increment_AutoDecrement()
    {
      //Arrange
      var initialValue = 5;
      var results = new List<int>();
      var effectsManager = new SideEffectsManager<int>();
      var store = new Store<int>(new IncrementReducer(), initialValue, effectsManager);

      effectsManager.AddEffectsClass(typeof(IncrementEffects));
      effectsManager.Start();

      var sub = store.GetState().Subscribe(val => results.Add(val));

      //Act
      store.Dispatch(new IncrementReducer.IncrementAction());

      //Assert
      CollectionAssert.AreEqual(new List<int> { initialValue, initialValue + 1, initialValue }, results);  //5, 6, 5

      sub.Dispose();
    }

    [TestMethod]
    public void SideEffect_Decrement_NoAutoDecrement()
    {
      //Arrange
      var initialValue = 5;
      var results = new List<int>();
      var effectsManager = new SideEffectsManager<int>();
      var store = new Store<int>(new IncrementReducer(), initialValue, effectsManager);

      effectsManager.AddEffectsClass(typeof(IncrementEffects));
      effectsManager.Start();

      var sub = store.GetState().Subscribe(val => results.Add(val));

      //Act
      store.Dispatch(new IncrementReducer.DecrementAction());

      //Assert
      CollectionAssert.AreEqual(new List<int> { initialValue, initialValue - 1 }, results);  //5, 4

      sub.Dispose();
    }

    [TestMethod]
    public async Task SideEffect_LoadAction_RaisesLoadSuccessAsync()
    {
      //Arrange
      var results = new List<AsyncReducer.ApplicationState>();
      var expected = new List<AsyncReducer.ApplicationState> {
        new AsyncReducer.ApplicationState(
          loading: false,
          data: ImmutableList<int>.Empty
        ),
        new AsyncReducer.ApplicationState(
          loading: true,
          data: ImmutableList<int>.Empty
        ),
        new AsyncReducer.ApplicationState(
          loading: false,
          data: ImmutableList.CreateRange<int>(new List<int> { 1, 8, 2 })
        )
      };

      var effectsManager = new SideEffectsManager<AsyncReducer.ApplicationState>();
      var store = new Store<AsyncReducer.ApplicationState>(new AsyncReducer(), null, effectsManager);

      effectsManager.AddEffectsClass(typeof(AsyncEffects));
      effectsManager.Start();

      //completes after 3 elements, so we can await it
      var sub = store.GetState().Subscribe(val => results.Add(val));

      var state1 = await store.GetStateSnapshotAsync();


      //Act
      store.Dispatch(new AsyncReducer.LoadAction());

      var state2 = await store.GetStateSnapshotAsync();

      // wait for the effect, the initial value has already passed, 2 remaining
      await store.GetState().Take(2).Timeout(TimeSpan.FromMilliseconds(300));

      var state3 = await store.GetStateSnapshotAsync();


      //Assert
      Assert.AreEqual(expected.Count, results.Count);
      var comparer = new AsyncEffects.AsyncReducerApplicationStateComparer();
      CollectionAssert.AreEqual(expected, results, comparer);
      Assert.IsTrue(comparer.Compare(state1, results[0]) == 0);
      Assert.IsTrue(comparer.Compare(state2, results[1]) == 0);
      Assert.IsTrue(comparer.Compare(state3, results[2]) == 0);

      sub.Dispose();
    }

    [TestMethod]
    public async Task Await_Subject_WithTake()
    {
      var subject = new Subject<int>();


      var task = Task.Run(async () =>
      {
        await Task.Delay(100);
        subject.OnNext(1);
        subject.OnNext(8);
        subject.OnNext(2);
        //no OnComplete
      });

      var result = await subject.Take(3); //await waits for the last method

      Assert.AreEqual(result, 2);
    }



  }
}
