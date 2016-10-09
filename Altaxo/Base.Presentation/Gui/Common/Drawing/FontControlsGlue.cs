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

using Altaxo.Drawing;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Gui.Common.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Altaxo.Gui.Drawing
{
	public class FontXControlsGlue : FrameworkElement
	{
		public FontXControlsGlue()
		{
		}

		#region Font

		private FontX _fontX;

		protected virtual FontX FontX
		{
			get
			{
				return _fontX;
			}
			set
			{
				_fontX = value;
			}
		}

		public FontX SelectedFont
		{
			get
			{
				return FontX;
			}
			set
			{
				FontX = value;

				if (null != CbFontFamily) CbFontFamily.SelectedFontFamily = GdiFontManager.GdiFontFamily(FontX);
				if (null != _cbFontStyle) CbFontStyle.SelectedFontStyle = FontX.Style;
				if (null != CbFontSize) CbFontSize.SelectedQuantityAsValueInPoints = FontX.Size;
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
				if (FontX != null && _cbFontFamily != null)
					_cbFontFamily.SelectedFontFamily = GdiFontManager.GdiFontFamily(FontX);

				if (_cbFontFamily != null)
					dpd.AddValueChanged(_cbFontFamily, EhFontFamily_SelectionChangeCommitted);
			}
		}

		private void EhFontFamily_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (FontX != null)
			{
				FontX = FontX.WithFamily(_cbFontFamily.SelectedFontFamily.Name);
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
				if (FontX != null && _cbFontStyle != null)
					_cbFontStyle.SelectedFontStyle = FontX.Style;

				if (_cbFontStyle != null)
				{
					dpd.AddValueChanged(_cbFontStyle, EhFontStyle_SelectionChangeCommitted);
				}
			}
		}

		private void EhFontStyle_SelectionChangeCommitted(object sender, EventArgs e)
		{
			if (FontX != null)
			{
				FontX = FontX.WithStyle(_cbFontStyle.SelectedFontStyle);
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
				if (FontX != null && _cbFontSize != null)
					_cbFontSize.SelectedQuantityAsValueInPoints = FontX.Size;

				if (_cbFontSize != null)
				{
					_cbFontSize.SelectedQuantityChanged += EhFontSize_SelectionChangeCommitted;
				}
			}
		}

		private void EhFontSize_SelectionChangeCommitted(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
		{
			if (FontX != null)
			{
				FontX = FontX.WithSize(_cbFontSize.SelectedQuantityAsValueInPoints);
				OnSelectedFontChanged();
			}
		}

		#endregion Size
	}

	public class FontX3DControlsGlue : FontXControlsGlue
	{
		private Altaxo.Drawing.D3D.FontX3D _fontX3D;

		protected override FontX FontX
		{
			get
			{
				return _fontX3D?.Font;
			}
			set
			{
				_fontX3D = new Altaxo.Drawing.D3D.FontX3D(value, _fontX3D.Depth);
			}
		}

		protected virtual Altaxo.Drawing.D3D.FontX3D FontX3D
		{
			get
			{
				return _fontX3D;
			}
			set
			{
				_fontX3D = value;
			}
		}

		public new Altaxo.Drawing.D3D.FontX3D SelectedFont
		{
			get
			{
				return _fontX3D;
			}
			set
			{
				_fontX3D = value;

				if (null != CbFontFamily) CbFontFamily.SelectedFontFamily = GdiFontManager.GdiFontFamily(FontX);
				if (null != CbFontStyle) CbFontStyle.SelectedFontStyle = FontX.Style;
				if (null != CbFontSize) CbFontSize.SelectedQuantityAsValueInPoints = FontX.Size;
				if (null != CbFontDepth) CbFontDepth.SelectedQuantityAsValueInPoints = FontX3D.Depth;
			}
		}

		#region Size

		private FontSizeComboBox _cbFontDepth;

		public FontSizeComboBox CbFontDepth
		{
			get { return _cbFontDepth; }
			set
			{
				if (_cbFontDepth != null)
				{
					_cbFontDepth.SelectedQuantityChanged -= EhFontDepth_SelectionChangeCommitted;
				}

				_cbFontDepth = value;
				if (FontX3D != null && _cbFontDepth != null)
					_cbFontDepth.SelectedQuantityAsValueInPoints = FontX3D.Depth;

				if (_cbFontDepth != null)
				{
					_cbFontDepth.SelectedQuantityChanged += EhFontDepth_SelectionChangeCommitted;
				}
			}
		}

		private void EhFontDepth_SelectionChangeCommitted(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
		{
			if (FontX != null)
			{
				FontX3D = FontX3D.WithDepth(_cbFontDepth.SelectedQuantityAsValueInPoints);
				OnSelectedFontChanged();
			}
		}

		#endregion Size
	}
}