#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using System.Linq;
using System.Windows.Input;
using Altaxo.Collections;
using Altaxo.Graph.Graph3D;
using Altaxo.Graph.Graph3D.Plot;
using Altaxo.Graph.Graph3D.Plot.Styles;
using Altaxo.Gui.Common;
using Altaxo.Main;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Graph.Graph3D.Plot.Styles
{
  /// <summary>
  /// Summary description for XYPlotStyleCollectionController.
  /// </summary>
  public interface IXYZPlotStyleCollectionView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Summary description for XYPlotStyleCollectionController.
  /// </summary>
  public interface IXYZPlotStyleCollectionController : IMVCANController
  {
    event EventHandler CollectionChangeCommit;

    /// <summary>Is fired when user selected a style for editing. The argument is the index of the style to edit.</summary>
    event Action<int> StyleEditRequested;
  }


  [UserControllerForObject(typeof(G3DPlotStyleCollection))]
  [ExpectedTypeOfView(typeof(IXYZPlotStyleCollectionView))]
  public class XYZPlotStyleCollectionController
    :
    MVCANControllerEditOriginalDocBase<G3DPlotStyleCollection, IXYZPlotStyleCollectionView>,
    IXYZPlotStyleCollectionController
  {
    /// <summary>Is fired when user selected a style for editing. The argument is the index of the style to edit.</summary>
    public event Action<int> StyleEditRequested;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public XYZPlotStyleCollectionController()
    {
      CmdAddPredefinedStyleSet = new RelayCommand(EhView_AddPredefinedStyleSet);
      CmdAddSingleStyle = new RelayCommand(EhView_AddSingleStyle);
      CmdStyleUp = new RelayCommand(EhView_StyleUp);
      CmdStyleDown = new RelayCommand(EhView_StyleDown);
      CmdStyleEdit = new RelayCommand(EhView_StyleEdit);
      CmdStyleRemove = new RelayCommand(EhView_StyleRemove);
    }

    #region Bindings

    public ICommand CmdAddPredefinedStyleSet { get; }
    public ICommand CmdAddSingleStyle { get; }
    public ICommand CmdStyleUp { get; }
    public ICommand CmdStyleDown { get; }
    public ICommand CmdStyleEdit { get; }
    public ICommand CmdStyleRemove { get; }

    private ItemsController<int> _predefinedStyleSetsAvailable;

    public ItemsController<int> PredefinedStyleSetsAvailable
    {
      get => _predefinedStyleSetsAvailable;
      set
      {
        if (!(_predefinedStyleSetsAvailable == value))
        {
          _predefinedStyleSetsAvailable = value;
          OnPropertyChanged(nameof(PredefinedStyleSetsAvailable));
        }
      }
    }

    private ItemsController<Type> _singleStylesAvailable;

    public ItemsController<Type> SingleStylesAvailable
    {
      get => _singleStylesAvailable;
      set
      {
        if (!(_singleStylesAvailable == value))
        {
          _singleStylesAvailable = value;
          OnPropertyChanged(nameof(SingleStylesAvailable));
        }
      }
    }

    private SelectableListNodeList _currentItems;

    public SelectableListNodeList CurrentItems
    {
      get => _currentItems;
      set
      {
        if (!(_currentItems == value))
        {
          _currentItems = value;
          OnPropertyChanged(nameof(CurrentItems));
        }
      }
    }


    #endregion


    public override void Dispose(bool isDisposing)
    {
      _predefinedStyleSetsAvailable = null;
      _singleStylesAvailable = null;
      _currentItems = null;
      StyleEditRequested = null;

      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        // predefined styles
        string[] names = G3DPlotStyleCollectionTemplates.GetAvailableNames();
        var predefinedStyleSetsAvailable = new SelectableListNodeList();
        for (int i = 0; i < names.Length; ++i)
          predefinedStyleSetsAvailable.Add(new SelectableListNode(names[i], i, false));
        PredefinedStyleSetsAvailable = new ItemsController<int>(predefinedStyleSetsAvailable);

        // single styles
        var singleStylesAvailable = new SelectableListNodeList();
        Type[] avtypes = ReflectionService.GetNonAbstractSubclassesOf(typeof(IG3DPlotStyle));
        for (int i = 0; i < avtypes.Length; i++)
        {
          if (avtypes[i] != typeof(G3DPlotStyleCollection))
          {
            singleStylesAvailable.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(avtypes[i]), avtypes[i], false));
          }
        }
        SingleStylesAvailable = new ItemsController<Type>(singleStylesAvailable);

        BuildCurrentStyleListNodeList();
      }
    }

    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
    }

    

    #region IXYZPlotStyleCollectionViewEventSink Members

    private void BuildCurrentStyleListNodeList()
    {
      // current styles
      _currentItems = new SelectableListNodeList();
      for (int i = 0; i < _doc.Count; ++i)
        _currentItems.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(_doc[i].GetType()), _doc[i], false));
    }

    public virtual void EhView_AddSingleStyle()
    {
      var sel = _singleStylesAvailable.SelectedItem;
      if (sel is null)
        return;

      var propertyContext = Altaxo.PropertyExtensions.GetPropertyContext(_doc);
      IG3DPlotStyle style = null;
      try
      {
        style = (IG3DPlotStyle)Activator.CreateInstance((Type)sel.Tag, propertyContext); // first try with a constructor which uses a property context
      }
      catch (System.MissingMethodException)
      {
      }

      if (style is null) // if style was not constructed
        style = (IG3DPlotStyle)Activator.CreateInstance((Type)sel.Tag); // try with parameterless constructor

      var layer = AbsoluteDocumentPath.GetRootNodeImplementing<IPlotArea>(_doc);
      var plotitem = AbsoluteDocumentPath.GetRootNodeImplementing<G3DPlotItem>(_doc);
      if (layer is not null && plotitem is not null)
        _doc.PrepareNewSubStyle(style, layer, plotitem.GetRangesAndPoints(layer));

      _currentItems.Add<IG3DPlotStyle>(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(style.GetType()), style, true), (docNodeToAdd) => _doc.Add(docNodeToAdd));

      OnCollectionChangeCommit();
    }

    public virtual void EhView_StyleUp()
    {
      _currentItems.MoveSelectedItemsUp((i, j) => _doc.ExchangeItemPositions(i, j));
      OnCollectionChangeCommit();
    }

    public virtual void EhView_StyleDown()
    {
      _currentItems.MoveSelectedItemsDown((i, j) => _doc.ExchangeItemPositions(i, j));
      OnCollectionChangeCommit();
    }

    public virtual void EhView_StyleEdit()
    {
      var idx = _currentItems.FirstSelectedNodeIndex;
      if (idx >= 0 && StyleEditRequested is not null)
        StyleEditRequested(idx);
    }

    public virtual void EhView_StyleRemove()
    {
      _currentItems.RemoveSelectedItems((i, tag) => _doc.RemoveAt(i));
      OnCollectionChangeCommit();
    }

    public void EhView_AddPredefinedStyleSet()
    {
      var sel = _predefinedStyleSetsAvailable.SelectedItem;
      if (sel is null)
        return;

      var template = G3DPlotStyleCollectionTemplates.GetTemplate((int)sel.Tag, _doc.GetPropertyContext());
      _currentItems.Clear(() => _doc.Clear());
      for (int i = 0; i < template.Count; i++)
      {
        var listNode = new SelectableListNode(Current.Gui.GetUserFriendlyClassName(template[i].GetType()), template[i], false);
        _currentItems.Add<IG3DPlotStyle>(listNode, (docNode) => _doc.Add(docNode));
      }

      OnCollectionChangeCommit();
    }

    #endregion IXYZPlotStyleCollectionViewEventSink Members

    #region CollectionController Members

    public event EventHandler CollectionChangeCommit;

    public virtual void OnCollectionChangeCommit()
    {
      CollectionChangeCommit?.Invoke(this, EventArgs.Empty);
    }

    #endregion CollectionController Members
  }
}
