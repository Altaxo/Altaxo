using System;
using Altaxo.Graph.GUI;
using Altaxo.Serialization;
using Altaxo.Main.GUI;

namespace Altaxo.Graph.Axes.Scaling
{
  /// <summary>
  /// Summary description for NumericAxisRescaleController.
  /// </summary>
  [UserControllerForObject(typeof(LogarithmicAxisRescaleConditions),1)]
  public class LogarithmicAxisRescaleController 
    :
    NumericAxisRescaleController
  {
    public LogarithmicAxisRescaleController(LogarithmicAxisRescaleConditions doc, NumericalAxis ax)
      : base(doc,ax)
    {
    }
 


    #region IOrgEndSpanControlEventReceiver Members

    public override bool EhValue1Changed(string txt)
    {
      if(!NumberConversion.IsNumeric(txt))
        return true;

      double val;
      NumberConversion.IsDouble(txt,out val);
      if(val>0) 
      {
        _org = val;
        _orgChanged = true;
      }
      return val<=0;
    }

    public override bool EhValue2Changed(string txt)
    {
      if(!NumberConversion.IsNumeric(txt))
        return true;

      double val;
      NumberConversion.IsDouble(txt,out val);
      if(val>0)
      {
        _end = val;
        _endChanged = true;
      }
      return val<=0;
    }

    public override bool EhValue3Changed(string txt)
    {
      if(!NumberConversion.IsNumeric(txt))
        return true;

      double val;
      NumberConversion.IsDouble(txt,out val);
      if(val>0) 
        _span = val;
      return val<=0;
    }

    #endregion

  
    }
}
