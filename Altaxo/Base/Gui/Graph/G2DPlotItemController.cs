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
using Altaxo.Gui.Common;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Gdi.Plot.Groups;
using Altaxo.Graph.Plot.Groups;

using System.Collections;

namespace Altaxo.Gui.Graph
{
  
  /// <summary>
  /// Summary description for XYColumnPlotItemController.
  /// </summary>
  [UserControllerForObject(typeof(G2DPlotItem))]
  public class G2DPlotItemController 
    :
    TabbedElementController,
    IMVCANController, 
    IXYPlotGroupViewEventSink
  {
    UseDocument _useDocument;
    G2DPlotItem _doc;
    G2DPlotItem _tempdoc;
    PlotGroupStyleCollection _groupStyles;
    
    IG2DPlotStyle _additionalPlotStyle;
    IXYPlotGroupView _plotGroupView;

    IMVCAController _dataController;
    IMVCAController _plotGroupController;
    IXYPlotStyleCollectionController _styleCollectionController;
    List<IMVCANController> _styleControllerList = new List<IMVCANController>();
    Dictionary<IG2DPlotStyle, IMVCANController> _styleControllerDictionary = new Dictionary<IG2DPlotStyle, IMVCANController>();
    IMVCANController _additionalPlotStyleController;
    Common.MultiChildController _combinedScatterLineGroupController;
    

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
      if (args == null || args.Length == 0)
        return false;
      
      if (!(args[0] is G2DPlotItem))
        return false;
      else
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

      InitializeCollectionAndData();
      InitializeStyles();
      BringTabToFront(2);

      return true;
    }

    public UseDocument UseDocumentCopy
    {
      set { _useDocument = value; }
    }

