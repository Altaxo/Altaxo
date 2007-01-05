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
using Altaxo.Graph.GUI;
using Altaxo.Serialization;

using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Rescaling;

namespace Altaxo.Gui.Graph.Scales.Rescaling
{
  /// <summary>
  /// Summary description for NumericAxisRescaleController.
  /// </summary>
  [UserControllerForObject(typeof(LogarithmicAxisRescaleConditions),1)]
  public class LogarithmicAxisRescaleController 
    :
    NumericAxisRescaleController
  {
    public LogarithmicAxisRescaleController(LogarithmicAxisRescaleConditions doc, NumericalScale ax)
      : base(doc,ax)
    {
    }
 


    #region IOrgEndSpanControlEventReceiver Members

    public override bool EhValue1Changed(string txt)
    {
      if(!GUIConversion.IsDouble(txt))
        return true;

      double val;
      GUIConversion.IsDouble(txt,out val);
      if(val>0) 
      {
        _org = val;
        _orgChanged = true;
      }
      return val<=0;
    }

    public override bool EhValue2Changed(string txt)
    {
      if(!GUIConversion.IsDouble(txt))
        return true;

      double val;
      GUIConversion.IsDouble(txt,out val);
      if(val>0)
      {
        _end = val;
        _endChanged = true;
      }
      return val<=0;
    }

    public override bool EhValue3Changed(string txt)
    {
      if(!GUIConversion.IsDouble(txt))
        return true;

      double val;
      GUIConversion.IsDouble(txt,out val);
      if(val>0) 
        _span = val;
      return val<=0;
    }

    #endregion

  
  }
}
