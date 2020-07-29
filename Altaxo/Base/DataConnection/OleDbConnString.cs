#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Original Copyright (C) Bernardo Castilho
//    Original Source: http://www.codeproject.com/Articles/43171/A-Visual-SQL-Query-Designer
//		Licence: The Code Project Open License (CPOL) 1.02, http://www.codeproject.com/info/cpol10.aspx
//
//    Modified 2014 by Dr. Dirk Lellinger for
//    Altaxo:  a data processing and data plotting program
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
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace Altaxo.DataConnection
{
  /// <summary>
  /// OleDb Connection string utilities.
  /// </summary>
  public static class OleDbConnString
  {
    /// <summary>
    /// Prompts the user and gets a connection string.
    /// </summary>
    public static string GetConnectionString()
    {
      return EditConnectionString(string.Empty);
    }

    /// <summary>
    /// Prompts the user and edits a connection string.
    /// </summary>
    public static string EditConnectionString(string connString)
    {
      try
      {
        // create objects we'll need
        dynamic? dlinks = null, conn = null;

        var type = Type.GetTypeFromProgID("DataLinks");
        if (!(type is null))
          dlinks = Activator.CreateInstance(type);

        var connType = Type.GetTypeFromProgID("ADODB.Connection");
        if (!(connType is null))
          conn = Activator.CreateInstance(connType); // new ADODB.ConnectionClass();

        // sanity
        if (dlinks is null || conn is null)
        {
          Warning(@"Failed to create DataLinks.\r\nPlease check that oledb32.dll is properly installed and registered.\r\n(the usual location is c:\Program Files\Common Files\System\Ole DB\oledb32.dll).");
          return connString;
        }

        // initialize object
        if (!string.IsNullOrEmpty(connString))
        {
          conn.ConnectionString = connString;
        }

        // show connection picker dialog
        object obj = conn;
        //	if (owner != null)			{		dlinks.hWnd = (int)owner.Handle;	}
        if (dlinks.PromptEdit(ref obj))
        {
          connString = conn.ConnectionString;
        }
      }
      catch (Exception x)
      {
        Warning("Failed to build connection string: {0}", x.Message);
      }

      // done
      return connString;
    }

    /// <summary>
    /// Trims a connection string for display.
    /// </summary>
    public static string TrimConnectionString(string text)
    {
      string[] keys = new string[] { "Provider", "Initial Catalog", "Data Source" };
      var sb = new StringBuilder();
      foreach (var item in text.Split(';'))
      {
        foreach (var key in keys)
        {
          if (item.IndexOf(key, StringComparison.InvariantCultureIgnoreCase) > -1)
          {
            if (sb.Length > 0)
            {
              sb.Append("...");
            }
            sb.Append(item.Split('=')[1].Trim());
          }
        }
      }
      return sb.ToString();
    }

    /// <summary>
    /// Translates Sql and Access ODBC connection strings into OleDb.
    /// </summary>
    /// <param name="connString">ODBC connection string.</param>
    /// <returns>An equivalent OleDb connection string.</returns>
    public static string TranslateConnectionString(string connString)
    {
      // we are only interested in the MSDASQL provider (ODBC data sources)
      if (connString is null ||
          connString.IndexOf("provider=msdasql", StringComparison.OrdinalIgnoreCase) < 0)
      {
        return connString ?? string.Empty;
      }

      // get name of ODBC data source
      var match = Regex.Match(connString, "Data Source=(?<ds>[^;]+)", RegexOptions.IgnoreCase);
      string ds = match.Groups["ds"].Value;
      if (ds == null || ds.Length == 0)
      {
        return connString;
      }

      // look up ODBC entry in registry (LocalMachine and CurrentUser) <<B166>>
      string keyName = @"software\odbc\odbc.ini\" + ds;
      using (var key = Registry.LocalMachine.OpenSubKey(keyName))
      {
        if (key != null)
        {
          return TranslateConnectionString(connString, key);
        }
      }
      using (var key = Registry.CurrentUser.OpenSubKey(keyName))
      {
        if (key != null)
        {
          return TranslateConnectionString(connString, key);
        }
      }

      // key not found...
      return connString;
    }

    private static string TranslateConnectionString(string connString, RegistryKey key)
    {
      // get driver
      string? driver = key.GetValue("driver") as string;

      // translate Access (jet) data sources
      if (driver != null && driver.ToLower().IndexOf("odbcjt") > -1)
      {
        string? mdb = key.GetValue("dbq") as string;
        if (mdb != null && mdb.ToLower().EndsWith(".mdb"))
        {
          return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mdb + ";";
        }
      }

      // translate SqlServer data sources
      if (driver != null && driver.ToLower().IndexOf("sqlsrv") > -1)
      {
        string? server = key.GetValue("server") as string;
        string? dbase = key.GetValue("database") as string;
        if (server != null && server.Length > 0 && dbase != null && dbase.Length > 0)
        {
          string fmt =
              "Provider=SQLOLEDB.1;Integrated Security=SSPI;" +
              "Initial Catalog={0};Data Source={1}";
          return string.Format(fmt, dbase, server);
        }
      }

      // unsupported data source...
      return connString;
    }

    // issue a warning
    private static void Warning(string format, params object[] args)
    {
      string msg = string.Format(format, args);
      Current.Gui.InfoMessageBox(msg);
    }
  }
}
