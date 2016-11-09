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
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Graph2D.Plot.Groups;
using Altaxo.Graph.Graph2D.Plot.Styles;
using Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols;
using Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols.Frames;
using Altaxo.Gui.Graph.Plot.Groups;
using System;
using System.Collections.Generic;

namespace Altaxo.Gui.Graph.Gdi.Plot.Styles
{
	#region Interfaces

	/// <summary>
	/// This view interface is for showing the options of the XYXYPlotScatterStyle
	/// </summary>
	public interface IScatterPlotStyleView
	{
		/// <summary>
		/// Material for the scatter symbol.
		/// </summary>
		NamedColor Color { get; set; }

		/// <summary>
		/// Indicates, whether only colors of plot color sets should be shown.
		/// </summary>
		bool ShowPlotColorsOnly { set; }

		/// <summary>
		/// Initializes the symbol size combobox.
		/// </summary>
		double SymbolSize { get; set; }

		/// <summary>
		/// Initializes the independent symbol size check box.
		/// </summary>
		bool IndependentSymbolSize { get; set; }

		bool IndependentScatterSymbol { get; set; }

		/// <summary>
		/// Initializes the symbol shape combobox.
		/// </summary>
		IScatterSymbol ScatterSymbol { get; set; }

		bool UseSymbolFrame { get; set; }

		SelectableListNodeList Inset { set; }

		bool IndependentColor { get; set; }

		bool IndependentSkipFrequency { get; set; }

		int SkipFrequency { get; set; }

		bool OverrideAbsoluteStructureWidth { get; set; }

		double OverriddenAbsoluteStructureWidth { get; set; }

		bool OverrideRelativeStructureWidth { get; set; }

		double OverriddenRelativeStructureWidth { get; set; }

		bool OverridePlotColorInfluence { get; set; }
		PlotColorInfluence OverriddenPlotColorInfluence { get; set; }

		bool OverrideFillColor { get; set; }
		NamedColor OverriddenFillColor { get; set; }
		bool OverrideFrameColor { get; set; }
		NamedColor OverriddenFrameColor { get; set; }
		bool OverrideInsetColor { get; set; }
		NamedColor OverriddenInsetColor { get; set; }

		#region events

		event Action IndependentColorChanged;

		event Action ScatterSymbolChanged;

		#endregion events
	}

	#endregion Interfaces

	/// <summary>
	/// Summary description for XYPlotScatterStyleController.
	/// </summary>
	[UserControllerForObject(typeof(ScatterPlotStyle))]
	[ExpectedTypeOfView(typeof(IScatterPlotStyleView))]
	public class ScatterPlotStyleController : MVCANControllerEditOriginalDocBase<ScatterPlotStyle, IScatterPlotStyleView>
	{
		/// <summary>Tracks the presence of a color group style in the parent collection.</summary>
		private ColorGroupStylePresenceTracker _colorGroupStyleTracker;

		private SelectableListNodeList _symbolInsetChoices;

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break; // no subcontrollers
		}

