using System;
using System.Collections.Generic;
using System.Text;
using Altaxo.Data;
namespace Altaxo.Graph
{
  /// <summary>
  /// This is an immutable class used to identify axis styles. You can safely use it without cloning.
  /// </summary>
  public sealed class CS2DLineID 
  {
    /// <summary>
    /// Number of axis: 0==X-Axis, 1==Y-Axis
    /// </summary>
    int _parallelAxisNumber;

    /// <summary>
    /// The logical value of the isoline.
    /// </summary>
    double _logicalValueOther0;

    /// <summary>
    /// True when the isoline of this axis is determined by a physical value together with the corresponding axis scale
    /// </summary>
    bool _usePhysicalValueOther0;

    /// <summary>
    /// The physical value of this axis.
    /// </summary>
    AltaxoVariant _physicalValueOther0;

    public CS2DLineID(int parallelAxisNumber, int logicalValueOther)
      : this(parallelAxisNumber,(double)logicalValueOther)
    {
    }
    
    public CS2DLineID(int parallelAxisNumber, double logicalValueOther)
    {
      _parallelAxisNumber = parallelAxisNumber;
      _logicalValueOther0 = logicalValueOther;
      _usePhysicalValueOther0 = false;
    }

    private CS2DLineID()
    {
    }

    public static CS2DLineID FromPhysicalValue(int parallelAxisNumber, double physicalValueOther)
    {
      if (double.IsNaN(physicalValueOther))
        throw new ArgumentException("You can not set physical values to NaN");

      CS2DLineID id = new CS2DLineID();
      id._parallelAxisNumber = parallelAxisNumber;
      id._physicalValueOther0 = physicalValueOther;
      id._logicalValueOther0 = double.NaN;
      id._usePhysicalValueOther0 = true;
      return id;
    }

    public static CS2DLineID FromPhysicalVariant(int parallelAxisNumber, AltaxoVariant physicalValueOther)
    {
      if (!physicalValueOther.Equals(physicalValueOther))
        throw new ArgumentException("You can not set physical values that return false when compared to itself, value is: " + physicalValueOther.ToString());

      
      CS2DLineID id = new CS2DLineID();
      id._parallelAxisNumber = parallelAxisNumber;
      id._physicalValueOther0 = physicalValueOther;
      id._logicalValueOther0 = double.NaN;
      id._usePhysicalValueOther0 = true;
      return id;
    }


    /// <summary>
    /// Number of axis: 0==X-Axis, 1==Y-Axis
    /// </summary>
    public int ParallelAxisNumber { get { return _parallelAxisNumber; } }

    /// <summary>
    /// Number of axis: 0==X-Axis, 1==Y-Axis, 2==Z-Axis
    /// </summary>
    public int AxisNumberOther { get { return _parallelAxisNumber==0 ? 1 : 0; } }

    /// <summary>
    /// The logical value of the isoline. It can be set only in the constructor, or if the UsePhysicalValue property is true.
    /// </summary>
    public double LogicalValueOther { 
      get { return _logicalValueOther0; }
      set
      {
        if (_usePhysicalValueOther0)
          _logicalValueOther0 = value;
        else
          throw new NotSupportedException("You must not set the logical value of this identifier unless the property UsePhysicalValue is true");
      }
    }

    /// <summary>
    /// True when the isoline of this axis is determined by a physical value together with the corresponding axis scale
    /// </summary>
    public bool UsePhysicalValueOther { get { return _usePhysicalValueOther0; } }

    /// <summary>
    /// The physical value of this axis.
    /// </summary>
    public AltaxoVariant PhysicalValueOther { get { return _physicalValueOther0; } }


    public override bool Equals(object obj)
    {
      if (!(obj is CS2DLineID))
        return false;
      CS2DLineID from = (CS2DLineID)obj;

      bool result = true;
      result &= this._parallelAxisNumber == from._parallelAxisNumber;
      result &= this._usePhysicalValueOther0 == from._usePhysicalValueOther0;
      if (result == false)
        return result;

      if (_usePhysicalValueOther0)
        return this._physicalValueOther0 == from._physicalValueOther0;
      else
        return this._logicalValueOther0 == from._logicalValueOther0;
    }

    public override int GetHashCode()
    {
      int result = _parallelAxisNumber.GetHashCode();
      result += _usePhysicalValueOther0.GetHashCode();
      if (_usePhysicalValueOther0)
        result += _physicalValueOther0.GetHashCode();
      else
        result += _logicalValueOther0.GetHashCode();

      return result;
    }

    public static bool operator ==(CS2DLineID a, CS2DLineID b)
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
     
    public static bool operator !=(CS2DLineID x, CS2DLineID y)
    {
        return !(x==y);
    }

    public static CS2DLineID X0
    {
      get { return new CS2DLineID(0, 0); }
    }
    public static CS2DLineID X1
    {
      get { return new CS2DLineID(0, 1); }
    }
    public static CS2DLineID Y0
    {
      get { return new CS2DLineID(1, 0); }
    }
    public static CS2DLineID Y1
    {
      get { return new CS2DLineID(1, 1); }
    }
   
  }

}
