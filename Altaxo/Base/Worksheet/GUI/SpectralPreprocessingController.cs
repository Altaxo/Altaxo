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
	/// Controls the SpectralPreprocessingControl GUI for choosing <see>SpectralPreprocessingOptions</see>
	/// </summary>
	public class SpectralPreprocessingController : Main.GUI.IApplyController
	{
    SpectralPreprocessingControl _view;
    SpectralPreprocessingOptions _doc;
    


		public SpectralPreprocessingController(SpectralPreprocessingOptions doc)
		{
      _doc = doc;
    }

    void SetElements(bool bInit)
    {
      if(null!=_view)
      {
        _view.InitializeMethod(_doc.Method);
        _view.InitializeDetrending(_doc.DetrendingOrder);
        _view.InitializeEnsembleScale(_doc.EnsembleScale);
      }
    }


    public SpectralPreprocessingControl View
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

    #region IApplyController Members

    public bool Apply()
    {
      // TODO:  Add SpectralPreprocessingController.Apply implementation
      return false;
    }

    #endregion
  }
}
