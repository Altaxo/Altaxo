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

using Altaxo.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph
{
  /// <summary>
  /// Identifier for isolines in 2D and 3D coordinate systems. An isoline is created by varying one logical coordinate, while the other logical coordinates are fixed.
  /// The position of the isoline in the coordinate space can be specified either by the non-varying logical values (0..1), or by physical values (which are converted into logical values by the scale).
  /// </summary>
  public sealed class CSLineID : Main.IImmutable
  {
    /// <summary>
    /// Number of axis: 0==X-Axis, 1==Y-Axis, 2==Z-Axis
    /// </summary>
    private int _parallelAxisNumber;

    /// <summary>
    /// The logical value of the isoline.
    /// </summary>
    private double _logicalValueFirstOther;

    /// <summary>
    /// True when the isoline of this axis is determined by a physical value together with the corresponding axis scale
    /// </summary>
    private bool _usePhysicalValueFirstOther;

    /// <summary>
    /// The physical value of this axis.
    /// </summary>
    private AltaxoVariant _physicalValueFirstOther;

    /// <summary>
    /// The second logical value of the isoline. Only used in 3D mode.
    /// </summary>
    private double _logicalValueSecondOther;

    /// <summary>
    /// True when the isoline of this axis is determined by the second physical value together with the corresponding axis scale
    /// </summary>
    private bool _usePhysicalValueSecondOther;

    /// <summary>
    /// The second physical value of this axis.
    /// </summary>
    private AltaxoVariant _physicalValueSecondOther;

    #region Serialization

    #region Version 0

    /// <summary>
    /// Only for deserialization purposes.
    /// </summary>
    private CSLineID()
    {
      _logicalValueSecondOther = double.NaN;
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(CSLineID), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        CSLineID s = (CSLineID)obj;

        info.AddValue("Axis", s._parallelAxisNumber);

        info.AddValue("Logical1", s._logicalValueFirstOther);
        info.AddValue("UsePhysical1", s._usePhysicalValueFirstOther);
        if (s._usePhysicalValueFirstOther)
          info.AddValue("Physical1", (object)(s._physicalValueFirstOther));

        bool is3D = s.Is3DIdentifier;
        info.AddValue("Is3DIdentifier", is3D);

        if (is3D)
        {
          info.AddValue("Logical2", s._logicalValueSecondOther);
          info.AddValue("UsePhysical2", s._usePhysicalValueSecondOther);
          if (s._usePhysicalValueSecondOther)
            info.AddValue("Physical2", (object)(s._physicalValueSecondOther));
        }
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = new CSLineID();
        s._parallelAxisNumber = info.GetInt32("Axis");

        s._logicalValueFirstOther = info.GetDouble("Logical1");
        s._usePhysicalValueFirstOther = info.GetBoolean("UsePhysical1");
        if (s._usePhysicalValueFirstOther)
          s._physicalValueFirstOther = (AltaxoVariant)info.GetValue("Physical1", s);

        bool is3D = info.GetBoolean("Is3D");
        if (is3D)
        {
          s._logicalValueSecondOther = info.GetDouble("Logical1");
          s._usePhysicalValueSecondOther = info.GetBoolean("UsePhysical2");
          if (s._usePhysicalValueSecondOther)
            s._physicalValueSecondOther = (AltaxoVariant)info.GetValue("Physical2", s);
        }

        return s;
      }
    }

    #endregion Version 0

    #endregion Serialization

    /// <summary>
    /// Initialized a 2D identifier from the parallel axis and the physical value of the perpendicular axis.
    /// </summary>
    /// <param name="parallelAxisNumber">Number of parallel axis (0->X, 1->Y, 2->Z).</param>
    /// <param name="logicalValueOther">The logical value of the axis perpendicular to the parallel axis.</param>
    public CSLineID(int parallelAxisNumber, double logicalValueOther)
    {
      // test arguments
      if (parallelAxisNumber < 0 || parallelAxisNumber > 1)
        throw new ArgumentOutOfRangeException("AxisNumber must be either 0 or 1, but you provide: " + parallelAxisNumber.ToString());
      if (double.IsNaN(logicalValueOther))
        throw new ArgumentOutOfRangeException("LogicalValue is NaN, but it must be a valid number.");

      _parallelAxisNumber = parallelAxisNumber;
      _logicalValueFirstOther = logicalValueOther;
      _logicalValueSecondOther = double.NaN;
    }

    public CSLineID(int parallelAxisNumber, double logicalValueOtherFirst, double logicalValueOtherSecond)
    {
      // test arguments
      if (parallelAxisNumber < 0 || parallelAxisNumber > 2)
        throw new ArgumentOutOfRangeException("AxisNumber must be either 0, 1, or 2, but you provide: " + parallelAxisNumber.ToString());
      if (double.IsNaN(logicalValueOtherFirst))
        throw new ArgumentOutOfRangeException("LogicalValueFirst is NaN, but it must be a valid number.");
      if (double.IsNaN(logicalValueOtherSecond))
        throw new ArgumentOutOfRangeException("LogicalValueSecond is NaN, but it must be a valid number.");

      _parallelAxisNumber = parallelAxisNumber;
      _logicalValueFirstOther = logicalValueOtherFirst;
      _logicalValueSecondOther = logicalValueOtherSecond;
    }

    /// <summary>
    /// Constructs the identifier from the axis number, and a set of logical values. The part of the logical value that belongs to the provided axis number is ignored.
    /// </summary>
    /// <param name="parallelAxisNumber">Number of the axis (0: X, 1: Y, 2: Z).</param>
    /// <param name="logicalValuesOther">Set of values that determine the position of the axis line in the coordinate space.</param>
    public CSLineID(int parallelAxisNumber, Logical3D logicalValuesOther)
    {
      _parallelAxisNumber = parallelAxisNumber;
      switch (_parallelAxisNumber)
      {
        case 0:
          _logicalValueFirstOther = logicalValuesOther[1];
          _logicalValueSecondOther = logicalValuesOther[2];
          break;

        case 1:
          _logicalValueFirstOther = logicalValuesOther[0];
          _logicalValueSecondOther = logicalValuesOther[2];
          break;

        case 2:
          _logicalValueFirstOther = logicalValuesOther[0];
          _logicalValueSecondOther = logicalValuesOther[1];
          break;

        default:
          throw new ArgumentOutOfRangeException("AxisNumber must be either 0, 1, or 2, but you provide: " + parallelAxisNumber.ToString());
      }
    }

    /// <summary>
    /// Creates an instance from a given template instance and an offset to the first logical value.
    /// </summary>
    /// <param name="from">Instance to copy from.</param>
    /// <param name="offset1">Offset to the first logical value. The value given here is added to the first logical value of the template instance.</param>
    /// <returns>A new instance.</returns>
    /// <remarks>If template is using physical values instead of logical values, the offset will have no effect.</remarks>
    public static CSLineID FromIDandFirstLogicalOffset(CSLineID from, double offset1)
    {
      return from.WithLogicalValueOtherFirst(from.LogicalValueOtherFirst + offset1);
    }

    /// <summary>
    /// Initialized a 2D identifier from the parallel axis and the physical value of the perpendicular axis.
    /// </summary>
    /// <param name="parallelAxisNumber">Number of parallel axis (0->X, 1->Y, 2->Z).</param>
    /// <param name="physicalValueOther">Physical value of the axis perendicular to the parallel axis.</param>
    /// <returns>A freshly created 2D line identifier.</returns>
    public static CSLineID FromPhysicalValue(int parallelAxisNumber, double physicalValueOther)
    {
      if (parallelAxisNumber < 0 || parallelAxisNumber > 2)
        throw new ArgumentOutOfRangeException("AxisNumber must be either 0, 1, or 2, but you provide: " + parallelAxisNumber.ToString());

      if (double.IsNaN(physicalValueOther))
        throw new ArgumentException("Physical value is NaN, but it must be a valid number.");

      return new CSLineID()
      {
        _parallelAxisNumber = parallelAxisNumber,
        _physicalValueFirstOther = physicalValueOther,
        _logicalValueFirstOther = double.NaN,
        _usePhysicalValueFirstOther = true,
        _logicalValueSecondOther = double.NaN
      };
    }

    /// <summary>
    /// Initialized a 2D identifier from the parallel axis and the physical value of the perpendicular axis.
    /// </summary>
    /// <param name="parallelAxisNumber">Number of parallel axis (0->X, 1->Y, 2->Z).</param>
    /// <param name="physicalValueOther">Physical value of the axis perendicular to the parallel axis.</param>
    /// <returns>A freshly created 2D line identifier.</returns>
    public static CSLineID FromPhysicalVariant(int parallelAxisNumber, AltaxoVariant physicalValueOther)
    {
      if (parallelAxisNumber < 0 || parallelAxisNumber > 2)
        throw new ArgumentOutOfRangeException("AxisNumber must be either 0, 1, or 2, but you provide: " + parallelAxisNumber.ToString());

      if (!physicalValueOther.Equals(physicalValueOther))
        throw new ArgumentException("You can not set physical values that return false when compared to itself, value is: " + physicalValueOther.ToString());

      return new CSLineID()
      {
        _parallelAxisNumber = parallelAxisNumber,
        _physicalValueFirstOther = physicalValueOther,
        _logicalValueFirstOther = double.NaN,
        _usePhysicalValueFirstOther = true,
        _logicalValueSecondOther = double.NaN
      };
    }

    /// <summary>
    /// Returns true when this is an identifier for 3D coordinate sytems.
    /// </summary>
    public bool Is3DIdentifier
    {
      get
      {
        return _usePhysicalValueSecondOther || !double.IsNaN(_logicalValueSecondOther);
      }
    }

    /// <summary>
    /// Returns true when this is an identifier for 2D coordinate systems, i.e. when the 3rd dimension is not initialized.
    /// </summary>
    public bool Is2DIdentifier
    {
      get
      {
        return !Is3DIdentifier;
      }
    }

    /// <summary>
    /// Number of axis: 0==X-Axis, 1==Y-Axis, 2=Z-Axis of the coordinate system.
    /// </summary>
    public int ParallelAxisNumber { get { return _parallelAxisNumber; } }

    /// <summary>
    /// Number of first alternate axis: 0==X-Axis, 1==Y-Axis, 2==Z-Axis
    /// </summary>
    public int AxisNumberOtherFirst
    {
      get
      {
        return _parallelAxisNumber == 0 ? 1 : 0;
      }
    }

    /// <summary>
    /// Number of first alternate axis: 0==X-Axis, 1==Y-Axis, 2==Z-Axis
    /// </summary>
    public int AxisNumberOtherSecond
    {
      get
      {
        return _parallelAxisNumber == 2 ? 1 : 2;
      }
    }

    /// <summary>
    /// The logical value of the isoline. It can be set only in the constructor, or if the UsePhysicalValue property is true.
    /// </summary>
    public double LogicalValueOtherFirst
    {
      get
      {
        return _logicalValueFirstOther;
      }
    }

    public CSLineID WithLogicalValueOtherFirst(double logicalValueOtherFirst)
    {
      if (_logicalValueFirstOther == logicalValueOtherFirst)
      {
        return this;
      }
      else
      {
        // setting this value is only intended if UsePhysicalValue is true, and the logical value is calculated from the physical value. The result can then be stored here.
        if (!_usePhysicalValueFirstOther)
          throw new NotSupportedException("You must not set the logical value of this identifier unless the property UsePhysicalValue is true");

        var result = (CSLineID)this.MemberwiseClone();
        result._logicalValueFirstOther = logicalValueOtherFirst;
        return result;
      }
    }

    /// <summary>
    /// The logical value of the isoline. It can be set only in the constructor, with one exception:
    /// If <see cref="UsePhysicalValueOtherSecond"/> property is true, then the logical value is calculated from the current scale, and the logical value
    /// is set here for further reference during the painting.
    /// </summary>
    public double LogicalValueOtherSecond
    {
      get
      {
        return _logicalValueSecondOther;
      }
    }

    public CSLineID WithLogicalValueOtherSecond(double logicalValueOtherSecond)
    {
      if (_logicalValueSecondOther == logicalValueOtherSecond)
      {
        return this;
      }
      else
      {
        // setting this value is only intended if UsePhysicalValue is true, and the logical value is calculated from the physical value. The result can then be stored here.
        if (!_usePhysicalValueSecondOther)
          throw new NotSupportedException("You must not set the logical value of this identifier unless the property UsePhysicalValue is true");

        var result = (CSLineID)this.MemberwiseClone();
        result._logicalValueSecondOther = logicalValueOtherSecond;
        return result;
      }
    }

    /// <summary>
    /// True when the isoline of this axis is determined by a physical value together with the corresponding axis scale
    /// </summary>
    public bool UsePhysicalValueOtherFirst
    {
      get
      {
        return _usePhysicalValueFirstOther;
      }
    }

    /// <summary>
    /// True when the isoline of this axis is determined by a physical value together with the corresponding axis scale
    /// </summary>
    public bool UsePhysicalValueOtherSecond
    {
      get
      {
        return _usePhysicalValueSecondOther;
      }
    }

    /// <summary>
    /// The physical value of the first alternate plane.
    /// </summary>
    public AltaxoVariant PhysicalValueOtherFirst { get { return _physicalValueFirstOther; } }

    /// <summary>
    /// The physical value of the second alternate plane.
    /// </summary>
    public AltaxoVariant PhysicalValueOtherSecond { get { return _physicalValueSecondOther; } }

    /// <summary>
    /// Gets a logical point on the line.
    /// </summary>
    /// <param name="logicalCoordOnLine">Is the coordinate of the axis[parallelaxisnumber]. The other two values are determined by the values stored internally.</param>
    /// <returns>The logical point on the axis.</returns>
    public Logical3D GetLogicalPoint(double logicalCoordOnLine)
    {
      if (Is2DIdentifier)
      {
        switch (_parallelAxisNumber)
        {
          case 0:
            return new Logical3D(logicalCoordOnLine, _logicalValueFirstOther);

          case 1:
          default:
            return new Logical3D(_logicalValueFirstOther, logicalCoordOnLine);
        }
      }
      else
      {
        switch (_parallelAxisNumber)
        {
          case 0:
            return new Logical3D(logicalCoordOnLine, _logicalValueFirstOther, _logicalValueSecondOther);

          case 1:
            return new Logical3D(_logicalValueFirstOther, logicalCoordOnLine, _logicalValueSecondOther);

          case 2:
          default:
            return new Logical3D(_logicalValueFirstOther, _logicalValueSecondOther, logicalCoordOnLine);
        }
      }
    }

    public Logical3D Begin { get { return GetLogicalPoint(0); } }

    public Logical3D End { get { return GetLogicalPoint(1); } }

    public override bool Equals(object obj)
    {
      if (!(obj is CSLineID))
        return false;
      CSLineID from = (CSLineID)obj;

      if (this.Is3DIdentifier != from.Is3DIdentifier)
        return false;

      bool result = true;
      result &= this._parallelAxisNumber == from._parallelAxisNumber;
      result &= this._usePhysicalValueFirstOther == from._usePhysicalValueFirstOther;
      if (result == false)
        return false;

      if (_usePhysicalValueFirstOther)
        result &= (this._physicalValueFirstOther == from._physicalValueFirstOther);
      else
        result &= (this._logicalValueFirstOther == from._logicalValueFirstOther);
      if (result == false)
        return false;

      if (this.Is3DIdentifier)
      {
        if (this._usePhysicalValueSecondOther != from._usePhysicalValueSecondOther)
          return false;
        if (_usePhysicalValueSecondOther)
          result &= (this._physicalValueSecondOther == from._physicalValueSecondOther);
        else
          result &= (this._logicalValueSecondOther == from._logicalValueSecondOther);
      }

      return result;
    }

    public override int GetHashCode()
    {
      int result = _parallelAxisNumber.GetHashCode();
      result += _usePhysicalValueFirstOther.GetHashCode();
      if (_usePhysicalValueFirstOther)
        result += _physicalValueFirstOther.GetHashCode();
      else
        result += _logicalValueFirstOther.GetHashCode();

      if (Is3DIdentifier)
      {
        result += _usePhysicalValueSecondOther.GetHashCode();
        if (_usePhysicalValueSecondOther)
          result += _physicalValueSecondOther.GetHashCode();
        else
          result += _logicalValueSecondOther.GetHashCode();
      }

      return result;
    }

    public static bool operator ==(CSLineID a, CSLineID b)
    {
      // If both are null, or both are same instance, return true.
      if (System.Object.ReferenceEquals(a, b))
      {
        return true;
      }

      // If one is null, but not both, return false.
      if (((object)a == null) || ((object)b == null))
      {
        return false;
      }

      // Return true if the fields match:
      return a.Equals(b);
    }

    public static bool operator !=(CSLineID x, CSLineID y)
    {
      return !(x == y);
    }

    public static CSLineID X0
    {
      get { return new CSLineID(0, 0); }
    }

    public static CSLineID X1
    {
      get { return new CSLineID(0, 1); }
    }

    public static CSLineID Y0
    {
      get { return new CSLineID(1, 0); }
    }

    public static CSLineID Y1
    {
      get { return new CSLineID(1, 1); }
    }
  }
}
