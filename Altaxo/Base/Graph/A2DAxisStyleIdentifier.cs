using System;
using System.Collections.Generic;
using System.Text;
using Altaxo.Data;
namespace Altaxo.Graph
{
  /// <summary>
  /// This is an immutable class used to identify axis styles. You can safely use it without cloning.
  /// </summary>
  public sealed class A2DAxisStyleIdentifier 
  {
    /// <summary>
    /// Number of axis: 0==X-Axis, 1==Y-Axis, 2==Z-Axis
    /// </summary>
    int _axisNumber;

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

    public A2DAxisStyleIdentifier(int axisNumber, int logicalValue)
      : this(axisNumber,(double)logicalValue)
    {
    }
    
    public A2DAxisStyleIdentifier(int axisNumber, double logicalValue)
    {
      _axisNumber = axisNumber;
      _logicalValue = logicalValue;
      _usePhysicalValue = false;
    }

    public A2DAxisStyleIdentifier(int axisNumber, AltaxoVariant physicalValue)
    {
      _axisNumber = axisNumber;
      _usePhysicalValue = true;
      _physicalValue = physicalValue;
    }

    /// <summary>
    /// Number of axis: 0==X-Axis, 1==Y-Axis, 2==Z-Axis
    /// </summary>
    public int AxisNumber { get { return _axisNumber; } }

    /// <summary>
    /// The logical value of the isoline.
    /// </summary>
    public double LogicalValue { get { return _logicalValue; } }

    /// <summary>
    /// True when the isoline of this axis is determined by a physical value together with the corresponding axis scale
    /// </summary>
    public bool UsePhysicalValue { get { return _usePhysicalValue; } }

    /// <summary>
    /// The physical value of this axis.
    /// </summary>
    public AltaxoVariant PhysicalValue { get { return _physicalValue; } }


    public override bool Equals(object obj)
    {
      if (!(obj is A2DAxisStyleIdentifier))
        return false;
      A2DAxisStyleIdentifier from = (A2DAxisStyleIdentifier)obj;

      bool result = true;
      result &= this._axisNumber == from._axisNumber;
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
      int result = _axisNumber.GetHashCode();
      result += _usePhysicalValue.GetHashCode();
      if (_usePhysicalValue)
        result += _physicalValue.GetHashCode();
      else
        result += _logicalValue.GetHashCode();

      return result;
    }

    public static bool operator ==(A2DAxisStyleIdentifier a, A2DAxisStyleIdentifier b)
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
     
    public static bool operator !=(A2DAxisStyleIdentifier x, A2DAxisStyleIdentifier y)
    {
        return !(x==y);
    }

    public static A2DAxisStyleIdentifier X0
    {
      get { return new A2DAxisStyleIdentifier(0, 0); }
    }
    public static A2DAxisStyleIdentifier X1
    {
      get { return new A2DAxisStyleIdentifier(0, 1); }
    }
    public static A2DAxisStyleIdentifier Y0
    {
      get { return new A2DAxisStyleIdentifier(1, 0); }
    }
    public static A2DAxisStyleIdentifier Y1
    {
      get { return new A2DAxisStyleIdentifier(1, 1); }
    }
   
  }

}
