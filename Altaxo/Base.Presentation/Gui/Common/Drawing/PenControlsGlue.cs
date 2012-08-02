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
		private bool _userChangedAbsStartCapSize;
		private bool _userChangedAbsEndCapSize;

		private bool _userChangedRelStartCapSize;
		private bool _userChangedRelEndCapSize;

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
			if (null != CbLineThickness) CbLineThickness.SelectedQuantityAsValueInPoints = _pen.Width;
			if (null != CbDashStyle) CbDashStyle.SelectedDashStyle = _pen.DashStyleEx;
			if (null != CbDashCap) CbDashCap.SelectedDashCap = _pen.DashCap;
			if (null != CbStartCap) CbStartCap.SelectedLineCap = _pen.StartCap;
			if (null != CbStartCapAbsSize) CbStartCapAbsSize.SelectedQuantityAsValueInPoints = _pen.StartCap.MinimumAbsoluteSizePt;
			if (null != CbStartCapRelSize) CbStartCapRelSize.SelectedQuantityAsValueInSIUnits = _pen.StartCap.MinimumRelativeSize;
			if (null != CbEndCap) CbEndCap.SelectedLineCap = _pen.EndCap;
			if (null != CbEndCapAbsSize) CbEndCapAbsSize.SelectedQuantityAsValueInPoints = _pen.EndCap.MinimumAbsoluteSizePt;
			if (null != CbEndCapRelSize) CbEndCapRelSize.SelectedQuantityAsValueInSIUnits = _pen.EndCap.MinimumRelativeSize;
			if (null != CbLineJoin) CbLineJoin.SelectedLineJoin = _pen.LineJoin;
			if (null != CbMiterLimit) CbMiterLimit.SelectedQuantityAsValueInPoints = _pen.MiterLimit;

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
					_cbThickness.SelectedQuantityAsValueInPoints = _pen.Width;

				if (_cbThickness != null)
					_cbThickness.SelectedQuantityChanged += EhThickness_ChoiceChanged;
			}
		}

		void EhThickness_ChoiceChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (_pen != null)
			{
				BeginPenUpdate();
				_pen.Width = (float)_cbThickness.SelectedQuantityAsValueInPoints;
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
				var cap = _cbStartCap.SelectedLineCap;
				if (_userChangedAbsStartCapSize && _cbStartCapAbsSize != null)
					cap = cap.Clone(_cbStartCapAbsSize.SelectedQuantityAsValueInPoints, cap.MinimumRelativeSize);
				if (_userChangedRelStartCapSize && _cbStartCapRelSize != null)
					cap = cap.Clone(cap.MinimumAbsoluteSizePt, _cbStartCapRelSize.SelectedQuantityAsValueInSIUnits);

				BeginPenUpdate();
				_pen.StartCap = cap;
				EndPenUpdate();

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

		LineCapSizeComboBox _cbStartCapAbsSize;

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

		void EhStartCapAbsSize_SelectionChangeCommitted(object sender, DependencyPropertyChangedEventArgs e)
		{
			_userChangedAbsStartCapSize = true;

			if (_pen != null)
			{
				var cap = _pen.StartCap;
				cap = cap.Clone(_cbStartCapAbsSize.SelectedQuantityAsValueInPoints, cap.MinimumRelativeSize);

				BeginPenUpdate();
				_pen.StartCap = cap;
				EndPenUpdate();

				OnPenChanged();
			}

		}


		QuantityWithUnitTextBox _cbStartCapRelSize;
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

		void EhStartCapRelSize_SelectionChangeCommitted(object sender, DependencyPropertyChangedEventArgs e)
		{
			_userChangedRelStartCapSize = true;

			if (_pen != null)
			{
				var cap = _pen.StartCap;
				cap = cap.Clone(cap.MinimumAbsoluteSizePt, _cbStartCapRelSize.SelectedQuantityAsValueInSIUnits);

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
				var cap = _cbEndCap.SelectedLineCap;
				if (_userChangedAbsEndCapSize && _cbEndCapAbsSize != null)
					cap = cap.Clone(_cbEndCapAbsSize.SelectedQuantityAsValueInPoints, cap.MinimumRelativeSize);
				if (_userChangedRelEndCapSize && _cbEndCapRelSize != null)
					cap = cap.Clone(cap.MinimumAbsoluteSizePt, _cbEndCapRelSize.SelectedQuantityAsValueInSIUnits);

				BeginPenUpdate();
				_pen.EndCap = cap;
				EndPenUpdate();

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

		LineCapSizeComboBox _cbEndCapAbsSize;

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

		void EhEndCapAbsSize_SelectionChangeCommitted(object sender, DependencyPropertyChangedEventArgs e)
		{
			_userChangedAbsEndCapSize = true;

			if (_pen != null)
			{
				var cap = _pen.EndCap;
				cap = cap.Clone(_cbEndCapAbsSize.SelectedQuantityAsValueInPoints, cap.MinimumRelativeSize);

				BeginPenUpdate();
				_pen.EndCap = cap;
				EndPenUpdate();

				OnPenChanged();
			}
		}

		QuantityWithUnitTextBox _cbEndCapRelSize;
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

		void EhEndCapRelSize_SelectionChangeCommitted(object sender, DependencyPropertyChangedEventArgs e)
		{
			_userChangedRelEndCapSize = true;

			if (_pen != null)
			{
				var cap = _pen.EndCap;
				cap = cap.Clone(cap.MinimumAbsoluteSizePt, _cbEndCapRelSize.SelectedQuantityAsValueInSIUnits);

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
					_cbMiterLimit.SelectedQuantityAsValueInPoints = _pen.MiterLimit;

				if (_cbLineJoin != null)
					_cbMiterLimit.SelectedQuantityChanged += EhMiterLimit_SelectionChangeCommitted;
			}
		}

		void EhMiterLimit_SelectionChangeCommitted(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (_pen != null)
			{
				BeginPenUpdate();
				_pen.MiterLimit = (float)_cbMiterLimit.SelectedQuantityAsValueInPoints;
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

			using (var grfx = _previewBitmap.BeginGdiPainting())
			{
				var fullRect = _previewBitmap.GdiRectangle;
				grfx.FillRectangle(System.Drawing.Brushes.White, fullRect);
				_pen.BrushHolder.SetEnvironment(fullRect, BrushX.GetEffectiveMaximumResolution(grfx));
				grfx.DrawLine(_pen, fullRect.Width / 6, fullRect.Height / 2, (fullRect.Width * 5) / 6, fullRect.Height / 2);

				_previewBitmap.EndGdiPainting();
			}

		}

		#endregion

	}
}
