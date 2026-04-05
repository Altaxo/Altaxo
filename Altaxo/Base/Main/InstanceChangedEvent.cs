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

namespace Altaxo.Main
{
  /// <summary>
  /// Event data describing a transition from one instance to another.
  /// </summary>
  public class InstanceChangedEventArgs : SelfAccumulateableEventArgs
  {
    /// <summary>
    /// The previous instance.
    /// </summary>
    protected object? _oldObject;

    /// <summary>
    /// The new instance.
    /// </summary>
    protected object? _newObject;

    /// <summary>
    /// Initializes a new instance of the <see cref="InstanceChangedEventArgs"/> class.
    /// </summary>
    /// <param name="oldObject">The previous instance.</param>
    /// <param name="newObject">The new instance.</param>
    public InstanceChangedEventArgs(object? oldObject, object? newObject)
    {
      _oldObject = oldObject;
      _newObject = newObject;
    }

    /// <summary>
    /// Gets the new instance.
    /// </summary>
    public object? NewInstance
    {
      get { return _newObject; }
    }

    /// <summary>
    /// Gets the previous instance.
    /// </summary>
    public object? OldInstance
    {
      get { return _oldObject; }
    }

    /// <inheritdoc/>
    public override void Add(SelfAccumulateableEventArgs e)
    {
      if (e is null)
        throw new ArgumentNullException("e");
      var other = e as InstanceChangedEventArgs;
      if (other is null)
        throw new ArgumentException("e is not of type: " + GetType().Name);
      if (!object.ReferenceEquals(_newObject, other._oldObject))
        throw new ArgumentException("this.NewObject should be other.OldObject, but this is not the case. The overrides for GetHashCode and Equals should ensure this. Please debug.");

      _newObject = other._newObject;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return GetType().GetHashCode(); // unfortunately, we have to match all instances of the class, because in Equals we must compare the new Instance with the old Instance
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
      if (obj is null || GetType() != obj.GetType())
        return false;

      var other = (InstanceChangedEventArgs)obj;

      return object.ReferenceEquals(_newObject, other._oldObject);
    }
  }
}
