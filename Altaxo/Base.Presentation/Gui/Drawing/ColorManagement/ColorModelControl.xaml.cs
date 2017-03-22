#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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
//    along with ctrl program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Altaxo.Gui.Drawing.ColorManagement
{
	using Altaxo.Geometry;
	using Altaxo.Gui.Common;

	/// <summary>
	/// Interaction logic for ColorModelControl.xaml
	/// </summary>
	public partial class ColorModelControl : UserControl
	{
		private Int32UpDown[] _guiComponents = new Int32UpDown[4];
		private TextBox[] _guiAltComponents = new TextBox[4];
		private Label[] _guiLabelForComponents = new Label[4];
		private Label[] _guiLabelForAltComponents = new Label[4];

		private IColorModel _colorModel = new ColorModelRGB();
		private IColorModel _altColorModel = new ColorModelRGB();

		public ColorModelControl()
		{
			InitializeComponent();

			_guiLabelForComponents[0] = _guiLabelForComponent0;
			_guiLabelForComponents[1] = _guiLabelForComponent1;
			_guiLabelForComponents[2] = _guiLabelForComponent2;
			_guiLabelForComponents[3] = _guiLabelForComponent3;

			_guiComponents[0] = _guiComponent0;
			_guiComponents[1] = _guiComponent1;
			_guiComponents[2] = _guiComponent2;
			_guiComponents[3] = _guiComponent3;

			_guiLabelForAltComponents[0] = _guiLabelForAltComponent0;
			_guiLabelForAltComponents[1] = _guiLabelForAltComponent1;
			_guiLabelForAltComponents[2] = _guiLabelForAltComponent2;
			_guiLabelForAltComponents[3] = _guiLabelForAltComponent3;

			_guiAltComponents[0] = _guiAltComponent0;
			_guiAltComponents[1] = _guiAltComponent1;
			_guiAltComponents[2] = _guiAltComponent2;
			_guiAltComponents[3] = _guiAltComponent3;
		}

		/// <summary>
		/// Occurs if a color component was changed by the user.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="RoutedPropertyChangedEventArgs{System.Int32}"/> instance containing the event data.</param>
		private void EhComponentChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			var color = _colorModel.GetColorFromComponents(_guiComponents.Select(c => c.Value).ToArray());
			var pos = _colorModel.GetRelativePositionsFor2Dand1DColorSurfaceFromColor(color);

			_gui1DColorControl.SelectionRectangleRelativePositionChanged -= Eh1DColorControl_ValueChanged;
			_gui2DColorControl.SelectionRectangleRelativePositionChanged -= Eh2DColorControl_ValueChanged;

			_gui2DColorControl.SelectionRectangleRelativePosition = pos.Item1;
			_gui1DColorControl.SelectionRectangleRelativePosition = pos.Item2;

			_gui1DColorControl.SelectionRectangleRelativePositionChanged += Eh1DColorControl_ValueChanged;
			_gui2DColorControl.SelectionRectangleRelativePositionChanged += Eh2DColorControl_ValueChanged;
		}

		private void Eh2DColorControl_ValueChanged(PointD2D relPos)
		{
			double relValue = _gui1DColorControl.SelectionRectangleRelativePosition;
			var baseColor = _colorModel.GetColorFor1DColorSurfaceFromRelativePosition(relValue);
			var currentColor = _colorModel.GetColorFor2DColorSurfaceFromRelativePosition(relPos, baseColor);

			// now calculate components
			var components = _colorModel.GetComponentsForColor(currentColor);

			UpdateComponentValues(() =>
			{
				for (int i = 0; i < components.Length; ++i)
					_guiComponents[i].Value = components[i];
			}
			);
		}

		private void Eh1DColorControl_ValueChanged(double relValue)
		{
			var baseColor = _colorModel.GetColorFor1DColorSurfaceFromRelativePosition(relValue);
			var imgSource = ColorBitmapCreator.GetBitmap(relPos => _colorModel.GetColorFor2DColorSurfaceFromRelativePosition(relPos, baseColor));
			_gui2DColorControl.Set2DColorImage(imgSource);

			var currentColor = _colorModel.GetColorFor2DColorSurfaceFromRelativePosition(_gui2DColorControl.SelectionRectangleRelativePosition, baseColor);

			// now calculate components
			var components = _colorModel.GetComponentsForColor(currentColor);

			UpdateComponentValues(() =>
			{
				for (int i = 0; i < components.Length; ++i)
					_guiComponents[i].Value = components[i];
			}
			);
		}

		private void UpdateComponentValues(Action updateAction)
		{
			_guiComponent0.ValueChanged -= EhComponentChanged;
			_guiComponent1.ValueChanged -= EhComponentChanged;
			_guiComponent2.ValueChanged -= EhComponentChanged;
			_guiComponent3.ValueChanged -= EhComponentChanged;

			updateAction();

			_guiComponent0.ValueChanged += EhComponentChanged;
			_guiComponent1.ValueChanged += EhComponentChanged;
			_guiComponent2.ValueChanged += EhComponentChanged;
			_guiComponent3.ValueChanged += EhComponentChanged;
		}
	}
}