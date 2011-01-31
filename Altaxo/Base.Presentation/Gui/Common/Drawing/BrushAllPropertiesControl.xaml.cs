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

using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Common.Drawing
{
	/// <summary>
	/// Interaction logic for BrushAllPropertiesControl.xaml
	/// </summary>
	public partial class BrushAllPropertiesControl : UserControl, IBrushViewAdvanced
	{
		BrushControlsGlue _glue;

		public BrushAllPropertiesControl()
		{
			InitializeComponent();

			_glue = new BrushControlsGlue();
			_glue.CbBrushType = _cbBrushType;
			_glue.CbColor1 = _cbColor;
			_glue.CbColor2 = _cbBackColor;
			_glue.LabelColor2 = _lblBackColor;
			_glue.CbWrapMode = _cbWrapMode;
			_glue.LabelWrapMode = _lblWrapMode;
			_glue.CbGradientFocus = _cbGradientFocus;
			_glue.LabelGradientFocus = _lblGradientFocus;
			_glue.CbGradientScale = _cbColorScale;
			_glue.LabelGradientScale = _lblColorScale;
			_glue.CbTextureScale = _cbTextureScale;
			_glue.LabelTextureScale = _lblTextureScale;

			_glue.CbHatchStyle = _cbHatchStyle;
			_glue.LabelHatchStyle = _lblHatchStyle;
			_glue.CbGradientMode = _cbGradientMode;
			_glue.LabelGradientMode = _lblGradientMode;
			_glue.CbGradientShape = _cbGradientShape;
			_glue.LabelGradientShape = _lblGradientShape;
			_glue.ChkExchangeColors = _chkExchangeColors;
			_glue.LabelExchangeColors = _lblExchangeColors;
			_glue.CbTextureImage = _cbTextureImage;
			_glue.LabelTextureImage = _lblTextureImage;

			_glue.PreviewPanel = _previewPanel;

		}

		public BrushX Brush
		{
			get
			{
				return _glue.Brush;
			}
			set
			{
				_glue.Brush = value;
			}
		}
	}
}
