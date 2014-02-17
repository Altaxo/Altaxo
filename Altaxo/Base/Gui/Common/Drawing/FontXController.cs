#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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
using System.Drawing;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Common.Drawing
{
	public interface IFontXView
	{
		System.Drawing.FontFamily SelectedFontFamily { get; set; }

		double SelectedFontSize { get; set; }
	}

	[ExpectedTypeOfView(typeof(IFontXView))]
	public class FontXController : MVCANControllerBase<FontX, IFontXView>
	{
		protected override void Initialize(bool initData)
		{
			if (initData)
			{
			}
			if (null != _view)
			{
				// fill the font name combobox with all fonts
				_view.SelectedFontFamily = _doc.GdiFontFamily();
				_view.SelectedFontSize = _doc.Size;
			}
		}

		private void ApplyFontFamily()
		{
			FontFamily ff = _view.SelectedFontFamily;
			// make sure that regular style is available
			if (ff.IsStyleAvailable(FontStyle.Regular))
				this._doc = GdiFontManager.GetFont(ff, this._doc.Size, FontStyle.Regular);
			else if (ff.IsStyleAvailable(FontStyle.Bold))
				this._doc = GdiFontManager.GetFont(ff, this._doc.Size, FontStyle.Bold);
			else if (ff.IsStyleAvailable(FontStyle.Italic))
				this._doc = GdiFontManager.GetFont(ff, this._doc.Size, FontStyle.Italic);
		}

		private void ApplyFontSize()
		{
			var newSize = _view.SelectedFontSize;
			FontX oldFont = this._doc;
			this._doc = oldFont.GetFontWithNewSize(newSize);
		}

		public override bool Apply()
		{
			ApplyFontFamily();
			ApplyFontSize();

			_originalDoc = _doc; // this is safe because FontX is a immutable class

			return true;
		}
	}
}