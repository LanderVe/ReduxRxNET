using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.State.Shape
{
  public class MainState
  {
    private readonly bool isPaneOpen;
    private readonly CurrentViewState currentView;

    public MainState(bool isPaneOpen, CurrentViewState currentView)
    {
      this.isPaneOpen = isPaneOpen;
      this.currentView = currentView;
    }

    public bool IsPaneOpen => isPaneOpen;
    public CurrentViewState CurrentView => currentView;
  }

  public class CurrentViewState
  {
    private readonly Type type;
    private readonly object arguments;

    public CurrentViewState(Type type, object arguments)
    {
      this.type = type;
      this.arguments = arguments;
    }

    public Type Type => type;
    public object Arguments => arguments;
  }
}
