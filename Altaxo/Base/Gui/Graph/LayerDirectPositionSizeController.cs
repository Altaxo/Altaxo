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
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Gui;
using Altaxo.Serialization;
using Altaxo.Units;
using System;

namespace Altaxo.Gui.Graph
{
	#region Interfaces

	public interface ILayerDirectPositionSizeView
	{
		void InitializeXPosition(Units.DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env);

		void InitializeYPosition(Units.DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env);

		void InitializeYSize(Units.DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env);

		void InitializeXSize(Units.DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env);

		Units.DimensionfulQuantity XPosition { get; }

		Units.DimensionfulQuantity YPosition { get; }

		Units.DimensionfulQuantity XSize { get; }

		Units.DimensionfulQuantity YSize { get; }

		double Rotation { get; set; }

		double Scale { get; set; }
	}

	#endregion Interfaces

	/// <summary>
	/// Summary description for LayerPositionController.
	/// </summary>
	[ExpectedTypeOfView(typeof(ILayerDirectPositionSizeView))]
	public class LayerDirectPositionSizeController : MVCANControllerBase<ItemLocationDirect, ILayerDirectPositionSizeView>
	{
		private PointD2D _parentLayerSize;
		private QuantityWithUnitGuiEnvironment _xSizeEnvironment, _xPositionEnvironment;
		private QuantityWithUnitGuiEnvironment _ySizeEnvironment, _yPositionEnvironment;

		private ChangeableRelativePercentUnit _percentLayerXSizeUnit = new ChangeableRelativePercentUnit("% Parent X-Size", "%", new DimensionfulQuantity(1, Units.Length.Point.Instance));
		private ChangeableRelativePercentUnit _percentLayerYSizeUnit = new ChangeableRelativePercentUnit("% Parent Y-Size", "%", new DimensionfulQuantity(1, Units.Length.Point.Instance));

		public override bool InitializeDocument(params object[] args)
		{
			if (args.Length < 2)
				return false;
			if (!(args[1] is PointD2D))
				return false;
			_parentLayerSize = (PointD2D)args[1];
			return base.InitializeDocument(args);
		}

		protected override void Initialize(bool initData)
		{
			if (initData)
			{
				_percentLayerXSizeUnit.ReferenceQuantity = new DimensionfulQuantity(_parentLayerSize.X, Units.Length.Point.Instance);
				_percentLayerYSizeUnit.ReferenceQuantity = new DimensionfulQuantity(_parentLayerSize.Y, Units.Length.Point.Instance);
				_xSizeEnvironment = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection, _percentLayerXSizeUnit);
				_ySizeEnvironment = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection, _percentLayerYSizeUnit);
				_xPositionEnvironment = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection, _percentLayerXSizeUnit);
				_yPositionEnvironment = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection, _percentLayerYSizeUnit);
			}
			if (null != _view)
			{
				var xSize = _doc.SizeX.IsAbsolute ? new DimensionfulQuantity(_doc.SizeX.Value, Units.Length.Point.Instance) : new DimensionfulQuantity(_doc.SizeX.Value * 100, _percentLayerXSizeUnit);
				_view.InitializeXSize(xSize, _xSizeEnvironment);
				var ySize = _doc.SizeY.IsAbsolute ? new DimensionfulQuantity(_doc.SizeY.Value, Units.Length.Point.Instance) : new DimensionfulQuantity(_doc.SizeY.Value * 100, _percentLayerYSizeUnit);
				_view.InitializeYSize(ySize, _ySizeEnvironment);

				var xPos = _doc.PositionX.IsAbsolute ? new DimensionfulQuantity(_doc.PositionX.Value, Units.Length.Point.Instance) : new DimensionfulQuantity(_doc.PositionX.Value * 100, _percentLayerXSizeUnit);
				_view.InitializeXPosition(xPos, _xPositionEnvironment);
				var yPos = _doc.PositionY.IsAbsolute ? new DimensionfulQuantity(_doc.PositionY.Value, Units.Length.Point.Instance) : new DimensionfulQuantity(_doc.PositionY.Value * 100, _percentLayerYSizeUnit);
				_view.InitializeYPosition(yPos, _yPositionEnvironment);

				_view.Rotation = _doc.Rotation;
				_view.Scale = _doc.Scale;
			}
		}

		#region IApplyController Members

		public override bool Apply()
		{
			try
			{
				_doc.Scale = _view.Scale;
				_doc.Rotation = _view.Rotation;

				var xSize = _view.XSize;
				var ySize = _view.YSize;

				var xPos = _view.XPosition;
				var yPos = _view.YPosition;

				if (object.ReferenceEquals(xSize.Unit, _percentLayerXSizeUnit))
					_doc.SizeX = Calc.RelativeOrAbsoluteValue.NewRelativeValue(xSize.Value / 100);
				else
					_doc.SizeX = Calc.RelativeOrAbsoluteValue.NewAbsoluteValue(xSize.AsValueIn(Units.Length.Point.Instance));

				if (object.ReferenceEquals(ySize.Unit, _percentLayerYSizeUnit))
					_doc.SizeY = Calc.RelativeOrAbsoluteValue.NewRelativeValue(ySize.Value / 100);
				else
					_doc.SizeY = Calc.RelativeOrAbsoluteValue.NewAbsoluteValue(ySize.AsValueIn(Units.Length.Point.Instance));

				if (object.ReferenceEquals(xPos.Unit, _percentLayerXSizeUnit))
					_doc.PositionX = Calc.RelativeOrAbsoluteValue.NewRelativeValue(xPos.Value / 100);
				else
					_doc.PositionX = Calc.RelativeOrAbsoluteValue.NewAbsoluteValue(xPos.AsValueIn(Units.Length.Point.Instance));

				if (object.ReferenceEquals(yPos.Unit, _percentLayerYSizeUnit))
					_doc.PositionY = Calc.RelativeOrAbsoluteValue.NewRelativeValue(yPos.Value / 100);
				else
					_doc.PositionY = Calc.RelativeOrAbsoluteValue.NewAbsoluteValue(yPos.AsValueIn(Units.Length.Point.Instance));

				_originalDoc.CopyFrom(_doc);
			}
			catch (Exception)
			{
				return false; // indicate that something failed
			}
			return true;
		}

		#endregion IApplyController Members
	}
}