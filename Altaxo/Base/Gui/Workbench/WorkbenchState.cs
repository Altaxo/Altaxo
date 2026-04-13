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
    /// <summary>
    /// Gets or sets the window bounds of the workbench.
    /// </summary>
    public RectangleD2D Bounds { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the workbench window is maximized.
    /// </summary>
    public bool IsMaximized { get; set; }

    #region Serialization

    /// <summary>
    /// XML serialization surrogate for the legacy workbench state format.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoSDGui", "Altaxo.Gui.Workbench.AltaxoWorkbench+WorkbenchState", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version");
        /*
                var s = (WorkbenchState)obj;

                info.AddValue("Bounds", s.Bounds);
                info.AddValue("WindowState", (int)s.WindowState);
                */
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = o as WorkbenchState ?? new WorkbenchState();

        s.Bounds = (RectangleD2D)info.GetValue("Bounds", null);
        s.IsMaximized = 2 == info.GetInt32("WindowState");
        return s;
      }
    }

    /// <summary>
    /// XML serialization surrogate for the current workbench state format.
    /// </summary>
    /// <remarks>
    /// 2017-12-02 Moved to new assembly (AltaxoState). WindowState replaced by boolean IsMaximized.
    /// </remarks>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(WorkbenchState), 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (WorkbenchState)o;

        info.AddValue("Bounds", s.Bounds);
        info.AddValue("IsMaximized", s.IsMaximized);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
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
