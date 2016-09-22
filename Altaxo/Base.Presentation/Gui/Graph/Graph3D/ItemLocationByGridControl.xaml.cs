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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Altaxo.Gui.Graph.Graph3D
{
	/// <summary>
	/// Interaction logic for LayerPositionControl.xaml
	/// </summary>
	public partial class ItemLocationByGridControl : UserControl, IItemLocationByGridView
	{
		public ItemLocationByGridControl()
		{
			InitializeComponent();
		}

		public double RotationX
		{
			get
			{
				return _guiRotationX.SelectedQuantityAsValueInDegrees;
			}
			set
			{
				_guiRotationX.SelectedQuantityAsValueInDegrees = value;
			}
		}

		public double RotationY
		{
			get
			{
				return _guiRotationY.SelectedQuantityAsValueInDegrees;
			}
			set
			{
				_guiRotationY.SelectedQuantityAsValueInDegrees = value;
			}
		}

		public double RotationZ
		{
			get
			{
				return _guiRotationZ.SelectedQuantityAsValueInDegrees;
			}
			set
			{
				_guiRotationZ.SelectedQuantityAsValueInDegrees = value;
			}
		}

		public double ShearX
		{
			get
			{
				return _guiShearX.SelectedQuantityInSIUnits;
			}
			set
			{
				_guiShearX.SelectedQuantityInSIUnits = value;
			}
		}

		public double ShearY
		{
			get
			{
				return _guiShearY.SelectedQuantityInSIUnits;
			}
			set
			{
				_guiShearY.SelectedQuantityInSIUnits = value;
			}
		}

		public double ShearZ
		{
			get
			{
				return _guiShearZ.SelectedQuantityInSIUnits;
			}
			set
			{
				_guiShearZ.SelectedQuantityInSIUnits = value;
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

		public double ScaleZ
		{
			get
			{
				return _guiScaleZ.SelectedQuantityInSIUnits;
			}
			set
			{
				_guiScaleZ.SelectedQuantityInSIUnits = value;
			}
		}

		public double GridPosX
		{
			get
			{
				return _guiPosX.Value;
			}
			set
			{
				_guiPosX.Value = value;
			}
		}

		public double GridPosY
		{
			get
			{
				return _guiPosY.Value;
			}
			set
			{
				_guiPosY.Value = value;
			}
		}

		public double GridPosZ
		{
			get
			{
				return _guiPosZ.Value;
			}
			set
			{
				_guiPosZ.Value = value;
			}
		}

		public double GridSpanX
		{
			get
			{
				return _guiSpanX.Value;
			}
			set
			{
				_guiSpanX.Value = value;
			}
		}

		public double GridSpanY
		{
			get
			{
				return _guiSpanY.Value;
			}
			set
			{
				_guiSpanY.Value = value;
			}
		}

		public double GridSpanZ
		{
			get
			{
				return _guiSpanZ.Value;
			}
			set
			{
				_guiSpanZ.Value = value;
			}
		}

		public bool ForceFitIntoCell
		{
			get
			{
				return true == _guiForceFitIntoCell.IsChecked;
			}
			set
			{
				_guiForceFitIntoCell.IsChecked = value;
			}
		}
	}
}