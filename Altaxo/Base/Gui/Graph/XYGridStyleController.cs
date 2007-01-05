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
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Axis;

using Altaxo.Gui.Common.Drawing;


namespace Altaxo.Gui.Graph
{
  #region Interfaces

  public interface IXYGridStyleView
  {
    IXYGridStyleViewEventSink Controller { get; set; }
    void InitializeBegin();
    void InitializeEnd();
    void InitializeMajorGridStyle( IColorTypeThicknessPenController controller);
    void InitializeMinorGridStyle( IColorTypeThicknessPenController controller);
    void InitializeShowGrid(bool value);
    void InitializeShowMinorGrid(bool value);
    void InitializeShowZeroOnly(bool value);
    void InitializeElementEnabling(bool majorstyle, bool minorstyle, bool showminor, bool showzeroonly);
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
  [UserControllerForObject(typeof(GridStyle))]
  [ExpectedTypeOfView(typeof(IXYGridStyleView))]
  public class XYGridStyleController : IMVCANController, IXYGridStyleViewEventSink
  {
    IXYGridStyleView _view;
    GridStyle _doc;
    GridStyle _tempdoc;
    IColorTypeThicknessPenController _majorController;
    IColorTypeThicknessPenController _minorController;

    UseDocument _useDocument;
    public UseDocument UseDocumentCopy { set { _useDocument = value; } }

    public XYGridStyleController() 
    {
    }

    public XYGridStyleController(GridStyle doc)
    {
      if (!InitializeDocument(doc))
        throw new ApplicationException("Programming error");
    }

    public bool InitializeDocument(params object[] args)
    {
      if (args.Length == 0 || (args[0]!=null && !(args[0] is GridStyle)))
        return false;

      bool isVirgin = null == _doc;
      _doc = (GridStyle)args[0];
      _tempdoc = _doc;

      if(_useDocument == UseDocument.Copy && null!=_doc)
      {
        _tempdoc = (GridStyle)_doc.Clone();
      }
      if (null == _doc)
      {
        _doc = _tempdoc = new GridStyle();
        _tempdoc.ShowGrid = false;
        _useDocument = UseDocument.Directly;
      }

      Initialize(true);
      return true;
    }

    public void Initialize(bool init)
    {
      if (init)
      {
        _majorController = new ColorTypeThicknessPenController(_tempdoc.MajorPen);
        _minorController = new ColorTypeThicknessPenController(_tempdoc.MinorPen);
      }

      if(_view!=null)
      {
        _view.InitializeBegin();
        
        _view.InitializeMajorGridStyle(_majorController);
        _view.InitializeMinorGridStyle(_minorController);
        _view.InitializeShowMinorGrid(_tempdoc.ShowMinor);
        _view.InitializeShowZeroOnly(_tempdoc.ShowZeroOnly);
        _view.InitializeShowGrid(_tempdoc.ShowGrid);
        InitializeElementEnabling();
        
        _view.InitializeEnd();
      }
    }

    public void InitializeElementEnabling()
    {
      if (_view != null)
      {
        bool majorstyle = _tempdoc.ShowGrid;
        bool showzeroonly = _tempdoc.ShowGrid;
        bool showminor = _tempdoc.ShowGrid && !_tempdoc.ShowZeroOnly;
        bool minorstyle = _tempdoc.ShowMinor && showminor;
        _view.InitializeElementEnabling(majorstyle, minorstyle, showminor, showzeroonly);
      }
    }

    #region IXYGridStyleViewEventSink Members

    public void EhView_ShowGridChanged(bool newval)
    {
      _tempdoc.ShowGrid = newval;
      InitializeElementEnabling();
    }

    public void EhView_ShowMinorGridChanged(bool newval)
    {
      _tempdoc.ShowMinor = newval;
      InitializeElementEnabling();
    }

    public void EhView_ShowZeroOnly(bool newval)
    {
      _tempdoc.ShowZeroOnly = newval;
      if (newval == true && _tempdoc.ShowMinor)
      {
        _tempdoc.ShowMinor = false;
        _view.InitializeShowMinorGrid(_tempdoc.ShowMinor);
      }
      InitializeElementEnabling();
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
        
        Initialize(false);

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

      if (_useDocument == UseDocument.Copy)
      {
        _doc.CopyFrom(_tempdoc);
      }
      
      return true;
    }

    #endregion

 
  }
}
