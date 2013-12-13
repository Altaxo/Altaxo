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

#endregion Copyright

using Altaxo.Collections;
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
	using Altaxo.Graph;

	/// <summary>
	/// Interaction logic for LayerPositionControl.xaml
	/// </summary>
	public partial class ItemLocationDirectControl : UserControl, IItemLocationDirectView
	{
		public ItemLocationDirectControl()
		{
			InitializeComponent();
		}

		public void InitializeXPosition(Units.DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env)
		{
			_guiXPosition.UnitEnvironment = env;
			_guiXPosition.SelectedQuantity = x;
		}

		public void InitializeYPosition(Units.DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env)
		{
			_guiYPosition.UnitEnvironment = env;
			_guiYPosition.SelectedQuantity = x;
		}

		public void ShowSizeElements(bool isVisible, bool isEnabled)
		{
			var vis = isVisible ? Visibility.Visible : Visibility.Collapsed;
			_guiXSize.Visibility = vis;
			_guiXSize.IsEnabled = isEnabled;
			_guiYSize.Visibility = vis;
			_guiYSize.IsEnabled = isEnabled;
			_guiXSizeLabel.Visibility = vis;
			_guiYSizeLabel.Visibility = vis;
		}

		public void ShowScaleElements(bool isVisible, bool isEnabled)
		{
			var vis = isVisible ? Visibility.Visible : Visibility.Collapsed;

			_guiScaleX.Visibility = vis;
			_guiScaleX.IsEnabled = isEnabled;
			_guiScaleY.Visibility = vis;
			_guiScaleY.IsEnabled = isEnabled;
			_guiLabelScaleX.Visibility = vis;
			_guiLabelScaleY.Visibility = vis;
		}

		public void InitializeYSize(Units.DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env)
		{
			_guiYSize.UnitEnvironment = env;
			_guiYSize.SelectedQuantity = x;
		}

		public void InitializeXSize(Units.DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env)
		{
			_guiXSize.UnitEnvironment = env;
			_guiXSize.SelectedQuantity = x;
		}

		public Units.DimensionfulQuantity XPosition
		{
			get { return _guiXPosition.SelectedQuantity; }
		}

		public Units.DimensionfulQuantity YPosition
		{
			get { return _guiYPosition.SelectedQuantity; }
		}

		public new Units.DimensionfulQuantity XSize
		{
			get { return _guiXSize.SelectedQuantity; }
		}

		public new Units.DimensionfulQuantity YSize
		{
			get { return _guiYSize.SelectedQuantity; }
		}

		public double Rotation
		{
			get
			{
				return _guiRotation.SelectedQuantityAsValueInDegrees;
			}
			set
			{
				_guiRotation.SelectedQuantityAsValueInDegrees = value;
			}
		}

		public double Shear
		{
			get
			{
				return _guiShear.SelectedQuantityInSIUnits;
			}
			set
			{
				_guiShear.SelectedQuantityInSIUnits = value;
			}
		}

		public double ScaleX
		{
			get
			{
				return _guiScaleX.SelectedQuantityInSIUnits;
			}
			set
			{
				_guiScaleX.SelectedQuantityInSIUnits = value;
			}
		}

		public double ScaleY
		{
			get
			{
				return _guiScaleY.SelectedQuantityInSIUnits;
			}
			set
			{
				_guiScaleY.SelectedQuantityInSIUnits = value;
			}
		}

		public void InitializePivot(RADouble pivotX, RADouble pivotY, PointD2D sizeOfTextGraphic)
		{
			_guiAnchoring.SetSelectedPivot(pivotX, pivotY, sizeOfTextGraphic);
		}

		public RADouble PivotX
		{
			get
			{
				return _guiAnchoring.SelectedPivotX;
			}
		}

		public RADouble PivotY
		{
			get
			{
				return _guiAnchoring.SelectedPivotY;
			}
		}

		public void InitializeReference(RADouble pivotX, RADouble pivotY, PointD2D sizeOfTextGraphic)
		{
			_guiParentReferencePoint.SetSelectedPivot(pivotX, pivotY, sizeOfTextGraphic);
		}

		public RADouble ReferenceX
		{
			get
			{
				return _guiParentReferencePoint.SelectedPivotX;
			}
		}

		public RADouble ReferenceY
		{
			get
			{
				return _guiParentReferencePoint.SelectedPivotY;
			}
		}

		public event Action SizeXChanged;

		private void EhSizeXChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var actn = SizeXChanged;
			if (null != actn)
				actn();
		}

		public event Action SizeYChanged;

		private void EhSizeYChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var actn = SizeYChanged;
			if (null != actn)
				actn();
		}

		public event Action ScaleXChanged;

		private void EhScaleXChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var actn = ScaleXChanged;
			if (null != actn)
				actn();
		}

		public event Action ScaleYChanged;

		private void EhScaleYChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var actn = ScaleYChanged;
			if (null != actn)
				actn();
		}
	}
}