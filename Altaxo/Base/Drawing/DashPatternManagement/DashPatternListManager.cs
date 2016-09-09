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

using Altaxo.Drawing;
using Altaxo.Drawing.D3D;
using Altaxo.Graph;
using Altaxo.Graph.Graph3D.Plot.Styles;
using Altaxo.Main;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Drawing.DashPatternManagement
{
	public class DashPatternListManager : StyleListManagerBaseForClasses<DashPatternList, IDashPattern, StyleListManagerBaseEntryValue<DashPatternList, IDashPattern>>
	{
		public static readonly Main.Properties.PropertyKey<DashPatternListBag> PropertyKeyUserDefinedDashPatternLists;

		private static DashPatternListManager _instance;

		static DashPatternListManager()
		{
			PropertyKeyUserDefinedDashPatternLists =
				new Main.Properties.PropertyKey<DashPatternListBag>(
				"6C8F87E2-F80A-458E-A5C5-DFF92EBDBA90",
				"Graph3D\\UserDefinedDashPatternLists",
				Main.Properties.PropertyLevel.Application,
				() => new DashPatternListBag(Enumerable.Empty<DashPatternList>()));

			Instance = new DashPatternListManager();
		}

		protected DashPatternListManager()
			: base(
					(list, level) => new StyleListManagerBaseEntryValue<DashPatternList, IDashPattern>(list, level),
					new DashPatternList("BuiltinDefault", new IDashPattern[] {
					new Drawing.DashPatterns.Solid(),
					new Drawing.DashPatterns.Dash(),
					new Drawing.DashPatterns.Dot(),
					new Drawing.DashPatterns.DashDot(),
					new Drawing.DashPatterns.DashDotDot(),
			})
					)

		{
			DashPatternListBag userStyleLists;
			Current.PropertyService.UserSettings.TryGetValue(PropertyKeyUserDefinedDashPatternLists, out userStyleLists);
			if (null != userStyleLists)
			{
				DashPatternList dummy;
				foreach (var list in userStyleLists.StyleLists)
				{
					InternalTryRegisterList(list, ItemDefinitionLevel.UserDefined, out dummy, false);
				}
			}

			RebuildListEntryToListDictionary();
		}

		/// <summary>
		/// Gets the buildin default solid dash pattern belonging to the BuildinDefault list.
		/// </summary>
		/// <value>
		/// The buildin default solid dash pattern belonging to the BuildinDefault list.
		/// </value>
		public IDashPattern BuiltinDefaultSolid { get { return this.BuiltinDefault[0]; } }

		public static DashPatternListManager Instance
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

		public override DashPatternList CreateNewList(string name, IEnumerable<IDashPattern> symbols)
		{
			return new DashPatternList(name, symbols);
		}

		protected override void OnUserDefinedListAddedChangedRemoved(DashPatternList list)
		{
			var listBag = new DashPatternListBag(_allLists.Values.Where(entry => entry.Level == ItemDefinitionLevel.UserDefined).Select(entry => entry.List));
			Current.PropertyService.UserSettings.SetValue(PropertyKeyUserDefinedDashPatternLists, listBag);
		}
	}
}