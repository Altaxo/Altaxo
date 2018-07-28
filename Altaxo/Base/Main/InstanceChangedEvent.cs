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

using System;

namespace Altaxo.Main
{
  public class InstanceChangedEventArgs : SelfAccumulateableEventArgs
  {
    protected object _oldObject, _newObject;

    public InstanceChangedEventArgs(object oldObject, object newObject)
    {
      this._oldObject = oldObject;
      this._newObject = newObject;
    }

    public object NewInstance
    {
      get { return this._newObject; }
    }

    public object OldInstance
    {
      get { return this._oldObject; }
    }

    public override void Add(SelfAccumulateableEventArgs e)
    {
      if (e == null)
        throw new ArgumentNullException("e");
      var other = e as InstanceChangedEventArgs;
      if (null == other)
        throw new ArgumentException("e is not of type: " + this.GetType().Name);
      if (!object.ReferenceEquals(this._newObject, other._oldObject))
        throw new ArgumentException("this.NewObject should be other.OldObject, but this is not the case. The overrides for GetHashCode and Equals should ensure this. Please debug.");

      this._newObject = other._newObject;
    }

    public override int GetHashCode()
    {
      return this.GetType().GetHashCode(); // unfortunately, we have to match all instances of the class, because in Equals we must compare the new Instance with the old Instance
    }

    public override bool Equals(object obj)
    {
      if (null == obj || this.GetType() != obj.GetType())
        return false;

      var other = (InstanceChangedEventArgs)obj;

      return object.ReferenceEquals(this._newObject, other._oldObject);
    }
  }
}
