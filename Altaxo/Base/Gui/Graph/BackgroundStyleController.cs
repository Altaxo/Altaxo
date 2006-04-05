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
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Altaxo.Calc;
using Altaxo.Main.GUI;
using Altaxo.Graph;
using Altaxo.Graph.BackgroundStyles;

namespace Altaxo.Gui.Graph
{
  #region Interfaces
  public interface IBackgroundStyleViewEventSink
  {
    /// <summary>
    /// Called if the background color is changed.
    /// </summary>
    /// <param name="newValue">The new selected item of the combo box.</param>
    void EhView_BackgroundColorChanged(Color newValue);

    /// <summary>
    /// Called if the background style changed.
    /// </summary>
    /// <param name="newValue">The new index of the style.</param>
    void EhView_BackgroundStyleChanged(int newValue);
  }

  public interface IBackgroundStyleView
  {

    /// <summary>
    /// Get/sets the controller of this view.
    /// </summary>
    IBackgroundStyleViewEventSink Controller { get; set; }

    /// <summary>
    /// Initializes the content of the background color combo box.
    /// </summary>
    void BackgroundColor_Initialize(System.Drawing.Color color);

    /// <summary>
    /// Initializes the enable state of the background color combo box.
    /// </summary>
    void BackgroundColorEnable_Initialize(bool enable);

    /// <summary>
    /// Initializes the background styles.
    /// </summary>
    /// <param name="names"></param>
    /// <param name="selection"></param>
    void BackgroundStyle_Initialize(string[] names, int selection);
  }



  #endregion


  /// <summary>
  /// Controls a IBackgroundStyle instance.
  /// </summary>
  [UserControllerForObject(typeof(IBackgroundStyle))]
  public class BackgroundStyleController : IBackgroundStyleViewEventSink, Main.GUI.IMVCAController
  {
    IBackgroundStyleView _view;
    IBackgroundStyle _doc;
    IBackgroundStyle _tempDoc;

    protected System.Type[] _backgroundStyles;

    public event EventHandler TemporaryModelObjectChanged;

    public BackgroundStyleController(IBackgroundStyle doc)
    {
      _doc = doc;
      _tempDoc = _doc==null ? null : (IBackgroundStyle)doc.Clone();
      Initialize(true);
    }

    void Initialize(bool bInit)
    {
      if (bInit)
      {
        _backgroundStyles = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(Altaxo.Graph.BackgroundStyles.IBackgroundStyle));
      }

      if (null != _view)
      {
        InitializeBackgroundStyle();
      }
    }

    void InitializeBackgroundStyle()
    {
      int sel = Array.IndexOf(this._backgroundStyles, this._tempDoc == null ? null : this._tempDoc.GetType());
      _view.BackgroundStyle_Initialize(Current.Gui.GetUserFriendlyClassName(this._backgroundStyles, true), sel + 1);

      if (this._tempDoc != null && this._tempDoc.SupportsColor)
        _view.BackgroundColor_Initialize(this._tempDoc.Color);
      else
        _view.BackgroundColor_Initialize(Color.Transparent);

      _view.BackgroundColorEnable_Initialize(this._tempDoc != null && this._tempDoc.SupportsColor);

    }


    #region IMVCController Members

    public object ViewObject
    {
      get { return _view; }
      set
      {
        if (_view != null)
          _view.Controller = null;

        _view = value as IBackgroundStyleView;

        Initialize(false);

        if (_view != null)
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

    public object TemporaryModelObject
    {
      get
      {
        return _tempDoc;
      }
    }


    #endregion

    #region IApplyController Members

    public bool Apply()
    {
      _doc = _tempDoc;
      return true;
    }

    #endregion

    #region IPlotRangeViewEventSink Members

    public void EhView_BackgroundColorChanged(System.Drawing.Color color)
    {
      if (this._tempDoc != null && this._tempDoc.SupportsColor)
      {
        this._tempDoc.Color = color;
        OnTemporaryModelObjectChanged();
      }
    }

    /// <summary>
    /// Called if the background style changed.
    /// </summary>
    /// <param name="newValue">The new index of the style.</param>
    public void EhView_BackgroundStyleChanged(int newValue)
    {

      Color backgroundColor = Color.Transparent;

      if (newValue != 0)
      {
        _tempDoc = (Altaxo.Graph.BackgroundStyles.IBackgroundStyle)Activator.CreateInstance(this._backgroundStyles[newValue - 1]);
        backgroundColor = _tempDoc.Color;
        
      }
      else // is null
      {
        _tempDoc = null;
        
      }

      if (_tempDoc != null && _tempDoc.SupportsColor)
      {
        _view.BackgroundColor_Initialize(backgroundColor);
        _view.BackgroundColorEnable_Initialize(true);
      }
      else
      {
        _view.BackgroundColorEnable_Initialize(false);
      }

      OnTemporaryModelObjectChanged();

    }

    void OnTemporaryModelObjectChanged()
    {
      if (TemporaryModelObjectChanged != null)
        TemporaryModelObjectChanged(this, EventArgs.Empty);
    }
    #endregion
  }
}
