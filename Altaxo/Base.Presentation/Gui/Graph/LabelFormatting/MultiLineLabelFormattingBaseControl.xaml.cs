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

namespace Altaxo.Gui.Graph.LabelFormatting
{
	/// <summary>
	/// Interaction logic for MultiLineLabelFormattingBaseControl.xaml
	/// </summary>
	public partial class MultiLineLabelFormattingBaseControl : UserControl, IMultiLineLabelFormattingBaseView
	{
		public MultiLineLabelFormattingBaseControl()
		{
			InitializeComponent();
		}

		public double LineSpacing
		{
			get
			{
				return _guiLineSpacing.SelectedQuantityAsValueInSIUnits;
			}
			set
			{
				_guiLineSpacing.SelectedQuantityAsValueInSIUnits = value;
			}
		}

		public Collections.SelectableListNodeList TextBlockAlignement
		{
			set
			{
				GuiHelper.Initialize(_guiTextBlockAligment, value);
			}
		}

		private void EhTextBlockAligmentChanged(object sender, SelectionChangedEventArgs e)
		{
			GuiHelper.SynchronizeSelectionFromGui(_guiTextBlockAligment);
		}
	}
}
