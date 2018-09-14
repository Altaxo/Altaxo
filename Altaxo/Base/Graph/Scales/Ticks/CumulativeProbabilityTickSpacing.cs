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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Calc;
using Altaxo.Data;

namespace Altaxo.Graph.Scales.Ticks
{
  using System.Collections;
  using Altaxo.Graph.Scales.Rescaling;

  /// <summary>
  /// Tick settings for a Probability scale.
  /// </summary>
  public class CumulativeProbabilityTickSpacing : NumericTickSpacing
  {
    #region Inner list classes

    private class ProbabilityList : ICollection<double>
    {
      private double _org, _end;
      private HashSet<double> _coll = new HashSet<double>();

      public int NumberOfEntriesWithinBounds { get; private set; }

      public ProbabilityList(double org, double end)
      {
        _org = org;
        _end = end;
      }

      public void Add(double x)
      {
        if (_coll.Add(x))
        {
          if (_org <= x && x <= _end)
            ++NumberOfEntriesWithinBounds;
        }
      }

      public void AddRange(IEnumerable<double> plist)
      {
        foreach (var p in plist)
          Add(p);
      }

      public IEnumerable<double> ElementsInbetweenOrgAndEnd(double org, double end)
      {
        const double delta = 1.0 / 65536.0;

        // try to avoid rounding errors by modifying org and end
        var orgm = org - delta * (org);
        var endm = end + delta * (1 - end);

        foreach (var p in _coll)
          if (orgm <= p && p <= endm)
            yield return p;
      }

      public int Count
      {
        get
        {
          return _coll.Count;
        }
      }

      public bool IsReadOnly
      {
        get
        {
          return false;
        }
      }

      public void Clear()
      {
        _coll.Clear();
      }

      public bool Contains(double item)
      {
        return _coll.Contains(item);
      }

      public void CopyTo(double[] array, int arrayIndex)
      {
        _coll.CopyTo(array, arrayIndex);
      }

      public bool Remove(double item)
      {
        return _coll.Remove(item);
      }

      public IEnumerator<double> GetEnumerator()
      {
        return _coll.GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
        return _coll.GetEnumerator();
      }
    }

    private class ProbabilityListMajor : ProbabilityList
    {
      public int StartIdxLower { get; set; }

      public int SkipStepLower { get; set; }

      public int StartIdxUpper { get; set; }

      public int SkipStepUpper { get; set; }

      public ProbabilityListMajor(double org, double end)
        : base(org, end)
      { }
    }

    #endregion Inner list classes

    /// <summary>Maximum allowed number of ticks in case manual tick input will produce a big amount of ticks.</summary>
    protected static readonly int _maxSafeNumberOfTicks = 10000;

    private double _orgGrace = 1 / 16.0;
    private double _endGrace = 1 / 16.0;

    private int _targetNumberOfMajorTicks = 5;
    private int _targetNumberOfMinorTicks = 2;

    private double _transformationDivider = 1;
    private bool _transformationOperationIsMultiply = true;

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

      public double TickGeneratingOrg, TickGeneratingEnd;

      public CachedMajorMinor(double org, double end, double tickGeneratingOrg, double tickGeneratingEnd)
      {
        Org = org;
        End = end;

        TickGeneratingOrg = tickGeneratingOrg;
        TickGeneratingEnd = tickGeneratingEnd;
      }

      public object Clone()
      {
        return MemberwiseClone();
      }
    }

    private CachedMajorMinor _cachedMajorMinor;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(CumulativeProbabilityTickSpacing), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (CumulativeProbabilityTickSpacing)obj;

        info.AddValue("MinGrace", s._orgGrace);
        info.AddValue("MaxGrace", s._endGrace);
        info.AddEnum("SnapOrgToTick", s._snapOrgToTick);
        info.AddEnum("SnapEndToTick", s._snapEndToTick);

