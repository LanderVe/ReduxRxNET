using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReduxRxNET.Store;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Linq;
using ReduxRxNET.Store.Tests.Reducers;
using System.Threading.Tasks;
using System.Collections.Immutable;

namespace ReduxRxNet.Store.Tests
{
  [TestClass]
  public class StoreTests
  {
    #region GetState
    [TestMethod]
    public void GetState_GetsFirstValue()
    {
      //Arrange
      var initialValue = 5;
      var results = new List<int>();
      var store = new Store<int>(new IncrementReducer(), initialValue);

      //Act
      var sub = store.GetState().Subscribe(val => results.Add(val));

      //Assert
      CollectionAssert.AreEqual(new List<int> { initialValue }, results);

      sub.Dispose();
    }
    #endregion

    #region GetStateSnapshotAsync
    [TestMethod]
    public async Task GetStateSnapshotAsync_BeforeDispatch()
    {
      //Arrange
      var initialValue = 5;
      var results = new List<int>();
      var store = new Store<int>(new IncrementReducer(), initialValue);

      //Act
      var state = await store.GetStateSnapshotAsync();

      //Assert
      Assert.AreEqual(initialValue, state);
    }

    [TestMethod]
    public async Task GetStateSnapshotAsync_AfterDispatch()
    {
      //Arrange
      var initialValue = 5;
      var results = new List<int>();
      var store = new Store<int>(new IncrementReducer(), initialValue);

      //Act
      store.Dispatch(new IncrementReducer.IncrementAction());
      var state = await store.GetStateSnapshotAsync();

      //Assert
      Assert.AreEqual(initialValue + 1, state);
    }
    #endregion

    #region Dispatch
    [TestMethod]
    public void Dispatch_ReceivesNewValue()
    {
      //Arrange
      var initialValue = 5;
      var results = new List<int>();
      var store = new Store<int>(new IncrementReducer(), initialValue);

      var sub = store.GetState().Subscribe(val => results.Add(val));

      //Act
      store.Dispatch(new IncrementReducer.IncrementAction());


      //Assert
      CollectionAssert.AreEqual(new List<int> { initialValue, initialValue + 1 }, results);  //5, 6

      sub.Dispose();
    }

    [TestMethod]
    public void Dispatch_NonExistingAction_ReturnsUnchanged()
    {
      //Arrange
      //using IncrementObjectReducer instead of int to check the reference of the object rather than the value
      var initialValue = new IncrementObjectReducer.Counter(5);
      var results = new List<IncrementObjectReducer.Counter>();
      var store = new Store<IncrementObjectReducer.Counter>(new IncrementObjectReducer(), initialValue);

      var sub = store.GetState().Subscribe(val => results.Add(val));

      //Act
      store.Dispatch(new IncrementReducer.NonExistingAction());


      //Assert
      CollectionAssert.AreEqual(results, new List<IncrementObjectReducer.Counter> { initialValue });  //5 
      Assert.IsTrue(Object.ReferenceEquals(initialValue, results[0]));

      sub.Dispose();
    }

    [TestMethod]
    public void Dispatch_ReceivesValueAfterSubscription_WhenFirstSub()
    {
      //Arrange
      var initialValue = 5;
      var results = new List<int>();
      var store = new Store<int>(new IncrementReducer(), initialValue);

      //Act
      store.Dispatch(new IncrementReducer.IncrementAction());

      //Subscribe after act
      var sub = store.GetState().Subscribe(val => results.Add(val));

      //Assert
      CollectionAssert.AreEqual(new List<int> { initialValue + 1 }, results);

      sub.Dispose();
    }

    [TestMethod]
    public void Dispatch_ReceivesValueAfterSubscription_WhenNotFirstSub()
    {
      //Arrange
      var initialValue = 5;
      var results1 = new List<int>();
      var results2 = new List<int>();
      var store = new Store<int>(new IncrementReducer(), initialValue);

      //Act
      //sub1 before dispatch
      var sub1 = store.GetState().Subscribe(val => results1.Add(val));

      store.Dispatch(new IncrementReducer.IncrementAction());

      //sub2 after dispatch
      var sub2 = store.GetState().Subscribe(val => results2.Add(val));


      //Assert
      CollectionAssert.AreEqual(new List<int> { initialValue, initialValue + 1 }, results1);  // 5, 6
      CollectionAssert.AreEqual(new List<int> { initialValue + 1 }, results2);               // 6

      sub1.Dispose();
      sub2.Dispose();
    }

    [TestMethod]
    public void Dispatch_Multiple_ReceivesAll()
    {
      //Arrange
      var initialValue = 5;
      var numberOfCalls = 3;
      var results = new List<int>();
      var store = new Store<int>(new IncrementReducer(), initialValue);

      var sub = store.GetState().Subscribe(val => results.Add(val));

      //Act
      for (int i = 0; i < numberOfCalls; i++)
      {
        store.Dispatch(new IncrementReducer.IncrementAction());

      }

      //Assert
      CollectionAssert.AreEqual(Enumerable.Range(initialValue, numberOfCalls + 1).ToList(), results);

      sub.Dispose();
    }

