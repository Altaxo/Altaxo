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
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using System.Drawing;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Background;
using Altaxo.Gui.Common.Drawing;

namespace Altaxo.Gui.Graph
{
  public class BackgroundControlsGlue : Component
  {


    public BackgroundControlsGlue()
    {

    }

    #region IBackgroundStyle

    IBackgroundStyle _doc;
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IBackgroundStyle BackgroundStyle
    {
      get
      {
        return _doc;
      }
      set
      {
        _doc = value;

        CbStyle = _cbStyle;
        CbBrush = _cbBrush;
      }
    }

    public event EventHandler BackgroundStyleChanged;
    protected virtual void OnBackgroundStyleChanged()
    {
      if (BackgroundStyleChanged != null)
        BackgroundStyleChanged(this, EventArgs.Empty);
    }

    #endregion



    #region Style

    System.Type[] _backgroundStyles = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IBackgroundStyle));
    System.Windows.Forms.ComboBox _cbStyle;
    public System.Windows.Forms.ComboBox CbStyle
    {
      get { return _cbStyle; }
      set
      {
        if (_cbStyle != null)
          _cbStyle.SelectionChangeCommitted -= EhStyle_SelectionChangeCommitted;

        _cbStyle = value;

        if (_cbStyle != null)
        {
          InitializeBackgroundStyle();
          _cbStyle.SelectionChangeCommitted += EhStyle_SelectionChangeCommitted;
        }
      }
    }

    void EhStyle_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_cbStyle.SelectedIndex>0)
        _doc = (IBackgroundStyle)Activator.CreateInstance(this._backgroundStyles[_cbStyle.SelectedIndex - 1]);
      else
        _doc = null;

      OnBackgroundStyleChanged();
      UpdateBrushState();
      
    }

    void InitializeBackgroundStyle()
    {
      if (this.DesignMode)
        return;

      int sel = Array.IndexOf(this._backgroundStyles, this._doc == null ? null : this._doc.GetType());
      string[] names = Current.Gui.GetUserFriendlyClassName(this._backgroundStyles, true);
      _cbStyle.Items.Clear();
      //_cbStyle.Items.Add("<none>");
      foreach(string name in names)
        _cbStyle.Items.Add(name);

      _cbStyle.SelectedIndex = sel+1;
    }



    #endregion

    #region Brush

    BrushColorComboBox _cbBrush;
    public BrushColorComboBox CbBrush
    {
      get { return _cbBrush; }
      set
      {
        if (_cbBrush != null)
        {
          _cbBrush.SelectionChangeCommitted -= EhBrush_SelectionChangeCommitted;
        }

        _cbBrush = value;
        if (_doc != null && _cbBrush != null)
          _cbBrush.Brush = _doc.Brush;

        if (_cbBrush != null)
        {
          _cbBrush.SelectionChangeCommitted += EhBrush_SelectionChangeCommitted;
        }

        UpdateBrushState();
      }
    }

    void EhBrush_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_doc != null)
      {
        _doc.Brush = _cbBrush.Brush;
        OnBackgroundStyleChanged();
      }
    }

    System.Windows.Forms.Control _lblBrush;
    public System.Windows.Forms.Control LabelBrush
    {
      get
      {
        return _lblBrush;
      }
      set
      {
        _lblBrush = value;
        UpdateBrushState();
      }
    }

    void UpdateBrushState()
    {
      bool vis = _doc != null && _doc.SupportsBrush;

      if(_cbBrush!=null)
      _cbBrush.Enabled = vis;
    if (_lblBrush != null)
      _lblBrush.Enabled = vis;
    }

    #endregion




  }
}
