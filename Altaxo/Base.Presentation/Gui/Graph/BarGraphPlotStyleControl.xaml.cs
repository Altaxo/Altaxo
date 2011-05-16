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
	using Altaxo.Gui.Common;
	using Altaxo.Gui.Common.Drawing;

	/// <summary>
	/// Interaction logic for BarGraphPlotStyleControl.xaml
	/// </summary>
	public partial class BarGraphPlotStyleControl : UserControl, IBarGraphPlotStyleView
	{
		PenControlsGlue _framePenGlue;

		public BarGraphPlotStyleControl()
		{
			InitializeComponent();
			_framePenGlue = new PenControlsGlue(false);
			_framePenGlue.CbBrush = _cbPenColor;
		}

		private void _chkFrameBar_CheckedChanged(object sender, RoutedEventArgs e)
		{

		}

		private void _chkUsePreviousItem_CheckedChanged(object sender, RoutedEventArgs e)
		{

		}

		#region IBarGraphPlotStyleView

	 public bool IndependentColor
    {
      get
      {
        return true==_chkIndependentColor.IsChecked;
      }
      set
      {
        _chkIndependentColor.IsChecked = value;
      }
    }

	 public Altaxo.Graph.Gdi.BrushX FillBrush
	 {
		 get
		 {
			 return this._cbFillBrush.SelectedBrush;
		 }
		 set
		 {
			 _cbFillBrush.SelectedBrush = value;
		 }
	 }

	 public Altaxo.Graph.Gdi.PenX FillPen
	 {
		 get
		 {
			 if (true==_chkFrameBar.IsChecked)
				 return _framePenGlue.Pen;
			 else
				 return null;
		 }
		 set
		 {
			 _chkFrameBar.IsChecked = (value != null);
			 _cbPenColor.IsEnabled = (value != null);
			 if (value != null)
				 _framePenGlue.Pen = value;
			 else
				 _framePenGlue.Pen = new Altaxo.Graph.Gdi.PenX(NamedColor.Black);
		 }
	 }

	 public string InnerGap
	 {
		 get
		 {
			 return _edInnerGap.Text;
		 }
		 set
		 {
			 _edInnerGap.Text = value;
		 }
	 }

	 public string OuterGap
	 {
		 get
		 {
			 return _edOuterGap.Text;
		 }
		 set
		 {
			 _edOuterGap.Text = value;
		 }
	 }

	 public bool UsePhysicalBaseValue
	 {
		 get
		 {
			 return false;
		 }
		 set
		 {
		 }
	 }

	 public string BaseValue
	 {
		 get
		 {
			 return _edBaseValue.Text;
		 }
		 set
		 {
			 _edBaseValue.Text = value;
		 }
	 }

	 public bool StartAtPreviousItem
	 {
		 get
		 {
			 return true==_chkUsePreviousItem.IsChecked;
		 }
		 set
		 {
			 _chkUsePreviousItem.IsChecked = value;
			 _edYGap.IsEnabled = value;
		 }
	 }

	 public string YGap
	 {
		 get
		 {
			 return _edYGap.Text;
		 }
		 set
		 {
			 _edYGap.Text = value;
		 }
	 }

		#endregion
	}
}
