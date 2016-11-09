using Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols;
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

namespace Altaxo.Gui.Graph.Graph2D.Plot.Styles.ScatterSymbols
{
	/// <summary>
	/// Interaction logic for PlotColorInfluenceControl.xaml
	/// </summary>
	public partial class PlotColorInfluenceControl : UserControl
	{
		public PlotColorInfluenceControl()
		{
			InitializeComponent();
		}

		public PlotColorInfluence SelectedValue
		{
			get
			{
				PlotColorInfluence result = PlotColorInfluence.None;

				if (_guiFillAlpha.IsChecked == true) result |= PlotColorInfluence.FillColorPreserveAlpha;
				if (_guiFillFull.IsChecked == true) result |= PlotColorInfluence.FillColorFull;

				if (_guiFrameAlpha.IsChecked == true) result |= PlotColorInfluence.FrameColorPreserveAlpha;
				if (_guiFrameFull.IsChecked == true) result |= PlotColorInfluence.FrameColorFull;

				if (_guiInsetAlpha.IsChecked == true) result |= PlotColorInfluence.InsetColorPreserveAlpha;
				if (_guiInsetFull.IsChecked == true) result |= PlotColorInfluence.InsetColorFull;

				return result;
			}
			set
			{
				if (value.HasFlag(PlotColorInfluence.FillColorFull))
					_guiFillFull.IsChecked = true;
				else if (value.HasFlag(PlotColorInfluence.FillColorPreserveAlpha))
					_guiFillAlpha.IsChecked = true;
				else
					_guiFillNone.IsChecked = true;

				if (value.HasFlag(PlotColorInfluence.FrameColorFull))
					_guiFrameFull.IsChecked = true;
				else if (value.HasFlag(PlotColorInfluence.FrameColorPreserveAlpha))
					_guiFrameAlpha.IsChecked = true;
				else
					_guiFrameNone.IsChecked = true;

				if (value.HasFlag(PlotColorInfluence.InsetColorFull))
					_guiInsetFull.IsChecked = true;
				else if (value.HasFlag(PlotColorInfluence.InsetColorPreserveAlpha))
					_guiInsetAlpha.IsChecked = true;
				else
					_guiInsetNone.IsChecked = true;
			}
		}
	}
}