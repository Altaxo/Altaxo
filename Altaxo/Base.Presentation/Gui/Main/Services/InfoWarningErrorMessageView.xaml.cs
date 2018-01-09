using Altaxo.Main.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

namespace Altaxo.Gui.Main.Services
{
	/// <summary>
	/// Interaction logic for MessageView.xaml
	/// </summary>
	public partial class InfoWarningErrorMessageView : UserControl, IInfoWarningErrorMessageView
	{
		#region Helper class

		private class ListReverser : System.Collections.IEnumerable
		{
			private System.Collections.IList _baseList;

			public ListReverser(System.Collections.IList baseList)
			{
				_baseList = baseList;
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				for (int i = _baseList.Count - 1; i >= 0; i--)
					yield return _baseList[i];
			}
		}

		#endregion Helper class

		public InfoWarningErrorMessageView()
		{
			InitializeComponent();

			// We apply for the DragCompletedEvent in order to get notified when a column was resized (please not the "true" as last argument - we get notified even if the event is already handled)
			_listView.AddHandler(System.Windows.Controls.Primitives.Thumb.DragCompletedEvent, new RoutedEventHandler(EhListView_ColumnResized), true);
		}

		#region IMessageView Members

		public int[] ColumnWidths
		{
			get
			{
				return GuiHelper.GetColumnWidths(_listView);
			}
			set
			{
				GuiHelper.SetColumnWidths(_listView, value);
			}
		}

		#endregion IMessageView Members

		private void ResizeLastColumnToFitExactly()
		{
			var remaining = _listView.ActualWidth - _col0.ActualWidth - _col1.ActualWidth - _col2.ActualWidth;
			if (remaining > 20)
				_col3.Width = remaining;
		}

		private bool _isViewDirectionRecentIsFirst;

		private void EhContextMenuOpened(object sender, RoutedEventArgs e)
		{
			_menuItemRecentFirst.IsChecked = _isViewDirectionRecentIsFirst;
			_menuItemRecentLast.IsChecked = !_isViewDirectionRecentIsFirst;
		}

		private void EhListView_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			ResizeLastColumnToFitExactly();
		}

		private void EhListView_ColumnResized(object sender, RoutedEventArgs e)
		{
			ResizeLastColumnToFitExactly();
		}
	}

	[ValueConversion(typeof(MessageLevel), typeof(Brush))]
	public class MessageLevelToBrushConverter : IValueConverter
	{
		private static Brush _infoBrush = new SolidColorBrush(Colors.LightGreen);
		private static Brush _warningBrush = new SolidColorBrush(Colors.Yellow);
		private static Brush _errorBrush = new SolidColorBrush(Colors.LightPink);

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var c1 = (MessageLevel)value;
			switch (c1)
			{
				case MessageLevel.Info:
					return _infoBrush;

				case MessageLevel.Warning:
					return _warningBrush;

				case MessageLevel.Error:
					return _errorBrush;

				default:
					throw new NotImplementedException("Unknown MessageLevel");
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}