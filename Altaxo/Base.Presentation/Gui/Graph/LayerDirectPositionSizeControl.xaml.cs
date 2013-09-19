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
	/// <summary>
	/// Interaction logic for LayerPositionControl.xaml
	/// </summary>
	public partial class LayerDirectPositionSizeControl : UserControl, ILayerDirectPositionSizeView
	{
		public LayerDirectPositionSizeControl()
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

		public double Scale
		{
			get
			{
				return _guiScale.SelectedQuantityInSIUnits;
			}
			set
			{
				_guiScale.SelectedQuantityInSIUnits = value;
			}
		}
	}
}