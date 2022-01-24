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
using System.ComponentModel;
using Altaxo.Collections;

namespace Altaxo.Gui.Common
{
  public interface ITypeAndInstanceView : IDataContextAwareView
  {
  }

  public interface ITypeAndInstanceController : INotifyPropertyChanged
  {
    /// <summary>
    /// Gets the label text associated with the type choice combo box.
    /// </summary>
    public string TypeLabel { get; }

    /// <summary>
    /// Gets the contents of the type choice combo box. The tags of the items must be of type <see cref="System.Type"/>
    /// </summary>
    /// <value>
    /// The type names.
    /// </value>
    public SelectableListNodeList TypeNames { get; }

    public Type SelectedType { get; set; }

    public object? InstanceView { get; }
  }
}
