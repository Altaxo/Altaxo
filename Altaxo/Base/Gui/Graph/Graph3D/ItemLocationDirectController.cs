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

using Altaxo.Geometry;
using Altaxo.Graph.Graph3D;
using Altaxo.Units;
using AUL = Altaxo.Units.Length;
using System;

namespace Altaxo.Gui.Graph.Graph3D
{
	#region Interfaces

	public interface IItemLocationDirectView
	{
		void InitializeXPosition(DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env);

		void InitializeYPosition(DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env);

		void InitializeZPosition(DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env);

		void ShowSizeElements(bool isVisible, bool isEnabled);

		void ShowScaleElements(bool isVisible, bool isEnabled);

		void ShowPositionElements(bool isVisible, bool isEnabled);

		void ShowAnchorElements(bool isVisible, bool isEnabled);

		void InitializeXSize(DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env);

		void InitializeYSize(DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env);

		void InitializeZSize(DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env);

		DimensionfulQuantity XPosition { get; }

		DimensionfulQuantity YPosition { get; }

		DimensionfulQuantity ZPosition { get; }

		DimensionfulQuantity XSize { get; }

		DimensionfulQuantity YSize { get; }

		DimensionfulQuantity ZSize { get; }

		double RotationX { get; set; }

		double RotationY { get; set; }

		double RotationZ { get; set; }

		double ShearX { get; set; }

		double ShearY { get; set; }

		double ShearZ { get; set; }

		double ScaleX { get; set; }

		double ScaleY { get; set; }

		double ScaleZ { get; set; }

		void InitializePivot(RADouble pivotX, RADouble pivotY, RADouble pivotZ, VectorD3D sizeOfOwnGraphic);

		RADouble PivotX { get; }

		RADouble PivotY { get; }

		RADouble PivotZ { get; }

		void InitializeReference(RADouble referenceX, RADouble referenceY, RADouble referenceZ, VectorD3D sizeOfParent);

		RADouble ReferenceX { get; }

		RADouble ReferenceY { get; }

		RADouble ReferenceZ { get; }

		event Action SizeXChanged;

		event Action SizeYChanged;

		event Action SizeZChanged;

		event Action ScaleXChanged;

		event Action ScaleYChanged;

		event Action ScaleZChanged;
	}

	#endregion Interfaces

	/// <summary>
	/// Summary description for LayerPositionController.
	/// </summary>
	[ExpectedTypeOfView(typeof(IItemLocationDirectView))]
	[UserControllerForObject(typeof(ItemLocationDirect))]
	public class ItemLocationDirectController : MVCANControllerEditOriginalDocBase<ItemLocationDirect, IItemLocationDirectView>
	{
		private VectorD3D _parentSize;
		private QuantityWithUnitGuiEnvironment _xSizeEnvironment, _xPositionEnvironment;
		private QuantityWithUnitGuiEnvironment _ySizeEnvironment, _yPositionEnvironment;
		private QuantityWithUnitGuiEnvironment _zSizeEnvironment, _zPositionEnvironment;

		private ChangeableRelativePercentUnit _percentLayerXSizeUnit = new ChangeableRelativePercentUnit("% Parent X-Size", "%", new DimensionfulQuantity(1, AUL.Point.Instance));
		private ChangeableRelativePercentUnit _percentLayerYSizeUnit = new ChangeableRelativePercentUnit("% Parent Y-Size", "%", new DimensionfulQuantity(1, AUL.Point.Instance));
		private ChangeableRelativePercentUnit _percentLayerZSizeUnit = new ChangeableRelativePercentUnit("% Parent Z-Size", "%", new DimensionfulQuantity(1, AUL.Point.Instance));

		protected bool _showPositionElements_Enabled = true, _showPositionElements_IsVisible = true;
		protected bool _showSizeElements_Enabled = true, _showSizeElements_IsVisible = true;
		protected bool _showScaleElements_Enabled = true, _showScaleElements_IsVisible = true;
		protected bool _showAnchorElements_Enabled = true, _showAnchorElements_IsVisible = true;

		public override System.Collections.Generic.IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break;
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				_parentSize = _doc.ParentSize;
				_percentLayerXSizeUnit.ReferenceQuantity = new DimensionfulQuantity(_parentSize.X, AUL.Point.Instance);
				_percentLayerYSizeUnit.ReferenceQuantity = new DimensionfulQuantity(_parentSize.Y, AUL.Point.Instance);
				_percentLayerZSizeUnit.ReferenceQuantity = new DimensionfulQuantity(_parentSize.Z, AUL.Point.Instance);

