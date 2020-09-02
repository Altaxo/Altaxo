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
using Altaxo.Main;

namespace Altaxo.Worksheet
{
  /// <summary>
  /// Stores information how a worksheet is shown in the view. Currently, this instance stores a link to the <see cref="WorksheetLayout"/> that defines the colors and the widths of the table columns.
  /// Later, it is planned to additionally store here the positions of horizontal and vertical scroll values in order to be able to restore those settings.
  /// </summary>
  public class WorksheetViewLayout : IProjectItemPresentationModel
  {
    private WorksheetLayout _worksheetLayout;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Worksheet.GUI.WorksheetController", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoSDGui", "Altaxo.Worksheet.GUI.SDWorksheetController", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoSDGui", "Altaxo.Gui.SharpDevelop.SDWorksheetViewContent", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoSDGui", "Altaxo.Gui.SharpDevelop.SDWorksheetViewContent", 1)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Gui.Worksheet.Viewing.WorksheetController", 1)] // until 2012-02-01 buid 743
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(WorksheetViewLayout), 0)] // since 2012-02-01 buid 744
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      private AbsoluteDocumentPath? _pathToLayout;
      private WorksheetViewLayout? _tableController;

      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (WorksheetViewLayout)obj;
        info.AddValue("Layout", AbsoluteDocumentPath.GetAbsolutePath(s.WorksheetLayout));
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (WorksheetViewLayout?)o ?? new WorksheetViewLayout(info);

        var surr = new XmlSerializationSurrogate0
        {
          _tableController = s
        };
        if (info.CurrentElementName == "Controller")
        {
          info.OpenElement();
          surr._pathToLayout = (AbsoluteDocumentPath)info.GetValue("Layout", s);
          info.CloseElement();
        }
        else if (info.CurrentElementName == "BaseType")
        {
          info.GetString("BaseType");
          surr._pathToLayout = (AbsoluteDocumentPath)info.GetValue("Layout", s);
        }
        else
        {
          surr._pathToLayout = (AbsoluteDocumentPath)info.GetValue("Layout", s);
        }

        info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(surr.EhDeserializationFinished);

        return s;
      }

      private void EhDeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, Main.IDocumentNode documentRoot, bool isFinallyCall)
      {
        if (_pathToLayout is not null)
        {
          var o = AbsoluteDocumentPath.GetObject(_pathToLayout, documentRoot);
          if (o is Altaxo.Worksheet.WorksheetLayout layout)
          {
            _tableController!._worksheetLayout = layout;
            _pathToLayout = null;
          }
        }

        if (_pathToLayout is null)
        {
          info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(EhDeserializationFinished);
        }
      }
    }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    private WorksheetViewLayout(Altaxo.Serialization.Xml.IXmlDeserializationInfo _)
    {
    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

    #endregion Serialization

    public WorksheetViewLayout(WorksheetLayout worksheetLayout)
    {
      _worksheetLayout = worksheetLayout;
    }



    public WorksheetLayout WorksheetLayout
    {
      get
      {
        return _worksheetLayout;
      }
    }

    IProjectItem IProjectItemPresentationModel.Document
    {
      get
      {
        return _worksheetLayout.DataTable;
      }
    }
  }
}
