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
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Scales.Ticks
{
  /// <summary>
  /// Represents tick spacing that shows the span between origin and end.
  /// </summary>
  [DisplayName("${res:ClassNames.Altaxo.Graph.Scales.Ticks.SpanTickSpacing}")]
  public class SpanTickSpacing : Altaxo.Graph.Scales.Ticks.TickSpacing
  {
    /// <summary>Relative tick position (0 at org, 1 at end).</summary>
    private double _relTickPosition;

    /// <summary>If true, it shows the ratio of end to org. If false, it shows the difference of end and org.</summary>
    private bool _showRatioEndOrg;

    private double _transformationDivider = 1;
    private bool _transformationOperationIsMultiply;

    private Altaxo.Data.AltaxoVariant _org;
    private Altaxo.Data.AltaxoVariant _end;
    private Data.AltaxoVariant _span;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SpanTickSpacing), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc />
      public virtual void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SpanTickSpacing)o;
        info.AddValue("ShowRatioEndOrg", s._showRatioEndOrg);
        info.AddValue("RelTickPosition", s._relTickPosition);

        info.AddValue("TransformationDivider", s._transformationDivider);
        info.AddValue("TransformationIsMultiply", s._transformationOperationIsMultiply);
      }

      /// <inheritdoc />
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (SpanTickSpacing?)o ?? new SpanTickSpacing();

        s._showRatioEndOrg = info.GetBoolean("ShowRatioEndOrg");
        s._relTickPosition = info.GetDouble("RelTickPosition");

        s._transformationDivider = info.GetDouble("TransformationDivider");
        s._transformationOperationIsMultiply = info.GetBoolean("TransformationIsMultiply");

        return s;
      }

      
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="SpanTickSpacing"/> class.
    /// </summary>
    public SpanTickSpacing()
    {
      _relTickPosition = 0.5;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpanTickSpacing"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The instance to copy.</param>
    public SpanTickSpacing(SpanTickSpacing from)
      : base(from) // everything is done here, since CopyFrom is virtual!
    {
    }

    /// <inheritdoc />
    public override bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      var from = obj as SpanTickSpacing;
      if (from is null)
        return false;

      using (var suspendToken = SuspendGetToken())
      {
        _relTickPosition = from._relTickPosition;
        _showRatioEndOrg = from._showRatioEndOrg;
        _transformationDivider = from._transformationDivider;
        _transformationOperationIsMultiply = from._transformationOperationIsMultiply;

        EhSelfChanged();
        suspendToken.Resume();
      }
      return true;
    }

    /// <inheritdoc />
    public override object Clone()
    {
      return new SpanTickSpacing(this);
    }

    /// <inheritdoc />
    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      yield break;
    }

    /// <summary>
    /// Gets or sets the relative position of the span tick.
    /// </summary>
    public double RelativeTickPosition
    {
      get
      {
        return _relTickPosition;
      }
      set
      {
        _relTickPosition = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the end-to-origin ratio is shown instead of the difference.
    /// </summary>
    public bool ShowEndOrgRatioInsteadOfDifference
    {
      get
      {
        return _showRatioEndOrg;
      }
      set
      {
        _showRatioEndOrg = value;
      }
    }

    /// <summary>
    /// Gets or sets the divider used for the transformation.
    /// </summary>
    public double TransformationDivider
    {
      get
      {
        return _transformationDivider;
      }
      set
      {
        var oldValue = _transformationDivider;
        _transformationDivider = value;
        if (oldValue != value)
          EhSelfChanged();
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the transformation operation uses multiplication.
    /// </summary>
    public bool TransformationOperationIsMultiply
    {
      get
      {
        return _transformationOperationIsMultiply;
      }
      set
      {
        var oldValue = _transformationOperationIsMultiply;
        _transformationOperationIsMultiply = value;
        if (oldValue != value)
          EhSelfChanged();
      }
    }

    private double TransformOriginalToModified(double x)
    {
      if (_transformationOperationIsMultiply)
        return x * _transformationDivider;
      else
        return x / _transformationDivider;
    }

    private double TransformModifiedToOriginal(double y)
    {
      if (_transformationOperationIsMultiply)
        return y / _transformationDivider;
      else
        return y * _transformationDivider;
    }

    /// <inheritdoc />
    public override bool PreProcessScaleBoundaries(ref Altaxo.Data.AltaxoVariant org, ref Altaxo.Data.AltaxoVariant end, bool isOrgExtendable, bool isEndExtendable)
    {
      return false;
    }

    /// <inheritdoc />
    public override void FinalProcessScaleBoundaries(Altaxo.Data.AltaxoVariant org, Altaxo.Data.AltaxoVariant end, Altaxo.Graph.Scales.Scale scale)
    {
      _org = org;
      _end = end;

      try
      {
        if (_showRatioEndOrg)
          _span = end / org;
        else
          _span = end - org;
      }
      catch (Exception)
      {
        _span = new Data.AltaxoVariant(string.Empty);
      }
    }

    /// <inheritdoc />
    public override double[] GetMajorTicksNormal(Scale scale)
    {
      return new double[] { 0, 1 };
    }

    /// <inheritdoc />
    public override Altaxo.Data.AltaxoVariant[] GetMajorTicksAsVariant()
    {
      if (_transformationDivider == 1)
      {
        return new Altaxo.Data.AltaxoVariant[] { _org, _end };
      }
      else
      {
        try
        {
          if (_transformationOperationIsMultiply)
            return new Altaxo.Data.AltaxoVariant[] { _org * _transformationDivider, _end * _transformationDivider };
          else
            return new Altaxo.Data.AltaxoVariant[] { _org / _transformationDivider, _end / _transformationDivider };
        }
        catch (Exception)
        {
        }
      }
      return new Altaxo.Data.AltaxoVariant[] { new Altaxo.Data.AltaxoVariant(string.Empty), new Altaxo.Data.AltaxoVariant(string.Empty) };
    }

    /// <inheritdoc />
    public override double[] GetMinorTicksNormal(Scale scale)
    {
      return new double[] { _relTickPosition };
    }

    /// <inheritdoc />
    public override Altaxo.Data.AltaxoVariant[] GetMinorTicksAsVariant()
    {
      return new Altaxo.Data.AltaxoVariant[] { TransformOriginalToModified(_span) };
    }
  }
}
