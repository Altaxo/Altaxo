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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace Altaxo.Main.Services.PropertyReflection
{
  /// <summary>
  ///
  /// </summary>
  /// <remarks>
  /// <para>This class originated from the 'WPG Property Grid' project (<see href="http://wpg.codeplex.com"/>), licensed under Ms-PL.</para>
  /// </remarks>
  internal class ExpandableProperty : Property
  {
    private PropertyCollection _propertyCollection;
    private bool _automaticlyExpandObjects;
    private string _filter;

    public ExpandableProperty(object instance, PropertyDescriptor property, bool automaticlyExpandObjects, string filter)
      : base(instance, property)
    {
      _automaticlyExpandObjects = automaticlyExpandObjects;
      _filter = filter;
    }

    public ObservableCollection<Item> Items
    {
      get
      {
        if (_propertyCollection == null)
        {
          //Lazy initialisation prevent from deep search and looping
          _propertyCollection = new PropertyCollection(_property.GetValue(_instance), true, _automaticlyExpandObjects, _filter);
        }

        return _propertyCollection.Items;
      }
    }
  }
}
