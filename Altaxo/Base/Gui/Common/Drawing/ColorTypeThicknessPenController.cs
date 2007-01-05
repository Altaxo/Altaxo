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

namespace Altaxo.Gui.Common.Drawing
{
  #region interfaces

  public interface IColorTypeThicknessPenView
  {
    IColorTypeThicknessPenViewEventSink Controller { get; set; }
    PenX DocPen  { get; set; }
  }

  public interface IColorTypeThicknessPenViewEventSink
  {
    void EhView_ShowFullPenDialog();
  }

  public interface IColorTypeThicknessPenController : IColorTypeThicknessPenViewEventSink, IMVCAController
  {
  }

  #endregion
  /// <summary>
  /// Summary description for ColorTypeWidthPenController.
  /// </summary>
  public class ColorTypeThicknessPenController : IColorTypeThicknessPenController
  {
    PenX _doc;
    PenX _tempDoc;
    IColorTypeThicknessPenView _view;

    public ColorTypeThicknessPenController(PenX doc)
    {
      if(doc == null) throw new ArgumentNullException("doc");
      _doc = doc;
      _tempDoc = (PenX)doc.Clone();
    }


    void Initialize()
    {
      if(_view!=null)
      {
        _view.DocPen = _tempDoc;
      }
    }
    #region IColorTypeThicknessPenViewEventSink Members

   

    public void EhView_ShowFullPenDialog()
    {
      PenAllPropertiesControl ctrl = new PenAllPropertiesControl();
      ctrl.Pen = _tempDoc;
      Current.Gui.ShowDialog(ctrl, "Pen properties");
    }

    #endregion

    #region IMVCController Members

    public object ViewObject
    {
      get
      {
        
        return _view;
      }
      set
      {
        if(_view!=null)
          _view.Controller = null;

        _view = value as IColorTypeThicknessPenView;
        
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
      _doc.CopyFrom(_tempDoc);
      return true;
    }

    #endregion
  }
}
