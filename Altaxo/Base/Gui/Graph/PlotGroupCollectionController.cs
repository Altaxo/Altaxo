#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using Altaxo.Collections;
using Altaxo.Graph.Plot.Groups;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Groups;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Graph
{
  #region Interfaces
  public interface IPlotGroupCollectionView
  {
    IPlotGroupCollectionViewEventSink Controller { set; }
    void InitializeAvailableCoordinateTransformingGroupStyles(SelectableListNodeList list);
    void InitializeAvailableNormalGroupStyles(SelectableListNodeList list);
    void InitializeUpdateMode(SelectableListNodeList list, bool inheritFromParent, bool distributeToChilds);
    void InitializeCurrentNormalGroupStyles(CheckableSelectableListNodeList list);
    void SynchronizeCurrentNormalGroupStyles();
    void QueryUpdateMode(out bool inheritFromParent, out bool distributeToChilds);
    
  }

  public interface IPlotGroupCollectionViewEventSink
  {
    void EhView_CoordinateTransformingGroupStyleChanged();
    void EhView_CoordinateTransformingGroupStyleEdit();
    void EhView_AddNormalGroupStyle();
    void EhView_RemoveNormalGroupStyle();

    void EhView_IndentGroupStyle();
    void EhView_UnindentGroupStyle();
    void EhView_MoveUpGroupStyle();
    void EhView_MoveDownGroupStyle();
  }
  #endregion

  [UserControllerForObject(typeof(PlotGroupStyleCollection))]
  [ExpectedTypeOfView(typeof(IPlotGroupCollectionView))]
  public class PlotGroupCollectionController : IMVCANController, IPlotGroupCollectionViewEventSink
  {
    PlotGroupStyleCollection _origdoc;
    PlotGroupStyleCollection _doc;
    IGPlotItem _parent; // usually the parent is the PlotItemCollection
    IPlotGroupCollectionView _view;
    UseDocument _useDocument;

    SelectableListNodeList _availableTransfoStyles;
    SelectableListNodeList _availableNormalStyles;
    CheckableSelectableListNodeList _currentNormalStyles;
    SelectableListNodeList _availableUpdateModes;
    Type _currentTransfoStyle;
    int _currentStepItems;

    void Initialize(bool initDoc)
    {
      if (initDoc)
      {
        // available Update modes
        _availableUpdateModes = new SelectableListNodeList();
        foreach (object obj in Enum.GetValues(typeof(PlotGroupStrictness)))
          _availableUpdateModes.Add(new SelectableListNode(obj.ToString(), obj, ((PlotGroupStrictness)obj) == PlotGroupStrictness.Normal));

        Type[] types;
        // Transfo-Styles
        _currentTransfoStyle = _doc.CoordinateTransformingStyle == null ? null : _doc.CoordinateTransformingStyle.GetType();
        _availableTransfoStyles = new SelectableListNodeList();
        _availableTransfoStyles.Add(new SelectableListNode("None",null,null==_currentTransfoStyle));
         types = ReflectionService.GetNonAbstractSubclassesOf(typeof(ICoordinateTransformingGroupStyle));
        foreach (Type t in types)
        {
            _availableTransfoStyles.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(t), t, t==_currentTransfoStyle));
        }

        // Normal Styles
        _availableNormalStyles = new SelectableListNodeList();
        if (_parent != null) // if possible, collect only those styles that are applicable
        {
          PlotGroupStyleCollection avstyles = new PlotGroupStyleCollection();
          _parent.CollectStyles(avstyles);
          foreach(IPlotGroupStyle style in avstyles)
          {
            if(!_doc.ContainsType(style.GetType()))
            _availableNormalStyles.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(style.GetType()),style.GetType(),false));
          }
        }
        else // or else, find all available styles
        {
          types = ReflectionService.GetNonAbstractSubclassesOf(typeof(IPlotGroupStyle));
          foreach (Type t in types)
          {
            if (!_doc.ContainsType(t))
              _availableNormalStyles.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(t), t, false));
          }
        }

        _currentNormalStyles = new CheckableSelectableListNodeList();
        _currentStepItems = 0;
          // first those items that have no childs
        foreach (IPlotGroupStyle s in _doc)
        {
          CheckableSelectableListNode node = new CheckableSelectableListNode(Current.Gui.GetUserFriendlyClassName(s.GetType()), s.GetType(), false, false);

          if (s.CanHaveChilds())
          {
            node.Checked = s.IsStepEnabled;
            _currentNormalStyles.Insert(_currentStepItems, node);
            _currentStepItems++;
          }
          else
          {
            node.Checked = s.IsStepEnabled;
            _currentNormalStyles.Add(node);
          }
        }
        
       UpdateCurrentNormalOrder(); // bring the items in the right order
      }
      if (_view != null)
      {
        _view.InitializeAvailableCoordinateTransformingGroupStyles(_availableTransfoStyles);
        _view.InitializeAvailableNormalGroupStyles(_availableNormalStyles);
        _view.InitializeCurrentNormalGroupStyles(_currentNormalStyles);
        _view.InitializeUpdateMode(_availableUpdateModes, _doc.InheritFromParentGroups, _doc.DistributeToChildGroups);
      }
    }

    void UpdateCurrentNormalIndentation()
    {
        int indentation = 0;
        for (int i = 0; i < _currentStepItems; i++)
        {
          Type childtype = i==0 ? null : _doc.GetChildTypeOf((Type)_currentNormalStyles[i - 1].Item);
          if (childtype == (Type)_currentNormalStyles[i].Item)
            indentation++;
          else
            indentation = 0;

          if (indentation > 0)
          {
            StringBuilder stb = new StringBuilder();
            stb.Append(' ', indentation * 3);
            stb.Append(_currentNormalStyles[i].Name.Trim());
            _currentNormalStyles[i].Name = stb.ToString();
          }
          else
          {
            _currentNormalStyles[i].Name = _currentNormalStyles[i].Name.Trim();
          }
        }
    }

    /// <summary>
    /// This updates the list, presuming that the number of items has not changed.
    /// </summary>
    void UpdateCurrentNormalOrder()
    {
      // if possible, we try to maintain the order in the list in which the items
      // appear

      if (0 == _currentStepItems)
        return; // then there is nothing to do now

      IPlotGroupStyle previousStyle = null;
      IPlotGroupStyle style = null;
      for (int i = 0; i < _currentStepItems; i++, previousStyle=style)
      {
        CheckableSelectableListNode node = _currentNormalStyles[i];
        style = _doc.GetPlotGroupStyle((Type)node.Item);

        if (previousStyle != null)
        {
          Type prevchildtype = _doc.GetChildTypeOf(previousStyle.GetType());
          if (prevchildtype != null)
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
        if (parenttype != null && 
          (previousStyle==null || previousStyle.GetType()!=parenttype))
          {
            int pi = _currentNormalStyles.IndexOfObject(parenttype);
            _currentNormalStyles.Exchange(i, pi);
          }

         
      }
      UpdateCurrentNormalIndentation();

      
    }

    #region IMVCANController Members

    public bool InitializeDocument(params object[] args)
    {
      if (args == null || args.Length == 0 || !(args[0] is PlotGroupStyleCollection))
        return false;

      _origdoc = (PlotGroupStyleCollection)args[0];
      _doc = _useDocument == UseDocument.Directly ? _origdoc : _origdoc.Clone();

      if (args.Length >= 2 && args[1] is IGPlotItem)
        _parent = (IGPlotItem)args[1];

      Initialize(true);
      return true;
    }

    public UseDocument UseDocumentCopy
    {
      set { _useDocument = value; }
    }

    #endregion

    #region IMVCController Members

    public object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        if (_view != null)
          _view.Controller = null;

        _view = value as IPlotGroupCollectionView;

        if (_view != null)
        {
          Initialize(false);
          _view.Controller = this;
        }
      }
    }

    public object ModelObject
    {
      get { return _origdoc; }
    }

    #endregion

    #region IApplyController Members

    public bool Apply()
    {
      foreach (SelectableListNode node in _availableTransfoStyles)
      {
        if (node.Selected)
        {
          _currentTransfoStyle = (Type)node.Item;
          break;
        }
      }

      if (null != _currentTransfoStyle)
        _doc.CoordinateTransformingStyle = (ICoordinateTransformingGroupStyle)Activator.CreateInstance(_currentTransfoStyle);
      else
        _doc.CoordinateTransformingStyle = null;

      _view.SynchronizeCurrentNormalGroupStyles(); // synchronize the checked state of the items
      foreach (CheckableSelectableListNode node in _currentNormalStyles)
      {
        IPlotGroupStyle style = _doc.GetPlotGroupStyle((Type)node.Item);
        style.IsStepEnabled = node.Checked;
      }

      bool inherit, distribute;
      _view.QueryUpdateMode(out inherit, out distribute);
      _doc.InheritFromParentGroups = inherit;
      _doc.DistributeToChildGroups = distribute;
      foreach (SelectableListNode node in _availableUpdateModes)
      {
        if (node.Selected)
        {
          _doc.PlotGroupStrictness = (PlotGroupStrictness)node.Item;
          break;
        }
      }

      if (_useDocument == UseDocument.Copy)
        _origdoc.CopyFrom(_doc);
      return true;
    }

    #endregion

    #region IPlotGroupCollectionViewEventSink Members

    public void EhView_CoordinateTransformingGroupStyleChanged()
    {

    }

    public void EhView_CoordinateTransformingGroupStyleEdit()
    {

    }



    public void EhView_AddNormalGroupStyle()
    {
      SelectableListNode selected = null;
      foreach (SelectableListNode node in _availableNormalStyles)
      {
        if (node.Selected)
        {
          selected = node;
          break;
        }
      }
      if (null != selected)
      {
        _availableNormalStyles.Remove(selected);

        IPlotGroupStyle s = (IPlotGroupStyle)Activator.CreateInstance((Type)selected.Item);
        _doc.Add(s);
        CheckableSelectableListNode node = new CheckableSelectableListNode(
          Current.Gui.GetUserFriendlyClassName(s.GetType()),
          s.GetType(), true, s.IsStepEnabled);
        if (s.CanHaveChilds())
        {
          _currentNormalStyles.Insert(_currentStepItems, node);
          _currentStepItems++;
        }
        else
        {
          _currentNormalStyles.Add(node);
        }

        _view.InitializeAvailableNormalGroupStyles(_availableNormalStyles);
        _view.InitializeCurrentNormalGroupStyles(_currentNormalStyles);
      }
    }

    public void EhView_RemoveNormalGroupStyle()
    {
      for(int i=_currentNormalStyles.Count-1;i>=0;i--)
      {
        CheckableSelectableListNode selected = _currentNormalStyles[i];
        if (!selected.Selected)
          continue;

        _doc.RemoveType((Type)selected.Item);

        _currentNormalStyles.RemoveAt(i);
        if (i < _currentStepItems)
          _currentStepItems--;

        _availableNormalStyles.Add(new SelectableListNode(
          Current.Gui.GetUserFriendlyClassName((Type)selected.Item),
          selected.Item,
          true));

      }

      UpdateCurrentNormalIndentation();
      _view.InitializeAvailableNormalGroupStyles(_availableNormalStyles);
      _view.InitializeCurrentNormalGroupStyles(_currentNormalStyles);

    }

    public void EhView_IndentGroupStyle()
    {
      // for all selected items: append it as child to the item upward

      for (int i = 1; i < _currentStepItems; i++)
      {
        CheckableSelectableListNode selected = _currentNormalStyles[i];
        if (!selected.Selected)
          continue;

        if (null != _doc.GetParentTypeOf((Type)selected.Item))
          continue; // only ident those items who dont have a parent

        IPlotGroupStyle style = _doc.GetPlotGroupStyle((Type)selected.Item);
        _doc.RemoveType(style.GetType()); // Removing the type so removing also the parent-child-relationship
        _doc.Add(style,(Type)_currentNormalStyles[i-1].Item); // Add the type again, but this time without parents or childs
      }
      // this requires the whole currentNormalStyle list to be updated
      UpdateCurrentNormalOrder();
      _view.InitializeCurrentNormalGroupStyles(_currentNormalStyles);

    }

    public void EhView_UnindentGroupStyle()
    {
      // make sure that all the selected items are not child of another item
      for (int i = _currentNormalStyles.Count - 1; i >= 0; i--)
      {
        CheckableSelectableListNode selected = _currentNormalStyles[i];
        if (!selected.Selected)
          continue;

        if (null != _doc.GetParentTypeOf((Type)selected.Item))
        {
          IPlotGroupStyle style = _doc.GetPlotGroupStyle((Type)selected.Item);
          _doc.RemoveType(style.GetType()); // Removing the type so removing also the parent-child-relationship
          _doc.Add(style); // Add the type again, but this time without parents or childs
        }

      }

      // this requires the whole currentNormalStyle list to be updated
      UpdateCurrentNormalOrder();
      _view.InitializeCurrentNormalGroupStyles(_currentNormalStyles);

    }

    public void EhView_MoveUpGroupStyle()
    {
      if (0 == _currentStepItems || _currentNormalStyles[0].Selected)
        return; // can not move up any more

      for (int i = 1; i < _currentStepItems; i++)
      {
        CheckableSelectableListNode selected = _currentNormalStyles[i];
        if (!selected.Selected)
          continue;

        IPlotGroupStyle style = _doc.GetPlotGroupStyle((Type)selected.Item);
        Type parenttype = _doc.GetParentTypeOf(style.GetType());
        _doc.RemoveType(style.GetType()); // Removing the type so removing also the parent-child-relationship
        if (parenttype == null)
        {
          _doc.Add(style);
        }
        else
        {
          _doc.Insert(style,parenttype); // Add the type, but parent type is this time the child type
        }
        _currentNormalStyles.Exchange(i, i - 1);
      }
      // this requires the whole currentNormalStyle list to be updated
      UpdateCurrentNormalOrder();
      _view.InitializeCurrentNormalGroupStyles(_currentNormalStyles);

    }

    public void EhView_MoveDownGroupStyle()
    {
      if (0==_currentStepItems || _currentNormalStyles[_currentStepItems-1].Selected)
        return; // can not move down any more

      for (int i = _currentStepItems-2; i >=0; i--)
      {
        CheckableSelectableListNode selected = _currentNormalStyles[i];
        if (!selected.Selected)
          continue;

        IPlotGroupStyle style = _doc.GetPlotGroupStyle((Type)selected.Item);
        Type childtype = _doc.GetChildTypeOf(style.GetType());
        _doc.RemoveType(style.GetType()); // Removing the type so removing also the parent-child-relationship
        if (childtype == null)
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
      _view.InitializeCurrentNormalGroupStyles(_currentNormalStyles);
      
    }

    #endregion
  }
}
