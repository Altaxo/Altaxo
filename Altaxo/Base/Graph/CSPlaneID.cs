#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using Altaxo.Data;

namespace Altaxo.Graph
{
  /// <summary>
  /// This is a class used to identify axis lines in 2D or planes in 3D.
  /// A plane or axis is specified by the axis that is perpendicular to this plane or axis, and by the logical value along the
  /// perpendicular axis.
  /// </summary>
  /// <remarks>
  /// The axis numbering is intended for the coordinate system. For cartesic coordinate systems
  /// in 2D this means 0==X-Axis (normally horizontal), and 1==Y-Axis (vertical).
  /// For 3D this means 0==X-Axis (horizontal), 1==Y-Axis (vertical, and 2==Z-Axis (points in the screen).
  /// </remarks>
  public sealed class CSPlaneID : ICloneable
  {
    /// <summary>
    /// Number of axis: 0==X-Axis, 1==Y-Axis, 2==Z-Axis
    /// </summary>
    int _perpendicularAxisNumber;

    /// <summary>
    /// The logical value of the isoline.
    /// </summary>
    double _logicalValue;

    /// <summary>
    /// True when the isoline of this axis is determined by a physical value together with the corresponding axis scale
    /// </summary>
    bool _usePhysicalValue;

    /// <summary>
    /// The physical value of this axis.
    /// </summary>
    AltaxoVariant _physicalValue;


    #region Serialization
    #region Version 0
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(CSPlaneID), 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        CSPlaneID s = (CSPlaneID)obj;
        info.AddValue("Axis", s._perpendicularAxisNumber);
        info.AddValue("Logical", s._logicalValue);
        info.AddValue("UsePhysical", s._usePhysicalValue);
        if(s._usePhysicalValue)
          info.AddValue("Physical", (object)s._physicalValue);
      }
      protected virtual CSPlaneID SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        CSPlaneID s = (o == null ? new CSPlaneID() : (CSPlaneID)o);

        s._perpendicularAxisNumber = info.GetInt32("Axis");
        s._logicalValue = info.GetDouble("Logical");
        s._usePhysicalValue = info.GetBoolean("UsePhysical");
        if (s._usePhysicalValue)
          s._physicalValue = (AltaxoVariant)info.GetValue("Physical", s);

