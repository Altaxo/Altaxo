#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2019 Dr. Dirk Lellinger
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
  /// Can be used for a control to denote which type of controller can control this.
  /// </summary>
  public class UserControlPriorityAttribute : System.Attribute, IComparable
  {
    protected int _priority;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserControlPriorityAttribute"/> class.
    /// </summary>
    /// <param name="priority">The priority. Higher values mean higher priority.</param>
    public UserControlPriorityAttribute(int priority)
    {
      _priority = priority;
    }


    /// <summary>
    /// Gets the priority.
    /// </summary>
    /// <value>
    /// The priority value.
    /// </value>
    public int Priority
    {
      get { return _priority; }
    }

    #region IComparable Members

    /// <summary>
    /// Compares to another priority attribute.
    /// </summary>
    /// <param name="obj">The other priority attribute.</param>
    /// <returns>1 if the other priority attribute has higher priority than this attribute, -1 if this attribute has higher priority than the other; otherwise 0.</returns>
    public int CompareTo(object? obj)
    {
      // Attention - we sort the items so that the item with the highest priority value is the first (!) entry in a sorted list
      if (obj is UserControlPriorityAttribute to)
        return _priority == to._priority ? 0 : (_priority > to._priority ? -1 : 1);
      else
        throw new InvalidOperationException($"Can not compare {this.GetType()} with {obj}");
    }

    #endregion IComparable Members
  }
}
