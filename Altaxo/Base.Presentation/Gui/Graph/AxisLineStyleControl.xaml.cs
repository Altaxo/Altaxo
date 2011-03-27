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
	using Altaxo.Collections;
	using Altaxo.Graph.Gdi;
	using Altaxo.Gui.Common.Drawing;

	/// <summary>
	/// Interaction logic for AxisLineStyleControl.xaml
	/// </summary>
	public partial class AxisLineStyleControl : UserControl, IAxisLineStyleView
	{
		PenControlsGlue _linePenGlue;
		PenControlsGlue _majorPenGlue;
		PenControlsGlue _minorPenGlue;

		public AxisLineStyleControl()
		{
			InitializeComponent();

			_linePenGlue = new PenControlsGlue(false);
			_linePenGlue.CbBrush = _lineBrushColor;
			_linePenGlue.CbLineThickness = _lineLineThickness;


			_majorPenGlue = new PenControlsGlue(false);
			_majorPenGlue.CbBrush = _majorLineColor;
			_majorPenGlue.CbLineThickness = _lineMajorThickness;

			_minorPenGlue = new PenControlsGlue(false);
			_minorPenGlue.CbBrush = _minorLineColor;
			_minorPenGlue.CbLineThickness = _lineMinorThickness;

			_linePenGlue.PenChanged += new EventHandler(EhLinePen_Changed);
		}

		void EhLinePen_Changed(object sender, EventArgs e)
		{
			if (false==_chkCustomMajorColor.IsChecked)
			{
				if (this._majorPenGlue.Pen != null)
					this._majorPenGlue.Pen.BrushHolder = _linePenGlue.Pen.BrushHolder;
			}
			if (false==_chkCustomMinorColor.IsChecked)
			{
				if (this._minorPenGlue.Pen != null)
					this._minorPenGlue.Pen.BrushHolder = _linePenGlue.Pen.BrushHolder;
			}

			if (false==_chkCustomMajorThickness.IsChecked)
				_lineMajorThickness.SelectedThickness = _lineLineThickness.SelectedThickness;
			if (false==_chkCustomMinorThickness.IsChecked)
				_lineMinorThickness.SelectedThickness = _lineLineThickness.SelectedThickness;

		}

		private void EhIndividualMajorColor_CheckChanged(object sender, RoutedEventArgs e)
		{
			if (false==_chkCustomMajorColor.IsChecked)
				_majorLineColor.SelectedBrush = _lineBrushColor.SelectedBrush;
			_majorLineColor.IsEnabled = true==_chkCustomMajorColor.IsChecked;
		}

		private void EhIndividualMajorThickness_CheckChanged(object sender, RoutedEventArgs e)
		{
			if (false==_chkCustomMajorThickness.IsChecked)
				_lineMajorThickness.SelectedThickness = _lineLineThickness.SelectedThickness;
			_lineMajorThickness.IsEnabled = true==_chkCustomMajorThickness.IsChecked;
		}

		private void EhIndividualMinorColor_CheckChanged(object sender, RoutedEventArgs e)
		{
			if (false == _chkCustomMinorColor.IsChecked)
				_minorLineColor.SelectedBrush = _lineBrushColor.SelectedBrush;
			_minorLineColor.IsEnabled = true == _chkCustomMinorColor.IsChecked;
		}

		private void EhIndividualMinorThickness_CheckChanged(object sender, RoutedEventArgs e)
		{
			if (false == _chkCustomMinorThickness.IsChecked)
				_lineMinorThickness.SelectedThickness = _lineLineThickness.SelectedThickness;
			_lineMinorThickness.IsEnabled = true == _chkCustomMinorThickness.IsChecked;
		}

		#region Helper

		bool CustomMajorThickness
		{
			set
			{
				_chkCustomMajorThickness.IsChecked = value;
				_lineMajorThickness.IsEnabled = value;

			}
		}
		bool CustomMinorThickness
		{
			set
			{
				_chkCustomMinorThickness.IsChecked = value;
				_lineMinorThickness.IsEnabled = value;

			}
		}

		bool CustomMajorColor
		{
			set
			{
				this._chkCustomMajorColor.IsChecked = value;
				this._majorLineColor.IsEnabled = value;
			}
		}
		bool CustomMinorColor
		{
			set
			{
				this._chkCustomMinorColor.IsChecked = value;
				this._minorLineColor.IsEnabled = value;
			}
		}

		#endregion

		#region IAxisLineStyleView

		public bool ShowLine
		{
			get
			{
				return true==_chkEnableLine.IsChecked;
			}
			set
			{
				_chkEnableLine.IsChecked = value;
			}
		}

		public Altaxo.Graph.Gdi.PenX LinePen
		{
			get
			{
				return _linePenGlue.Pen;
			}
			set
			{
				_linePenGlue.Pen = value;
			}
		}

		public Altaxo.Graph.Gdi.PenX MajorPen
		{
			get
			{
				return _majorPenGlue.Pen;
			}
			set
			{
				_majorPenGlue.Pen = value;
				if (value != null)
				{
					CustomMajorColor = !PenX.AreEqualUnlessWidth(value, _linePenGlue.Pen);
					CustomMajorThickness = (value.Width != _linePenGlue.Pen.Width);
				}
			}
		}

		public Altaxo.Graph.Gdi.PenX MinorPen
		{
			get
			{
				return _minorPenGlue.Pen;
			}
			set
			{
				_minorPenGlue.Pen = value;
				if (value != null)
				{
					CustomMinorColor = !PenX.AreEqualUnlessWidth(value, _linePenGlue.Pen);
					CustomMinorThickness = (value.Width != _linePenGlue.Pen.Width);
				}
			}
		}

		public double MajorTickLength
		{
			get
			{
				return _lineMajorLength.SelectedThickness;
			}
			set
			{
				_lineMajorLength.SelectedThickness = value;
			}
		}

		public double MinorTickLength
		{
			get
			{
				return _lineMinorLength.SelectedThickness;
			}
			set
			{
				_lineMinorLength.SelectedThickness = value;
			}
		}

		public Collections.SelectableListNodeList MajorPenTicks
		{
			get
			{
				SelectableListNodeList list = new SelectableListNodeList();
				foreach (CheckBox chk in _majorWhichTicksLayout.Children)
				{
					SelectableListNode n = new SelectableListNode(chk.Content as string, chk.Tag, true==chk.IsChecked);
					list.Add(n);
				}
				return list;
			}
			set
			{
				_majorWhichTicksLayout.Children.Clear();
				foreach (SelectableListNode n in value)
				{
					CheckBox chk = new CheckBox();
					chk.Content = n.Name;
					chk.Tag = n.Item;
					chk.IsChecked = n.IsSelected;
					_majorWhichTicksLayout.Children.Add(chk);
				}
			}
		}

		public Collections.SelectableListNodeList MinorPenTicks
		{
			get
			{
				SelectableListNodeList list = new SelectableListNodeList();
				foreach (CheckBox chk in _minorWhichTicksLayout.Children)
				{
					SelectableListNode n = new SelectableListNode(chk.Content as string, chk.Tag, true==chk.IsChecked);
					list.Add(n);
				}
				return list;
			}
			set
			{
				_minorWhichTicksLayout.Children.Clear();
				foreach (SelectableListNode n in value)
				{
					CheckBox chk = new CheckBox();
					chk.Content = n.Name;
					chk.Tag = n.Item;
					chk.IsChecked = n.IsSelected;
					_minorWhichTicksLayout.Children.Add(chk);
				}
			}
		}

		#endregion
	}
}
