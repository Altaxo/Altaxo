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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


using Altaxo.Graph;
using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Common.Drawing
{
  public class PenControlsGlue : FrameworkElement
  {
    private bool _userChangedStartCapSize;
    private bool _userChangedEndCapSize;
		
		private bool _isAllPropertiesGlue;

    public PenControlsGlue()
    {
    }

		public PenControlsGlue(bool isAllPropertiesGlue)
		{
			_isAllPropertiesGlue = isAllPropertiesGlue;
		}

    #region Pen

    PenX _pen;
    public PenX Pen
    {
      get
      {
        return _pen;
      }
      set
      {
				if (null != _pen)
				{
					_pen.Changed -= EhPenChanged;
				}
        _pen = value;

				if (null != _pen)
				{
					_pen.Changed += EhPenChanged;
					InitControlProperties();
				}

      }
    }


    void InitControlProperties()
    {
			if (null != CbBrush) CbBrush.SelectedBrush = _pen.BrushHolder;
			if (null != CbLineThickness) CbLineThickness.SelectedQuantityInPoints = _pen.Width;
			if (null != CbDashStyle) CbDashStyle.SelectedDashStyle = _pen.DashStyleEx;
			if (null != CbDashCap) CbDashCap.SelectedDashCap = _pen.DashCap;
			if (null != CbStartCap) CbStartCap.SelectedLineCap = _pen.StartCap;
			if (null != CbStartCapSize) CbStartCapSize.SelectedQuantityInPoints = _pen.StartCap.Size;
      if (null != CbEndCap) CbEndCap.SelectedLineCap = _pen.EndCap;
			if (null != CbEndCapSize) CbEndCapSize.SelectedQuantityInPoints = _pen.EndCap.Size;
			if (null != CbLineJoin) CbLineJoin.SelectedLineJoin = _pen.LineJoin;
			if (null != CbMiterLimit) CbMiterLimit.SelectedQuantityInPoints = _pen.MiterLimit;
    }

    public event EventHandler PenChanged;
    protected virtual void OnPenChanged()
    {
      if (PenChanged != null)
        PenChanged(this, EventArgs.Empty);

			UpdatePreviewPanel();
    }

    void BeginPenUpdate()
    {
      _pen.Changed -= EhPenChanged;
    }
    void EndPenUpdate()
    {
      _pen.Changed += EhPenChanged;
    }


    void EhPenChanged(object sender, EventArgs e)
    {
      OnPenChanged();
    }

    #endregion

		#region Brush

		BrushComboBox _cbBrush;
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
					_cbBrush.SelectedBrush = _pen.BrushHolder;

				if (_cbBrush != null)
				{
					dpd.AddValueChanged(_cbBrush, EhBrush_SelectionChangeCommitted);
					if (!_isAllPropertiesGlue)
					{
						var menuItem = new MenuItem();
						menuItem.Header = "Custom Pen ...";
						menuItem.Click += EhShowCustomPenDialog;
						_cbBrush.ContextMenu.Items.Insert(0, menuItem);
					}
				}
			}
		}

		void EhBrush_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (_pen != null)
			{
				_pen.BrushHolder = _cbBrush.SelectedBrush;
				OnPenChanged();
			}
		}

		#endregion

		#region Dash

		DashStyleComboBox _cbDashStyle;
    public DashStyleComboBox CbDashStyle
    {
      get { return _cbDashStyle; }
      set
      {
				var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(DashStyleComboBox.SelectedDashStyleProperty, typeof(DashStyleComboBox));

        if (_cbDashStyle != null)
          dpd.RemoveValueChanged(_cbDashStyle, EhDashStyle_SelectionChangeCommitted);

        _cbDashStyle = value;
        if (_pen != null && _cbDashStyle != null)
          _cbDashStyle.SelectedDashStyle = _pen.DashStyleEx;

        if (_cbDashStyle != null)
          dpd.AddValueChanged(_cbDashStyle, EhDashStyle_SelectionChangeCommitted);
      }
    }

    void EhDashStyle_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_pen != null)
      {
        _pen.DashStyleEx = _cbDashStyle.SelectedDashStyle;
        OnPenChanged();
      }
    }

    DashCapComboBox _cbDashCap;
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

    void EhDashCap_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_pen != null)
      {
        _pen.DashCap = _cbDashCap.SelectedDashCap;
        OnPenChanged();
      }
    }

    #endregion

    #region Width

    LineThicknessComboBox _cbThickness;
    public LineThicknessComboBox CbLineThickness
    {
      get { return _cbThickness; }
      set
      {
        if (_cbThickness != null)
          _cbThickness.SelectedQuantityChanged -= EhThickness_ChoiceChanged;

        _cbThickness = value;
        if (_pen != null && _cbThickness != null)
					_cbThickness.SelectedQuantityInPoints = _pen.Width;

				if (_cbThickness != null)
					_cbThickness.SelectedQuantityChanged += EhThickness_ChoiceChanged;
      }
    }

    void EhThickness_ChoiceChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (_pen != null)
      {
        BeginPenUpdate();
				_pen.Width = (float)_cbThickness.SelectedQuantityInPoints;
        EndPenUpdate();

        OnPenChanged();
      }
    }
  

    #endregion

    #region StartCap

    LineCapComboBox _cbStartCap;

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

    void EhStartCap_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_pen != null)
      {
        LineCapEx cap = _cbStartCap.SelectedLineCap;
        if (_userChangedStartCapSize && _cbStartCapSize != null)
					cap.Size = (float)_cbStartCapSize.SelectedQuantityInPoints;

        BeginPenUpdate();
        _pen.StartCap = cap;
        EndPenUpdate();

        if (_cbStartCapSize != null)
					_cbStartCapSize.SelectedQuantityInPoints = cap.Size;

        OnPenChanged();
      }

    }

    LineCapSizeComboBox _cbStartCapSize;

    public LineCapSizeComboBox CbStartCapSize
    {
      get { return _cbStartCapSize; }
      set
      {
				if (_cbStartCapSize != null)
					_cbStartCapSize.SelectedQuantityChanged -= EhStartCapSize_SelectionChangeCommitted;

				_cbStartCapSize = value;
        if (_pen != null && _cbStartCapSize != null)
					_cbStartCapSize.SelectedQuantityInPoints = _pen.StartCap.Size;

        if (_cbStartCapSize != null)
					_cbStartCapSize.SelectedQuantityChanged += EhStartCapSize_SelectionChangeCommitted;
			}
    }

    void EhStartCapSize_SelectionChangeCommitted(object sender, DependencyPropertyChangedEventArgs e)
    {
      _userChangedStartCapSize = true;

      if (_pen != null)
      {
        LineCapEx cap = _pen.StartCap;
				cap.Size = (float)_cbStartCapSize.SelectedQuantityInPoints;

        BeginPenUpdate();
        _pen.StartCap = cap;
        EndPenUpdate();

        OnPenChanged();
      }

    }
   

    #endregion

    #region EndCap

    LineCapComboBox _cbEndCap;

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



    void EhEndCap_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_pen != null)
      {
        LineCapEx cap = _cbEndCap.SelectedLineCap;
        if (_userChangedEndCapSize && _cbEndCapSize != null)
					cap.Size = (float)_cbEndCapSize.SelectedQuantityInPoints;

        BeginPenUpdate();
        _pen.EndCap = cap;
        EndPenUpdate();

        if (_cbEndCapSize != null)
					_cbEndCapSize.SelectedQuantityInPoints = cap.Size;

        OnPenChanged();
      }


    }

    LineCapSizeComboBox _cbEndCapSize;

    public LineCapSizeComboBox CbEndCapSize
    {
      get { return _cbEndCapSize; }
      set
      {
        if (_cbEndCapSize != null)
					_cbEndCapSize.SelectedQuantityChanged -= EhEndCapSize_SelectionChangeCommitted;

        _cbEndCapSize = value;
        if (_pen != null && _cbEndCapSize != null)
					_cbEndCapSize.SelectedQuantityInPoints = _pen.EndCap.Size;

        if (_cbEndCapSize != null)
					_cbEndCapSize.SelectedQuantityChanged += EhEndCapSize_SelectionChangeCommitted;
			}
    }

    void EhEndCapSize_SelectionChangeCommitted(object sender, DependencyPropertyChangedEventArgs e)
    {
      _userChangedEndCapSize = true;

      if (_pen != null)
      {
        LineCapEx cap = _pen.EndCap;
				cap.Size = (float)_cbEndCapSize.SelectedQuantityInPoints;

        BeginPenUpdate();
        _pen.EndCap = cap;
        EndPenUpdate();

        OnPenChanged();
      }

    }

    #endregion

    #region LineJoin

    LineJoinComboBox _cbLineJoin;

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

    void EhLineJoin_SelectionChangeCommitted(object sender, EventArgs e)
    {
      if (_pen != null)
      {
        BeginPenUpdate();
        _pen.LineJoin = _cbLineJoin.SelectedLineJoin;
        EndPenUpdate();

        OnPenChanged();
      }
    }

    #endregion

    #region Miter
    MiterLimitComboBox _cbMiterLimit;

    public MiterLimitComboBox CbMiterLimit
    {
      get { return _cbMiterLimit; }
      set
      {
        if (_cbMiterLimit != null)
          _cbMiterLimit.SelectedQuantityChanged -= EhMiterLimit_SelectionChangeCommitted;

        _cbMiterLimit = value;
        if (_pen != null && _cbMiterLimit != null)
					_cbMiterLimit.SelectedQuantityInPoints = _pen.MiterLimit;

        if (_cbLineJoin != null)
					_cbMiterLimit.SelectedQuantityChanged += EhMiterLimit_SelectionChangeCommitted;
			}
    }

    void EhMiterLimit_SelectionChangeCommitted(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (_pen != null)
      {
        BeginPenUpdate();
				_pen.MiterLimit = (float)_cbMiterLimit.SelectedQuantityInPoints;
        EndPenUpdate();

        OnPenChanged();
      }
    }

    #endregion

    #region Dialog

    void EhShowCustomPenDialog(object sender, EventArgs e)
    {
			PenAllPropertiesController ctrler = new PenAllPropertiesController((PenX)this.Pen.Clone());
      ctrler.ViewObject = new PenAllPropertiesControl();
      if (Current.Gui.ShowDialog(ctrler, "Edit pen properties"))
      {
        this.Pen = (PenX)ctrler.ModelObject;
      }
    }

    #endregion

		#region Preview

		Image _previewPanel;
		GdiToWpfBitmap _previewBitmap;
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

		void EhPreviewPanel_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			UpdatePreviewPanel();
		}


		void UpdatePreviewPanel()
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

			var grfx = _previewBitmap.GdiGraphics;

			var fullRect = _previewBitmap.GdiRectangle;
			grfx.FillRectangle(System.Drawing.Brushes.White, fullRect);
			_pen.BrushHolder.SetEnvironment(fullRect, BrushX.GetEffectiveMaximumResolution(grfx));
			grfx.DrawLine(_pen, fullRect.Width/6, fullRect.Height / 2, (fullRect.Width*5)/6, fullRect.Height / 2);

			_previewBitmap.WpfBitmap.Invalidate();

		}

		#endregion

  }
}
