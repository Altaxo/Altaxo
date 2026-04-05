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

#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Altaxo.Graph
{
  /// <summary>
  /// Stores descriptive information for a coordinate-system plane.
  /// </summary>
  public class CSPlaneInformation : ICloneable
  {
    private CSPlaneID _identifier;
    private string _name;

    /// <summary>
    /// Initializes a new instance of the <see cref="CSPlaneInformation"/> class.
    /// </summary>
    /// <param name="identifier">The plane identifier.</param>
    public CSPlaneInformation(CSPlaneID identifier)
    {
      _identifier = identifier;
      _name = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CSPlaneInformation"/> class by copying another instance.
    /// </summary>
    /// <param name="from">The instance to copy from.</param>
    public CSPlaneInformation(CSPlaneInformation from)
    {
      _identifier = from._identifier;
      CopyWithoutIdentifierFrom(from);
    }

    /// <summary>
    /// Copies all information from another instance.
    /// </summary>
    /// <param name="from">The instance to copy from.</param>
    public void CopyFrom(CSPlaneInformation from)
    {
      if (ReferenceEquals(this, from))
        return;

      _identifier = from._identifier;
      CopyWithoutIdentifierFrom(from);
    }

    /// <summary>
    /// Copies all information except the identifier from another instance.
    /// </summary>
    /// <param name="from">The instance to copy from.</param>
    [MemberNotNull(nameof(_name))]
    public void CopyWithoutIdentifierFrom(CSPlaneInformation from)
    {
      _name = from._name;
    }

    /// <summary>
    /// Sets default values derived from the current plane identifier.
    /// </summary>
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

    /// <summary>
    /// Creates a copy of this instance.
    /// </summary>
    /// <returns>The cloned instance.</returns>
    public CSPlaneInformation Clone()
    {
      return new CSPlaneInformation(this);
    }

    /// <inheritdoc />
    object ICloneable.Clone()
    {
      return new CSPlaneInformation(this);
    }

    /// <summary>
    /// Gets the plane identifier.
    /// </summary>
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
