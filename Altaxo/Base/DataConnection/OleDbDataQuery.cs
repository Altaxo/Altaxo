using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.DataConnection
{
	public class OleDbDataQuery : Main.ICopyFrom
	{
		protected string _connectionString;
		protected string _selectionlStatement;

		public OleDbDataQuery(string connectionString, string sqlStatement)
		{
			_connectionString = connectionString;
			_selectionlStatement = sqlStatement;
		}

		protected OleDbDataQuery()
		{
		}

		public virtual bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			var from = obj as OleDbDataQuery;
			if (null != from)
			{
				this._connectionString = from._connectionString;
				this._selectionlStatement = from._selectionlStatement;
				return true;
			}
			return false;
		}

		public virtual object Clone()
		{
			var result = new OleDbDataQuery();
			result.CopyFrom(this);
			return result;
		}

		public string SelectionStatement
		{
			get
			{
				return _selectionlStatement;
			}
		}

		public string ConnectionString
		{
			get
			{
				return _connectionString;
			}
		}

		public void ReadDataFromOleDbConnection(Action<System.Data.Common.DbDataReader> readAction)
		{
			using (var connection = new System.Data.OleDb.OleDbConnection(_connectionString))
			{
				using (var command = new System.Data.OleDb.OleDbCommand(_selectionlStatement, connection))
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