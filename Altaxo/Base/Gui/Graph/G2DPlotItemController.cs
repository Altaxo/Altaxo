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

using Altaxo.Main;
using Altaxo.Gui.Common;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Gdi.Plot.Groups;
using Altaxo.Graph.Plot.Groups;

using System.Collections;

namespace Altaxo.Gui.Graph
{
	#region Interfaces

	public interface IG2DPlotItemView
	{
		/// <summary>
		/// Removes all Tab pages from the dialog.
		/// </summary>
		void ClearTabs();

		/// <summary>
		/// Adds a Tab page to the dialog
		/// </summary>
		/// <param name="title">The title of the tab page.</param>
		/// <param name="view">The view (must be currently of type Control.</param>
		void AddTab(string title, object view);

		/// <summary>
		/// Activates the tab page with the title <code>title</code>.
		/// </summary>
		/// <param name="index">The index of the tab page to focus.</param>
		void BringTabToFront(int index);


		event Action<object, InstanceChangedEventArgs<object>> SelectedPage_Changed;

		/// <summary>
		/// Sets the plot style view, i.e. the control where we can add or remove plot styles.
		/// </summary>
		/// <param name="view"></param>
		void SetPlotStyleView(object view);


		void SetPlotGroupCollectionView(object view);
	}

	#endregion

	/// <summary>
  /// Summary description for XYColumnPlotItemController.
  /// </summary>
  [UserControllerForObject(typeof(G2DPlotItem))]
	[ExpectedTypeOfView(typeof(IG2DPlotItemView))]
  public class G2DPlotItemController : IMVCANController
    
  {
    UseDocument _useDocument;
    G2DPlotItem _doc;
    G2DPlotItem _tempdoc;
    PlotGroupStyleCollection _groupStyles;

		IG2DPlotItemView _view;

		/// <summary>Controller for the <see cref="PlotGroupStyleCollection"/> that is associated with the parent of this plot item.</summary>
		IMVCANController _plotGroupController;


    IG2DPlotStyle _additionalPlotStyle;
    IPlotGroupCollectionViewSimple _plotGroupView;

    IMVCAController _dataController;
    IXYPlotStyleCollectionController _styleCollectionController;
    List<IMVCANController> _styleControllerList = new List<IMVCANController>();
    Dictionary<IG2DPlotStyle, IMVCANController> _styleControllerDictionary = new Dictionary<IG2DPlotStyle, IMVCANController>();
    IMVCANController _additionalPlotStyleController;
    
    

    public G2DPlotItemController()
    {
    }

    public G2DPlotItemController(G2DPlotItem doc)
      : this(doc,null)
    {
    }
    public G2DPlotItemController(G2DPlotItem doc, PlotGroupStyleCollection parent)
    {
      if (!InitializeDocument(doc, parent))
        throw new ArgumentException();
    }

    public bool InitializeDocument(params object[] args)
    {
      if (args == null || args.Length == 0  || !(args[0] is G2DPlotItem))
        return false;
      
       _doc = _tempdoc = (G2DPlotItem)args[0];

      if (args.Length >= 2 && args[1] != null)
      {
        if (!(args[1] is PlotGroupStyleCollection))
          return false;
        else
          _groupStyles = (PlotGroupStyleCollection)args[1];
      }
      else
      {
        if(_doc.ParentCollection!=null)
          _groupStyles = _doc.ParentCollection.GroupStyles;
      }

      if(_useDocument==UseDocument.Copy)
        _tempdoc = (G2DPlotItem)_doc.Clone();

			/*
      InitializeCollectionAndData();
      InitializeStyles();
      BringTabToFront(2);
			*/

			Initialize(true);
      return true;
    }

    public UseDocument UseDocumentCopy
    {
      set { _useDocument = value; }
    }


		void Initialize(bool initData)
		{
			if (initData)
			{
				_plotGroupController = new PlotGroupCollectionController();
				_plotGroupController.InitializeDocument(_groupStyles);


				// find the style collection controller
				_styleCollectionController = (IXYPlotStyleCollectionController)Current.Gui.GetControllerAndControl(new object[] { _tempdoc.Style }, typeof(IXYPlotStyleCollectionController));
				_styleCollectionController.CollectionChangeCommit += new EventHandler(_styleCollectionController_CollectionChangeCommit);
				_styleCollectionController.StyleEditRequested += new Action<int>(_styleCollectionController_StyleEditRequested);


				// Initialize the data controller
				_dataController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _tempdoc.DataObject, _tempdoc }, typeof(IMVCAController));

				// Initialize the style controller list
				InitializeStyleControllerList();

			}