    [TestMethod]
    public void Dispatch_MultiThreaded_NoRace()
    {
      //Arrange
      var initialValue = 5;
      var results = new List<int>();
      var store = new Store<int>(new RaceSensitiveIncrementReducer(), initialValue);

      var sub = store.GetState().Subscribe(val => results.Add(val));

      var numberOfThreads = 3;
      var signal = new ManualResetEventSlim(false);
      var threads = new List<Thread>();

      //create threads
      for (int i = 0; i < numberOfThreads; i++)
      {
        var thread = new Thread(() =>
        {
          signal.Wait();
          store.Dispatch(new RaceSensitiveIncrementReducer.IncrementAction());
        });
        thread.Start();
        threads.Add(thread);
      }


      //Act
      signal.Set();

      //wait until all threads are done
      for (int i = 0; i < numberOfThreads; i++)
      {
        threads[i].Join();
      }

      //Assert
      CollectionAssert.AreEqual(Enumerable.Range(initialValue, numberOfThreads + 1).ToList(), results);

      sub.Dispose();
    }

    [TestMethod]
    public void Dispatch_ClassNoInitialValue_ReceivesFromReducer()
    {
      //Arrange
      var testId = 1;
      var testValue = "pinkey";
      var results = new List<ComplexObjectReducer.ApplicationState>();
      var store = new Store<ComplexObjectReducer.ApplicationState>(new ComplexObjectReducer());


      //Act
      var sub = store.GetState().Subscribe(val => results.Add(val));
      //make change in current slice
      store.Dispatch(new ComplexObjectReducer.AddEntity1Action(testId, testValue));


      //Assert
      // receives inital state, and new state
      Assert.IsTrue(results.Count == 2, $"results contains {results.Count} elements instead of 2");
      Assert.AreEqual(results[0], ComplexObjectReducer.initialState);
      Assert.AreNotEqual(results[1], ComplexObjectReducer.initialState);

      sub.Dispose();
    }

    [TestMethod]
    public void DispatchCombinedReducer_ClassNoInitialValue_ReceivesFromReducer()
    {
      //Arrange
      var testId = 1;
      var testValue = "pinkey";
      var results = new List<CombinedObjectReducer.ApplicationState>();
      var store = new Store<CombinedObjectReducer.ApplicationState>(new CombinedObjectReducer());


      //Act
      var sub = store.GetState().Subscribe(val => results.Add(val));
      //make change in current slice
      store.Dispatch(new CombinedObjectReducer.AddEntity1Action(testId, testValue));


      //Assert
      // receives inital state, and new state
      Assert.IsTrue(results.Count == 2, $"results contains {results.Count} elements instead of 2");
      Assert.AreEqual(results[0].UI, CombinedObjectReducer.UIReducer.initialState);
      Assert.AreEqual(results[0].Data, CombinedObjectReducer.DataReducer.initialState);
      Assert.AreEqual(results[1].UI, CombinedObjectReducer.UIReducer.initialState);
      Assert.AreNotEqual(results[1].Data, CombinedObjectReducer.DataReducer.initialState);

      sub.Dispose();
    }

    [TestMethod]
    public void Dispatch_StructNoInitialValue_ReceivesFromReducer()
    {
      //Arrange
      var results = new List<int>();
      var store = new Store<int>(new IncrementReducer());

      //Act
      var sub = store.GetState().Subscribe(val => results.Add(val));
      //make change in current slice
      store.Dispatch(new IncrementReducer.IncrementAction());

      //Assert
      // receives inital state, and new state
      Assert.IsTrue(results.Count == 2, $"results contains {results.Count} elements instead of 2");
      CollectionAssert.AreEqual(new List<int> { 0, 1 }, results);

      sub.Dispose();
    }
    #endregion

    #region Select
    [TestMethod]
    public void Select_DispatchOnCurrentSlice_ReceivesUpdate()
    {
      //Arrange
      var initialValue = new ComplexObjectReducer.ApplicationState(
        ui: new ComplexObjectReducer.UIState(isVisible: false),
        data: new ComplexObjectReducer.DataState(
           entities1: ImmutableSortedDictionary.Create<int, ComplexObjectReducer.Entitiy1>(),
           entities2: ImmutableSortedDictionary.Create<int, ComplexObjectReducer.Entitiy2>()
        )
      );
      var testId = 1;
      var testValue = "pinkey";
      var results = new List<ImmutableSortedDictionary<int, ComplexObjectReducer.Entitiy1>>();
      var store = new Store<ComplexObjectReducer.ApplicationState>(new ComplexObjectReducer(), initialValue);


      //Act
      var sub = store.Select(state => state.Data.Entities1).Subscribe(val => results.Add(val));
      //make change in current slice
      store.Dispatch(new ComplexObjectReducer.AddEntity1Action(testId, testValue));


      //Assert
      // receives inital state, and new state
      Assert.IsTrue(results.Count == 2, $"results contains {results.Count} elements instead of 2");
      Assert.IsFalse(object.ReferenceEquals(results[0], results[1]));
      Assert.IsTrue(results[1].Count == 1, $"entities1 contains {results[1].Count} elements instead of 1");

      var entity1 = results[1].First().Value;
      Assert.AreEqual(testId, entity1.Id);
      Assert.AreEqual(testValue, entity1.Value);

      sub.Dispose();
    }

