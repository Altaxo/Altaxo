using System;
using Altaxo.Graph.GUI;
using Altaxo.Serialization;
using Altaxo.Main.GUI;

namespace Altaxo.Graph.Axes.Scaling
{
	/// <summary>
	/// Summary description for NumericAxisRescaleController.
	/// </summary>
	[UserControllerForObject(typeof(NumericAxisRescaleConditions))]
	public class NumericAxisRescaleController 
    :
    IOrgEndSpanControlEventReceiver,
    IMVCAController
	{
    IOrgEndSpanControl _view;
    NumericAxisRescaleConditions _doc;
    
    double _org;
    double _end;
    double _span;

    BoundaryRescaling _orgRescaling;
    BoundaryRescaling _endRescaling;
    BoundaryRescaling _spanRescaling;

  


    public NumericAxisRescaleController(NumericAxisRescaleConditions doc, Axis ax)
    {
      _doc = doc;
      
      _orgRescaling =  _doc.OrgRescaling;
      _endRescaling =  _doc.EndRescaling;
      _spanRescaling = _doc.SpanRescaling;

      _org =  _doc.Org;
      _end =  _doc.End;
      _span = _doc.Span;
    }

    public NumericAxisRescaleController(object[] o)
    {
    }

    /// <summary>
    /// Has to match the indices of BoundaryRescaling
    /// </summary>
    static readonly string[] _choices = { "Auto", "Fixed", "<=", ">=" };

    protected virtual void InitView()
    {
      _view.SetLabel1("Org:");
      _view.SetLabel2("End:");
      _view.SetLabel3("Span:");

      _view.SetChoice1(_choices, (int)_orgRescaling);
      _view.SetChoice2(_choices, (int)_endRescaling);
      _view.SetChoice3(_choices, (int)_spanRescaling);


      _view.SetValue1(NumberConversion.ToString(_org));
      _view.SetValue2(NumberConversion.ToString(_end));
      _view.SetValue3(NumberConversion.ToString(_span));

      SetEnableState();
    }

    protected virtual void SetEnableState()
    {
      bool enableSpan = _spanRescaling!=BoundaryRescaling.Auto;
      _view.EnableChoiceValue1(!enableSpan);
      _view.EnableChoiceValue2(!enableSpan);
      _view.EnableChoiceValue3(enableSpan);
    }

    #region IOrgEndSpanControlEventReceiver Members

    public void EhChoice1Changed(string txt, int selected)
    {
      _orgRescaling = (BoundaryRescaling)selected;
    }

    public void EhChoice2Changed(string txt, int selected)
    {
      _endRescaling = (BoundaryRescaling)selected;
    }

    public void EhChoice3Changed(string txt, int selected)
    {
      _spanRescaling = (BoundaryRescaling)selected;
      SetEnableState();
    }

    public bool EhValue1Changed(string txt)
    {
      if(!NumberConversion.IsNumeric(txt))
        return true;

      NumberConversion.IsDouble(txt,out _org);
      return false;
    }

    public bool EhValue2Changed(string txt)
    {
      if(!NumberConversion.IsNumeric(txt))
        return true;

      NumberConversion.IsDouble(txt,out _end);
      return false;   
    }

    public bool EhValue3Changed(string txt)
    {
      if(!NumberConversion.IsNumeric(txt))
        return true;

      NumberConversion.IsDouble(txt,out _span);
      return false;  
    }

    #endregion

    #region IMVCController Members

    public object ViewObject
    {
      get { return _view; }
      set
      {
        if(null!=_view && _view.Controller==this)
          _view.Controller=null;

        _view = (IOrgEndSpanControl)value;

        if(_view!=null)
        {
          InitView();
          _view.Controller=this;
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

    public bool Apply()
    {
      if(_spanRescaling!=BoundaryRescaling.Auto)
        _doc.SetSpan(_spanRescaling,_span);
      else
        _doc.SetOrgAndEnd(_orgRescaling,_org,_endRescaling,_end);

      return true;
    }

    #endregion
  }
}
