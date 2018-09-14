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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Calc;
using Altaxo.Data;

namespace Altaxo.Graph.Scales.Ticks
{
  /// <summary>
  /// Tick settings for a linear scale.
  /// </summary>
  public class LinearTickSpacing : NumericTickSpacing
  {
    private static readonly double[] _majorSpanValues = new double[] { 1, 1.5, 2, 2.5, 3, 4, 5, 10 };
    private static readonly double[] _minorSpanValues = new double[] { 1, 1.5, 2, 2.5, 3, 4, 5, 10 };

    /// <summary>Maximum allowed number of ticks in case manual tick input will produce a big amount of ticks.</summary>
    protected static readonly int _maxSafeNumberOfTicks = 10000;

    /// <summary>If set, gives the number of minor ticks choosen by the user.</summary>
    private int? _userDefinedMinorTicks;

    /// <summary>If set, gives the physical value between two major ticks choosen by the user.</summary>
    private double? _userDefinedMajorSpan;

    private double _zeroLever = 0.25;
    private double _orgGrace = 1 / 16.0;
    private double _endGrace = 1 / 16.0;

    private int _targetNumberOfMajorTicks = 4;
    private int _targetNumberOfMinorTicks = 2;

    private double _transformationDivider = 1;
    private bool _transformationOperationIsMultiply;
    private double _transformationOffset = 0;

    /// <summary>If true, the boundaries will be set on a minor or major tick.</summary>
    private BoundaryTickSnapping _snapOrgToTick;

    private BoundaryTickSnapping _snapEndToTick;

    private SuppressedTicks _suppressedMajorTicks;
    private SuppressedTicks _suppressedMinorTicks;
    private AdditionalTicks _additionalMajorTicks;
    private AdditionalTicks _additionalMinorTicks;

    // Results
    private List<double> _majorTicks;

    private List<double> _minorTicks;

    // Cached values
    private class CachedMajorMinor : ICloneable
    {
      public double Org, End;
      /// <summary>Physical span value between two major ticks.</summary>

      public double MajorSpan;

      /// <summary>Minor ticks per Major tick ( if there is one minor tick between two major ticks m_minorticks is 2!</summary>

      public int MinorTicks;

      /// <summary>Current axis origin divided by the major tick span value.</summary>
      public double AxisOrgByMajor;

      /// <summary>Current axis end divided by the major tick span value.</summary>
      public double AxisEndByMajor;

      public CachedMajorMinor(double org, double end, double major, int minor)
      {
        Org = org;
        End = end;
        MajorSpan = major;
        MinorTicks = minor;
      }

      public object Clone()
      {
        return MemberwiseClone();
      }
    }

    private CachedMajorMinor _cachedMajorMinor;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LinearTickSpacing), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (LinearTickSpacing)obj;

        info.AddValue("ZeroLever", s._zeroLever);
        info.AddValue("MinGrace", s._orgGrace);
        info.AddValue("MaxGrace", s._endGrace);
        info.AddEnum("SnapOrgToTick", s._snapOrgToTick);
        info.AddEnum("SnapEndToTick", s._snapEndToTick);

        info.AddValue("TargetNumberOfMajorTicks", s._targetNumberOfMajorTicks);
        info.AddValue("TargetNumberOfMinorTicks", s._targetNumberOfMinorTicks);
        info.AddValue("UserDefinedMajorSpan", s._userDefinedMajorSpan);
        info.AddValue("UserDefinedMinorTicks", s._userDefinedMinorTicks);

        info.AddValue("TransformationOffset", s._transformationOffset);
        info.AddValue("TransformationDivider", s._transformationDivider);
        info.AddValue("TransformationIsMultiply", s._transformationOperationIsMultiply);

        if (s._suppressedMajorTicks.IsEmpty)
          info.AddValue("SuppressedMajorTicks", (object)null);
        else
          info.AddValue("SuppressedMajorTicks", s._suppressedMajorTicks);

        if (s._suppressedMinorTicks.IsEmpty)
          info.AddValue("SuppressedMinorTicks", (object)null);
        else
          info.AddValue("SuppressedMinorTicks", s._suppressedMinorTicks);

        if (s._additionalMajorTicks.IsEmpty)
          info.AddValue("AdditionalMajorTicks", (object)null);
        else
          info.AddValue("AdditionalMajorTicks", s._additionalMajorTicks);

