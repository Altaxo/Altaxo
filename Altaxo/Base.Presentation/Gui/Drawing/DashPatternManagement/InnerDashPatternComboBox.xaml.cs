#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using Altaxo.Drawing;
using Altaxo.Drawing.DashPatterns;
using Altaxo.Gui.Common;
using Altaxo.Gui.Common.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Altaxo.Gui.Drawing.DashPatternManagement
{
	/// <summary>
	/// InnerDashPatternComboBox manages dashpatterns, but without the list manager.
	/// </summary>
	public partial class InnerDashPatternComboBox : EditableImageComboBox
	{
		#region Converter

		private class CC : IValueConverter
		{
			private ComboBox _cb;
			private object _originalToolTip;

			public CC(ComboBox c)
			{
				_cb = c;
			}

			public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				var val = (IDashPattern)value;
				if (!(val is Custom))
					return val.GetType().Name;
				else
				{
					var stb = new StringBuilder();
					var custom = ((Custom)val);
					for (int i = 0; i < custom.Count - 1; ++i)
						stb.AppendFormat("{0}; ", custom[i]);
					stb.AppendFormat("{0}", custom[custom.Count - 1]);
					return stb.ToString();
				}
			}

			private static IDashPattern ConvertFromText(string text, out string error)
			{
				error = null;
				text = text.Trim();
				if (_knownStylesDict.ContainsKey(text))
					return _knownStylesDict[text];

				var parts = text.Split(new char[] { ';', '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
				var valList = new List<double>();
				foreach (var part in parts)
				{
					var parttrimmed = part.Trim();
					if (string.IsNullOrEmpty(parttrimmed))
						continue;

					double val;
					if (!Altaxo.Serialization.GUIConversion.IsDouble(parttrimmed, out val))
						error = "Provided string can not be converted to a numeric value";
					else if (!(val > 0 && val < double.MaxValue))
						error = "One of the provided values is not a valid positive number";
					else
						valList.Add(val);
				}

				if (valList.Count < 1 && error == null) // only use this error, if there is no other error;
					error = "At least one number is neccessary";

				return new Custom(valList);
			}

			public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
			{
				string text = (string)value;
				string error;
				var result = ConvertFromText(text, out error);

				if (error == null)
					return result;
				else
					return Binding.DoNothing;
			}

			public string EhValidateText(object obj, System.Globalization.CultureInfo info)
			{
				string text = (string)obj;
				string error;
				var result = ConvertFromText(text, out error);

				if (null != error)
				{
					_originalToolTip = _cb.ToolTip;
					_cb.ToolTip = error;
				}
				else
				{
					_cb.ToolTip = _originalToolTip;
					_originalToolTip = null;
				}

				return error;
			}
		}

		#endregion Converter

		private static Dictionary<IDashPattern, ImageSource> _cachedImages = new Dictionary<IDashPattern, ImageSource>();
		private CC _valueConverter;
		private static Dictionary<string, IDashPattern> _knownStylesDict = new Dictionary<string, IDashPattern>();
		private static IDashPattern[] _knownStylesList;

		static InnerDashPatternComboBox()
		{
			_knownStylesList = new IDashPattern[] { new Solid(), new Dash(), new Dot(), new DashDot(), new DashDotDot() };

			foreach (var e in _knownStylesList)
				_knownStylesDict.Add(e.GetType().Name, e);
		}

		public InnerDashPatternComboBox()
		{
			InitializeComponent();

			var _valueBinding = new Binding();
			_valueBinding.Source = this;
			_valueBinding.Path = new PropertyPath(_nameOfValueProp);
			_valueConverter = new CC(this);
			_valueBinding.Converter = _valueConverter;
			_valueBinding.ValidationRules.Add(new ValidationWithErrorString(_valueConverter.EhValidateText));
			this.SetBinding(ComboBox.TextProperty, _valueBinding);

			_img.Source = GetImage(SelectedDashStyle);
		}

		#region Dependency property

		private const string _nameOfValueProp = "SelectedDashStyle";

		public IDashPattern SelectedDashStyle
		{
			get { return (IDashPattern)GetValue(SelectedDashStyleProperty); }
			set { SetValue(SelectedDashStyleProperty, value); }
		}

		public static readonly DependencyProperty SelectedDashStyleProperty =
				DependencyProperty.Register(_nameOfValueProp, typeof(IDashPattern), typeof(InnerDashPatternComboBox),
				new FrameworkPropertyMetadata(new Solid(), OnSelectedDashStyleChanged, EhDashPatternCoerce));

		private static object EhDashPatternCoerce(DependencyObject obj, object baseValue)
		{
			if (null == baseValue)
				return new Solid();
			else
				return baseValue;
		}

		private static void OnSelectedDashStyleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((InnerDashPatternComboBox)obj).EhSelectedDashStyleChanged(obj, args);
		}

		#endregion Dependency property

		protected virtual void EhSelectedDashStyleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			if (null != _img)
			{
				var val = (IDashPattern)args.NewValue;
				_img.Source = GetImage(val);
			}
		}

		protected override void OnSelectionChanged(SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count == 1)
			{
				var item = e.AddedItems[0] as ImageComboBoxItem;
				if (null != item)
					SelectedDashStyle = item.Value as IDashPattern;
				e.Handled = true;
			}

			base.OnSelectionChanged(e);
		}

		public override ImageSource GetItemImage(object item)
		{
			var val = (IDashPattern)item;
			ImageSource result;
			if (!_cachedImages.TryGetValue(val, out result))
				_cachedImages.Add(val, result = GetImage(val));
			return result;
		}

		public override string GetItemText(object item)
		{
			return (string)_valueConverter.Convert(item, typeof(string), null, System.Globalization.CultureInfo.CurrentUICulture);
		}

		public static ImageSource GetImage(IDashPattern val)
		{
			const double height = 1;
			const double width = 2;
			const double lineWidth = height / 5;

			DashStyle dashStyle;
			if (val is Solid)
				dashStyle = DashStyles.Solid;
			else if (val is Dash)
				dashStyle = DashStyles.Dash;
			else if (val is Dot)
				dashStyle = DashStyles.Dot;
			else if (val is DashDot)
				dashStyle = DashStyles.DashDot;
			else if (val is DashDotDot)
				dashStyle = DashStyles.DashDotDot;
			else
				dashStyle = new DashStyle(val, 0);

			// draws a transparent outline to fix the borders
			var drawingGroup = new DrawingGroup();

			var geometryDrawing = new GeometryDrawing();
			geometryDrawing.Geometry = new RectangleGeometry(new Rect(0, 0, width, height));
			geometryDrawing.Pen = new Pen(Brushes.Transparent, 0);
			drawingGroup.Children.Add(geometryDrawing);

			geometryDrawing = new GeometryDrawing() { Geometry = new LineGeometry(new Point(0, height / 2), new Point(width, height / 2)) };
			geometryDrawing.Pen = new Pen(Brushes.Black, lineWidth) { DashStyle = dashStyle };
			drawingGroup.Children.Add(geometryDrawing);

			var geometryImage = new DrawingImage(drawingGroup);

			// Freeze the DrawingImage for performance benefits.
			geometryImage.Freeze();
			return geometryImage;
		}
	}
}