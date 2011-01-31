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
using System.Reflection;
using System.Diagnostics;

namespace Altaxo.Gui.Common.Drawing
{
	public class ImageComboBox : ComboBox
	{
		static double _standardHeight;
		protected double _relativeImageWidth = 1;


		public ImageComboBox()
		{
		}
		

		public static double StandardHeight
		{
			get
			{
				return 0!=_standardHeight ? _standardHeight : 24;
			}
		}

		public double RelativeImageWidth
		{
			get
			{
				return _relativeImageWidth;
			}
			set
			{
				_relativeImageWidth = value;
			}
		}
		

		protected override Size MeasureOverride(Size constraint)
		{
			var result = base.MeasureOverride(constraint);

			return result;
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			if (_standardHeight == 0)
				_standardHeight = sizeInfo.NewSize.Height;

			base.OnRenderSizeChanged(sizeInfo);
		}

		public virtual ImageSource GetItemImage(object item)
		{
			return null;
		}

		public virtual string GetItemText(object item)
		{
			return string.Empty;
		}
	}
}
