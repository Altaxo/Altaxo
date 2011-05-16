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

using Altaxo.Graph;
using Altaxo.Gui.Common.Drawing;
using Altaxo.Science;

namespace Altaxo.Gui.Graph
{
	/// <summary>
	/// Interaction logic for XYPlotLabelStyleControl.xaml
	/// </summary>
	public partial class XYPlotLabelStyleControl : UserControl, IXYPlotLabelStyleView
	{
		FontControlsGlue _fontControlsGlue;
		BackgroundControlsGlue _backgroundGlue;
		public event Action LabelColumnSelected;
		public event Action FontSizeChanged;

		public XYPlotLabelStyleControl()
		{
			InitializeComponent();

			_fontControlsGlue = new FontControlsGlue() { CbFontFamily = _cbFontFamily, CbFontStyle = _cbFontStyle, CbFontSize = _cbFontSize };
			_fontControlsGlue.SelectedFontChanged += EhFontSizeChanged;
			_backgroundGlue = new BackgroundControlsGlue() { CbStyle = _cbBackgroundStyle, CbBrush = _cbBackgroundBrush };
		}

		


		private void EhSelectLabelColumn_Click(object sender, RoutedEventArgs e)
		{
			if (null != LabelColumnSelected)
				LabelColumnSelected();
		}

		private void EhFontSizeChanged(object sender, EventArgs e)
		{
			if (null != FontSizeChanged)
				FontSizeChanged();
		}

		private void EhIndependentColor_CheckChanged(object sender, RoutedEventArgs e)
		{
				this._cbColor.IsEnabled = true==_chkIndependentColor.IsChecked;
		}

		private void EhAttachToAxis_CheckedChanged(object sender, RoutedEventArgs e)
		{
			this._cbAttachedAxis.IsEnabled = true==_chkAttachToAxis.IsChecked;
		}
		

		#region IXYPlotLabelStyleView

		public void Init_LabelColumn(string labelColumnAsText)
		{
			this._edLabelColumn.Text = labelColumnAsText;
		}


		public new Altaxo.Graph.Gdi.Background.IBackgroundStyle Background
		{
			get
			{
				return _backgroundGlue.BackgroundStyle;
			}
			set
			{
				_backgroundGlue.BackgroundStyle = value;
			}
		}


		public double SelectedRotation
		{
			get
			{
				return this._cbRotation.SelectedRotation;
			}
			set
			{
				this._cbRotation.SelectedRotation = value;
			}
		}

		public void Init_XOffset(QuantityWithUnitGuiEnvironment environment, QuantityWithUnit value)
		{
			this._edXOffset.UnitEnvironment = environment;
			this._edXOffset.SelectedQuantity = value;
		}

		public void Init_YOffset(QuantityWithUnitGuiEnvironment environment, QuantityWithUnit value)
		{
			this._edYOffset.UnitEnvironment = environment;
			this._edYOffset.SelectedQuantity = value;
		}

		public QuantityWithUnit XOffset
		{
			get { return _edXOffset.SelectedQuantity; }
		}
		public QuantityWithUnit YOffset
		{
			get { return _edYOffset.SelectedQuantity; }
		}

		public System.Drawing.Font SelectedFont
		{
			get
			{
				return _fontControlsGlue.SelectedFont;
			}
			set
			{
				_fontControlsGlue.SelectedFont = value;
			}
		}

		public NamedColor SelectedColor
		{
			get
			{
				return _cbColor.SelectedColor;
			}
			set
			{
				_cbColor.SelectedColor = value;
			}
		}

		public void Init_HorizontalAlignment(Collections.SelectableListNodeList list)
		{
			GuiHelper.Initialize(_cbHorizontalAlignment, list);
		}

		public Collections.ListNode SelectedHorizontalAlignment
		{
			get { return (Collections.ListNode)_cbHorizontalAlignment.SelectedItem; }
		}

		public void Init_VerticalAlignment(Collections.SelectableListNodeList list)
		{
			GuiHelper.Initialize(_cbVerticalAlignment, list);
		}

		public Collections.ListNode SelectedVerticalAlignment
		{
			get { return (Collections.ListNode)_cbVerticalAlignment.SelectedItem; }
		}

		public bool AttachToAxis
		{
			get
			{
				return true == _chkAttachToAxis.IsChecked;
			}
			set
			{
				_chkAttachToAxis.IsChecked = value;
				_cbAttachedAxis.IsEnabled = value;
			}
		}

		public void Init_AttachedAxis(Collections.SelectableListNodeList names)
		{
			GuiHelper.Initialize(_cbAttachedAxis, names);
		}

		public Collections.ListNode AttachedAxis
		{
			get { return (Collections.ListNode)_cbAttachedAxis.SelectedItem; }
		}

		public bool IsIndependentColorSelected
		{
			get
			{
				return true == _chkIndependentColor.IsChecked;
			}
			set
			{
				_chkIndependentColor.IsChecked = value;
				_cbColor.IsEnabled = value;
			}
		}


		#endregion

	








	
	}
}
