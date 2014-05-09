using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Altaxo.Gui.Data
{
	// see http://weblogs.asp.net/jgalloway/archive/2006/10/29/Showing-a-Connection-String-prompt-in-a-WinForm-application.aspx
	// or http://www.codeproject.com/Articles/6080/Using-DataLinks-to-get-or-edit-a-connection-string
	// and http://blogs.msdn.com/b/vsdata/archive/2010/02/02/data-connection-dialog-source-code-is-released-on-code-gallery.aspx

	public interface IDataBaseConnectionView
	{
	}

	public class DataBaseConnectionController
	{
		public static string PromptConnectionString()
		{
			Type type = Type.GetTypeFromProgID("DataLinks");
			object links = Activator.CreateInstance(type);

			// the next statement returns an ADODB._Connection object, for members of this object see here: http://www.w3schools.com/ado/ado_ref_connection.asp
			object str = type.InvokeMember("PromptNew", BindingFlags.InvokeMethod, null, links, null);
			string s = str.GetType().InvokeMember("ConnectionString", BindingFlags.GetProperty, null, str, new object[0]) as string;

			if (!string.IsNullOrEmpty(s))
			{
				var oleCon = new OleDbConnection((string)s);
				oleCon.Open();
				var schema = oleCon.GetSchema();

				// get a table with all tables
				var schemaTable = oleCon.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });

				// get all columns of table tRaumtemperatur
				var columnTable = oleCon.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, "tRaumtemperatur" });


				foreach (System.Data.DataRow row in schemaTable.Rows)
				{
					Console.WriteLine(row[2]);
				}

			//	ReadData(s, "SELECT * FROM dbo.tRaumtemperatur");

				
				oleCon.Close();
			}
			return s;
		}

		public static void ReadData(string connectionString, string queryString)
		{
			using (OleDbConnection connection = new OleDbConnection(connectionString))
			{
				OleDbCommand command = new OleDbCommand(queryString, connection);

				connection.Open();
				OleDbDataReader reader = command.ExecuteReader();

				while (reader.Read())
				{
					Console.WriteLine(reader[0].ToString());
				}
				reader.Close();
			}
		}
	}
}