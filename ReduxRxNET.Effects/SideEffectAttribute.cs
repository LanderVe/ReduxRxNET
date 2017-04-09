using System;
using System.Collections.Generic;
using System.Text;

namespace ReduxRxNET.SideEffects
{

  [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
  public class SideEffect : Attribute
  {
    readonly Type type;
    public Type Type => type;

    public SideEffect(Type positionalString)
    {
      this.type = positionalString;
    }

    public SideEffect() { }

  }
}
