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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Altaxo.Drawing.ColorManagement;
using Altaxo.Graph.Gdi;
using Altaxo.Gui.Drawing;
using Altaxo.Gui.Drawing.DashPatternManagement;

namespace Altaxo.Gui.Common.Drawing
{
  public class PenControlsGlue : FrameworkElement
  {
    private bool _userChangedAbsStartCapSize;
    private bool _userChangedAbsEndCapSize;

    private bool _userChangedRelStartCapSize;
    private bool _userChangedRelEndCapSize;

    private bool _isAllPropertiesGlue;

    public PenControlsGlue()
      : this(false)
    {
    }

    public PenControlsGlue(bool isAllPropertiesGlue)
    {
      InternalSelectedPen = new PenX(ColorSetManager.Instance.BuiltinDarkPlotColors[0]);
      _isAllPropertiesGlue = isAllPropertiesGlue;
    }

    #region Pen

    private PenX _pen;

    /// <summary>
    /// Gets or sets the pen. The pen you get is a clone of the pen that is used internally. Similarly, when setting the pen, a clone is created, so that the pen
    /// can be used internally, without interfering with external functions that changes the pen.
    /// </summary>
    /// <value>
    /// The pen.
    /// </value>
    public PenX Pen
    {
      get
      {
        return _pen.Clone(); // Pen is not immutable. Before giving it out, make a copy, so an external program can meddle with this without disturbing us
      }
      set
      {
        if (null == value)
          throw new NotImplementedException("Pen is null");
        InternalSelectedPen = value.Clone(); // Pen is not immutable. Before changing it here in this control, make a copy, so an external program can change the old pen without interference
      }
    }

    /// <summary>
    /// Gets or sets the selected pen internally, <b>but without cloning it. Use this function only internally.</b>
    /// </summary>
    /// <value>
    /// The selected pen.
    /// </value>
    protected PenX InternalSelectedPen
    {
      get
      {
        return _pen;
      }
      set
      {
        if (null == value)
          throw new NotImplementedException("Pen is null");

        if (null != _pen && null != _weakPenChangedHandler)
        {
          _weakPenChangedHandler.Remove();
          _weakPenChangedHandler = null;
        }

        _pen = value;

        if (null != _pen)
        {
          var pen = _pen;
          pen.Changed += (_weakPenChangedHandler = new WeakEventHandler(EhPenChanged, handler => pen.Changed -= handler));
        }

        InitControlProperties();
      }
    }

    private void InitControlProperties()
    {
      if (null != CbBrush)
        CbBrush.SelectedBrush = _pen.BrushHolder;
      if (null != CbLineThickness)
        CbLineThickness.SelectedQuantityAsValueInPoints = _pen.Width;
      if (null != CbDashPattern)
        CbDashPattern.SelectedItem = _pen.DashPattern;
      if (null != CbDashCap)
        CbDashCap.SelectedDashCap = _pen.DashCap;
      if (null != CbStartCap)
        CbStartCap.SelectedLineCap = _pen.StartCap;
      if (null != CbStartCapAbsSize)
        CbStartCapAbsSize.SelectedQuantityAsValueInPoints = _pen.StartCap.MinimumAbsoluteSizePt;
      if (null != CbStartCapRelSize)
        CbStartCapRelSize.SelectedQuantityAsValueInSIUnits = _pen.StartCap.MinimumRelativeSize;
      if (null != CbEndCap)
        CbEndCap.SelectedLineCap = _pen.EndCap;
      if (null != CbEndCapAbsSize)
        CbEndCapAbsSize.SelectedQuantityAsValueInPoints = _pen.EndCap.MinimumAbsoluteSizePt;
      if (null != CbEndCapRelSize)
        CbEndCapRelSize.SelectedQuantityAsValueInSIUnits = _pen.EndCap.MinimumRelativeSize;
      if (null != CbLineJoin)
        CbLineJoin.SelectedLineJoin = _pen.LineJoin;
      if (null != CbMiterLimit)
        CbMiterLimit.SelectedQuantityInSIUnits = _pen.MiterLimit;

      _userChangedAbsStartCapSize = false;
      _userChangedAbsEndCapSize = false;

      _userChangedRelStartCapSize = false;
      _userChangedRelEndCapSize = false;
    }

