#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

using Altaxo.Graph.Graph3D.Plot.Styles;
using Altaxo.Main;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Graph3D.Plot.Groups
{
	public class ScatterSymbolListManager : StyleListManagerBase<ScatterSymbolList, IScatterSymbol, StyleListManagerBaseEntryValue<ScatterSymbolList, IScatterSymbol>>
	{
		private static ScatterSymbolListManager _instance;

		static ScatterSymbolListManager()
		{
			Instance = new ScatterSymbolListManager();
		}

		protected ScatterSymbolListManager()
			: base(
					(list, level) => new StyleListManagerBaseEntryValue<ScatterSymbolList, IScatterSymbol>(list, level),
					new ScatterSymbolList("BuiltinDefault", new IScatterSymbol[] {
				new Styles.ScatterSymbols.Cube(),
					new Styles.ScatterSymbols.Sphere(),
					new Styles.ScatterSymbols.TetrahedronUp(),
					new Styles.ScatterSymbols.TetrahedronDown()
			})
					)

		{
		}

		public static ScatterSymbolListManager Instance
		{
			get
			{
				return _instance;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException(nameof(value));

				if (null != _instance)
					Current.ProjectService.ProjectClosed -= _instance.EhProjectClosed;

				_instance = value;

				if (null != _instance)
					Current.ProjectService.ProjectClosed += _instance.EhProjectClosed;
			}
		}

		public override ScatterSymbolList CreateNewList(string name, IEnumerable<IScatterSymbol> symbols)
		{
			return new ScatterSymbolList(name, symbols);
		}
	}
}