#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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

using Altaxo.Graph3D;
using Altaxo.Units;
using System;
using System.Collections.ObjectModel;

namespace Altaxo.Gui.Graph3D
{
	public interface IGridPartitioningView
	{
		QuantityWithUnitGuiEnvironment XPartitionEnvironment { set; }

		QuantityWithUnitGuiEnvironment YPartitionEnvironment { set; }

		QuantityWithUnitGuiEnvironment ZPartitionEnvironment { set; }

		/// <summary>Sets the default x quantity, i.e. the quantity that is used if the user inserts a new item in the XPartition.</summary>
		Units.DimensionfulQuantity DefaultXQuantity { set; }

		/// <summary>Sets the default y quantity, i.e. the quantity that is used if the user inserts a new item in the YPartition.</summary>
		Units.DimensionfulQuantity DefaultYQuantity { set; }

		/// <summary>Sets the default z quantity, i.e. the quantity that is used if the user inserts a new item in the YPartition.</summary>
		Units.DimensionfulQuantity DefaultZQuantity { set; }

		ObservableCollection<Units.DimensionfulQuantity> XPartitionValues { set; }

		ObservableCollection<Units.DimensionfulQuantity> YPartitionValues { set; }

		ObservableCollection<Units.DimensionfulQuantity> ZPartitionValues { set; }
	}

	[ExpectedTypeOfView(typeof(IGridPartitioningView))]
	[UserControllerForObject(typeof(GridPartitioning3D))]
	public class GridPartitioningController : MVCANControllerEditOriginalDocBase<GridPartitioning3D, IGridPartitioningView>
	{
		private ObservableCollection<Units.DimensionfulQuantity> _xPartitionValues;
		private ObservableCollection<Units.DimensionfulQuantity> _yPartitionValues;
		private ObservableCollection<Units.DimensionfulQuantity> _zPartitionValues;

		private QuantityWithUnitGuiEnvironment _xSizeEnvironment;
		private QuantityWithUnitGuiEnvironment _ySizeEnvironment;
		private QuantityWithUnitGuiEnvironment _zSizeEnvironment;

		private ChangeableRelativePercentUnit _percentLayerXSizeUnit = new ChangeableRelativePercentUnit("Relative X-Size", "%", new DimensionfulQuantity(1, Units.Length.Point.Instance));
		private ChangeableRelativePercentUnit _percentLayerYSizeUnit = new ChangeableRelativePercentUnit("Relative Y-Size", "%", new DimensionfulQuantity(1, Units.Length.Point.Instance));
		private ChangeableRelativePercentUnit _percentLayerZSizeUnit = new ChangeableRelativePercentUnit("Relative Z-Size", "%", new DimensionfulQuantity(1, Units.Length.Point.Instance));

		private VectorD3D _parentLayerSize;

		public override System.Collections.Generic.IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break;
		}

		public override void Dispose(bool isDisposing)
		{
			_xPartitionValues = null;
			_yPartitionValues = null;
			_zPartitionValues = null;
			_xSizeEnvironment = null;
			_ySizeEnvironment = null;
			_zSizeEnvironment = null;
			_percentLayerXSizeUnit = null;
			_percentLayerYSizeUnit = null;
			_percentLayerZSizeUnit = null;

			base.Dispose(isDisposing);
		}

		public override bool InitializeDocument(params object[] args)
		{
			if (args.Length < 2)
				return false;
			if (!(args[0] is GridPartitioning3D))
				return false;

			if (args[1] is VectorD3D)
				_parentLayerSize = (VectorD3D)args[1];
			else if (args[1] is HostLayer3D)
				_parentLayerSize = ((HostLayer3D)args[1]).Size;
			else
				_parentLayerSize = new VectorD3D(600, 400, 400);

			return base.InitializeDocument(args);
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				_percentLayerXSizeUnit.ReferenceQuantity = new DimensionfulQuantity(_parentLayerSize.X, Units.Length.Point.Instance);
				_percentLayerYSizeUnit.ReferenceQuantity = new DimensionfulQuantity(_parentLayerSize.Y, Units.Length.Point.Instance);
				_percentLayerZSizeUnit.ReferenceQuantity = new DimensionfulQuantity(_parentLayerSize.Z, Units.Length.Point.Instance);
				_xSizeEnvironment = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection, _percentLayerXSizeUnit);
				_ySizeEnvironment = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection, _percentLayerYSizeUnit);
				_zSizeEnvironment = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection, _percentLayerZSizeUnit);

				_xPartitionValues = new ObservableCollection<Units.DimensionfulQuantity>();
				_yPartitionValues = new ObservableCollection<Units.DimensionfulQuantity>();
				_zPartitionValues = new ObservableCollection<Units.DimensionfulQuantity>();

				foreach (var xp in _doc.XPartitioning)
					_xPartitionValues.Add(xp.IsAbsolute ? new DimensionfulQuantity(xp.Value, Units.Length.Point.Instance) : new DimensionfulQuantity(xp.Value * 100, _percentLayerXSizeUnit));

				foreach (var yp in _doc.YPartitioning)
					_yPartitionValues.Add(yp.IsAbsolute ? new DimensionfulQuantity(yp.Value, Units.Length.Point.Instance) : new DimensionfulQuantity(yp.Value * 100, _percentLayerYSizeUnit));

				foreach (var zp in _doc.ZPartitioning)
					_zPartitionValues.Add(zp.IsAbsolute ? new DimensionfulQuantity(zp.Value, Units.Length.Point.Instance) : new DimensionfulQuantity(zp.Value * 100, _percentLayerZSizeUnit));
			}
			if (null != _view)
			{
				_view.XPartitionEnvironment = _xSizeEnvironment;
				_view.YPartitionEnvironment = _ySizeEnvironment;
				_view.ZPartitionEnvironment = _zSizeEnvironment;
				_view.DefaultXQuantity = new DimensionfulQuantity(0, _percentLayerXSizeUnit);
				_view.DefaultYQuantity = new DimensionfulQuantity(0, _percentLayerYSizeUnit);
				_view.DefaultZQuantity = new DimensionfulQuantity(0, _percentLayerZSizeUnit);

				_view.XPartitionValues = _xPartitionValues;
				_view.YPartitionValues = _yPartitionValues;
				_view.ZPartitionValues = _zPartitionValues;
			}
		}

		public override bool Apply(bool disposeController)
		{
			_doc.XPartitioning.Clear();
			_doc.YPartitioning.Clear();
			_doc.ZPartitioning.Clear();

			foreach (var val in _xPartitionValues)
			{
				if (object.ReferenceEquals(val.Unit, _percentLayerXSizeUnit))
					_doc.XPartitioning.Add(RADouble.NewRel(val.Value / 100));
				else
					_doc.XPartitioning.Add(RADouble.NewAbs(val.AsValueIn(Units.Length.Point.Instance)));
			}

			foreach (var val in _yPartitionValues)
			{
				if (object.ReferenceEquals(val.Unit, _percentLayerYSizeUnit))
					_doc.YPartitioning.Add(RADouble.NewRel(val.Value / 100));
				else
					_doc.YPartitioning.Add(RADouble.NewAbs(val.AsValueIn(Units.Length.Point.Instance)));
			}

			foreach (var val in _zPartitionValues)
			{
				if (object.ReferenceEquals(val.Unit, _percentLayerZSizeUnit))
					_doc.ZPartitioning.Add(RADouble.NewRel(val.Value / 100));
				else
					_doc.ZPartitioning.Add(RADouble.NewAbs(val.AsValueIn(Units.Length.Point.Instance)));
			}

			return ApplyEnd(true, disposeController);
		}
	}
}