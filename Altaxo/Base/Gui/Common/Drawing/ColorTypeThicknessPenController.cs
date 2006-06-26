#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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

namespace Altaxo.Gui.Common.Drawing
{
  #region interfaces

  public interface IColorTypeThicknessPenView
  {
    IColorTypeThicknessPenViewEventSink Controller { get; set; }
    void InitializeColor(System.Drawing.Color selectedColor);
    void InitializeLineType(DashStyleEx selection);
    void InitializeLineWidth(float selection);
    ColorType ColorType  { get; set; }
  }

  public interface IColorTypeThicknessPenViewEventSink
  {
    void EhView_ColorChanged(System.Drawing.Color selection);
    void EhView_LineTypeChanged(DashStyleEx selection);
    void EhView_LineWidthChanged(float value);
    void EhView_ShowFullPenDialog();
  }

  public interface IColorTypeThicknessPenController : IColorTypeThicknessPenViewEventSink, Main.GUI.IMVCAController
  {
  }

  #endregion
  /// <summary>
  /// Summary description for ColorTypeWidthPenController.
  /// </summary>
  public class ColorTypeThicknessPenController : IColorTypeThicknessPenController
  {
    PenHolder _doc;
    PenHolder _tempDoc;
    IColorTypeThicknessPenView _view;

    public ColorTypeThicknessPenController(PenHolder doc)
    {
      if(doc == null) throw new ArgumentNullException("doc");
      _doc = doc;
      _tempDoc = (PenHolder)doc.Clone();
    }


    void Initialize()
    {
      if(_view!=null)
      {
        string[] names;
        string name;

        _view.InitializeColor(_tempDoc.Color);
        _view.InitializeLineType(_tempDoc.DashStyleEx);
        _view.InitializeLineWidth(_tempDoc.Width);


      }
    }
    #region IColorTypeThicknessPenViewEventSink Members

    public void EhView_ColorChanged(System.Drawing.Color selection)
    {
      _tempDoc.Color = selection;
    }

    public void EhView_LineTypeChanged(DashStyleEx selection)
    {
      _tempDoc.DashStyleEx = selection;
    }

    public void EhView_LineWidthChanged(float value)
    {
      _tempDoc.Width = value;
    }

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
