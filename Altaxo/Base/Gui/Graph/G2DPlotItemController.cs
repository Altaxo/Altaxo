#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
using Altaxo.Gui.Common;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Gdi.Plot.Groups;
using Altaxo.Graph.PlotGroups;
using Altaxo.Main.GUI;
using System.Collections;

namespace Altaxo.Gui.Graph
{
  
  /// <summary>
  /// Summary description for XYColumnPlotItemController.
  /// </summary>
  [UserControllerForObject(typeof(G2DPlotItem))]
  public class G2DPlotItemController : TabbedElementController, Main.GUI.IMVCAController, IXYPlotGroupViewEventSink
  {
    G2DPlotItem _doc;
    G2DPlotItem _tempdoc;
    G2DPlotGroupStyleCollection _groupStyles;
    
    IG2DPlotStyle _additionalPlotStyle;
    int _insertAdditionalPlotStyle=-1;
    IXYPlotGroupView _plotGroupView;

    IXYPlotStyleCollectionController _styleCollectionController;
    public G2DPlotItemController(G2DPlotItem doc)
      : this(doc,null)
    {
    }
    public G2DPlotItemController(G2DPlotItem doc, G2DPlotGroupStyleCollection parent)
    {
      _groupStyles = parent;
      _doc = doc;
      _tempdoc = (G2DPlotItem)_doc.Clone();

      InitializeCollectionAndData();
      InitializeStyles();
      BringTabToFront(2);
    }

