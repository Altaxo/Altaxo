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

using Altaxo.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Graph.Graph3D.CS
{
	public class G3DCartesicCoordinateSystem : G3DCoordinateSystem
	{
		/// <summary>
		/// Is the normal position of x and y axes interchanged, for instance x is vertical and y horizontal.
		/// </summary>
		private bool _isXYInterchanged;

		/// <summary>
		/// Is the direction of the x axis reverse, for instance runs from right to left.
		/// </summary>
		protected bool _isXreverse;

		/// <summary>
		/// Is the direction of the y axis reverse, for instance runs from top to bottom.
		/// </summary>
		protected bool _isYreverse;

		/// <summary>
		/// Is the direction of the z axis reverse, for instance runs from top to bottom.
		/// </summary>
		protected bool _isZreverse;

		#region Serialization

		#region Version 0

		/// <summary>
		/// 2015-11-14 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(G3DCartesicCoordinateSystem), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (G3DCartesicCoordinateSystem)obj;

				info.AddValue("XReverse", s._isXreverse);
				info.AddValue("YReverse", s._isYreverse);
				info.AddValue("ZReverse", s._isZreverse);
				info.AddValue("XYInterchanged", s._isXYInterchanged);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (G3DCartesicCoordinateSystem)o ?? new G3DCartesicCoordinateSystem();

				s._isXreverse = info.GetBoolean("XReverse");
				s._isYreverse = info.GetBoolean("YReverse");
				s._isZreverse = info.GetBoolean("ZReverse");
				s._isXYInterchanged = info.GetBoolean("XYInterchanged");

				return s;
			}
		}

		#endregion Version 0

		#endregion Serialization

		#region Construction and cloning

		/// <summary>
		/// Copies the member variables from another coordinate system.
		/// </summary>
		/// <param name="fromb">The coordinate system to copy from.</param>
		public override void CopyFrom(G3DCoordinateSystem fromb)
		{
			if (object.ReferenceEquals(this, fromb))
				return;

			base.CopyFrom(fromb);
			if (fromb is G3DCartesicCoordinateSystem)
			{
				var from = (G3DCartesicCoordinateSystem)fromb;
				this._isXreverse = from._isXreverse;
				this._isYreverse = from._isYreverse;
				this._isZreverse = from._isZreverse;
				this._isXYInterchanged = from._isXYInterchanged;
			}
		}

		public override object Clone()
		{
			var result = new G3DCartesicCoordinateSystem();
			result.CopyFrom(this);
			return result;
		}

		#endregion Construction and cloning

		#region Properties

		public override bool Is3D
		{
			get
			{
				return true;
			}
		}

		public override bool IsAffine
		{
			get
			{
				return true;
			}
		}

		public override bool IsOrthogonal
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Is the normal position of x and y axes interchanged, for instance x is vertical and y horizontal.
		/// </summary>
		public bool IsXYInterchanged
		{
			get { return _isXYInterchanged; }
			set
			{
				if (_isXYInterchanged != value)
				{
					_isXYInterchanged = value;
					ClearCachedObjects();
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Is the direction of the x axis reverse, for instance runs from right to left.
		/// </summary>
		public bool IsXReverse
		{
			get { return _isXreverse; }
			set
			{
				if (_isXreverse != value)
				{
					_isXreverse = value;
					ClearCachedObjects();
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Is the direction of the y axis reverse, for instance runs from top to bottom.
		/// </summary>
		public bool IsYReverse
		{
			get { return _isYreverse; }
			set
			{
				if (_isYreverse != value)
				{
					_isYreverse = value;
					ClearCachedObjects();
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Is the direction of the y axis reverse, for instance runs from top to bottom.
		/// </summary>
		public bool IsZReverse
		{
			get { return _isZreverse; }
			set
			{
				if (_isZreverse != value)
				{
					_isZreverse = value;
					ClearCachedObjects();
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		#endregion Properties

		#region Logical to physical conversion and vice versa

		public override IPolylineD3D GetIsoline(Logical3D r0, Logical3D r1)
		{
			PointD3D pt0, pt1;
			LogicalToLayerCoordinates(r0, out pt0);
			LogicalToLayerCoordinates(r1, out pt1);
			return new StraightLineAsPolylineD3D(pt0, pt1);
		}

		public override bool LayerToLogicalCoordinates(PointD3D location, out Logical3D r)
		{
			double rx = location.X / _layerSize.X;
			double ry = location.Y / _layerSize.Y;
			double rz = location.Z / _layerSize.Z;

			rx = _isXreverse ? 1 - rx : rx;
			ry = _isYreverse ? 1 - ry : ry;
			rz = _isZreverse ? 1 - rz : rz;
			if (_isXYInterchanged)
			{
				double hr = rx;
				rx = ry;
				ry = hr;
			}

			r = new Logical3D(rx, ry, rz);
			return !double.IsNaN(rx) && !double.IsNaN(ry) && !double.IsNaN(rz);
		}

		public override bool LogicalToLayerCoordinates(Logical3D r, out PointD3D location)
		{
			double rx = _isXreverse ? 1 - r.RX : r.RX;
			double ry = _isYreverse ? 1 - r.RY : r.RY;
			double rz = _isZreverse ? 1 - r.RZ : r.RZ;
			if (_isXYInterchanged)
			{
				double hr = rx;
				rx = ry;
				ry = hr;
			}
			location = new PointD3D(rx * _layerSize.X, ry * _layerSize.Y, rz * _layerSize.Z);

			return !double.IsNaN(rx) && !double.IsNaN(ry) && !double.IsNaN(rz);
		}

		public override bool LogicalToLayerCoordinatesAndDirection(Logical3D r0, Logical3D r1, double t, out PointD3D position, out VectorD3D direction)
		{
			PointD3D pt0, pt1;
			LogicalToLayerCoordinates(r0, out pt0);
			LogicalToLayerCoordinates(r1, out pt1);
			LogicalToLayerCoordinates(r0.InterpolateTo(r1, t), out position);
			direction = (pt1 - pt0).Normalized;
			return true;
		}

		#endregion Logical to physical conversion and vice versa

		#region Axis naming

		/// <summary>
		/// Gets the untransformed axis side vector, the the vector that points out of the plane of the axis side.
		/// </summary>
		/// <param name="id">The axis identifier.</param>
		/// <param name="side">The side identifier.</param>
		/// <returns>The vector corresponding to the axis side.</returns>
		/// <exception cref="System.NotImplementedException">
		/// </exception>
		private static VectorD3D GetUntransformedAxisSideVector(CSLineID id, CSAxisSide side)
		{
			VectorD3D r = VectorD3D.Empty;
			switch (id.ParallelAxisNumber)
			{
				case 0: // parallel axis is X
					{
						switch (side)
						{
							case CSAxisSide.FirstDown:
								r = new VectorD3D(0, -1, 0); // "front";
								break;

							case CSAxisSide.FirstUp:
								r = new VectorD3D(0, 1, 0); // "back";
								break;

							case CSAxisSide.SecondDown:
								r = new VectorD3D(0, 0, -1); //"down";
								break;

							case CSAxisSide.SecondUp:
								r = new VectorD3D(0, 0, 1); // "up";
								break;

							default:
								throw new NotImplementedException();
						}
					}
					break;

				case 1: // parallel axis is Y
					{
						switch (side)
						{
							case CSAxisSide.FirstDown:
								r = new VectorD3D(-1, 0, 0); // "left";
								break;

							case CSAxisSide.FirstUp:
								r = new VectorD3D(1, 0, 0); // "right";
								break;

							case CSAxisSide.SecondDown:
								r = new VectorD3D(0, 0, -1); // "down";
								break;

							case CSAxisSide.SecondUp:
								r = new VectorD3D(0, 0, 1); // "up";
								break;

							default:
								throw new NotImplementedException();
						}
					}
					break;

				case 2: // parallel axis is Z
					{
						switch (side)
						{
							case CSAxisSide.FirstDown:
								r = new VectorD3D(-1, 0, 0); // "left";
								break;

							case CSAxisSide.FirstUp:
								r = new VectorD3D(1, 0, 0); // "right";
								break;

							case CSAxisSide.SecondDown:
								r = new VectorD3D(0, -1, 0); // "front";
								break;

							case CSAxisSide.SecondUp:
								r = new VectorD3D(0, 1, 0); // "back";
								break;

							default:
								throw new NotImplementedException();
						}
					}
					break;

				default:
					throw new NotImplementedException();
			}

			return r;
		}

		public VectorD3D GetAxisLineVector(CSLineID id)
		{
			switch (id.ParallelAxisNumber)
			{
				case 0: // parallel axis is X
					return new VectorD3D(0, id.LogicalValueOtherFirst * 2 - 1, id.LogicalValueOtherSecond * 2 - 1);

				case 1:
					return new VectorD3D(id.LogicalValueOtherFirst * 2 - 1, 0, id.LogicalValueOtherSecond * 2 - 1);

				case 2:
					return new VectorD3D(id.LogicalValueOtherFirst * 2 - 1, id.LogicalValueOtherSecond * 2 - 1, 0);

				default:
					throw new NotImplementedException();
			}
		}

		private static CSAxisSide GetAxisSide(CSLineID id, VectorD3D v)
		{
			switch (id.ParallelAxisNumber)
			{
				case 0:
					return GetAxisSide(v.Y, v.Z);

				case 1:
					return GetAxisSide(v.X, v.Z);

				case 2:
					return GetAxisSide(v.X, v.Y);

				default:
					throw new NotImplementedException();
			}
		}

		private static CSAxisSide GetAxisSide(double c1, double c2)
		{
			if (-1 == c1)
				return CSAxisSide.FirstDown;
			else if (+1 == c1)
				return CSAxisSide.FirstUp;
			else if (-1 == c2)
				return CSAxisSide.SecondDown;
			else if (1 == c2)
				return CSAxisSide.SecondUp;
			else
				throw new ArgumentOutOfRangeException("For arguments c1 and c2: one is expected to be 0, the other argument to be either +1 or -1");
		}

		private Matrix3x3 VectorTransformation
		{
			get
			{
				if (_isXYInterchanged)
					return new Matrix3x3(
						0, _isXreverse ? -1 : 1, 0,
						_isYreverse ? -1 : 1, 0, 0,
						0, 0, _isZreverse ? -1 : 1);
				else
					return new Matrix3x3(
						_isXreverse ? -1 : 1, 0, 0,
						0, _isYreverse ? -1 : 1, 0,
						0, 0, _isZreverse ? -1 : 1);
			}
		}

		private static string GetAxisSideNameFromVector(VectorD3D v)
		{
			if (v.X == 1)
				return "right";
			else if (v.X == -1)
				return "left";
			else if (v.Y == 1)
				return "back";
			else if (v.Y == -1)
				return "front";
			else if (v.Z == 1)
				return "up";
			else if (v.Z == -1)
				return "down";
			else throw new ArgumentOutOfRangeException("The vector v was expected to have only one element either being +1 or -1");
		}

		public override string GetAxisSideName(CSLineID id, CSAxisSide side)
		{
			var v = GetUntransformedAxisSideVector(id, side);
			var tv = VectorTransformation.Transform(v);
			var name = GetAxisSideNameFromVector(tv);
			return name;
		}

		public string GetAxisLineName(VectorD3D v)
		{
			string specname;
			if (0 == v.X)
			{
				if (-1 == v.Y && -1 == v.Z)
					specname = "bottom-front";
				else if (1 == v.Y && -1 == v.Z)
					specname = "bottom-back";
				else if (-1 == v.Y && 1 == v.Z)
					specname = "top-front";
				else if (1 == v.Y && 1 == v.Z)
					specname = "top-back";
				else
					throw new ArgumentOutOfRangeException("Vector v is expected to have one element set to 0, and the other elements either being +1 or -1");
			}
			else if (0 == v.Y)
			{
				if (-1 == v.X && -1 == v.Z)
					specname = "left-bottom";
				else if (1 == v.X && -1 == v.Z)
					specname = "right-bottom";
				else if (-1 == v.X && 1 == v.Z)
					specname = "left-top";
				else if (1 == v.X && 1 == v.Z)
					specname = "right-top";
				else
					throw new ArgumentOutOfRangeException("Vector v is expected to have one element set to 0, and the other elements either being +1 or -1");
			}
			else if (0 == v.Z)
			{
				if (-1 == v.X && -1 == v.Y)
					specname = "left-front";
				else if (1 == v.X && -1 == v.Y)
					specname = "right-front";
				else if (-1 == v.X && 1 == v.Y)
					specname = "left-back";
				else if (1 == v.X && 1 == v.Y)
					specname = "right-back";
				else
					throw new ArgumentOutOfRangeException("Vector v is expected to have one element set to 0, and the other elements either being +1 or -1");
			}
			else
			{
				throw new ArgumentOutOfRangeException("Vector v is expected to have one element set to 0, and the other elements either being +1 or -1");
			}
			return specname;
		}

		public string GetAxisLineName(CSLineID id)
		{
			var v = GetAxisLineVector(id);
			var tv = VectorTransformation.Transform(v);
			var name = GetAxisLineName(tv);
			return name;
		}

		public VectorD3D GetPreferredLabelSide(VectorD3D v)
		{
			VectorD3D result;
			if (0 == v.X)
			{
				if (-1 == v.Y && -1 == v.Z)
					result = new VectorD3D(0, 0, -1); // "bottom-front";
				else if (1 == v.Y && -1 == v.Z)
					result = new VectorD3D(0, 0, -1); // "bottom-back";
				else if (-1 == v.Y && 1 == v.Z)
					result = new VectorD3D(0, 0, 1); //"top-front";
				else if (1 == v.Y && 1 == v.Z)
					result = new VectorD3D(0, 0, 1); // "top-back";
				else
					throw new ArgumentOutOfRangeException("Vector v is expected to have one element set to 0, and the other elements either being +1 or -1");
			}
			else if (0 == v.Y)
			{
				if (-1 == v.X && -1 == v.Z)
					result = new VectorD3D(-1, 0, 0); // "left-bottom";
				else if (1 == v.X && -1 == v.Z)
					result = new VectorD3D(+1, 0, 0); // "right-bottom";
				else if (-1 == v.X && 1 == v.Z)
					result = new VectorD3D(-1, 0, 0); // "left-top";
				else if (1 == v.X && 1 == v.Z)
					result = new VectorD3D(+1, 0, 0); // "right-top";
				else
					throw new ArgumentOutOfRangeException("Vector v is expected to have one element set to 0, and the other elements either being +1 or -1");
			}
			else if (0 == v.Z)
			{
				if (-1 == v.X && -1 == v.Y)
					result = new VectorD3D(-1, 0, 0); // "left-front";
				else if (1 == v.X && -1 == v.Y)
					result = new VectorD3D(+1, 0, 0); // "right-front";
				else if (-1 == v.X && 1 == v.Y)
					result = new VectorD3D(-1, 0, 0); // "left-back";
				else if (1 == v.X && 1 == v.Y)
					result = new VectorD3D(+1, 0, 0); // "right-back";
				else
					throw new ArgumentOutOfRangeException("Vector v is expected to have one element set to 0, and the other elements either being +1 or -1");
			}
			else
			{
				throw new ArgumentOutOfRangeException("Vector v is expected to have one element set to 0, and the other elements either being +1 or -1");
			}
			return result;
		}

		public CSAxisSide GetPreferredLabelSide(CSLineID id)
		{
			var u_axisVector = GetAxisLineVector(id);
			var t_axisVector = VectorTransformation.Transform(u_axisVector);
			var t_labelSide = GetPreferredLabelSide(t_axisVector);
			var u_labelSide = VectorTransformation.InverseTransformVector(t_labelSide);
			return GetAxisSide(id, u_labelSide);
		}

		private bool GetHasLabelsByDefault(CSLineID lineId)
		{
			var uv = GetAxisLineVector(lineId);
			var v = VectorTransformation.Transform(uv);
			bool result = false;
			if (0 == v.X) // x-axis
				result = -1 == v.Y && -1 == v.Z; // front-bottom
			else if (0 == v.Y) // y-axis
				result = -1 == v.X && -1 == v.Z; // front-left
			else if (0 == v.Z) // z-axis
				result = 1 == v.X && -1 == v.Y; // front - right
			else
			{
				throw new NotImplementedException();
			}
			return result;
		}

		protected override void UpdateAxisInfo()
		{
			if (null != _axisStyleInformation)
				_axisStyleInformation.Clear();
			else
				_axisStyleInformation = new List<CSAxisInformation>();

			for (int axisnumber = 0; axisnumber <= 2; ++axisnumber)
			{
				for (int firstother = 0; firstother <= 1; ++firstother)
				{
					for (int secondother = 0; secondother <= 1; ++secondother)
					{
						var lineId = new CSLineID(axisnumber, firstother, secondother);
						var item = new CSAxisInformation(
							Identifier: lineId,
							NameOfAxisStyle: GetAxisLineName(lineId),
							NameOfFirstDownSide: GetAxisSideName(lineId, CSAxisSide.FirstDown),
							NameOfFirstUpSide: GetAxisSideName(lineId, CSAxisSide.FirstUp),
							NameOfSecondDownSide: GetAxisSideName(lineId, CSAxisSide.SecondDown),
							NameOfSecondUpSide: GetAxisSideName(lineId, CSAxisSide.SecondUp),
							PreferredLabelSide: GetPreferredLabelSide(lineId),
							PreferredTickSide: GetPreferredLabelSide(lineId),
							IsShownByDefault: true, // lineId.LogicalValueOtherFirst :: 0 && lineId.LogicalValueOtherSecond :: 0.
							HasTicksByDefault: true,
							HasLabelsByDefault: GetHasLabelsByDefault(lineId),
							HasTitleByDefault: GetHasLabelsByDefault(lineId));

						_axisStyleInformation.Add(item);
					}
				}
			}
		}

		#endregion Axis naming
	}
}