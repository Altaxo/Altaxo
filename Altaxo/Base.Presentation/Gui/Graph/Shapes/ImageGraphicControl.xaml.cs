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

namespace Altaxo.Gui.Graph.Shapes
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
				var x = _edPositionX.SelectedQuantity.AsValueIn(Units.Length.Point.Instance);
				var y = _edPositionY.SelectedQuantity.AsValueIn(Units.Length.Point.Instance);
				return new PointD2D(x, y);
			}
			set
			{
				_edPositionX.SelectedQuantity = new Units.DimensionfulQuantity(value.X, Units.Length.Point.Instance).AsQuantityIn(_edPositionX.UnitEnvironment.DefaultUnit);
				_edPositionY.SelectedQuantity = new Units.DimensionfulQuantity(value.Y, Units.Length.Point.Instance).AsQuantityIn(_edPositionY.UnitEnvironment.DefaultUnit);
			}
		}

		public PointD2D DocSize
		{
			get
			{
				var x = _edSizeX.SelectedQuantity.AsValueIn(Units.Length.Point.Instance);
				var y = _edSizeY.SelectedQuantity.AsValueIn(Units.Length.Point.Instance);
				return new PointD2D(x, y);
			}
			set
			{
				_edSizeX.SelectedQuantity = new Units.DimensionfulQuantity(value.X, Units.Length.Point.Instance).AsQuantityIn(_edSizeX.UnitEnvironment.DefaultUnit);
				_edSizeY.SelectedQuantity = new Units.DimensionfulQuantity(value.Y, Units.Length.Point.Instance).AsQuantityIn(_edSizeY.UnitEnvironment.DefaultUnit);
			}
		}


		public PointD2D SourceSize
		{
			set
			{
				_guiSrcSizeX.SelectedQuantity = new Units.DimensionfulQuantity(value.X, Units.Length.Point.Instance).AsQuantityIn(_guiSrcSizeX.UnitEnvironment.DefaultUnit);
				_guiSrcSizeY.SelectedQuantity = new Units.DimensionfulQuantity(value.Y, Units.Length.Point.Instance).AsQuantityIn(_guiSrcSizeY.UnitEnvironment.DefaultUnit);
			}
		}

		public PointD2D DocScale
		{
			get
			{
				var x = _edScaleX.SelectedQuantityInSIUnits;
				var y = _edScaleY.SelectedQuantityInSIUnits;
				return new PointD2D(x, y);
			}
			set
			{
				_edScaleX.SelectedQuantityInSIUnits = value.X;
				_edScaleY.SelectedQuantityInSIUnits = value.Y;
			}
		}
		public double DocRotation
		{
			get
			{
				return _edRotation.SelectedQuantityAsValueInDegrees;
			}
			set
			{
				_edRotation.SelectedQuantityAsValueInDegrees = value;
			}
		}

		public double DocShear
		{
			get
			{
				return _edShear.SelectedQuantityInSIUnits;
			}
			set
			{
				_edShear.SelectedQuantityInSIUnits = value;
			}
		}

		public Altaxo.Graph.AspectRatioPreservingMode AspectPreserving
		{
			get
			{
				if (true == _guiKeepAspectX.IsChecked)
					return Altaxo.Graph.AspectRatioPreservingMode.PreserveXPriority;
				else if (true == _guiKeepAspectY.IsChecked)
					return Altaxo.Graph.AspectRatioPreservingMode.PreserveYPriority;
				else
					return Altaxo.Graph.AspectRatioPreservingMode.None;
			}
			set
			{
				if (value == Altaxo.Graph.AspectRatioPreservingMode.PreserveXPriority)
					_guiKeepAspectX.IsChecked = true;
				else if (value == Altaxo.Graph.AspectRatioPreservingMode.PreserveYPriority)
					_guiKeepAspectY.IsChecked = true;
				else
					_guiKeepAspectNo.IsChecked = true;
			}
		}

		public bool IsSizeCalculationBasedOnSourceSize
		{
			get
			{
				return _guiScaleWithSource.IsChecked == true;
			}
			set
			{
				if (value)
					_guiScaleWithSource.IsChecked = true;
				else
					_guiScaleWithAbs.IsChecked = true;
			}
		}

		public event Action AspectPreservingChanged;
		private void EhKeepAspectChanged(object sender, RoutedEventArgs e)
		{
			if (null != AspectPreservingChanged)
				AspectPreservingChanged();
		}

		public event Action ScalingModeChanged;
		private void EhScalingModeChanged(object sender, RoutedEventArgs e)
		{
			if (null != ScalingModeChanged)
				ScalingModeChanged();
		}

		public event Action ScaleXChanged;
		private void EhScaleXChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (null != ScaleXChanged)
				ScaleXChanged();
		}

		public event Action ScaleYChanged;
		private void EhScaleYChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (null != ScaleYChanged)
				ScaleYChanged();
		}

		public event Action SizeXChanged;
		private void EhSizeXChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (null != SizeXChanged)
				SizeXChanged();
		}

		public event Action SizeYChanged;
		private void EhSizeYChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (null != SizeYChanged)
				SizeYChanged();
		}
	}
}
