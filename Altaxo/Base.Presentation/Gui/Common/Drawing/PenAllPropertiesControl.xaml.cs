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
	/// Interaction logic for PenAllPropertiesControl.xaml
	/// </summary>
	public partial class PenAllPropertiesControl : UserControl, IPenAllPropertiesView
	{
		PenControlsGlue _glue;
		public PenAllPropertiesControl()
		{
			InitializeComponent();

			_glue = new PenControlsGlue(true);

			_glue.CbBrush = _cbBrush;
			_glue.CbLineThickness = _cbThickness;
			_glue.CbDashStyle = _cbDashStyle;
			_glue.CbDashCap = _cbDashCap;
			_glue.CbStartCap = _cbStartCap;
			_glue.CbStartCapSize = _cbStartCapSize;
			_glue.CbEndCap = _cbEndCap;
			_glue.CbEndCapSize = _cbEndCapSize;
			_glue.CbLineJoin = _cbLineJoin;
			_glue.CbMiterLimit = _cbMiterLimit;
			_glue.PreviewPanel = _previewPanel;
		}

		public PenX Pen
		{
			get
			{
				return _glue.Pen;
			}
			set
			{
				_glue.Pen = value;
			}
		}

	}
}
