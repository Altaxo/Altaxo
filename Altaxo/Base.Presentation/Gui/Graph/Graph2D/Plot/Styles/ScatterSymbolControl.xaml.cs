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

using Altaxo.Collections;
using Altaxo.Drawing;
using Altaxo.Graph.Graph2D.Plot.Styles;
using Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols;
using Altaxo.Gui.Graph.Gdi.Plot.Styles;
using Altaxo.Settings;
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

namespace Altaxo.Gui.Graph.Graph2D.Plot.Styles
{
	/// <summary>
	/// Interaction logic for ScatterSymbolControl.xaml
	/// </summary>
	public partial class ScatterSymbolControl : UserControl, IScatterSymbolView
	{
		public ScatterSymbolControl()
		{
			InitializeComponent();
		}

		public NamedColor FillColor
		{
			get
			{
				return _guiFillColor.SelectedColor;
			}

			set
			{
				_guiFillColor.SelectedColor = value;
			}
		}

		public SelectableListNodeList FrameChoices
		{
			set
			{
				GuiHelper.Initialize(_guiFrame, value);
			}
		}

		public NamedColor FrameColor
		{
			get
			{
				return _guiFrameColor.SelectedColor;
			}

			set
			{
				_guiFrameColor.SelectedColor = value;
			}
		}

		public SelectableListNodeList InsetChoices
		{
			set
			{
				GuiHelper.Initialize(_guiInset, value);
			}
		}

		public NamedColor InsetColor
		{
			get
			{
				return _guiInsetColor.SelectedColor;
			}

			set
			{
				_guiInsetColor.SelectedColor = value;
			}
		}

		public PlotColorInfluence PlotColorInfluence
		{
			get
			{
				return _guiPlotColorInfluence.SelectedValue;
			}

			set
			{
				_guiPlotColorInfluence.SelectedValue = value;
			}
		}

		public double RelativeStructureWidth
		{
			get
			{
				return _guiStructureWidth.SelectedQuantityAsValueInSIUnits;
			}

			set
			{
				_guiStructureWidth.SelectedQuantityAsValueInSIUnits = value;
			}
		}

		public SelectableListNodeList ShapeChoices
		{
			set
			{
				GuiHelper.Initialize(_guiShape, value);
			}
		}

		public event Action<NamedColor> FillColorChanged;

		public event Action<Type> FrameChanged;

		public event Action<NamedColor> FrameColorChanged;

		public event Action<Type> InsetChanged;

		public event Action<NamedColor> InsetColorChanged;

		public event Action<PlotColorInfluence> PlotColorInfluenceChanged;

		public event Action<double> RelativeStructureWidthChanged;

		public event Action<Type> ShapeChanged;

		private void EhShapeChanged(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiShape);
			ShapeChanged?.Invoke(((SelectableListNode)_guiShape.SelectedValue).Tag as Type);
		}

		private void EhStructureWidthChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			RelativeStructureWidthChanged?.Invoke(_guiStructureWidth.SelectedQuantityAsValueInSIUnits);
		}

		private void EhFrameChanged(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiFrame);
			FrameChanged?.Invoke(((SelectableListNode)_guiFrame.SelectedValue).Tag as Type);
		}

		private void EhInsetChanged(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiInset);
			InsetChanged?.Invoke(((SelectableListNode)_guiInset.SelectedValue).Tag as Type);
		}

		private void EhPlotColorInfluenceChanged(object sender, EventArgs e)
		{
			PlotColorInfluenceChanged?.Invoke(_guiPlotColorInfluence.SelectedValue);
		}

		private void EhFillColorChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			FillColorChanged?.Invoke(_guiFillColor.SelectedColor);
		}

		private void EhFrameColorChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			FrameColorChanged?.Invoke(_guiFrameColor.SelectedColor);
		}

		private void EhInsetColorChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			InsetColorChanged?.Invoke(_guiInsetColor.SelectedColor);
		}

		private ScatterSymbolToImageSourceConverter _converterBlack_White;
		private ScatterSymbolToImageSourceConverter _converterBlue_Golden;
		private ScatterSymbolToImageSourceConverter _converterGolden_Blue;

		public IScatterSymbol ScatterSymbolForPreview
		{
			set
			{
				const double symbolSize = 32;
				if (_converterBlack_White == null)
					_converterBlack_White = new ScatterSymbolToImageSourceConverter() { PlotColor = NamedColors.Black, SymbolSize = symbolSize };

				if (_converterBlue_Golden == null)
					_converterBlue_Golden = new ScatterSymbolToImageSourceConverter() { PlotColor = NamedColors.Blue, SymbolSize = symbolSize };

				if (_converterGolden_Blue == null)
					_converterGolden_Blue = new ScatterSymbolToImageSourceConverter() { PlotColor = NamedColors.Goldenrod, SymbolSize = symbolSize };

				_guiPreviewBlack_White.Source = (ImageSource)_converterBlack_White.Convert(value, typeof(ImageSource), null, GuiCulture.Instance); ;
				_guiPreviewBlue_Goldenrod.Source = (ImageSource)_converterBlue_Golden.Convert(value, typeof(ImageSource), null, GuiCulture.Instance); ;
				_guiPreviewGoldenrod_Blue.Source = (ImageSource)_converterGolden_Blue.Convert(value, typeof(ImageSource), null, GuiCulture.Instance); ;
			}
		}
	}
}