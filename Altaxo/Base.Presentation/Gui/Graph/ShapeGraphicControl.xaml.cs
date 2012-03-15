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

namespace Altaxo.Gui.Graph
{
	/// <summary>
	/// Interaction logic for ShapeGraphicControl.xaml
	/// </summary>
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
