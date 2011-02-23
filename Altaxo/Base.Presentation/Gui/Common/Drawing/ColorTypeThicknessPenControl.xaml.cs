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

namespace Altaxo.Gui.Common.Drawing
{
	/// <summary>
	/// Interaction logic for ColorTypeThicknessPenControl.xaml
	/// </summary>
	public partial class ColorTypeThicknessPenControl : UserControl, IColorTypeThicknessPenView
	{
		PenControlsGlue _glue;

		public ColorTypeThicknessPenControl()
		{
			InitializeComponent();

			_glue = new PenControlsGlue(false);
			_glue.CbBrush = _cbColor;
			_glue.CbDashStyle = _cbLineType;
			_glue.CbLineThickness = _cbThickness;

		}

		#region IColorTypeThicknessPenView

		IColorTypeThicknessPenViewEventSink _controller;
		public IColorTypeThicknessPenViewEventSink Controller
		{
			get { return _controller; }
			set { _controller = value; }
		}


		public Altaxo.Graph.Gdi.PenX DocPen
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
		#endregion
	}
}
