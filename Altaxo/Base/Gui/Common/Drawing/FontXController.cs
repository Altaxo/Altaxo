#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
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

using System.Collections.Generic;
using Altaxo.Drawing;
using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Common.Drawing
{
  public interface IFontXView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IFontXView))]
  public class FontXController : MVCANControllerEditImmutableDocBase<FontX, IFontXView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public FontXController()
    {
    }
    public FontXController(FontX doc)
    {
      InitializeDocument(doc);
    }

    #region Bindings

    public string SelectedFontFamilyName
    {
      get => _doc.FontFamilyName;
      set
      {
        if (!(SelectedFontFamilyName == value))
        {
          ApplyFontFamily(value);
          OnPropertyChanged(nameof(SelectedFontFamilyName));
          OnPropertyChanged(nameof(SelectedFontSize));
        }
      }
    }

    public double SelectedFontSize
    {
      get => _doc.Size;
      set
      {
        if (!(SelectedFontSize == value))
        {
          _doc = _doc.WithSize(value);
          OnPropertyChanged(nameof(SelectedFontSize));
        }
      }
    }

    public FontXStyle SelectedFontStyle
    {
      get => _doc.Style;
      set
      {
        if (!(SelectedFontStyle == value))
        {
         _doc = _doc.WithStyle(value);
          OnPropertyChanged(nameof(SelectedFontStyle));
        }
      }
    }


    #endregion


    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        var ff = GdiFontManager.GetValidFontFamilyName(_doc);
        if (!(ff == _doc.FontFamilyName))
        {
          ApplyFontFamily(ff);
        }
      }
    }

    private void ApplyFontFamily(string fontFamilyName)
    {
      // make sure that regular style is available
      if (GdiFontManager.IsFontFamilyAndStyleAvailable(fontFamilyName, FontXStyle.Regular))
        _doc = GdiFontManager.GetFontX(fontFamilyName, _doc.Size, FontXStyle.Regular);
      else if (GdiFontManager.IsFontFamilyAndStyleAvailable(fontFamilyName, FontXStyle.Bold))
        _doc = GdiFontManager.GetFontX(fontFamilyName, _doc.Size, FontXStyle.Bold);
      else if (GdiFontManager.IsFontFamilyAndStyleAvailable(fontFamilyName, FontXStyle.Italic))
        _doc = GdiFontManager.GetFontX(fontFamilyName, _doc.Size, FontXStyle.Italic);
      else if (GdiFontManager.IsFontFamilyAndStyleAvailable(fontFamilyName, FontXStyle.Bold | FontXStyle.Italic))
        _doc = GdiFontManager.GetFontX(fontFamilyName, _doc.Size, FontXStyle.Bold | FontXStyle.Italic);
    }

    public override bool Apply(bool disposeController)
    {
      _originalDoc = _doc; // this is safe because FontX is an immutable class
      return ApplyEnd(true, disposeController);
    }
  }
}
