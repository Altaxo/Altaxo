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
	/// Interaction logic for ShapeGraphicControl.xaml
	/// </summary>
	[UserControlForController(typeof(IShapeGraphicViewEventSink))]
	public partial class ShapeGraphicControl : UserControl, IShapeGraphicView
	{
		public ShapeGraphicControl()
		{
			InitializeComponent();
		}

		#region IShapeGraphicView
		public Altaxo.Graph.Gdi.PenX DocPen
		{
			get
			{
				return _outlinePenControl.SelectedPen;
			}
			set
			{
				_outlinePenControl.SelectedPen = value;
			}
		}

		public Altaxo.Graph.Gdi.BrushX DocBrush
		{
			get
			{
				return _fillingBrushControl.SelectedBrush;
			}
			set
			{
				_fillingBrushControl.SelectedBrush = value;
			}
		}

		public bool IsFilled
		{
			get
			{
				return _fillingBrushControl.IsBrushEnabled;
			}
			set
			{
				_fillingBrushControl.IsBrushEnabled = value;
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

		public double DocShear
		{
			get
			{
				return _positioningControl.PositionSizeGlue.Shear;
			}
			set
			{
				_positioningControl.PositionSizeGlue.Shear = value;
			}
		}

		public double DocScaleX
		{
			get
			{
				return _positioningControl.PositionSizeGlue.ScaleX;
			}
			set
			{
				_positioningControl.PositionSizeGlue.ScaleX = value;
			}
		}

		public double DocScaleY
		{
			get
			{
				return _positioningControl.PositionSizeGlue.ScaleY;
			}
			set
			{
				_positioningControl.PositionSizeGlue.ScaleY = value;
			}
		}
		#endregion
	}
}
