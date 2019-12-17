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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Geometry;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// Represents a workbench state that is not minimized, i.e. either maximized or normal.
  /// </summary>
  public class WorkbenchState
  {
    public RectangleD2D Bounds { get; set; }

    public bool IsMaximized { get; set; }

    #region Serialization

    /// <summary>
    ///
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoSDGui", "Altaxo.Gui.Workbench.AltaxoWorkbench+WorkbenchState", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version");
        /*
                var s = (WorkbenchState)obj;

                info.AddValue("Bounds", s.Bounds);
                info.AddValue("WindowState", (int)s.WindowState);
                */
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = o as WorkbenchState ?? new WorkbenchState();

        s.Bounds = (RectangleD2D)info.GetValue("Bounds", null);
        s.IsMaximized = 2 == info.GetInt32("WindowState");
        return s;
      }
    }

    /// <summary>
    /// 2017-12-02 Moved to new assembly (AltaxoState). WindowState replaced by boolean IsMaximized
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(WorkbenchState), 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (WorkbenchState)obj;

        info.AddValue("Bounds", s.Bounds);
        info.AddValue("IsMaximized", s.IsMaximized);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = o as WorkbenchState ?? new WorkbenchState();

        s.Bounds = (RectangleD2D)info.GetValue("Bounds", null);
        s.IsMaximized = info.GetBoolean("IsMaximized");
        return s;
      }
    }

    #endregion Serialization
  }
}
