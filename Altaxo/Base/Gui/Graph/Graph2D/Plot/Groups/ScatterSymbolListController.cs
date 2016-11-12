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

using Altaxo.Collections;
using Altaxo.Drawing;
using Altaxo.Graph.Graph2D.Plot.Groups;
using Altaxo.Graph.Graph2D.Plot.Styles;
using Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols;
using Altaxo.Gui.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Graph.Graph2D.Plot.Groups
{
	public interface IScatterSymbolListView : IStyleListView
	{
		SelectableListNodeList ShapeChoices { set; }

		SelectableListNodeList FrameChoices { set; }
		SelectableListNodeList InsetChoices { set; }

		event Action<double> StructureWithForAllSelected;

		event Action<Type> ShapeForAllSelected;

		event Action<Type> FrameForAllSelected;

		event Action<Type> InsetForAllSelected;

		event Action<PlotColorInfluence> PlotColorInfluenceForAllSelected;

		event Action<NamedColor> FillColorForAllSelected;

		event Action<NamedColor> FrameColorForAllSelected;

		event Action<NamedColor> InsetColorForAllSelected;
	}

	[ExpectedTypeOfView(typeof(IScatterSymbolListView))]
	[UserControllerForObject(typeof(ScatterSymbolList))]
	public class ScatterSymbolListController : StyleListController<ScatterSymbolListManager, ScatterSymbolList, IScatterSymbol>
	{
		private SelectableListNodeList _shapes, _frames, _insets;

		private IScatterSymbolListView _view1;

		public ScatterSymbolListController()
			: base(ScatterSymbolListManager.Instance)
		{
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				var shapes = new List<Tuple<string, Type>>(Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IScatterSymbol)).Select(t => new Tuple<string, Type>(t.Name, t)));
				var frames = new List<Tuple<string, Type>>(Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IScatterSymbolFrame)).Select(t => new Tuple<string, Type>(t.Name, t)));
				var insets = new List<Tuple<string, Type>>(Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IScatterSymbolInset)).Select(t => new Tuple<string, Type>(t.Name, t)));

				shapes.Sort((x, y) => string.Compare(x.Item1, y.Item1));
				frames.Sort((x, y) => string.Compare(x.Item1, y.Item1));
				insets.Sort((x, y) => string.Compare(x.Item1, y.Item1));

				_shapes = new SelectableListNodeList(shapes.Select(tuple => new SelectableListNode(tuple.Item1, tuple.Item2, false)));
				_shapes[0].IsSelected = true;
				_frames = new SelectableListNodeList(frames.Select(tuple => new SelectableListNode(tuple.Item1, tuple.Item2, false)));
				_frames.Insert(0, new SelectableListNode("None", null, true));
				_insets = new SelectableListNodeList(insets.Select(tuple => new SelectableListNode(tuple.Item1, tuple.Item2, false)));
				_insets.Insert(0, new SelectableListNode("None", null, true));
			}

			if (null != _view)
			{
				_view1 = _view as IScatterSymbolListView;
				_view1.ShapeChoices = _shapes;
				_view1.FrameChoices = _frames;
				_view1.InsetChoices = _insets;
			}
		}

		protected override void AttachView()
		{
			base.AttachView();

			_view1.StructureWithForAllSelected += EhStructureWithForAllSelected;

			_view1.ShapeForAllSelected += EhShapeForAllSelected;

			_view1.FrameForAllSelected += EhFrameForAllSelected;

			_view1.InsetForAllSelected += EhInsetForAllSelected;

			_view1.PlotColorInfluenceForAllSelected += EhPlotColorInfluenceForAllSelected;

			_view1.FillColorForAllSelected += EhFillColorForAllSelected;

			_view1.FrameColorForAllSelected += EhFrameColorForAllSelected;

			_view1.InsetColorForAllSelected += EhInsetColorForAllSelected;
		}

		protected override void DetachView()
		{
			_view1.StructureWithForAllSelected -= EhStructureWithForAllSelected;

			_view1.ShapeForAllSelected -= EhShapeForAllSelected;

			_view1.FrameForAllSelected -= EhFrameForAllSelected;

			_view1.InsetForAllSelected -= EhInsetForAllSelected;

			_view1.PlotColorInfluenceForAllSelected -= EhPlotColorInfluenceForAllSelected;

			_view1.FillColorForAllSelected -= EhFillColorForAllSelected;

			_view1.FrameColorForAllSelected -= EhFrameColorForAllSelected;

			_view1.InsetColorForAllSelected -= EhInsetColorForAllSelected;

			base.DetachView();
		}

		private void EhInsetColorForAllSelected(NamedColor obj)
		{
			foreach (var node in _currentItems)
			{
				if (node.IsSelected)
				{
					var item = (IScatterSymbol)node.Tag;
					if (item.Inset != null)
						node.Tag = item.WithInset(item.Inset.WithColor(obj));
				}
			}

			View_CurrentItems_Initialize();
			SetListDirty();
		}

		private void EhFrameColorForAllSelected(NamedColor obj)
		{
			foreach (var node in _currentItems)
			{
				if (node.IsSelected)
				{
					var item = (IScatterSymbol)node.Tag;
					if (item.Frame != null)
						node.Tag = item.WithFrame(item.Frame.WithColor(obj));
				}
			}

			View_CurrentItems_Initialize();
			SetListDirty();
		}

		private void EhFillColorForAllSelected(NamedColor obj)
		{
			foreach (var node in _currentItems)
			{
				if (node.IsSelected)
				{
					var item = (IScatterSymbol)node.Tag;
					node.Tag = item.WithFillColor(obj);
				}
			}

			View_CurrentItems_Initialize();
			SetListDirty();
		}

		private void EhPlotColorInfluenceForAllSelected(PlotColorInfluence obj)
		{
			foreach (var node in _currentItems)
			{
				if (node.IsSelected)
				{
					var item = (IScatterSymbol)node.Tag;
					node.Tag = item.WithPlotColorInfluence(obj);
				}
			}

			View_CurrentItems_Initialize();
			SetListDirty();
		}

		private void EhInsetForAllSelected(Type obj)
		{
			var insetTemplate = obj == null ? null : (IScatterSymbolInset)Activator.CreateInstance(obj);

			if (null == insetTemplate)
			{
				foreach (var node in _currentItems)
				{
					if (node.IsSelected)
					{
						var item = (IScatterSymbol)node.Tag;
						node.Tag = item.WithInset(null);
					}
				}
			}
			else
			{
				foreach (var node in _currentItems)
				{
					if (node.IsSelected)
					{
						var item = (IScatterSymbol)node.Tag;
						node.Tag = item = item.WithInset(item.Inset == null ? insetTemplate : insetTemplate.WithColor(item.Inset.Color));
					}
				}
			}

			View_CurrentItems_Initialize();
			SetListDirty();
		}

		private void EhFrameForAllSelected(Type obj)
		{
			var frameTemplate = obj == null ? null : (IScatterSymbolFrame)Activator.CreateInstance(obj);

			if (null == frameTemplate)
			{
				foreach (var node in _currentItems)
				{
					if (node.IsSelected)
					{
						var item = (IScatterSymbol)node.Tag;
						node.Tag = item.WithFrame(null);
					}
				}
			}
			else
			{
				foreach (var node in _currentItems)
				{
					if (node.IsSelected)
					{
						var item = (IScatterSymbol)node.Tag;
						node.Tag = item = item.WithFrame(item.Frame == null ? frameTemplate : frameTemplate.WithColor(item.Frame.Color));
					}
				}
			}

			View_CurrentItems_Initialize();
			SetListDirty();
		}

		private void EhShapeForAllSelected(Type obj)
		{
			var shapeTemplate = (IScatterSymbol)Activator.CreateInstance(obj);

			foreach (var node in _currentItems)
			{
				if (node.IsSelected)
				{
					var item = (IScatterSymbol)node.Tag;

					var newItem = shapeTemplate;
					newItem = newItem
						.WithRelativeStructureWidth(item.RelativeStructureWidth)
						.WithPlotColorInfluence(item.PlotColorInfluence)
						.WithFillColor(item.FillColor);

					if (item.Frame != null)
						newItem = newItem.WithFrame(item.Frame);
					if (item.Inset != null)
						newItem = newItem.WithInset(item.Inset);

					node.Tag = newItem;
					node.Text = ToDisplayName(newItem);
				}
			}
			View_CurrentItems_Initialize();
			SetListDirty();
		}

		private void EhStructureWithForAllSelected(double obj)
		{
			foreach (var node in _currentItems)
			{
				if (node.IsSelected)
				{
					var item = (IScatterSymbol)node.Tag;
					node.Tag = item.WithRelativeStructureWidth(obj);
				}
			}
			View_CurrentItems_Initialize();
			SetListDirty();
		}
	}
}