		public override void Dispose(bool isDisposing)
		{
			_colorGroupStyleTracker = null;

			_symbolInsetChoices = null;

			base.Dispose(isDisposing);
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				_colorGroupStyleTracker = new ColorGroupStylePresenceTracker(_doc, EhIndependentColorChanged);

				var symbolTypes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IScatterSymbolInset));
				_symbolInsetChoices = new SelectableListNodeList();
				_symbolInsetChoices.Add(new SelectableListNode("No inset", null, false));
				foreach (var ty in symbolTypes)
				{
					_symbolInsetChoices.Add(new SelectableListNode(ty.Name, ty, false));
				}
			}
			if (_view != null)
			{
				_view.IndependentSkipFrequency = _doc.IndependentSkipFrequency;
				_view.SkipFrequency = _doc.SkipFrequency;

				_view.IndependentScatterSymbol = _doc.IndependentScatterSymbol;
				_view.ScatterSymbol = _doc.ScatterSymbol;
				_view.Inset = _symbolInsetChoices;
				_view.UseSymbolFrame = _doc.ScatterSymbol.Frame != null;

				// now we have to set all dialog elements to the right values
				_view.IndependentColor = _doc.IndependentColor;
				_view.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor);
				_view.Color = _doc.Color;

				_view.IndependentSymbolSize = _doc.IndependentSymbolSize;
				_view.SymbolSize = _doc.SymbolSize;

				_view.OverrideAbsoluteStructureWidth = _doc.OverrideStructureWidthOffset.HasValue;
				_view.OverriddenAbsoluteStructureWidth = _doc.OverrideStructureWidthOffset ?? 0;

				_view.OverrideRelativeStructureWidth = _doc.OverrideStructureWidthFactor.HasValue;
				_view.OverriddenRelativeStructureWidth = _doc.OverrideStructureWidthFactor ?? 0;

				_view.OverridePlotColorInfluence = _doc.OverridePlotColorInfluence.HasValue;
				_view.OverriddenPlotColorInfluence = _doc.OverridePlotColorInfluence ?? PlotColorInfluence.None;

				_view.OverrideFillColor = _doc.OverrideFillColor.HasValue;
				_view.OverriddenFillColor = _doc.OverrideFillColor ?? _doc.ScatterSymbol.FillColor;
				_view.OverrideFrameColor = _doc.OverrideFrameColor.HasValue;
				_view.OverriddenFrameColor = _doc.OverrideFrameColor ?? _doc.ScatterSymbol.Frame?.Color ?? NamedColors.Transparent;
				_view.OverrideInsetColor = _doc.OverrideInsetColor.HasValue;
				_view.OverriddenInsetColor = _doc.OverrideInsetColor ?? _doc.ScatterSymbol.Inset?.Color ?? NamedColors.Transparent;

				EhScatterSymbolChanged();
			}
		}

		public override bool Apply(bool disposeController)
		{
			bool applyResult = true;
			// don't trust user input, so all into a try statement
			try
			{
				// Skip points

				_doc.IndependentSkipFrequency = _view.IndependentSkipFrequency;
				_doc.SkipFrequency = _view.SkipFrequency;

				// Symbol Shape
				_doc.IndependentScatterSymbol = _view.IndependentScatterSymbol;
				_doc.ScatterSymbol = _view.ScatterSymbol;

				// Symbol Color
				_doc.IndependentColor = _view.IndependentColor;
				_doc.Color = _view.Color;

				// Symbol Size
				_doc.IndependentSymbolSize = _view.IndependentSymbolSize;
				_doc.SymbolSize = _view.SymbolSize;

				_doc.OverrideStructureWidthOffset = _view.OverrideAbsoluteStructureWidth ? _view.OverriddenAbsoluteStructureWidth : (double?)null;
				_doc.OverrideStructureWidthFactor = _view.OverrideRelativeStructureWidth ? _view.OverriddenRelativeStructureWidth : (double?)null;

				_doc.OverridePlotColorInfluence = _view.OverridePlotColorInfluence ? _view.OverriddenPlotColorInfluence : (PlotColorInfluence?)null;
				_doc.OverrideFillColor = _view.OverrideFillColor ? _view.OverriddenFillColor : (NamedColor?)null;
				_doc.OverrideFrameColor = _view.OverrideFrameColor ? _view.OverriddenFrameColor : (NamedColor?)null;
				_doc.OverrideInsetColor = _view.OverrideInsetColor ? _view.OverriddenInsetColor : (NamedColor?)null;

				bool cancellationRequested;
				_doc.ScatterSymbol = CheckForNeedToCreateNewSymbol(_doc.ScatterSymbol, out cancellationRequested);
				if (cancellationRequested)
				{
					applyResult = false;
				}

				if (!disposeController)
				{
					_view.ScatterSymbol = _doc.ScatterSymbol;
					EhScatterSymbolChanged();
				}
			}
			catch (Exception ex)
			{
				Current.Gui.ErrorMessageBox("A problem occured: " + ex.Message);
				return false;
			}

			return ApplyEnd(true, disposeController);
		}

		protected override void AttachView()
		{
			base.AttachView();
			_view.IndependentColorChanged += EhIndependentColorChanged;
			_view.ScatterSymbolChanged += EhScatterSymbolChanged;
		}

		protected override void DetachView()
		{
			_view.IndependentColorChanged -= EhIndependentColorChanged;
			_view.ScatterSymbolChanged += EhScatterSymbolChanged;
			base.DetachView();
		}

		private void EhIndependentColorChanged()
		{
			if (null != _view)
			{
				_doc.IndependentColor = _view.IndependentColor;
				_view.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor);
			}
		}

		private void EhScatterSymbolChanged()
		{
			var symbol = _view.ScatterSymbol;

			// Inset
			_symbolInsetChoices.SetSelection(node => symbol.Inset?.GetType() == (Type)node.Tag);
			_view.Inset = _symbolInsetChoices;

			// Frame
			_view.UseSymbolFrame = symbol.Frame != null;

			// Structure width
			if (!_view.OverrideRelativeStructureWidth)
				_view.OverriddenRelativeStructureWidth = symbol.RelativeStructureWidth;

			// Plot color influence
			if (!_view.OverridePlotColorInfluence)
				_view.OverriddenPlotColorInfluence = symbol.PlotColorInfluence;

			// Fill color
			if (!_view.OverrideFillColor)
				_view.OverriddenFillColor = symbol.FillColor;

			// FrameColor
			if (!_view.OverrideFrameColor && symbol.Frame != null)
				_view.OverriddenFrameColor = symbol.Frame.Color;

			// InsetColor
			if (!_view.OverrideInsetColor && symbol.Inset != null)
				_view.OverriddenInsetColor = symbol.Inset.Color;
		}

		private IScatterSymbol CheckForNeedToCreateNewSymbol(IScatterSymbol symbol, out bool cancellationRequested)
		{
			cancellationRequested = false;
			bool hasNewInset = false;
			bool hasNewFrame = false;
			if (symbol.Inset?.GetType() != (Type)_symbolInsetChoices.FirstSelectedNode.Tag)
				hasNewInset = true;
			if ((symbol.Frame != null) != _view.UseSymbolFrame)
				hasNewFrame = true;

			if (!hasNewFrame && !hasNewInset)
				return symbol; // nothing to do

			// we have to create a new symbol - if IsIndependent symbol is not checked, we have to create a new series of symbols

			bool createNewSymbolList;
			int originalItemIndex;
			IEnumerable<IScatterSymbol> scatterSymbolsToModify;
			if (_view.IndependentScatterSymbol)
			{
				scatterSymbolsToModify = new IScatterSymbol[] { symbol };
				originalItemIndex = 0;
				createNewSymbolList = false;
			}
			else
			{
				var parentList = ScatterSymbolListManager.Instance.GetParentList(symbol);
				if (null != parentList)
				{
					scatterSymbolsToModify = parentList;
					originalItemIndex = parentList.IndexOf(symbol);
					createNewSymbolList = true;
				}
				else
				{
					scatterSymbolsToModify = new IScatterSymbol[] { symbol };
					originalItemIndex = 0;
					createNewSymbolList = false;
				}
			}

			var newSymbols = new List<IScatterSymbol>();
			foreach (var symbolToModify in scatterSymbolsToModify)
			{
				var newSymbol = symbolToModify;

				var newInsetType = (Type)_symbolInsetChoices.FirstSelectedNode.Tag;
				if (null != newInsetType)
				{
					newSymbol = newSymbol.WithInset((IScatterSymbolInset)Activator.CreateInstance(newInsetType));
				}
				else
				{
					newSymbol = newSymbol.WithInset(null);
				}

				if (_view.UseSymbolFrame)
				{
					newSymbol = newSymbol.WithFrame(new ConstantThicknessFrame());
				}
				else
				{
					newSymbol = newSymbol.WithFrame(null);
				}

				if (_view.OverrideRelativeStructureWidth)
					newSymbol = newSymbol.WithRelativeStructureWidth(_view.OverriddenRelativeStructureWidth);

				if (_view.OverridePlotColorInfluence)
					newSymbol = newSymbol.WithPlotColorInfluence(_view.OverriddenPlotColorInfluence);

				if (_view.OverrideFillColor)
					newSymbol = newSymbol.WithFillColor(_view.OverriddenFillColor);

				if (_view.OverrideFrameColor && newSymbol.Frame != null)
					newSymbol = newSymbol.WithFrame(newSymbol.Frame.WithColor(_view.OverriddenFrameColor));

				if (_view.OverrideInsetColor && newSymbol.Inset != null)
					newSymbol = newSymbol.WithInset(newSymbol.Inset.WithColor(_view.OverriddenInsetColor));

				newSymbols.Add(newSymbol);
			}

			if (createNewSymbolList)
			{
				string newName = "Custom";
				if (!Current.Gui.ShowDialog(ref newName, "Enter a name for the new scatter symbol set", false))
				{
					cancellationRequested = true;
					return symbol;
				}

				var newScatterSymbolList = new ScatterSymbolList(newName, newSymbols);
				ScatterSymbolList resultList;
				ScatterSymbolListManager.Instance.TryRegisterList(newScatterSymbolList, Altaxo.Main.ItemDefinitionLevel.Project, out resultList);
				// return the item at the original list index.
				return resultList[originalItemIndex];
			}
			else
			{
				ScatterSymbolList dummyList;
				IScatterSymbol result;
				if (ScatterSymbolListManager.Instance.TryFindListContaining(newSymbols[originalItemIndex], out dummyList, out result))
					return result;
				else
					return newSymbols[originalItemIndex];
			}
		}
	} // end of class XYPlotScatterStyleController
} // end of namespace