			if (null != _view)
			{
				if (null == _plotGroupController.ViewObject)
					Current.Gui.FindAndAttachControlTo(_plotGroupController);
				_view.SetPlotGroupCollectionView(_plotGroupController.ViewObject);

				// add the style controller
				_view.SetPlotStyleView(_styleCollectionController.ViewObject);

				View_SetAllTabViews();
			}
		}

	


		void View_SetAllTabViews()
		{
			_view.ClearTabs();

			// Add the data tab item
			if (_dataController != null)
				_view.AddTab("Data", _dataController.ViewObject);

			// set the plot style tab items
			for (int i = 0; i < _styleControllerList.Count; ++i)
			{
				string title = string.Format("#{0}:{1}", (i + 1), Current.Gui.GetUserFriendlyClassName(_tempdoc.Style[i].GetType()));
				_view.AddTab(title, _styleControllerList[i].ViewObject);
			}
		}

    void ApplyPlotGroupView()
    {
			_plotGroupController.Apply();
			_groupStyles.CopyFrom((PlotGroupStyleCollection)_plotGroupController.ModelObject);

      // now distribute the new style to the other plot items
      if (_doc.ParentCollection != null)
      {
				_doc.ParentCollection.GroupStyles.CopyFrom(_groupStyles);
        _doc.ParentCollection.DistributePlotStyleFromTemplate(_doc, _groupStyles.PlotGroupStrictness);
        _doc.ParentCollection.DistributeChanges(_doc);
      }
    }

		/// <summary>
		/// Gets the controller for a certain style instance from either the dictionary, or if not found, by creating a new instance.
		/// </summary>
		/// <param name="style"></param>
		/// <returns></returns>
    IMVCANController GetStyleController(IG2DPlotStyle style)
    {
      if (_styleControllerDictionary.ContainsKey(style))
        return _styleControllerDictionary[style];

      IMVCANController ct = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { style }, typeof(IMVCANController), UseDocument.Directly);

      if (ct != null)
        _styleControllerDictionary.Add(style, ct);

      return ct;
    }

		void InitializeStyleControllerList()
		{
			// Clear the previous controller cache
			_styleControllerList.Clear();

			// start to create new controllers
			for (int i = 0; i < _tempdoc.Style.Count; i++)
			{
				IMVCANController ctrl = GetStyleController(_tempdoc.Style[i]);
				_styleControllerList.Add(ctrl);
			}
		}

	


    protected void EhView_ActiveChildControlChanged(object sender, InstanceChangedEventArgs<object> e)
    {
        // test if it is the view of the normal styles
        for (int i = 0; i < _styleControllerList.Count; i++)
        {
          if (_styleControllerList[i] != null && object.ReferenceEquals(_styleControllerList[i].ViewObject, e.OldInstance))
          {
            if (!_styleControllerList[i].Apply())
              return;

            DistributeStyleChange(i);
          }
        }
    }


   

    /// <summary>
    /// This distributes changes made to one of the sub plot styles to all other plot styles. Additionally, the controller
    /// for this styles are also updated.
    /// </summary>
    /// <param name="pivotelement"></param>
    void DistributeStyleChange(int pivotelement)
    {
      IPlotArea layer = DocumentPath.GetRootNodeImplementing<IPlotArea>(_doc);
      _tempdoc.Style.DistributeSubStyleChange(pivotelement,layer,_doc.GetRangesAndPoints(layer));

      // now all style controllers must be updated
      for (int i = 0; i < _styleControllerList.Count; i++)
      {
        if(null!=_styleControllerList[i])
					_styleControllerList[i].InitializeDocument(_tempdoc.Style[i]);
      }

      if (_additionalPlotStyle != null && _additionalPlotStyleController!=null)
      {
        _tempdoc.Style.PrepareNewSubStyle(_additionalPlotStyle, layer, _doc.GetRangesAndPoints(layer));
        _additionalPlotStyleController.InitializeDocument(_additionalPlotStyle);
      }
    }

    #region IMVCController Members

		public object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				if (null != _view)
				{
					_view.SelectedPage_Changed -= EhView_ActiveChildControlChanged;
				}

				_view = value as IG2DPlotItemView;
				if (null != _view)
				{
					Initialize(false);
					_view.SelectedPage_Changed += EhView_ActiveChildControlChanged;
				}
			}
		}

    public object ModelObject
    {
      get
      {
        return _doc;
      }
    }

    #endregion

    #region IApplyController Members

    int _applySuspend; // to avoid multiple invoking here because some of the child controls
    // have this here as controller too     
    public bool Apply()
    {
      if(_applySuspend++ > 0)
      {
        _applySuspend--;
        return true;
      }

      bool applyResult = false;


			if (!_dataController.Apply())
				return false;

			for(int i=0;i<_styleControllerList.Count;++i)

			{
				if (false == _styleControllerList[i].Apply())
        {
          _view.BringTabToFront(i);
          applyResult = false;
          goto end_of_function;
        }
      }

      if(!object.ReferenceEquals(_doc,_tempdoc))
        _doc.CopyFrom(_tempdoc);

      ApplyPlotGroupView();

      applyResult = true;

      end_of_function:
        _applySuspend--;
      return applyResult;
    }

    #endregion

   
  
    


   

    /// <summary>
    /// Returns the tab index of the first style that is shown.
    /// </summary>
    /// <returns>Tab index of the first shown style.</returns>
    private int GetFirstStyleTabIndex()
    {
      int result = 0;
      if (_dataController != null)
        ++result;
     
      return result;
    }

    private void _styleCollectionController_CollectionChangeCommit(object sender, EventArgs e)
    {
      if(true==_styleCollectionController.Apply())
      {
				InitializeStyleControllerList();
				View_SetAllTabViews();
      }
    }

		void _styleCollectionController_StyleEditRequested(int styleIndex)
		{
			if (null != _view)
			{
				_view.BringTabToFront(styleIndex + GetFirstStyleTabIndex());
			}
		}

	
	}
}
