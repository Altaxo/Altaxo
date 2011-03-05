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

namespace Altaxo.Gui.Common
{
	public class RichTextBoxWithDefaultSize : RichTextBox
	{

		#region Dependency property
		public double DefaultWidth
		{
			get { return (double)GetValue(DefaultWidthProperty); }
			set { SetValue(DefaultWidthProperty, value); }
		}
		public double DefaultHeigth
		{
			get { return (double)GetValue(DefaultHeightProperty); }
			set { SetValue(DefaultHeightProperty, value); }
		}

		public static readonly DependencyProperty DefaultWidthProperty =
				DependencyProperty.Register("DefaultWidth", typeof(double), typeof(RichTextBoxWithDefaultSize),
				new FrameworkPropertyMetadata(100.0d));

		public static readonly DependencyProperty DefaultHeightProperty =
				DependencyProperty.Register("DefaultHeight", typeof(double), typeof(RichTextBoxWithDefaultSize),
				new FrameworkPropertyMetadata(100.0d));
		#endregion






		protected override Size MeasureOverride(Size constraint)
		{
			double w = constraint.Width;
			double h = constraint.Height;

			if (!IsLoaded)
				return new Size(DefaultWidth, DefaultHeigth);
			else
				return new Size(ActualWidth, ActualHeight);
		}
	}
}
