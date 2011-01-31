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
	public partial class DashCapComboBox : ImageComboBox
	{
		bool _isForEndCap;

		
		static SortedDictionary<string, int> _lineCaps;

		public DashCapComboBox()
		{
			InitializeComponent();


			SetDefaultValues();
		}

		public bool IsForEndCap
		{
			get { return _isForEndCap; }
			set { _isForEndCap = value; }
		}

		void SetDefaultValues()
		{
			_lineCaps = new SortedDictionary<string, int>();

			int i = 0;
			foreach (LineCapEx cap in LineCapEx.GetValues())
			{
				_lineCaps.Add(cap.Name, i);
				this.Items.Add(new ImageComboBoxItem(this,cap));
				++i;
			}
		}

	

		#region Dependency property
		public LineCapEx LineCap
		{
			get { var result = (LineCapEx)GetValue(LineCapProperty); return result; }
			set
			{
				SetValue(LineCapProperty, value);
				this.SelectedIndex = _lineCaps[value.Name];
			}
		}

		public static readonly DependencyProperty LineCapProperty =
				DependencyProperty.Register("LineCap", typeof(double), typeof(LineCapComboBox),
				new FrameworkPropertyMetadata(LineCapEx.NoAnchor, OnLineCapChanged));

		private static void OnLineCapChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{

		}
		#endregion




		protected override void OnSelectionChanged(SelectionChangedEventArgs e)
		{
			LineCap = (LineCapEx)((ImageComboBoxItem)this.SelectedItem).Value;
			base.OnSelectionChanged(e);
		}


		public override string GetItemText(object item)
		{
			var value = (LineCapEx)item;
			return value.Name;
		}


		public override ImageSource GetItemImage(object item)
		{
			var value = (LineCapEx)item;
			return GetImage(value, _isForEndCap);
		}

		public static DrawingImage GetImage(LineCapEx join, bool isForEndCap)
		{
			double height = 20;
			double width = 40;
			double lineWidth = 0.375 * height;
			//
			// Create the Geometry to draw.
			//
			GeometryGroup geometryGroup = new GeometryGroup();
			if (isForEndCap)
				geometryGroup.Children.Add(new LineGeometry(new Point(0, height * 0.5), new Point(width * 0.75, height * 0.5)));
			else
				geometryGroup.Children.Add(new LineGeometry(new Point(width * 0.25, height * 0.5), new Point(width, height * 0.5)));
			GeometryDrawing aGeometryDrawing = new GeometryDrawing();
			aGeometryDrawing.Geometry = geometryGroup;
			// Outline the drawing with a solid color.

			var pen = new Pen(Brushes.Black, lineWidth);
			if (isForEndCap)
				pen.EndLineCap = PenLineCap.Square;
			else
				pen.StartLineCap = PenLineCap.Square;

			aGeometryDrawing.Pen = new Pen(Brushes.Black, lineWidth);



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
