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

using System;
using System.Collections.Generic;

namespace Altaxo.Graph.Graph3D.Plot.Styles
{
  using Altaxo.Data;
  using Altaxo.Main;
  using Background;
  using Drawing;
  using Drawing.D3D;
  using Drawing.D3D.Material;
  using Geometry;
  using Graph.Plot.Data;
  using Graph.Plot.Groups;
  using GraphicsContext;
  using Plot.Data;
  using Plot.Groups;

  public class LabelPlotStyle
    :
    Main.SuspendableDocumentNodeWithEventArgs,
    IG3DPlotStyle
  {
    protected Altaxo.Data.IReadableColumnProxy _labelColumnProxy;

    /// <summary>The axis where the label is attached to (if it is attached).</summary>
    protected CSPlaneID _attachedPlane;

    protected bool _independentSkipFrequency;

    /// <summary>
    /// Skip frequency.
    /// </summary>
    protected int _skipFrequency;

    /// <summary>If true, group styles that shift the logical position of the items (for instance <see cref="BarSizePosition3DGroupStyle"/>) are not applied. I.e. when true, the position of the item remains unperturbed.</summary>
    protected bool _independentOnShiftingGroupStyles;

    /// <summary>
    /// The label format string (C# format).
    /// </summary>
    protected string _labelFormatString;

    /// <summary>The font of the label.</summary>
    protected FontX3D _font;

    /// <summary>
    /// Offset used to calculate the font size in dependence on the symbol size., according to the formula:
    /// fontSize = <see cref="_fontSizeOffset"/> + <see cref="_fontSizeFactor"/> * <see cref="_symbolSize"/>;
    /// </summary>
    protected double _fontSizeOffset;

    /// <summary>
    /// Factor used to calculate the font size in dependence on the symbol size, according to the formula:
    /// fontSize = <see cref="_fontSizeOffset"/> + <see cref="_fontSizeFactor"/> * <see cref="_symbolSize"/>;
    /// </summary>
    protected double _fontSizeFactor;

    protected bool _independentSymbolSize;

    protected double _symbolSize;

    /// <summary>The brush for the label.</summary>
    protected IMaterial _material;

    /// <summary>
    /// True if the color of the label is not dependent on the color of the parent plot style.
    /// </summary>
    protected bool _independentColor;

    protected Alignment _alignmentX;
    protected Alignment _alignmentY;
    protected Alignment _alignmentZ;

    /// <summary>The rotation around x-axis of the label.</summary>
    protected double _rotationX;

    /// <summary>The rotation around y-axis of the label.</summary>
    protected double _rotationY;

    /// <summary>The rotation around z-axis of the label.</summary>
    protected double _rotationZ;

    /// <summary>The x offset in EM units.
    /// Total offset is calculated according to:
    /// totalOffset = _offset_Points + _offset_EmUnits * emSize + _offset_SymbolSizeUnits * symbolSize;</summary>
    protected double _offsetX_EmUnits;

    /// <summary>The x offset int points.
    /// Total offset is calculated according to:
    /// totalOffset = _offset_Points + _offset_EmUnits * emSize + _offset_SymbolSizeUnits * symbolSize;</summary>
    protected double _offsetX_Points;

    /// <summary>The x offset factor to be multiplied with the symbol size.
    /// Total offset is calculated according to:
    /// totalOffset = _offset_Points + _offset_EmUnits * emSize + _offset_SymbolSizeUnits * symbolSize;</summary>
    protected double _offsetX_SymbolSizeUnits;

    /// <summary>The y offset in EM units.</summary>
    protected double _offsetY_EmUnits;

    /// <summary>The y offset int points.
    /// Total offset is calculated according to:
    /// totalOffset = _offset_Points + _offset_EmUnits * emSize + _offset_SymbolSizeUnits * symbolSize;</summary>
    protected double _offsetY_Points;

    /// <summary>The y offset factor to be multiplied with the symbol size.
    /// Total offset is calculated according to:
    /// totalOffset = _offset_Points + _offset_EmUnits * emSize + _offset_SymbolSizeUnits * symbolSize;</summary>
    protected double _offsetY_SymbolSizeUnits;

    /// <summary>The z offset in EM units.</summary>
    protected double _offsetZ_EmUnits;

    /// <summary>The z offset int points.
    /// Total offset is calculated according to:
    /// totalOffset = _offset_Points + _offset_EmUnits * emSize + _offset_SymbolSizeUnits * symbolSize;</summary>
    protected double _offsetZ_Points;

    /// <summary>The z offset factor to be multiplied with the symbol size.
    /// Total offset is calculated according to:
    /// totalOffset = _offset_Points + _offset_EmUnits * emSize + _offset_SymbolSizeUnits * symbolSize;</summary>
    protected double _offsetZ_SymbolSizeUnits;

    protected ColorLinkage _backgroundColorLinkage;

    /// <summary>The style for the background.</summary>
    protected IBackgroundStyle _backgroundStyle;

    // cached values:
    /// <summary>If this function is set, then _symbolSize is ignored and the symbol size is evaluated by this function.</summary>
    [field: NonSerialized]
    protected Func<int, double> _cachedSymbolSizeForIndexFunction;

    /// <summary>If this function is set, the label color is determined by calling this function on the index into the data.</summary>
    [field: NonSerialized]
    protected Func<int, System.Drawing.Color> _cachedColorForIndexFunction;

    /// <summary>Logical x shift between the location of the real data point and the point where the item is finally drawn.</summary>
    private double _cachedLogicalShiftX;

    /// <summary>Logical y shift between the location of the real data point and the point where the item is finally drawn.</summary>
    private double _cachedLogicalShiftY;

    /// <summary>Logical z shift between the location of the real data point and the point where the item is finally drawn.</summary>
    private double _cachedLogicalShiftZ;

    #region Serialization

    /// <summary>
    /// <para>Date: 2016-06-22</para>
    /// <para>Added: BackgroundColorLinkage</para>
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LabelPlotStyle), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        SSerialize(obj, info);
      }

      public static void SSerialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (LabelPlotStyle)obj;

        info.AddValue("LabelColumn", s._labelColumnProxy);
        info.AddValue("AttachedAxis", s._attachedPlane);
        info.AddValue("IndependentSkipFreq", s._independentSkipFrequency);
        info.AddValue("SkipFreq", s._skipFrequency);
        info.AddValue("IndependentOnShiftingGroupStyles", s._independentOnShiftingGroupStyles);
        info.AddValue("LabelFormat", s._labelFormatString);

        info.AddValue("IndependentSymbolSize", s._independentSymbolSize);
        info.AddValue("SymbolSize", s._symbolSize);

        info.AddValue("FontSizeOffset", s._fontSizeOffset);
        info.AddValue("FontSizeFactor", s._fontSizeFactor);
        info.AddValue("Font", s._font);
        info.AddValue("Material", s._material);
        info.AddValue("IndependentColor", s._independentColor);

        info.AddEnum("AlignmentX", s._alignmentX);
        info.AddEnum("AlignmentY", s._alignmentY);
        info.AddEnum("AlignmentZ", s._alignmentZ);

        info.AddValue("RotationX", s._rotationX);
        info.AddValue("RotationY", s._rotationY);
        info.AddValue("RotationZ", s._rotationZ);

        info.AddValue("OffsetXPoints", s._offsetX_Points);
        info.AddValue("OffsetXEm", s._offsetX_EmUnits);
        info.AddValue("OffsetXSymbolSize", s._offsetX_SymbolSizeUnits);
        info.AddValue("OffsetYPoints", s._offsetY_Points);
        info.AddValue("OffsetYEm", s._offsetY_EmUnits);
        info.AddValue("OffsetYSymbolSize", s._offsetY_SymbolSizeUnits);
        info.AddValue("OffsetZPoints", s._offsetZ_Points);
        info.AddValue("OffsetZEm", s._offsetZ_EmUnits);
        info.AddValue("OffsetZSymbolSize", s._offsetZ_SymbolSizeUnits);

        info.AddEnum("BackgroundColorLinkage", s._backgroundColorLinkage);
        info.AddValue("Background", s._backgroundStyle);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        return SDeserialize(o, info, parent, true);
      }

      public static object SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent, bool nativeCall)
      {
        var s = (LabelPlotStyle)o ?? new LabelPlotStyle(info);

        s.LabelColumnProxy = (Altaxo.Data.IReadableColumnProxy)info.GetValue("LabelColumn", s);
        s._attachedPlane = (CSPlaneID)info.GetValue("AttachedPlane", s);
        s._independentSkipFrequency = info.GetBoolean("IndependentSkipFreq");
        s._skipFrequency = info.GetInt32("SkipFreq");
        s._independentOnShiftingGroupStyles = info.GetBoolean("IndependentOnShiftingGroupStyles");
        s._labelFormatString = info.GetString("LabelFormat");

        s._independentSymbolSize = info.GetBoolean("IndependentSymbolSize");
        s._symbolSize = info.GetDouble("SymbolSize");

        s._fontSizeOffset = info.GetDouble("FontSizeOffset");
        s._fontSizeFactor = info.GetDouble("FontSizeFactor");

        s._font = (FontX3D)info.GetValue("Font", s);
        s._material = (IMaterial)info.GetValue("Material", s);
        s._independentColor = info.GetBoolean("IndependentColor");

        s._alignmentX = (Alignment)info.GetEnum("AlignmentX", typeof(Alignment));
        s._alignmentY = (Alignment)info.GetEnum("AlignmentY", typeof(Alignment));
        s._alignmentZ = (Alignment)info.GetEnum("AlignmentZ", typeof(Alignment));

        s._rotationX = info.GetDouble("RotationX");
        s._rotationY = info.GetDouble("RotationY");
        s._rotationZ = info.GetDouble("RotationZ");

        s._offsetX_Points = info.GetDouble("OffsetXPoints");
        s._offsetX_EmUnits = info.GetDouble("OffsetXEm");
        s._offsetX_SymbolSizeUnits = info.GetDouble("OffsetXSymbolSize");

        s._offsetY_Points = info.GetDouble("OffsetYPoints");
        s._offsetY_EmUnits = info.GetDouble("OffsetYEm");
        s._offsetY_SymbolSizeUnits = info.GetDouble("OffsetYSymbolSize");

        s._offsetZ_Points = info.GetDouble("OffsetZPoints");
        s._offsetZ_EmUnits = info.GetDouble("OffsetZEm");
        s._offsetZ_SymbolSizeUnits = info.GetDouble("OffsetZSymbolSize");

        s._backgroundColorLinkage = (ColorLinkage)info.GetEnum("BackgroundColorLinkage", typeof(ColorLinkage));

        s._backgroundStyle = (IBackgroundStyle)info.GetValue("Background", s);
        if (null != s._backgroundStyle)
          s._backgroundStyle.ParentObject = s;

        if (nativeCall)
        {
          // restore the cached values
          s.SetCachedValues();
        }

        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Deserialization constructor.
    /// </summary>
    /// <param name="info">The deserialization information.</param>
    protected LabelPlotStyle(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      _backgroundColorLinkage = ColorLinkage.Independent;
    }

    public bool CopyFrom(object obj, bool copyWithDataReferences)
    {
      if (object.ReferenceEquals(this, obj))
        return true;
      var from = obj as LabelPlotStyle;
      if (null == from)
        return false;

      using (var suspendToken = SuspendGetToken())
      {
        _attachedPlane = from._attachedPlane;
        _independentSkipFrequency = from._independentSkipFrequency;
        _skipFrequency = from._skipFrequency;
        _independentOnShiftingGroupStyles = from._independentOnShiftingGroupStyles;
        _labelFormatString = from._labelFormatString;

        _independentSymbolSize = from._independentSymbolSize;
        _symbolSize = from._symbolSize;

        _fontSizeOffset = from._fontSizeOffset;
        _fontSizeFactor = from._fontSizeFactor;

        _font = from._font;
        _material = from._material;
        _independentColor = from._independentColor;

        _alignmentX = from._alignmentX;
        _alignmentY = from._alignmentY;
        _alignmentZ = from._alignmentZ;

        _rotationX = from._rotationX;
        _rotationY = from._rotationY;
        _rotationZ = from._rotationZ;

        _offsetX_Points = from._offsetX_Points;
        _offsetX_EmUnits = from._offsetX_EmUnits;
        _offsetX_SymbolSizeUnits = from._offsetX_SymbolSizeUnits;

        _offsetY_Points = from._offsetY_Points;
        _offsetY_EmUnits = from._offsetY_EmUnits;
        _offsetY_SymbolSizeUnits = from._offsetY_SymbolSizeUnits;

        _offsetZ_Points = from._offsetZ_Points;
        _offsetZ_EmUnits = from._offsetZ_EmUnits;
        _offsetZ_SymbolSizeUnits = from._offsetZ_SymbolSizeUnits;

        _backgroundColorLinkage = from._backgroundColorLinkage;
        ChildCopyToMember(ref _backgroundStyle, from._backgroundStyle);

        _cachedLogicalShiftX = from._cachedLogicalShiftX;
        _cachedLogicalShiftY = from._cachedLogicalShiftY;
        _cachedLogicalShiftZ = from._cachedLogicalShiftZ;

        if (copyWithDataReferences)
          LabelColumnProxy = (Altaxo.Data.IReadableColumnProxy)from._labelColumnProxy.Clone();

        EhSelfChanged(EventArgs.Empty);
        suspendToken.Resume();
      }
      return true;
    }

    /// <summary>
    /// Copies the member variables from another instance.
    /// </summary>
    /// <param name="obj">Another instance to copy the data from.</param>
    /// <returns>True if data was copied, otherwise false.</returns>
    public bool CopyFrom(object obj)
    {
      return CopyFrom(obj, true);
    }

    /// <inheritdoc/>
    public object Clone(bool copyWithDataReferences)
    {
      return new LabelPlotStyle(this, copyWithDataReferences);
    }

    /// <inheritdoc/>
    public object Clone()
    {
      return new LabelPlotStyle(this, true);
    }

    public LabelPlotStyle(LabelPlotStyle from, bool copyWithDataReferences)
    {
      CopyFrom(from, copyWithDataReferences);
    }

    public LabelPlotStyle(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
      : this(null, context)
    {
    }

    public LabelPlotStyle(Altaxo.Data.IReadableColumn labelColumn, Altaxo.Main.Properties.IReadOnlyPropertyBag context)
    {
      _font = GraphDocument.GetDefaultFont(context);
      _fontSizeOffset = _font.Size;

      var color = GraphDocument.GetDefaultPlotColor(context);
      _independentColor = false;
      _material = new MaterialWithUniformColor(color);
      _backgroundColorLinkage = ColorLinkage.Independent;
      LabelColumnProxy = Altaxo.Data.ReadableColumnProxyBase.FromColumn(labelColumn);
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (null != _backgroundStyle)
        yield return new Main.DocumentNodeAndName(_backgroundStyle, "Background");

      if (null != _labelColumnProxy)
        yield return new Main.DocumentNodeAndName(_labelColumnProxy, "LabelColumn");
    }

    private void EhLabelColumnProxyChanged(object sender, EventArgs e)
    {
      EhSelfChanged(EventArgs.Empty);
    }

    protected Altaxo.Data.IReadableColumnProxy LabelColumnProxy
    {
      set
      {
        if (ChildSetMember(ref _labelColumnProxy, value))
        {
          if (null != _labelColumnProxy)
            EhChildChanged(_labelColumnProxy, EventArgs.Empty);
          else
            EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public Altaxo.Data.IReadableColumn LabelColumn
    {
      get
      {
        return _labelColumnProxy == null ? null : _labelColumnProxy.Document;
      }
      set
      {
        if (object.ReferenceEquals(LabelColumn, value))
          return;

        LabelColumnProxy = Altaxo.Data.ReadableColumnProxyBase.FromColumn(value);
      }
    }

    /// <summary>
    /// Gets the name of the label column, if it is a data column. Otherwise, null is returned.
    /// </summary>
    /// <value>
    /// The name of the label column if it is a data column. Otherwise, null.
    /// </value>
    public string LabelColumnDataColumnName
    {
      get
      {
        return _labelColumnProxy.DocumentPath.LastPartOrDefault;
      }
    }

    public IEnumerable<(
      string ColumnLabel, // Column label
      IReadableColumn Column, // the column as it was at the time of this call
      string ColumnName, // the name of the column (last part of the column proxies document path)
      Action<IReadableColumn> ColumnSetAction // action to set the column during Apply of the controller
      )> GetAdditionallyUsedColumns()
    {
      yield return ("Label", LabelColumn, LabelColumnDataColumnName, (col) => LabelColumn = col);
    }

    public string LabelFormatString
    {
      get
      {
        return _labelFormatString;
      }
      set
      {
        if (!(_labelFormatString == value))
        {
          _labelFormatString = value;
          EhSelfChanged();
        }
      }
    }

    /// <summary>
    /// true if the symbol size is independent, i.e. is not published nor updated by a group style.
    /// </summary>
    public bool IndependentSymbolSize
    {
      get { return _independentSymbolSize; }
      set
      {
        if (!(_independentSymbolSize == value))
        {
          _independentSymbolSize = value;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>Controls the length of the end bar.</summary>
    public double SymbolSize
    {
      get { return _symbolSize; }
      set
      {
        if (!Calc.RMath.IsFinite(value))
          throw new ArgumentException(nameof(value), "Value must be a finite number");

        if (!(_symbolSize == value))
        {
          _symbolSize = value;
          EhSelfChanged();
        }
      }
    }

    /// <summary>The font of the label.</summary>
    public FontX3D Font
    {
      get { return _font; }
      set
      {
        if (null == value)
          throw new ArgumentNullException();
        var oldValue = _font;
        _font = value;
        if (!value.Equals(oldValue))
          EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>
    /// Offset used to calculate the font size in dependence on the symbol size., according to the formula:
    /// fontSize = <see cref="FontSizeOffset"/> + <see cref="FontSizeFactor"/> * <see cref="_symbolSize"/>;
    /// </summary>
    public double FontSizeOffset
    {
      get
      {
        return _fontSizeOffset;
      }
      set
      {
        if (!(_fontSizeOffset == value))
        {
          _fontSizeOffset = value;
          EhSelfChanged();
        }
      }
    }

    /// <summary>
    /// Offset used to calculate the font size in dependence on the symbol size., according to the formula:
    /// fontSize = <see cref="FontSizeOffset"/> + <see cref="FontSizeFactor"/> * <see cref="_symbolSize"/>;
    /// </summary>
    public double FontSizeFactor
    {
      get
      {
        return _fontSizeFactor;
      }
      set
      {
        if (!(_fontSizeFactor == value))
        {
          _fontSizeFactor = value;
          EhSelfChanged();
        }
      }
    }

    /// <summary>
    /// Determines whether or not the color of the label is independent of the color of the parent plot style.
    /// </summary>
    public bool IndependentColor
    {
      get { return _independentColor; }
      set
      {
        if (!(_independentColor == value))
        {
          _independentColor = value;

          if (true == _independentColor)
            _cachedColorForIndexFunction = null;

          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public IMaterial Material
    {
      get
      {
        return _material;
      }
      set
      {
        if (null == value)
          throw new ArgumentNullException(nameof(value));

        if (!object.ReferenceEquals(value, _material))
        {
          _material = value;

          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>The background style.</summary>
    public IBackgroundStyle BackgroundStyle
    {
      get
      {
        return _backgroundStyle;
      }
      set
      {
        IBackgroundStyle oldValue = _backgroundStyle;
        if (!object.ReferenceEquals(value, oldValue))
        {
          _backgroundStyle = value;
          EhSelfChanged(EventArgs.Empty); // Fire Changed event
        }
      }
    }

    public ColorLinkage BackgroundColorLinkage
    {
      get
      {
        return _backgroundColorLinkage;
      }
      set
      {
        var oldValue = _backgroundColorLinkage;
        _backgroundColorLinkage = value;
        if (oldValue != value)
          EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>The x offset in points.
    /// Total offset is calculated according to:
    /// totalOffset = <see cref="OffsetXPoints"/> +  <see cref="OffsetXEmUnits"/> * emSize + <see cref="OffsetXSymbolSizeUnits"/> * symbolSize</summary>
    public double OffsetXPoints
    {
      get
      {
        return _offsetX_Points;
      }
      set
      {
        if (!(_offsetX_Points == value))
        {
          _offsetX_Points = value;
          EhSelfChanged();
        }
      }
    }

    /// <summary>The x offset relative to font size, i.e. a value of 1 is 1*FontSize.
    /// Total offset is calculated according to:
    /// totalOffset = <see cref="OffsetXPoints"/> +  <see cref="OffsetXEmUnits"/> * emSize + <see cref="OffsetXSymbolSizeUnits"/> * symbolSize</summary>
    public double OffsetXEmUnits
    {
      get { return _offsetX_EmUnits; }
      set
      {
        double oldValue = _offsetX_EmUnits;
        _offsetX_EmUnits = value;
        if (value != oldValue)
        {
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>The x offset in symbol size units.
    /// Total offset is calculated according to:
    /// totalOffset = <see cref="OffsetXPoints"/> +  <see cref="OffsetXEmUnits"/> * emSize + <see cref="OffsetXSymbolSizeUnits"/> * symbolSize</summary>
    public double OffsetXSymbolSizeUnits
    {
      get
      {
        return _offsetX_SymbolSizeUnits;
      }
      set
      {
        if (!(_offsetX_SymbolSizeUnits == value))
        {
          _offsetX_SymbolSizeUnits = value;
          EhSelfChanged();
        }
      }
    }

    /// <summary>The y offset in points.
    /// Total offset is calculated according to:
    /// totalOffset = <see cref="OffsetYPoints"/> +  <see cref="OffsetYEmUnits"/> * emSize + <see cref="OffsetYSymbolSizeUnits"/> * symbolSize</summary>
    public double OffsetYPoints
    {
      get
      {
        return _offsetY_Points;
      }
      set
      {
        if (!(_offsetY_Points == value))
        {
          _offsetY_Points = value;
          EhSelfChanged();
        }
      }
    }

    /// <summary>The y offset relative to font size, i.e. a value of 1 is 1*FontSize.</summary>
    public double OffsetYEmUnits
    {
      get { return _offsetY_EmUnits; }
      set
      {
        double oldValue = _offsetY_EmUnits;
        _offsetY_EmUnits = value;
        if (value != oldValue)
        {
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>The y offset in symbol size units.
    /// Total offset is calculated according to:
    /// totalOffset = <see cref="OffsetYPoints"/> +  <see cref="OffsetYEmUnits"/> * emSize + <see cref="OffsetYSymbolSizeUnits"/> * symbolSize</summary>
    public double OffsetYSymbolSizeUnits
    {
      get
      {
        return _offsetY_SymbolSizeUnits;
      }
      set
      {
        if (!(_offsetY_SymbolSizeUnits == value))
        {
          _offsetY_SymbolSizeUnits = value;
          EhSelfChanged();
        }
      }
    }

    /// <summary>The z offset in points.
    /// Total offset is calculated according to:
    /// totalOffset = <see cref="OffsetZPoints"/> +  <see cref="OffsetZEmUnits"/> * emSize + <see cref="OffsetZSymbolSizeUnits"/> * symbolSize</summary>
    public double OffsetZPoints
    {
      get
      {
        return _offsetZ_Points;
      }
      set
      {
        if (!(_offsetZ_Points == value))
        {
          _offsetZ_Points = value;
          EhSelfChanged();
        }
      }
    }

    /// <summary>The y offset relative to font size, i.e. a value of 1 is 1*FontSize.</summary>
    public double OffsetZEmUnits
    {
      get { return _offsetZ_EmUnits; }
      set
      {
        double oldValue = _offsetZ_EmUnits;
        _offsetZ_EmUnits = value;
        if (value != oldValue)
        {
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>The z offset in symbol size units.
    /// Total offset is calculated according to:
    /// totalOffset = <see cref="OffsetZPoints"/> +  <see cref="OffsetZEmUnits"/> * emSize + <see cref="OffsetZSymbolSizeUnits"/> * symbolSize</summary>
    public double OffsetZSymbolSizeUnits
    {
      get
      {
        return _offsetZ_SymbolSizeUnits;
      }
      set
      {
        if (!(_offsetZ_SymbolSizeUnits == value))
        {
          _offsetZ_SymbolSizeUnits = value;
          EhSelfChanged();
        }
      }
    }

    /// <summary>The angle of the label around x-axis.</summary>
    public double RotationX
    {
      get { return _rotationX; }
      set
      {
        double oldValue = _rotationX;
        _rotationX = value;
        if (value != oldValue)
        {
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>The angle of the label around y-axis.</summary>
    public double RotationY
    {
      get { return _rotationY; }
      set
      {
        double oldValue = _rotationY;
        _rotationY = value;
        if (value != oldValue)
        {
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>The angle of the label around z-axis.</summary>
    public double RotationZ
    {
      get { return _rotationZ; }
      set
      {
        double oldValue = _rotationZ;
        _rotationZ = value;
        if (value != oldValue)
        {
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>Horizontal alignment of the label.</summary>
    public Alignment AlignmentX
    {
      get
      {
        return _alignmentX;
      }
      set
      {
        if (!(_alignmentX == value))
        {
          _alignmentX = value;
          EhSelfChanged();
        }
      }
    }

    /// <summary>Vertical aligment of the label.</summary>
    public Alignment AlignmentY
    {
      get
      {
        return _alignmentY;
      }
      set
      {
        if (!(_alignmentY == value))
        {
          _alignmentY = value;
          EhSelfChanged();
        }
      }
    }

    /// <summary>Vertical aligment of the label.</summary>
    public Alignment AlignmentZ
    {
      get
      {
        return _alignmentZ;
      }
      set
      {
        if (!(_alignmentZ == value))
        {
          _alignmentZ = value;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>Gets/sets the axis this label is attached to. If set to null, the label is positioned normally.</summary>
    public CSPlaneID AttachedPlane
    {
      get { return _attachedPlane; }
      set
      {
        CSPlaneID oldValue = _attachedPlane;
        _attachedPlane = value;
        if (value != oldValue)
        {
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>Controls how many items are plotted. A value of 1 means every item, a value of 2 every other item, and so on.</summary>
    public int SkipFrequency
    {
      get { return _skipFrequency; }
      set
      {
        if (!(_skipFrequency == value))
        {
          _skipFrequency = Math.Max(1, value);
          EhSelfChanged();
        }
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the skip frequency is independent on other sub group styles using <see cref="SkipFrequency"/>.
    /// </summary>
    /// <value>
    /// <c>true</c> if the skip frequency is independent on other sub group styles using <see cref="SkipFrequency"/>; otherwise, <c>false</c>.
    /// </value>
    public bool IndependentSkipFrequency
    {
      get { return _independentSkipFrequency; }
      set
      {
        if (!(_independentSkipFrequency == value))
        {
          _independentSkipFrequency = value;
          EhSelfChanged();
        }
      }
    }

    /// <summary>
    /// True when we don't want to shift the position of the items, for instance due to the bar graph plot group.
    /// </summary>
    public bool IndependentOnShiftingGroupStyles
    {
      get
      {
        return _independentOnShiftingGroupStyles;
      }
      set
      {
        if (!(_independentOnShiftingGroupStyles == value))
        {
          _independentOnShiftingGroupStyles = value;
          EhSelfChanged();
        }
      }
    }

    protected void SetCachedValues()
    {
    }

    /// <summary>
    /// Paints one label.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="label"></param>
    /// <param name="symbolSize">The symbol size used to calculate the offset.</param>
    /// <param name="variableTextBrush">If not null, this argument provides the text brush that should be used now. If null, then the <see cref="_material"/> is used instead.</param>
    /// <param name="variableBackBrush"></param>
    public void Paint(IGraphicsContext3D g, string label, double symbolSize, IMaterial variableTextBrush, IMaterial variableBackBrush)
    {
      var fontSize = _font.Size;

      var xpos = _offsetX_Points + (_offsetX_EmUnits * fontSize) + (_offsetX_SymbolSizeUnits * symbolSize / 2);
      var ypos = _offsetY_Points + (_offsetY_EmUnits * fontSize) + (_offsetY_SymbolSizeUnits * symbolSize / 2);
      var zpos = _offsetZ_Points + (_offsetZ_EmUnits * fontSize) + (_offsetZ_SymbolSizeUnits * symbolSize / 2);
      var stringsize = g.MeasureString(label, _font, new PointD3D(xpos, ypos, zpos));

      if (_backgroundStyle != null)
      {
        var x = xpos;
        var y = ypos;
        var z = zpos;

        switch (_alignmentX)
        {
          case Alignment.Center:
            x -= stringsize.X / 2;
            break;

          case Alignment.Far:
            x -= stringsize.X;
            break;
        }

        switch (_alignmentY)
        {
          case Alignment.Center:
            y -= stringsize.Y / 2;
            break;

          case Alignment.Far:
            y -= stringsize.Y;
            break;
        }
        if (null == variableBackBrush)
        {
          _backgroundStyle.Draw(g, new RectangleD3D(x, y, z, stringsize.X, stringsize.Y, stringsize.Z));
        }
        else
        {
          _backgroundStyle.Draw(g, new RectangleD3D(x, y, z, stringsize.X, stringsize.Y, stringsize.Z), variableBackBrush);
        }
      }

      var brush = null != variableTextBrush ? variableTextBrush : _material;
      g.DrawString(label, _font, brush, new PointD3D(xpos, ypos, zpos), _alignmentX, _alignmentY, _alignmentZ);
    }

    public void Paint(IGraphicsContext3D g, IPlotArea layer, Processed3DPlotData pdata, Processed3DPlotData prevItemData, Processed3DPlotData nextItemData)
    {
      if (_labelColumnProxy.Document == null)
        return;

      if (null != _attachedPlane)
        _attachedPlane = layer.UpdateCSPlaneID(_attachedPlane);

      PlotRangeList rangeList = pdata.RangeList;
      var ptArray = pdata.PlotPointsInAbsoluteLayerCoordinates;
      Altaxo.Data.IReadableColumn labelColumn = _labelColumnProxy.Document;

      bool isUsingVariableColorForLabelText = null != _cachedColorForIndexFunction && IsColorReceiver;
      bool isUsingVariableColorForLabelBackground = null != _cachedColorForIndexFunction &&
        (null != _backgroundStyle && _backgroundStyle.SupportsUserDefinedMaterial && (_backgroundColorLinkage == ColorLinkage.Dependent || _backgroundColorLinkage == ColorLinkage.PreserveAlpha));
      bool isUsingVariableColor = isUsingVariableColorForLabelText || isUsingVariableColorForLabelBackground;
      IMaterial clonedTextBrush = _material;
      IMaterial clonedBackBrush = null;
      if (isUsingVariableColorForLabelBackground)
        clonedBackBrush = _backgroundStyle.Material;

      // save the graphics stat since we have to translate the origin
      var gs = g.SaveGraphicsState();

      double xpos = 0, ypos = 0, zpos = 0;
      double xpre, ypre, zpre;
      double xdiff, ydiff, zdiff;

      bool isFormatStringContainingBraces = _labelFormatString?.IndexOf('{') >= 0;
      var culture = System.Threading.Thread.CurrentThread.CurrentCulture;

      bool mustUseLogicalCoordinates = null != _attachedPlane || 0 != _cachedLogicalShiftX || 0 != _cachedLogicalShiftY || 0 != _cachedLogicalShiftZ;

      for (int r = 0; r < rangeList.Count; r++)
      {
        int lower = rangeList[r].LowerBound;
        int upper = rangeList[r].UpperBound;
        int offset = rangeList[r].OffsetToOriginal;
        for (int j = lower; j < upper; j += _skipFrequency)
        {
          string label;
          if (string.IsNullOrEmpty(_labelFormatString))
          {
            label = labelColumn[j + offset].ToString();
          }
          else if (!isFormatStringContainingBraces)
          {
            label = labelColumn[j + offset].ToString(_labelFormatString, culture);
          }
          else
          {
            // the label format string can contain {0} for the label column item, {1} for the row index, {2} .. {4} for the x, y and z component of the data point
            label = string.Format(_labelFormatString, labelColumn[j + offset], j + offset, pdata.GetPhysical(0, j + offset), pdata.GetPhysical(1, j + offset), pdata.GetPhysical(2, j + offset));
          }

          if (string.IsNullOrEmpty(label))
            continue;

          double localSymbolSize = _symbolSize;
          if (null != _cachedSymbolSizeForIndexFunction)
          {
            localSymbolSize = _cachedSymbolSizeForIndexFunction(j + offset);
          }

          double localFontSize = _fontSizeOffset + _fontSizeFactor * localSymbolSize;
          if (!(localFontSize > 0))
            continue;

          _font = _font.WithSize(localFontSize);

          // Start of preparation of brushes, if a variable color is used
          if (isUsingVariableColor)
          {
            var c = _cachedColorForIndexFunction(j + offset);

            if (isUsingVariableColorForLabelText)
            {
              clonedTextBrush = clonedTextBrush.WithColor(new NamedColor(AxoColor.FromArgb(c.A, c.R, c.G, c.B), "e"));
            }
            if (isUsingVariableColorForLabelBackground)
            {
              if (_backgroundColorLinkage == ColorLinkage.PreserveAlpha)
                clonedBackBrush = clonedBackBrush.WithColor(new NamedColor(AxoColor.FromArgb(clonedBackBrush.Color.Color.A, c.R, c.G, c.B), "e"));
              else
                clonedBackBrush = clonedBackBrush.WithColor(new NamedColor(AxoColor.FromArgb(c.A, c.R, c.G, c.B), "e"));
            }
          }
          // end of preparation of brushes for variable colors

          if (mustUseLogicalCoordinates) // we must use logical coordinates because either there is a shift of logical coordinates, or an attached plane
          {
            Logical3D r3d = layer.GetLogical3D(pdata, j + offset);
            r3d.RX += _cachedLogicalShiftX;
            r3d.RY += _cachedLogicalShiftY;
            r3d.RZ += _cachedLogicalShiftZ;

            if (null != _attachedPlane)
            {
              var pp = layer.CoordinateSystem.GetPointOnPlane(_attachedPlane, r3d);
              xpre = pp.X;
              ypre = pp.Y;
              zpre = pp.Z;
            }
            else
            {
              layer.CoordinateSystem.LogicalToLayerCoordinates(r3d, out var pt);
              xpre = pt.X;
              ypre = pt.Y;
              zpre = pt.Z;
            }
          }
          else // no shifting, thus we can use layer coordinates
          {
            xpre = ptArray[j].X;
            ypre = ptArray[j].Y;
            zpre = ptArray[j].Z;
          }

          xdiff = xpre - xpos;
          ydiff = ypre - ypos;
          zdiff = zpre - zpos;
          xpos = xpre;
          ypos = ypre;
          zpos = zpre;
          g.TranslateTransform(xdiff, ydiff, zdiff);
          g.RotateTransform(_rotationX, _rotationY, _rotationZ);

          Paint(g, label, localSymbolSize, clonedTextBrush, clonedBackBrush);

          g.RotateTransform(-_rotationX, -_rotationY, -_rotationZ);
        } // end for
      }

      g.RestoreGraphicsState(gs); // Restore the graphics state
    }

    public RectangleD3D PaintSymbol(IGraphicsContext3D g, RectangleD3D bounds)
    {
      return bounds;
    }

    /// <summary>
    /// Prepares the scale of this plot style. Since this style does not utilize a scale, this function does nothing.
    /// </summary>
    /// <param name="layer">The parent layer.</param>
    public void PrepareScales(Graph3D.IPlotArea layer)
    {
    }

    #region I3DPlotStyle Members

    public bool IsColorProvider
    {
      get { return _independentColor == false; }
    }

    public bool IsColorReceiver
    {
      get { return IndependentColor == false; }
    }

    /// <summary>
    /// Gets a value indicating whether the background can provide a color for use by other plot styles.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if the background is able to provide a color; otherwise, <c>false</c>.
    /// </value>
    public bool IsBackgroundColorProvider
    {
      get
      {
        return
          _backgroundStyle != null &&
          _backgroundStyle.SupportsUserDefinedMaterial &&
          _backgroundColorLinkage == ColorLinkage.Dependent;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the background can receive a color value.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is a receiver for background color; otherwise, <c>false</c>.
    /// </value>
    public bool IsBackgroundColorReceiver
    {
      get
      {
        return
          _backgroundStyle != null &&
          _backgroundStyle.SupportsUserDefinedMaterial &&
          (_backgroundColorLinkage == ColorLinkage.Dependent || _backgroundColorLinkage == ColorLinkage.PreserveAlpha);
      }
    }

    #region IG3DPlotStyle Members

    public void CollectExternalGroupStyles(PlotGroupStyleCollection externalGroups)
    {
      if (IsColorProvider)
        ColorGroupStyle.AddExternalGroupStyle(externalGroups);
    }

    public void CollectLocalGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {
      if (IsColorProvider)
        ColorGroupStyle.AddLocalGroupStyle(externalGroups, localGroups);
    }

    public void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups, IPlotArea layer, Processed3DPlotData pdata)
    {
      if (IsColorProvider)
        ColorGroupStyle.PrepareStyle(externalGroups, localGroups, delegate ()
        { return Material.Color; });
      else if (IsBackgroundColorProvider)
        ColorGroupStyle.PrepareStyle(externalGroups, localGroups, delegate ()
        { return _backgroundStyle.Material.Color; });
    }

    public void ApplyGroupStyles(PlotGroupStyleCollection externalGroups, PlotGroupStyleCollection localGroups)
    {
      // SkipFrequency should be the same for all sub plot styles, so there is no "private" property
      if (!_independentSkipFrequency)
      {
        _skipFrequency = 1;
        SkipFrequencyGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (int c)
        { _skipFrequency = c; });
      }

      // Symbol size
      if (!_independentSymbolSize)
      {
        _symbolSize = 0;
        SymbolSizeGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (double size)
        { _symbolSize = size; });
        // but if there is an symbol size evaluation function, then use this with higher priority.
        if (!VariableSymbolSizeGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (Func<int, double> evalFunc)
        { _cachedSymbolSizeForIndexFunction = evalFunc; }))
          _cachedSymbolSizeForIndexFunction = null;
      }

      // Color
      _cachedColorForIndexFunction = null;
      if (IsColorReceiver)
      {
        // try to get a constant color ...
        ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (NamedColor c)
        { Material = Material.WithColor(c); });
      }

      if (IsBackgroundColorReceiver)
      {
        if (_backgroundColorLinkage == ColorLinkage.Dependent)
          ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (NamedColor c)
          { _backgroundStyle.Material = _backgroundStyle.Material.WithColor(c); });
        else if (_backgroundColorLinkage == ColorLinkage.PreserveAlpha)
          ColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (NamedColor c)
          { _backgroundStyle.Material = _backgroundStyle.Material.WithColor(c.NewWithAlphaValue(_backgroundStyle.Material.Color.Color.A)); });
      }

      if (IsColorReceiver || IsBackgroundColorReceiver)
      {
        // but if there is a color evaluation function, then use that function with higher priority
        VariableColorGroupStyle.ApplyStyle(externalGroups, localGroups, delegate (Func<int, System.Drawing.Color> evalFunc)
        { _cachedColorForIndexFunction = evalFunc; });
      }

      // Shift the items ?
      _cachedLogicalShiftX = 0;
      _cachedLogicalShiftY = 0;
      if (!_independentOnShiftingGroupStyles)
      {
        var shiftStyle = PlotGroupStyle.GetFirstStyleToApplyImplementingInterface<IShiftLogicalXYZGroupStyle>(externalGroups, localGroups);
        if (null != shiftStyle)
        {
          shiftStyle.Apply(out _cachedLogicalShiftX, out _cachedLogicalShiftY, out _cachedLogicalShiftZ);
        }
      }
    }

    #endregion IG3DPlotStyle Members

    #endregion I3DPlotStyle Members

    #region IDocumentNode Members

    /// <summary>
    /// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
    /// to change a plot so that the plot items refer to another table.
    /// </summary>
    /// <param name="Report">Information what to replace.</param>
    public void VisitDocumentReferences(DocNodeProxyReporter Report)
    {
      Report(_labelColumnProxy, this, "LabelColumn");
    }

    #endregion IDocumentNode Members
  }
}
