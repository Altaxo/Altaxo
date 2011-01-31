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

namespace Altaxo.Gui.Common.Drawing
{
	/// <summary>
	/// ComboBox for <see cref="Altaxo.Graph.Gdi.TextureImage"/>.
	/// </summary>
	public partial class TextureImageComboBox : ImageComboBox
	{
		class CC : IValueConverter
		{
			TextureImageComboBox _cb;
			object _originalToolTip;
			bool _hasValidationError;

			public CC(TextureImageComboBox c)
			{
				_cb = c;
			}

			public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				if (value != null)
				{
					var val = (ImageProxy)value;
					return _cb._cachedItems[val.ContentHash];
				}
				else
				{
					return null;
				}
			}

			public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				var it = (ImageComboBoxItem)value;
				var pair = (KeyValuePair<string, ImageProxy>)(it.Value);
				return pair.Value;
			}
		}

		/// <summary>Key is the content hash of the image proxy, value is the cached image.</summary>
		static Dictionary<string, ImageSource> _cachedImages = new Dictionary<string, ImageSource>();

		/// <summary>Cached items. Key is the content hash of the image proxy, value is the combobox item.</summary>
		Dictionary<string, ImageComboBoxItem> _cachedItems = new Dictionary<string, ImageComboBoxItem>();

		static TextureImageComboBox()
		{
		}

		public TextureImageComboBox()
		{
			InitializeComponent();

			SetDataSource();
		

			var _valueBinding = new Binding();
			_valueBinding.Source = this;
			_valueBinding.Path = new PropertyPath(_nameOfValueProp);
			_valueBinding.Converter = new CC(this);
			this.SetBinding(ComboBox.SelectedItemProperty, _valueBinding);
		}


		void SetDataSource()
		{
			_cachedItems.Clear();

			ImageComboBoxItem it;

			foreach (KeyValuePair<string, ImageProxy> pair in TextureManager.BuiltinTextures)
			{
				_cachedItems.Add(pair.Value.ContentHash, it=new ImageComboBoxItem(this, pair));
				this.Items.Add(it);
			}

			foreach (KeyValuePair<string, ImageProxy> pair in TextureManager.UserTextures)
			{
				_cachedItems.Add(pair.Value.ContentHash, it = new ImageComboBoxItem(this, pair));
				this.Items.Add(it);
			}
		}

		void AddImage(ImageProxy img)
		{
			if (_cachedItems.ContainsKey(img.ContentHash))
				return;
			ImageComboBoxItem it;
			_cachedItems.Add(img.ContentHash, it = new ImageComboBoxItem(this, new KeyValuePair<string, ImageProxy>(img.Name, img)));
			this.Items.Add(it);
		}

		#region Dependency property
		private const string _nameOfValueProp = "TextureImage";
		public ImageProxy TextureImage
		{
			get { return (ImageProxy)GetValue(TextureImageProperty); }
			set {	SetValue(TextureImageProperty, value); }
		}

		public static readonly DependencyProperty TextureImageProperty =
				DependencyProperty.Register(_nameOfValueProp, typeof(ImageProxy), typeof(TextureImageComboBox),
				new FrameworkPropertyMetadata(OnTextureImageChanged));

		private static void OnTextureImageChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((TextureImageComboBox)obj).EhTextureImageChanged(obj,args);
		}
		#endregion

		protected virtual void EhTextureImageChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{

		}


		void EhLoadFromFile(object sender, EventArgs e)
		{
			OpenFileOptions options = new OpenFileOptions();
			options.AddFilter("*.*", "All files (*.*");
			options.FilterIndex = 0;
			if (Current.Gui.ShowOpenFileDialog(options))
			{
				ImageProxy img = ImageProxy.FromFile(options.FileName);
				if (img.IsValid)
				{
					TextureManager.UserTextures.Add(img);
					AddImage(img);
					TextureImage = img;
				}
			}
		}
	

		public override string GetItemText(object item)
		{
			var val = (KeyValuePair<string,ImageProxy>)item;
			return val.Key;
		}

		public override ImageSource GetItemImage(object item)
		{
			var val = (KeyValuePair<string, ImageProxy>)item;
			ImageSource result;
			if (!_cachedImages.TryGetValue(val.Key, out result))
				_cachedImages.Add(val.Key, result = GetImage(val.Value));
			return result;
		}


		public static DrawingImage GetImage(ImageProxy val)
		{
			double height = 1;
			double width = 2;

			//
			// Create the Geometry to draw.
			//
			GeometryGroup geometryGroup = new GeometryGroup();
			geometryGroup.Children.Add(new RectangleGeometry(new Rect(0,0,width,height)));

			var geometryDrawing = new GeometryDrawing() { Geometry = geometryGroup };
			geometryDrawing.Pen = new Pen(Brushes.Black, 1);
		

			DrawingImage geometryImage = new DrawingImage(geometryDrawing);

			// Freeze the DrawingImage for performance benefits.
			geometryImage.Freeze();
			return geometryImage;
		}
	}
}
