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
using Altaxo.Gui.Common.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using sd = System.Drawing;

namespace Altaxo.Gui.Graph3D
{
	using Altaxo.Graph.Graph3D;

	public class GdiFontGlue : Altaxo.Gui.Graph.GdiFontGlue
	{
		private double _fontDepth = 1;

		public double FontDepth
		{
			get { return _fontDepth; }
			set
			{
				var oldValue = _fontDepth;
				_fontDepth = value;
				if (_guiFontDepth != null && oldValue != value)
					_guiFontDepth.SelectedQuantityAsValueInPoints = value;
			}
		}

		public new FontX3D SelectedFont
		{
			get
			{
				return new FontX3D(base.SelectedFont, _fontDepth);
			}
			set
			{
				base.SelectedFont = value.Font;
				FontDepth = value.Depth;
			}
		}

		private FontSizeComboBox _guiFontDepth;

		public FontSizeComboBox GuiFontDepth
		{
			get { return _guiFontDepth; }
			set
			{
				if (null != _guiFontDepth)
					_guiFontDepth.SelectedQuantityChanged -= EhSelectedFontDepthChanged;

				_guiFontDepth = value;
				_guiFontDepth.SelectedQuantityAsValueInPoints = _fontDepth;

				if (null != _guiFontDepth)
					_guiFontDepth.SelectedQuantityChanged += EhSelectedFontDepthChanged;
			}
		}

		private void EhSelectedFontDepthChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var oldFontSize = _fontDepth;
			_fontDepth = _guiFontDepth.SelectedQuantityAsValueInPoints;

			if (oldFontSize != _fontDepth)
				OnFontChanged();
		}
	}
}