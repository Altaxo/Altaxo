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
	/// Interaction logic for LineThicknessComboBox.xaml
	/// </summary>
	public partial class LineThicknessComboBox : EditableImageComboBox
	{
		public LineThicknessComboBox()
		{
			InitializeComponent();

			var binding = new Binding();
			binding.Source = this;
			binding.Path = new PropertyPath("Thickness");
			//binding.ValidationRules.Add(new ValidationWithErrorString(this.EhValidateText));
			this.SetBinding(ComboBox.TextProperty, binding);
		}
	

	#region Dependency property
		public double Thickness
		{
			get { var result = (double)GetValue(ThicknessProperty); return result; }
			set { SetValue(ThicknessProperty, value); }
		}

		public static readonly DependencyProperty ThicknessProperty =
				DependencyProperty.Register("Thickness", typeof(double), typeof(LineThicknessComboBox),
				new FrameworkPropertyMetadata(OnThicknessChanged));

		private static void OnThicknessChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{

		}
		#endregion

		protected override void SetImageFromContent()
		{
			if (null == _img)
				return;
			double val;
			if(Altaxo.Serialization.GUIConversion.IsDouble(this.Text,out val))
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
