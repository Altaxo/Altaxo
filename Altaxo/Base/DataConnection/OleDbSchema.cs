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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Altaxo.DataConnection
{
	/// <summary>
	/// DataSet that knows how to retrieve a schema (tables, columns, views,
	/// sprocs, constraints, relations) based on a connection string.
	/// </summary>
	[System.ComponentModel.DesignTimeVisible(false)]
	public class OleDbSchema : DataSet
	{
		//----------------------------------------------------------------

		#region ** fields

		// connection string
		private string _connString = string.Empty;

		// constants
		private const string TABLE = "TABLE";

		private const string VIEW = "VIEW";
		private const string LINK = "LINK";
		private const string PARAMETERS = "PARAMETERS";
		private const string TABLE_NAME = "TABLE_NAME";
		private const string TABLE_TYPE = "TABLE_TYPE";
		private const string TABLE_DEFINITION = "TABLE_DEFINITION";
		private const string TABLE_SCHEMA = "TABLE_SCHEMA";
		private const string COLUMN_NAME = "COLUMN_NAME";
		private const string PK_TABLE_NAME = "PK_TABLE_NAME";
		private const string FK_TABLE_NAME = "FK_TABLE_NAME";
		private const string PK_COLUMN_NAME = "PK_COLUMN_NAME";
		private const string FK_COLUMN_NAME = "FK_COLUMN_NAME";
		private const string PROCEDURE_NAME = "PROCEDURE_NAME";
		private const string PROCEDURE_SCHEMA = "PROCEDURE_SCHEMA";
		private const string PARAMETER_NAME = "PARAMETER_NAME";
		private const string PARAMETER_DEFAULT = "PARAMETER_DEFAULT";
		private const string PROCEDURE_PARAMETERS = "PROCEDURE_PARAMETERS";
		private const string PROCEDURE_DEFINITION = "PROCEDURE_DEFINITION";
		private const string DATA_TYPE = "DATA_TYPE";
		private const string RETURN_VALUE = "RETURN_VALUE";
		private const string TABLE_RETURN_VALUE = "TABLE_RETURN_VALUE";

		#endregion ** fields

		//----------------------------------------------------------------

		#region ** object model

		/// <summary>
		/// Gets or sets the connection string used to fill this schema.
		/// </summary>
		public string ConnectionString
		{
			get { return _connString; }
			set
			{
				if (value != _connString)
				{
					_connString = value;
					GetSchema();
				}
			}
		}

		/// <summary>
		/// Gets a <see cref="OleDbSchema"/> that contains the schema for a
		/// given connection string.
		/// </summary>
		/// <param name="connString">OleDb connection string used to
		/// initialize the new <see cref="OleDbSchema"/>.</param>
		/// <returns>
		/// A new <see cref="OleDbSchema"/> containing the schema for the
		/// given <paramref name="connString"/> or null if the connection
		/// string is invalid.
		/// </returns>
		public static OleDbSchema GetSchema(string connString)
		{
			// trivial test
			connString = OleDbConnString.TranslateConnectionString(connString);
			if (string.IsNullOrEmpty(connString) ||
					connString.IndexOf("Provider=", StringComparison.OrdinalIgnoreCase) < 0)
			{
				return null;
			}

			// connString looks OK, try getting the schema
			try
			{
				var ds = new OleDbSchema();
				ds.ConnectionString = connString;
				return ds;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Gets a select statement that can be used to retrieve data from a given table,
		/// view, or stored procedure.
		/// </summary>
		/// <param name="table"><see cref="DataTable"/> that specifies the source table,
		/// view, or stored procedure that contains the data.</param>
		/// <returns>A select statement that can be used to retrieve the data.</returns>
		public static string GetSelectStatement(DataTable table)
		{
			return GetTableType(table) == TableType.Procedure
					? string.Format("EXEC {0}{1}", GetFullTableName(table), GetProcedureParameters(table))
					: string.Format("SELECT * from {0}", GetFullTableName(table));
		}

		/// <summary>
		/// Gets the table's underlying type (Table, View, or Stored Procedure).
		/// </summary>
		/// <param name="table"><see cref="DataTable"/> whose type will be returned.</param>
		/// <returns>The table type.</returns>
		public static TableType GetTableType(DataTable table)
		{
			// use TABLE_TYPE property
			switch (table.ExtendedProperties[TABLE_TYPE] as string)
			{
				case TABLE:
					return TableType.Table;

				case LINK: // ?
				case VIEW:
					return TableType.View;
			}

			// no type, this is a stored procedure
			return TableType.Procedure;
		}

		/// <summary>
		/// Gets the full table name (e.g. 'dbo.[Order Details]).
		/// </summary>
		/// <param name="table"><see cref="DataTable"/> whose name will be returned.</param>
		/// <returns>The full table name, including table schema and brackets when needed.</returns>
		public static string GetFullTableName(DataTable table)
		{
			// build table name
			var sb = new StringBuilder();

			// get schema name
			var schema = table.ExtendedProperties[TABLE_SCHEMA] as string;
			if (schema != null)
			{
				sb.AppendFormat("{0}.", schema);
			}

			// append actual table name
			sb.Append(BracketName(table.TableName));

			// done
			return sb.ToString();
		}

		/// <summary>
		/// Encloses a name in square brackets.
		/// </summary>
		/// <param name="name">Name to enclose in brackets.</param>
		/// <returns>The name enclosed in brackets.</returns>
		public static string BracketName(string name)
		{
			// already bracketed
			if (name.Length > 1 && name[0] == '[' && name[name.Length - 1] == ']')
			{
				return name;
			}

			// see if brackets are needed (never bracket expressions)
			bool needsBrackets = false; // force
			if (!IsExpression(name))
			{
				for (int i = 0; i < name.Length && !needsBrackets; i++)
				{
					char c = name[i];
					needsBrackets = i == 0
							? !char.IsLetter(c)
							: !char.IsLetterOrDigit(c) && c != '_';
				}
			}

			// done
			return needsBrackets
					? string.Format("[{0}]", name)
					: name;
		}

		private static char[] _expressionChars = "(),*".ToCharArray();

		private static bool IsExpression(string name)
		{
			return name.IndexOfAny(_expressionChars) > -1;
		}

		/// <summary>
		/// Gets the parameters required by a stored procedure.
		/// </summary>
		/// <param name="table">DataTable that contains the schema for the stored procedure.</param>
		/// <returns>A list of <see cref="OleDbParameter"/> objects.</returns>
		public static List<OleDbParameter> GetTableParameters(DataTable table)
		{
			return table.ExtendedProperties[PROCEDURE_PARAMETERS] as List<OleDbParameter>;
		}

		#endregion ** object model

		//----------------------------------------------------------------

		#region ** utilities to deal with types (.NET, OleDb, Access)

		/// <summary>
		/// Checks whether a given type is numeric.
		/// </summary>
		/// <param name="type"><see cref="Type"/> to check.</param>
		/// <returns>True if the type is numeric; false otherwise.</returns>
		public static bool IsNumeric(Type type)
		{
			// handle regular types
			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Single:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.SByte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					return true;
			}

			// handle nullable types
			type = Nullable.GetUnderlyingType(type);
			return type != null
					? IsNumeric(type)
					: false;
		}

		/// <summary>
		/// Translates an <see cref="OleDbType"/> into a .NET type.
		/// </summary>
		/// <param name="oleDbType"><see cref="OleDbType"/> to translate.</param>
		/// <returns>The corresponding .NET <see cref="Type"/>.</returns>
		public static Type GetType(OleDbType oleDbType)
		{
			switch ((int)oleDbType)
			{
				case 0: return typeof(Nullable);
				case 2: return typeof(short);
				case 3: return typeof(int);
				case 4: return typeof(float);
				case 5: return typeof(double);
				case 6: return typeof(decimal);
				case 7: return typeof(DateTime);
				case 8: return typeof(string);
				case 9: return typeof(object);
				case 10: return typeof(Exception);
				case 11: return typeof(bool);
				case 12: return typeof(object);
				case 13: return typeof(object);
				case 14: return typeof(decimal);
				case 16: return typeof(sbyte);
				case 17: return typeof(byte);
				case 18: return typeof(ushort);
				case 19: return typeof(uint);
				case 20: return typeof(long);
				case 21: return typeof(ulong);
				case 64: return typeof(DateTime);
				case 72: return typeof(Guid);
				case 128: return typeof(byte[]);
				case 129: return typeof(string);
				case 130: return typeof(string);
				case 131: return typeof(decimal);
				case 133: return typeof(DateTime);
				case 134: return typeof(TimeSpan);
				case 135: return typeof(DateTime);
				case 138: return typeof(object);
				case 139: return typeof(decimal);
				case 200: return typeof(string);
				case 201: return typeof(string);
				case 202: return typeof(string);
				case 203: return typeof(string);
				case 204: return typeof(byte[]);
				case 205: return typeof(byte[]);
			}
			Debug.WriteLine("** unknown type: " + oleDbType.ToString());
			return typeof(string);
		}

		/// <summary>
		/// Translates an Access parameter type (in PARAMETERS clause) into an <see cref="OleDbType"/>.
		/// </summary>
		/// <param name="typeName">Access parameter type.</param>
		/// <returns>The corresponding <see cref="OleDbType"/></returns>
		public static OleDbType GetOleDbType(string typeName)
		{
			switch (typeName.ToLower())
			{
				case "Bit": return OleDbType.Boolean;
				case "Byte": return OleDbType.TinyInt;
				case "Short": return OleDbType.SmallInt;
				case "Long": return OleDbType.BigInt;
				case "Currency": return OleDbType.Currency;
				case "IEEESingle": return OleDbType.Single;
				case "IEEEDouble": return OleDbType.Double;
				case "DateTime": return OleDbType.Date;
				case "Text": return OleDbType.VarChar;
				case "Decimal": return OleDbType.Decimal;
			}

			Debug.WriteLine("** unknown type: '{0}'", typeName);
			return OleDbType.VarChar;
		}

		#endregion ** utilities to deal with types (.NET, OleDb, Access)

		//----------------------------------------------------------------

		#region ** implementation

		private void GetSchema()
		{
			// translate ODBC requests into OleDb
			string connString = OleDbConnString.TranslateConnectionString(_connString);

			// initialize this DataSet
			this.Reset();

			// go get the schema
			EnforceConstraints = false;
			using (var conn = new OleDbConnection(connString))
			{
				conn.Open();
				GetTables(conn);
				GetRelations(conn);
				GetConstraints(conn);
				GetStoredProcedures(conn);
				conn.Close();
			}
		}

		private void GetTables(OleDbConnection conn)
		{
			// add tables
			var dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
			foreach (DataRow dr in dt.Rows)
			{
				// get type (table/view)
				var type = (string)dr[TABLE_TYPE];
				if (type != TABLE && type != VIEW && type != LINK)
				{
					continue;
				}

				// create table
				var name = (string)dr[TABLE_NAME];
				var table = new DataTable(name);
				table.ExtendedProperties[TABLE_TYPE] = type;

				// save definition in extended properties
				foreach (DataColumn col in dt.Columns)
				{
					table.ExtendedProperties[col.ColumnName] = dr[col];
				}

				// get table schema and add to collection
				try
				{
					var select = GetSelectStatement(table);
					var da = new OleDbDataAdapter(select, conn);
					da.FillSchema(table, SchemaType.Mapped);
					Tables.Add(table);
				}
				catch { }
			}
		}

		private void GetStoredProcedures(OleDbConnection conn)
		{
			// add stored procedures
			var dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Procedures, null);
			foreach (DataRow dr in dt.Rows)
			{
				// get the procedure name, skip system stuff
				var name = dr[PROCEDURE_NAME] as string;
				if (name.StartsWith("~") || name.StartsWith("dt_", StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}
				var schema = dr[PROCEDURE_SCHEMA] as string;
				if (object.Equals(schema, "sys"))
				{
					continue;
				}

				// trim number that comes after name in Sql databases
				int pos = name.LastIndexOf(';');
				if (pos > -1)
				{
					int i;
					if (int.TryParse(name.Substring(pos + 1), out i))
					{
						name = name.Substring(0, pos);
					}
				}

				// create table
				var table = new DataTable(name);

				// get parameters
				List<OleDbParameter> parmList = new List<OleDbParameter>();
				table.ExtendedProperties[PROCEDURE_PARAMETERS] = parmList;
				if (!conn.Provider.Contains("SQLOLEDB"))
				{
					GetAccessParameters(dr, parmList);
				}
				else
				{
					var dtParms = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Procedure_Parameters, new object[] { null, null, name, null });
					var returnsValue = GetSqlServerParameters(dtParms, parmList);
					if (!returnsValue)
					{
						continue;
					}
				}

				// get table schema
				try
				{
					var select = GetSelectStatement(table);
					var da = new OleDbDataAdapter(select, ConnectionString);
					da.FillSchema(table, SchemaType.Mapped);
				}
				catch (Exception x)
				{
					Debug.WriteLine(x.Message);
				}

				// add to collection (with or without schema)
				Tables.Add(table);
			}
		}

		// gets parameters from "PARAMETERS" statement in procedure definition (Access)
		private void GetAccessParameters(DataRow dr, List<OleDbParameter> list)
		{
			var procDef = dr[PROCEDURE_DEFINITION] as string;
			if (procDef != null && procDef.StartsWith(PARAMETERS, StringComparison.OrdinalIgnoreCase))
			{
				int pos = procDef.IndexOf(';');
				if (pos > -1)
				{
					var parmDef = procDef.Substring(11, pos - 11);
					foreach (string parm in parmDef.Split(','))
					{
						pos = parm.LastIndexOf(' ');
						if (pos > -1)
						{
							var p = new OleDbParameter();
							p.ParameterName = parm.Substring(0, pos).Trim();
							p.OleDbType = GetOleDbType(parm.Substring(pos + 1));
							list.Add(p);
						}
					}
				}
			}
		}

		// get parameters from the parameters table (SqlServer)
		private bool GetSqlServerParameters(DataTable dtParms, List<OleDbParameter> list)
		{
			bool returnsValue = false;
			foreach (DataRow parm in dtParms.Rows)
			{
				// get parameter name
				var name = ((string)parm[PARAMETER_NAME]).Substring(1);
				if (name == RETURN_VALUE || name == TABLE_RETURN_VALUE)
				{
					returnsValue = true;
					continue;
				}

				// save parameter
				var p = new OleDbParameter(name, (OleDbType)parm[DATA_TYPE]);
				p.Value = parm[PARAMETER_DEFAULT] as string;
				if (p.Value == null)
				{
					p.Value = string.Empty;
				}
				list.Add(p);
			}

			// done
			return returnsValue;
		}

		// get relations from schema
		private void GetRelations(OleDbConnection conn)
		{
			var dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Foreign_Keys, null);
			foreach (DataRow dr in dt.Rows)
			{
				// get primary/foreign table and column names
				string pkTableName = (string)dr[PK_TABLE_NAME];
				string fkTableName = (string)dr[FK_TABLE_NAME];
				string pkColumnName = (string)dr[PK_COLUMN_NAME];
				string fkColumnName = (string)dr[FK_COLUMN_NAME];

				// make sure both tables are in our DataSet
				if (Tables.Contains(pkTableName) && Tables.Contains(fkTableName))
				{
					// make sure tables are different
					if (pkTableName != fkTableName)
					{
						// get unique relation name
						string relationName = pkTableName + '_' + fkTableName;
						if (Relations.Contains(relationName))
						{
							relationName += Relations.Count.ToString();
						}

						// add to collection
						DataColumn pkColumn = Tables[pkTableName].Columns[pkColumnName];
						DataColumn fkColumn = Tables[fkTableName].Columns[fkColumnName];
						Relations.Add(relationName, pkColumn, fkColumn, true);
					}
				}
			}
		}

		// get constraints from schema
		private void GetConstraints(OleDbConnection conn)
		{
			DataTable dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Primary_Keys, null);
			var uniqueTables = new Dictionary<string, string>();
			foreach (DataRow dr in dt.Rows)
			{
				// get primary key info
				string tableName = dr[TABLE_NAME].ToString();
				string columnName = dr[COLUMN_NAME].ToString();

				// make sure this table is in our DataSet
				if (Tables.Contains(tableName))
				{
					// make sure it's unique
					if (uniqueTables.ContainsKey(tableName))
					{
						uniqueTables.Remove(tableName);
						continue;
					}

					// save and move on
					uniqueTables[tableName] = columnName;
				}
			}

			// built unique list, now set up primary key columns
			foreach (string tableName in uniqueTables.Keys)
			{
				// set up column
				var columnName = (string)uniqueTables[tableName];
				var table = Tables[tableName];
				var pk = table.Columns[columnName];
				pk.Unique = true;
				pk.AllowDBNull = false;

				// try setting auto increment
				if (pk.DataType != typeof(string) && pk.DataType != typeof(byte))
				{
					try
					{
						pk.AutoIncrement = true;
						pk.ReadOnly = true;
					}
					catch { }
				}

				// set primary key on parent table
				Tables[tableName].PrimaryKey = new DataColumn[] { pk };
			}
		}

		// returns a string containing stored procedure parameters
		private static string GetProcedureParameters(DataTable table)
		{
			var sb = new StringBuilder();

			// get Sql server type parameters
			var parms = table.ExtendedProperties[PROCEDURE_PARAMETERS] as List<OleDbParameter>;
			if (parms != null)
			{
				foreach (OleDbParameter parm in parms)
				{
					if (sb.Length > 0)
					{
						sb.Append(", ");
					}
					var value = parm.Value as string;
					sb.AppendFormat("'{0}'", value.Replace("'", "''"));
				}
			}
			return sb.ToString();
		}

		#endregion ** implementation
	}
}