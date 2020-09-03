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
using System.Drawing.Drawing2D;
using Altaxo.Data;
using Altaxo.Graph.Gdi.Background;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Ticks;

namespace Altaxo.Graph.Gdi.Axis
{
  using System.Diagnostics.CodeAnalysis;
  using Altaxo.Drawing;
  using Altaxo.Main;
  using Gdi.LabelFormatting;
  using Geometry;

  /// <summary>
  /// Responsible for setting position, rotation, font, color etc. of axis labels.
  /// </summary>
  public class AxisLabelStyle :
    Main.SuspendableDocumentNodeWithSetOfEventArgs,
    IRoutedPropertyReceiver,
    Main.ICopyFrom
  {
    protected FontX _font;

    protected StringAlignment _horizontalAlignment;
    protected StringAlignment _verticalAlignment;

    protected StringFormat _stringFormat;
    protected BrushX _brush;

    /// <summary>The x offset in EM units.</summary>
    protected double _xOffset;

    /// <summary>The y offset in EM units.</summary>
    protected double _yOffset;

    /// <summary>The rotation of the label.</summary>
    protected double _rotation;

    /// <summary>The style for the background.</summary>
    protected Gdi.Background.IBackgroundStyle? _backgroundStyle;

    protected bool _automaticRotationShift;

    protected SuppressedTicks _suppressedLabels;

    private ILabelFormatting _labelFormatting;

    /// <summary>
    /// If set, this overrides the preferred label side that comes along with the axis style.
    /// </summary>
    private CSAxisSide? _labelSide;

    private string? _prefixText;

    private string? _postfixText;

    private CSAxisInformation? _cachedAxisStyleInfo;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYAxisLabelStyle", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotSupportedException("Serialization of old versions is not supported - probably a programming error");
        /*
                XYAxisLabelStyle s = (XYAxisLabelStyle)obj;
                info.AddValue("Edge",s._edge);
                info.AddValue("Font",s._font);
                */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (AxisLabelStyle?)o ?? new AxisLabelStyle(info);

        var edge = (Edge)info.GetValue("Edge", s);
        s._font = (FontX)info.GetValue("Font", s);

        s._brush = new BrushX(NamedColors.Black);
        s._automaticRotationShift = true;
        s._suppressedLabels = new SuppressedTicks() { ParentObject = s };
        s.ChildSetMember(ref s._labelFormatting, new Gdi.LabelFormatting.NumericLabelFormattingAuto());
        s.SetStringFormat();
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYAxisLabelStyle", 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotSupportedException("Serialization of old versions is not supported - probably a programming error");
        /*
                XYAxisLabelStyle s = (XYAxisLabelStyle)obj;
                info.AddValue("Edge",s._edge);
                info.AddValue("Font",s._font);
                info.AddValue("Brush",s._brush);
                info.AddValue("Background",s._backgroundStyle);

                info.AddValue("AutoAlignment",s._automaticRotationShift);
                info.AddEnum("HorzAlignment",s._horizontalAlignment);
                info.AddEnum("VertAlignment",s._verticalAlignment);

                info.AddValue("Rotation",s._rotation);
                info.AddValue("XOffset",s._xOffset);
                info.AddValue("YOffset",s._yOffset);

                info.AddValue("LabelFormat",s._labelFormatting);
                */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (AxisLabelStyle?)o ?? new AxisLabelStyle(info);

        var edge = (Edge)info.GetValue("Edge", s);
        s._font = (FontX)info.GetValue("Font", s);
        s._brush = (BrushX)info.GetValue("Brush", s);
        s.ChildSetMember(ref s._backgroundStyle, info.GetValueOrNull<IBackgroundStyle>("Background", s));

        s._automaticRotationShift = info.GetBoolean("AutoAlignment");
        s._horizontalAlignment = (StringAlignment)info.GetEnum("HorzAlignment", typeof(StringAlignment));
        s._verticalAlignment = (StringAlignment)info.GetEnum("VertAlignment", typeof(StringAlignment));
        s._rotation = info.GetDouble("Rotation");
        s._xOffset = info.GetDouble("XOffset");
        s._yOffset = info.GetDouble("YOffset");

        s.ChildSetMember(ref s._labelFormatting, (ILabelFormatting)info.GetValue("LabelFormat", s));

        s._suppressedLabels = new SuppressedTicks() { ParentObject = s };

        // Modification of StringFormat is necessary to avoid
        // too big spaces between successive words
        s._stringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
        s._stringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYAxisLabelStyle", 2)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AxisLabelStyle), 3)]
    private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (AxisLabelStyle)obj;
        info.AddValue("Font", s._font);
        info.AddValue("Brush", s._brush);
        info.AddValueOrNull("Background", s._backgroundStyle);

        info.AddValue("AutoAlignment", s._automaticRotationShift);
        info.AddEnum("HorzAlignment", s._horizontalAlignment);
        info.AddEnum("VertAlignment", s._verticalAlignment);

        info.AddValue("Rotation", s._rotation);
        info.AddValue("XOffset", s._xOffset);
        info.AddValue("YOffset", s._yOffset);

        info.AddValue("LabelFormat", s._labelFormatting);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (AxisLabelStyle?)o ?? new AxisLabelStyle(info);

        s._font = (FontX)info.GetValue("Font", s);
        s._brush = (BrushX)info.GetValue("Brush", s);
        s.ChildSetMember(ref s._backgroundStyle, info.GetValueOrNull<IBackgroundStyle>("Background", s));
        s._automaticRotationShift = info.GetBoolean("AutoAlignment");
        s._horizontalAlignment = (StringAlignment)info.GetEnum("HorzAlignment", typeof(StringAlignment));
        s._verticalAlignment = (StringAlignment)info.GetEnum("VertAlignment", typeof(StringAlignment));
        s._rotation = info.GetDouble("Rotation");
        s._xOffset = info.GetDouble("XOffset");
        s._yOffset = info.GetDouble("YOffset");

        s.ChildSetMember(ref s._labelFormatting, (ILabelFormatting)info.GetValue("LabelFormat", s));

        s._suppressedLabels = new SuppressedTicks() { ParentObject = s };

        // Modification of StringFormat is necessary to avoid
        // too big spaces between successive words
        s._stringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
        s._stringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AxisLabelStyle), 4)]
    private class XmlSerializationSurrogate4 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Trying to serialize old version");
        /*
                AxisLabelStyle s = (AxisLabelStyle)obj;
                info.AddValue("Font", s._font);
                info.AddValue("Brush", s._brush);
                info.AddValue("Background", s._backgroundStyle);

                info.AddValue("AutoAlignment", s._automaticRotationShift);
                info.AddEnum("HorzAlignment", s._horizontalAlignment);
                info.AddEnum("VertAlignment", s._verticalAlignment);

                info.AddValue("Rotation", s._rotation);
                info.AddValue("XOffset", s._xOffset);
                info.AddValue("YOffset", s._yOffset);

                if (s._suppressedLabels.IsEmpty)
                    info.AddValue("SuppressedLabels", (object)null);
                else
                    info.AddValue("SuppressedLabels", s._suppressedLabels);

                info.AddValue("LabelFormat", s._labelFormatting);
                */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (AxisLabelStyle?)o ?? new AxisLabelStyle(info);

        s._font = (FontX)info.GetValue("Font", s);
        s._brush = (BrushX)info.GetValue("Brush", s);
        s.ChildSetMember(ref s._backgroundStyle, info.GetValueOrNull<IBackgroundStyle>("Background", s));
        s._automaticRotationShift = info.GetBoolean("AutoAlignment");
        s._horizontalAlignment = (StringAlignment)info.GetEnum("HorzAlignment", typeof(StringAlignment));
        s._verticalAlignment = (StringAlignment)info.GetEnum("VertAlignment", typeof(StringAlignment));
        s._rotation = info.GetDouble("Rotation");
        s._xOffset = info.GetDouble("XOffset");
        s._yOffset = info.GetDouble("YOffset");

        s.ChildSetMember(ref s._suppressedLabels, info.GetValueOrNull<SuppressedTicks>("SuppressedLabels", s) ?? new SuppressedTicks());
        s.ChildSetMember(ref s._labelFormatting, (ILabelFormatting)info.GetValue("LabelFormat", s));

        // Modification of StringFormat is necessary to avoid
        // too big spaces between successive words
        s._stringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
        s._stringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AxisLabelStyle), 5)]
    private class XmlSerializationSurrogate5 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      // 2012-03-30 new member _labelSide, _prefixText and _postfixText

      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Try to serialize outdated version");
        /*
                AxisLabelStyle s = (AxisLabelStyle)obj;
                info.AddValue("Font", s._font);
                info.AddValue("Brush", s._brush);
                info.AddValue("Background", s._backgroundStyle);

                info.AddValue("AutoAlignment", s._automaticRotationShift);
                info.AddEnum("HorzAlignment", s._horizontalAlignment);
                info.AddEnum("VertAlignment", s._verticalAlignment);

                info.AddValue("Rotation", s._rotation);
                info.AddValue("XOffset", s._xOffset);
                info.AddValue("YOffset", s._yOffset);

                if (s._suppressedLabels.IsEmpty)
                    info.AddValue("SuppressedLabels", (object)null);
                else
                    info.AddValue("SuppressedLabels", s._suppressedLabels);

                info.AddValue("LabelFormat", s._labelFormatting);

                info.AddNullableEnum("LabelSide", s._labelSide);
                info.AddValue("PrefixText", s._prefixText);
                info.AddValue("PostfixText", s._postfixText);
                */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (AxisLabelStyle?)o ?? new AxisLabelStyle(info);

        s._font = (FontX)info.GetValue("Font", s);
        s._brush = (BrushX)info.GetValue("Brush", s);
        s.ChildSetMember(ref s._backgroundStyle, (IBackgroundStyle)info.GetValue("Background", s));
        s._automaticRotationShift = info.GetBoolean("AutoAlignment");
        s._horizontalAlignment = (StringAlignment)info.GetEnum("HorzAlignment", typeof(StringAlignment));
        s._verticalAlignment = (StringAlignment)info.GetEnum("VertAlignment", typeof(StringAlignment));
        s._rotation = info.GetDouble("Rotation");
        s._xOffset = info.GetDouble("XOffset");
        s._yOffset = info.GetDouble("YOffset");

        s.ChildSetMember(ref s._suppressedLabels, info.GetValueOrNull<SuppressedTicks>("SuppressedLabels", s) ?? new SuppressedTicks());
        s.ChildSetMember(ref s._labelFormatting, (ILabelFormatting)info.GetValue("LabelFormat", s));

        s._labelSide = info.GetNullableEnum<CSAxisSide>("LabelSide");
        s._labelFormatting.PrefixText = info.GetString("PrefixText");
        s._labelFormatting.SuffixText = info.GetString("PostfixText");

        // Modification of StringFormat is necessary to avoid
        // too big spaces between successive words
        s._stringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
        s._stringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AxisLabelStyle), 6)]
    private class XmlSerializationSurrogate6 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      // 2012-05-28 _prefixText and _postfixText deprecated and moved to LabelFormattingBase

      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (AxisLabelStyle)obj;
        info.AddValue("Font", s._font);
        info.AddValue("Brush", s._brush);
        info.AddValueOrNull("Background", s._backgroundStyle);

        info.AddValue("AutoAlignment", s._automaticRotationShift);
        info.AddEnum("HorzAlignment", s._horizontalAlignment);
        info.AddEnum("VertAlignment", s._verticalAlignment);

        info.AddValue("Rotation", s._rotation);
        info.AddValue("XOffset", s._xOffset);
        info.AddValue("YOffset", s._yOffset);

        if (s._suppressedLabels.IsEmpty)
          info.AddValueOrNull("SuppressedLabels", (object?)null);
        else
          info.AddValue("SuppressedLabels", s._suppressedLabels);

        info.AddValue("LabelFormat", s._labelFormatting);

        info.AddNullableEnum("LabelSide", s._labelSide);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (AxisLabelStyle?)o ?? new AxisLabelStyle(info);

        s._font = (FontX)info.GetValue("Font", s);
        s._brush = (BrushX)info.GetValue("Brush", s);

        s.ChildSetMember(ref s._backgroundStyle, info.GetValueOrNull<IBackgroundStyle>("Background", s));

        s._automaticRotationShift = info.GetBoolean("AutoAlignment");
        s._horizontalAlignment = (StringAlignment)info.GetEnum("HorzAlignment", typeof(StringAlignment));
        s._verticalAlignment = (StringAlignment)info.GetEnum("VertAlignment", typeof(StringAlignment));
        s._rotation = info.GetDouble("Rotation");
        s._xOffset = info.GetDouble("XOffset");
        s._yOffset = info.GetDouble("YOffset");

        s.ChildSetMember(ref s._suppressedLabels, info.GetValueOrNull<SuppressedTicks>("SuppressedLabels", s) ?? new SuppressedTicks());
        s.ChildSetMember(ref s._labelFormatting, (ILabelFormatting)info.GetValue("LabelFormat", s));

        s._labelSide = info.GetNullableEnum<CSAxisSide>("LabelSide");

        // Modification of StringFormat is necessary to avoid
        // too big spaces between successive words
        s._stringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
        s._stringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

        return s;
      }
    }

    /// <summary>
    /// Finale measures after deserialization.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public virtual void OnDeserialization(object obj)
    {
    }

    #endregion Serialization

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected AxisLabelStyle(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    {
    }

    public AxisLabelStyle(Altaxo.Main.Properties.IReadOnlyPropertyBag? context)
    {
      context ??= PropertyExtensions.GetPropertyContextOfProject();

      _font = context.GetValue(GraphDocument.PropertyKeyDefaultFont);
      var foreColor = context.GetValue(GraphDocument.PropertyKeyDefaultForeColor);

      _brush = new BrushX(foreColor);
      _automaticRotationShift = true;
      _suppressedLabels = new SuppressedTicks() { ParentObject = this };
      _labelFormatting = new Gdi.LabelFormatting.NumericLabelFormattingAuto() { ParentObject = this };
      SetStringFormat();
    }

    public AxisLabelStyle(AxisLabelStyle from)
    {
      CopyFrom(from);
    }

    [MemberNotNull(nameof(_font), nameof(_stringFormat), nameof(_brush), nameof(_suppressedLabels), nameof(_labelFormatting))]
    protected void CopyFrom(AxisLabelStyle from)
    {
      using (var suspendToken = SuspendGetToken())
      {
        _cachedAxisStyleInfo = from._cachedAxisStyleInfo;

        _font = from._font;
        CopyHelper.Copy(ref _stringFormat, from._stringFormat);
        _horizontalAlignment = from._horizontalAlignment;
        _verticalAlignment = from._verticalAlignment;

        _brush = from._brush;

        _automaticRotationShift = from._automaticRotationShift;
        _xOffset = from._xOffset;
        _yOffset = from._yOffset;
        _rotation = from._rotation;
        ChildCopyToMember(ref _backgroundStyle, from._backgroundStyle);
        ChildCopyToMember(ref _labelFormatting, from._labelFormatting);
        _labelSide = from._labelSide;
        _prefixText = from._prefixText;
        _postfixText = from._postfixText;
        ChildCopyToMember(ref _suppressedLabels, from._suppressedLabels);
        EhSelfChanged(EventArgs.Empty);

        suspendToken.Resume();
      }

    }

    public virtual bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;
      if (obj is AxisLabelStyle from)
      {
        CopyFrom(from);
        return true;
      }

      return false;
    }

    public virtual object Clone()
    {
      return new AxisLabelStyle(this);
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_labelFormatting is not null)
        yield return new Main.DocumentNodeAndName(_labelFormatting, "LabelFormatting");

      if (_backgroundStyle is not null)
        yield return new Main.DocumentNodeAndName(_backgroundStyle, "BackgroundStyle");

      if (_suppressedLabels is not null)
        yield return new Main.DocumentNodeAndName(_suppressedLabels, "SuppressedLabels");
    }

    [MemberNotNull(nameof(_stringFormat))]
    private void SetStringFormat()
    {
      // Modification of StringFormat is necessary to avoid
      // too big spaces between successive words
      _stringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
      _stringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

      /*
            // set the alignment and line alignment of the strings
            switch(this._edge.TypeOfEdge)
            {
                case EdgeType.Bottom:
                    _verticalAlignment = StringAlignment.Near;
                    _horizontalAlignment = StringAlignment.Center;
                    break;

                case EdgeType.Top:
                    _verticalAlignment = StringAlignment.Far;
                    _horizontalAlignment = StringAlignment.Center;
                    break;

                case EdgeType.Left:
                    _verticalAlignment = StringAlignment.Center;
                    _horizontalAlignment = StringAlignment.Far;
                    break;

                case EdgeType.Right:
                    _verticalAlignment = StringAlignment.Center;
                    _horizontalAlignment = StringAlignment.Near;
                    break;
            }
             */
    }

    #region Properties

    /// <summary>The font of the label.</summary>
    public FontX Font
    {
      get { return _font; }
      set
      {
        if (value is null)
          throw new ArgumentNullException();

        var oldValue = _font;
        _font = value;

        if (value.Equals(oldValue))
          EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>The font size of the label.</summary>
    public virtual double FontSize
    {
      get { return _font.Size; }
      set
      {
        var oldValue = _font.Size;
        var newValue = Math.Max(0, value);

        if (newValue != oldValue)
        {
          FontX oldFont = _font;
          _font = oldFont.WithSize(newValue);

          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>The brush color. If you set this, the font brush will be set to a solid brush.</summary>
    public NamedColor Color
    {
      get { return _brush.Color; }
      set
      {
        if (!(Color == value))
        {
          _brush = new BrushX(value);
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>The brush. During setting, the brush is cloned.</summary>
    public BrushX Brush
    {
      get
      {
        return _brush;
      }
      set
      {
        if (!(_brush == value))
        {
          _brush = value;
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>The background style.</summary>
    public Gdi.Background.IBackgroundStyle? BackgroundStyle
    {
      get
      {
        return _backgroundStyle;
      }
      set
      {
        if (ChildSetMember(ref _backgroundStyle, value))
        {
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public SuppressedTicks SuppressedLabels
    {
      get
      {
        return _suppressedLabels;
      }
    }

    public ILabelFormatting LabelFormat
    {
      get
      {
        return _labelFormatting;
      }
      set
      {
        if (value is null)
          throw new ArgumentNullException("value");

        if (object.ReferenceEquals(_labelFormatting, value))
          return;

        _labelFormatting = value;
        _labelFormatting.ParentObject = this;
        EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>Gets or sets the label side. If the value is set to <c>null</c>, the label side is chosen automatically. If set to a value different from <c>null</c>,
    /// the label is shown on this side of the axis.</summary>
    /// <value>The label side where the label should be shown, or <c>null</c> to choose the side automatically.</value>
    public CSAxisSide? LabelSide
    {
      get
      {
        return _labelSide;
      }
      set
      {
        var oldValue = _labelSide;
        _labelSide = value;
        if (value != oldValue)
          EhSelfChanged(EventArgs.Empty);
      }
    }

    public string PrefixText
    {
      get
      {
        return _labelFormatting.PrefixText;
      }
      set
      {
        _labelFormatting.PrefixText = value;
      }
    }

    public string SuffixText
    {
      get
      {
        return _labelFormatting.SuffixText;
      }
      set
      {
        var oldValue = _labelFormatting.SuffixText;
        _labelFormatting.SuffixText = value;
        if (value != oldValue)
          EhSelfChanged(EventArgs.Empty);
      }
    }

    /// <summary>The x offset relative to font size, i.e. a value of 1 is 1*FontSize.</summary>
    public double XOffset
    {
      get { return _xOffset; }
      set
      {
        double oldValue = _xOffset;
        _xOffset = value;
        if (value != oldValue)
        {
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>The y offset relative to font size, i.e. a value of 1 is 1*FontSize.</summary>
    public double YOffset
    {
      get { return _yOffset; }
      set
      {
        double oldValue = _yOffset;
        _yOffset = value;
        if (value != oldValue)
        {
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>The angle of the label.</summary>
    public double Rotation
    {
      get { return _rotation; }
      set
      {
        double oldValue = _rotation;
        _rotation = value;
        if (value != oldValue)
        {
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    public bool AutomaticAlignment
    {
      get
      {
        return _automaticRotationShift;
      }
      set
      {
        bool oldValue = AutomaticAlignment;
        _automaticRotationShift = value;
        if (value != oldValue)
        {
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>Horizontal alignment of the label.</summary>
    public System.Drawing.StringAlignment HorizontalAlignment
    {
      get
      {
        return _horizontalAlignment;
      }
      set
      {
        System.Drawing.StringAlignment oldValue = HorizontalAlignment;
        _horizontalAlignment = value;
        if (value != oldValue)
        {
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    /// <summary>Vertical aligment of the label.</summary>
    public System.Drawing.StringAlignment VerticalAlignment
    {
      get { return _verticalAlignment; }
      set
      {
        System.Drawing.StringAlignment oldValue = VerticalAlignment;
        _verticalAlignment = value;
        if (value != oldValue)
        {
          EhSelfChanged(EventArgs.Empty);
        }
      }
    }

    #endregion Properties

    public CSLineID AxisStyleID
    {
      get
      {
        if (_cachedAxisStyleInfo is null)
          throw new InvalidOperationException($"{nameof(_cachedAxisStyleInfo)} is null");
        return _cachedAxisStyleInfo.Identifier;
      }
    }

    public CSAxisInformation? CachedAxisInformation
    {
      get
      {
        return _cachedAxisStyleInfo;
      }
      set
      {
        _cachedAxisStyleInfo = value;
      }
    }

    public virtual IHitTestObject? HitTest(IPlotArea layer, PointD2D pt)
    {
      GraphicsPath gp = GetSelectionPath();
      if (gp.IsVisible((PointF)pt))
        return new HitTestObject(gp, this);
      else
        return null;
    }

    public virtual IHitTestObject? HitTest(IPlotArea layer, HitTestRectangularData parentHitData)
    {
      GraphicsPath gp = GetSelectionPath();
      if (gp.PointCount > 0 && parentHitData.IsCovering(gp.PathPoints))
        return new HitTestObject(gp, this);
      else
        return null;
    }

    public void AdjustRectangle(ref RectangleD2D r, StringAlignment horz, StringAlignment vert)
    {
      switch (vert)
      {
        case StringAlignment.Near:
          break;

        case StringAlignment.Center:
          r.Y -= 0.5 * r.Height;
          break;

        case StringAlignment.Far:
          r.Y -= r.Height;
          break;
      }
      switch (horz)
      {
        case StringAlignment.Near:
          break;

        case StringAlignment.Center:
          r.X -= 0.5 * r.Width;
          break;

        case StringAlignment.Far:
          r.X -= r.Width;
          break;
      }
    }

    /// <summary>
    /// Gives the path where the hit test is successfull.
    /// </summary>
    /// <returns></returns>
    public virtual GraphicsPath GetSelectionPath()
    {
      return (GraphicsPath)_enclosingPath.Clone();
    }

    private GraphicsPath _enclosingPath = new GraphicsPath(); // with Winding also overlapping rectangles are selected

    /// <summary>Predicts the side, where the label will be shown using the given axis information.</summary>
    /// <param name="axisInformation">The axis information.</param>
    /// <returns>The side of the axis where the label will be shown.</returns>
    public virtual CSAxisSide PredictLabelSide(CSAxisInformation axisInformation)
    {
      return _labelSide is not null ? _labelSide.Value : axisInformation.PreferredLabelSide;
    }

    /// <summary>
    /// Paints the axis style labels.
    /// </summary>
    /// <param name="g">Graphics environment.</param>
    /// <param name="coordSyst">The coordinate system. Used to get the path along the axis.</param>
    /// <param name="scale">Scale.</param>
    /// <param name="tickSpacing">If not <c>null</c>, this parameter provides a custom tick spacing that is used instead of the default tick spacing of the scale.</param>
    /// <param name="styleInfo">Information about begin of axis, end of axis.</param>
    /// <param name="outerDistance">Distance between axis and labels.</param>
    /// <param name="useMinorTicks">If true, minor ticks are shown.</param>
    public virtual void Paint(Graphics g, G2DCoordinateSystem coordSyst, Scale scale, TickSpacing tickSpacing, CSAxisInformation styleInfo, double outerDistance, bool useMinorTicks)
    {
      _cachedAxisStyleInfo = styleInfo;
      CSLineID styleID = styleInfo.Identifier;
      Scale raxis = scale;
      TickSpacing ticking = tickSpacing;

      _enclosingPath.Reset();
      _enclosingPath.FillMode = FillMode.Winding; // with Winding also overlapping rectangles are selected
      var helperPath = new GraphicsPath();
      var math = new Matrix();

      Logical3D r0 = styleID.GetLogicalPoint(styleInfo.LogicalValueAxisOrg);
      Logical3D r1 = styleID.GetLogicalPoint(styleInfo.LogicalValueAxisEnd);

      Logical3D outer;
      var dist_x = outerDistance; // Distance from axis tick point to label
      var dist_y = outerDistance; // y distance from axis tick point to label

      // dist_x += this._font.SizeInPoints/3; // add some space to the horizontal direction in order to separate the chars a little from the ticks

      // next statement is necessary to have a consistent string length both
      // on 0 degree rotated text and rotated text
      // without this statement, the text is fitted to the pixel grid, which
      // leads to "steps" during scaling
      g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

      double[] relpositions;
      AltaxoVariant[] ticks;
      if (useMinorTicks)
      {
        relpositions = ticking.GetMinorTicksNormal(raxis);
        ticks = ticking.GetMinorTicksAsVariant();
      }
      else
      {
        relpositions = ticking.GetMajorTicksNormal(raxis);
        ticks = ticking.GetMajorTicksAsVariant();
      }

      if (!_suppressedLabels.IsEmpty)
      {
        var filteredTicks = new List<AltaxoVariant>();
        var filteredRelPositions = new List<double>();

        for (int i = 0; i < ticks.Length; i++)
        {
          if (_suppressedLabels.ByValues.Contains(ticks[i]))
            continue;
          if (_suppressedLabels.ByNumbers.Contains(i))
            continue;
          if (_suppressedLabels.ByNumbers.Contains(i - ticks.Length))
            continue;

          filteredTicks.Add(ticks[i]);
          filteredRelPositions.Add(relpositions[i]);
        }
        ticks = filteredTicks.ToArray();
        relpositions = filteredRelPositions.ToArray();
      }

      IMeasuredLabelItem[] labels = _labelFormatting.GetMeasuredItems(g, _font, _stringFormat, ticks);

      double emSize = _font.Size;
      CSAxisSide labelSide = _labelSide is not null ? _labelSide.Value : styleInfo.PreferredLabelSide;
      for (int i = 0; i < ticks.Length; i++)
      {
        double r = relpositions[i];

        if (!Altaxo.Calc.RMath.IsInIntervalCC(r, -1000, 1000))
          continue;

        outer = coordSyst.GetLogicalDirection(styleID.ParallelAxisNumber, labelSide);
        PointD2D tickorg = coordSyst.GetNormalizedDirection(r0, r1, r, outer, out var outVector);
        PointD2D tickend = tickorg + outVector * outerDistance;

        PointD2D msize = labels[i].Size;
        PointD2D morg = tickend;

        if (_automaticRotationShift)
        {
          double alpha = _rotation * Math.PI / 180 - Math.Atan2(outVector.Y, outVector.X);
          double shift = msize.Y * 0.5 * Math.Abs(Math.Sin(alpha)) + (msize.X + _font.Size / 2) * 0.5 * Math.Abs(Math.Cos(alpha));
          morg = morg + outVector * shift;
        }
        else
        {
          morg = morg.WithXPlus(outVector.X * _font.Size / 3);
        }

        var mrect = new RectangleD2D(morg, msize);
        if (_automaticRotationShift)
          AdjustRectangle(ref mrect, StringAlignment.Center, StringAlignment.Center);
        else
          AdjustRectangle(ref mrect, _horizontalAlignment, _verticalAlignment);

        math.Reset();
        math.Translate((float)morg.X, (float)morg.Y);
        if (_rotation != 0)
        {
          math.Rotate((float)-_rotation);
        }
        math.Translate((float)(mrect.X - morg.X + emSize * _xOffset), (float)(mrect.Y - morg.Y + emSize * _yOffset));

        System.Drawing.Drawing2D.GraphicsState gs = g.Save();
        g.MultiplyTransform(math);

        if (_backgroundStyle is not null)
          _backgroundStyle.Draw(g, new RectangleD2D(PointD2D.Empty, msize));

        var envbrush = new BrushXEnv(_brush, new RectangleD2D(PointD2D.Empty, msize), BrushCacheGdi.GetEffectiveMaximumResolution(g, 1));
        labels[i].Draw(g, envbrush, new PointF(0, 0));
        g.Restore(gs); // Restore the graphics state

        helperPath.Reset();
        helperPath.AddRectangle(new RectangleF(PointF.Empty, (SizeF)msize));
        helperPath.Transform(math);

        _enclosingPath.AddPath(helperPath, true);
      }
    }

    #region IRoutedPropertyReceiver Members

    public IEnumerable<(string PropertyName, object PropertyValue, Action<object> PropertySetter)> GetRoutedProperties(string propertyName)
    {
      switch (propertyName)
      {
        case "FontSize":
          yield return (propertyName, _font.Size, (value) => Font = _font.WithSize((double)value));
          break;

        case "FontFamily":
          yield return (propertyName, _font.FontFamilyName, (value) => Font = _font.WithFamily((string)value));
          break;
      }

      yield break;
    }

    #endregion IRoutedPropertyReceiver Members
  }
}
