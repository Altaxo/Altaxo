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
    
    I2DPlotStyle _additionalStyle;
    int _insertAdditionalStyle;
    IXYPlotGroupView _plotGroupView;


		public XYColumnPlotItemController(XYColumnPlotItem doc)
		{
			_doc = doc;
      Initialize();
    }

    void Initialize()
    {

      _plotGroupView = (IXYPlotGroupView)Current.Gui.GetControl(this,typeof(IXYPlotGroupView));

     

      IMVCAController ctrl = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _doc.Data, _doc }, typeof(IMVCAController));
      AddTab("Data", ctrl, ctrl.ViewObject);

      // prepare the style 
      // if there is only one line style or one scatter style,
      // then add an additional scatter or line style, but set this additional style initially to have no function
      _additionalStyle = null;
      I2DPlotStyle[] lineScatterPair = new I2DPlotStyle[2];

      if (_doc.Style.Count > 0)
      {
        if (_doc.Style[0] is XYPlotLineStyle && (_doc.Style.Count == 1 || !(_doc.Style[1] is XYPlotScatterStyle)))
        {
          XYPlotScatterStyle scatterStyle = new XYPlotScatterStyle();
          _additionalStyle = scatterStyle;
          scatterStyle.Shape = Altaxo.Graph.XYPlotScatterStyles.Shape.NoSymbol;

          _insertAdditionalStyle = 1;
          lineScatterPair[0] = _doc.Style[0];
          lineScatterPair[1] = _additionalStyle;
        }
        else if (_doc.Style[0] is XYPlotScatterStyle && (_doc.Style.Count == 1 || !(_doc.Style[1] is XYPlotLineStyle)))
        {
          XYPlotLineStyle lineStyle = new XYPlotLineStyle();
          _additionalStyle = lineStyle;
          lineStyle.Connection = Altaxo.Graph.XYPlotLineStyles.ConnectionStyle.NoLine;

          _insertAdditionalStyle = 0;
          lineScatterPair[0] = _additionalStyle;
          lineScatterPair[1] = _doc.Style[0];
        }
        else if (_doc.Style.Count >= 2 &&
          (
          ((_doc.Style[0] is XYPlotLineStyle) && (_doc.Style[1] is XYPlotScatterStyle)) ||
            ((_doc.Style[0] is XYPlotScatterStyle) && (_doc.Style[1] is XYPlotLineStyle))
            )
            )
        {
          lineScatterPair[0] = _doc.Style[0];
          lineScatterPair[1] = _doc.Style[1];
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


      for (int i = continue_With_I; i < _doc.Style.Count; i++)
      {
        ctrl = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _doc.Style[i] }, typeof(IMVCAController));
        AddTab("Style " + (i + 1).ToString(), ctrl, ctrl.ViewObject);
      }
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

    public bool Apply()
    {
      for(int i=0;i<TabCount;i++)
      {
        if(false==Tab(i).Controller.Apply())
        {
          BringTabToFront(i);
          return false;
        }
      }

      bool bAdditionalStyleIsVisible = false;
      if (_additionalStyle is XYPlotScatterStyle)
        bAdditionalStyleIsVisible = ((XYPlotScatterStyle)_additionalStyle).IsVisible;
      if (_additionalStyle is XYPlotLineStyle)
        bAdditionalStyleIsVisible = ((XYPlotLineStyle)_additionalStyle).IsVisible;

      if (bAdditionalStyleIsVisible)
      {
        _doc.Style.Insert(_insertAdditionalStyle,_additionalStyle);
      }

      return true;
    }

    #endregion

    #region IXYPlotGroupViewEventSink Members

    public void EhView_PlotGroupIndependent_Changed(bool bPlotGroupIsIndependent)
    {
      throw new Exception("The method or operation is not implemented.");
    }

    #endregion
}
}
