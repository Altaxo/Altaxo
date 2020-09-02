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

#nullable disable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Altaxo.Drawing;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Common.Drawing
{
  public interface IFontXView
  {
    string SelectedFontFamilyName { get; set; }

    double SelectedFontSize { get; set; }
  }

  [ExpectedTypeOfView(typeof(IFontXView))]
  public class FontXController : MVCANControllerEditImmutableDocBase<FontX, IFontXView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
      }
      if (_view is not null)
      {
        // fill the font name combobox with all fonts
        _view.SelectedFontFamilyName = GdiFontManager.GetValidFontFamilyName(_doc);
        _view.SelectedFontSize = _doc.Size;
      }
    }

    private void ApplyFontFamily()
    {
      var ff = _view.SelectedFontFamilyName;

      // make sure that regular style is available
      if (GdiFontManager.IsFontFamilyAndStyleAvailable(ff, FontXStyle.Regular))
        _doc = GdiFontManager.GetFontX(ff, _doc.Size, FontXStyle.Regular);
      else if (GdiFontManager.IsFontFamilyAndStyleAvailable(ff, FontXStyle.Bold))
        _doc = GdiFontManager.GetFontX(ff, _doc.Size, FontXStyle.Bold);
      else if (GdiFontManager.IsFontFamilyAndStyleAvailable(ff, FontXStyle.Italic))
        _doc = GdiFontManager.GetFontX(ff, _doc.Size, FontXStyle.Italic);
      else if (GdiFontManager.IsFontFamilyAndStyleAvailable(ff, FontXStyle.Bold | FontXStyle.Italic))
        _doc = GdiFontManager.GetFontX(ff, _doc.Size, FontXStyle.Bold | FontXStyle.Italic);
    }

    private void ApplyFontSize()
    {
      var newSize = _view.SelectedFontSize;
      FontX oldFont = _doc;
      _doc = oldFont.WithSize(newSize);
    }

    public override bool Apply(bool disposeController)
    {
      ApplyFontFamily();
      ApplyFontSize();

      _originalDoc = _doc; // this is safe because FontX is an immutable class

      return ApplyEnd(true, disposeController);
    }
  }
}
