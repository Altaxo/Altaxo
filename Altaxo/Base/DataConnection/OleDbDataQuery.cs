#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

namespace Altaxo.DataConnection
{
  /// <summary>
  /// Immutable class that holds a connection string and an SQL selection statement.
  /// </summary>
  public sealed class OleDbDataQuery : Main.IImmutable
  {
    private AltaxoOleDbConnectionString _connectionString;
    private string _selectionStatement;
    private static OleDbDataQuery _emptyInstance = new OleDbDataQuery(string.Empty, null);

    /// <summary>
    /// Initializes a new instance of the <see cref="OleDbDataQuery"/> class.
    /// </summary>
    /// <param name="selectionStatement">The SQL selection statement.</param>
    /// <param name="connectionString">The OLE DB connection string.</param>
    public OleDbDataQuery(string selectionStatement, AltaxoOleDbConnectionString? connectionString)
    {
      _selectionStatement = selectionStatement;

      if (connectionString is null)
        _connectionString = AltaxoOleDbConnectionString.Empty;
      else
        _connectionString = connectionString;
    }

    #region Version 0

    /// <summary>
    /// 2014-06-13 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(OleDbDataQuery), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (OleDbDataQuery)o;

        info.AddValue("Connection", s._connectionString.OriginalConnectionString);
        info.AddValue("Statement", s._selectionStatement);
      }

      protected virtual OleDbDataQuery SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        string connectionString = info.GetString("Connection");
        string selectionStatement = info.GetString("Statement");
        return new OleDbDataQuery(selectionStatement, new AltaxoOleDbConnectionString(connectionString, null));
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Version 0

    /// <summary>
    /// Gets the SQL selection statement.
    /// </summary>
    public string SelectionStatement
    {
      get
      {
        return _selectionStatement;
      }
    }

    /// <summary>
    /// Returns a copy of this query with a different selection statement.
    /// </summary>
    /// <param name="value">The new selection statement.</param>
    /// <returns>A new query instance.</returns>
    public OleDbDataQuery WithSelectionStatement(string value)
    {
      return new OleDbDataQuery(value, _connectionString);
    }

    /// <summary>
    /// Gets the OLE DB connection string.
    /// </summary>
    public AltaxoOleDbConnectionString ConnectionString
    {
      get
      {
        return _connectionString;
      }
    }

    /// <summary>
    /// Returns a copy of this query with a different connection string.
    /// </summary>
    /// <param name="value">The new connection string.</param>
    /// <returns>A new query instance.</returns>
    public OleDbDataQuery WithConnectionString(AltaxoOleDbConnectionString value)
    {
      return new OleDbDataQuery(_selectionStatement, value);
    }

    /// <summary>
    /// Gets a value indicating whether this query is empty.
    /// </summary>
    public bool IsEmpty
    {
      get
      {
        return string.IsNullOrEmpty(SelectionStatement) || _connectionString.IsEmpty;
      }
    }

    /// <summary>
    /// Gets an empty query instance.
    /// </summary>
    public static OleDbDataQuery Empty
    {
      get
      {
        return _emptyInstance;
      }
    }

    /// <summary>
    /// Reads data from the configured OLE DB connection.
    /// </summary>
    /// <param name="readAction">The action that consumes the data reader.</param>
    public void ReadDataFromOleDbConnection(Action<System.Data.Common.DbDataReader> readAction)
    {
      using (var connection = new System.Data.OleDb.OleDbConnection(_connectionString.ConnectionStringWithTemporaryCredentials))
      {
        using (var command = new System.Data.OleDb.OleDbCommand(_selectionStatement, connection))
        {
          connection.Open();
          var reader = command.ExecuteReader();
          readAction(reader);
          reader.Close();
        }
      }
    }
  }
}
