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

namespace Altaxo.Gui.Graph
{
	/// <summary>
	/// Interaction logic for ImageGraphicControl.xaml
	/// </summary>
	public partial class ImageGraphicControl : UserControl, IImageGraphicView
	{
		public ImageGraphicControl()
		{
			InitializeComponent();
		}

		public PointD2D DocPosition
		{
			get
			{
				return _ctrlPosSize.PositionSizeGlue.Position;
			}
			set
			{
				_ctrlPosSize.PositionSizeGlue.Position = value;
			}
		}

		public PointD2D DocSize
		{
			get
			{
				return _ctrlPosSize.PositionSizeGlue.Size;
			}
			set
			{
				_ctrlPosSize.PositionSizeGlue.Size = value;
			}
		}
		public double DocRotation
		{
			get
			{
				return _ctrlPosSize.PositionSizeGlue.Rotation;
			}
			set
			{
				_ctrlPosSize.PositionSizeGlue.Rotation = value;
			}
		}
	}
}
