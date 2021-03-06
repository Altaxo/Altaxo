﻿#region Copyright

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
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Altaxo.Graph
{
  public class CSPlaneInformation : ICloneable
  {
    private CSPlaneID _identifier;
    private string _name;

    public CSPlaneInformation(CSPlaneID identifier)
    {
      _identifier = identifier;
      _name = string.Empty;
    }

    public CSPlaneInformation(CSPlaneInformation from)
    {
      _identifier = from._identifier;
      CopyWithoutIdentifierFrom(from);
    }

    public void CopyFrom(CSPlaneInformation from)
    {
      if (ReferenceEquals(this, from))
        return;

      _identifier = from._identifier;
      CopyWithoutIdentifierFrom(from);
    }

    [MemberNotNull(nameof(_name))]
    public void CopyWithoutIdentifierFrom(CSPlaneInformation from)
    {
      _name = from._name;
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