    void InitializeCollectionAndData()
    {
      _dataController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _tempdoc.DataObject, _tempdoc }, typeof(IMVCAController));
      if (_dataController != null)
        AddTab("Data", _dataController, _dataController.ViewObject);



      InitializePlotGroupView();

      if (_plotGroupController != null)
      {
        AddTab("Grouping", _plotGroupController, _plotGroupController.ViewObject);
      }


      _styleCollectionController = (IXYPlotStyleCollectionController)Current.Gui.GetControllerAndControl(new object[] { _tempdoc.Style }, typeof(IXYPlotStyleCollectionController));
      AddTab("Styles", _styleCollectionController, _styleCollectionController.ViewObject);
      _styleCollectionController.CollectionChangeCommit += new EventHandler(_styleCollectionController_CollectionChangeCommit);
    }

    void InitializePlotGroupView()
    {
      bool bStandard = true;
      bool bSerial = false;
      bool color = false;
      bool linestyle = false;
      bool symbol = false;

      if (_groupStyles != null)
      {
        color = _groupStyles.ContainsType(typeof(ColorGroupStyle));
        linestyle = _groupStyles.ContainsType(typeof(LineStyleGroupStyle));
        symbol = _groupStyles.ContainsType(typeof(SymbolShapeStyleGroupStyle));

        if (_groupStyles.Count != (color ? 1 : 0) + (linestyle ? 1 : 0) + (symbol ? 1 : 0))
          bStandard = false;

        if(color && linestyle && typeof(LineStyleGroupStyle)==_groupStyles.GetChildTypeOf(typeof(ColorGroupStyle)))
        {
          bSerial = true;
        }
        if((linestyle && symbol) && typeof(SymbolShapeStyleGroupStyle)==_groupStyles.GetChildTypeOf(typeof(LineStyleGroupStyle)))
        {
          if (color && !bSerial)
            bStandard = false;
          else
            bSerial = true;
        }

        if (color && !((ColorGroupStyle)_groupStyles.GetPlotGroupStyle(typeof(ColorGroupStyle))).IsStepEnabled)
          bStandard = false;
        if (linestyle && !((LineStyleGroupStyle)_groupStyles.GetPlotGroupStyle(typeof(LineStyleGroupStyle))).IsStepEnabled)
          bStandard = false;
        if (symbol && !((SymbolShapeStyleGroupStyle)_groupStyles.GetPlotGroupStyle(typeof(SymbolShapeStyleGroupStyle))).IsStepEnabled)
          bStandard = false;
        if (_groupStyles.CoordinateTransformingStyle != null)
          bStandard = false;
      }

      if (bStandard && _groupStyles!=null)
      {
        _plotGroupView = (IXYPlotGroupView)Current.Gui.FindAndAttachControlTo(this, typeof(IXYPlotGroupView));
        _plotGroupView.InitializePlotGroupConditions(
            color,
            linestyle,
            symbol,
            !bSerial, //_parentPlotGroup.ChangeStylesConcurrently,
            PlotGroupStrictness.Normal //_parentPlotGroup.ChangeStylesStrictly
            );
        _plotGroupView.AdvancedPlotGroupControl += EhAdvancedPlotGroupControlRequired;
      }
      else if (_groupStyles != null)
      {
        _plotGroupController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _groupStyles }, typeof(IMVCAController));
      }
    }

    void EhAdvancedPlotGroupControlRequired(object sender, EventArgs e)
    {
      _plotGroupView.AdvancedPlotGroupControl -= EhAdvancedPlotGroupControlRequired;
      ApplyPlotGroupView();
      _plotGroupView = null;
      if (_groupStyles != null)
      {
        _plotGroupController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _groupStyles }, typeof(IMVCAController));
        // remove the tabs 1.., leaving only the style and data tab
        if (_dataController == null)
          RemoveTabRange(0, TabCount);
        else
          RemoveTabRange(1, TabCount - 1);

        AddTab("Grouping", _plotGroupController, _plotGroupController.ViewObject);
        AddTab("Styles", _styleCollectionController, _styleCollectionController.ViewObject);

        InitializeStyles();

        BringTabToFront(_dataController == null ? 0 : 1);
      }
    }

    void ApplyPlotGroupView()
    {

      if (null != _plotGroupView)
      {

        bool color = _plotGroupView.PlotGroupColor;
        bool linestyle = _plotGroupView.PlotGroupLineType;
        bool symbol = _plotGroupView.PlotGroupSymbol;
        bool serial = !_plotGroupView.PlotGroupConcurrently;

        if (_groupStyles.ContainsType(typeof(ColorGroupStyle)))
          _groupStyles.RemoveType(typeof(ColorGroupStyle));
        if (_groupStyles.ContainsType(typeof(LineStyleGroupStyle)))
          _groupStyles.RemoveType(typeof(LineStyleGroupStyle));
        if (_groupStyles.ContainsType(typeof(SymbolShapeStyleGroupStyle)))
          _groupStyles.RemoveType(typeof(SymbolShapeStyleGroupStyle));


        if (color)
        {
          _groupStyles.Add(new ColorGroupStyle());
        }
        if (linestyle)
        {
          if (serial && color)
            _groupStyles.Add(new LineStyleGroupStyle(), typeof(ColorGroupStyle));
          else
            _groupStyles.Add(new LineStyleGroupStyle());
        }
        if (symbol)
        {
          if (serial && linestyle)
            _groupStyles.Add(new SymbolShapeStyleGroupStyle(), typeof(LineStyleGroupStyle));
          else if (serial && color)
            _groupStyles.Add(new SymbolShapeStyleGroupStyle(), typeof(ColorGroupStyle));
          else
            _groupStyles.Add(new SymbolShapeStyleGroupStyle());
        }

        _groupStyles.PlotGroupStrictness = _plotGroupView.PlotGroupStrict;
      }


      // now distribute the new style to the other plot items
      if (_doc.ParentCollection != null)
      {
        _doc.ParentCollection.DistributePlotStyleFromTemplate(_doc, _groupStyles.PlotGroupStrictness);
        _doc.ParentCollection.DistributeChanges(_doc);
      }
    }

    /// <summary>
    /// Return true if we need a helper style (either line or scatter) to fill a combined tab.
    /// </summary>
    /// <returns>True if we need a helper style.</returns>
    /// <remarks>
    /// Presumption, that we add a helper style, is:
    /// - first item must be either line or scatter
    /// - second item must be neither line nor scatter
    /// - the group view must be the simple group view
    /// </remarks>
    bool NeedsHelperStyle()
    {
      bool addHelperStyle;
      addHelperStyle = (_tempdoc.Style[0] is LinePlotStyle) || (_tempdoc.Style[0] is ScatterPlotStyle);
      if (_tempdoc.Style.Count > 1)
        addHelperStyle &= !((_tempdoc.Style[1] is LinePlotStyle) || (_tempdoc.Style[1] is ScatterPlotStyle));

      return addHelperStyle;
    }

    /// <summary>
    /// Returns true if we can use a tab that combines line, scatter, and a simplified group view
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// presumption for a combined tab is:
    /// - the group view must be the simple group view
    /// -either a helper style must be added or there are at least 2 preexisting substyles, namely scatter and line
    /// </remarks>
    bool UseCombinedScatterLineGroupTab()
    {
      bool useCombinedTab;
      useCombinedTab = _plotGroupView != null;
      useCombinedTab &= (_tempdoc.Style.Count >= 2 && (_tempdoc.Style[0] is ScatterPlotStyle) && (_tempdoc.Style[1] is LinePlotStyle)) || NeedsHelperStyle();

      return useCombinedTab;
    }

    IMVCANController GetStyleController(IG2DPlotStyle style)
    {
      if (_styleControllerDictionary.ContainsKey(style))
        return _styleControllerDictionary[style];

      IMVCANController ct = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { style }, typeof(IMVCANController), UseDocument.Directly);

      if (ct != null)
        _styleControllerDictionary.Add(style, ct);

      return ct;
    }

    void InitializeStyles()
    {
      // Clear the previous controller cache
      _additionalPlotStyle = null;
      if (_combinedScatterLineGroupController != null)
      {
        _combinedScatterLineGroupController.ChildControlChanged -= EhView_ActiveChildControlChanged;
        _combinedScatterLineGroupController = null;
      }
      _styleControllerList.Clear();


      // start to create new controllers
      if (_tempdoc.Style.Count > 0)
      {
        bool addHelperStyle = NeedsHelperStyle();
        bool useCombinedTab = UseCombinedScatterLineGroupTab();


        if (useCombinedTab)
        {
          List<ControlViewElement> combList = new List<ControlViewElement>();

          // create the controllers
          IMVCANController ct1 = GetStyleController(_tempdoc.Style[0]);
          _styleControllerList.Add(ct1);


          if (addHelperStyle)
          {
            IPlotArea layer = Main.DocumentPath.GetRootNodeImplementing<IPlotArea>(_doc);
            // add either line or scatter
            if (_tempdoc.Style[0] is LinePlotStyle)
            {
              ScatterPlotStyle scatterStyle = new ScatterPlotStyle();
              scatterStyle.ParentObject = _tempdoc.Style;
              _tempdoc.Style.PrepareNewSubStyle(scatterStyle, layer, _doc.GetRangesAndPoints(layer));
              _additionalPlotStyle = scatterStyle;
              scatterStyle.Shape = Altaxo.Graph.Gdi.Plot.Styles.XYPlotScatterStyles.Shape.NoSymbol;

              _additionalPlotStyleController = GetStyleController( _additionalPlotStyle);
              combList.Add(new ControlViewElement("Symbol", _additionalPlotStyleController));
              combList.Add(new ControlViewElement("Line", ct1));
            }
            else
            {
              LinePlotStyle lineStyle = new LinePlotStyle();
              lineStyle.ParentObject = _tempdoc.Style;
              _tempdoc.Style.PrepareNewSubStyle(lineStyle, layer, _doc.GetRangesAndPoints(layer));
              _additionalPlotStyle = lineStyle;
              lineStyle.Connection = Altaxo.Graph.Gdi.Plot.Styles.XYPlotLineStyles.ConnectionStyle.NoLine;

              _additionalPlotStyleController = GetStyleController( _additionalPlotStyle );
              combList.Add(new ControlViewElement("Symbol", ct1));
              combList.Add(new ControlViewElement("Line", _additionalPlotStyleController));
            }
          }
          else // no helper style, i.e. second style is line style
          {
            // create the controllers
            IMVCANController ct2 = GetStyleController( _tempdoc.Style[1] );
            _styleControllerList.Add(ct2);
            combList.Add(new ControlViewElement("Symbol", ct1));
            combList.Add(new ControlViewElement("Line", ct2));
          }

          combList.Add(new ControlViewElement(string.Empty, this, this._plotGroupView));
          _combinedScatterLineGroupController = new Common.MultiChildController(combList.ToArray(), true);
          Current.Gui.FindAndAttachControlTo(_combinedScatterLineGroupController);
          string title;
          if(null!=_additionalPlotStyle)
            title = string.Format("#{0}:{1}", 1, Current.Gui.GetUserFriendlyClassName(_tempdoc.Style[0].GetType()));
          else
            title = "#1&&2:Symbol&&Line";
          AddTab(title, _combinedScatterLineGroupController, _combinedScatterLineGroupController.ViewObject);
          _combinedScatterLineGroupController.ChildControlChanged += this.EhView_ActiveChildControlChanged;
        } // if use CombinedTab

        // now the remaining styles
        int start = useCombinedTab ? (addHelperStyle ? 1 : 2) : 0;
        for (int i = start; i < _tempdoc.Style.Count; i++)
        {
          IMVCANController ctrl = GetStyleController( _tempdoc.Style[i] );
          _styleControllerList.Add(ctrl);
          string title = string.Format("#{0}:{1}", (i + 1),Current.Gui.GetUserFriendlyClassName(_tempdoc.Style[i].GetType()));
          AddTab(title, ctrl, ctrl != null ? ctrl.ViewObject : null);
        }
      }
      base.SetElements(false);
    }


    protected override void EhView_ActiveChildControlChanged(object sender, Main.InstanceChangedEventArgs<object> e)
    {
      // first: test if this is the view of the additional style
      if (_additionalPlotStyleController != null && object.ReferenceEquals(_additionalPlotStyleController.ViewObject, e.OldInstance))
      {
        if (!_additionalPlotStyleController.Apply())
          return;

        if (_additionalPlotStyle is LinePlotStyle && ((LinePlotStyle)_additionalPlotStyle).IsVisible)
        {
          MakeAdditionalPlotStylePermanent();
        }
        else if (_additionalPlotStyle is ScatterPlotStyle && ((ScatterPlotStyle)_additionalPlotStyle).IsVisible)
        {
          MakeAdditionalPlotStylePermanent();
        }
      }
      else
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
    }


    void MakeAdditionalPlotStylePermanent()
    {
      IG2DPlotStyle additionalPlotStyle = _additionalPlotStyle;
      _additionalPlotStyle = null;

      if (additionalPlotStyle is LinePlotStyle)
      {
        _tempdoc.Style.Insert(1, additionalPlotStyle);
        _styleControllerList.Insert(1, _additionalPlotStyleController);
        _additionalPlotStyleController = null;
        DistributeStyleChange(1);
        _styleCollectionController.InitializeDocument(_tempdoc.Style);
      }
      else if(additionalPlotStyle is ScatterPlotStyle)
      {
        _tempdoc.Style.Insert(0, additionalPlotStyle);
        _styleControllerList.Insert(0, _additionalPlotStyleController);
        _additionalPlotStyleController = null;
        DistributeStyleChange(0);
        _styleCollectionController.InitializeDocument(_tempdoc.Style);
      }
    }

    /// <summary>
    /// This distributes changes made to one of the sub plot styles to all other plot styles. Additionally, the controller
    /// for this styles are also updated.
    /// </summary>
    /// <param name="pivotelement"></param>
    void DistributeStyleChange(int pivotelement)
    {
      IPlotArea layer = Main.DocumentPath.GetRootNodeImplementing<IPlotArea>(_doc);
      _tempdoc.Style.DistributeSubStyleChange(pivotelement,layer,_doc.GetRangesAndPoints(layer));

      // now all style controllers must be updated
      for (int i = 0; i < _styleControllerList.Count; i++)
      {
        _styleControllerList[i].InitializeDocument(_tempdoc.Style[i]);
      }

      if (_additionalPlotStyle != null && _additionalPlotStyleController!=null)
      {
        _tempdoc.Style.PrepareNewSubStyle(_additionalPlotStyle, layer, _doc.GetRangesAndPoints(layer));
        _additionalPlotStyleController.InitializeDocument(_additionalPlotStyle);
      }
    }

    #region IMVCController Members

    public override object ModelObject
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
    public override bool Apply()
    {
      if(_applySuspend++ > 0)
      {
        _applySuspend--;
        return true;
      }

      bool applyResult = false;

      if (_additionalPlotStyleController != null)
      {
        if (!_additionalPlotStyleController.Apply())
        {
          applyResult = false;
          goto end_of_function;
        }

        if (_additionalPlotStyle is LinePlotStyle && ((LinePlotStyle)_additionalPlotStyle).IsVisible)
        {
          MakeAdditionalPlotStylePermanent();
        }
        else if (_additionalPlotStyle is ScatterPlotStyle && ((ScatterPlotStyle)_additionalPlotStyle).IsVisible)
        {
          MakeAdditionalPlotStylePermanent();
        }
      }



      for(int i=0;i<TabCount;i++)
      {
        if (Tab(i).Controller == null)
          continue;
        if(false==Tab(i).Controller.Apply())
        {
          BringTabToFront(i);
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
      if (_plotGroupController != null)
        ++result;
      if (_styleCollectionController != null)
        ++result;

      return result;
    }

    private void _styleCollectionController_CollectionChangeCommit(object sender, EventArgs e)
    {
      if(true==_styleCollectionController.Apply())
      {
        // remove the tabs 2..
        int firstStyle = GetFirstStyleTabIndex();
        RemoveTabRange(firstStyle,TabCount-firstStyle);
        InitializeStyles();
        BringTabToFront(GetFirstStyleTabIndex() - 1);
      }
    }

   
  }
}
