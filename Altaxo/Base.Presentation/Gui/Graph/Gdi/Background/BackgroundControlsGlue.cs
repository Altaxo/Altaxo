#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

#endregion Copyright

using Altaxo.Drawing;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Background;
using Altaxo.Gui.Common.Drawing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Graph.Gdi.Background
{
  public class BackgroundControlsGlue : FrameworkElement
  {
    public BackgroundControlsGlue()
    {
    }

    #region IBackgroundStyle

    private IBackgroundStyle _doc;

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

    /// <summary>
    /// Occurs when the background style instance changed to another instance. This event is <b>not</b> fired when only members of the background style changed (e.g. the brush).
    /// </summary>
    public event EventHandler BackgroundStyleChanged;

    protected virtual void OnBackgroundStyleChanged()
    {
      if (BackgroundStyleChanged != null)
        BackgroundStyleChanged(this, EventArgs.Empty);
    }

    #endregion IBackgroundStyle

    #region Style

    private System.Type[] _backgroundStyles = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IBackgroundStyle));
    private ComboBox _cbStyle;

    public ComboBox CbStyle
    {
      get { return _cbStyle; }
      set
      {
        if (_cbStyle != null)
          _cbStyle.SelectionChanged -= EhStyle_SelectionChangeCommitted;

        _cbStyle = value;

        if (_cbStyle != null)
        {
          InitializeBackgroundStyle();
          _cbStyle.SelectionChanged += EhStyle_SelectionChangeCommitted;
        }
      }
    }

    private void EhStyle_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_cbStyle.SelectedIndex > 0)
        _doc = (IBackgroundStyle)Activator.CreateInstance(this._backgroundStyles[_cbStyle.SelectedIndex - 1]);
      else
        _doc = null;

      // Apply the currently selected brush to the newly created instance
      if (_doc != null && _cbBrush != null)
        _doc.Brush = _cbBrush.SelectedBrush;

      OnBackgroundStyleChanged();
      UpdateBrushState();
    }

    private void InitializeBackgroundStyle()
    {
      int sel = Array.IndexOf(this._backgroundStyles, this._doc == null ? null : this._doc.GetType());
      string[] names = Current.Gui.GetUserFriendlyClassName(this._backgroundStyles, true);
      _cbStyle.Items.Clear();
      //_cbStyle.Items.Add("<none>");
      foreach (string name in names)
        _cbStyle.Items.Add(name);

      _cbStyle.SelectedIndex = sel + 1;
    }

    #endregion Style

    #region Brush

    /// <summary>
    /// Occurs when the background brush changed.
    /// </summary>
    public event EventHandler BackgroundBrushChanged;

    protected virtual void OnBackgroundBrushChanged()
    {
      if (BackgroundBrushChanged != null)
        BackgroundBrushChanged(this, EventArgs.Empty);
    }

    private BrushComboBox _cbBrush;

    public BrushComboBox CbBrush
    {
      get { return _cbBrush; }
      set
      {
        if (_cbBrush != null)
        {
          _cbBrush.SelectedBrushChanged -= EhBrush_SelectionChangeCommitted;
        }

        _cbBrush = value;
        _cbBrush.ShowPlotColorsOnly = _showPlotColorsOnly;
        _cbBrush.SelectedBrush = new BrushX(NamedColors.Aqua);

        if (_doc != null && _cbBrush != null && _doc.Brush != null)
          _cbBrush.SelectedBrush = _doc.Brush;

        if (_cbBrush != null)
        {
          _cbBrush.SelectedBrushChanged += EhBrush_SelectionChangeCommitted;
        }

        UpdateBrushState();
      }
    }

    private void EhBrush_SelectionChangeCommitted(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (_doc != null)
      {
        _doc.Brush = _cbBrush.SelectedBrush;
        OnBackgroundBrushChanged();
      }
    }

    private Control _lblBrush;

    public Control LabelBrush
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

    private void UpdateBrushState()
    {
      bool vis = _doc != null && _doc.SupportsBrush;

      if (_cbBrush != null)
        _cbBrush.IsEnabled = vis;
      if (_lblBrush != null)
        _lblBrush.IsEnabled = vis;
    }

    #endregion Brush

    #region ShowPlotColorsOnly

    private bool _showPlotColorsOnly;

    public bool ShowPlotColorsOnly
    {
      get { return _showPlotColorsOnly; }
      set
      {
        _showPlotColorsOnly = value;
        if (null != _cbBrush)
          _cbBrush.ShowPlotColorsOnly = value;
      }
    }

    #endregion ShowPlotColorsOnly
  }
}
