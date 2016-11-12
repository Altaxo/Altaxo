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
using Altaxo.Graph.Graph2D.Plot.Styles;
using Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols;
using Altaxo.Gui;
using Altaxo.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Graph.Graph2D.Plot.Styles
{
	public interface IScatterSymbolView
	{
		SelectableListNodeList ShapeChoices { set; }

		SelectableListNodeList FrameChoices { set; }

		SelectableListNodeList InsetChoices { set; }

		double RelativeStructureWidth { get; set; }

		PlotColorInfluence PlotColorInfluence { get; set; }

		NamedColor FillColor { get; set; }

		NamedColor FrameColor { get; set; }

		NamedColor InsetColor { get; set; }

		IScatterSymbol ScatterSymbolForPreview { set; }

		event Action<double> RelativeStructureWidthChanged;

		event Action<Type> ShapeChanged;

		event Action<Type> FrameChanged;

		event Action<Type> InsetChanged;

		event Action<PlotColorInfluence> PlotColorInfluenceChanged;

		event Action<NamedColor> FillColorChanged;

		event Action<NamedColor> FrameColorChanged;

		event Action<NamedColor> InsetColorChanged;
	}

	[UserControllerForObject(typeof(IScatterSymbol))]
	[ExpectedTypeOfView(typeof(IScatterSymbolView))]
	public class ScatterSymbolController : MVCANControllerEditOriginalDocBase<IScatterSymbol, IScatterSymbolView>
	{
		private SelectableListNodeList _shapes, _frames, _insets;

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break;
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

				_shapes = new SelectableListNodeList(shapes.Select(tuple => new SelectableListNode(tuple.Item1, tuple.Item2, _doc.GetType() == tuple.Item2)));

				_frames = new SelectableListNodeList(frames.Select(tuple => new SelectableListNode(tuple.Item1, tuple.Item2, _doc.Frame?.GetType() == tuple.Item2)));
				_frames.Insert(0, new SelectableListNode("None", null, _doc.Frame == null));

				_insets = new SelectableListNodeList(insets.Select(tuple => new SelectableListNode(tuple.Item1, tuple.Item2, _doc.Inset?.GetType() == tuple.Item2)));
				_insets.Insert(0, new SelectableListNode("None", null, _doc.Inset == null));
			}

			if (null != _view)
			{
				_view.ShapeChoices = _shapes;
				_view.FrameChoices = _frames;
				_view.InsetChoices = _insets;

				_view.RelativeStructureWidth = _doc.RelativeStructureWidth;
				_view.PlotColorInfluence = _doc.PlotColorInfluence;
				_view.FillColor = _doc.FillColor;
				_view.FrameColor = _doc.Frame?.Color ?? NamedColors.Transparent;
				_view.InsetColor = _doc.Inset?.Color ?? NamedColors.Transparent;

				_view.ScatterSymbolForPreview = _doc;
			}
		}

		public override bool Apply(bool disposeController)
		{
			return ApplyEnd(true, disposeController);
		}

		protected override void AttachView()
		{
			base.AttachView();

			_view.RelativeStructureWidthChanged += EhRelativeStructureWidthChanged;

			_view.ShapeChanged += EhShapeChanged;

			_view.FrameChanged += EhFrameChanged;

			_view.InsetChanged += EhInsetChanged;

			_view.PlotColorInfluenceChanged += EhPlotColorInfluenceChanged;

			_view.FillColorChanged += EhFillColorChanged;

			_view.FrameColorChanged += EhFrameColorChanged;

			_view.InsetColorChanged += EhInsetColorChanged;
		}

		protected override void DetachView()
		{
			_view.RelativeStructureWidthChanged -= EhRelativeStructureWidthChanged;

			_view.ShapeChanged -= EhShapeChanged;

			_view.FrameChanged -= EhFrameChanged;

			_view.InsetChanged -= EhInsetChanged;

			_view.PlotColorInfluenceChanged -= EhPlotColorInfluenceChanged;

			_view.FillColorChanged -= EhFillColorChanged;

			_view.FrameColorChanged -= EhFrameColorChanged;

			_view.InsetColorChanged -= EhInsetColorChanged;

			base.DetachView();
		}

		private void EhInsetColorChanged(NamedColor obj)
		{
			if (null != _doc.Inset)
			{
				_doc = _doc.WithInset(_doc.Inset.WithColor(obj));
				_view.ScatterSymbolForPreview = _doc;
			}
		}

		private void EhFrameColorChanged(NamedColor obj)
		{
			if (null != _doc.Frame)
			{
				_doc = _doc.WithFrame(_doc.Frame.WithColor(obj));
				_view.ScatterSymbolForPreview = _doc;
			}
		}

		private void EhFillColorChanged(NamedColor obj)
		{
			_doc = _doc.WithFillColor(obj);
			_view.ScatterSymbolForPreview = _doc;
		}

		private void EhPlotColorInfluenceChanged(PlotColorInfluence obj)
		{
			_doc = _doc.WithPlotColorInfluence(obj);
			_view.ScatterSymbolForPreview = _doc;
		}

		private void EhInsetChanged(Type obj)
		{
			if (_doc.Inset?.GetType() == obj)
				return;

			var inset = obj == null ? null : (IScatterSymbolInset)Activator.CreateInstance(obj);
			if (null != inset && null != _doc.Inset)
				inset = inset.WithColor(_doc.Inset.Color);

			_doc = _doc.WithInset(inset);

			// Update Gui
			_insets.ForEachDo(node => node.IsSelected = _doc.Inset?.GetType() == (Type)node.Tag);
			_view.InsetChoices = _insets;
			if (null != _doc.Inset) _view.InsetColor = _doc.Inset.Color;

			_view.ScatterSymbolForPreview = _doc;
		}

		private void EhFrameChanged(Type obj)
		{
			if (_doc.Frame?.GetType() == obj)
				return;

			var frame = obj == null ? null : (IScatterSymbolFrame)Activator.CreateInstance(obj);
			if (null != frame && null != _doc.Frame)
				frame = frame.WithColor(_doc.Frame.Color);

			_doc = _doc.WithFrame(frame);

			_frames.ForEachDo(node => node.IsSelected = _doc.Frame?.GetType() == (Type)node.Tag);
			_view.FrameChoices = _frames;

			if (null != _doc.Frame)
				_view.FrameColor = _doc.Frame.Color;

			_view.ScatterSymbolForPreview = _doc;
		}

		private void EhShapeChanged(Type obj)
		{
			var newItem = (IScatterSymbol)Activator.CreateInstance(obj);
			newItem = newItem
				.WithRelativeStructureWidth(_doc.RelativeStructureWidth)
				.WithPlotColorInfluence(_doc.PlotColorInfluence)
				.WithFillColor(_doc.FillColor);

			if (_doc.Frame != null)
				newItem = newItem.WithFrame(_doc.Frame);
			if (_doc.Inset != null)
				newItem = newItem.WithInset(_doc.Inset);

			_doc = newItem;

			// update all Gui controls
			_view.RelativeStructureWidth = _doc.RelativeStructureWidth;
			_view.FillColor = _doc.FillColor;
			_frames.ForEachDo(node => node.IsSelected = _doc.Frame?.GetType() == (Type)node.Tag);
			_view.FrameChoices = _frames;
			_insets.ForEachDo(node => node.IsSelected = _doc.Inset?.GetType() == (Type)node.Tag);
			_view.InsetChoices = _insets;
			_view.PlotColorInfluence = _doc.PlotColorInfluence;

			if (_doc.Frame != null) _view.FrameColor = _doc.Frame.Color;
			if (_doc.Inset != null) _view.InsetColor = _doc.Inset.Color;

			_view.ScatterSymbolForPreview = _doc;
		}

		private void EhRelativeStructureWidthChanged(double obj)
		{
			_doc = _doc.WithRelativeStructureWidth(obj);
			_view.ScatterSymbolForPreview = _doc;
		}
	}
}