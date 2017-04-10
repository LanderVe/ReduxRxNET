﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApp.Models;

namespace TestApp.State.Actions
{
  internal class SearchListAction { }
  internal class SearchListSuccessAction
  {
    private readonly ImmutableList<int> data;

    public SearchListSuccessAction(ImmutableList<int> data)
    {
      this.data = data;
    }
    public ImmutableList<int> Data => data;
  }
  internal class SearchListFailAction { }

  internal class ListSelectItemAction
  {
    private readonly int selectedId;
    private readonly bool isNew;

    public ListSelectItemAction(int selectedId, bool isNew)
    {
      this.selectedId = selectedId;
      this.isNew = isNew;
    }
    public int SelectedId => selectedId;
    public bool IsNew => isNew;
  }
}
