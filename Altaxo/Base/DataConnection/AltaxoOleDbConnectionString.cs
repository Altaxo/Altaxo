using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.DataConnection
{
	/// <summary>
	/// Contains an connection string (with or without password,
	/// </summary>
	public class AltaxoOleDbConnectionString : Main.IImmutable
	{
		private const string UserIDKey = "User ID";
		private const string PasswordKey = "Password";
		private static AltaxoOleDbConnectionString _emptyInstance = new AltaxoOleDbConnectionString(null, null);

		private string _originalConnectionString;
		private string _connectionStringWithCredentials;

		public AltaxoOleDbConnectionString(string originalConnectionString, LoginCredentials credentials)
		{
			_originalConnectionString = originalConnectionString;

			if (null != credentials && !credentials.AreEmpty)
			{
				var connBuilder = new System.Data.OleDb.OleDbConnectionStringBuilder(_originalConnectionString);
				connBuilder[UserIDKey] = credentials.UserName;
				connBuilder[PasswordKey] = credentials.Password;
				_connectionStringWithCredentials = connBuilder.ConnectionString;
			}
			else
			{
				_connectionStringWithCredentials = originalConnectionString;
			}
		}

		public bool IsEmpty { get { return string.IsNullOrEmpty(_connectionStringWithCredentials); } }

		public static AltaxoOleDbConnectionString Empty { get { return _emptyInstance; } }

		public string OriginalConnectionString { get { return _originalConnectionString; } }

		public string ConnectionStringWithTemporaryCredentials { get { return this._connectionStringWithCredentials; } }

		public LoginCredentials GetCredentials()
		{
			var connBuilder = new System.Data.OleDb.OleDbConnectionStringBuilder(_originalConnectionString);
			string username = null, password = null;
			if (connBuilder.ContainsKey(UserIDKey))
				username = connBuilder[UserIDKey] as string;
			if (connBuilder.ContainsKey(PasswordKey))
				password = connBuilder[PasswordKey] as String;
			return new LoginCredentials(username, password);
		}

		public override bool Equals(object obj)
		{
			var from = obj as AltaxoOleDbConnectionString;
			if (null == from)
				return false;
			else
				return _originalConnectionString == from._originalConnectionString && _connectionStringWithCredentials == from._connectionStringWithCredentials;
		}

		public override string ToString()
		{
			return _originalConnectionString;
		}

		public override int GetHashCode()
		{
			return _connectionStringWithCredentials.GetHashCode();
		}

		public static bool operator ==(AltaxoOleDbConnectionString x, AltaxoOleDbConnectionString y)
		{
			if (System.Object.ReferenceEquals(x, y))
				return true;

			if ((null == (object)x) || (null == (object)y))
				return false;

			return x._originalConnectionString == y._originalConnectionString && x._connectionStringWithCredentials == y._connectionStringWithCredentials;
		}

		public static bool operator !=(AltaxoOleDbConnectionString x, AltaxoOleDbConnectionString y)
		{
			return !(x == y);
		}
	}
}