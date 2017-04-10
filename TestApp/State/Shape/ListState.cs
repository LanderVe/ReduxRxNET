﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.State.Shape
{
  public class ListState
  {
    private readonly ListSearchState search;
    private readonly ListSelectedItemState selectedItem;

    public ListState(ListSearchState search, ListSelectedItemState selectedItem)
    {
      this.search = search;
      this.selectedItem = selectedItem;
    }

    public ListSearchState Search => search;
    public ListSelectedItemState SelectedItem => selectedItem;
  }

  public class ListSearchState
  {
    private readonly string searchTerm;
    private readonly string isSearching;
    private readonly ImmutableList<int> contactIds;

    public ListSearchState(string searchTerm, string isSearching, ImmutableList<int> contactIds)
    {
      this.searchTerm = searchTerm;
      this.isSearching = isSearching;
      this.contactIds = contactIds;
    }

    public string SearchTerm => searchTerm;
    public string IsSearching => isSearching;
    public ImmutableList<int> ContactIds => contactIds;
  }

  public class ListSelectedItemState
  {
    private readonly int selectedId;
    private readonly bool isSelectedSaving; // update, delete, insert
    private readonly bool isSelectedNew; // when adding a new element, instead of editing an old one

    public ListSelectedItemState(int selectedId, bool isSelectedSaving, bool isSelectedNew)
    {
      this.selectedId = selectedId;
      this.isSelectedSaving = isSelectedSaving;
      this.isSelectedNew = isSelectedNew;
    }

    public int SelectedId => selectedId;
    public bool IsSelectedSaving => isSelectedSaving;
    public bool IsSelectedNew => isSelectedNew;
  }
}
