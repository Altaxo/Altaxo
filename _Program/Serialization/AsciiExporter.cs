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

			int nRows = table.RowCount;
			int nColumns = table.ColumnCount;


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
