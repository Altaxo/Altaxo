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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Altaxo.Units;

namespace Altaxo.Gui
{
  using Altaxo.Units.Angle;

  public static class GuiAngleUnits
  {
    private static ReadOnlyCollection<IUnit> _instance;

    static GuiAngleUnits()
    {
      var instance = new List<IUnit>
      {
        Radian.Instance,
        Degree.Instance,
        Pi.Instance
      };
      _instance = instance.AsReadOnly();
    }

    /// <summary>
    /// Gets a read-only collection of the units that can be used for the Gui when a physical distance is needed.
    /// </summary>
    public static IList<IUnit> Collection
    {
      get
      {
        return _instance;
      }
    }
  }
}
