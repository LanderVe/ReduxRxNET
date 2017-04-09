using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ReduxRxNET.Store.Tests
{
  //[TestClass]
  public class PerformanceTests
  {
    private int iterations = 50000000;

    public PerformanceTests()
    {
    }

    [TestMethod]
    public void Compare_Type_NoMatch()
    {
      //Arrange
      IAction obj = new Dummy1Action(0);
      var counter = 0;
      var sw = new Stopwatch();

      //Act
      sw.Start();
      for (int i = 0; i < iterations; i++)
      {
        if (obj is Dummy2Action)
        {
          ++counter;
        }
      }
      sw.Stop();

      //Assert
      Trace.WriteLine($"Compare_Type_NoMatch: Elapsed={sw.Elapsed}");
      Assert.IsTrue(true);
    }

    [TestMethod]
    public void Compare_Type_Match()
    {
      //Arrange
      IAction obj = new Dummy1Action(0);
      var counter = 0;
      var sw = new Stopwatch();

      //Act
      sw.Start();
      for (int i = 0; i < iterations; i++)
      {
        if (obj is Dummy1Action)
        {
          ++counter;
        }
      }
      sw.Stop();

      //Assert
      Trace.WriteLine($"Compare_Type_Match: Elapsed={sw.Elapsed}");
      Assert.IsTrue(true);
    }

    [TestMethod]
    public void Compare_String_NoMatch()
    {
      //Arrange
      IAction obj = new Dummy1Action(0);
      var counter = 0;
      var sw = new Stopwatch();

      //Act
      sw.Start();
      for (int i = 0; i < iterations; i++)
      {
        if (obj.Type == "THIS_DOES_NOT_MATCH")
        {
          ++counter;
        }
      }
      sw.Stop();

      //Assert
      Trace.WriteLine($"Compare_String_NoMatch: Elapsed={sw.Elapsed}");
      Assert.IsTrue(true);
    }

    [TestMethod]
    public void Compare_String_Match()
    {
      //Arrange
      IAction obj = new Dummy1Action(0);
      var counter = 0;
      var sw = new Stopwatch();

      //Act
      sw.Start();
      for (int i = 0; i < iterations; i++)
      {
        if (obj.Type == nameof(Dummy1Action))
        {
          ++counter;
        }
      }
      sw.Stop();

      //Assert
      Trace.WriteLine($"Compare_String_Match: Elapsed={sw.Elapsed}");
      Assert.IsTrue(true);
    }

    [TestMethod]
    public void Compare_TypeWithAs()
    {
      //Arrange
      IAction obj = new Dummy1Action(0);
      var dummy = 0;
      var sw = new Stopwatch();

      //Act
      sw.Start();
      for (int i = 0; i < iterations; i++)
      {
        var c1 = obj as Dummy1Action;
        if (c1 != null)
        {
          dummy += c1.Value;
        }
      }
      sw.Stop();

      //Assert
      Trace.WriteLine($"Compare_TypeWithAs: Elapsed={sw.Elapsed}");
      Assert.IsTrue(true);
    }

    [TestMethod]
    public void Compare_TypeWithCast()
    {
      //Arrange
      IAction obj = new Dummy1Action(0);
      var counter = 0;
      var sw = new Stopwatch();

      //Act
      sw.Start();
      for (int i = 0; i < iterations; i++)
      {
        if (obj is Dummy1Action)
        {
          var c1 = (Dummy1Action)obj;
          counter += c1.Value;
        }
      }
      sw.Stop();

      //Assert
      Trace.WriteLine($"Compare_TypeWithCast: Elapsed={sw.Elapsed}");
      Assert.IsTrue(true);
    }

    [TestMethod]
    public void Compare_TypeWithGetType()
    {
      //Arrange
      IAction obj = new Dummy1Action(0);
      var counter = 0;
      var sw = new Stopwatch();

      //Act
      sw.Start();
      for (int i = 0; i < iterations; i++)
      {
        if (obj.GetType() == typeof(Dummy1Action))
        {
          var c1 = (Dummy1Action)obj;
          counter += c1.Value;
        }
      }
      sw.Stop();

      //Assert
      Trace.WriteLine($"Compare_TypeWithCast: Elapsed={sw.Elapsed}");
      Assert.IsTrue(true);
    }

    [TestMethod]
    public void Compare_StringWithCast()
    {
      //Arrange
      IAction obj = new Dummy1Action(0);
      var counter = 0;
      var sw = new Stopwatch();

      //Act
      sw.Start();
      for (int i = 0; i < iterations; i++)
      {
        if (obj.Type == nameof(Dummy1Action))
        {
          var c1 = (Dummy1Action)obj;
          counter += c1.Value;
        }
      }
      sw.Stop();

      //Assert
      Trace.WriteLine($"Compare_StringWithCast: Elapsed={sw.Elapsed}");
      Assert.IsTrue(true);
    }

    #region dummy types
    interface IAction
    {
      string Type { get; }
    }

    class Dummy1Action : IAction
    {
      public string Type => nameof(Dummy1Action);

      private readonly int value;
      public int Value => value;

      public Dummy1Action(int value)
      {
        this.value = value;
      }
    }

    class Dummy2Action : IAction
    {
      public string Type => nameof(Dummy2Action);


      private readonly int value;
      public int Value => value;

      public Dummy2Action(int value)
      {
        this.value = value;
      }
    }
    #endregion

  }
}