    public event EventHandler PenChanged;

    protected virtual void OnPenChanged()
    {
      if (PenChanged != null)
        PenChanged(this, EventArgs.Empty);

      UpdatePreviewPanel();
    }

    private WeakEventHandler _weakPenChangedHandler;

    private void EhPenChanged(object sender, EventArgs e)
    {
      OnPenChanged();
    }

    #endregion Pen

    #region Brush

    private bool _showPlotColorsOnly;
    private BrushComboBox _cbBrush;

    public BrushComboBox CbBrush
    {
      get { return _cbBrush; }
      set
      {
        var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(BrushComboBox.SelectedBrushProperty, typeof(BrushComboBox));

        if (_cbBrush != null)
          dpd.RemoveValueChanged(_cbBrush, EhBrush_SelectionChangeCommitted);

        _cbBrush = value;
        if (_cbBrush != null && null != _pen)
        {
          _cbBrush.ShowPlotColorsOnly = _showPlotColorsOnly;
          _cbBrush.SelectedBrush = _pen.BrushHolder;
        }

        if (_cbBrush != null)
        {
          dpd.AddValueChanged(_cbBrush, EhBrush_SelectionChangeCommitted);
          if (!_isAllPropertiesGlue)
          {
            var menuItem = new MenuItem
            {
              Header = "Custom Pen ..."
            };
            menuItem.Click += EhShowCustomPenDialog;
            _cbBrush.ContextMenu.Items.Insert(0, menuItem);
          }
        }
      }
    }

    public bool ShowPlotColorsOnly
    {
      get
      {
        return _showPlotColorsOnly;
      }
      set
      {
        _showPlotColorsOnly = value;
        if (null != _cbBrush)
          _cbBrush.ShowPlotColorsOnly = _showPlotColorsOnly;
      }
    }

