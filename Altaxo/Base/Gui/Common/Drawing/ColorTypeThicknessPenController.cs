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

namespace Altaxo.Gui.Common.Drawing
{
  #region interfaces

  public interface IColorTypeThicknessPenView
  {
    IColorTypeThicknessPenViewEventSink Controller { get; set; }
    void InitializeColors(string[] names, int selection);
    void InitializeLineType(string[] names, int selection);
    void InitializeLineWidth(string[] names, string selection);
  }

  public interface IColorTypeThicknessPenViewEventSink
  {
    void EhView_ColorChanged(int selection);
    void EhView_LineTypeChanged(int selection);
    void EhView_LineWidthChanged(string value, System.ComponentModel.CancelEventArgs e);
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
        string[] names = System.Enum.GetNames(typeof(System.Drawing.KnownColor));
        string name = _tempDoc.Color.IsKnownColor ? _tempDoc.Color.Name : _tempDoc.Color.ToString();
        _view.InitializeColors(names,Array.IndexOf(names,name));


        names = System.Enum.GetNames(typeof(System.Drawing.Drawing2D.DashStyle));
        name = _tempDoc.DashStyle.ToString();
        _view.InitializeLineType(names,Array.IndexOf(names,name));


        double[] thickness = new double[]{0.2, 0.5, 1.0, 2.0, 3.0};
        names = new string[thickness.Length];
        for(int i=0;i<names.Length;++i)
          names[i] = Altaxo.Serialization.GUIConversion.ToString(thickness[i]);
        name = Altaxo.Serialization.GUIConversion.ToString(_tempDoc.Width);
        _view.InitializeLineWidth(names,name);


      }
    }
    #region IColorTypeThicknessPenViewEventSink Members

    public void EhView_ColorChanged(int selection)
    {
      System.Drawing.KnownColor cc = (System.Drawing.KnownColor)Enum.GetValues(typeof(System.Drawing.KnownColor)).GetValue(selection);
      _tempDoc.Color = System.Drawing.Color.FromKnownColor(cc);
    }

    public void EhView_LineTypeChanged(int selection)
    {
      _tempDoc.DashStyle = (System.Drawing.Drawing2D.DashStyle)selection;
    }

    public void EhView_LineWidthChanged(string value,  System.ComponentModel.CancelEventArgs e)
    {
      double w;
      if(Altaxo.Serialization.GUIConversion.IsDouble(value, out w))
        _tempDoc.Width = (float)w;
      else
        e.Cancel = true;
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
