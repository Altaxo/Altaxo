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

namespace Altaxo.Gui.Graph
{
	/// <summary>
	/// Interaction logic for LineGraphicControl.xaml
	/// </summary>
	[UserControlForController(typeof(ILineGraphicViewEventSink))]
	public partial class LineGraphicControl : UserControl, ILineGraphicView
	{
		public LineGraphicControl()
		{
			InitializeComponent();
		}

		#region ILineGraphicView

		public Altaxo.Graph.Gdi.PenX DocPen
		{
			get
			{
				return _penControl.Pen;
			}
			set
			{
				_penControl.Pen = value;
			}
		}

		public Altaxo.Graph.PointD2D DocPosition
		{
			get
			{
				return _positioningControl.PositionSizeGlue.Position;
			}
			set
			{
				_positioningControl.PositionSizeGlue.Position = value;
			}
		}

		public Altaxo.Graph.PointD2D DocSize
		{
			get
			{
				return _positioningControl.PositionSizeGlue.Size;
			}
			set
			{
				_positioningControl.PositionSizeGlue.Size = value;
			}
		}

		public double DocRotation
		{
			get
			{
				return _positioningControl.PositionSizeGlue.Rotation;
			}
			set
			{
				_positioningControl.PositionSizeGlue.Rotation = value;
			}
		}

		#endregion
	}
}