        return s;
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        CSPlaneID s = SDeserialize(o, info, parent);
        return s;
      }
    }
    #endregion
    #endregion

    public CSPlaneID(int perpendicularAxisNumber, int logicalValue)
      : this(perpendicularAxisNumber, (double)logicalValue)
    {
    }

    public CSPlaneID(int perpendicularAxisNumber, double logicalValue)
    {
      _perpendicularAxisNumber = perpendicularAxisNumber;
      _logicalValue = logicalValue;
      _usePhysicalValue = false;
    }

    private CSPlaneID()
    {
    }

    public CSPlaneID(CSPlaneID from)
    {
      this._perpendicularAxisNumber = from._perpendicularAxisNumber;
      this._logicalValue = from._logicalValue;
      this._usePhysicalValue = from._usePhysicalValue;
      this._physicalValue = from._physicalValue;
    }
    object ICloneable.Clone()
    {
      return new CSPlaneID(this);
    }
    public CSPlaneID Clone()
    {
      return new CSPlaneID(this);
    }


    public static CSPlaneID FromPhysicalValue(int perpendicularAxisNumber, double physicalValue)
    {
      if (double.IsNaN(physicalValue))
        throw new ArgumentException("You can not set physical values that return false when compared to itself, value is: " + physicalValue.ToString());

      CSPlaneID id = new CSPlaneID();
      id._perpendicularAxisNumber = perpendicularAxisNumber;
      id._physicalValue = physicalValue;
      id._logicalValue = double.NaN;
      id._usePhysicalValue = true;
      return id;
    }

    public static CSPlaneID FromPhysicalVariant(int perpendicularAxisNumber, AltaxoVariant physicalValue)
    {
      if (!(physicalValue == physicalValue))
        throw new ArgumentException("You can not set physical values that return false when compared to itself, value is: " + physicalValue.ToString());


      CSPlaneID id = new CSPlaneID();
      id._perpendicularAxisNumber = perpendicularAxisNumber;
      id._physicalValue = physicalValue;
      id._logicalValue = double.NaN;
      id._usePhysicalValue = true;
      return id;
    }


    /// <summary>
    /// Number of axis: 0==X-Axis, 1==Y-Axis, 2==Z-Axis
    /// </summary>
    public int PerpendicularAxisNumber { get { return _perpendicularAxisNumber; } }

    /// <summary>
    /// The logical value of the isoline. It can be set only in the constructor, or if the UsePhysicalValue property is true.
    /// </summary>
    public double LogicalValue
    {
      get { return _logicalValue; }
      set
      {
        if (_usePhysicalValue)
          _logicalValue = value;
        else
          throw new NotSupportedException("You must not set the logical value of this identifier unless the property UsePhysicalValue is true");
      }
    }

    /// <summary>
    /// True when the isoline of this axis is determined by a physical value together with the corresponding axis scale
    /// </summary>
    public bool UsePhysicalValue { get { return _usePhysicalValue; } }

    /// <summary>
    /// The physical value of this axis.
    /// </summary>
    public AltaxoVariant PhysicalValue { get { return _physicalValue; } }

    /// <summary>
    /// Returns the axis number of the first axis that lies on the plane.
    /// </summary>
    public int InPlaneAxisNumber1
    {
      get { return _perpendicularAxisNumber == 0 ? 1 : 0; }
    }

    /// <summary>
    /// Returns the axis number of the second axis that lies on the plane.
    /// </summary>
    public int InPlaneAxisNumber2
    {
      get { return _perpendicularAxisNumber == 2 ? 1 : 2; }
    }

    public override bool Equals(object obj)
    {
      if (!(obj is CSPlaneID))
        return false;
      CSPlaneID from = (CSPlaneID)obj;

      bool result = true;
      result &= this._perpendicularAxisNumber == from._perpendicularAxisNumber;
      result &= this._usePhysicalValue == from._usePhysicalValue;
      if (result == false)
        return result;

      if (_usePhysicalValue)
        return this._physicalValue == from._physicalValue;
      else
        return this._logicalValue == from._logicalValue;
    }

    public override int GetHashCode()
    {
      int result = _perpendicularAxisNumber.GetHashCode();
      result += _usePhysicalValue.GetHashCode();
      if (_usePhysicalValue)
        result += _physicalValue.GetHashCode();
      else
        result += _logicalValue.GetHashCode();

      return result;
    }

    public static bool operator ==(CSPlaneID a, CSPlaneID b)
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

    public static bool operator !=(CSPlaneID x, CSPlaneID y)
    {
      return !(x == y);
    }

    public static CSPlaneID Left
    {
      get { return new CSPlaneID(0, 0); }
    }
    public static CSPlaneID Right
    {
      get { return new CSPlaneID(0, 1); }
    }
    public static CSPlaneID Bottom
    {
      get { return new CSPlaneID(1, 0); }
    }
    public static CSPlaneID Top
    {
      get { return new CSPlaneID(1, 1); }
    }
    public static CSPlaneID Front
    {
      get { return new CSPlaneID(2, 0); }
    }
    public static CSPlaneID Back
    {
      get { return new CSPlaneID(2, 1); }
    }

    public static explicit operator CSLineID(CSPlaneID plane)
    {
      if (plane._perpendicularAxisNumber < 0)
        throw new ArgumentOutOfRangeException("Axis number of plane is negative");
      if (plane._perpendicularAxisNumber > 1)
        throw new ArgumentOutOfRangeException("Axis number is greater than 1. You can only convert to a line if the axis number is 0 or 1");

      if (plane.UsePhysicalValue)
        return CSLineID.FromPhysicalVariant(plane.PerpendicularAxisNumber == 0 ? 1 : 0, plane.PhysicalValue);
      else
        return new CSLineID(plane.PerpendicularAxisNumber == 0 ? 1 : 0, plane.LogicalValue);
    }

    public static explicit operator CSPlaneID(CSLineID line)
    {
      if (line.UsePhysicalValueOtherFirst)
        return CSPlaneID.FromPhysicalVariant(line.ParallelAxisNumber == 0 ? 1 : 0, line.PhysicalValueOtherFirst);
      else
        return new CSPlaneID(line.ParallelAxisNumber == 0 ? 1 : 0, line.LogicalValueOtherFirst);
    }


  }
}
