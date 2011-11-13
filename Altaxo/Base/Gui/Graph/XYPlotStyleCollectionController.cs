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
#endregion

using System;
using System.Collections.Generic;

using Altaxo.Collections;
using Altaxo.Main;
using Altaxo.Main.Services;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Styles;

namespace Altaxo.Gui.Graph
{
  #region interfaces
  /// <summary>
  /// Summary description for XYPlotStyleCollectionController.
  /// </summary>
  public interface IXYPlotStyleCollectionView
  {
    IXYPlotStyleCollectionViewEventSink Controller { get; set; }
    void InitializePredefinedStyles(SelectableListNodeList list);
    void InitializeStyleList(SelectableListNodeList list);
    void InitializeAvailableStyleList(SelectableListNodeList list);
  }
  /// <summary>
  /// Summary description for XYPlotStyleCollectionController.
  /// </summary>
  public interface IXYPlotStyleCollectionViewEventSink
  {
    void EhView_AddStyle();
    void EhView_StyleUp();
    void EhView_StyleDown();
    void EhView_StyleEdit();
    void EhView_StyleRemove();
    void EhView_PredefinedStyleSelected();
  }
  /// <summary>
  /// Summary description for XYPlotStyleCollectionController.
  /// </summary>
  public interface IXYPlotStyleCollectionController : IMVCANController
  {
    event EventHandler CollectionChangeCommit;

		/// <summary>Is fired when user selected a style for editing. The argument is the index of the style to edit.</summary>
		event Action<int> StyleEditRequested;
  }

 
  #endregion

  [UserControllerForObject(typeof(G2DPlotStyleCollection))]
  [ExpectedTypeOfView(typeof(IXYPlotStyleCollectionView))]
  public class XYPlotStyleCollectionController : IXYPlotStyleCollectionViewEventSink, IXYPlotStyleCollectionController
  {
    protected IXYPlotStyleCollectionView _view;
    protected G2DPlotStyleCollection _doc;

		SelectableListNodeList _predefinedStyleSetsAvailable;
		SelectableListNodeList _singleStylesAvailable;
		SelectableListNodeList _currentItems;



		/// <summary>Is fired when user selected a style for editing. The argument is the index of the style to edit.</summary>
		public event Action<int> StyleEditRequested;

    public bool InitializeDocument(params object[] args)
    {
      if (args == null || args.Length == 0 || !(args[0] is G2DPlotStyleCollection))
        return false;
      
      _doc = (G2DPlotStyleCollection)args[0];

      Initialize(true);
      return true;
    }

    public UseDocument UseDocumentCopy
    {
      set { }
    }

    public void Initialize(bool initData)
    {
			if (initData)
			{
				// predefined styles
				string[] names = G2DPlotStyleCollectionTemplates.GetAvailableNames();
				_predefinedStyleSetsAvailable = new SelectableListNodeList();
				for (int i = 0; i < names.Length; ++i)
					_predefinedStyleSetsAvailable.Add(new SelectableListNode(names[i], i, false));

				// single styles
				_singleStylesAvailable = new SelectableListNodeList();
				Type[] avtypes = ReflectionService.GetNonAbstractSubclassesOf(typeof(IG2DPlotStyle));
				for (int i = 0; i < avtypes.Length; i++)
				{
					if (avtypes[i] != typeof(G2DPlotStyleCollection))
					{
						_singleStylesAvailable.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(avtypes[i]),avtypes[i],false));
					}
				}

				// current styles
				_currentItems = new SelectableListNodeList();
				for (int i = 0; i < _doc.Count; ++i)
					_currentItems.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(_doc[i].GetType()), _doc[i], false)); 
			}


			if (null != _view)
			{
				_view.InitializePredefinedStyles(_predefinedStyleSetsAvailable);
				_view.InitializeAvailableStyleList(_singleStylesAvailable);
				_view.InitializeStyleList(_currentItems);
			}
    }

 

    #region IXYPlotStyleCollectionViewEventSink Members

    public virtual void EhView_AddStyle()
    {
			var sel = _singleStylesAvailable.FirstSelectedNode;
			if (null == sel)
				return;

      IG2DPlotStyle style = (IG2DPlotStyle)Activator.CreateInstance((Type)sel.Tag);
      IPlotArea layer = DocumentPath.GetRootNodeImplementing<IPlotArea>(_doc);
      G2DPlotItem plotitem = DocumentPath.GetRootNodeImplementing<G2DPlotItem>(_doc);
      if(layer!=null && plotitem!=null)
        _doc.PrepareNewSubStyle(style, layer, plotitem.GetRangesAndPoints(layer));

			_currentItems.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(style.GetType()),style,true));

      OnCollectionChangeCommit();
    }

 

    public virtual void EhView_StyleUp()
    {
			_currentItems.MoveSelectedItemsUp();

      OnCollectionChangeCommit();
      
    }

    public virtual void EhView_StyleDown()
    {
			_currentItems.MoveSelectedItemsDown();
      OnCollectionChangeCommit();
    }

    public virtual void EhView_StyleEdit()
    {
			var idx = _currentItems.FirstSelectedNodeIndex;
			if (idx >= 0 && null != StyleEditRequested)
				StyleEditRequested(idx);
    }

    public virtual void EhView_StyleRemove()
    {
			_currentItems.RemoveSelectedItems();
      OnCollectionChangeCommit();
    }

    public void EhView_PredefinedStyleSelected()
    {
			var sel = _predefinedStyleSetsAvailable.FirstSelectedNode;
      if (null==sel)
        return;

      G2DPlotStyleCollection template = G2DPlotStyleCollectionTemplates.GetTemplate((int)sel.Tag);
			_currentItems.Clear();
      for (int i = 0; i < template.Count; i++)
				_currentItems.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(template[i].GetType()), template[i], false));

      OnCollectionChangeCommit();
    }

    #endregion

    #region CollectionController Members

    public event EventHandler CollectionChangeCommit;
    public virtual void OnCollectionChangeCommit()
    {
      if(CollectionChangeCommit!=null)
        CollectionChangeCommit(this,EventArgs.Empty);
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
				{
					_view.Controller = null;
				}

        _view = value as IXYPlotStyleCollectionView;
       

				if (_view != null)
				{
					Initialize(false);
					_view.Controller = this;
				}
      }
    }

    public virtual object ModelObject
    {
      get
      {
        return _doc;
      }
    }

    #endregion

    #region IApplyController Members

    public virtual bool Apply()
    {
      _doc.BeginUpdate();
      _doc.Clear();
			foreach (var node in _currentItems)
				_doc.Add((IG2DPlotStyle)(node.Tag));
      _doc.EndUpdate();
      return true;
    }

    #endregion
  }

 
}
