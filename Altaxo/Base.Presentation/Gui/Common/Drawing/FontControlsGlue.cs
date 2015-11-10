#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Altaxo.Gui.Common.Drawing
{
	public class FontControlsGlue : FrameworkElement
	{
		public FontControlsGlue()
		{
		}

		#region Font

		private FontX _font;

		public FontX SelectedFont
		{
			get
			{
				return _font;
			}
			set
			{
				_font = value;

				if (null != CbFontFamily) CbFontFamily.SelectedFontFamily = GdiFontManager.GdiFontFamily(_font);
				if (null != _cbFontStyle) CbFontStyle.SelectedFontStyle = _font.Style;
				if (null != CbFontSize) CbFontSize.SelectedQuantityAsValueInPoints = _font.Size;
			}
		}

		public event EventHandler SelectedFontChanged;

		protected virtual void OnSelectedFontChanged()
		{
			if (SelectedFontChanged != null)
				SelectedFontChanged(this, EventArgs.Empty);
		}

		#endregion Font

		#region Font

		private FontFamilyComboBox _cbFontFamily;

		public FontFamilyComboBox CbFontFamily
		{
			get { return _cbFontFamily; }
			set
			{
				var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(FontFamilyComboBox.SelectedFontFamilyProperty, typeof(FontFamilyComboBox));
				if (null == dpd)
					throw new InvalidOperationException("DependencePropertyDescriptor is null! Please check the corresponding DependencyProperty");

				if (_cbFontFamily != null)
					dpd.RemoveValueChanged(_cbFontFamily, EhFontFamily_SelectionChangeCommitted);

				_cbFontFamily = value;
				if (_font != null && _cbFontFamily != null)
					_cbFontFamily.SelectedFontFamily = GdiFontManager.GdiFontFamily(_font);

				if (_cbFontFamily != null)
					dpd.AddValueChanged(_cbFontFamily, EhFontFamily_SelectionChangeCommitted);
			}
		}

		private void EhFontFamily_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (_font != null)
			{
				_font = _font.GetFontWithNewFamily(_cbFontFamily.SelectedFontFamily.Name);
				OnSelectedFontChanged();
			}
		}

		#endregion Font

		#region Style

		private FontStyleComboBox _cbFontStyle;

		public FontStyleComboBox CbFontStyle
		{
			get { return _cbFontStyle; }
			set
			{
				var dpd = System.ComponentModel.DependencyPropertyDescriptor.FromProperty(FontStyleComboBox.SelectedFontStyleProperty, typeof(FontStyleComboBox));

				if (_cbFontStyle != null)
				{
					dpd.RemoveValueChanged(_cbFontStyle, EhFontStyle_SelectionChangeCommitted);
				}

				_cbFontStyle = value;
				if (_font != null && _cbFontStyle != null)
					_cbFontStyle.SelectedFontStyle = _font.Style;

				if (_cbFontStyle != null)
				{
					dpd.AddValueChanged(_cbFontStyle, EhFontStyle_SelectionChangeCommitted);
				}
			}
		}

		private void EhFontStyle_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (_font != null)
			{
				_font = _font.GetFontWithNewStyle(_cbFontStyle.SelectedFontStyle);
				OnSelectedFontChanged();
			}
		}

		#endregion Style

		#region Size

		private FontSizeComboBox _cbFontSize;

		public FontSizeComboBox CbFontSize
		{
			get { return _cbFontSize; }
			set
			{
				if (_cbFontSize != null)
				{
					_cbFontSize.SelectedQuantityChanged -= EhFontSize_SelectionChangeCommitted;
				}

				_cbFontSize = value;
				if (_font != null && _cbFontSize != null)
					_cbFontSize.SelectedQuantityAsValueInPoints = _font.Size;

				if (_cbFontSize != null)
				{
					_cbFontSize.SelectedQuantityChanged += EhFontSize_SelectionChangeCommitted;
				}
			}
		}

		private void EhFontSize_SelectionChangeCommitted(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
		{
			if (_font != null)
			{
				_font = _font.GetFontWithNewSize(_cbFontSize.SelectedQuantityAsValueInPoints);
				OnSelectedFontChanged();
			}
		}

		#endregion Size
	}
}