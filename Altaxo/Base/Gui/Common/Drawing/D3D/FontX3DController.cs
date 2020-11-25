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

#nullable disable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Altaxo.Drawing;
using Altaxo.Drawing.D3D;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Common.Drawing.D3D
{
  public interface IFontX3DView
  {
    string SelectedFontFamilyName { get; set; }

    double SelectedFontSize { get; set; }

    double SelectedFontDepth { get; set; }
  }

  [ExpectedTypeOfView(typeof(IFontX3DView))]
  public class FontX3DController : MVCANControllerEditImmutableDocBase<FontX3D, IFontX3DView>
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
        _view.SelectedFontFamilyName = GdiFontManager.GetValidFontFamilyName(_doc.Font);
        _view.SelectedFontSize = _doc.Size;
        _view.SelectedFontDepth = _doc.Depth;
      }
    }

    private void ApplyFontFamily()
    {
      var ff = _view.SelectedFontFamilyName;

      // make sure that regular style is available
      if (GdiFontManager.IsFontFamilyAndStyleAvailable(ff, FontXStyle.Regular))
        _doc = _doc.WithFamily(ff).WithStyle(FontXStyle.Regular);
      else if (GdiFontManager.IsFontFamilyAndStyleAvailable(ff, FontXStyle.Bold))
        _doc = _doc.WithFamily(ff).WithStyle(FontXStyle.Bold);
      else if (GdiFontManager.IsFontFamilyAndStyleAvailable(ff, FontXStyle.Italic))
        _doc = _doc.WithFamily(ff).WithStyle(FontXStyle.Italic);
      else if (GdiFontManager.IsFontFamilyAndStyleAvailable(ff, FontXStyle.Bold | FontXStyle.Italic))
        _doc = _doc.WithFamily(ff).WithStyle(FontXStyle.Bold | FontXStyle.Italic);
    }

    private void ApplyFontSize()
    {
      var newSize = _view.SelectedFontSize;
      _doc = _doc.WithSize(_view.SelectedFontSize);
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
