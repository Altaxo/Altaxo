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

using Altaxo.Serialization;

namespace Altaxo.Gui.Graph
{
	/// <summary>
	/// Interaction logic for ArrangeLayersControl.xaml
	/// </summary>
	[UserControlForController(typeof(IArrangeLayersViewEventSink))]
	public partial class ArrangeLayersControl : UserControl, IArrangeLayersView
	{
		public ArrangeLayersControl()
		{
			InitializeComponent();
		}

		private void _edNumberOfRows_Validating(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			if (_controller != null)
				_controller.EhNumberOfRowsChanged(_edNumberOfRows.Value);
		}

		private void _edNumberOfColumns_Validating(object sender, RoutedPropertyChangedEventArgs<int> e)
		{
			if (_controller != null)
				_controller.EhNumberOfColumnsChanged(_edNumberOfColumns.Value);
		}

		private void _edNumberOfColumns_Validating(object sender, ValidationEventArgs<string> e)
		{

		}

		private void _edTopMargin_Validating(object sender, ValidationEventArgs<string> e)
		{
			bool Cancel = false;
			if (_controller != null)
				Cancel |= _controller.EhTopMarginChanged(e.ValueToValidate);

			if (Cancel)
				e.AddError("The provided string could not be converted to a numeric value");
		}

		private void _edLeftMargin_Validating(object sender, ValidationEventArgs<string> e)
		{
			bool Cancel = false;
			if (_controller != null)
				Cancel |= _controller.EhLeftMarginChanged(e.ValueToValidate);

			if (Cancel)
				e.AddError("The provided string could not be converted to a numeric value");
		}

		private void _edBottomMargin_Validating(object sender, ValidationEventArgs<string> e)
		{
			bool Cancel = false;
			if (_controller != null)
				Cancel |= _controller.EhBottomMarginChanged(e.ValueToValidate);

			if (Cancel)
				e.AddError("The provided string could not be converted to a numeric value");
		}

		private void _edRightMargin_Validating(object sender, ValidationEventArgs<string> e)
		{
			bool Cancel = false;
			if (_controller != null)
				Cancel |= _controller.EhRightMarginChanged(e.ValueToValidate);

			if (Cancel)
				e.AddError("The provided string could not be converted to a numeric value");
		}

		private void _cbSuperfluousLayersAction_SelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
		{
			if (_controller != null)
				_controller.EhSuperfluousLayersActionChanged((Altaxo.Collections.SelectableListNode)_cbSuperfluousLayersAction.SelectedItem);
		}

		#region IArrangeLayersView

		IArrangeLayersViewEventSink _controller;
		public IArrangeLayersViewEventSink Controller
		{
			get
			{

				return _controller;
			}
			set
			{
				_controller = value;
			}
		}

		public void InitializeRowsColumns(int numRows, int numColumns)
		{
			this._edNumberOfRows.Value = numRows;
			this._edNumberOfColumns.Value = numColumns;
		}

		public void InitializeSpacing(double rowSpacing, double columnSpacing)
		{
			this._edRowSpacing.Text = GUIConversion.ToString(columnSpacing);
			this._edColumnSpacing.Text = GUIConversion.ToString(rowSpacing);
		}

		public void InitializeMargins(double top, double left, double bottom, double right)
		{
			this._edTopMargin.Text = GUIConversion.ToString(top);
			this._edBottomMargin.Text = GUIConversion.ToString(bottom);
			this._edRightMargin.Text = GUIConversion.ToString(right);
			this._edLeftMargin.Text = GUIConversion.ToString(left);
		}

		public void InitializeSuperfluosLayersQuestion(Collections.SelectableListNodeList list)
		{
			GuiHelper.Initialize(_cbSuperfluousLayersAction, list);
		}

		public void InitializeEnableConditions(bool rowSpacingEnabled, bool columnSpacingEnabled, bool superfluousEnabled)
		{
			_edRowSpacing.IsEnabled = rowSpacingEnabled;
			_edColumnSpacing.IsEnabled = columnSpacingEnabled;
			_cbSuperfluousLayersAction.IsEnabled = superfluousEnabled;
		}

		#endregion IArrangeLayersView

	
	}
}
