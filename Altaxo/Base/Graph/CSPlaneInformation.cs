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