    [TestMethod]
    public void Select_DispatchOnOtherSlice_NoUpdate()
    {
      //Arrange
      var initialValue = new ComplexObjectReducer.ApplicationState(
        ui: new ComplexObjectReducer.UIState(isVisible: false),
        data: new ComplexObjectReducer.DataState(
           entities1: ImmutableSortedDictionary.Create<int, ComplexObjectReducer.Entitiy1>(),
           entities2: ImmutableSortedDictionary.Create<int, ComplexObjectReducer.Entitiy2>()
        )
      );
      var testId = 1;
      var testValue = "pinkey";
      var results = new List<ImmutableSortedDictionary<int, ComplexObjectReducer.Entitiy1>>();
      var store = new Store<ComplexObjectReducer.ApplicationState>(new ComplexObjectReducer(), initialValue);


      //Act
      var sub = store.Select(state => state.Data.Entities1).Subscribe(val => results.Add(val));
      //make change in other slice
      store.Dispatch(new ComplexObjectReducer.AddEntity2Action(testId, testValue));


      //Assert
      // receives inital state, but no new state
      Assert.IsTrue(results.Count == 1, $"results contains {results.Count} elements instead of 1");

      sub.Dispose();
    }

    [TestMethod]
    public void SelectCombinedReducer_DispatchOnCurrentSlice_ReceivesUpdate()
    {
      //Arrange
      var initialValue = new CombinedObjectReducer.ApplicationState(
        ui: new CombinedObjectReducer.UIState(isVisible: false),
        data: new CombinedObjectReducer.DataState(
           entities1: ImmutableSortedDictionary.Create<int, CombinedObjectReducer.Entitiy1>(),
           entities2: ImmutableSortedDictionary.Create<int, CombinedObjectReducer.Entitiy2>()
        )
      );
      var testId = 1;
      var testValue = "pinkey";
      var results = new List<ImmutableSortedDictionary<int, CombinedObjectReducer.Entitiy1>>();
      var store = new Store<CombinedObjectReducer.ApplicationState>(new CombinedObjectReducer(), initialValue);


      //Act
      var sub = store.Select(state => state.Data.Entities1).Subscribe(val => results.Add(val));
      //make change in current slice
      store.Dispatch(new CombinedObjectReducer.AddEntity1Action(testId, testValue));


      //Assert
      // receives inital state, and new state
      Assert.IsTrue(results.Count == 2, $"results contains {results.Count} elements instead of 2");
      Assert.IsFalse(object.ReferenceEquals(results[0], results[1]));
      Assert.IsTrue(results[1].Count == 1, $"entities1 contains {results[1].Count} elements instead of 1");

      var entity1 = results[1].First().Value;
      Assert.AreEqual(testId, entity1.Id);
      Assert.AreEqual(testValue, entity1.Value);

      sub.Dispose();
    }

    [TestMethod]
    public void SelectCombinedReducer_DispatchOnOtherSlice_NoUpdate()
    {
      //Arrange
      var initialValue = new CombinedObjectReducer.ApplicationState(
        ui: new CombinedObjectReducer.UIState(isVisible: false),
        data: new CombinedObjectReducer.DataState(
           entities1: ImmutableSortedDictionary.Create<int, CombinedObjectReducer.Entitiy1>(),
           entities2: ImmutableSortedDictionary.Create<int, CombinedObjectReducer.Entitiy2>()
        )
      );
      var testId = 1;
      var testValue = "pinkey";
      var results = new List<ImmutableSortedDictionary<int, CombinedObjectReducer.Entitiy1>>();
      var store = new Store<CombinedObjectReducer.ApplicationState>(new CombinedObjectReducer(), initialValue);


      //Act
      var sub = store.Select(state => state.Data.Entities1).Subscribe(val => results.Add(val));
      //make change in other slice
      store.Dispatch(new CombinedObjectReducer.AddEntity2Action(testId, testValue));


      //Assert
      // receives inital state, but no new state
      Assert.IsTrue(results.Count == 1, $"results contains {results.Count} elements instead of 1");

      sub.Dispose();
    }
    #endregion

  }
}
