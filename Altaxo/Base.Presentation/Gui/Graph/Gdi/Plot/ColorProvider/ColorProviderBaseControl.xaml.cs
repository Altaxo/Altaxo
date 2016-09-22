#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

namespace Altaxo.Gui.Graph.Gdi.Plot.ColorProvider
{
	using Altaxo.Drawing;
	using Altaxo.Graph;

	/// <summary>
	/// Interaction logic for ColorProviderBaseControl.xaml
	/// </summary>
	public partial class ColorProviderBaseControl : UserControl, IColorProviderBaseView
	{
		public ColorProviderBaseControl()
		{
			InitializeComponent();
		}

		protected virtual void OnChoiceChanged()
		{
			if (null != ChoiceChanged)
				ChoiceChanged();
		}

		#region IColorProviderBaseView Members

		public event Action ChoiceChanged;

		public NamedColor ColorBelow
		{
			get
			{
				return _cbColorBelow.SelectedColor;
			}
			set
			{
				_cbColorBelow.SelectedColor = value;
			}
		}

		public NamedColor ColorAbove
		{
			get
			{
				return _cbColorAbove.SelectedColor;
			}
			set
			{
				_cbColorAbove.SelectedColor = value;
			}
		}

		public NamedColor ColorInvalid
		{
			get
			{
				return _cbInvalid.SelectedColor;
			}
			set
			{
				_cbInvalid.SelectedColor = value;
			}
		}

		public double Transparency
		{
			get
			{
				return (double)(_edTransparency.Value / 100);
			}
			set
			{
				_edTransparency.Value = (decimal)(value * 100);
			}
		}

		public int ColorSteps
		{
			get
			{
				return _edColorSteps.Value; ;
			}
			set
			{
				_edColorSteps.Value = value;
			}
		}

		#endregion IColorProviderBaseView Members

		private void EhColorBelowChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			OnChoiceChanged();
		}

		private void EhColorAboveChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			OnChoiceChanged();
		}

		private void EhColorInvalidChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			OnChoiceChanged();
		}

		private void EhTransparencyChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
		{
			OnChoiceChanged();
		}

		private void EhColorStepsChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			OnChoiceChanged();
		}
	}
}