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

#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Altaxo.Drawing;
using Altaxo.Geometry;

namespace Altaxo.Graph.Gdi.HatchBrushes
{
  /// <summary>
  /// Provides the base implementation for repeatable hatch brush textures.
  /// </summary>
  public abstract class HatchBrushBase : ImageProxy, IHatchBrushTexture
  {
    /// <summary>
    /// The default effective resolution in dpi.
    /// </summary>
    protected const double DefaultEffectiveResolution = 300;
    /// <summary>
    /// The default background color for generated hatch textures.
    /// </summary>
    protected static readonly NamedColor _defaultBackColor = NamedColors.Transparent;
    /// <summary>
    /// The default foreground color for generated hatch textures.
    /// </summary>
    protected static readonly NamedColor _defaultForeColor = NamedColors.Black;

    /// <summary>
    /// Repeat length in points (1/72 inch) in x-direction.
    /// </summary>
    protected double _repeatLengthPt = 10;

    /// <summary>
    /// Structure factor. For hatch brushes based on lines, the line width is this value times the repeat length. For shape based brushes, the size
    /// of the shape is this value times the repeat length.
    /// </summary>
    protected double _structureFactor = 0.2;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(HatchBrushBase), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (HatchBrushBase)o;
        info.AddValue("RepeatLength", s._repeatLengthPt);
        info.AddValue("StructureFactor", s._structureFactor);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (HatchBrushBase)(o ?? throw new ArgumentNullException(nameof(o)));

        s._repeatLengthPt = info.GetDouble("RepeatLength");
        s._structureFactor = info.GetDouble("StructureFactor");

        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Creates the hatch image for the specified resolution and colors.
    /// </summary>
    /// <param name="maxEffectiveResolutionDpi">The maximum effective resolution in DPI.</param>
    /// <param name="foreColor">The foreground color.</param>
    /// <param name="backColor">The background color.</param>
    /// <returns>The generated image.</returns>
    protected abstract Image GetImage(double maxEffectiveResolutionDpi, NamedColor foreColor, NamedColor backColor);

    /// <inheritdoc />
    public virtual System.IO.Stream GetContentStream(double maxEffectiveResolutionDpi, NamedColor foreColor, NamedColor backColor)
    {
      var str = new System.IO.MemoryStream();
      using (var img = GetImage(maxEffectiveResolutionDpi, foreColor, backColor))
      {
        img.Save(str, System.Drawing.Imaging.ImageFormat.Png);
        str.Flush();
        str.Seek(0, System.IO.SeekOrigin.Begin);
      }
      return str;
    }

    /// <inheritdoc />
    public override VectorD2D Size
    {
      get { return new VectorD2D(_repeatLengthPt, _repeatLengthPt); }
    }

    /// <summary>
    /// Gets the repeat length in points.
    /// </summary>
    [System.ComponentModel.Editor(typeof(Altaxo.Gui.Common.LengthValueInPointController), typeof(Altaxo.Gui.IMVCANController))]
    [Altaxo.Main.Services.PropertyReflection.DisplayOrder(1)]
    public double RepeatLength
    {
      get { return _repeatLengthPt; }
    }

    /// <summary>
    /// Creates a copy with a different repeat length.
    /// </summary>
    /// <param name="repeatLength">The repeat length in points.</param>
    /// <returns>The updated instance or this instance if unchanged.</returns>
    public HatchBrushBase WithRepeatLength(double repeatLength)
    {
      if (!(_repeatLengthPt == repeatLength))
      {
        var result = (HatchBrushBase)MemberwiseClone();
        result._repeatLengthPt = repeatLength;
        return result;
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Gets the structure factor.
    /// </summary>
    [System.ComponentModel.Editor(typeof(Altaxo.Gui.Common.RelationValueInUnityController), typeof(Altaxo.Gui.IMVCANController))]
    [Altaxo.Main.Services.PropertyReflection.DisplayOrder(2)]
    public double StructureFactor
    {
      get { return _structureFactor; }
    }

    /// <summary>
    /// Creates a copy with a different structure factor.
    /// </summary>
    /// <param name="structureFactor">The structure factor.</param>
    /// <returns>The updated instance or this instance if unchanged.</returns>
    public HatchBrushBase WithStructureFactor(double structureFactor)
    {
      if (!(_structureFactor == structureFactor))
      {
        var result = (HatchBrushBase)MemberwiseClone();
        result._structureFactor = structureFactor;
        return result;
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Calculates the bitmap dimension used to generate the hatch texture.
    /// </summary>
    /// <param name="maxEffectiveResolutionDpi">The maximum effective resolution in dpi.</param>
    /// <returns>The bitmap dimension in pixels.</returns>
    protected int GetPixelDimensions(double maxEffectiveResolutionDpi)
    {
      // use a factor of 2 for safety, thus we have at least two pixels of the bitmap mapping to 1 pixel of the drawing
      int pixels = (int)((2 * InchesPerPoint) * maxEffectiveResolutionDpi * _repeatLengthPt);
      pixels = Math.Max(pixels, 16);
      pixels = Math.Min(pixels, 8192);
      pixels = (pixels + 15) & 0xFFFFFF0; // round to multiples of 16
      return pixels;
    }

    /// <inheritdoc />
    public override string ContentHash
    {
      get
      {
        return GetType().ToString();
      }
    }

    /// <inheritdoc />
    public override string Name
    {
      get { return GetType().ToString(); }
    }



    /// <inheritdoc />
    public virtual System.IO.Stream GetContentStream(double maxEffectiveResolutionDpi)
    {
      return GetContentStream(maxEffectiveResolutionDpi, _defaultForeColor, _defaultBackColor);
    }

    /// <inheritdoc />
    public override System.IO.Stream GetContentStream()
    {
      return GetContentStream(300);
    }

    /// <inheritdoc />
    public override bool IsValid
    {
      get { return true; }
    }
  }
}