    private void EhBrush_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_pen != null)
      {
        _pen.BrushHolder = _cbBrush.SelectedBrush;
        OnPenChanged();
      }
    }

    #endregion Brush

    #region Dash

    private DashPatternComboBox _cbDashPattern;

    public DashPatternComboBox CbDashPattern
    {
      get { return _cbDashPattern; }
      set
      {
        var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(DashPatternComboBox.SelectedItemProperty, typeof(DashPatternComboBox));

        if (_cbDashPattern != null)
          dpd.RemoveValueChanged(_cbDashPattern, EhDashPattern_SelectionChangeCommitted);

        _cbDashPattern = value;
        if (_pen != null && _cbDashPattern != null)
          _cbDashPattern.SelectedItem = _pen.DashPattern;

        if (_cbDashPattern != null)
          dpd.AddValueChanged(_cbDashPattern, EhDashPattern_SelectionChangeCommitted);
      }
    }

    private void EhDashPattern_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_pen != null)
      {
        _pen.DashPattern = _cbDashPattern.SelectedItem;
        OnPenChanged();
      }
    }

    private DashCapComboBox _cbDashCap;

    public DashCapComboBox CbDashCap
    {
      get { return _cbDashCap; }
      set
      {
        var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(DashCapComboBox.SelectedDashCapProperty, typeof(DashCapComboBox));

        if (_cbDashCap != null)
          dpd.RemoveValueChanged(_cbDashCap, EhDashCap_SelectionChangeCommitted);

        _cbDashCap = value;
        if (_pen != null && _cbDashCap != null)
          _cbDashCap.SelectedDashCap = _pen.DashCap;

        if (_cbDashCap != null)
          dpd.AddValueChanged(_cbDashCap, EhDashCap_SelectionChangeCommitted);
      }
    }

    private void EhDashCap_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_pen != null)
      {
        _pen.DashCap = _cbDashCap.SelectedDashCap;
        OnPenChanged();
      }
    }

    #endregion Dash

    #region Width

    private LineThicknessComboBox _cbThickness;

    public LineThicknessComboBox CbLineThickness
    {
      get { return _cbThickness; }
      set
      {
        if (_cbThickness != null)
          _cbThickness.SelectedQuantityChanged -= EhThickness_ChoiceChanged;

        _cbThickness = value;
        if (_pen != null && _cbThickness != null)
          _cbThickness.SelectedQuantityAsValueInPoints = _pen.Width;

        if (_cbThickness != null)
          _cbThickness.SelectedQuantityChanged += EhThickness_ChoiceChanged;
      }
    }

    private void EhThickness_ChoiceChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (_pen != null)
      {
        using (var suspendToken = _pen.SuspendGetToken())
        {
          _pen.Width = (float)_cbThickness.SelectedQuantityAsValueInPoints;
          suspendToken.ResumeSilently();
        };

        OnPenChanged();
      }
    }

    #endregion Width

    #region StartCap

    private LineCapComboBox _cbStartCap;

    public LineCapComboBox CbStartCap
    {
      get { return _cbStartCap; }
      set
      {
        var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(LineCapComboBox.SelectedLineCapProperty, typeof(LineCapComboBox));

        if (_cbStartCap != null)
          dpd.RemoveValueChanged(_cbStartCap, EhStartCap_SelectionChangeCommitted);

        _cbStartCap = value;
        if (_pen != null && _cbStartCap != null)
          _cbStartCap.SelectedLineCap = _pen.StartCap;

        if (_cbStartCap != null)
          dpd.AddValueChanged(_cbStartCap, EhStartCap_SelectionChangeCommitted);
      }
    }

    private void EhStartCap_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_pen != null)
      {
        var cap = _cbStartCap.SelectedLineCap;
        if (_userChangedAbsStartCapSize && _cbStartCapAbsSize != null)
          cap = cap.Clone(_cbStartCapAbsSize.SelectedQuantityAsValueInPoints, cap.MinimumRelativeSize);
        if (_userChangedRelStartCapSize && _cbStartCapRelSize != null)
          cap = cap.Clone(cap.MinimumAbsoluteSizePt, _cbStartCapRelSize.SelectedQuantityAsValueInSIUnits);

        using (var suspendToken = _pen.SuspendGetToken())
        {
          _pen.StartCap = cap;
          suspendToken.ResumeSilently();
        };

        if (_cbStartCapAbsSize != null && cap != null)
        {
          var oldValue = _userChangedAbsStartCapSize;
          _cbStartCapAbsSize.SelectedQuantityAsValueInPoints = cap.MinimumAbsoluteSizePt;
          _userChangedAbsStartCapSize = oldValue;
        }
        if (_cbStartCapRelSize != null && cap != null)
        {
          var oldValue = _userChangedRelStartCapSize;
          _cbStartCapRelSize.SelectedQuantityAsValueInSIUnits = cap.MinimumRelativeSize;
          _userChangedRelStartCapSize = oldValue;
        }

        OnPenChanged();
      }
    }

    private LineCapSizeComboBox _cbStartCapAbsSize;

    public LineCapSizeComboBox CbStartCapAbsSize
    {
      get { return _cbStartCapAbsSize; }
      set
      {
        if (_cbStartCapAbsSize != null)
          _cbStartCapAbsSize.SelectedQuantityChanged -= EhStartCapAbsSize_SelectionChangeCommitted;

        _cbStartCapAbsSize = value;
        if (_pen != null && _cbStartCapAbsSize != null)
          _cbStartCapAbsSize.SelectedQuantityAsValueInPoints = _pen.StartCap.MinimumAbsoluteSizePt;

        if (_cbStartCapAbsSize != null)
          _cbStartCapAbsSize.SelectedQuantityChanged += EhStartCapAbsSize_SelectionChangeCommitted;
      }
    }

    private void EhStartCapAbsSize_SelectionChangeCommitted(object sender, DependencyPropertyChangedEventArgs e)
    {
      _userChangedAbsStartCapSize = true;

      if (_pen != null)
      {
        var cap = _pen.StartCap;
        cap = cap.Clone(_cbStartCapAbsSize.SelectedQuantityAsValueInPoints, cap.MinimumRelativeSize);

        using (var suspendToken = _pen.SuspendGetToken())
        {
          _pen.StartCap = cap;
          suspendToken.ResumeSilently();
        };

        OnPenChanged();
      }
    }

    private QuantityWithUnitTextBox _cbStartCapRelSize;

    public QuantityWithUnitTextBox CbStartCapRelSize
    {
      get { return _cbStartCapRelSize; }
      set
      {
        if (_cbStartCapRelSize != null)
          _cbStartCapRelSize.SelectedQuantityChanged -= EhStartCapRelSize_SelectionChangeCommitted;

        _cbStartCapRelSize = value;
        if (_pen != null && _cbStartCapRelSize != null)
          _cbStartCapRelSize.SelectedQuantityAsValueInSIUnits = _pen.StartCap.MinimumRelativeSize;

        if (_cbStartCapRelSize != null)
          _cbStartCapRelSize.SelectedQuantityChanged += EhStartCapRelSize_SelectionChangeCommitted;
      }
    }

    private void EhStartCapRelSize_SelectionChangeCommitted(object sender, DependencyPropertyChangedEventArgs e)
    {
      _userChangedRelStartCapSize = true;

      if (_pen != null)
      {
        var cap = _pen.StartCap;
        cap = cap.Clone(cap.MinimumAbsoluteSizePt, _cbStartCapRelSize.SelectedQuantityAsValueInSIUnits);

        using (var suspendToken = _pen.SuspendGetToken())
        {
          _pen.StartCap = cap;
          suspendToken.ResumeSilently();
        };

        OnPenChanged();
      }
    }

    #endregion StartCap

    #region EndCap

    private LineCapComboBox _cbEndCap;

    public LineCapComboBox CbEndCap
    {
      get { return _cbEndCap; }
      set
      {
        var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(LineCapComboBox.SelectedLineCapProperty, typeof(LineCapComboBox));

        if (_cbEndCap != null)
          dpd.RemoveValueChanged(_cbEndCap, EhEndCap_SelectionChangeCommitted);

        _cbEndCap = value;
        if (_pen != null && _cbEndCap != null)
          _cbEndCap.SelectedLineCap = _pen.EndCap;

        if (_cbEndCap != null)
          dpd.AddValueChanged(_cbEndCap, EhEndCap_SelectionChangeCommitted);
      }
    }

    private void EhEndCap_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_pen != null)
      {
        var cap = _cbEndCap.SelectedLineCap;
        if (_userChangedAbsEndCapSize && _cbEndCapAbsSize != null)
          cap = cap.Clone(_cbEndCapAbsSize.SelectedQuantityAsValueInPoints, cap.MinimumRelativeSize);
        if (_userChangedRelEndCapSize && _cbEndCapRelSize != null)
          cap = cap.Clone(cap.MinimumAbsoluteSizePt, _cbEndCapRelSize.SelectedQuantityAsValueInSIUnits);

        using (var suspendToken = _pen.SuspendGetToken())
        {
          _pen.EndCap = cap;
          suspendToken.ResumeSilently();
        };

        if (_cbEndCapAbsSize != null)
        {
          var oldValue = _userChangedAbsEndCapSize;
          _cbEndCapAbsSize.SelectedQuantityAsValueInPoints = cap.MinimumAbsoluteSizePt;
          _userChangedAbsEndCapSize = oldValue;
        }
        if (_cbEndCapRelSize != null)
        {
          var oldValue = _userChangedRelEndCapSize;
          _cbEndCapRelSize.SelectedQuantityAsValueInSIUnits = cap.MinimumRelativeSize;
          _userChangedRelEndCapSize = oldValue;
        }

        OnPenChanged();
      }
    }

    private LineCapSizeComboBox _cbEndCapAbsSize;

    public LineCapSizeComboBox CbEndCapAbsSize
    {
      get { return _cbEndCapAbsSize; }
      set
      {
        if (_cbEndCapAbsSize != null)
          _cbEndCapAbsSize.SelectedQuantityChanged -= EhEndCapAbsSize_SelectionChangeCommitted;

        _cbEndCapAbsSize = value;
        if (_pen != null && _cbEndCapAbsSize != null)
          _cbEndCapAbsSize.SelectedQuantityAsValueInPoints = _pen.EndCap.MinimumAbsoluteSizePt;

        if (_cbEndCapAbsSize != null)
          _cbEndCapAbsSize.SelectedQuantityChanged += EhEndCapAbsSize_SelectionChangeCommitted;
      }
    }

    private void EhEndCapAbsSize_SelectionChangeCommitted(object sender, DependencyPropertyChangedEventArgs e)
    {
      _userChangedAbsEndCapSize = true;

      if (_pen != null)
      {
        var cap = _pen.EndCap;
        cap = cap.Clone(_cbEndCapAbsSize.SelectedQuantityAsValueInPoints, cap.MinimumRelativeSize);

        using (var suspendToken = _pen.SuspendGetToken())
        {
          _pen.EndCap = cap;
          suspendToken.ResumeSilently();
        };

        OnPenChanged();
      }
    }

    private QuantityWithUnitTextBox _cbEndCapRelSize;

    public QuantityWithUnitTextBox CbEndCapRelSize
    {
      get { return _cbEndCapRelSize; }
      set
      {
        if (_cbEndCapRelSize != null)
          _cbEndCapRelSize.SelectedQuantityChanged -= EhEndCapRelSize_SelectionChangeCommitted;

        _cbEndCapRelSize = value;
        if (_pen != null && _cbEndCapRelSize != null)
          _cbEndCapRelSize.SelectedQuantityAsValueInSIUnits = _pen.EndCap.MinimumRelativeSize;

        if (_cbEndCapRelSize != null)
          _cbEndCapRelSize.SelectedQuantityChanged += EhEndCapRelSize_SelectionChangeCommitted;
      }
    }

    private void EhEndCapRelSize_SelectionChangeCommitted(object sender, DependencyPropertyChangedEventArgs e)
    {
      _userChangedRelEndCapSize = true;

      if (_pen != null)
      {
        var cap = _pen.EndCap;
        cap = cap.Clone(cap.MinimumAbsoluteSizePt, _cbEndCapRelSize.SelectedQuantityAsValueInSIUnits);

        using (var suspendToken = _pen.SuspendGetToken())
        {
          _pen.EndCap = cap;
          suspendToken.ResumeSilently();
        };

        OnPenChanged();
      }
    }

    #endregion EndCap

    #region LineJoin

    private LineJoinComboBox _cbLineJoin;

    public LineJoinComboBox CbLineJoin
    {
      get { return _cbLineJoin; }
      set
      {
        var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(LineJoinComboBox.SelectedLineJoinProperty, typeof(LineJoinComboBox));

        if (_cbLineJoin != null)
          dpd.RemoveValueChanged(_cbLineJoin, EhLineJoin_SelectionChangeCommitted);

        _cbLineJoin = value;
        if (_pen != null && _cbLineJoin != null)
          _cbLineJoin.SelectedLineJoin = _pen.LineJoin;

        if (_cbLineJoin != null)
          dpd.AddValueChanged(_cbLineJoin, EhLineJoin_SelectionChangeCommitted);
      }
    }

    private void EhLineJoin_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_pen != null)
      {
        using (var suspendToken = _pen.SuspendGetToken())
        {
          _pen.LineJoin = _cbLineJoin.SelectedLineJoin;
          suspendToken.ResumeSilently();
        };

        OnPenChanged();
      }
    }

    #endregion LineJoin

    #region Miter

    private MiterLimitComboBox _cbMiterLimit;

    public MiterLimitComboBox CbMiterLimit
    {
      get { return _cbMiterLimit; }
      set
      {
        if (_cbMiterLimit != null)
          _cbMiterLimit.SelectedQuantityChanged -= EhMiterLimit_SelectionChangeCommitted;

        _cbMiterLimit = value;
        if (_pen != null && _cbMiterLimit != null)
          _cbMiterLimit.SelectedQuantityInSIUnits = _pen.MiterLimit;

        if (_cbLineJoin != null)
          _cbMiterLimit.SelectedQuantityChanged += EhMiterLimit_SelectionChangeCommitted;
      }
    }

    private void EhMiterLimit_SelectionChangeCommitted(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (_pen != null)
      {
        using (var suspendToken = _pen.SuspendGetToken())
        {
          _pen.MiterLimit = (float)_cbMiterLimit.SelectedQuantityInSIUnits;
          suspendToken.ResumeSilently();
        };

        OnPenChanged();
      }
    }

    #endregion Miter

    #region Dialog

    private void EhShowCustomPenDialog(object sender, EventArgs e)
    {
      var ctrler = new PenAllPropertiesController(Pen.Clone())
      {
        ShowPlotColorsOnly = _showPlotColorsOnly,
        ViewObject = new PenAllPropertiesControl()
      };
      if (Current.Gui.ShowDialog(ctrler, "Edit pen properties"))
      {
        Pen = (PenX)ctrler.ModelObject;
      }
    }

    #endregion Dialog

    #region Preview

    private Image _previewPanel;
    private GdiToWpfBitmap _previewBitmap;

    public Image PreviewPanel
    {
      get
      {
        return _previewPanel;
      }
      set
      {
        if (null != _previewPanel)
        {
          _previewPanel.SizeChanged -= EhPreviewPanel_SizeChanged;
        }

        _previewPanel = value;

        if (null != _previewPanel)
        {
          _previewPanel.SizeChanged += EhPreviewPanel_SizeChanged;
          UpdatePreviewPanel();
        }
      }
    }

    private void EhPreviewPanel_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      UpdatePreviewPanel();
    }

    private void UpdatePreviewPanel()
    {
      if (null == _previewPanel || null == _pen)
        return;

      int height = (int)_previewPanel.ActualHeight;
      int width = (int)_previewPanel.ActualWidth;
      if (height <= 0)
        height = 64;
      if (width <= 0)
        width = 64;

      if (null == _previewBitmap)
      {
        _previewBitmap = new GdiToWpfBitmap(width, height);
        _previewPanel.Source = _previewBitmap.WpfBitmap;
      }

      if (width != _previewBitmap.GdiRectangle.Width || height != _previewBitmap.GdiRectangle.Height)
      {
        _previewBitmap.Resize(width, height);
        _previewPanel.Source = _previewBitmap.WpfBitmap;
      }

      using (var grfx = _previewBitmap.BeginGdiPainting())
      {
        var fullRect = _previewBitmap.GdiRectangle;
        grfx.FillRectangle(System.Drawing.Brushes.White, fullRect);
        _pen.BrushHolder.SetEnvironment(fullRect, BrushX.GetEffectiveMaximumResolution(grfx));
        grfx.DrawLine(_pen, fullRect.Width / 6, fullRect.Height / 2, (fullRect.Width * 5) / 6, fullRect.Height / 2);

        _previewBitmap.EndGdiPainting();
      }
    }

    #endregion Preview
  }
}
