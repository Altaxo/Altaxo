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

namespace Altaxo.Main
{

  public class InstanceChangedEventArgs<T> : System.EventArgs
  {
    protected T _oldObject, _newObject;

    public InstanceChangedEventArgs(T oldObject, T newObject)
    {
      this._oldObject = oldObject;
      this._newObject = newObject;
    }

    public T NewInstance
    {
      get { return this._newObject; }
    }

    public T OldInstance
    {
      get { return this._oldObject; }
    }
  }

  public delegate void InstanceChangedEventHandler<T>(object sender, InstanceChangedEventArgs<T> e);

}
