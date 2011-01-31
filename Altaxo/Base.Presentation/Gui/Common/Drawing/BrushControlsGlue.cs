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
	public class BrushControlsGlue : FrameworkElement
	{
		MenuItem _customBrushMenuItem;

		public BrushControlsGlue()
		{
			_customBrushMenuItem = new MenuItem();
			_customBrushMenuItem.Header = "Custom Brush ...";
			_customBrushMenuItem.Click += EhShowCustomBrushDialog;

			InsertContextMenuItem(_customBrushMenuItem);
		}

		void EhShowCustomBrushDialog(object sender, EventArgs e)
		{
			BrushAllPropertiesControl ctrl = new BrushAllPropertiesControl();
			ctrl.Brush = this.Brush;
			throw new NotImplementedException();
			/*
			if (Current.Gui.ShowDialog(ctrl, "Brush properties"))
			{
				this.Brush = ctrl.Brush;
				OnBrushChanged();
			}
			 */
		}

		BrushX _brush;
		public BrushX Brush
		{
			get { return _brush; }
			set
			{
				if (_brush != null)
				{
					_brush.Changed -= EhBrushChanged;
				}

				_brush = value;

				if (_brush != null)
				{
					CbBrushType = _cbBrushType;
					CbHatchStyle = _cbHatchStyle;
					CbColor1 = _cbColor1;
					CbColor2 = _cbColor2;
					ChkExchangeColors = _chkExchangeColors;
					CbGradientMode = _cbGradientMode;
					CbGradientShape = _cbGradientShape;
					CbWrapMode = _cbWrapMode;
					CbGradientFocus = _cbGradientFocus;
					CbGradientScale = _cbGradientScale;
					CbTextureImage = _cbTextureImage;
					CbTextureScale = _cbTextureScale;


					_brush.Changed += EhBrushChanged;
					UpdatePreviewPanel();
				}
			}
		}

		void EhBrushChanged(object sender, EventArgs e)
		{
			OnBrushChanged();
			UpdatePreviewPanel();
		}


		public event EventHandler BrushChanged;
		protected virtual void OnBrushChanged()
		{
			if (BrushChanged != null)
				BrushChanged(this, EventArgs.Empty);
		}


		ColorType _colorType = ColorType.KnownAndSystemColor;
		public ColorType ColorType
		{
			get
			{
				return _colorType;
			}
			set
			{
				_colorType = value;
				if (_cbColor1 != null)
					_cbColor1.ColorType = value; // only for color1
			}
		}

		#region BrushType
		BrushTypeComboBox _cbBrushType;

		public BrushTypeComboBox CbBrushType
		{
			get { return _cbBrushType; }
			set
			{
				var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(BrushTypeComboBox.BrushTypeProperty, typeof(BrushTypeComboBox));

				if (null != _cbBrushType)
					dpd.RemoveValueChanged(_cbBrushType, EhBrushType_SelectionChangeCommitted);

				_cbBrushType = value;
				if (_brush != null && CbBrushType != null)
					_cbBrushType.BrushType = _brush.BrushType;

				if (null != _cbBrushType)
					dpd.AddValueChanged(_cbBrushType, EhBrushType_SelectionChangeCommitted);


			}
		}

		void EhBrushType_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (_brush != null)
			{
				_brush.BrushType = _cbBrushType.BrushType;

				OnBrushChanged();

				UpdateColor2State();
				UpdateExchangeColorsState();
				UpdateHatchStyleState();
				UpdateWrapModeState();
				UpdateGradientModeState();
				UpdateGradientShapeState();
				UpdateGradientFocusState();
				UpdateGradientScaleState();
				UpdateTextureImageState();
				UpdateTextureScaleState();
			}
		}

		#endregion

		#region HatchStyle
		HatchStyleComboBox _cbHatchStyle;
		public HatchStyleComboBox CbHatchStyle
		{
			get { return _cbHatchStyle; }
			set
			{
				var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(HatchStyleComboBox.HatchStyleProperty, typeof(HatchStyleComboBox));
				if (_cbHatchStyle != null)
					dpd.RemoveValueChanged(_cbHatchStyle, EhHatchStyle_SelectionChangeCommitted);

				_cbHatchStyle = value;
				if (_brush != null && _cbHatchStyle != null)
					_cbHatchStyle.HatchStyle = _brush.HatchStyle;

				if (_cbHatchStyle != null)
					dpd.AddValueChanged(_cbHatchStyle, EhHatchStyle_SelectionChangeCommitted);

				UpdateHatchStyleState();

			}
		}

		void EhHatchStyle_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (_brush != null)
			{
				_brush.HatchStyle = _cbHatchStyle.HatchStyle;
				OnBrushChanged();
			}
		}

		Control _lblHatchStyle;
		public Control LabelHatchStyle
		{
			get
			{
				return _lblHatchStyle;
			}
			set
			{
				_lblHatchStyle = value;
				UpdateHatchStyleState();
			}
		}
		void UpdateHatchStyleState()
		{
			if (_brush != null)
			{
				BrushType btype = _brush.BrushType;
				var vis = (btype == BrushType.HatchBrush) ? Visibility.Visible : Visibility.Collapsed;
				if (_lblHatchStyle != null)
					_lblHatchStyle.Visibility = vis;
				if (_cbHatchStyle != null)
					_cbHatchStyle.Visibility = vis;
			}
		}

		#endregion

		#region Color1

		ColorComboBox _cbColor1;
		public ColorComboBox CbColor1
		{
			get { return _cbColor1; }
			set
			{
				var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(ColorComboBox.SelectedColorProperty, typeof(ColorComboBox));

				if (_cbColor1 != null)
				{
					dpd.RemoveValueChanged(_cbColor1, EhColor1_ColorChoiceChanged);

					foreach (var item in _customContextMenuItems)
						_cbColor1.ContextMenu.Items.Remove(item);
				}

				_cbColor1 = value;
				if (_cbColor1 != null)
					_cbColor1.ColorType = _colorType;
				if (_brush != null && _cbColor1 != null)
					_cbColor1.SelectedColor = GuiHelper.ToWpf(_brush.Color);

				if (_cbColor1 != null)
				{
					dpd.AddValueChanged(_cbColor1, EhColor1_ColorChoiceChanged);


					foreach (var item in _customContextMenuItems)
						_cbColor1.ContextMenu.Items.Insert(0, item);
				}
			}
		}

		void EhColor1_ColorChoiceChanged(object sender, EventArgs e)
		{
			if (_brush != null)
			{
				_brush.Color = GuiHelper.FromWpf(_cbColor1.SelectedColor);
				OnBrushChanged();
			}
		}

		#endregion

		#region Color2
		ColorComboBox _cbColor2;

		public ColorComboBox CbColor2
		{
			get { return _cbColor2; }
			set
			{
				var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(ColorComboBox.SelectedColorProperty, typeof(ColorComboBox));

				if (_cbColor2 != null)
					dpd.RemoveValueChanged(_cbColor2,EhColor2_ColorChoiceChanged);

				_cbColor2 = value;
				if (_brush != null && _cbColor2 != null)
					_cbColor2.SelectedColor = GuiHelper.ToWpf(_brush.BackColor);

				if (_cbColor2 != null)
					dpd.AddValueChanged(_cbColor2, EhColor2_ColorChoiceChanged);

				UpdateColor2State();
			}
		}

		void EhColor2_ColorChoiceChanged(object sender, EventArgs e)
		{
			if (_brush != null)
			{
				_brush.BackColor = GuiHelper.FromWpf(_cbColor2.SelectedColor);
				OnBrushChanged();
			}
		}


		Control _lblColor2;
		public Control LabelColor2
		{
			get
			{
				return _lblColor2;
			}
			set
			{
				_lblColor2 = value;
				UpdateColor2State();
			}
		}
		void UpdateColor2State()
		{
			if (_brush != null)
			{
				BrushType btype = _brush.BrushType;
				var vis = ((btype != BrushType.SolidBrush) && (btype != BrushType.TextureBrush)) ? Visibility.Visible : Visibility.Collapsed;
				if (_lblColor2 != null)
					_lblColor2.Visibility = vis;
				if (_cbColor2 != null)
					_cbColor2.Visibility = vis;
			}
		}


		#endregion

		#region ExchangeColors
		CheckBox _chkExchangeColors;

		public CheckBox ChkExchangeColors
		{
			get { return _chkExchangeColors; }
			set
			{
				if (_chkExchangeColors != null)
					_chkExchangeColors.Click -= EhChkExchangeColors_CheckedChanged;

				_chkExchangeColors = value;
				if (_brush != null && _chkExchangeColors != null)
					_chkExchangeColors.IsChecked = _brush.ExchangeColors;

				if (_chkExchangeColors != null)
					_chkExchangeColors.Click += EhChkExchangeColors_CheckedChanged;

				UpdateExchangeColorsState();
			}
		}

		void EhChkExchangeColors_CheckedChanged(object sender, EventArgs e)
		{
			if (_brush != null)
			{
				_brush.ExchangeColors = (bool)_chkExchangeColors.IsChecked;
				OnBrushChanged();
			}
		}


		Control _lblExchangeColors;
		public Control LabelExchangeColors
		{
			get
			{
				return _lblExchangeColors;
			}
			set
			{
				_lblExchangeColors = value;
				UpdateExchangeColorsState();
			}
		}
		void UpdateExchangeColorsState()
		{
			if (_brush != null)
			{
				BrushType btype = _brush.BrushType;
				var vis = ((btype == BrushType.HatchBrush) || (btype == BrushType.LinearGradientBrush) || (btype == BrushType.PathGradientBrush)) ? Visibility.Visible : Visibility.Collapsed;
				if (_lblExchangeColors != null)
					_lblExchangeColors.Visibility = vis;
				if (_chkExchangeColors != null)
					_chkExchangeColors.Visibility = vis;
			}
		}


		#endregion

		#region Wrap Mode
		WrapModeComboBox _cbWrapMode;
		public WrapModeComboBox CbWrapMode
		{
			get { return _cbWrapMode; }
			set
			{
				var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(WrapModeComboBox.WrapModeProperty, typeof(WrapModeComboBox));
				if (_cbWrapMode != null)
					dpd.RemoveValueChanged(_cbWrapMode, EhWrapMode_SelectionChangeCommitted);

				_cbWrapMode = value;
				if (_brush != null && _cbWrapMode != null)
					_cbWrapMode.WrapMode = _brush.WrapMode;

				if (_cbWrapMode != null)
					dpd.AddValueChanged(_cbWrapMode, EhWrapMode_SelectionChangeCommitted);

				UpdateWrapModeState();

			}
		}

		void EhWrapMode_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (_brush != null)
			{
				_brush.WrapMode = _cbWrapMode.WrapMode;
				OnBrushChanged();
			}
		}

		Control _lblWrapMode;
		public Control LabelWrapMode
		{
			get
			{
				return _lblWrapMode;
			}
			set
			{
				_lblWrapMode = value;
				UpdateWrapModeState();
			}
		}
		void UpdateWrapModeState()
		{
			if (_brush != null)
			{
				BrushType btype = _brush.BrushType;
				var vis = ((btype == BrushType.LinearGradientBrush || btype == BrushType.PathGradientBrush || btype == BrushType.TextureBrush)) ? Visibility.Visible : Visibility.Collapsed;
				if (_lblWrapMode != null)
					_lblWrapMode.Visibility = vis;
				if (_cbWrapMode != null)
					_cbWrapMode.Visibility = vis;
			}
		}
		#endregion

		#region Gradient Mode

		LinearGradientModeComboBox _cbGradientMode;
		public LinearGradientModeComboBox CbGradientMode
		{
			get { return _cbGradientMode; }
			set
			{
				var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(LinearGradientModeComboBox.LinearGradientModeProperty, typeof(LinearGradientModeComboBox));

				if (_cbGradientMode != null)
					dpd.RemoveValueChanged(_cbGradientMode, EhGradientMode_SelectionChangeCommitted);

				_cbGradientMode = value;
				if (_brush != null && _cbGradientMode != null)
					_cbGradientMode.LinearGradientMode = _brush.GradientMode;

				if (_cbGradientMode != null)
					dpd.AddValueChanged(_cbGradientMode, EhGradientMode_SelectionChangeCommitted);

				UpdateGradientModeState();

			}
		}

		void EhGradientMode_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (_brush != null)
			{
				_brush.GradientMode = _cbGradientMode.LinearGradientMode;
				OnBrushChanged();
			}
		}

		Control _lblGradientMode;
		public Control LabelGradientMode
		{
			get
			{
				return _lblGradientMode;
			}
			set
			{
				_lblGradientMode = value;
				UpdateGradientModeState();
			}
		}
		void UpdateGradientModeState()
		{
			if (_brush != null)
			{
				BrushType btype = _brush.BrushType;
				var vis = (btype == BrushType.LinearGradientBrush) ? Visibility.Visible : Visibility.Collapsed;
				if (_lblGradientMode != null)
					_lblGradientMode.Visibility = vis;
				if (_cbGradientMode != null)
					_cbGradientMode.Visibility = vis;
			}
		}

		#endregion

		#region Gradient Shape
		LinearGradientShapeComboBox _cbGradientShape;
		public LinearGradientShapeComboBox CbGradientShape
		{
			get { return _cbGradientShape; }
			set
			{
				var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(LinearGradientShapeComboBox.LinearGradientShapeProperty, typeof(LinearGradientShapeComboBox));

				if (_cbGradientShape != null)
					dpd.RemoveValueChanged(_cbGradientShape, EhGradientShape_SelectionChangeCommitted);

				_cbGradientShape = value;
				if (_brush != null && _cbGradientShape != null)
					_cbGradientShape.LinearGradientShape = _brush.GradientShape;

				if (_cbWrapMode != null)
					dpd.AddValueChanged(_cbGradientShape, EhGradientShape_SelectionChangeCommitted);

				UpdateGradientShapeState();

			}
		}

		void EhGradientShape_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (_brush != null)
			{
				_brush.GradientShape = _cbGradientShape.LinearGradientShape;
				OnBrushChanged();
			}
		}

		Control _lblGradientShape;
		public Control LabelGradientShape
		{
			get
			{
				return _lblGradientShape;
			}
			set
			{
				_lblGradientShape = value;
				UpdateGradientShapeState();
			}
		}
		void UpdateGradientShapeState()
		{
			if (_brush != null)
			{
				BrushType btype = _brush.BrushType;
				var vis = (btype == BrushType.LinearGradientBrush) ? Visibility.Visible : Visibility.Collapsed;
				if (_lblGradientShape != null)
					_lblGradientShape.Visibility = vis;
				if (_cbGradientShape != null)
					_cbGradientShape.Visibility = vis;
			}
		}
		#endregion

		#region Gradient Focus

		GradientFocusComboBox _cbGradientFocus;
		public GradientFocusComboBox CbGradientFocus
		{
			get { return _cbGradientFocus; }
			set
			{
				var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(GradientFocusComboBox.GradientFocusProperty, typeof(GradientFocusComboBox));
				if (_cbGradientFocus != null)
				{
					dpd.RemoveValueChanged(_cbGradientFocus, EhGradientFocus_SelectionChangeCommitted);
				}

				_cbGradientFocus = value;
				if (_brush != null && _cbGradientFocus != null)
					_cbGradientFocus.GradientFocus = _brush.GradientFocus;

				if (_cbGradientFocus != null)
				{
					dpd.AddValueChanged(_cbGradientFocus, EhGradientFocus_SelectionChangeCommitted);
				}

				UpdateGradientFocusState();
			}
		}

		void EhGradientFocus_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (_brush != null)
			{
				_brush.GradientFocus = (float)_cbGradientFocus.GradientFocus;
				OnBrushChanged();
			}
		}

		Control _lblGradientFocus;
		public Control LabelGradientFocus
		{
			get
			{
				return _lblGradientFocus;
			}
			set
			{
				_lblGradientFocus = value;
				UpdateGradientFocusState();
			}
		}
		void UpdateGradientFocusState()
		{
			if (_brush != null)
			{
				BrushType btype = _brush.BrushType;
				var vis = (btype == BrushType.LinearGradientBrush) ? Visibility.Visible : Visibility.Collapsed;
				if (_lblGradientFocus != null)
					_lblGradientFocus.Visibility = vis;
				if (_cbGradientFocus != null)
					_cbGradientFocus.Visibility = vis;
			}
		}
		#endregion

		#region Gradient Scale
		ColorScaleComboBox _cbGradientScale;
		public ColorScaleComboBox CbGradientScale
		{
			get { return _cbGradientScale; }
			set
			{
				var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(ColorScaleComboBox.ColorScaleProperty, typeof(ColorScaleComboBox));

				if (_cbGradientScale != null)
				{
					dpd.RemoveValueChanged(_cbGradientScale, EhGradientScale_SelectionChangeCommitted);
				}

				_cbGradientScale = value;
				if (_brush != null && _cbGradientScale != null)
					_cbGradientScale.ColorScale = _brush.GradientScale;

				if (_cbGradientScale != null)
				{
					dpd.AddValueChanged(_cbGradientScale, EhGradientScale_SelectionChangeCommitted);
				}

				UpdateGradientScaleState();
			}
		}

		void EhGradientScale_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (_brush != null)
			{
				_brush.GradientScale = (float)_cbGradientScale.ColorScale;
				OnBrushChanged();
			}
		}
	

		Control _lblGradientScale;
		public Control LabelGradientScale
		{
			get
			{
				return _lblGradientScale;
			}
			set
			{
				_lblGradientScale = value;
				UpdateGradientScaleState();
			}
		}
		void UpdateGradientScaleState()
		{
			if (_brush != null)
			{
				BrushType btype = _brush.BrushType;
				var vis = (btype == BrushType.LinearGradientBrush) ? Visibility.Visible : Visibility.Collapsed;
				if (_lblGradientScale != null)
					_lblGradientScale.Visibility = vis;
				if (_cbGradientScale != null)
					_cbGradientScale.Visibility = vis;
			}
		}
		#endregion

		#region Texture Image

		TextureImageComboBox _cbTextureImage;
		public TextureImageComboBox CbTextureImage
		{
			get { return _cbTextureImage; }
			set
			{
				var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(TextureImageComboBox.TextureImageProperty, typeof(TextureImageComboBox));

				if (_cbTextureImage != null)
					dpd.RemoveValueChanged(_cbTextureImage, EhTextureImage_SelectionChangeCommitted);

				_cbTextureImage = value;
				if (_brush != null && _cbTextureImage != null)
					_cbTextureImage.TextureImage = _brush.TextureImage;

				if (_cbTextureImage != null)
					dpd.AddValueChanged(_cbTextureImage, EhTextureImage_SelectionChangeCommitted);

				UpdateTextureImageState();

			}
		}

		void EhTextureImage_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (_brush != null)
			{
				_brush.TextureImage = _cbTextureImage.TextureImage;
				OnBrushChanged();
			}
		}

		Control _lblTextureImage;
		public Control LabelTextureImage
		{
			get
			{
				return _lblTextureImage;
			}
			set
			{
				_lblTextureImage = value;
				UpdateTextureImageState();
			}
		}
		void UpdateTextureImageState()
		{
			if (_brush != null)
			{
				BrushType btype = _brush.BrushType;
				var vis = (btype == BrushType.TextureBrush) ? Visibility.Visible : Visibility.Collapsed;
				if (_lblTextureImage != null)
					_lblTextureImage.Visibility = vis;
				if (_cbTextureImage != null)
					_cbTextureImage.Visibility = vis;
			}
		}

		#endregion

		#region Texture Scale
		TextureScaleComboBox _cbTextureScale;
		public TextureScaleComboBox CbTextureScale
		{
			get { return _cbTextureScale; }
			set
			{
				var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(TextureScaleComboBox.TextureScaleProperty, typeof(TextureScaleComboBox));

				if (_cbTextureScale != null)
				{
					dpd.RemoveValueChanged(_cbTextureScale, EhTextureScale_SelectionChangeCommitted);
				}

				_cbTextureScale = value;
				if (_brush != null && _cbTextureScale != null)
					_cbTextureScale.TextureScale = _brush.TextureScale;

				if (_cbTextureScale != null)
				{
					dpd.AddValueChanged(_cbTextureScale, EhTextureScale_SelectionChangeCommitted);
				}

				UpdateTextureScaleState();
			}
		}

		void EhTextureScale_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (_brush != null)
			{
				_brush.TextureScale = (float)_cbTextureScale.TextureScale;
				OnBrushChanged();
			}
		}
		

		Control _lblTextureScale;
		public Control LabelTextureScale
		{
			get
			{
				return _lblTextureScale;
			}
			set
			{
				_lblTextureScale = value;
				UpdateTextureScaleState();
			}
		}
		void UpdateTextureScaleState()
		{
			if (_brush != null)
			{
				BrushType btype = _brush.BrushType;
				var vis = (btype == BrushType.TextureBrush) ? Visibility.Visible : Visibility.Collapsed;
				if (_lblTextureScale != null)
					_lblTextureScale.Visibility = vis;
				if (_cbTextureScale != null)
					_cbTextureScale.Visibility = vis;
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
				if(null!=_previewPanel)
				{
				_previewPanel.SizeChanged -= EhPreviewPanel_SizeChanged;
				}

				_previewPanel = value;

				if(null!=_previewPanel)
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
			if (null == _previewPanel || null==_brush)
				return;

			int height = (int)_previewPanel.ActualHeight;
			int width = (int)_previewPanel.ActualWidth;
			if (height <= 0)
				height=64;
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

				grfx.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
					grfx.FillRectangle(System.Drawing.Brushes.Transparent, fullRect);
					grfx.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

					var r2 = fullRect;
					r2.Inflate(-r2.Width / 4, -r2.Height / 4);
					//grfx.FillRectangle(System.Drawing.Brushes.Black, r2);

					_brush.SetEnvironment(fullRect, BrushX.GetEffectiveMaximumResolution(grfx));
					grfx.FillRectangle(_brush, fullRect);

					_previewBitmap.WpfBitmap.Invalidate();
			
		}

		#endregion

		#region Context Menu
		List<MenuItem> _customContextMenuItems = new List<MenuItem>();

		public void InsertContextMenuItem(MenuItem item)
		{
			if (_customContextMenuItems.Contains(item))
				return;

			_customContextMenuItems.Add(item);

			if (_cbColor1 != null)
				_cbColor1.ContextMenu.Items.Insert(0, item);
		}
		public void RemoveContextMenuItem(MenuItem item)
		{
			if (!_customContextMenuItems.Contains(item))
				return;

			_customContextMenuItems.Remove(item);

			if (_cbColor1 != null)
				_cbColor1.ContextMenu.Items.Remove(item);
		}

		#endregion

	}
}
