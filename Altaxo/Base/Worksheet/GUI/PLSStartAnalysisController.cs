#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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
using Altaxo.Calc.Regression.PLS;

namespace Altaxo.Worksheet.GUI
{
  /// <summary>
  /// Summary description for PLSStartAnalysisController.
  /// </summary>
  public class PLSStartAnalysisController : Altaxo.Main.GUI.IApplyController
  {
    PLSAnalysisOptions _doc;
    PLSStartAnalysisControl _view;

    public PLSStartAnalysisController(PLSAnalysisOptions options)
    {
      _doc = options;
    }

    void SetElements(bool bInit)
    {
      if(null!=_view)
      {
        _view.InitializeNumberOfFactors(_doc.MaxNumberOfFactors);
        _view.InitializeCrossPressCalculation(_doc.CrossPRESSCalculation);
      }
    }

    public PLSStartAnalysisControl View
    {
      get { return _view; }
      set 
      {
        
        if(null!=_view)
          _view.Controller = null;
        
        _view = value;

        if(null!=_view)
        {
          _view.Controller = this;
          SetElements(false); // set only the view elements, dont't initialize the variables
        }
      }
    }

    public    PLSAnalysisOptions Doc
    {
      get { return _doc; }
    }


    public void EhView_MaxNumberOfFactorsChanged(int numFactors)
    {
      _doc.MaxNumberOfFactors = numFactors;
    }
    public void EhView_CrossValidationSelected(CrossPRESSCalculationType val)
    {
      _doc.CrossPRESSCalculation = val; 
    }
    #region IApplyController Members

    public bool Apply()
    {
      // nothing to do here, since the hosted doc is a struct
      return true;
    }

    #endregion
  }
}
