#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

using Markdig.Extensions.Tables;

namespace Altaxo.Text.Renderers.Maml.Extensions
{
	/// <summary>
	/// Maml renderer for a <see cref="Table"/>.
	/// </summary>
	/// <seealso cref="MamlObjectRenderer{Table}" />
	public class TableRenderer : MamlObjectRenderer<Table>
	{
		protected override void Write(MamlRenderer renderer, Table table)
		{
			renderer.Push(MamlElements.table);
			renderer.Push(MamlElements.tableHeader);

			foreach (TableRow row in table)
			{
				if (row.IsHeader)
				{
					renderer.Push(MamlElements.row);

					foreach (TableCell cell in row)
					{
						renderer.Push(MamlElements.entry);
						renderer.Write(cell);
						renderer.PopTo(MamlElements.entry);
					}

					renderer.PopTo(MamlElements.row);
				}
			}

			renderer.PopTo(MamlElements.tableHeader);

			foreach (TableRow row in table)
			{
				if (!row.IsHeader)
				{
					renderer.Push(MamlElements.row);

					foreach (TableCell cell in row)
					{
						renderer.Push(MamlElements.entry);
						renderer.Write(cell);
						renderer.PopTo(MamlElements.entry);
					}

					renderer.PopTo(MamlElements.row);
				}
			}

			renderer.PopTo(MamlElements.table);
		}
	}
}
