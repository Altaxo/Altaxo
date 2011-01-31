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

namespace Altaxo.Gui.Common.Drawing
{
	/// <summary>
	/// Interaction logic for LineCapSizeComboBox.xaml
	/// </summary>
	public partial class LineCapSizeComboBox : EditableImageComboBox
	{
		public LineCapSizeComboBox()
		{
			InitializeComponent();

			var binding = new Binding();
			binding.Source = this;
			binding.Path = new PropertyPath("Thickness");
			//binding.ValidationRules.Add(new ValidationWithErrorString(this.EhValidateText));
			this.SetBinding(ComboBox.TextProperty, binding);
		}


		#region Dependency property
		public double LineCapSize
		{
			get { var result = (double)GetValue(LineCapSizeProperty); return result; }
			set { SetValue(LineCapSizeProperty, value); }
		}

		public static readonly DependencyProperty LineCapSizeProperty =
				DependencyProperty.Register("LineCapSize", typeof(double), typeof(LineThicknessComboBox),
				new FrameworkPropertyMetadata(OnLineCapSizeChanged));

		private static void OnLineCapSizeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{

		}
		#endregion

		protected override void SetImageFromContent()
		{
			if (null == _img)
				return;
			double val;
			if (Altaxo.Serialization.GUIConversion.IsDouble(this.Text, out val))
			{
				_img.Source = GetImage(val);
			}
		}


		public override string GetItemText(object item)
		{
			var value = (double)item;
			return Altaxo.Serialization.GUIConversion.ToString(value);
		}

		public override ImageSource GetItemImage(object item)
		{
			var value = (double)item;
			return base.GetItemImage(item);
		}
			

		public static DrawingImage GetImage(double thickness)
		{
			double height = 24;
			double width = 48;

			GeometryGroup geometryGroup = new GeometryGroup();
			geometryGroup.Children.Add(new LineGeometry(new Point(0, height / 2), new Point(width, height / 2)));
			GeometryDrawing aGeometryDrawing = new GeometryDrawing();
			aGeometryDrawing.Geometry = geometryGroup;

			// Outline the drawing with a solid color.
			aGeometryDrawing.Pen = new Pen(Brushes.Black, Math.Min(height, thickness));

			//
			// Use a DrawingImage and an Image control
			// to display the drawing.
			//
			DrawingImage geometryImage = new DrawingImage(aGeometryDrawing);

			// Freeze the DrawingImage for performance benefits.
			geometryImage.Freeze();
			return geometryImage;
		}
	}
}
