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

#nullable disable warnings
using System;
using System.Collections.Generic;
using System.Text;
using Altaxo.AddInItems;
using Altaxo.Drawing;

namespace Altaxo.Main
{
  /// <summary>
  /// Creates a named color from the codon identifier and color value.
  /// </summary>
  /// <attribute name="Value" use="required">
  /// The value of the color as string in standard format.
  /// </attribute>
  /// <usage>Only in /Altaxo/ApplicationColorSets, embedded in a ColorSetDoozer</usage>
  /// <returns>
  /// An NamedColor object that represents the item.</returns>
  public class NamedColorDoozer : IDoozer
  {
    /// <inheritdoc/>
    public bool HandleConditions
    {
      get
      {
        return false;
      }
    }

    /// <inheritdoc/>
    public object BuildItem(BuildItemArgs args)
    {
      string id = args.Codon.Id;
      string value = args.Codon.Properties["Value"];
      if (!string.IsNullOrEmpty(value))
      {
        var c = AxoColor.FromInvariantString(value);
        return new NamedColor(c, id);
      }
      return null;
    }
  }
}
