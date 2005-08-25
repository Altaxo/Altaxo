using System;
using Altaxo.Gui.Common;
using Altaxo.Graph;
using Altaxo.Main.GUI;
using System.Collections;

namespace Altaxo.Gui.Graph
{
	/// <summary>
	/// Summary description for XYColumnPlotItemController.
	/// </summary>
	[UserControllerForObject(typeof(XYColumnPlotItem))]
	public class XYColumnPlotItemController : TabbedElementController, Main.GUI.IMVCAController, IXYPlotGroupViewEventSink
	{
    XYColumnPlotItem _doc;
    XYColumnPlotItem _tempdoc;
    PlotGroup _parentPlotGroup;
    
    I2DPlotStyle _additionalStyle;
    int _insertAdditionalStyle;
    IXYPlotGroupView _plotGroupView;

    IXYPlotStyleCollectionController _styleCollectionController;
    public XYColumnPlotItemController(XYColumnPlotItem doc)
     : this(doc,null)
    {
    }
		public XYColumnPlotItemController(XYColumnPlotItem doc, PlotGroup parent)
		{
      _parentPlotGroup = parent;
			_doc = doc;
      _tempdoc = (XYColumnPlotItem)_doc.Clone();

      InitializeCollectionAndData();
      InitializeStyles();
    }

    void InitializeCollectionAndData()
    {

      _styleCollectionController = (IXYPlotStyleCollectionController)Current.Gui.GetControllerAndControl(new object[]{_tempdoc.Style},typeof(IXYPlotStyleCollectionController));
      AddTab("Styles",_styleCollectionController,_styleCollectionController.ViewObject);
      _styleCollectionController.CollectionChangeCommit += new EventHandler(_styleCollectionController_CollectionChangeCommit);

      _plotGroupView = (IXYPlotGroupView)Current.Gui.GetControl(this,typeof(IXYPlotGroupView));
      if (_parentPlotGroup != null)
        _plotGroupView.InitializePlotGroupConditions(
          0 != (_parentPlotGroup.Style & PlotGroupStyle.Color),
          0 != (_parentPlotGroup.Style & PlotGroupStyle.Line),
          0 != (_parentPlotGroup.Style & PlotGroupStyle.Symbol),
          _parentPlotGroup.ChangeStylesConcurrently,
          _parentPlotGroup.ChangeStylesStrictly
          );

     

      IMVCAController ctrl = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _tempdoc.Data, _tempdoc }, typeof(IMVCAController));
      AddTab("Data", ctrl, ctrl.ViewObject);
    }

    void InitializeStyles()
    {

     IMVCAController ctrl;

      // prepare the style 
      // if there is only one line style or one scatter style,
      // then add an additional scatter or line style, but set this additional style initially to have no function
      _additionalStyle = null;
      I2DPlotStyle[] lineScatterPair = new I2DPlotStyle[2];

      if (_tempdoc.Style.Count > 0)
      {
        if (_tempdoc.Style[0] is XYPlotLineStyle && (_tempdoc.Style.Count == 1 || !(_tempdoc.Style[1] is XYPlotScatterStyle)))
        {
          XYPlotScatterStyle scatterStyle = new XYPlotScatterStyle();
          _additionalStyle = scatterStyle;
          scatterStyle.Shape = Altaxo.Graph.XYPlotScatterStyles.Shape.NoSymbol;

          _insertAdditionalStyle = 1;
          lineScatterPair[0] = _tempdoc.Style[0];
          lineScatterPair[1] = _additionalStyle;
        }
        else if (_tempdoc.Style[0] is XYPlotScatterStyle && (_tempdoc.Style.Count == 1 || !(_tempdoc.Style[1] is XYPlotLineStyle)))
        {
          XYPlotLineStyle lineStyle = new XYPlotLineStyle();
          _additionalStyle = lineStyle;
          lineStyle.Connection = Altaxo.Graph.XYPlotLineStyles.ConnectionStyle.NoLine;

          _insertAdditionalStyle = 0;
          lineScatterPair[0] = _additionalStyle;
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
            arr.Add(new ControlViewElement(string.Empty,ct));
        }
        arr.Add(new ControlViewElement(string.Empty,this,this._plotGroupView));
        Common.MultiChildController mctrl = new Common.MultiChildController((ControlViewElement[])arr.ToArray(typeof(ControlViewElement)), true);
        Current.Gui.GetControl(mctrl);

        AddTab(_additionalStyle == null ? "Style 1&&2" : "Style 1", mctrl, mctrl.ViewObject);

        continue_With_I = _additionalStyle == null ? 2 : 1;
      }


      for (int i = continue_With_I; i < _tempdoc.Style.Count; i++)
      {
        ctrl = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _tempdoc.Style[i] }, typeof(IMVCAController));
        AddTab("Style " + (i + 1).ToString(), ctrl, ctrl.ViewObject);
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
        return true;

      bool applyResult = false;




      for(int i=0;i<TabCount;i++)
      {
        if(false==Tab(i).Controller.Apply())
        {
          BringTabToFront(i);
          applyResult = false;
          goto end_of_function;
        }
      }

      bool bAdditionalStyleIsVisible = false;
      if (_additionalStyle is XYPlotScatterStyle)
        bAdditionalStyleIsVisible = ((XYPlotScatterStyle)_additionalStyle).IsVisible;
      if (_additionalStyle is XYPlotLineStyle)
        bAdditionalStyleIsVisible = ((XYPlotLineStyle)_additionalStyle).IsVisible;

      if (bAdditionalStyleIsVisible)
      {
        _tempdoc.Style.Insert(_insertAdditionalStyle,_additionalStyle);
      }

      _doc.CopyFrom(_tempdoc);


      if (null != _parentPlotGroup)
      {
        PlotGroupStyle plotGroupStyle = 0;
          if (_plotGroupView.PlotGroupColor)
            plotGroupStyle |= PlotGroupStyle.Color;
          if (_plotGroupView.PlotGroupLineType)
            plotGroupStyle |= PlotGroupStyle.Line;
          if (_plotGroupView.PlotGroupSymbol)
            plotGroupStyle |= PlotGroupStyle.Symbol;

          _parentPlotGroup.SetPropertiesOnly(plotGroupStyle, _plotGroupView.PlotGroupConcurrently, _plotGroupView.PlotGroupStrict);
          if (_plotGroupView.PlotGroupUpdate)
            _parentPlotGroup.UpdateMembers(plotGroupStyle, _doc);
      }

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
      XYColumnPlotItemController _parent;

      public MyPlotStyleCollectionController(XYColumnPlotItemController parent)
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