        info.AddValue("TargetNumberOfMajorTicks", s._targetNumberOfMajorTicks);
        info.AddValue("TargetNumberOfMinorTicks", s._targetNumberOfMinorTicks);

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
        CumulativeProbabilityTickSpacing s = SDeserialize(o, info, parent);
        return s;
      }

      protected virtual CumulativeProbabilityTickSpacing SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        CumulativeProbabilityTickSpacing s = null != o ? (CumulativeProbabilityTickSpacing)o : new CumulativeProbabilityTickSpacing();
        s._orgGrace = info.GetDouble("MinGrace");
        s._endGrace = info.GetDouble("MaxGrace");
        s._snapOrgToTick = (BoundaryTickSnapping)info.GetEnum("SnapOrgToTick", typeof(BoundaryTickSnapping));
        s._snapEndToTick = (BoundaryTickSnapping)info.GetEnum("SnapEndToTick", typeof(BoundaryTickSnapping));

        s._targetNumberOfMajorTicks = info.GetInt32("TargetNumberOfMajorTicks");
        s._targetNumberOfMinorTicks = info.GetInt32("TargetNumberOfMinorTicks");

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

    public CumulativeProbabilityTickSpacing()
    {
      _majorTicks = new List<double>();
      _minorTicks = new List<double>();
      _suppressedMajorTicks = new SuppressedTicks() { ParentObject = this };
      _suppressedMinorTicks = new SuppressedTicks() { ParentObject = this };
      _additionalMajorTicks = new AdditionalTicks() { ParentObject = this };
      _additionalMinorTicks = new AdditionalTicks() { ParentObject = this };
    }

    public CumulativeProbabilityTickSpacing(CumulativeProbabilityTickSpacing from)
      : base(from) // everything is done here, since CopyFrom is virtual!
    {
    }

    public override bool CopyFrom(object obj)
    {
      if (object.ReferenceEquals(this, obj))
        return true;

      var from = obj as CumulativeProbabilityTickSpacing;
      if (null == from)
        return false;

      using (var suspendToken = SuspendGetToken())
      {
        CopyHelper.Copy(ref _cachedMajorMinor, from._cachedMajorMinor);

        _targetNumberOfMajorTicks = from._targetNumberOfMajorTicks;
        _targetNumberOfMinorTicks = from._targetNumberOfMinorTicks;

        _orgGrace = from._orgGrace;
        _endGrace = from._endGrace;

        _snapOrgToTick = from._snapOrgToTick;
        _snapEndToTick = from._snapEndToTick;

        ChildCopyToMember(ref _suppressedMajorTicks, from._suppressedMajorTicks);
        ChildCopyToMember(ref _suppressedMinorTicks, from._suppressedMinorTicks);
        ChildCopyToMember(ref _additionalMajorTicks, from._additionalMajorTicks);
        ChildCopyToMember(ref _additionalMinorTicks, from._additionalMinorTicks);

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
      return new CumulativeProbabilityTickSpacing(this);
    }

    public override bool Equals(object obj)
    {
      if (object.ReferenceEquals(this, obj))
        return true;
      else if (!(obj is CumulativeProbabilityTickSpacing))
        return false;
      else
      {
        var from = (CumulativeProbabilityTickSpacing)obj;

        if (_targetNumberOfMajorTicks != from._targetNumberOfMajorTicks)
          return false;
        if (_targetNumberOfMinorTicks != from._targetNumberOfMinorTicks)
          return false;

        if (_orgGrace != from._orgGrace)
          return false;
        if (_endGrace != from._endGrace)
          return false;

        if (_snapOrgToTick != from._snapOrgToTick)
          return false;
        if (_snapEndToTick != from._snapEndToTick)
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

    public SuppressedTicks SuppressedMajorTicks
    {
      get
      {
        return _suppressedMajorTicks;
      }
      protected set
      {
        _suppressedMajorTicks = value ?? new SuppressedTicks();
        _suppressedMajorTicks.ParentObject = this;
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
        _suppressedMinorTicks = value ?? new SuppressedTicks();
        _suppressedMinorTicks.ParentObject = this;
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
        _additionalMajorTicks = value ?? new AdditionalTicks();
        _additionalMajorTicks.ParentObject = this;
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
        _additionalMinorTicks = value ?? new AdditionalTicks();
        _additionalMinorTicks.ParentObject = this;
      }
    }

    #endregion User parameters

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
        return (y) / _transformationDivider;
      else
        return (y) * _transformationDivider;
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

    private void InternalCalculateMajorTicks(ProbabilityListMajor rawMajorTicks, double org, double end)
    {
      foreach (var p in rawMajorTicks.ElementsInbetweenOrgAndEnd(org, end))
        _majorTicks.Add(TransformOriginalToModified(p));

      // Remove suppressed ticks
      _suppressedMajorTicks.RemoveSuppressedTicks(_majorTicks);

      // Add additional ticks
      if (!_additionalMajorTicks.IsEmpty)
      {
        foreach (AltaxoVariant v in _additionalMajorTicks.Values)
        {
          _majorTicks.Add(v);
        }
      }

      _majorTicks.Sort();
    }

    private void InternalCalculateMinorTicks(ProbabilityList rawMinorTicks, double org, double end)
    {
      _minorTicks.Clear();

      foreach (var p in rawMinorTicks.ElementsInbetweenOrgAndEnd(org, end))
        _minorTicks.Add(TransformOriginalToModified(p));

      _minorTicks.Sort();
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

      if (InternalPreProcessScaleBoundaries(ref dorg, ref dend, isOrgExtendable, isEndExtendable))
      {
        org = dorg;
        end = dend;
        return true;
      }
      else
      {
        return false;
      }
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

      if (_cachedMajorMinor == null || _cachedMajorMinor.Org != dorg || _cachedMajorMinor.End != dend)
      {
        InternalPreProcessScaleBoundaries(ref dorg, ref dend, false, false); // make sure that _cachedMajorMinor is valid now
      }

      if (!(null != _cachedMajorMinor))
        throw new InvalidProgramException();

      _majorTicks.Clear();
      _minorTicks.Clear();
      var rawMajorTicks = InternalCalculateRawMajorTicks(_cachedMajorMinor.TickGeneratingOrg, _cachedMajorMinor.TickGeneratingEnd, TargetNumberOfMajorTicks);
      InternalCalculateMajorTicks(rawMajorTicks, dorg, dend);
      var rawMinorTicks = InternalCalculateRawMinorTicks(_cachedMajorMinor.TickGeneratingOrg, _cachedMajorMinor.TickGeneratingEnd, TargetNumberOfMajorTicks, TargetNumberOfMinorTicks, rawMajorTicks);
      InternalCalculateMinorTicks(rawMinorTicks, dorg, dend);
    }

    #region Calculation of tick values

    /// <summary>
    /// Internal calculates raw major ticks. They are considered raw because i) they are not transformed yet (thus they are really probabilities), and they exceed the range between scale org and end,
    /// which is by design in order to snap the scale bounds to those ticks.
    /// </summary>
    /// <param name="org">The scale origin</param>
    /// <param name="end">The scale end.</param>
    /// <param name="TargetNumberOfMajorTicks">The target number of major ticks.</param>
    /// <returns>List with raw major ticks.</returns>
    private static ProbabilityListMajor InternalCalculateRawMajorTicks(double org, double end, int TargetNumberOfMajorTicks)
    {
      var list1stGen = new ProbabilityListMajor(org, end);

      if (TargetNumberOfMajorTicks > 0)
      {
        bool useAllMajorTicks1stGen = false;

        double quantOrg = ProbabilityToLinear(org);
        double quantEnd = ProbabilityToLinear(end);

        // first generation
        double majVal = 0.5;

        list1stGen.Add(majVal);

        if (TargetNumberOfMajorTicks > 1)
        {
          double quantCenter = ProbabilityToLinear(0.5);

          // how many 1st generation major ticks from org
          var lgOrg = -Math.Floor(Math.Log10(org)); // 0.1 -> 1, 0.01 => 2 etc.

          // how many 1st generation major ticks from end
          var lgEnd = -Math.Floor(Math.Log10(1 - end)); // 0.1 -> 1, 0.01 => 2 etc.

          if (TargetNumberOfMajorTicks >= (1 + lgOrg + lgEnd)) // then we can use all major ticks
          {
            list1stGen.SkipStepLower = 1;
            list1stGen.SkipStepUpper = 1;
            useAllMajorTicks1stGen = true;
            for (int i = (int)lgOrg; i > 0; --i)
            {
              var p = RMath.Pow(10, -i);
              list1stGen.Add(p);
            }
            for (int i = (int)lgEnd; i > 0; --i)
            {
              var p = RMath.Pow(10, -i);
              list1stGen.Add(1 - p);
            }
          }
          else // we have to comb out some ticks
          {
            var relLowerSpace = (quantCenter - quantOrg) / (quantEnd - quantOrg);
            var relUpperSpace = (quantEnd - quantCenter) / (quantEnd - quantOrg);

            var targetNumberOfTicksLower = (TargetNumberOfMajorTicks - 1) * relLowerSpace;
            var targetNumberOfTicksUpper = (TargetNumberOfMajorTicks - 1) * relUpperSpace;

            var combFactorLower = Math.Max(1, (int)Math.Round(lgOrg / targetNumberOfTicksLower));
            var combFactorUpper = Math.Max(1, (int)Math.Round(lgEnd / targetNumberOfTicksUpper));

            // we try to find the best start for i, we can choose the start i between 1 .. combFactor
            // the strategy will be to calculate the linear distance
            Func<double, double, double> distanceFunc = (dist1, dist2) =>
            {
              if (dist2 < dist1)
              { var h = dist2; dist2 = dist1; dist1 = h; }
              return Math.Abs(1 - dist1 / dist2);
            };

            double minDistance = double.MaxValue;
            int starti = combFactorLower;
            for (int i = 1; i <= combFactorLower; ++i)
            {
              var p1 = RMath.Pow(10, -i);
              var p2 = RMath.Pow(10, -i - combFactorLower);
              p2 = Math.Max(p2, org);

              var linp1 = ProbabilityToLinear(p1);
              var linp2 = ProbabilityToLinear(p2);

              var dist1 = quantCenter - linp1;
              var dist2 = linp1 - linp2;
              var dist = distanceFunc(dist1, dist2);
              if (dist < minDistance)
              {
                minDistance = dist;
                starti = i;
              }
            }

            list1stGen.SkipStepLower = combFactorLower;
            list1stGen.StartIdxLower = starti;

            for (int i = starti; i <= lgOrg + (combFactorLower - 1); i += combFactorLower)
            {
              var p = RMath.Pow(10, -i);
              list1stGen.Add(p);
            }

            minDistance = double.MaxValue;
            starti = combFactorUpper;
            for (int i = 1; i <= combFactorUpper; ++i)
            {
              var p1 = 1 - RMath.Pow(10, -i);
              var p2 = 1 - RMath.Pow(10, -i - combFactorLower);
              p2 = Math.Min(p2, end);

              var linp1 = ProbabilityToLinear(p1);
              var linp2 = ProbabilityToLinear(p2);

              var dist1 = linp1 - quantCenter;
              var dist2 = linp2 - linp1;
              var dist = distanceFunc(dist1, dist2);
              if (dist < minDistance)
              {
                minDistance = dist;
                starti = i;
              }
            }

            list1stGen.SkipStepUpper = combFactorUpper;
            list1stGen.StartIdxUpper = starti;

            for (int i = starti; i <= lgEnd + (combFactorUpper - 1); i += combFactorUpper)
            {
              var p = RMath.Pow(10, -i);
              list1stGen.Add(1 - p);
            }
          }
        } // if (TargetNumberOfMajorTicks > 1)
        else if (org > 0.5)
        {
          list1stGen.SkipStepLower = 0;
          list1stGen.StartIdxLower = 0;

          // how many 1st generation major ticks from org
          var lgOrg = (int)(-Math.Floor(Math.Log10(1 - org))); // 0.1 -> 1, 0.01 => 2 etc.

          // how many 1st generation major ticks from end
          var lgEnd = (int)(-Math.Floor(Math.Log10(1 - end))); // 0.1 -> 1, 0.01 => 2 etc.

          var ticks = 1 + lgEnd - lgOrg;
          var combFactor = Math.Max(1, (int)Math.Round((ticks) / (double)TargetNumberOfMajorTicks));
          useAllMajorTicks1stGen = 1 == combFactor;

          // in order to choose the starti, we compare all possible starti values and we choose that value, which leads to the
          // maximum number of major ticks
          int maxNumTicks = 0;
          int bestStarti = 0;
          for (int starti = 0; starti < combFactor; starti++)
          {
            int numTicks = 0;
            for (int i = starti + lgOrg; i <= lgEnd; i += combFactor)
            {
              var p = 1 - RMath.Pow(10, -i);
              if (org <= p && p <= end)
                ++numTicks;
            }
            if (numTicks > maxNumTicks)
            {
              maxNumTicks = numTicks;
              bestStarti = starti;
            }
          }

          list1stGen.StartIdxUpper = bestStarti + lgOrg;
          list1stGen.SkipStepUpper = combFactor;

          for (int i = bestStarti + lgOrg; i <= lgEnd + (combFactor - 1); i += combFactor)
          {
            var p = 1 - RMath.Pow(10, -i);
            list1stGen.Add(p);
          }
        }
        else // end <0.5
        {
          list1stGen.StartIdxUpper = 0;
          list1stGen.SkipStepUpper = 0;

          // how many 1st generation major ticks from org
          var lgOrg = (int)(-Math.Floor(Math.Log10(org))); // 0.1 -> 1, 0.01 => 2 etc.

          // how many 1st generation major ticks from end
          var lgEnd = (int)(-Math.Floor(Math.Log10(end))); // 0.1 -> 1, 0.01 => 2 etc.

          var ticks = 1 - lgEnd + lgOrg;
          var combFactor = Math.Max(1, (int)Math.Round((ticks) / (double)TargetNumberOfMajorTicks));
          useAllMajorTicks1stGen = 1 == combFactor;

          // in order to choose the starti, we compare all possible starti values and we choose that value, which leads to the
          // maximum number of major ticks
          int maxNumTicks = 0;
          int bestStarti = 0;
          for (int starti = 0; starti < combFactor; starti++)
          {
            int numTicks = 0;
            for (int i = starti + lgOrg; i <= lgEnd; i += combFactor)
            {
              var p = RMath.Pow(10, -i);
              if (org <= p && p <= end)
                ++numTicks;
            }
            if (numTicks > maxNumTicks)
            {
              maxNumTicks = numTicks;
              bestStarti = starti;
            }
          }

          list1stGen.StartIdxLower = bestStarti + lgOrg;
          list1stGen.SkipStepLower = combFactor;

          for (int i = bestStarti + lgOrg; i <= lgEnd + (combFactor - 1); i += combFactor)
          {
            var p = RMath.Pow(10, -i);
            list1stGen.Add(p);
          }
        }

        var list2ndGen = new ProbabilityList(org, end);

        if (useAllMajorTicks1stGen) // then maybe additional major ticks are required
        {
          int prevCount = list1stGen.NumberOfEntriesWithinBounds;

          foreach (var func in new Action<ProbabilityList, ProbabilityList, double, double>[] { AddMajorTicks2ndGen, AddMajorTicks3rdGen, AddMajorTicks4thGen })
          {
            var currList = new ProbabilityList(org, end);

            func(list1stGen, currList, org, end);

            int currCount = list1stGen.NumberOfEntriesWithinBounds + currList.NumberOfEntriesWithinBounds;

            if (currCount >= TargetNumberOfMajorTicks)
            {
              // decide between this and previous list
              var diff1 = Math.Abs(prevCount - TargetNumberOfMajorTicks);
              var diff2 = Math.Abs(currCount - TargetNumberOfMajorTicks);

              if (diff1 < diff2)
              {
                // we use prevList, thus do nothing
              }
              else
              {
                // we use currList
                list2ndGen = currList;
              }
              break;
            }
            list2ndGen = currList;
            prevCount = currCount;
          }
        }

        // Transform probabilities to tick values
        list1stGen.AddRange(list2ndGen);
      }

      return list1stGen;
    }

    /// <summary>
    /// Internal calculates raw minor ticks. They are considered raw because i) they are not transformed yet (thus they are really probabilities), and they exceed the range between scale org and end,
    /// which is by design in order to snap the scale bounds to those ticks.
    /// </summary>
    /// <param name="org">The scale origin</param>
    /// <param name="end">The scale end.</param>
    /// <param name="TargetNumberOfMajorTicks">The target number of major ticks.</param>
    /// <param name="TargetNumberOfMinorTicks">The target number of minor ticks.</param>
    /// <param name="rawMajorTicks">The list of raw major ticks.</param>
    /// <returns>List with raw minor ticks.</returns>
    private static ProbabilityList InternalCalculateRawMinorTicks(double org, double end, int TargetNumberOfMajorTicks, int TargetNumberOfMinorTicks, ProbabilityListMajor rawMajorTicks)
    {
      var rawMinorTicks1stGen = new ProbabilityList(org, end);

      if (TargetNumberOfMinorTicks > 1 && rawMajorTicks.Count > 0)
      {
        AddMinorTicks1stGen(rawMajorTicks, rawMinorTicks1stGen, org, end);

        var rawMajorTicksPlusMinorTicks1stGen = new ProbabilityList(org, end);
        rawMajorTicksPlusMinorTicks1stGen.AddRange(rawMajorTicks);
        rawMajorTicksPlusMinorTicks1stGen.AddRange(rawMinorTicks1stGen);

        int prevCount = rawMinorTicks1stGen.NumberOfEntriesWithinBounds;
        ProbabilityList rawMinorTicks2ndGen = null;

        foreach (var func in new Action<ProbabilityList, ProbabilityList, double, double>[] { AddMajorTicks2ndGen, AddMajorTicks3rdGen, AddMajorTicks4thGen })
        {
          var currList = new ProbabilityList(org, end);

          func(rawMajorTicksPlusMinorTicks1stGen, currList, org, end);

          int currCount = rawMinorTicks1stGen.NumberOfEntriesWithinBounds + currList.NumberOfEntriesWithinBounds;

          if (currCount >= TargetNumberOfMinorTicks * Math.Max(1, TargetNumberOfMajorTicks))
          {
            // decide between this and previous list
            var diff1 = Math.Abs(prevCount - (TargetNumberOfMinorTicks - 1) * Math.Max(1, TargetNumberOfMajorTicks));
            var diff2 = Math.Abs(currCount - (TargetNumberOfMinorTicks - 1) * Math.Max(1, TargetNumberOfMajorTicks));

            if (diff1 < diff2)
            {
              // we use prevList, thus do nothing
            }
            else
            {
              // we use currList
              rawMinorTicks2ndGen = currList;
            }
            break;
          }
          rawMinorTicks2ndGen = currList;
          prevCount = currCount;
        }
        if (null != rawMinorTicks2ndGen)
          rawMinorTicks1stGen.AddRange(rawMinorTicks2ndGen);
      }
      return rawMinorTicks1stGen;
    }

    /// <summary>
    /// Determines whether the provided probability is a lower probability (equal to or less than 0.1) and is a power of 10.
    /// </summary>
    /// <param name="p">The probability</param>
    /// <param name="power">The power of ten.</param>
    /// <returns>True if it is a lower decimal probability.</returns>
    private static bool IsLowerDecimalProbability(double p, out int power)
    {
      if (p >= 0.5)
      {
        power = 0;
        return false;
      }

      var l = Math.Log10(p);
      power = (int)Math.Round(l, 0);
      return Math.Abs(l - power) < 0.001;
    }

    /// <summary>
    /// Determines whether the provided probability is a upper probability (equal to or greater than 0.9) and the difference to 1 is a power of 10.
    /// </summary>
    /// <param name="p">The probability</param>
    /// <param name="power">The power of ten.</param>
    /// <returns>True if it is a lower decimal probability.</returns>
    public static bool IsUpperDecimalProbability(double p, out int power)
    {
      if (p <= 0.5)
      {
        power = 0;
        return false;
      }

      var l = Math.Log10(1 - p);
      power = (int)-Math.Round(l, 0);
      return Math.Abs(l + power) < 0.001;
    }

    /// <summary>
    /// Adds the minor ticks of first generation. 1st generation is defined to fill the gaps in the power of 10 of the major ticks (example: major ticks at 0.1 and 0.001 results in minor tick 0.01).
    /// </summary>
    /// <param name="list1stGen">The list of major probability ticks.</param>
    /// <param name="list2ndGen">The list to fill with 1st generation tick values.</param>
    /// <param name="org">The scale origin</param>
    /// <param name="end">The scale end.</param>
    private static void AddMinorTicks1stGen(ProbabilityListMajor list1stGen, ICollection<double> list2ndGen, double org, double end)
    {
      // 1st gen includes to fill the major probabilities that were skipped

      // lower
      if (list1stGen.SkipStepLower > 1)
      {
        double min = org;
        if (list1stGen.Count > 0)
          min = Math.Min(min, list1stGen.Min());

        int endIdx = (int)-Math.Floor(Math.Log10(org));
        for (int i = list1stGen.StartIdxLower; i <= endIdx; i += list1stGen.SkipStepLower)
        {
          for (int j = 1; j < list1stGen.SkipStepLower; ++j)
            list2ndGen.Add(RMath.Pow(10, -i - j));
        }
      }

      // upper
      if (list1stGen.SkipStepUpper > 1)
      {
        double max = end;
        if (list1stGen.Count > 0)
          max = Math.Max(max, list1stGen.Max());

        int endIdx = (int)-Math.Floor(Math.Log10(1 - max));
        for (int i = list1stGen.StartIdxUpper; i <= endIdx; i += list1stGen.SkipStepUpper)
        {
          for (int j = 1; j < list1stGen.SkipStepUpper; ++j)
            list2ndGen.Add(1 - RMath.Pow(10, -i - j));
        }
      }
    }

    /// <summary>
    /// Adds major (or minor) ticks of 2nd generation. 2nd generation is defined to add the probabilities 0.25 and 0.75 and probability values ending with 5 (example: major ticks at 0.1 and 0.01 results in adding 0.05).
    /// </summary>
    /// <param name="list1stGen">The list of major probability ticks.</param>
    /// <param name="list2ndGen">The list to fill with 2nd generation tick values.</param>
    /// <param name="org">The scale origin</param>
    /// <param name="end">The scale end.</param>
    private static void AddMajorTicks2ndGen(IEnumerable<double> list1stGen, ICollection<double> list2ndGen, double org, double end)
    {
      foreach (var pp in new[] { 0.25, 0.75 })
        list2ndGen.Add(pp);

      foreach (var prob in list1stGen)
      {
        if (IsLowerDecimalProbability(prob, out var power) && power < -1)
        {
          list2ndGen.Add(5 * RMath.Pow(10, power));
        }
        else if (IsUpperDecimalProbability(prob, out power))
        {
          list2ndGen.Add(1 - 5 * RMath.Pow(10, -power - 1));
        }
      }
    }

    /// <summary>
    /// Adds major (or minor) ticks of 2nd generation. 2nd generation is defined to add the probabilities 0.2, 0.3 .. 0.8 and probability values ending with 2 and 5 (example: major ticks at 0.1 and 0.01 results in adding 0.02 and 0.05).
    /// </summary>
    /// <param name="list1stGen">The list of major probability ticks.</param>
    /// <param name="list3rdGen">The list to fill with 3rd generation tick values.</param>
    /// <param name="org">The scale origin</param>
    /// <param name="end">The scale end.</param>
    private static void AddMajorTicks3rdGen(IEnumerable<double> list1stGen, ICollection<double> list3rdGen, double org, double end)
    {
      foreach (var pp in new[] { 0.2, 0.3, 0.4, 0.6, 0.7, 0.8 })
        list3rdGen.Add(pp);

      foreach (var prob in list1stGen)
      {
        if (IsLowerDecimalProbability(prob, out var power) && power < -1)
        {
          list3rdGen.Add(2 * RMath.Pow(10, power));
          list3rdGen.Add(5 * RMath.Pow(10, power));
        }
        else if (IsUpperDecimalProbability(prob, out power))
        {
          list3rdGen.Add(1 - 5 * RMath.Pow(10, -power - 1));
          list3rdGen.Add(1 - 2 * RMath.Pow(10, -power - 1));
        }
      }
    }

    /// <summary>
    /// Adds major (or minor) ticks of 4th generation. 4th generation is defined to add the probabilities 0.15, 0.2, 0.25 .. 0.85 and probability values ending with 2..9 (example: major ticks at 0.1 and 0.01 results in adding 0.02, 0.023 0.04, .. 0.09).
    /// </summary>
    /// <param name="list1stGen">The list of major probability ticks.</param>
    /// <param name="list4thGen">The list to fill with 4th generation tick values.</param>
    /// <param name="org">The scale origin</param>
    /// <param name="end">The scale end.</param>
    private static void AddMajorTicks4thGen(IEnumerable<double> list1stGen, ICollection<double> list4thGen, double org, double end)
    {
      foreach (var pp in new[] { 0.15, 0.2, 0.25, 0.3, 0.35, 0.4, 0.45, 0.55, 0.6, 0.65, 0.7, 0.75, 0.8, 0.85 })
        list4thGen.Add(pp);

      foreach (var prob in list1stGen)
      {
        if (IsLowerDecimalProbability(prob, out var power) && power < -1)
        {
          for (int i = 2; i <= 9; ++i)
            list4thGen.Add(i * RMath.Pow(10, power));
        }
        else if (IsUpperDecimalProbability(prob, out power))
        {
          for (int i = 9; i >= 2; --i)
            list4thGen.Add(1 - i * RMath.Pow(10, -power - 1));
        }
      }
    }

    #endregion Calculation of tick values

    #region Functions to predict change of scale by tick snapping, grace, and OneLever

    private bool InternalPreProcessScaleBoundaries(ref double xorg, ref double xend, bool isOrgExtendable, bool isEndExtendable)
    {
      _cachedMajorMinor = null;
      bool modified = false;

      if (xorg <= 0)
      {
        xorg = CumulativeProbabilityScaleRescaleConditions.DefaultOrgValue;
        modified = true;
      }
      else if (xorg >= 1)
      {
        xorg = CumulativeProbabilityScaleRescaleConditions.DefaultEndValue;
        modified = true;
      }

      if (xend <= 0)
      {
        xend = CumulativeProbabilityScaleRescaleConditions.DefaultOrgValue;
        modified = true;
      }
      else if (xend >= 1)
      {
        xend = CumulativeProbabilityScaleRescaleConditions.DefaultEndValue;
        modified = true;
      }

      if (xend == xorg)
      {
        double h = Math.Abs(xorg / 2);
        xorg -= h;
        xend += h;
        modified = true;
      }

      if (!(xorg <= xend))
      {
        var h = xorg;
        xorg = xend;
        xend = h;
        modified = true;
      }

      bool modGraceAndOneLever = GetOrgEndWithGrace(xorg, xend, isOrgExtendable, isEndExtendable, out var xOrgWithGraceAndOneLever, out var xEndWithGraceAndOneLever);
      bool modTickSnapping = GetOrgEndWithTickSnappingOnly(xorg, xend, isOrgExtendable, isEndExtendable, out var xOrgWithTickSnapping, out var xEndWithTickSnapping);

      // now compare the two
      if (xOrgWithTickSnapping <= xOrgWithGraceAndOneLever && xEndWithTickSnapping >= xEndWithGraceAndOneLever)
      {
        // then there is no need to apply Grace and OneLever
        modified |= modTickSnapping;
        _cachedMajorMinor = new CachedMajorMinor(xOrgWithTickSnapping, xEndWithTickSnapping, xorg, xend);
      }
      else
      {
        modified |= modGraceAndOneLever;
        modified |= GetOrgEndWithTickSnappingOnly(xOrgWithGraceAndOneLever, xEndWithGraceAndOneLever, isOrgExtendable, isEndExtendable, out xOrgWithTickSnapping, out xEndWithTickSnapping);
        _cachedMajorMinor = new CachedMajorMinor(xOrgWithTickSnapping, xEndWithTickSnapping, xOrgWithGraceAndOneLever, xEndWithGraceAndOneLever);
      }

      xorg = xOrgWithTickSnapping;
      xend = xEndWithTickSnapping;

      return modified;
      ;
    }

    private static double ProbabilityToLinear(double p)
    {
      const double SquareRootOf2 = 1.4142135623730950488016887242097;
      return SquareRootOf2 * Altaxo.Calc.ErrorFunction.InverseErf(-1 + 2 * p);
    }

    /// <summary>
    /// Applies the value for <see cref="OrgGrace"/>, <see cref="EndGrace"/> to the scale and calculated proposed values for the boundaries.
    /// </summary>
    /// <param name="scaleOrg">Scale origin.</param>
    /// <param name="scaleEnd">Scale end.</param>
    /// <param name="isOrgExtendable">True if the scale org can be extended.</param>
    /// <param name="isEndExtendable">True if the scale end can be extended.</param>
    /// <param name="propOrg">Returns the proposed value of the scale origin.</param>
    /// <param name="propEnd">Returns the proposed value of the scale end.</param>
    public bool GetOrgEndWithGrace(double scaleOrg, double scaleEnd, bool isOrgExtendable, bool isEndExtendable, out double propOrg, out double propEnd)
    {
      double SquareRootOf2 = Math.Sqrt(2);
      var modified = false;

      propOrg = scaleOrg;
      propEnd = scaleEnd;

      double quantOrg = SquareRootOf2 * Altaxo.Calc.ErrorFunction.InverseErf(-1 + 2 * scaleOrg);
      double quantEnd = SquareRootOf2 * Altaxo.Calc.ErrorFunction.InverseErf(-1 + 2 * scaleEnd);

      if (isOrgExtendable && OrgGrace > 0)
      {
        double propQuantOrg = quantOrg - OrgGrace * (quantEnd - quantOrg);
        propOrg = 0.5 * (1 + Altaxo.Calc.ErrorFunction.Erf(propQuantOrg / SquareRootOf2));
        modified = true;
      }

      if (isEndExtendable && EndGrace > 0)
      {
        double propQuantEnd = quantEnd + EndGrace * (quantEnd - quantOrg);
        propEnd = 0.5 * (1 + Altaxo.Calc.ErrorFunction.Erf(propQuantEnd / SquareRootOf2));
        modified = true;
      }

      return modified;
    }

    /// <summary>Applies the tick snapping settings to the scale origin and scale end. This is done by a determination of the number of decades per major tick and the minor ticks per major tick interval.
    /// Then, the snapping values are applied, and the org and end values of the scale are adjusted (if allowed so).</summary>
    /// <param name="scaleOrg">The scale origin.</param>
    /// <param name="scaleEnd">The scale end.</param>
    /// <param name="isOrgExtendable">If set to <c>true</c>, it is allowed to adjust the scale org value.</param>
    /// <param name="isEndExtendable">if set to <c>true</c>, it is allowed to adjust the scale end value.</param>
    /// <param name="propOrg">The adjusted scale orgin.</param>
    /// <param name="propEnd">The adjusted scale end.</param>
    /// <returns>True if at least either org or end were adjusted to a new value.</returns>
    private bool GetOrgEndWithTickSnappingOnly(double scaleOrg, double scaleEnd, bool isOrgExtendable, bool isEndExtendable, out double propOrg, out double propEnd)
    {
      bool modified = false;
      propOrg = scaleOrg;
      propEnd = scaleEnd;

      ProbabilityListMajor rawMajorTicks = null;
      ProbabilityList rawMinorTicks = null;

      if ((isOrgExtendable && _snapOrgToTick != BoundaryTickSnapping.SnapToNothing) || (isEndExtendable && _snapEndToTick != BoundaryTickSnapping.SnapToNothing))
      {
        rawMajorTicks = InternalCalculateRawMajorTicks(scaleOrg, scaleEnd, TargetNumberOfMajorTicks);
        rawMinorTicks = InternalCalculateRawMinorTicks(scaleOrg, scaleEnd, TargetNumberOfMajorTicks, TargetNumberOfMinorTicks, rawMajorTicks);
      }

      if (isOrgExtendable && _snapOrgToTick != BoundaryTickSnapping.SnapToNothing)
      {
        var list = new List<double>();

        switch (_snapOrgToTick)
        {
          case BoundaryTickSnapping.SnapToMajorOnly:
            list.AddRange(rawMajorTicks);
            break;

          case BoundaryTickSnapping.SnapToMinorOnly:
            list.AddRange(rawMinorTicks);
            break;

          case BoundaryTickSnapping.SnapToMinorOrMajor:
            list.AddRange(rawMajorTicks);
            list.AddRange(rawMinorTicks);
            break;

          default:
            break;
        }

        list.Sort();
        for (int i = list.Count - 1; i >= 0; --i)
        {
          if (list[i] <= scaleOrg)
          {
            modified = propOrg != list[i];
            propOrg = list[i];
            break;
          }
        }
      }

      if (isEndExtendable && _snapEndToTick != BoundaryTickSnapping.SnapToNothing)
      {
        var list = new List<double>();
        switch (_snapEndToTick)
        {
          case BoundaryTickSnapping.SnapToMajorOnly:
            list.AddRange(rawMajorTicks);
            break;

          case BoundaryTickSnapping.SnapToMinorOnly:
            list.AddRange(rawMinorTicks);
            break;

          case BoundaryTickSnapping.SnapToMinorOrMajor:
            list.AddRange(rawMajorTicks);
            list.AddRange(rawMinorTicks);
            break;

          default:
            break;
        }

        list.Sort();
        for (int i = 0; i < list.Count; ++i)
        {
          if (list[i] >= scaleEnd)
          {
            modified = propEnd != list[i];
            propEnd = list[i];
            break;
          }
        }
      }

      return modified;
    }

    #endregion Functions to predict change of scale by tick snapping, grace, and OneLever
  }
}
