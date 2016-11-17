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
using Altaxo.Graph.Graph2D.Plot.Styles;
using Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols.Frames;
using Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols.Insets;
using Altaxo.Main;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Graph2D.Plot.Groups
{
	public class ScatterSymbolListManager : StyleListManagerBaseForClasses<ScatterSymbolList, IScatterSymbol, StyleListManagerBaseEntryValue<ScatterSymbolList, IScatterSymbol>>
	{
		public static readonly Main.Properties.PropertyKey<ScatterSymbolListBag> PropertyKeyUserDefinedScatterSymbolLists;

		private static ScatterSymbolListManager _instance;

		public ScatterSymbolList BuiltinSolid { get; private set; }
		public ScatterSymbolList BuiltinOpen { get; private set; }
		public ScatterSymbolList BuiltinDotCenter { get; private set; }
		public ScatterSymbolList BuiltinHollow { get; private set; }

		public ScatterSymbolList OldSolid { get; private set; }
		public ScatterSymbolList OldOpen { get; private set; }
		public ScatterSymbolList OldDotCenter { get; private set; }
		public ScatterSymbolList OldHollow { get; private set; }
		public ScatterSymbolList OldPlus { get; private set; }
		public ScatterSymbolList OldTimes { get; private set; }
		public ScatterSymbolList OldBarHorz { get; private set; }
		public ScatterSymbolList OldBarVert { get; private set; }

		static ScatterSymbolListManager()
		{
			PropertyKeyUserDefinedScatterSymbolLists =
				new Main.Properties.PropertyKey<ScatterSymbolListBag>(
				"304FF675-2250-417A-A0BD-081DAC4947B6",
				"Graph\\UserDefinedScatterSymbolLists",
				Main.Properties.PropertyLevel.Application,
				() => new ScatterSymbolListBag(Enumerable.Empty<ScatterSymbolList>()));

			Instance = new ScatterSymbolListManager();
		}

		protected ScatterSymbolListManager()
			: base(
					(list, level) => new StyleListManagerBaseEntryValue<ScatterSymbolList, IScatterSymbol>(list, level),
					new ScatterSymbolList("BuiltinSolid", new IScatterSymbol[] {
					new Styles.ScatterSymbols.Square(),
					new Styles.ScatterSymbols.Circle(),
					new Styles.ScatterSymbols.UpTriangle(),
					new Styles.ScatterSymbols.Diamond(),
					new Styles.ScatterSymbols.DownTriangle(),
					new Styles.ScatterSymbols.Pentagon(),
					new Styles.ScatterSymbols.LeftTriangle(),
					new Styles.ScatterSymbols.Hexagon(),
					new Styles.ScatterSymbols.RightTriangle(),
			})
					)

		{
			BuiltinSolid = BuiltinDefault;

			ScatterSymbolList dummy;

			BuiltinOpen = new ScatterSymbolList("BuiltinOpen", BuiltinSolid.Select(s => s.WithFillColor(NamedColors.White).WithFrame(new ConstantThicknessFrame()).WithPlotColorInfluence(Styles.ScatterSymbols.PlotColorInfluence.FrameColorFull)));
			InternalTryRegisterList(BuiltinOpen, ItemDefinitionLevel.Builtin, out dummy, false);

			BuiltinHollow = new ScatterSymbolList("BuiltinHollow", BuiltinSolid.Select(s => s.WithFillColor(NamedColors.Transparent).WithFrame(new ConstantThicknessFrame()).WithPlotColorInfluence(Styles.ScatterSymbols.PlotColorInfluence.FrameColorFull)));
			InternalTryRegisterList(BuiltinHollow, ItemDefinitionLevel.Builtin, out dummy, false);

			BuiltinDotCenter = new ScatterSymbolList("BuiltinDotCenter", BuiltinSolid.Select(s => s.WithFillColor(NamedColors.White).WithFrame(new ConstantThicknessFrame()).WithInset(new CirclePointInset()).WithPlotColorInfluence(Styles.ScatterSymbols.PlotColorInfluence.FrameColorFull | Styles.ScatterSymbols.PlotColorInfluence.InsetColorFull)));
			InternalTryRegisterList(BuiltinDotCenter, ItemDefinitionLevel.Builtin, out dummy, false);

			// filled symbols
			OldSolid = new ScatterSymbolList("OldSolid", new IScatterSymbol[] {
					new Styles.ScatterSymbols.Square(),
					new Styles.ScatterSymbols.Circle(),
					new Styles.ScatterSymbols.UpTriangle(),
					new Styles.ScatterSymbols.DownTriangle(),
					new Styles.ScatterSymbols.Diamond(),
					new Styles.ScatterSymbols.CrossPlus(),
					new Styles.ScatterSymbols.CrossTimes(),
					new Styles.ScatterSymbols.Star(),
					new Styles.ScatterSymbols.HorizontalBar(),
					new Styles.ScatterSymbols.VerticalBar(),
			});
			InternalTryRegisterList(OldSolid, ItemDefinitionLevel.Builtin, out dummy, false);

			// open symbols
			OldOpen = new ScatterSymbolList("OldOpen", new IScatterSymbol[] {
					new Styles.ScatterSymbols.Square(NamedColors.White, false).WithFrame(new ConstantThicknessFrame(), true),
					new Styles.ScatterSymbols.Circle(NamedColors.White, false).WithFrame(new ConstantThicknessFrame(), true),
					new Styles.ScatterSymbols.UpTriangle(NamedColors.White, false).WithFrame(new ConstantThicknessFrame(), true),
					new Styles.ScatterSymbols.DownTriangle(NamedColors.White, false).WithFrame(new ConstantThicknessFrame(), true),
					new Styles.ScatterSymbols.Diamond(NamedColors.White, false).WithFrame(new ConstantThicknessFrame(), true),
					new Styles.ScatterSymbols.CrossPlus(NamedColors.Black, true),
					new Styles.ScatterSymbols.CrossTimes(NamedColors.Black, true),
					new Styles.ScatterSymbols.Star(NamedColors.Black, true),
					new Styles.ScatterSymbols.HorizontalBar(NamedColors.Black, true),
					new Styles.ScatterSymbols.VerticalBar(NamedColors.Black, true),
			});
			InternalTryRegisterList(OldOpen, ItemDefinitionLevel.Builtin, out dummy, false);

			// hollow symbols
			OldHollow = new ScatterSymbolList("OldHollow", new IScatterSymbol[] {
					new Styles.ScatterSymbols.Square(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true),
					new Styles.ScatterSymbols.Circle(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true),
					new Styles.ScatterSymbols.UpTriangle(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true),
					new Styles.ScatterSymbols.DownTriangle(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true),
					new Styles.ScatterSymbols.Diamond(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true),
					new Styles.ScatterSymbols.CrossPlus(NamedColors.Black, true),
					new Styles.ScatterSymbols.CrossTimes(NamedColors.Black, true),
					new Styles.ScatterSymbols.Star(NamedColors.Black, true),
					new Styles.ScatterSymbols.HorizontalBar(NamedColors.Black, true),
					new Styles.ScatterSymbols.VerticalBar(NamedColors.Black, true),
			});
			InternalTryRegisterList(OldHollow, ItemDefinitionLevel.Builtin, out dummy, false);

			// old dotcenter symbols
			OldDotCenter = new ScatterSymbolList("OldDotCenter", new IScatterSymbol[] {
					new Styles.ScatterSymbols.Square(NamedColors.White, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new CirclePointInset(), true),
					new Styles.ScatterSymbols.Circle(NamedColors.White, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new CirclePointInset(), true),
					new Styles.ScatterSymbols.UpTriangle(NamedColors.White, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new CirclePointInset(), true),
					new Styles.ScatterSymbols.DownTriangle(NamedColors.White, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new CirclePointInset(), true),
					new Styles.ScatterSymbols.Diamond(NamedColors.White, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new CirclePointInset(), true),
					new Styles.ScatterSymbols.CrossPlus(NamedColors.Black, true),
					new Styles.ScatterSymbols.CrossTimes(NamedColors.Black, true),
					new Styles.ScatterSymbols.Star(NamedColors.Black, true),
					new Styles.ScatterSymbols.HorizontalBar(NamedColors.Black, true),
					new Styles.ScatterSymbols.VerticalBar(NamedColors.Black, true),
			});
			InternalTryRegisterList(OldDotCenter, ItemDefinitionLevel.Builtin, out dummy, false);

			// old plus symbols
			OldPlus = new ScatterSymbolList("OldCrossPlus", new IScatterSymbol[] {
					new Styles.ScatterSymbols.Square(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new CrossPlusInset(), true),
					new Styles.ScatterSymbols.Circle(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new CrossPlusInset(), true),
					new Styles.ScatterSymbols.UpTriangle(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new CrossPlusInset(), true),
					new Styles.ScatterSymbols.DownTriangle(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new CrossPlusInset(), true),
					new Styles.ScatterSymbols.Diamond(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new CrossPlusInset(), true),
					new Styles.ScatterSymbols.CrossPlus(NamedColors.Black, true),
					new Styles.ScatterSymbols.CrossTimes(NamedColors.Black, true),
					new Styles.ScatterSymbols.Star(NamedColors.Black, true),
					new Styles.ScatterSymbols.HorizontalBar(NamedColors.Black, true),
					new Styles.ScatterSymbols.VerticalBar(NamedColors.Black, true),
	});
			InternalTryRegisterList(OldPlus, ItemDefinitionLevel.Builtin, out dummy, false);

			// old times symbols
			OldTimes = new ScatterSymbolList("OldCrossTimes", new IScatterSymbol[] {
					new Styles.ScatterSymbols.Square(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new CrossTimesInset(), true),
					new Styles.ScatterSymbols.Circle(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new CrossTimesInset(), true),
					new Styles.ScatterSymbols.UpTriangle(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new CrossTimesInset(), true),
					new Styles.ScatterSymbols.DownTriangle(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new CrossTimesInset(), true),
					new Styles.ScatterSymbols.Diamond(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new CrossTimesInset(), true),
					new Styles.ScatterSymbols.CrossPlus(NamedColors.Black, true),
					new Styles.ScatterSymbols.CrossTimes(NamedColors.Black, true),
					new Styles.ScatterSymbols.Star(NamedColors.Black, true),
					new Styles.ScatterSymbols.HorizontalBar(NamedColors.Black, true),
					new Styles.ScatterSymbols.VerticalBar(NamedColors.Black, true),
	});
			InternalTryRegisterList(OldTimes, ItemDefinitionLevel.Builtin, out dummy, false);

			OldBarHorz = new ScatterSymbolList("OldBarHorzizontal", new IScatterSymbol[] {
					new Styles.ScatterSymbols.Square(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new HorizontalBarInset(), true),
					new Styles.ScatterSymbols.Circle(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new HorizontalBarInset(), true),
					new Styles.ScatterSymbols.UpTriangle(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new HorizontalBarInset(), true),
					new Styles.ScatterSymbols.DownTriangle(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new HorizontalBarInset(), true),
					new Styles.ScatterSymbols.Diamond(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new HorizontalBarInset(), true),
					new Styles.ScatterSymbols.CrossPlus(NamedColors.Black, true),
					new Styles.ScatterSymbols.CrossTimes(NamedColors.Black, true),
					new Styles.ScatterSymbols.Star(NamedColors.Black, true),
					new Styles.ScatterSymbols.HorizontalBar(NamedColors.Black, true),
					new Styles.ScatterSymbols.VerticalBar(NamedColors.Black, true),
	});
			InternalTryRegisterList(OldBarHorz, ItemDefinitionLevel.Builtin, out dummy, false);

			OldBarVert = new ScatterSymbolList("OldBarVertical", new IScatterSymbol[] {
					new Styles.ScatterSymbols.Square(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new HorizontalBarInset(), true),
					new Styles.ScatterSymbols.Circle(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new HorizontalBarInset(), true),
					new Styles.ScatterSymbols.UpTriangle(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new HorizontalBarInset(), true),
					new Styles.ScatterSymbols.DownTriangle(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new HorizontalBarInset(), true),
					new Styles.ScatterSymbols.Diamond(NamedColors.Transparent, false).WithFrame(new ConstantThicknessFrame(), true).WithInset(new HorizontalBarInset(), true),
					new Styles.ScatterSymbols.CrossPlus(NamedColors.Black, true),
					new Styles.ScatterSymbols.CrossTimes(NamedColors.Black, true),
					new Styles.ScatterSymbols.Star(NamedColors.Black, true),
					new Styles.ScatterSymbols.HorizontalBar(NamedColors.Black, true),
					new Styles.ScatterSymbols.VerticalBar(NamedColors.Black, true),
	});
			InternalTryRegisterList(OldBarVert, ItemDefinitionLevel.Builtin, out dummy, false);

			ScatterSymbolListBag userStyleLists;
			Current.PropertyService.UserSettings.TryGetValue(PropertyKeyUserDefinedScatterSymbolLists, out userStyleLists);
			if (null != userStyleLists)
			{
				foreach (var listVar in userStyleLists.StyleLists)
				{
					InternalTryRegisterList(listVar, ItemDefinitionLevel.UserDefined, out dummy, false);
				}
			}

			RebuildListEntryToListDictionary();
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

		#region User defined lists

		protected override void OnUserDefinedListAddedChangedRemoved(ScatterSymbolList list)
		{
			var listBag = new ScatterSymbolListBag(_allLists.Values.Where(entry => entry.Level == ItemDefinitionLevel.UserDefined).Select(entry => entry.List));
			Current.PropertyService.UserSettings.SetValue(PropertyKeyUserDefinedScatterSymbolLists, listBag);
		}

		#endregion User defined lists
	}
}