				_xSizeEnvironment = new QuantityWithUnitGuiEnvironment(SizeEnvironment.Instance, new IUnit[] { _percentLayerXSizeUnit });
				_ySizeEnvironment = new QuantityWithUnitGuiEnvironment(SizeEnvironment.Instance, new IUnit[] { _percentLayerYSizeUnit });
				_zSizeEnvironment = new QuantityWithUnitGuiEnvironment(SizeEnvironment.Instance, new IUnit[] { _percentLayerZSizeUnit });

				_xPositionEnvironment = new QuantityWithUnitGuiEnvironment(PositionEnvironment.Instance, new IUnit[] { _percentLayerXSizeUnit });
				_yPositionEnvironment = new QuantityWithUnitGuiEnvironment(PositionEnvironment.Instance, new IUnit[] { _percentLayerYSizeUnit });
				_zPositionEnvironment = new QuantityWithUnitGuiEnvironment(PositionEnvironment.Instance, new IUnit[] { _percentLayerZSizeUnit });
			}

			if (null != _view)
			{
				_view.ShowSizeElements(!_doc.IsAutoSized, true);

				if (!_doc.IsAutoSized)
				{
					var xSize = _doc.SizeX.IsAbsolute ? new DimensionfulQuantity(_doc.SizeX.Value, AUL.Point.Instance) : new DimensionfulQuantity(_doc.SizeX.Value * 100, _percentLayerXSizeUnit);
					_view.InitializeXSize(xSize, _xSizeEnvironment);
					var ySize = _doc.SizeY.IsAbsolute ? new DimensionfulQuantity(_doc.SizeY.Value, AUL.Point.Instance) : new DimensionfulQuantity(_doc.SizeY.Value * 100, _percentLayerYSizeUnit);
					_view.InitializeYSize(ySize, _ySizeEnvironment);
					var zSize = _doc.SizeZ.IsAbsolute ? new DimensionfulQuantity(_doc.SizeZ.Value, AUL.Point.Instance) : new DimensionfulQuantity(_doc.SizeZ.Value * 100, _percentLayerZSizeUnit);
					_view.InitializeZSize(zSize, _zSizeEnvironment);
				}

				var xPos = _doc.PositionX.IsAbsolute ? new DimensionfulQuantity(_doc.PositionX.Value, AUL.Point.Instance) : new DimensionfulQuantity(_doc.PositionX.Value * 100, _percentLayerXSizeUnit);
				_view.InitializeXPosition(xPos, _xPositionEnvironment);
				var yPos = _doc.PositionY.IsAbsolute ? new DimensionfulQuantity(_doc.PositionY.Value, AUL.Point.Instance) : new DimensionfulQuantity(_doc.PositionY.Value * 100, _percentLayerYSizeUnit);
				_view.InitializeYPosition(yPos, _yPositionEnvironment);
				var zPos = _doc.PositionZ.IsAbsolute ? new DimensionfulQuantity(_doc.PositionZ.Value, AUL.Point.Instance) : new DimensionfulQuantity(_doc.PositionZ.Value * 100, _percentLayerZSizeUnit);
				_view.InitializeZPosition(zPos, _zPositionEnvironment);

				_view.RotationX = _doc.RotationX;
				_view.RotationY = _doc.RotationY;
				_view.RotationZ = _doc.RotationZ;
				_view.ShearX = _doc.ShearX;
				_view.ShearY = _doc.ShearY;
				_view.ShearZ = _doc.ShearZ;
				_view.ScaleX = _doc.ScaleX;
				_view.ScaleY = _doc.ScaleY;
				_view.ScaleZ = _doc.ScaleZ;
				_view.InitializePivot(_doc.LocalAnchorX, _doc.LocalAnchorY, _doc.LocalAnchorZ, _doc.AbsoluteSize);
				_view.InitializeReference(_doc.ParentAnchorX, _doc.ParentAnchorY, _doc.ParentAnchorZ, _doc.ParentSize);

				_view.ShowPositionElements(_showPositionElements_IsVisible, _showPositionElements_Enabled);
				_view.ShowSizeElements(_showSizeElements_IsVisible, _showSizeElements_Enabled);
				_view.ShowScaleElements(_showScaleElements_IsVisible, _showSizeElements_Enabled);
				_view.ShowAnchorElements(_showAnchorElements_IsVisible, _showAnchorElements_Enabled);
			}
		}

		public override bool Apply(bool disposeController)
		{
			try
			{
				_doc.RotationX = _view.RotationX;
				_doc.RotationY = _view.RotationY;
				_doc.RotationZ = _view.RotationZ;
				_doc.ShearX = _view.ShearX;
				_doc.ShearY = _view.ShearY;
				_doc.ShearZ = _view.ShearZ;
				_doc.ScaleX = _view.ScaleX;
				_doc.ScaleY = _view.ScaleY;
				_doc.ScaleZ = _view.ScaleZ;

				if (!_doc.IsAutoSized)
				{
					var xSize = _view.XSize;
					var ySize = _view.YSize;
					var zSize = _view.ZSize;

					if (object.ReferenceEquals(xSize.Unit, _percentLayerXSizeUnit))
						_doc.SizeX = RADouble.NewRel(xSize.Value / 100);
					else
						_doc.SizeX = RADouble.NewAbs(xSize.AsValueIn(AUL.Point.Instance));

					if (object.ReferenceEquals(ySize.Unit, _percentLayerYSizeUnit))
						_doc.SizeY = RADouble.NewRel(ySize.Value / 100);
					else
						_doc.SizeY = RADouble.NewAbs(ySize.AsValueIn(AUL.Point.Instance));

					if (object.ReferenceEquals(zSize.Unit, _percentLayerZSizeUnit))
						_doc.SizeZ = RADouble.NewRel(zSize.Value / 100);
					else
						_doc.SizeZ = RADouble.NewAbs(zSize.AsValueIn(AUL.Point.Instance));
				}

				var xPos = _view.XPosition;
				var yPos = _view.YPosition;
				var zPos = _view.ZPosition;

				if (object.ReferenceEquals(xPos.Unit, _percentLayerXSizeUnit))
					_doc.PositionX = RADouble.NewRel(xPos.Value / 100);
				else
					_doc.PositionX = RADouble.NewAbs(xPos.AsValueIn(AUL.Point.Instance));

				if (object.ReferenceEquals(yPos.Unit, _percentLayerYSizeUnit))
					_doc.PositionY = RADouble.NewRel(yPos.Value / 100);
				else
					_doc.PositionY = RADouble.NewAbs(yPos.AsValueIn(AUL.Point.Instance));

				if (object.ReferenceEquals(zPos.Unit, _percentLayerZSizeUnit))
					_doc.PositionZ = RADouble.NewRel(zPos.Value / 100);
				else
					_doc.PositionZ = RADouble.NewAbs(zPos.AsValueIn(AUL.Point.Instance));

				_doc.LocalAnchorX = _view.PivotX;
				_doc.LocalAnchorY = _view.PivotY;
				_doc.LocalAnchorZ = _view.PivotZ;

				_doc.ParentAnchorX = _view.ReferenceX;
				_doc.ParentAnchorY = _view.ReferenceY;
				_doc.ParentAnchorZ = _view.ReferenceZ;
			}
			catch (Exception ex)
			{
				Current.Gui.ErrorMessageBox(ex.Message, "Error applying ItemLocationDirect");
				return false; // indicate that something failed
			}

			return ApplyEnd(true, disposeController);
		}

		protected override void AttachView()
		{
			base.AttachView();
			_view.SizeXChanged += EhSizeXChanged;
			_view.SizeYChanged += EhSizeYChanged;
			_view.SizeZChanged += EhSizeZChanged;
			_view.ScaleXChanged += EhScaleXChanged;
			_view.ScaleYChanged += EhScaleYChanged;
			_view.ScaleZChanged += EhScaleZChanged;
		}

		protected override void DetachView()
		{
			_view.SizeXChanged -= EhSizeXChanged;
			_view.SizeYChanged -= EhSizeYChanged;
			_view.SizeZChanged -= EhSizeZChanged;
			_view.ScaleXChanged -= EhScaleXChanged;
			_view.ScaleYChanged -= EhScaleYChanged;
			_view.ScaleZChanged -= EhScaleZChanged;
			base.DetachView();
		}

		#region Service members

		public event Action<RADouble> SizeXChanged;

		private void EhSizeXChanged()
		{
			var actn = SizeXChanged;
			if (null != actn)
			{
				RADouble result;
				var xSize = _view.XSize;

				if (object.ReferenceEquals(xSize.Unit, _percentLayerXSizeUnit))
					result = RADouble.NewRel(xSize.Value / 100);
				else
					result = RADouble.NewAbs(xSize.AsValueIn(AUL.Point.Instance));
				actn(result);
			}
		}

		public event Action<RADouble> SizeYChanged;

		private void EhSizeYChanged()
		{
			var actn = SizeYChanged;
			if (null != actn)
			{
				RADouble result;
				var ySize = _view.YSize;

				if (object.ReferenceEquals(ySize.Unit, _percentLayerYSizeUnit))
					result = RADouble.NewRel(ySize.Value / 100);
				else
					result = RADouble.NewAbs(ySize.AsValueIn(AUL.Point.Instance));
				actn(result);
			}
		}

		public event Action<RADouble> SizeZChanged;

		private void EhSizeZChanged()
		{
			var actn = SizeZChanged;
			if (null != actn)
			{
				RADouble result;
				var zSize = _view.ZSize;

				if (object.ReferenceEquals(zSize.Unit, _percentLayerYSizeUnit))
					result = RADouble.NewRel(zSize.Value / 100);
				else
					result = RADouble.NewAbs(zSize.AsValueIn(AUL.Point.Instance));
				actn(result);
			}
		}

		public event Action<double> ScaleXChanged;

		private void EhScaleXChanged()
		{
			var actn = ScaleXChanged;
			if (null != actn)
			{
				actn(_view.ScaleX);
			}
		}

		public event Action<double> ScaleYChanged;

		private void EhScaleYChanged()
		{
			var actn = ScaleYChanged;
			if (null != actn)
			{
				actn(_view.ScaleY);
			}
		}

		public event Action<double> ScaleZChanged;

		private void EhScaleZChanged()
		{
			var actn = ScaleZChanged;
			if (null != actn)
			{
				actn(_view.ScaleZ);
			}
		}

		public void ShowSizeElements(bool isVisible, bool isEnabled)
		{
			_showSizeElements_IsVisible = isVisible;
			_showSizeElements_Enabled = isEnabled;
			if (null != _view)
				_view.ShowSizeElements(isVisible, isEnabled);
		}

		public void ShowScaleElements(bool isVisible, bool isEnabled)
		{
			_showScaleElements_IsVisible = isVisible;
			_showScaleElements_Enabled = isEnabled;
			if (null != _view)
				_view.ShowScaleElements(isVisible, isEnabled);
		}

		public void ShowPositionElements(bool isVisible, bool isEnabled)
		{
			_showPositionElements_IsVisible = isVisible;
			_showPositionElements_Enabled = isEnabled;
			if (null != _view)
				_view.ShowPositionElements(isVisible, isEnabled);
		}

		public void ShowAnchorElements(bool isVisible, bool isEnabled)
		{
			_showAnchorElements_IsVisible = isVisible;
			_showAnchorElements_Enabled = isEnabled;
			if (null != _view)
				_view.ShowAnchorElements(isVisible, isEnabled);
		}

		public RADouble SizeX
		{
			get
			{
				RADouble result;
				var xSize = _view.XSize;

				if (object.ReferenceEquals(xSize.Unit, _percentLayerXSizeUnit))
					result = RADouble.NewRel(xSize.Value / 100);
				else
					result = RADouble.NewAbs(xSize.AsValueIn(AUL.Point.Instance));

				return result;
			}
			set
			{
				var xSize = value.IsAbsolute ? new DimensionfulQuantity(value.Value, AUL.Point.Instance) : new DimensionfulQuantity(value.Value * 100, _percentLayerXSizeUnit);
				_view.InitializeXSize(xSize, _xSizeEnvironment);
			}
		}

		public RADouble SizeY
		{
			get
			{
				RADouble result;
				var ySize = _view.YSize;

				if (object.ReferenceEquals(ySize.Unit, _percentLayerXSizeUnit))
					result = RADouble.NewRel(ySize.Value / 100);
				else
					result = RADouble.NewAbs(ySize.AsValueIn(AUL.Point.Instance));

				return result;
			}
			set
			{
				var ySize = value.IsAbsolute ? new DimensionfulQuantity(value.Value, AUL.Point.Instance) : new DimensionfulQuantity(value.Value * 100, _percentLayerYSizeUnit);
				_view.InitializeYSize(ySize, _ySizeEnvironment);
			}
		}

		public RADouble SizeZ
		{
			get
			{
				RADouble result;
				var zSize = _view.ZSize;

				if (object.ReferenceEquals(zSize.Unit, _percentLayerXSizeUnit))
					result = RADouble.NewRel(zSize.Value / 100);
				else
					result = RADouble.NewAbs(zSize.AsValueIn(AUL.Point.Instance));

				return result;
			}
			set
			{
				var zSize = value.IsAbsolute ? new DimensionfulQuantity(value.Value, AUL.Point.Instance) : new DimensionfulQuantity(value.Value * 100, _percentLayerYSizeUnit);
				_view.InitializeYSize(zSize, _zSizeEnvironment);
			}
		}

		public double ScaleX
		{
			get
			{
				return _view.ScaleX;
			}
			set
			{
				_view.ScaleX = value;
			}
		}

		public double ScaleY
		{
			get
			{
				return _view.ScaleY;
			}
			set
			{
				_view.ScaleY = value;
			}
		}

		public double ScaleZ
		{
			get
			{
				return _view.ScaleZ;
			}
			set
			{
				_view.ScaleZ = value;
			}
		}

		#endregion Service members
	}
}