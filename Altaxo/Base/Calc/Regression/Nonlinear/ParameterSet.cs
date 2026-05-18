#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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
using System.Collections;
using System.Collections.Generic;

namespace Altaxo.Calc.Regression.Nonlinear
{
  /// <summary>
  /// Holds a bunch of <see cref="ParameterSetElement"/>, i.e. a collection of fit parameters together with their values.
  /// </summary>
  public class ParameterSet : ICloneable, IReadOnlyList<ParameterSetElement>
  {
    List<ParameterSetElement> _inner;

    /// <inheritdoc/>
    public int Count => _inner.Count;

    /// <summary>
    /// Additional linear constraints of the parameters that come as string expressions.
    /// Example: "a &gt; 0", "b &lt; 10", "c &gt; a", "d &lt; b + c" etc. These constraints are not evaluated by the ParameterSet itself, but they can be used by the user interface to display them and to check them.
    /// </summary>
    public string[] AdditionalConstraints { get => field; set => field = value ?? throw new ArgumentNullException(nameof(AdditionalConstraints)); } = Array.Empty<string>();

    /// <summary>
    /// Event is fired if the main initialization is finished. This event can be fired
    /// multiple times (every time the set has changed basically.
    /// </summary>
    public event EventHandler? InitializationFinished;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.Regression.Nonlinear.ParameterSet", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public virtual void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ParameterSet)o;

        info.CreateArray("Parameters", s.Count);
        for (int i = 0; i < s.Count; ++i)
          info.AddValue("e", s[i]);
        info.CommitArray();
      }

      /// <inheritdoc/>
      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        ParameterSet s = o is not null ? (ParameterSet)o : new ParameterSet();

        int arraycount = info.OpenArray();
        for (int i = 0; i < arraycount; ++i)
          s.Add((ParameterSetElement)info.GetValue("e", s));
        info.CloseArray(arraycount);

        return s;
      }
    }

    /// <summary>
    /// Version 1: 2026-05-12: Added AdditionalConstraints  
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ParameterSet), 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public virtual void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ParameterSet)o;

        info.CreateArray("Parameters", s.Count);
        for (int i = 0; i < s.Count; ++i)
          info.AddValue("e", s[i]);
        info.CommitArray();

        info.AddArray("AdditionalConstraints", s.AdditionalConstraints, s.AdditionalConstraints.Length);
      }

      /// <inheritdoc/>
      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        ParameterSet s = o is not null ? (ParameterSet)o : new ParameterSet();

        int arraycount = info.OpenArray();
        for (int i = 0; i < arraycount; ++i)
          s.Add((ParameterSetElement)info.GetValue("e", s));
        info.CloseArray(arraycount);

        s.AdditionalConstraints = info.GetArrayOfStrings("AdditionalConstraints");

        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterSet"/> class.
    /// </summary>
    public ParameterSet()
    {
      _inner = new List<ParameterSetElement>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterSet"/> class with the specified elements.
    /// </summary>
    /// <param name="elements">The parameter set elements.</param>
    public ParameterSet(IEnumerable<ParameterSetElement> elements)
    {
      _inner = new List<ParameterSetElement>(elements);
    }

    /// <summary>
    /// Raises the <see cref="InitializationFinished"/> event.
    /// </summary>
    public void OnInitializationFinished()
    {
      InitializationFinished?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc/>
    public ParameterSetElement this[int i]
    {
      get
      {
        return _inner[i];
      }
      set
      {
        _inner[i] = value ?? throw new ArgumentNullException(nameof(ParameterSetElement));
      }
    }

    /// <summary>
    /// Adds a parameter element to the set.
    /// </summary>
    /// <param name="ele">The parameter element to add.</param>
    public void Add(ParameterSetElement ele)
    {
      _inner.Add(ele);
    }

    #region ICloneable Members

    /// <inheritdoc/>
    public object Clone()
    {
      var result = new ParameterSet();
      for (int i = 0; i < Count; ++i)
        result.Add(this[i]);

      result.AdditionalConstraints = (string[])this.AdditionalConstraints.Clone();

      return result;
    }

    /// <inheritdoc/>
    IEnumerator<ParameterSetElement> IEnumerable<ParameterSetElement>.GetEnumerator()
    {
      return _inner.GetEnumerator();
    }

    /// <inheritdoc/>
    public IEnumerator GetEnumerator()
    {
      return _inner.GetEnumerator();
    }

    /// <summary>
    /// Removes all parameter elements from the set.
    /// </summary>
    internal void Clear()
    {
      _inner.Clear();
    }

    /// <summary>
    /// Returns a new parameter set with parameters set to fixed values where specified.
    /// </summary>
    /// <remarks>Parameters corresponding to non-null entries in the list are set to the provided value and
    /// marked as fixed (not varying). Other parameters are unchanged.</remarks>
    /// <param name="fixedParameters">A list of nullable double values representing the fixed values to assign to parameters. If an element is null,
    /// the corresponding parameter remains unchanged.</param>
    /// <returns>A new ParameterSet instance with parameters set to the specified fixed values and marked as not varying.</returns>
    public ParameterSet WithParametersSetToFixed(IReadOnlyList<double?> fixedParameters)
    {
      var result = (ParameterSet)this.Clone();

      for (int i = 0; i < Count; ++i)
      {
        if (fixedParameters[i] is { } fixedParam)
        {
          result[i] = result[i] with { Parameter = fixedParam, Vary = false };
        }
      }

      return result;
    }

    /// <summary>
    /// Creates a new ParameterSet with updated fixed parameter values and inclusive lower and upper bounds for each
    /// parameter.
    /// </summary>
    /// <param name="fixedParameters">A list of parameter values to fix. If an element is not null, the corresponding parameter is set to this value
    /// and marked as not varying.</param>
    /// <param name="boundsInclusive">A tuple containing arrays of inclusive lower and upper bounds. If a bound is specified for a parameter, it
    /// replaces the existing bound and is treated as inclusive.</param>
    /// <returns>A new ParameterSet instance with the specified fixed parameters and updated inclusive bounds applied.</returns>
    /// <remarks>The returned ParameterSet is a copy of the original with modifications applied. Parameters
    /// not specified in the input lists retain their original values and bounds.</remarks>
    public ParameterSet WithUpdatedFixedParametersAndBoundaries(IReadOnlyList<double?> fixedParameters, (double?[]? LowerBounds, double?[]? UpperBounds) boundsInclusive)
    {
      var result = (ParameterSet)this.Clone();

      for (int i = 0; i < Count; ++i)
      {
        var lb = result[i].LowerBound;
        var lbExcl = result[i].IsLowerBoundExclusive;
        var ub = result[i].UpperBound;
        var ubExcl = result[i].IsUpperBoundExclusive;
        var value = result[i].Parameter;
        var vary = result[i].Vary;

        if (fixedParameters[i] is { } fixedParam)
        {
          value = fixedParam;
          vary = false;
        }
        if (boundsInclusive.LowerBounds is { } lowerBounds && lowerBounds[i] is { } lbValue)
        {
          lb = lbValue;
          lbExcl = false;
        }
        if (boundsInclusive.UpperBounds is { } upperBounds && upperBounds[i] is { } ubValue)
        {
          ub = ubValue;
          ubExcl = false;
        }

        result[i] = result[i] with { Parameter = value, Vary = vary, LowerBound = lb, IsLowerBoundExclusive = lbExcl, UpperBound = ub, IsUpperBoundExclusive = ubExcl };

      }

      return result;
    }

    #endregion ICloneable Members
  }
}
