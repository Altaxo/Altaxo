using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph
{
  public class CSPlaneInformation : ICloneable
  {
    CSPlaneID _identifier;
    string _name;



    public CSPlaneInformation(CSPlaneID identifier)
    {
      _identifier = identifier;
    }
    public CSPlaneInformation(CSPlaneInformation from)
    {
      CopyFrom(from);
    }
    public void CopyFrom(CSPlaneInformation from)
    {
      this._identifier = from._identifier;
      CopyWithoutIdentifierFrom(from);
    }
    public void CopyWithoutIdentifierFrom(CSPlaneInformation from)
    {
      this._name = from._name;
     
    }

    public void SetDefaultValues()
    {
      switch (_identifier.PerpendicularAxisNumber)
      {
        case 0:
          _name = "YZ-Plane";
          break;
        case 1:
          _name = "XZ-Plane";
          break;
        case 2:
          _name = "XY-Plane";
          break;
        default:
          _name = "Plane" + _identifier.PerpendicularAxisNumber.ToString();
          break;
      }

      _name += string.Format(" (at L={0})", _identifier.LogicalValue.ToString());
    }

    public CSPlaneInformation Clone()
    {
      return new CSPlaneInformation(this);
    }
    object ICloneable.Clone()
    {
      return new CSPlaneInformation(this);
    }

    public CSPlaneID Identifier
    {
      get { return _identifier; }
    }

    /// <summary>
    /// Name of the axis style. For cartesian coordinates for instance left, right, bottom or top.
    /// </summary>
    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }

   
  }
}
