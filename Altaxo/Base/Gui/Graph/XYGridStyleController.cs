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
using Altaxo.Graph;
using Altaxo.Gui.Common.Drawing;
using Altaxo.Main.GUI;

namespace Altaxo.Gui.Graph
{
  

  #region Interfaces

  public interface IXYGridStyleView
  {
    IXYGridStyleViewEventSink Controller { get; set; }
    void InitializeMajorGridStyle( IColorTypeThicknessPenController controller);
    void InitializeMinorGridStyle( IColorTypeThicknessPenController controller);

    void InitializeShowGrid(bool value);
    void InitializeShowMinorGrid(bool value);
    void InitializeShowZeroOnly(bool value);
  }

  public interface IXYGridStyleViewEventSink
  {
    void EhView_ShowGridChanged(bool newval);
    void EhView_ShowMinorGridChanged(bool newval);
    void EhView_ShowZeroOnly(bool newval);
  }

  #endregion
	/// <summary>
	/// Summary description for XYGridStyleController.
	/// </summary>
	[UserControllerForObject(typeof(Altaxo.Graph.GridStyle))]
	public class XYGridStyleController : Main.GUI.IMVCAController, IXYGridStyleViewEventSink
	{
    IXYGridStyleView _view;
    GridStyle _doc;
    GridStyle _tempdoc;
    IColorTypeThicknessPenController _majorController;
    IColorTypeThicknessPenController _minorController;

		public XYGridStyleController(GridStyle doc)
		{
      _doc = doc;
      _tempdoc = (GridStyle)doc.Clone();

      _majorController = new ColorTypeThicknessPenController(_tempdoc.MajorPen);
      _minorController = new ColorTypeThicknessPenController(_tempdoc.MinorPen);
    }

    public void Initialize()
    {
      if(_view!=null)
      {
        _view.InitializeMajorGridStyle(_majorController);
        _view.InitializeMinorGridStyle(_minorController);
        _view.InitializeShowMinorGrid(_tempdoc.ShowMinor);
        _view.InitializeShowZeroOnly(_tempdoc.ShowZeroOnly);
        _view.InitializeShowGrid(_tempdoc.ShowGrid);
      }
    }

    #region IXYGridStyleViewEventSink Members

    public void EhView_ShowGridChanged(bool newval)
    {
      _tempdoc.ShowGrid = newval;
    }

    public void EhView_ShowMinorGridChanged(bool newval)
    {
      _tempdoc.ShowMinor = newval;
    }

    public void EhView_ShowZeroOnly(bool newval)
    {
      _tempdoc.ShowZeroOnly = newval;
    }

    #endregion

    #region IMVCController Members

    public object ViewObject
    {
      get { return _view; }
      set
      {
        if(_view!=null)
          _view.Controller = null;

        _view = value as IXYGridStyleView;
        
        Initialize();

        if(_view!=null)
          _view.Controller = this;
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
      if(!this._majorController.Apply())
        return false;

      if(!this._minorController.Apply())
        return false;

      _doc.CopyFrom(_tempdoc);
      
      return true;
    }

    #endregion

 
  }
}
