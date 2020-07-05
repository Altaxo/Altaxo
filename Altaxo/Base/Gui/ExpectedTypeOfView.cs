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

namespace Altaxo.Gui
{
  /// <summary>
  /// Can be used for a controller to denote which type can be controlled by this.
  /// </summary>
  public class ExpectedTypeOfViewAttribute : System.Attribute, IComparable, IClassForClassAttribute
  {
    private System.Type _type;
    private int _priority = 0;

    public ExpectedTypeOfViewAttribute(System.Type type)
    {
      _type = type;
    }

    public ExpectedTypeOfViewAttribute(System.Type type, int priority)
    {
      _type = type;
      _priority = priority;
    }

    public System.Type TargetType
    {
      get { return _type; }
    }

    public int Priority
    {
      get { return _priority; }
    }

    #region IComparable Members

    public int CompareTo(object? obj)
    {
      if (obj is ExpectedTypeOfViewAttribute to)
      {
        // Attention - we sort the items so that the item with the highest priority value is the first (!) entry in a sorted list

        return _priority == to._priority ? 0 : (_priority > to._priority ? -1 : 1);
      }
      else
      {
        throw new ArgumentException($"Argument is not of the expected type {GetType()}", nameof(obj));
      }
    }

    #endregion IComparable Members
  }
}
