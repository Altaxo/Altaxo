/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002 Dr. Dirk Lellinger
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

using System;
using System.IO;

namespace Altaxo.Serialization
{
	/// <summary>
	/// AsciiExporter provides some static methods to export tables or columns to ascii files
	/// </summary>
	public class AsciiExporter
	{
		public AsciiExporter()
		{
		}

					
		

			
			static public void ExportAscii(System.IO.Stream myStream, Altaxo.Data.DataTable table, char separator)
			{
				StreamWriter strwr = new StreamWriter(myStream);


			// export the table using tabulator as separator

			int nRows = table.DataColumns.RowCount;
			int nColumns = table.DataColumns.ColumnCount;


			for(int i=0;i<nRows;i++)
			{
				for(int j=0;j<nColumns;j++)
				{
					if(!table[j].IsElementEmpty(i))
						strwr.Write(table[j][i].ToString());

					if((j+1)<nColumns)
						strwr.Write(separator);
					else 
						strwr.WriteLine();
				}
			}
			strwr.Close();
		}


		static public void ExportAscii(string filename, Altaxo.Data.DataTable table, char separator)
		{

			System.IO.FileStream myStream = new System.IO.FileStream(filename,System.IO.FileMode.Create);

			ExportAscii(myStream,table,separator);
		}

	}
}
