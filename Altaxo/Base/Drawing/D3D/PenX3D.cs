#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Drawing.D3D.Material;
using Altaxo.Drawing.DashPatternManagement;

namespace Altaxo.Drawing.D3D
{
  /// <summary>
  /// Immutable 3D pen definition containing material, cross section, dash pattern, and caps.
  /// </summary>
  public class PenX3D : Altaxo.Main.IImmutable
  {
    #region Member variables

    private IMaterial _material;

    private ICrossSectionOfLine _crossSection;

    private IDashPattern _dashPattern;

    private ILineCap? _lineStartCap;

    private ILineCap? _lineEndCap;

    private ILineCap? _dashStartCap;

    private bool _dashStartCapSuppressionIfSpaceInsufficient;

    private ILineCap? _dashEndCap;

    private bool _dashEndCapSuppressionIfSpaceInsufficient;

    private PenLineJoin _lineJoin = PenLineJoin.Miter;

    private double _miterLimit = 10;

    #endregion Member variables

    #region Serialization

    /// <summary>
    /// 2015-11-14 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PenX3D), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PenX3D)o;

        info.AddValue("Material", s._material);
        info.AddValue("CrossSection", s._crossSection);

        info.AddEnum("LineJoin", s._lineJoin);
        info.AddValue("MiterLimit", s._miterLimit);

        if (s._lineStartCap is not null)
          info.AddValue("LineStartCap", s._lineStartCap);

        if (s._lineEndCap is not null)
          info.AddValue("LineEndCap", s._lineEndCap);

        // Note: we must even save the solid pattern if it belongs to another list than the BuiltinDefault list,
        // otherwise when deserializing we wouldn't know to which list the solid dash pattern belongs to.
        if (s._dashPattern is not null && (!Drawing.DashPatterns.Solid.Instance.Equals(s._dashPattern) || !object.ReferenceEquals(DashPatternListManager.Instance.BuiltinDefault, DashPatternListManager.Instance.GetParentList(s._dashPattern))))
        {
          info.AddValue("DashPattern", s._dashPattern);

          if (s._dashStartCap is not null)
          {
            info.AddValue("DashStartCap", s._dashStartCap);
            info.AddValue("DashStartCapSuppression", s._dashStartCapSuppressionIfSpaceInsufficient);
          }
          if (s._dashEndCap is not null)
          {
            info.AddValue("DashEndCap", s._dashEndCap);
            info.AddValue("DashEndCapSuppression", s._dashEndCapSuppressionIfSpaceInsufficient);
          }
        }
      }

      protected virtual PenX3D SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var material = (IMaterial)info.GetValue("Material", null);
        var crossSection = (ICrossSectionOfLine)info.GetValue("CrossSection", null);

        var lineJoin = (PenLineJoin)info.GetEnum("LineJoin", typeof(PenLineJoin));
        double miterLimit = info.GetDouble("MiterLimit");

        var lineStartCap = ("LineStartCap" == info.CurrentElementName) ? (ILineCap)info.GetValue("LineStartCap", null) : null;
        var lineEndCap = ("LineEndCap" == info.CurrentElementName) ? (ILineCap)info.GetValue("LineEndCap", null) : null;
        var dashPattern = ("DashPattern" == info.CurrentElementName) ? (IDashPattern)info.GetValue("DashPattern", null) : null;
        dashPattern = dashPattern ?? DashPatterns.Solid.Instance;
        ILineCap? dashStartCap = null, dashEndCap = null;
        bool dashStartCapSuppression = false, dashEndCapSuppression = false;
        if (!DashPatterns.Solid.Instance.Equals(dashPattern))
        {
          if ("DashStartCap" == info.CurrentElementName)
          {
            dashStartCap = (ILineCap)info.GetValue("DashStartCap", null);
            dashStartCapSuppression = info.GetBoolean("DashStartCapSuppression");
          }

          if ("DashEndCap" == info.CurrentElementName)
          {
            dashEndCap = (ILineCap)info.GetValue("DashEndCap", null);
            dashEndCapSuppression = info.GetBoolean("DashEndCapSuppression");
          }
        }