        if (s._additionalMinorTicks.IsEmpty)
          info.AddValue("AdditionalMinorTicks", (object)null);
        else
          info.AddValue("AdditionalMinorTicks", s._additionalMinorTicks);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        LinearTickSpacing s = SDeserialize(o, info, parent);
        return s;
      }

      protected virtual LinearTickSpacing SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        LinearTickSpacing s = null != o ? (LinearTickSpacing)o : new LinearTickSpacing();
        s._zeroLever = info.GetDouble("ZeroLever");
        s._orgGrace = info.GetDouble("MinGrace");
        s._endGrace = info.GetDouble("MaxGrace");
        s._snapOrgToTick = (BoundaryTickSnapping)info.GetEnum("SnapOrgToTick", typeof(BoundaryTickSnapping));
        s._snapEndToTick = (BoundaryTickSnapping)info.GetEnum("SnapEndToTick", typeof(BoundaryTickSnapping));

        s._targetNumberOfMajorTicks = info.GetInt32("TargetNumberOfMajorTicks");
        s._targetNumberOfMinorTicks = info.GetInt32("TargetNumberOfMinorTicks");
        s._userDefinedMajorSpan = info.GetNullableDouble("UserDefinedMajorSpan");
        s._userDefinedMinorTicks = info.GetNullableInt32("UserDefinedMinorTicks");

        s._transformationOffset = info.GetDouble("TransformationOffset");
        s._transformationDivider = info.GetDouble("TransformationDivider");
        s._transformationOperationIsMultiply = info.GetBoolean("TransformationIsMultiply");

        s.SuppressedMajorTicks = (SuppressedTicks)info.GetValue("SuppressedMajorTicks", s);
        s.SuppressedMinorTicks = (SuppressedTicks)info.GetValue("SuppressedMinorTicks", s);
        s.AdditionalMajorTicks = (AdditionalTicks)info.GetValue("AdditionalMajorTicks", s);
        s.AdditionalMinorTicks = (AdditionalTicks)info.GetValue("AdditionalMinorTicks", s);

        return s;
      }
    }

    #endregion Serialization

    public LinearTickSpacing()
    {
      _majorTicks = new List<double>();
      _minorTicks = new List<double>();
      _suppressedMajorTicks = new SuppressedTicks() { ParentObject = this };
      _suppressedMinorTicks = new SuppressedTicks() { ParentObject = this };
      _additionalMajorTicks = new AdditionalTicks() { ParentObject = this };
      _additionalMinorTicks = new AdditionalTicks() { ParentObject = this };
    }

    public LinearTickSpacing(LinearTickSpacing from)
      : base(from) // everything is done here, since CopyFrom is virtual!
    {
    }

    public override bool CopyFrom(object obj)
    {
      if (object.ReferenceEquals(this, obj))
        return true;

      var from = obj as LinearTickSpacing;
      if (null == from)
        return false;

      using (var suspendToken = SuspendGetToken())
      {
        CopyHelper.Copy(ref _cachedMajorMinor, from._cachedMajorMinor);

        _userDefinedMajorSpan = from._userDefinedMajorSpan;
        _userDefinedMinorTicks = from._userDefinedMinorTicks;

        _targetNumberOfMajorTicks = from._targetNumberOfMajorTicks;
        _targetNumberOfMinorTicks = from._targetNumberOfMinorTicks;

        _zeroLever = from._zeroLever;
        _orgGrace = from._orgGrace;
        _endGrace = from._endGrace;

        _snapOrgToTick = from._snapOrgToTick;
        _snapEndToTick = from._snapEndToTick;

        ChildCopyToMember(ref _suppressedMajorTicks, from._suppressedMajorTicks);
        ChildCopyToMember(ref _suppressedMinorTicks, from._suppressedMinorTicks);
        ChildCopyToMember(ref _additionalMajorTicks, from._additionalMajorTicks);
        ChildCopyToMember(ref _additionalMinorTicks, from._additionalMinorTicks);

        _transformationOffset = from._transformationOffset;
        _transformationDivider = from._transformationDivider;
        _transformationOperationIsMultiply = from._transformationOperationIsMultiply;

        _majorTicks = new List<double>(from._majorTicks);
        _minorTicks = new List<double>(from._minorTicks);

        EhSelfChanged();
        suspendToken.Resume();
      }

      return true;
    }

    protected override System.Collections.Generic.IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (null != _suppressedMajorTicks)
        yield return new Main.DocumentNodeAndName(_suppressedMajorTicks, "SuppressedMajorTicks");
      if (null != _suppressedMinorTicks)
        yield return new Main.DocumentNodeAndName(_suppressedMinorTicks, "SuppressedMinorTicks");

      if (null != _additionalMajorTicks)
        yield return new Main.DocumentNodeAndName(_additionalMajorTicks, "AdditionalMajorTicks");
      if (null != _additionalMinorTicks)
        yield return new Main.DocumentNodeAndName(_additionalMinorTicks, "AdditionalMinorTicks");
    }

    public override object Clone()
    {
      return new LinearTickSpacing(this);
    }

    public override bool Equals(object obj)
    {
      if (object.ReferenceEquals(this, obj))
        return true;
      else if (!(obj is LinearTickSpacing))
        return false;
      else
      {
        var from = (LinearTickSpacing)obj;

        if (_userDefinedMajorSpan != from._userDefinedMajorSpan)
          return false;
        if (_userDefinedMinorTicks != from._userDefinedMinorTicks)
          return false;

        if (_targetNumberOfMajorTicks != from._targetNumberOfMajorTicks)
          return false;
        if (_targetNumberOfMinorTicks != from._targetNumberOfMinorTicks)
          return false;

        if (_zeroLever != from._zeroLever)
          return false;
        if (_orgGrace != from._orgGrace)
          return false;
        if (_endGrace != from._endGrace)
          return false;

        if (_snapOrgToTick != from._snapOrgToTick)
          return false;
        if (_snapEndToTick != from._snapEndToTick)
          return false;

        if (_transformationOffset != from._transformationOffset)
          return false;
        if (_transformationDivider != from._transformationDivider)
          return false;
        if (_transformationOperationIsMultiply != from._transformationOperationIsMultiply)
          return false;

        if (!_suppressedMajorTicks.Equals(from._suppressedMajorTicks))
          return false;

        if (!_suppressedMinorTicks.Equals(from._suppressedMinorTicks))
          return false;

        if (!_additionalMajorTicks.Equals(from._additionalMajorTicks))
          return false;

        if (!_additionalMinorTicks.Equals(from._additionalMinorTicks))
          return false;
      }

      return true;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode() + 13 * _targetNumberOfMajorTicks + 31 * _targetNumberOfMinorTicks;
    }

    #region User parameters

    /// <summary>
    /// Gets or sets the zero lever. This is a value (0..1) relative to the span of the scale, that designates how far
    /// the origin or the end of the scale could be extended (in Auto rescale mode) in order to include the scale value = 0.
    /// </summary>
    /// <value>
    /// The zero lever.
    /// </value>
    public double ZeroLever
    {
      get
      {
        return _zeroLever;
      }
      set
      {
        var oldValue = _zeroLever;
        _zeroLever = value;
        if (oldValue != value)
          EhSelfChanged();
      }
    }

    /// <summary>
    /// Gets or sets the origin grace. This is a value (0..1) relative to the span of the scale, that designates how far
    /// the origin of the scale is extended in Auto rescale mode.
    /// </summary>
    /// <value>
    /// The origin grace.
    /// </value>
    public double OrgGrace
    {
      get
      {
        return _orgGrace;
      }
      set
      {
        var oldValue = _orgGrace;
        _orgGrace = value;
        if (oldValue != value)
          EhSelfChanged();
      }
    }

    /// <summary>
    /// Gets or sets the end grace. This is a value (0..1) relative to the span of the scale, that designates how far
    /// the end of the scale is extended in Auto rescale mode.
    /// </summary>
    /// <value>
    /// The origin grace.
    /// </value>
    public double EndGrace
    {
      get
      {
        return _endGrace;
      }
      set
      {
        var oldValue = _endGrace;
        _endGrace = value;
        if (oldValue != value)
          EhSelfChanged();
      }
    }

    public BoundaryTickSnapping SnapOrgToTick
    {
      get
      {
        return _snapOrgToTick;
      }
      set
      {
        var oldValue = _snapOrgToTick;
        _snapOrgToTick = value;
        if (oldValue != value)
          EhSelfChanged();
      }
    }

    public BoundaryTickSnapping SnapEndToTick
    {
      get
      {
        return _snapEndToTick;
      }
      set
      {
        var oldValue = _snapEndToTick;
        _snapEndToTick = value;
        if (oldValue != value)
          EhSelfChanged();
      }
    }

    public int TargetNumberOfMajorTicks
    {
      get
      {
        return _targetNumberOfMajorTicks;
      }
      set
      {
        var oldValue = _targetNumberOfMajorTicks;
        _targetNumberOfMajorTicks = value;
        if (oldValue != value)
          EhSelfChanged();
      }
    }

    public int TargetNumberOfMinorTicks
    {
      get
      {
        return _targetNumberOfMinorTicks;
      }
      set
      {
        var oldValue = _targetNumberOfMinorTicks;
        _targetNumberOfMinorTicks = value;
        if (oldValue != value)
          EhSelfChanged();
      }
    }

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

    public double TransformationOffset
    {
      get
      {
        return _transformationOffset;
      }
      set
      {
        var oldValue = _transformationOffset;
        _transformationOffset = value;
        if (oldValue != value)
          EhSelfChanged();
      }
    }

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

    public int? MinorTicks
    {
      get
      {
        return _userDefinedMinorTicks;
      }
      set
      {
        var oldValue = _userDefinedMinorTicks;
        _userDefinedMinorTicks = value;
        if (oldValue != value)
          EhSelfChanged();
      }
    }

    public double? MajorTickSpan
    {
      get
      {
        return _userDefinedMajorSpan;
      }
      set
      {
        var oldValue = _userDefinedMajorSpan;
        _userDefinedMajorSpan = value;
        if (oldValue != value)
          EhSelfChanged();
      }
    }

    public SuppressedTicks SuppressedMajorTicks
    {
      get
      {
        return _suppressedMajorTicks;
      }
      protected set
      {
        ChildSetMember(ref _suppressedMajorTicks, value ?? new SuppressedTicks());
      }
    }

    public SuppressedTicks SuppressedMinorTicks
    {
      get
      {
        return _suppressedMinorTicks;
      }
      protected set
      {
        ChildSetMember(ref _suppressedMinorTicks, value ?? new SuppressedTicks());
      }
    }

    public AdditionalTicks AdditionalMajorTicks
    {
      get
      {
        return _additionalMajorTicks;
      }
      protected set
      {
        ChildSetMember(ref _additionalMajorTicks, value ?? new AdditionalTicks());
      }
    }

    public AdditionalTicks AdditionalMinorTicks
    {
      get
      {
        return _additionalMinorTicks;
      }
      protected set
      {
        ChildSetMember(ref _additionalMinorTicks, value ?? new AdditionalTicks());
      }
    }

    #endregion User parameters

    private double TransformOriginalToModified(double x)
    {
      if (_transformationOperationIsMultiply)
        return _transformationOffset + x * _transformationDivider;
      else
        return _transformationOffset + x / _transformationDivider;
    }

    private double TransformModifiedToOriginal(double y)
    {
      if (_transformationOperationIsMultiply)
        return (y - _transformationOffset) / _transformationDivider;
      else
        return (y - _transformationOffset) * _transformationDivider;
    }

    /// <summary>
    /// GetMajorTicks returns the physical values
    /// at which major ticks should occur
    /// </summary>
    /// <returns>physical values for the major ticks</returns>
    public override double[] GetMajorTicks()
    {
      return _majorTicks.ToArray();
    }

    /// <summary>
    /// GetMinorTicks returns the physical values
    /// at which minor ticks should occur
    /// </summary>
    /// <returns>physical values for the minor ticks</returns>
    public override double[] GetMinorTicks()
    {
      return _minorTicks.ToArray();
    }

    public override double[] GetMajorTicksNormal(Scale scale)
    {
      double[] r = new double[_majorTicks.Count];
      for (int i = 0; i < r.Length; i++)
        r[i] = scale.PhysicalVariantToNormal(TransformModifiedToOriginal(_majorTicks[i]));

      return r;
    }

    public override double[] GetMinorTicksNormal(Scale scale)
    {
      double[] r = new double[_minorTicks.Count];
      for (int i = 0; i < r.Length; i++)
        r[i] = scale.PhysicalVariantToNormal(TransformModifiedToOriginal(_minorTicks[i]));

      return r;
    }

    /// <summary>
    /// Decides giving a raw org and end value, whether or not the scale boundaries should be extended to
    /// have more 'nice' values. If the boundaries should be changed, the function return true, and the
    /// org and end argument contain the proposed new scale boundaries.
    /// </summary>
    /// <param name="org">Raw scale org.</param>
    /// <param name="end">Raw scale end.</param>
    /// <param name="isOrgExtendable">True when the org is allowed to be extended.</param>
    /// <param name="isEndExtendable">True when the scale end can be extended.</param>
    /// <returns>True when org or end are changed. False otherwise.</returns>
    public override bool PreProcessScaleBoundaries(ref AltaxoVariant org, ref AltaxoVariant end, bool isOrgExtendable, bool isEndExtendable)
    {
      double dorg = org;
      double dend = end;

      dorg = TransformOriginalToModified(dorg);
      dend = TransformOriginalToModified(dend);

      if (InternalPreProcessScaleBoundaries(ref dorg, ref dend, isOrgExtendable, isEndExtendable))
      {
        org = TransformModifiedToOriginal(dorg);
        end = TransformModifiedToOriginal(dend);
        return true;
      }
      else
        return false;
    }

    /// <summary>
    /// Calculates the ticks based on the org and end of the scale.
    /// </summary>
    /// <param name="org">Scale origin.</param>
    /// <param name="end">Scale end.</param>
    /// <param name="scale">The underlying scale.</param>
    public override void FinalProcessScaleBoundaries(AltaxoVariant org, AltaxoVariant end, Scale scale)
    {
      double dorg = org;
      double dend = end;

      dorg = TransformOriginalToModified(dorg);
      dend = TransformOriginalToModified(dend);

      if (_cachedMajorMinor == null || _cachedMajorMinor.Org != dorg || _cachedMajorMinor.End != dend)
      {
        InternalPreProcessScaleBoundaries(ref dorg, ref dend, false, false); // make sure that _cachedMajorMinor is valid now
      }

      if (!(null != _cachedMajorMinor))
        throw new InvalidProgramException();

      double majorSpan = _cachedMajorMinor.MajorSpan;
      double axisOrgByMajor = dorg / majorSpan;
      double axisEndByMajor = dend / majorSpan;

      // supress rounding errors
      double spanByMajor = Math.Abs((dend - dorg) / majorSpan);
      if (axisOrgByMajor - Math.Floor(axisOrgByMajor) < 1e-7 * spanByMajor)
        axisOrgByMajor = Math.Floor(axisOrgByMajor);
      if (Math.Ceiling(axisEndByMajor) - axisEndByMajor < 1e-7 * spanByMajor)
        axisEndByMajor = Math.Ceiling(axisEndByMajor);

      _cachedMajorMinor.AxisOrgByMajor = axisOrgByMajor;
      _cachedMajorMinor.AxisEndByMajor = axisEndByMajor;

      _majorTicks.Clear();
      _minorTicks.Clear();
      InternalCalculateMajorTicks();
      InternalCalculateMinorTicks();
    }

    #region Calculation of tick values

    private void InternalCalculateMajorTicks()
    {
      _majorTicks.Clear();

      var axisOrgByMajor = _cachedMajorMinor.AxisOrgByMajor;
      var axisEndByMajor = _cachedMajorMinor.AxisEndByMajor;
      var majorSpan = _cachedMajorMinor.MajorSpan;

      double beg = System.Math.Ceiling(axisOrgByMajor);
      double end = System.Math.Floor(axisEndByMajor);
      double llen = end - beg;
      // limit the length to 10000 to limit the amount of space required
      llen = Math.Max(0, Math.Min(llen, _maxSafeNumberOfTicks));
      int len = (int)llen;

      for (int i = 0; i <= len; i++)
      {
        double v = (i + beg) * majorSpan;
        _majorTicks.Add(v);
      }

      // Remove suppressed ticks
      _suppressedMajorTicks.RemoveSuppressedTicks(_majorTicks);

      if (!_additionalMajorTicks.IsEmpty)
      {
        foreach (AltaxoVariant v in _additionalMajorTicks.Values)
        {
          _majorTicks.Add(v);
        }
      }
    }

    private void InternalCalculateMinorTicks()
    {
      _minorTicks.Clear();

      var axisOrgByMajor = _cachedMajorMinor.AxisOrgByMajor;
      var axisEndByMajor = _cachedMajorMinor.AxisEndByMajor;
      var majorSpan = _cachedMajorMinor.MajorSpan;
      var numberOfMinorTicks = _cachedMajorMinor.MinorTicks;

      if (numberOfMinorTicks < 2)
        return; // below 2 there are no minor ticks per definition

      double beg = System.Math.Ceiling(axisOrgByMajor);
      double end = System.Math.Floor(axisEndByMajor);
      int majorticks = 1 + (int)(end - beg);
      beg = System.Math.Ceiling(axisOrgByMajor * numberOfMinorTicks);
      end = System.Math.Floor(axisEndByMajor * numberOfMinorTicks);
      double llen = end - beg;
      // limit the length to 10000 to limit the amount of space and time required
      llen = Math.Max(0, Math.Min(llen, _maxSafeNumberOfTicks));
      int len = (int)llen;

      int shift = (int)(beg % numberOfMinorTicks);

      for (int i = 0; i <= len; i++)
      {
        if ((i + shift) % numberOfMinorTicks == 0)
          continue;

        double v = (i + beg) * majorSpan / numberOfMinorTicks;
        _minorTicks.Add(v);
      }

      // Remove suppressed ticks
      _suppressedMinorTicks.RemoveSuppressedTicks(_minorTicks);

      if (!_additionalMinorTicks.IsEmpty)
      {
        foreach (AltaxoVariant v in _additionalMinorTicks.Values)
        {
          _minorTicks.Add(v);
        }
      }
    }

    #endregion Calculation of tick values

    #region Functions to predict change of scale by tick snapping, grace, and OneLever

    private bool InternalPreProcessScaleBoundaries(ref double xorg, ref double xend, bool isOrgExtendable, bool isEndExtendable)
    {
      _cachedMajorMinor = null;
      bool modified = false;

      if (xend == xorg)
      {
        if (xend == 0)
        {
          xorg = -1;
          xend = 1;
        }
        else
        {
          xend += 0.25 * Math.Abs(xend);
          xorg -= 0.25 * Math.Abs(xend);
        }
        modified = true;
      }

      if (!(xorg > double.MinValue && xorg < double.MaxValue))
      {
        xorg = double.MinValue;
        modified = true;
      }
      if (!(xend > double.MinValue && xend < double.MaxValue))
      {
        xend = double.MaxValue;
        modified = true;
      }
      if (!(xorg <= xend))
      {
        var h = xorg;
        xorg = xend;
        xend = h;
        modified = true;
      }

      bool modGraceAndOneLever = GetOrgEndWithGraceAndZeroLever(xorg, xend, isOrgExtendable, isEndExtendable, out var xOrgWithGraceAndOneLever, out var xEndWithGraceAndOneLever);
      bool modTickSnapping = GetOrgEndWithTickSnappingOnly(xend - xorg, xorg, xend, isOrgExtendable, isEndExtendable, out var xOrgWithTickSnapping, out var xEndWithTickSnapping, out var majorTickSpan, out var minorTicks);

      // now compare the two
      if (xOrgWithTickSnapping <= xOrgWithGraceAndOneLever && xEndWithTickSnapping >= xEndWithGraceAndOneLever)
      {
        // then there is no need to apply Grace and OneLever
        modified |= modTickSnapping;
      }
      else
      {
        modified |= modGraceAndOneLever;
        modified |= GetOrgEndWithTickSnappingOnly(xEndWithGraceAndOneLever - xOrgWithGraceAndOneLever, xOrgWithGraceAndOneLever, xEndWithGraceAndOneLever, isOrgExtendable, isEndExtendable, out xOrgWithTickSnapping, out xEndWithTickSnapping, out majorTickSpan, out minorTicks);
      }

      xorg = xOrgWithTickSnapping;
      xend = xEndWithTickSnapping;

      _cachedMajorMinor = new CachedMajorMinor(xOrgWithTickSnapping, xEndWithTickSnapping, majorTickSpan, minorTicks);

      return modified;
      ;
    }

    /// <summary>
    /// Applies the value for <see cref="OrgGrace"/>, <see cref="EndGrace"/> and <see cref="ZeroLever"/> to the scale and calculated proposed values for the boundaries.
    /// </summary>
    /// <param name="scaleOrg">Scale origin.</param>
    /// <param name="scaleEnd">Scale end.</param>
    /// <param name="isOrgExtendable">True if the scale org can be extended.</param>
    /// <param name="isEndExtendable">True if the scale end can be extended.</param>
    /// <param name="propOrg">Returns the proposed value of the scale origin.</param>
    /// <param name="propEnd">Returns the proposed value of the scale end.</param>
    public bool GetOrgEndWithGraceAndZeroLever(double scaleOrg, double scaleEnd, bool isOrgExtendable, bool isEndExtendable, out double propOrg, out double propEnd)
    {
      bool modified = false;
      double scaleSpan = Math.Abs(scaleEnd - scaleOrg);
      propOrg = scaleOrg;
      if (isOrgExtendable)
      {
        propOrg -= Math.Abs(_orgGrace * scaleSpan);
        modified |= (0 != _orgGrace);
      }

      propEnd = scaleEnd;
      if (isEndExtendable)
      {
        propEnd += Math.Abs(_endGrace * scaleSpan);
        modified |= (0 != _endGrace);
      }

      double lever = Math.Abs(_zeroLever * scaleSpan);
      double propOrg2 = scaleOrg - lever;
      double propEnd2 = scaleEnd + lever;

      if (isOrgExtendable && propOrg > 0 && propOrg2 <= 0)
      {
        propOrg = 0;
        modified = true;
      }

      if (isEndExtendable && propEnd < 0 && propEnd2 >= 0)
      {
        propEnd = 0;
        modified = true;
      }

      double range = propEnd - propOrg;
      if (range == 0) // Emergency plan if range is zero
      {
        double extend = propOrg == 0 ? 0.5 : Math.Abs(propOrg / 10);
        if (isOrgExtendable && isEndExtendable)
        {
          propOrg -= extend;
          propEnd += extend;
          modified = true;
        }
        else if (isOrgExtendable)
        {
          propOrg -= 2 * extend;
          modified = true;
        }
        else if (isEndExtendable)
        {
          propEnd += 2 * extend;
          modified = true;
        }
      }
      return modified;
    }

    /// <summary>Applies the tick snapping settings to the scale origin and scale end. This is done by a determination of the number of decades per major tick and the minor ticks per major tick interval.
    /// Then, the snapping values are applied, and the org and end values of the scale are adjusted (if allowed so).</summary>
    /// <param name="overriddenScaleSpan">The overriden scale span.</param>
    /// <param name="scaleOrg">The scale origin.</param>
    /// <param name="scaleEnd">The scale end.</param>
    /// <param name="isOrgExtendable">If set to <c>true</c>, it is allowed to adjust the scale org value.</param>
    /// <param name="isEndExtendable">if set to <c>true</c>, it is allowed to adjust the scale end value.</param>
    /// <param name="propOrg">The adjusted scale orgin.</param>
    /// <param name="propEnd">The adjusted scale end.</param>
    /// <param name="majorSpan">The physical value that corresponds to one major tick interval.</param>
    /// <param name="minorTicks">Number of minor ticks per major tick interval. This variable has some special values (see <see cref="MinorTicks"/>).</param>
    /// <returns>True if at least either org or end were adjusted to a new value.</returns>
    private bool GetOrgEndWithTickSnappingOnly(double overriddenScaleSpan, double scaleOrg, double scaleEnd, bool isOrgExtendable, bool isEndExtendable, out double propOrg, out double propEnd, out double majorSpan, out int minorTicks)
    {
      bool modified = false;
      propOrg = scaleOrg;
      propEnd = scaleEnd;

      if (null != _userDefinedMajorSpan)
      {
        majorSpan = Math.Abs(_userDefinedMajorSpan.Value);
      }
      else
      {
        majorSpan = CalculateMajorSpan(overriddenScaleSpan, _targetNumberOfMajorTicks);
      }

      if (null != _userDefinedMinorTicks)
      {
        minorTicks = Math.Abs(_userDefinedMinorTicks.Value);
      }
      else
      {
        minorTicks = CalculateNumberOfMinorTicks(majorSpan, _targetNumberOfMinorTicks);
      }

      if (isOrgExtendable)
      {
        propOrg = GetOrgOrEndSnappedToTick(scaleOrg, majorSpan, minorTicks, _snapOrgToTick, false);
        modified |= BoundaryTickSnapping.SnapToNothing != _snapOrgToTick;
      }

      if (isEndExtendable)
      {
        propEnd = GetOrgOrEndSnappedToTick(scaleEnd, majorSpan, minorTicks, _snapEndToTick, true);
        modified |= BoundaryTickSnapping.SnapToNothing != _snapEndToTick;
      }

      return modified;
    }

    /// <summary>
    /// Adjusts the parameter <paramref name="x"/> so that <paramref name="x"/> snaps to a tick according to the setting of <paramref name="snapping"/>.
    /// </summary>
    /// <param name="x">The boundary value to adjust.</param>
    /// <param name="majorSpan">Value of the major tick span.</param>
    /// <param name="minorTicks">Number of minor ticks.</param>
    /// <param name="snapping">Setting of the tick snapping.</param>
    /// <param name="upwards">If true, the value is towards higher values, if false it is adjusted towards smaller values.</param>
    /// <returns>The adjusted value of x.</returns>
    public static double GetOrgOrEndSnappedToTick(double x, double majorSpan, int minorTicks, BoundaryTickSnapping snapping, bool upwards)
    {
      switch (snapping)
      {
        default:
        case BoundaryTickSnapping.SnapToNothing:
          {
            return x;
          }
        case BoundaryTickSnapping.SnapToMajorOnly:
          {
            double rel = x / majorSpan;
            if (upwards)
              return Math.Ceiling(rel) * majorSpan;
            else
              return Math.Floor(rel) * majorSpan;
          }
        case BoundaryTickSnapping.SnapToMinorOnly:
          {
            double rel = x * minorTicks / (majorSpan);
            if (upwards)
              rel = Math.Ceiling(rel);
            else
              rel = Math.Floor(rel);

            if (Math.IEEERemainder(rel, 1) != 0 && minorTicks > 1)
              rel = upwards ? rel + 1 : rel - 1;

            return rel * majorSpan / minorTicks;
          }
        case BoundaryTickSnapping.SnapToMinorOrMajor:
          {
            double rel = x * minorTicks / (majorSpan);
            if (upwards)
              return Math.Ceiling(rel) * majorSpan / minorTicks;
            else
              return Math.Floor(rel) * majorSpan / minorTicks;
          }
      }
    }

    /// <summary>
    /// Calculates the major span from the scale span, taking into account the setting for targetMajorTicks.
    /// </summary>
    /// <param name="scaleSpan">Scale span (end-origin).</param>
    /// <param name="targetNumberOfMajorTicks">Target number of major ticks.</param>
    public static double CalculateMajorSpan(double scaleSpan, int targetNumberOfMajorTicks)
    {
      if (!(scaleSpan > 0))
        throw new ArgumentOutOfRangeException("scaleSpan must be >0");

      double rawMajorSpan = targetNumberOfMajorTicks >= 1 ? scaleSpan / targetNumberOfMajorTicks : scaleSpan;
      int log10RawMajorSpan = (int)Math.Floor(Math.Log10(rawMajorSpan));

      double normMajorSpan = rawMajorSpan / RMath.Pow(10, log10RawMajorSpan); // number between 1 and 10
      foreach (double span in _majorSpanValues)
      {
        if (span >= normMajorSpan)
        {
          normMajorSpan = span;
          break;
        }
      }
      return normMajorSpan * RMath.Pow(10, log10RawMajorSpan);
    }

    /// <summary>
    /// Calculates the number of minor ticks from the major span value and the target number of minor ticks.
    /// </summary>
    /// <param name="majorSpan">Major span value.</param>
    /// <param name="targetNumberOfMinorTicks">Target number of minor ticks.</param>
    /// <returns></returns>
    public static int CalculateNumberOfMinorTicks(double majorSpan, int targetNumberOfMinorTicks)
    {
      if (targetNumberOfMinorTicks <= 0)
        return 1;
      majorSpan = Math.Abs(majorSpan);
      if (!majorSpan.IsFinite())
        return 1;

      double rawMinorSpan = majorSpan / targetNumberOfMinorTicks;
      int log10RawMinorSpan = (int)Math.Floor(Math.Log10(rawMinorSpan));

      double normMinorSpan = rawMinorSpan / RMath.Pow(10, log10RawMinorSpan); // number between 1 and 10
      for (int i = _minorSpanValues.Length - 1; i >= 0; i--)
      {
        double span = _minorSpanValues[i];
        if (span <= normMinorSpan)
        {
          normMinorSpan = span;
          break;
        }
      }
      double minorSpan = normMinorSpan * RMath.Pow(10, log10RawMinorSpan);
      int result = (int)Math.Round(majorSpan / minorSpan);
      return result < 1 ? 1 : result;
    }

    #endregion Functions to predict change of scale by tick snapping, grace, and OneLever
  }
}
