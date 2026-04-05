#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
using Altaxo.Drawing.D3D;
using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Common.Drawing.D3D
{
  /// <summary>
  /// Defines the view contract for editing <see cref="FontX3D"/> values.
  /// </summary>
  public interface IFontX3DView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for <see cref="FontX3D"/>.
  /// </summary>
  [ExpectedTypeOfView(typeof(IFontX3DView))]
  public class FontX3DController : MVCANDControllerEditImmutableDocBase<FontX3D, IFontX3DView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FontX3DController"/> class.
    /// </summary>
    public FontX3DController()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FontX3DController"/> class.
    /// </summary>
    /// <param name="doc">The font to edit.</param>
    public FontX3DController(FontX3D doc)
    {
      InitializeDocument(doc);
    }

    #region Bindings

    /// <summary>
    /// Gets or sets the selected font family name.
    /// </summary>
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
          OnPropertyChanged(nameof(SelectedFontDepth));
          OnMadeDirty();
        }
      }
    }

    /// <summary>
    /// Gets or sets the selected font size.
    /// </summary>
    public double SelectedFontSize
    {
      get => _doc.Size;
      set
      {
        if (!(SelectedFontSize == value))
        {
          _doc = _doc.WithSize(value);
          OnPropertyChanged(nameof(SelectedFontSize));
          OnMadeDirty();
        }
      }
    }


    /// <summary>
    /// Gets or sets the selected font depth.
    /// </summary>
    public double SelectedFontDepth
    {
      get => _doc.Depth;
      set
      {
        if (!(SelectedFontDepth == value))
        {
          _doc = _doc.WithDepth(value);
          OnPropertyChanged(nameof(SelectedFontDepth));
          OnMadeDirty();
        }
      }
    }

    /// <summary>
    /// Gets or sets the selected font style.
    /// </summary>
    public FontXStyle SelectedFontStyle
    {
      get => _doc.Style;
      set
      {
        if (!(SelectedFontStyle == value))
        {
          _doc = _doc.WithStyle(value);
          OnPropertyChanged(nameof(SelectedFontStyle));
          OnMadeDirty();
        }
      }
    }

    #endregion

    /// <summary>
    /// Gets the edited font document.
    /// </summary>
    public FontX3D Doc => _doc;

    /// <inheritdoc/>
    protected override void OnMadeDirty()
    {
      base.OnMadeDirty();
      OnPropertyChanged(nameof(Doc));
    }

    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        var ff = GdiFontManager.GetValidFontFamilyName(_doc.Font);
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
        _doc = _doc.WithFamily(fontFamilyName).WithStyle(FontXStyle.Regular);
      else if (GdiFontManager.IsFontFamilyAndStyleAvailable(fontFamilyName, FontXStyle.Bold))
        _doc = _doc.WithFamily(fontFamilyName).WithStyle(FontXStyle.Bold);
      else if (GdiFontManager.IsFontFamilyAndStyleAvailable(fontFamilyName, FontXStyle.Italic))
        _doc = _doc.WithFamily(fontFamilyName).WithStyle(FontXStyle.Italic);
      else if (GdiFontManager.IsFontFamilyAndStyleAvailable(fontFamilyName, FontXStyle.Bold | FontXStyle.Italic))
        _doc = _doc.WithFamily(fontFamilyName).WithStyle(FontXStyle.Bold | FontXStyle.Italic);
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      _originalDoc = _doc; // this is safe because FontX is an immutable class
      return ApplyEnd(true, disposeController);
    }
  }
}