        return new PenX3D(material, crossSection, lineJoin, miterLimit, lineStartCap, lineEndCap, dashPattern, dashStartCap, dashStartCapSuppression, dashEndCap, dashEndCapSuppression);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="PenX3D"/> class using a uniform color and thickness.
    /// </summary>
    /// <param name="color">The pen color.</param>
    /// <param name="thickness">The uniform line thickness.</param>
    public PenX3D(NamedColor color, double thickness)
    {
      _material = Materials.GetSolidMaterial(color);
      _crossSection = new CrossSections.Rectangular(thickness, thickness);
      _dashPattern = DashPatternListManager.Instance.BuiltinDefaultSolid;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PenX3D"/> class using the specified material and cross section.
    /// </summary>
    /// <param name="material">The material used to draw the pen.</param>
    /// <param name="crossSection">The line cross section.</param>
    public PenX3D(IMaterial material, ICrossSectionOfLine crossSection)
    {
      _material = material;
      _crossSection = crossSection;
      _dashPattern = DashPatternListManager.Instance.BuiltinDefaultSolid;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PenX3D"/> class with full line, dash, and cap configuration.
    /// </summary>
    /// <param name="material">The material used to draw the pen.</param>
    /// <param name="crossSection">The line cross section.</param>
    /// <param name="lineJoin">The line join style.</param>
    /// <param name="miterLimit">The miter limit used for miter joins.</param>
    /// <param name="lineStartCap">The cap applied at the start of the full line.</param>
    /// <param name="lineEndCap">The cap applied at the end of the full line.</param>
    /// <param name="dashPattern">The dash pattern.</param>
    /// <param name="dashStartCap">The cap applied at the start of each dash.</param>
    /// <param name="dashStartCapSuppressionIfSpaceInsufficient">If set, suppresses the dash start cap when space is insufficient.</param>
    /// <param name="dashEndCap">The cap applied at the end of each dash.</param>
    /// <param name="dashEndCapSuppressionIfSpaceInsufficient">If set, suppresses the dash end cap when space is insufficient.</param>
    public PenX3D(
        IMaterial material,
        ICrossSectionOfLine crossSection,
        PenLineJoin lineJoin,
        double miterLimit,
        ILineCap? lineStartCap, ILineCap? lineEndCap, IDashPattern dashPattern, ILineCap? dashStartCap, bool dashStartCapSuppressionIfSpaceInsufficient, ILineCap? dashEndCap, bool dashEndCapSuppressionIfSpaceInsufficient)
    {
      if (!(miterLimit >= 1))
        throw new ArgumentOutOfRangeException(nameof(miterLimit), "must be >= 1");
      if (dashPattern is null)
        throw new ArgumentNullException(nameof(dashPattern));

      _material = material;
      _crossSection = crossSection;
      _lineJoin = lineJoin;
      _miterLimit = miterLimit;
      _lineStartCap = lineStartCap;
      _lineEndCap = lineEndCap;
      _dashPattern = dashPattern;
      _dashStartCap = dashStartCap;
      _dashStartCapSuppressionIfSpaceInsufficient = dashStartCapSuppressionIfSpaceInsufficient;
      _dashEndCap = dashEndCap;
      _dashEndCapSuppressionIfSpaceInsufficient = dashEndCapSuppressionIfSpaceInsufficient;
    }

    /// <summary>
    /// Gets the first line thickness component.
    /// </summary>
    public double Thickness1
    {
      get
      {
        return _crossSection.Size1;
      }
    }

    /// <summary>
    /// Returns a copy of this pen with a different first thickness component.
    /// </summary>
    /// <param name="thickness1">The new first thickness component.</param>
    /// <returns>A pen with the updated first thickness component.</returns>
    public PenX3D WithThickness1(double thickness1)
    {
      var newCrossSection = _crossSection.WithSize1(thickness1);
      if (!object.ReferenceEquals(newCrossSection, _crossSection))
      {
        var result = (PenX3D)MemberwiseClone();
        result._crossSection = newCrossSection;
        return result;
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Gets the second line thickness component.
    /// </summary>
    public double Thickness2
    {
      get
      {
        return _crossSection.Size2;
      }
    }

    /// <summary>
    /// Returns a copy of this pen with a different second thickness component.
    /// </summary>
    /// <param name="thickness2">The new second thickness component.</param>
    /// <returns>A pen with the updated second thickness component.</returns>
    public PenX3D WithThickness2(double thickness2)
    {
      var newCrossSection = _crossSection.WithSize2(thickness2);
      if (!object.ReferenceEquals(newCrossSection, _crossSection))
      {
        var result = (PenX3D)MemberwiseClone();
        result._crossSection = newCrossSection;
        return result;
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Returns a copy of this pen with the same thickness in both cross-section dimensions.
    /// </summary>
    /// <param name="thickness">The new uniform thickness.</param>
    /// <returns>A pen with the updated uniform thickness.</returns>
    public PenX3D WithUniformThickness(double thickness)
    {
      var newCrossSection = _crossSection.WithSize(thickness, thickness);
      if (!object.ReferenceEquals(newCrossSection, _crossSection))
      {
        var result = (PenX3D)MemberwiseClone();
        result._crossSection = newCrossSection;
        return result;
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Gets the material used to render the pen.
    /// </summary>
    public IMaterial Material
    {
      get
      {
        return _material;
      }
    }

    /// <summary>
    /// Returns a copy of this pen with a different material.
    /// </summary>
    /// <param name="material">The new material.</param>
    /// <returns>A pen with the updated material.</returns>
    public PenX3D WithMaterial(IMaterial material)
    {
      if (material is null)
        throw new ArgumentNullException(nameof(material));

      var result = (PenX3D)MemberwiseClone();
      result._material = material;
      return result;
    }

    /// <summary>
    /// Gets the color exposed by the current material.
    /// </summary>
    public NamedColor Color
    {
      get
      {
        return _material.Color;
      }
    }

    /// <summary>
    /// Returns a copy of this pen with a different material color.
    /// </summary>
    /// <param name="color">The new color.</param>
    /// <returns>A pen with the updated color.</returns>
    public PenX3D WithColor(NamedColor color)
    {
      var result = (PenX3D)MemberwiseClone();
      result._material = Materials.GetMaterialWithNewColor(result._material, color);
      return result;
    }

    /// <summary>
    /// Gets a value indicating whether this pen uses the invisible material.
    /// </summary>
    public bool IsInvisible
    {
      get
      {
        return MaterialInvisible.Instance.Equals(this.Material);
      }
    }

    /// <summary>
    /// Gets the line cross section.
    /// </summary>
    public ICrossSectionOfLine CrossSection
    {
      get
      {
        return _crossSection;
      }
    }

    /// <summary>
    /// Returns a copy of this pen with a different cross section.
    /// </summary>
    /// <param name="crossSection">The new cross section.</param>
    /// <returns>A pen with the updated cross section.</returns>
    public PenX3D WithCrossSection(ICrossSectionOfLine crossSection)
    {
      if (!object.ReferenceEquals(crossSection, _crossSection))
      {
        var result = (PenX3D)MemberwiseClone();
        result._crossSection = crossSection;
        return result;
      }
      else
      {
        return this;
      }
    }

    #region Line Join

    /// <summary>
    /// Gets the line join style.
    /// </summary>
    public PenLineJoin LineJoin
    {
      get
      {
        return _lineJoin;
      }
    }

    /// <summary>
    /// Returns a copy of this pen with a different line join style.
    /// </summary>
    /// <param name="lineJoin">The new line join style.</param>
    /// <returns>A pen with the updated line join style.</returns>
    public PenX3D WithLineJoin(PenLineJoin lineJoin)
    {
      if (!(lineJoin == _lineJoin))
      {
        var result = (PenX3D)MemberwiseClone();
        result._lineJoin = lineJoin;
        return result;
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Gets the miter limit used when <see cref="LineJoin"/> is set to miter.
    /// </summary>
    public double MiterLimit
    {
      get
      {
        return _miterLimit;
      }
    }

    /// <summary>
    /// Returns a copy of this pen with a different miter limit.
    /// </summary>
    /// <param name="miterLimit">The new miter limit.</param>
    /// <returns>A pen with the updated miter limit.</returns>
    public PenX3D WithMiterLimit(double miterLimit)
    {
      if (!(miterLimit >= 1))
        miterLimit = 1;
      if (!(miterLimit < 1000))
        miterLimit = 1000;

      if (!(miterLimit == _miterLimit))
      {
        var result = (PenX3D)MemberwiseClone();
        result._miterLimit = miterLimit;
        return result;
      }
      else
      {
        return this;
      }
    }

    #endregion Line Join

    #region Dash Pattern

    /// <summary>
    /// Gets the dash pattern (or null if this pen doesn't use a dash pattern).
    /// </summary>
    /// <value>
    /// The dash pattern that this pen used. If the pen represents a solid line, the return value is null.
    /// </value>
    public IDashPattern DashPattern
    {
      get
      {
        if (_dashPattern is null)
          throw new InvalidProgramException("_dashPattern member should always be != null.");

        return _dashPattern;
      }
    }

    /// <summary>
    /// Returns a new instance of this pen, with the dash pattern provided in the argument.
    /// </summary>
    /// <param name="dashPattern">The dash pattern. Can be null to represent a solid line.</param>
    /// <returns>A new instance of this pen, with the dash pattern provided in the argument.</returns>
    public PenX3D WithDashPattern(IDashPattern dashPattern)
    {
      if (dashPattern is null)
        throw new ArgumentNullException(nameof(dashPattern));

      if (object.ReferenceEquals(_dashPattern, dashPattern)) // Reference equality is important, since the parent DashPatternList is determined by reference equality
      {
        return this;
      }
      else
      {
        var result = (PenX3D)MemberwiseClone();
        result._dashPattern = dashPattern;
        return result;
      }
    }

    #endregion Dash Pattern

    #region Line Start cap

    /// <summary>
    /// Gets the cap applied at the start of the whole line.
    /// </summary>
    public ILineCap? LineStartCap
    {
      get
      {
        return _lineStartCap;
      }
    }

    /// <summary>
    /// Returns a copy of this pen with a different line start cap.
    /// </summary>
    /// <param name="cap">The new start cap, or <see langword="null"/> for the default flat cap.</param>
    /// <returns>A pen with the updated line start cap.</returns>
    public PenX3D WithLineStartCap(ILineCap? cap)
    {
      if (cap is LineCaps.Flat)
        cap = null;

      if (object.Equals(_lineStartCap, cap))
        return this;

      var result = (PenX3D)MemberwiseClone();
      result._lineStartCap = cap;
      return result;
    }

    #endregion Line Start cap

    #region Line end cap

    /// <summary>
    /// Gets the cap applied at the end of the whole line.
    /// </summary>
    public ILineCap? LineEndCap
    {
      get
      {
        return _lineEndCap;
      }
    }

    /// <summary>
    /// Returns a copy of this pen with a different line end cap.
    /// </summary>
    /// <param name="cap">The new end cap, or <see langword="null"/> for the default flat cap.</param>
    /// <returns>A pen with the updated line end cap.</returns>
    public PenX3D WithLineEndCap(ILineCap? cap)
    {
      if (cap is LineCaps.Flat)
        cap = null;

      if (object.Equals(_lineEndCap, cap))
        return this;

      var result = (PenX3D)MemberwiseClone();
      result._lineEndCap = cap;
      return result;
    }

    #endregion Line end cap

    #region Dash start cap

    /// <summary>
    /// Gets the cap applied at the start of each dash.
    /// </summary>
    public ILineCap? DashStartCap
    {
      get
      {
        return _dashStartCap;
      }
    }

    /// <summary>
    /// Returns a copy of this pen with a different dash start cap.
    /// </summary>
    /// <param name="cap">The new dash start cap, or <see langword="null"/> for the default flat cap.</param>
    /// <returns>A pen with the updated dash start cap.</returns>
    public PenX3D WithDashStartCap(ILineCap? cap)
    {
      if (cap is LineCaps.Flat)
        cap = null;

      if (object.Equals(_dashStartCap, cap))
        return this;

      var result = (PenX3D)MemberwiseClone();
      result._dashStartCap = cap;
      return result;
    }

    #endregion Dash start cap

    #region Dash end cap

    /// <summary>
    /// Gets the cap applied at the end of each dash.
    /// </summary>
    public ILineCap? DashEndCap
    {
      get
      {
        return _dashEndCap;
      }
    }

    /// <summary>
    /// Returns a copy of this pen with a different dash end cap.
    /// </summary>
    /// <param name="cap">The new dash end cap, or <see langword="null"/> for the default flat cap.</param>
    /// <returns>A pen with the updated dash end cap.</returns>
    public PenX3D WithDashEndCap(ILineCap? cap)
    {
      if (cap is LineCaps.Flat)
        cap = null;

      if (object.Equals(_dashEndCap, cap))
        return this;

      var result = (PenX3D)MemberwiseClone();
      result._dashEndCap = cap;
      return result;
    }

    #endregion Dash end cap

    /// <summary>
    /// Determines whether two pens are equal except for their thickness values.
    /// </summary>
    /// <param name="pen1">The first pen.</param>
    /// <param name="pen2">The second pen.</param>
    /// <returns><see langword="true"/> if the pens are equal except for thickness; otherwise, <see langword="false"/>.</returns>
    public static bool AreEqualUnlessThickness(PenX3D pen1, PenX3D pen2)
    {
      bool isEqual =

          pen1.Material == pen2.Material &&
          pen1.CrossSection.GetType() == pen2.CrossSection.GetType() &&
          object.ReferenceEquals(pen1._dashPattern, pen2._dashPattern) && // Reference equality because DashPatterns parent list is determined by reference equality.
          object.Equals(pen1._lineStartCap, pen2._lineStartCap) &&
          object.Equals(pen1._lineEndCap, pen2._lineEndCap) &&
          object.Equals(pen1._dashStartCap, pen2._dashStartCap) &&
          object.Equals(pen1._dashEndCap, pen2._dashEndCap);

      return isEqual;
    }
  }
}
