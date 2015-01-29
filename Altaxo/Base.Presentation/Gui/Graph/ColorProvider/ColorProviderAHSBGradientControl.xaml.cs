#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2012 Dr. Dirk Lellinger
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
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Graph.ColorProvider
{
	/// <summary>
	/// Interaction logic for ColorProviderAHSBGradientControl.xaml
	/// </summary>
	public partial class ColorProviderAHSBGradientControl : UserControl, IColorProviderAHSBGradientView
	{
		public event Action ChoiceChanged;

		public ColorProviderAHSBGradientControl()
		{
			InitializeComponent();
		}

		public IColorProviderBaseView BaseView
		{
			get { return _guiBaseControl; }
		}

		public double Hue0
		{
			get
			{
				return _guiHue0.ValueAsDouble;
			}
			set
			{
				_guiHue0.ValueAsDouble = value;
			}
		}

		public double Hue1
		{
			get
			{
				return _guiHue1.ValueAsDouble;
			}
			set
			{
				_guiHue1.ValueAsDouble = value;
			}
		}

		public double Saturation0
		{
			get
			{
				return _guiSaturation0.ValueAsDouble;
			}
			set
			{
				_guiSaturation0.ValueAsDouble = value;
			}
		}

		public double Saturation1
		{
			get
			{
				return _guiSaturation1.ValueAsDouble;
			}
			set
			{
				_guiSaturation1.ValueAsDouble = value;
			}
		}

		public double Brightness0
		{
			get
			{
				return _guiBrightness0.ValueAsDouble;
			}
			set
			{
				_guiBrightness0.ValueAsDouble = value;
			}
		}

		public double Brightness1
		{
			get
			{
				return _guiBrightness1.ValueAsDouble;
			}
			set
			{
				_guiBrightness1.ValueAsDouble = value;
			}
		}

		public double Opaqueness0
		{
			get
			{
				return _guiOpaqueness0.ValueAsDouble;
			}
			set
			{
				_guiOpaqueness0.ValueAsDouble = value;
			}
		}

		public double Opaqueness1
		{
			get
			{
				return _guiOpaqueness1.ValueAsDouble;
			}
			set
			{
				_guiOpaqueness1.ValueAsDouble = value;
			}
		}

		private void EhDoubleUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
		{
			if (null != ChoiceChanged)
				ChoiceChanged();
		}
	}
}