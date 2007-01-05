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

namespace Altaxo.Worksheet
{
  /// <summary>
  /// Holds a table and its layout together of serialization/deserialization purposes.
  /// </summary>
  public class TablePlusLayout
  {
    Altaxo.Data.DataTable _table;
    Altaxo.Worksheet.WorksheetLayout _layout;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(TablePlusLayout),0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {

      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        TablePlusLayout s = (TablePlusLayout)obj;

        info.AddValue("Table",s._table);
        info.AddValue("Layout",s._layout);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        TablePlusLayout s = null!=o ? (TablePlusLayout)o : new TablePlusLayout();


        s._table = (Altaxo.Data.DataTable)info.GetValue("Table");
        s._layout = (Altaxo.Worksheet.WorksheetLayout)info.GetValue("Layout", parent);
        return s;
      }
    }
    #endregion


    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <param name="layout">The layout of the table.</param>
    public TablePlusLayout(Altaxo.Data.DataTable table, Altaxo.Worksheet.WorksheetLayout layout)
    {
      _table = table;
      _layout = layout;
    }

    /// <summary>
    /// Empty constructor only for deserialization purposes.
    /// </summary>
    protected TablePlusLayout()
    {
    }


    /// <summary>
    /// Gets the table.
    /// </summary>
    public Altaxo.Data.DataTable Table { get { return _table; }}
    
    /// <summary>
    /// Gets the layout.
    /// </summary>
    public Altaxo.Worksheet.WorksheetLayout Layout { get { return _layout; }}
  }
}
