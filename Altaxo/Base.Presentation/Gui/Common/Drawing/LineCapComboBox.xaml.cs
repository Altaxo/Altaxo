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

using sdd = System.Drawing.Drawing2D;
using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Common.Drawing
{
	/// <summary>
	/// Interaction logic for LineJoinComboBox.xaml
	/// </summary>
	public partial class LineCapComboBox : ImageComboBox
	{
		#region Converter

		class Converter : IValueConverter
		{
			LineCapComboBox _cb;

			public Converter(LineCapComboBox c)
			{
				_cb = c;
			}

			public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				var val = (LineCapEx)value;
				if (val.IsDefaultStyle)
					return _cb._cachedItems[LineCapEx.Flat.Name];
				else 
					return _cb._cachedItems[val.Name];


			}

			public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				return ((ImageComboBoxItem)value).Value;
			}
		}

		#endregion


		static Dictionary<string, ImageSource> _cachedImagesForStartCap = new Dictionary<string, ImageSource>();
		static Dictionary<string, ImageSource> _cachedImagesForEndCap = new Dictionary<string, ImageSource>();

		Dictionary<string, ImageComboBoxItem> _cachedItems = new Dictionary<string, ImageComboBoxItem>();

		static GdiToWpfBitmap _interopBitmap;


		bool _isForEndCap;

		
		static SortedDictionary<string, int> _lineCaps;

		public LineCapComboBox()
		{
			InitializeComponent();
			SetDefaultValues();

			var binding = new Binding();
			binding.Source = this;
			binding.Path = new PropertyPath(_nameOfValueProp);
			binding.Converter = new Converter(this);
			this.SetBinding(ComboBox.SelectedItemProperty, binding);
		}

		public bool IsForEndCap
		{
			get { return _isForEndCap; }
			set { _isForEndCap = value; }
		}

		void SetDefaultValues()
		{
			foreach (LineCapEx cap in LineCapEx.GetValues())
			{
				var item = new ImageComboBoxItem(this, cap);
				_cachedItems.Add(cap.Name,item);
				this.Items.Add(item);
			}
		}

	

		#region Dependency property
		private const string _nameOfValueProp = "SelectedLineCap";
		public LineCapEx SelectedLineCap
		{
			get { return (LineCapEx)GetValue(SelectedLineCapProperty); }
			set	{	SetValue(SelectedLineCapProperty, value); }
		}

		public static readonly DependencyProperty SelectedLineCapProperty =
				DependencyProperty.Register(_nameOfValueProp, typeof(LineCapEx), typeof(LineCapComboBox),
				new FrameworkPropertyMetadata(LineCapEx.Flat, OnSelectedLineCapChanged));

		private static void OnSelectedLineCapChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{

		}
		#endregion



		public override string GetItemText(object item)
		{
			var value = (LineCapEx)item;
			return value.Name;
		}


		public override ImageSource GetItemImage(object item)
		{
			var val = (LineCapEx)item;
			ImageSource result;
			if (_isForEndCap)
			{
				if (!_cachedImagesForEndCap.TryGetValue(val.Name, out result))
					_cachedImagesForEndCap.Add(val.Name, result = GetImage(val, _isForEndCap));
			}
			else
			{
				if (!_cachedImagesForStartCap.TryGetValue(val.Name, out result))
					_cachedImagesForStartCap.Add(val.Name, result = GetImage(val, _isForEndCap));
			}
			return result;
		}

		public static ImageSource GetImage(LineCapEx join, bool isForEndCap)
		{
			
			
			const int bmpHeight = 24;
			const int bmpWidth = 48;
			const double nominalHeight = 24; // height of a combobox item
			const double nominalWidth = (nominalHeight*bmpWidth)/bmpHeight;
			const double lineWidth = bmpHeight*0.4;

			if (null == _interopBitmap)
				_interopBitmap = new GdiToWpfBitmap(bmpWidth, bmpHeight);

			var grfx = _interopBitmap.GdiGraphics;

			grfx.CompositingMode = sdd.CompositingMode.SourceCopy;
			grfx.FillRectangle(System.Drawing.Brushes.Transparent, 0, 0, bmpWidth, bmpHeight);

      var linePen = new System.Drawing.Pen(System.Drawing.Brushes.Black, (float)Math.Ceiling(lineWidth));
      if (isForEndCap)
      {
        join.SetPenEndCap(linePen);
        grfx.DrawLine(linePen, 0, 0.5f * bmpHeight, bmpWidth*(1 - 0.25f), 0.5f * bmpHeight );
      }
      else
      {
        join.SetPenStartCap(linePen);
        grfx.DrawLine(linePen, 0.25f*bmpWidth, 0.5f * bmpHeight,  bmpWidth, 0.5f * bmpHeight);
      }

			var img = new WriteableBitmap(_interopBitmap.WpfBitmap);
			img.Freeze();
			return img;
		}
	}
}
