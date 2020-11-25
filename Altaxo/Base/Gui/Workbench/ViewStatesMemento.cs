#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// Stores the currently selected view, so that the last selected view can be selected again after loading a project.
  /// </summary>
  public class ViewStatesMemento
  {
    /// <summary>
    /// Gets or sets the name of the zip entry of the view that was selected during storing the project.
    /// </summary>
    public string SelectedView_EntryName { get; set; } = string.Empty;

    #region "Serialization"

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ViewStatesMemento), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ViewStatesMemento)obj;

        info.AddValue("SelectedView", s.SelectedView_EntryName);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (ViewStatesMemento?)o ?? new ViewStatesMemento();

        s.SelectedView_EntryName = info.GetString("SelectedView");

        return s;
      }
    }

    #endregion "Serialization"
  }
}