    void InitializeCollectionAndData()
    {

      _styleCollectionController = (IXYPlotStyleCollectionController)Current.Gui.GetControllerAndControl(new object[]{_tempdoc.Style},typeof(IXYPlotStyleCollectionController));
      AddTab("Styles",_styleCollectionController,_styleCollectionController.ViewObject);
      _styleCollectionController.CollectionChangeCommit += new EventHandler(_styleCollectionController_CollectionChangeCommit);

      InitializePlotGroupView();
     

      IMVCAController ctrl = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _tempdoc.DataObject, _tempdoc }, typeof(IMVCAController));
      if(ctrl!=null)
        AddTab("Data", ctrl, ctrl.ViewObject);
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
      }

      if (bStandard && _groupStyles!=null)
      {
        _plotGroupView = (IXYPlotGroupView)Current.Gui.GetControl(this, typeof(IXYPlotGroupView));
        _plotGroupView.InitializePlotGroupConditions(
            color,
            linestyle,
            symbol,
            !bSerial, //_parentPlotGroup.ChangeStylesConcurrently,
            PlotGroupStrictness.Normal //_parentPlotGroup.ChangeStylesStrictly
            );
      }
    }

    void ApplyPlotGroupView()
    {
      if (null == _groupStyles || null == _plotGroupView)
        return;


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



      // now distribute the new style to the other plot items
      if (_doc.ParentCollection != null)
      {
        _doc.ParentCollection.PrepareStyles(_groupStyles);
        _doc.ParentCollection.ApplyStyles(_groupStyles, _doc);
      }
    }


    void InitializeStyles()
    {

      IMVCAController ctrl;

      // prepare the style 
      // if there is only one line style or one scatter style,
      // then add an additional scatter or line style, but set this additional style initially to have no function
      _additionalPlotStyle = null;
      IG2DPlotStyle[] lineScatterPair = new IG2DPlotStyle[2];

      if (_tempdoc.Style.Count > 0)
      {
        if (_tempdoc.Style[0] is XYPlotLineStyle && (_tempdoc.Style.Count == 1 || !(_tempdoc.Style[1] is XYPlotScatterStyle)))
        {
          XYPlotScatterStyle scatterStyle = new XYPlotScatterStyle();
          _additionalPlotStyle = scatterStyle;
          scatterStyle.Shape = Altaxo.Graph.Gdi.Plot.Styles.XYPlotScatterStyles.Shape.NoSymbol;

          _insertAdditionalPlotStyle = 1;
          lineScatterPair[0] = _tempdoc.Style[0];
          lineScatterPair[1] = _additionalPlotStyle;
        }
        else if (_tempdoc.Style[0] is XYPlotScatterStyle && (_tempdoc.Style.Count == 1 || !(_tempdoc.Style[1] is XYPlotLineStyle)))
        {
          XYPlotLineStyle lineStyle = new XYPlotLineStyle();
          _additionalPlotStyle = lineStyle;
          lineStyle.Connection = Altaxo.Graph.Gdi.Plot.Styles.XYPlotLineStyles.ConnectionStyle.NoLine;

          _insertAdditionalPlotStyle = 0;
          lineScatterPair[0] = _additionalPlotStyle;
          lineScatterPair[1] = _tempdoc.Style[0];
        }
        else if (_tempdoc.Style.Count >= 2 &&
          (
          ((_tempdoc.Style[0] is XYPlotLineStyle) && (_tempdoc.Style[1] is XYPlotScatterStyle)) ||
          ((_tempdoc.Style[0] is XYPlotScatterStyle) && (_tempdoc.Style[1] is XYPlotLineStyle))
          )
          )
        {
          lineScatterPair[0] = _tempdoc.Style[0];
          lineScatterPair[1] = _tempdoc.Style[1];
        }
        else
        {
          lineScatterPair = null;
        }
      }


      int continue_With_I = 0;
      if (lineScatterPair != null)
      {
        ArrayList arr = new ArrayList();
        for (int i = 0; i < lineScatterPair.Length; ++i)
        {
          IMVCAController ct = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { lineScatterPair[i] }, typeof(IMVCAController));
          if(ct!=null)
          {
            arr.Add(new ControlViewElement(string.Empty,ct));
            if(i==_insertAdditionalPlotStyle)
            {
              if(ct is IXYPlotLineStyleController)
                (ct as IXYPlotLineStyleController).SetEnableDisableMain(true);
              else if (ct is IXYPlotScatterStyleController)
                (ct as IXYPlotScatterStyleController).SetEnableDisableMain(true);
            }
          }
        }
        arr.Add(new ControlViewElement(string.Empty,this,this._plotGroupView));
        Common.MultiChildController mctrl = new Common.MultiChildController((ControlViewElement[])arr.ToArray(typeof(ControlViewElement)), true);
        Current.Gui.GetControl(mctrl);

        AddTab(_additionalPlotStyle == null ? "Style 1&&2" : "Style 1", mctrl, mctrl.ViewObject);

        continue_With_I = _additionalPlotStyle == null ? 2 : 1;
      }


      for (int i = continue_With_I; i < _tempdoc.Style.Count; i++)
      {
        ctrl = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _tempdoc.Style[i] }, typeof(IMVCAController));
        AddTab("Style " + (i + 1).ToString(), ctrl, ctrl!=null ? ctrl.ViewObject : null);
      }

      base.SetElements(false);
    }


    #region IMVCController Members

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

      bool bAdditionalStyleIsVisible = false;
      if (_additionalPlotStyle is XYPlotScatterStyle)
        bAdditionalStyleIsVisible = ((XYPlotScatterStyle)_additionalPlotStyle).IsVisible;
      if (_additionalPlotStyle is XYPlotLineStyle)
        bAdditionalStyleIsVisible = ((XYPlotLineStyle)_additionalPlotStyle).IsVisible;

      if (bAdditionalStyleIsVisible)
      {
        _tempdoc.Style.Insert(_insertAdditionalPlotStyle,_additionalPlotStyle);
      }

      _doc.CopyFrom(_tempdoc);

      ApplyPlotGroupView();

      applyResult = true;

      end_of_function:
        _applySuspend--;
      return applyResult;
    }

    #endregion

    #region IXYPlotGroupViewEventSink Members

    public void EhView_PlotGroupIndependent_Changed(bool bPlotGroupIsIndependent)
    {
      
    }

    #endregion

    #region Helper Controller classes

    class MyPlotStyleCollectionController : XYPlotStyleCollectionController
    {
      G2DPlotItemController _parent;

      public MyPlotStyleCollectionController(G2DPlotItemController parent)
        : base(parent._tempdoc.Style)
      {
        _parent = parent;
      }

    }

    


    #endregion

    private void _styleCollectionController_CollectionChangeCommit(object sender, EventArgs e)
    {
      if(true==_styleCollectionController.Apply())
      {
        // remove the tabs 2..
        RemoveTabRange(2,TabCount-2);
        InitializeStyles();

      }
    }
  }
}
