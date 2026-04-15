#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

#nullable disable
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Altaxo.Collections;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Groups;
using Altaxo.Graph.Plot.Groups;
using Altaxo.Gui.Common;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Graph.Gdi.Plot.Groups
{
  /// <summary>
  /// Provides the view contract for <see cref="PlotGroupCollectionControllerAdvanced"/>.
  /// </summary>
  public interface IPlotGroupCollectionViewAdvanced : IDataContextAwareView
  {
  }


  /// <summary>
  /// Advanced controller for editing a plot-group style collection.
  /// </summary>
  [UserControllerForObject(typeof(PlotGroupStyleCollection))]
  [ExpectedTypeOfView(typeof(IPlotGroupCollectionViewAdvanced))]
  public class PlotGroupCollectionControllerAdvanced
    :
    MVCANControllerEditOriginalDocBase<PlotGroupStyleCollection, IPlotGroupCollectionViewAdvanced>

  {
    #region Inner classes

    private class MyListNode : CheckableSelectableListNode
    {
      /// <summary>
      /// Initializes a new instance.
      /// </summary>
      public MyListNode(string name, object item, bool isSelected, bool isChecked, bool isCheckBoxVisible)
        : base(name, item, isSelected, isChecked)
      {
        IsCheckBoxVisible = isCheckBoxVisible;
      }

      /// <summary>
      /// Gets or sets the i sc he ck bo xv is ib le.
      /// </summary>
      public bool IsCheckBoxVisible { get; set; }
    }

    #endregion Inner classes

    private IGPlotItem _parent; // usually the parent is the PlotItemCollection

    /// <summary>
    /// Gets or sets the g ro up st yl ec ha ng ed.
    /// </summary>
    public event Action GroupStyleChanged;

    /// <summary>
    /// Number of items where the property <see cref="IPlotGroupStyle.CanCarryOver"/> is true. The list of items is maintained in the way, that those items appear first in the list.
    /// </summary>
    private int _currentNoOfItemsThatCanHaveChilds;

    /// <inheritdoc />
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private ItemsController<ICoordinateTransformingGroupStyle?> _coordinateTransformingGroupStyles;

    /// <summary>
    /// Provides access to this member.
    /// </summary>
    public ItemsController<ICoordinateTransformingGroupStyle?> CoordinateTransformingGroupStyles
    {
      get => _coordinateTransformingGroupStyles;
      set
      {
        if (!(_coordinateTransformingGroupStyles == value))
        {
          _coordinateTransformingGroupStyles?.Dispose();
          _coordinateTransformingGroupStyles = value;
          OnPropertyChanged(nameof(CoordinateTransformingGroupStyles));
        }
      }
    }

    private ItemsController<PlotGroupStrictness> _plotGroupStrictness;

    /// <summary>
    /// Provides access to this member.
    /// </summary>
    public ItemsController<PlotGroupStrictness> PlotGroupStrictness
    {
      get => _plotGroupStrictness;
      set
      {
        if (!(_plotGroupStrictness == value))
        {
          _plotGroupStrictness = value;
          OnPropertyChanged(nameof(PlotGroupStrictness));
        }
      }
    }

    private bool _inheritFromParent;

    /// <summary>
    /// Provides access to this member.
    /// </summary>
    public bool InheritFromParent
    {
      get => _inheritFromParent;
      set
      {
        if (!(_inheritFromParent == value))
        {
          _inheritFromParent = value;
          OnPropertyChanged(nameof(InheritFromParent));
        }
      }
    }
    private bool  _distributeToChilds;

    /// <summary>
    /// Provides access to this member.
    /// </summary>
    public bool  DistributeToChilds
    {
      get => _distributeToChilds;
      set
      {
        if (!(_distributeToChilds == value))
        {
          _distributeToChilds = value;
          OnPropertyChanged(nameof(DistributeToChilds));
        }
      }
    }

    private SelectableListNodeList _availableNormalStyles;

    /// <summary>
    /// Provides access to this member.
    /// </summary>
    public SelectableListNodeList AvailableNormalStyles
    {
      get => _availableNormalStyles;
      set
      {
        if (!(_availableNormalStyles == value))
        {
          _availableNormalStyles = value;
          OnPropertyChanged(nameof(AvailableNormalStyles));
        }
      }
    }


    private CheckableSelectableListNodeList _currentNormalStyles;

    /// <summary>
    /// Provides access to this member.
    /// </summary>
    public CheckableSelectableListNodeList CurrentNormalStyles
    {
      get => _currentNormalStyles;
      set
      {
        if (!(_currentNormalStyles == value))
        {
          _currentNormalStyles = value;
          OnPropertyChanged(nameof(CurrentNormalStyles));
        }
      }
    }




    /// <summary>
    /// Gets or sets the c md ed it co or di na te tr an sf or mi ng gr ou ps ty le.
    /// </summary>
    public ICommand CmdEditCoordinateTransformingGroupStyle { get; }
    /// <summary>
    /// Gets or sets the c md ad dn or ma lg ro up st yl e.
    /// </summary>
    public ICommand CmdAddNormalGroupStyle { get; }
    /// <summary>
    /// Gets or sets the c md re mo ve no rm al gr ou ps ty le.
    /// </summary>
    public ICommand CmdRemoveNormalGroupStyle { get; }
    /// <summary>
    /// Gets or sets the c md in de nt gr ou ps ty le.
    /// </summary>
    public ICommand CmdIndentGroupStyle { get; }
    /// <summary>
    /// Gets or sets the c md un in de nt gr ou ps ty le.
    /// </summary>
    public ICommand CmdUnindentGroupStyle { get; }
    /// <summary>
    /// Gets or sets the c md mo ve up gr ou ps ty le.
    /// </summary>
    public ICommand CmdMoveUpGroupStyle { get; }
    /// <summary>
    /// Gets or sets the c md mo ve do wn gr ou ps ty le.
    /// </summary>
    public ICommand CmdMoveDownGroupStyle { get; }
    /// <summary>
    /// Gets or sets the c md cu rr en tg ro up st yl ed ou bl ec li ck.
    /// </summary>
    public ICommand CmdCurrentGroupStyleDoubleClick { get; }

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="PlotGroupCollectionControllerAdvanced"/> class.
    /// </summary>
    public PlotGroupCollectionControllerAdvanced()
    {
      CmdEditCoordinateTransformingGroupStyle = new RelayCommand(EhView_CoordinateTransformingGroupStyleEdit);

      CmdAddNormalGroupStyle = new RelayCommand(EhView_AddNormalGroupStyle);

      CmdRemoveNormalGroupStyle = new RelayCommand(EhView_RemoveNormalGroupStyle);

      CmdIndentGroupStyle = new RelayCommand(EhView_IndentGroupStyle);

      CmdUnindentGroupStyle = new RelayCommand(EhView_UnindentGroupStyle);

      CmdMoveUpGroupStyle = new RelayCommand(EhView_MoveUpGroupStyle);

      CmdMoveDownGroupStyle = new RelayCommand(EhView_MoveDownGroupStyle);

      CmdCurrentGroupStyleDoubleClick = new RelayCommand(EhCurrentGroupStyleDoubleClick);

    }

    /// <inheritdoc />
    public override void Dispose(bool isDisposing)
    {
      _parent = null;
       CoordinateTransformingGroupStyles = null;
      _availableNormalStyles = null;
      _currentNormalStyles = null;
      GroupStyleChanged = null;

      base.Dispose(isDisposing);
    }

    /// <inheritdoc />
    public override bool InitializeDocument(params object[] args)
    {
      if (args is not null && args.Length > 1 && args[1] is IGPlotItem)
        _parent = (IGPlotItem)args[1];

      return base.InitializeDocument(args);
    }

    /// <inheritdoc />
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        // available Update modes
        PlotGroupStrictness = new ItemsController<PlotGroupStrictness>(new SelectableListNodeList(Altaxo.Graph.Plot.Groups.PlotGroupStrictness.Normal));

        {
          // Transfo-Styles
          var currentTransfoStyle = _doc.CoordinateTransformingStyle is null ? null : (ICoordinateTransformingGroupStyle)_doc.CoordinateTransformingStyle.Clone();
          var availableTransfoStyles = new SelectableListNodeList
            {
             new SelectableListNode("None", null, currentTransfoStyle is null)
            };
          var types = ReflectionService.GetNonAbstractSubclassesOf(typeof(ICoordinateTransformingGroupStyle));
          foreach (Type t in types)
          {
            Type currentStyleType = currentTransfoStyle is null ? null : currentTransfoStyle.GetType();
            ICoordinateTransformingGroupStyle style;
            if (t == currentStyleType)
              style = currentTransfoStyle;
            else
              style = (ICoordinateTransformingGroupStyle)Activator.CreateInstance(t);

            availableTransfoStyles.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(t), style, t == currentStyleType));
          }
          CoordinateTransformingGroupStyles = new ItemsController<ICoordinateTransformingGroupStyle>(availableTransfoStyles);
        }

        {
          // Normal Styles
          _availableNormalStyles = new SelectableListNodeList();
          if (_parent is not null) // if possible, collect only those styles that are applicable
          {
            var avstyles = new PlotGroupStyleCollection();
            _parent.CollectStyles(avstyles);
            foreach (IPlotGroupStyle style in avstyles)
            {
              if (!_doc.ContainsType(style.GetType()))
                _availableNormalStyles.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(style.GetType()), style.GetType(), false));
            }
          }
          else // or else, find all available styles
          {
            var types = ReflectionService.GetNonAbstractSubclassesOf(typeof(IPlotGroupStyle));
            foreach (Type t in types)
            {
              if (!_doc.ContainsType(t))
                _availableNormalStyles.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(t), t, false));
            }
          }
        }

        var list = _doc.GetOrderedListOfItems(ComparePlotGroupStyles);
        _currentNormalStyles = new CheckableSelectableListNodeList();
        _currentNoOfItemsThatCanHaveChilds = 0;
        foreach (var item in list)
        {
          if (item.CanCarryOver)
            ++_currentNoOfItemsThatCanHaveChilds;

          var node = new MyListNode(Current.Gui.GetUserFriendlyClassName(item.GetType()), item.GetType(), false, item.IsStepEnabled, item.CanStep);

          _currentNormalStyles.Add(node);
        }
        UpdateCurrentNormalIndentation();

        InheritFromParent = _doc.InheritFromParentGroups;
        DistributeToChilds = _doc.DistributeToChildGroups;
      }
    }

    /// <inheritdoc />
    public override bool Apply(bool disposeController)
    {
      _doc.CoordinateTransformingStyle = CoordinateTransformingGroupStyles.SelectedValue;

      foreach (CheckableSelectableListNode node in _currentNormalStyles)
      {
        IPlotGroupStyle style = _doc.GetPlotGroupStyle((Type)node.Tag);
        style.IsStepEnabled = node.IsChecked;
      }

      _doc.InheritFromParentGroups = InheritFromParent;
      _doc.DistributeToChildGroups = DistributeToChilds;
      _doc.PlotGroupStrictness = PlotGroupStrictness.SelectedValue;
      

      return ApplyEnd(true, disposeController);
    }

    /// <summary>
    /// Comparison of plot group styles. Primarily they are sorted by the flag <see cref="IPlotGroupStyle.CanCarryOver"/>, so that items that can not have childs appear
    /// later in the list. Secondly, the items that can step appear earlier in the list.  Thirdly, the items are sorted by their parent-child relationship, and finally, by their name.
    /// </summary>
    /// <param name="x">The first plot group style to compare.</param>
    /// <param name="y">The second plot group style to compare.</param>
    /// <returns>A value indicating the relative sort order of the compared plot group styles.</returns>
    private int ComparePlotGroupStyles(IPlotGroupStyle x, IPlotGroupStyle y)
    {
      if (x.CanCarryOver != y.CanCarryOver)
        return x.CanCarryOver ? -1 : 1;
      if (x.CanStep != y.CanStep)
        return x.CanStep ? -1 : 1;
      else
        return string.Compare(Current.Gui.GetUserFriendlyClassName(x.GetType()), Current.Gui.GetUserFriendlyClassName(y.GetType()));
    }

    /// <summary>
    /// Prepends the names in the item list with spaces according to their tree level.
    /// </summary>
    private void UpdateCurrentNormalIndentation()
    {
      foreach (var item in _currentNormalStyles)
      {
        int level = _doc.GetTreeLevelOf((Type)item.Tag);
        var stb = new StringBuilder();
        stb.Append(' ', level * 3);
        stb.Append(item.Text.Trim());
        item.Text = stb.ToString();
      }
    }

    /// <summary>
    /// This updates the list, presuming that the number of items has not changed.
    /// </summary>
    private void UpdateCurrentNormalOrder()
    {
      // if possible, we try to maintain the order in the list in which the items
      // appear

      if (0 == _currentNoOfItemsThatCanHaveChilds)
        return; // then there is nothing to do now

      IPlotGroupStyle previousStyle = null;
      IPlotGroupStyle style = null;
      for (int i = 0; i < _currentNoOfItemsThatCanHaveChilds; i++, previousStyle = style)
      {
        CheckableSelectableListNode node = _currentNormalStyles[i];
        style = _doc.GetPlotGroupStyle((Type)node.Tag);

        if (previousStyle is not null)
        {
          Type prevchildtype = _doc.GetTypeOfChild(previousStyle.GetType());
          if (prevchildtype is not null)
          {
            if (prevchildtype != style.GetType())
            {
              int pi = _currentNormalStyles.IndexOfObject(prevchildtype);
              _currentNormalStyles.Exchange(i, pi);
            }
            continue;
          }
        }

        Type parenttype = _doc.GetParentTypeOf(style.GetType());
        if (parenttype is not null &&
          (previousStyle is null || previousStyle.GetType() != parenttype))
        {
          int pi = _currentNormalStyles.IndexOfObject(parenttype);
          _currentNormalStyles.Exchange(i, pi);
        }
      }
      UpdateCurrentNormalIndentation();
    }

    #region IPlotGroupCollectionViewEventSink Members

    /// <summary>
    /// Handles the v ie w c oo rd in at et ra ns fo rm in gg ro up st yl ee di t.
    /// </summary>
    public void EhView_CoordinateTransformingGroupStyleEdit()
    {
      if (CoordinateTransformingGroupStyles.SelectedValue is { } currentTransfoStyle)
      {
        Current.Gui.ShowDialog(new object[] { currentTransfoStyle }, "Edit transformation style");
      }
    }

    /// <summary>
    /// Handles the v ie w a dd no rm al gr ou ps ty le.
    /// </summary>
    public void EhView_AddNormalGroupStyle()
    {
      SelectableListNode selected = null;
      foreach (SelectableListNode node in _availableNormalStyles)
      {
        if (node.IsSelected)
        {
          selected = node;
          break;
        }
      }
      if (selected is not null)
      {
        _availableNormalStyles.Remove(selected);

        var s = (IPlotGroupStyle)Activator.CreateInstance((Type)selected.Tag);
        _doc.Add(s);
        var node = new MyListNode(
          Current.Gui.GetUserFriendlyClassName(s.GetType()),
          s.GetType(), true, s.IsStepEnabled, s.CanStep);
        if (s.CanCarryOver)
        {
          _currentNormalStyles.Insert(_currentNoOfItemsThatCanHaveChilds, node);
          _currentNoOfItemsThatCanHaveChilds++;
        }
        else
        {
          _currentNormalStyles.Add(node);
        }
      }
    }

    /// <summary>
    /// Handles the v ie w r em ov en or ma lg ro up st yl e.
    /// </summary>
    public void EhView_RemoveNormalGroupStyle()
    {
      for (int i = _currentNormalStyles.Count - 1; i >= 0; i--)
      {
        CheckableSelectableListNode selected = _currentNormalStyles[i];
        if (!selected.IsSelected)
          continue;

        _doc.RemoveType((Type)selected.Tag);

        _currentNormalStyles.RemoveAt(i);
        if (i < _currentNoOfItemsThatCanHaveChilds)
          _currentNoOfItemsThatCanHaveChilds--;

        _availableNormalStyles.Add(new SelectableListNode(
          Current.Gui.GetUserFriendlyClassName((Type)selected.Tag),
          selected.Tag,
          true));
      }

      UpdateCurrentNormalIndentation();
    }

    /// <summary>
    /// Handles the v ie w i nd en tg ro up st yl e.
    /// </summary>
    public void EhView_IndentGroupStyle()
    {
      // for all selected items: append it as child to the item upward
      int count = Math.Min(_currentNoOfItemsThatCanHaveChilds + 1, _currentNormalStyles.Count); // note: the first item that can step, but can not have childs, could also be indented, thats why 1+
      for (int i = 1; i < count; ++i)
      {
        CheckableSelectableListNode selected = _currentNormalStyles[i];
        if (!selected.IsSelected)
          continue;

        if (_doc.GetParentTypeOf((Type)selected.Tag) is not null)
          continue; // only ident those items who dont have a parent

        IPlotGroupStyle style = _doc.GetPlotGroupStyle((Type)selected.Tag);
        _doc.RemoveType(style.GetType()); // Removing the type so removing also the parent-child-relationship
        _doc.Add(style, (Type)_currentNormalStyles[i - 1].Tag); // Add the type again, but this time without parents or childs
      }
      // this requires the whole currentNormalStyle list to be updated
      UpdateCurrentNormalOrder();
      UpdateCurrentNormalIndentation();
    }

    /// <summary>
    /// Handles the v ie w u ni nd en tg ro up st yl e.
    /// </summary>
    public void EhView_UnindentGroupStyle()
    {
      // make sure that all the selected items are not child of another item
      for (int i = _currentNormalStyles.Count - 1; i >= 0; i--)
      {
        CheckableSelectableListNode selected = _currentNormalStyles[i];
        if (!selected.IsSelected)
          continue;

        if (_doc.GetParentTypeOf((Type)selected.Tag) is not null)
        {
          IPlotGroupStyle style = _doc.GetPlotGroupStyle((Type)selected.Tag);
          _doc.RemoveType(style.GetType()); // Removing the type so removing also the parent-child-relationship
          _doc.Add(style); // Add the type again, but this time without parents or childs
        }
      }

      // this requires the whole currentNormalStyle list to be updated
      UpdateCurrentNormalOrder();
      UpdateCurrentNormalIndentation();
    }

    /// <summary>
    /// Handles the v ie w m ov eu pg ro up st yl e.
    /// </summary>
    public void EhView_MoveUpGroupStyle()
    {
      if (0 == _currentNoOfItemsThatCanHaveChilds || _currentNormalStyles[0].IsSelected)
        return; // can not move up any more

      for (int i = 1; i < _currentNoOfItemsThatCanHaveChilds; i++)
      {
        CheckableSelectableListNode selected = _currentNormalStyles[i];
        if (!selected.IsSelected)
          continue;

        IPlotGroupStyle style = _doc.GetPlotGroupStyle((Type)selected.Tag);
        Type parenttype = _doc.GetParentTypeOf(style.GetType());
        _doc.RemoveType(style.GetType()); // Removing the type so removing also the parent-child-relationship
        if (parenttype is null)
        {
          _doc.Add(style);
        }
        else
        {
          _doc.Insert(style, parenttype); // Add the type, but parent type is this time the child type
        }
        _currentNormalStyles.Exchange(i, i - 1);
      }
      // this requires the whole currentNormalStyle list to be updated
      UpdateCurrentNormalOrder();
    }

    /// <summary>
    /// Handles the v ie w m ov ed ow ng ro up st yl e.
    /// </summary>
    public void EhView_MoveDownGroupStyle()
    {
      if (0 == _currentNoOfItemsThatCanHaveChilds || _currentNormalStyles[_currentNoOfItemsThatCanHaveChilds - 1].IsSelected)
        return; // can not move down any more

      for (int i = _currentNoOfItemsThatCanHaveChilds - 2; i >= 0; i--)
      {
        CheckableSelectableListNode selected = _currentNormalStyles[i];
        if (!selected.IsSelected)
          continue;

        IPlotGroupStyle style = _doc.GetPlotGroupStyle((Type)selected.Tag);
        Type childtype = _doc.GetTypeOfChild(style.GetType());
        _doc.RemoveType(style.GetType()); // Removing the type so removing also the parent-child-relationship
        if (childtype is null)
        {
          _doc.Add(style);
        }
        else
        {
          _doc.Add(style, childtype); // Add the type, but the child type this time is the parent type
        }
        _currentNormalStyles.Exchange(i, i + 1);
      }
      // this requires the whole currentNormalStyle list to be updated
      UpdateCurrentNormalOrder();
    }

    /// <summary>
    /// Handles the c ur re nt gr ou ps ty le do ub le cl ic k.
    /// </summary>
    public void EhCurrentGroupStyleDoubleClick()
    {

    }

    #endregion IPlotGroupCollectionViewEventSink Members
  }
}
