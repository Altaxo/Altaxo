#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Groups;
using Altaxo.Graph.Plot.Groups;
using Altaxo.Main.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Gui.Graph
{
	#region Interfaces

	/// <summary>
	/// This view interface is for showing the options of the XYLineScatterPlotStyle
	/// </summary>
	public interface IPlotGroupCollectionViewSimple
	{
		/// <summary>
		/// Initializes the plot group conditions.
		/// </summary>
		/// <param name="bColor">True if the color is changed.</param>
		/// <param name="bLineType">True if the line type is changed.</param>
		/// <param name="bSymbol">True if the symbol shape is changed.</param>
		/// <param name="bConcurrently">True if all styles are changed concurrently.</param>
		/// <param name="bStrict">True if the depending plot styles are enforced to have strictly the same properties than the parent style.</param>
		void InitializePlotGroupConditions(bool bColor, bool bLineType, bool bSymbol, bool bConcurrently, Altaxo.Graph.Plot.Groups.PlotGroupStrictness bStrict);

		#region Getter

		Altaxo.Graph.Plot.Groups.PlotGroupStrictness PlotGroupStrict { get; }

		bool PlotGroupColor { get; }

		bool PlotGroupLineType { get; }

		bool PlotGroupSymbol { get; }

		bool PlotGroupConcurrently { get; }

		bool PlotGroupUpdate { get; }

		#endregion Getter
	}

	#endregion Interfaces

	[ExpectedTypeOfView(typeof(IPlotGroupCollectionViewSimple))]
	public class PlotGroupCollectionControllerSimple : IMVCANController
	{
		private IPlotGroupCollectionViewSimple _view;
		private PlotGroupStyleCollection _origdoc;
		private PlotGroupStyleCollection _doc;

		private void Initialize(bool initData)
		{
			if (null != _view)
			{
				bool bSerial;
				bool color;
				bool linestyle;
				bool symbol;

				IsSimplePlotGrouping(_doc, out bSerial, out color, out linestyle, out symbol);

				_view.InitializePlotGroupConditions(
					color,
					linestyle,
					symbol,
					!bSerial, //_parentPlotGroup.ChangeStylesConcurrently,
					PlotGroupStrictness.Normal //_parentPlotGroup.ChangeStylesStrictly
					);
			}
		}

		/// <summary>
		/// Determines if a PlotGroupStyleCollection fullfills the requirements to be presented by a simple controller.
		/// </summary>
		/// <param name="plotGroupStyles">The <see cref="PlotGroupStyleCollection"/> to investigate.</param>
		/// <returns>True if the <see cref="PlotGroupStyleCollection"/> can be presented by a simple controller, otherwise False.</returns>
		public static bool IsSimplePlotGrouping(PlotGroupStyleCollection plotGroupStyles)
		{
			bool b1, b2, b3, b4;
			return IsSimplePlotGrouping(plotGroupStyles, out b1, out b2, out b3, out b4);
		}

		/// <summary>
		/// Determines if a PlotGroupStyleCollection fullfills the requirements to be presented by a simple controller.
		/// </summary>
		/// <param name="plotGroupStyles">The <see cref="PlotGroupStyleCollection"/> to investigate.</param>
		/// <param name="isSteppingSerial">On return: is True if the styles are changed serial, i.e. first all colors, then the line style, then the symbol style.</param>
		/// <param name="isGroupedByColor">On return: is True if the items are grouped by color.</param>
		/// <param name="isGroupedByLineStyle">On return: is True if the items are grouped by line style.</param>
		/// <param name="isGroupedBySymbolStyle">On return: is True if the items are grouped by symbol style.</param>
		/// <returns>True if the <see cref="PlotGroupStyleCollection"/> can be presented by a simple controller, otherwise False.</returns>
		public static bool IsSimplePlotGrouping(PlotGroupStyleCollection plotGroupStyles, out bool isSteppingSerial, out bool isGroupedByColor, out bool isGroupedByLineStyle, out bool isGroupedBySymbolStyle)
		{
			isSteppingSerial = false;
			isGroupedByColor = false;
			isGroupedByLineStyle = false;
			isGroupedBySymbolStyle = false;

			if (plotGroupStyles != null)
			{
				if (plotGroupStyles.CoordinateTransformingStyle != null)
					return false;

				isGroupedByColor = plotGroupStyles.ContainsType(typeof(ColorGroupStyle));
				isGroupedByLineStyle = plotGroupStyles.ContainsType(typeof(LineStyleGroupStyle));
				isGroupedBySymbolStyle = plotGroupStyles.ContainsType(typeof(SymbolShapeStyleGroupStyle));

				if (plotGroupStyles.Count != (isGroupedByColor ? 1 : 0) + (isGroupedByLineStyle ? 1 : 0) + (isGroupedBySymbolStyle ? 1 : 0))
					return false;

				var list = new List<Type>();
				if (isGroupedByColor)
					list.Add(typeof(ColorGroupStyle));
				if (isGroupedByLineStyle)
					list.Add(typeof(LineStyleGroupStyle));
				if (isGroupedBySymbolStyle)
					list.Add(typeof(SymbolShapeStyleGroupStyle));

				// Test for concurrent stepping
				bool isConcurrent = true;
				foreach (var t in list)
				{
					if (0 != plotGroupStyles.GetTreeLevelOf(t) || !plotGroupStyles.GetPlotGroupStyle(t).IsStepEnabled) // Tree level has to be 0 for concurrent items, and stepping must be enabled
					{
						isConcurrent = false;
						break;
					}
				}

				// Test for serial stepping
				isSteppingSerial = true;
				for (int i = 0; i < list.Count; ++i)
				{
					var t = list[i];

					if (i != plotGroupStyles.GetTreeLevelOf(t) || !plotGroupStyles.GetPlotGroupStyle(t).IsStepEnabled) // Tree level has to be i and step must be enabled
					{
						isSteppingSerial = false;
						break;
					}
				}

				if (!isConcurrent && !isSteppingSerial)
					return false;

				return true;
			}

			return false;
		}

		#region IMVCANController

		public bool InitializeDocument(params object[] args)
		{
			if (args == null || args.Length == 0 || !(args[0] is PlotGroupStyleCollection))
				return false;
			_origdoc = (PlotGroupStyleCollection)args[0];
			_doc = _origdoc.Clone();
			Initialize(true);
			return true;
		}

		public UseDocument UseDocumentCopy
		{
			set { }
		}

		public object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				if (_view != null)
				{
				}

				_view = value as IPlotGroupCollectionViewSimple;

				if (_view != null)
				{
					Initialize(false);
				}
			}
		}

		public object ModelObject
		{
			get { return _origdoc; }
		}

		public void Dispose()
		{
		}

		public bool Apply(bool disposeController)
		{
			bool color = _view.PlotGroupColor;
			bool linestyle = _view.PlotGroupLineType;
			bool symbol = _view.PlotGroupSymbol;
			bool serial = !_view.PlotGroupConcurrently;

			if (_doc.ContainsType(typeof(ColorGroupStyle)))
				_doc.RemoveType(typeof(ColorGroupStyle));
			if (_doc.ContainsType(typeof(LineStyleGroupStyle)))
				_doc.RemoveType(typeof(LineStyleGroupStyle));
			if (_doc.ContainsType(typeof(SymbolShapeStyleGroupStyle)))
				_doc.RemoveType(typeof(SymbolShapeStyleGroupStyle));

			if (color)
			{
				_doc.Add(ColorGroupStyle.NewExternalGroupStyle());
			}
			if (linestyle)
			{
				if (serial && color)
					_doc.Add(new LineStyleGroupStyle(), typeof(ColorGroupStyle));
				else
					_doc.Add(new LineStyleGroupStyle());
			}
			if (symbol)
			{
				if (serial && linestyle)
					_doc.Add(new SymbolShapeStyleGroupStyle(), typeof(LineStyleGroupStyle));
				else if (serial && color)
					_doc.Add(new SymbolShapeStyleGroupStyle(), typeof(ColorGroupStyle));
				else
					_doc.Add(new SymbolShapeStyleGroupStyle());
			}

			_doc.PlotGroupStrictness = _view.PlotGroupStrict;

			_origdoc.CopyFrom(_doc);

			return true;
		}

		/// <summary>
		/// Try to revert changes to the model, i.e. restores the original state of the model.
		/// </summary>
		/// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
		/// <returns>
		///   <c>True</c> if the revert operation was successfull; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
		/// </returns>
		public bool Revert(bool disposeController)
		{
			return false;
		}

		#endregion IMVCANController
	}
} // end of namespace