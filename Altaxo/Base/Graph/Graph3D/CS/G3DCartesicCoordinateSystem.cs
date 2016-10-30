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
		private static Dictionary<G3DCartesicCoordinateSystem, IList<CSAxisInformation>> _axisInformationLists;

		private static Dictionary<G3DCartesicCoordinateSystem, IList<CSPlaneInformation>> _planeInformationLists;

		/// <summary>
		/// Is the normal position of x and y axes interchanged, for instance x is vertical and y horizontal.
		/// </summary>
		private bool _isXYInterchanged;

		/// <summary>
		/// Is the direction of the x axis reverse, for instance runs from right to left.
		/// </summary>
		protected bool _isXreversed;

		/// <summary>
		/// Is the direction of the y axis reverse, for instance runs from top to bottom.
		/// </summary>
		protected bool _isYreversed;

		/// <summary>
		/// Is the direction of the z axis reverse, for instance runs from top to bottom.
		/// </summary>
		protected bool _isZreversed;

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

				info.AddValue("XReverse", s._isXreversed);
				info.AddValue("YReverse", s._isYreversed);
				info.AddValue("ZReverse", s._isZreversed);
				info.AddValue("XYInterchanged", s._isXYInterchanged);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (G3DCartesicCoordinateSystem)o ?? new G3DCartesicCoordinateSystem();

				s._isXreversed = info.GetBoolean("XReverse");
				s._isYreversed = info.GetBoolean("YReverse");
				s._isZreversed = info.GetBoolean("ZReverse");
				s._isXYInterchanged = info.GetBoolean("XYInterchanged");

				return s;
			}
		}

		#endregion Version 0

		#endregion Serialization

		#region Construction and cloning

		private class ComparerForStaticDictionary : IEqualityComparer<G3DCartesicCoordinateSystem>
		{
			public bool Equals(G3DCartesicCoordinateSystem x, G3DCartesicCoordinateSystem y)
			{
				return
					x._isXYInterchanged == y._isXYInterchanged &&
					x._isXreversed == y._isXreversed &&
					x._isYreversed == y._isYreversed &&
					x._isZreversed == y._isZreversed;
			}

			public int GetHashCode(G3DCartesicCoordinateSystem obj)
			{
				return
					(obj._isXYInterchanged ? 1 : 0) +
					(obj._isXreversed ? 2 : 0) +
					(obj._isYreversed ? 4 : 0) +
					(obj._isZreversed ? 8 : 0);
			}
		}

		static G3DCartesicCoordinateSystem()
		{
			_axisInformationLists = new Dictionary<G3DCartesicCoordinateSystem, IList<CSAxisInformation>>(new ComparerForStaticDictionary());
			_planeInformationLists = new Dictionary<G3DCartesicCoordinateSystem, IList<CSPlaneInformation>>(new ComparerForStaticDictionary());
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

		public G3DCartesicCoordinateSystem WithXYInterchangedAndXYZReversed(bool isXYInterchanged, bool isXReversed, bool isYReversed, bool isZReversed)
		{
			if (
				_isXYInterchanged == isXYInterchanged &&
				_isXreversed == isXReversed &&
				_isYreversed == isYReversed &&
				_isZreversed == isZReversed
				)
			{
				return this;
			}
			else
			{
				var result = (G3DCartesicCoordinateSystem)this.MemberwiseClone();
				result._isXYInterchanged = isXYInterchanged;
				result._isXreversed = isXReversed;
				result._isYreversed = isYReversed;
				result._isZreversed = isZReversed;
				return result;
			}
		}

		/// <summary>
		/// Is the normal position of x and y axes interchanged, for instance x is vertical and y horizontal.
		/// </summary>
		public bool IsXYInterchanged
		{
			get { return _isXYInterchanged; }
		}

		public G3DCartesicCoordinateSystem WithXYInterchanged(bool isXYInterchanged)
		{
			if (_isXYInterchanged == isXYInterchanged)
			{
				return this;
			}
			else
			{
				var result = (G3DCartesicCoordinateSystem)this.MemberwiseClone();
				result._isXYInterchanged = isXYInterchanged;
				return result;
			}
		}

		/// <summary>
		/// Is the direction of the x axis reverse, for instance runs from right to left.
		/// </summary>
		public bool IsXReversed
		{
			get { return _isXreversed; }
		}

		public G3DCartesicCoordinateSystem WithXReversed(bool isXreversed)
		{
			if (_isXreversed == isXreversed)
			{
				return this;
			}
			else
			{
				var result = (G3DCartesicCoordinateSystem)this.MemberwiseClone();
				result._isXreversed = isXreversed;
				return result;
			}
		}

		/// <summary>
		/// Is the direction of the y axis reverse, for instance runs from top to bottom.
		/// </summary>
		public bool IsYReversed
		{
			get { return _isYreversed; }
		}

		public G3DCartesicCoordinateSystem WithYReversed(bool isYreversed)
		{
			if (_isYreversed == isYreversed)
			{
				return this;
			}
			else
			{
				var result = (G3DCartesicCoordinateSystem)this.MemberwiseClone();
				result._isYreversed = isYreversed;
				return result;
			}
		}

		/// <summary>
		/// Is the direction of the y axis reverse, for instance runs from top to bottom.
		/// </summary>
		public bool IsZReversed
		{
			get { return _isZreversed; }
		}

		public G3DCartesicCoordinateSystem WithZReversed(bool isZreversed)
		{
			if (_isZreversed == isZreversed)
			{
				return this;
			}
			else
			{
				var result = (G3DCartesicCoordinateSystem)this.MemberwiseClone();
				result._isZreversed = isZreversed;
				return result;
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

			rx = _isXreversed ? 1 - rx : rx;
			ry = _isYreversed ? 1 - ry : ry;
			rz = _isZreversed ? 1 - rz : rz;
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
			double rx = _isXreversed ? 1 - r.RX : r.RX;
			double ry = _isYreversed ? 1 - r.RY : r.RY;
			double rz = _isZreversed ? 1 - r.RZ : r.RZ;
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

		public override IEnumerable<CSAxisInformation> AxisStyles
		{
			get
			{
				IList<CSAxisInformation> result;
				if (!_axisInformationLists.TryGetValue(this, out result))
				{
					result = GetAxisStyleInformations();
					lock (this)
					{
						_axisInformationLists[(G3DCartesicCoordinateSystem)this.WithLayerSize(VectorD3D.Empty)] = result;
					}
				}
				return result;
			}
		}

		public override IEnumerable<CSPlaneInformation> PlaneStyles
		{
			get
			{
				IList<CSPlaneInformation> result;
				if (!_planeInformationLists.TryGetValue(this, out result))
				{
					result = GetPlaneStyleInformations();
					lock (this)
					{
						_planeInformationLists[(G3DCartesicCoordinateSystem)this.WithLayerSize(VectorD3D.Empty)] = result;
					}
				}
				return result;
			}
		}

		/// <summary>
		/// Gets the axis line vector. This is a vector pointing from the origin to the axis line, when the layer is assumed to be a square of 2x2x2 size, centered at the origin.
		/// Thus the returned vector has one member set to zero, and the other two members set either to +1 or -1.
		/// Attention: This returns the untransformed vector, i.e. the vector assuming a regular G3DCoordinateSystem without reversing or exchanging of axes).
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>Axis line vector (untransformed, i.e. only for a regular G3DCoordinateSystem without reversing or exchanging of axes).</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public static VectorD3D GetUntransformedAxisLineVector(CSLineID id)
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

		public static VectorD3D GetUntransformedAxisPlaneVector(CSPlaneID id)
		{
			switch (id.PerpendicularAxisNumber)
			{
				case 0: // parallel axis is X
					return new VectorD3D(1, 0, 0);

				case 1:
					return new VectorD3D(0, 1, 0);

				case 2:
					return new VectorD3D(0, 0, 1);

				default:
					throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Gets the axis line vector. This is a vector pointing from the origin to the axis line, when the layer is assumed to be a square of 2x2x2 size, centered at the origin.
		/// Thus the returned vector has one member set to zero, and the other two members set either to +1 or -1.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns>Axis line vector (transformed, thus taking into account the properties for reversing and exchanging of axes).</returns>
		public VectorD3D GetTransformedAxisLineVector(CSLineID id)
		{
			return VectorTransformation.Transform(GetUntransformedAxisLineVector(id));
		}

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

		/// <summary>
		/// Gets the transformed axis side vector, i.e. the vector that points out of the plane of the axis side.
		/// </summary>
		/// <param name="id">The axis identifier.</param>
		/// <param name="side">The side identifier.</param>
		/// <returns>The vector corresponding to the axis side.</returns>
		/// <exception cref="System.NotImplementedException">
		/// </exception>
		private VectorD3D GetTransformedAxisSideVector(CSLineID id, CSAxisSide side)
		{
			return this.VectorTransformation.Transform(GetUntransformedAxisSideVector(id, side));
		}

		/// <summary>
		/// Given a vector <paramref name="v"/> and a line id, this returns the axis side this vector belongs to.
		/// An exception is thrown if the vector does not belong to one of the four axis sides.
		/// Attention: this function does not take properties like ExchangeXY or IsXReversed into account!
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="v">The vector that describes the axis side.</param>
		/// <returns></returns>
		/// <exception cref="System.NotImplementedException"></exception>
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
			if (-1 == c1 && 0 == c2)
				return CSAxisSide.FirstDown;
			else if (+1 == c1 && 0 == c2)
				return CSAxisSide.FirstUp;
			else if (-1 == c2 && 0 == c1)
				return CSAxisSide.SecondDown;
			else if (1 == c2 && 0 == c1)
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
						0, _isXreversed ? -1 : 1, 0,
						_isYreversed ? -1 : 1, 0, 0,
						0, 0, _isZreversed ? -1 : 1);
				else
					return new Matrix3x3(
						_isXreversed ? -1 : 1, 0, 0,
						0, _isYreversed ? -1 : 1, 0,
						0, 0, _isZreversed ? -1 : 1);
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
			var v = GetUntransformedAxisLineVector(id);
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
			var u_axisVector = GetUntransformedAxisLineVector(id);
			var t_axisVector = VectorTransformation.Transform(u_axisVector);
			var t_labelSide = GetPreferredLabelSide(t_axisVector);
			var u_labelSide = VectorTransformation.InverseTransform(t_labelSide);
			return GetAxisSide(id, u_labelSide);
		}

		private bool GetHasLabelsByDefault(CSLineID lineId)
		{
			var uv = GetUntransformedAxisLineVector(lineId);
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

		private IList<CSAxisInformation> GetAxisStyleInformations()
		{
			var axisStyleInformations = new List<CSAxisInformation>();

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

						axisStyleInformations.Add(item);
					}
				}
			}

			return axisStyleInformations.AsReadOnly();
		}

		private IList<CSPlaneInformation> GetPlaneStyleInformations()
		{
			var axisStyleInformations = new List<CSPlaneInformation>();

			for (int axisnumber = 0; axisnumber <= 2; ++axisnumber)
			{
				for (int firstother = 0; firstother <= 1; ++firstother)
				{
					var planeId = new CSPlaneID(axisnumber, firstother);
					string name = GetNameOfPlane(planeId);

					var item = new CSPlaneInformation(planeId) { Name = name };

					axisStyleInformations.Add(item);
				}
			}

			return axisStyleInformations.AsReadOnly();
		}

		public override string GetNameOfPlane(CSPlaneID planeId)
		{
			string name = "";
			if (planeId.UsePhysicalValue)
			{
				switch (planeId.PerpendicularAxisNumber)
				{
					case 0:
						name = string.Format("X = {0}", planeId.PhysicalValue);
						break;

					case 1:
						name = string.Format("Y = {0}", planeId.PhysicalValue);
						break;

					case 2:
						name = string.Format("Z = {0}", planeId.PhysicalValue);
						break;

					default:
						throw new NotImplementedException();
				}
			}
			else
			{
				var uv = GetUntransformedAxisPlaneVector(planeId);
				var tv = VectorTransformation.Transform(uv);

				var lv = planeId.LogicalValue;

				if (tv.X == -1 || tv.Y == -1 || tv.Z == -1)
					lv = 1 - lv;
				if (Math.Abs(tv.X) == 1)
				{
					if (lv == 0)
						name = "Left";
					else if (lv == 1)
						name = "Right";
					else
						name = string.Format("{0}% between left and right", lv * 100);
				}
				else if (Math.Abs(tv.Y) == 1)
				{
					if (lv == 0)
						name = "Front";
					else if (lv == 1)
						name = "Back";
					else
						name = string.Format("{0}% between front and back", lv * 100);
				}
				else if (Math.Abs(tv.Z) == 1)
				{
					if (lv == 0)
						name = "Bottom";
					else if (lv == 1)
						name = "Top";
					else
						name = string.Format("{0}% between bottom and top", lv * 100);
				}
				else
				{
					throw new NotImplementedException();
				}
			}

			return name;
		}

		#endregion Axis naming

		#region Utility functions

		/// <summary>
		/// When changing the properties of the coordinate systen (e.g. reversing x-axis), the axis including ticks and labels will move from one end of the graph to the other.
		/// In order to keep the axes at their location, new <see cref="CSLineID"/>s have to be found for the new coordinate system, that correspond to the same axis location
		/// in the old coordinate system. This function returns a new <see cref="CSLineID"/>, or null if no corresponding <see cref="CSLineID"/> could be found.
		/// </summary>
		/// <param name="oldCoordinateSystem">The old coordinate system.</param>
		/// <param name="oldLineID">The old line identifier of the axis.</param>
		/// <param name="newCoordinateSystem">The new coordinate system.</param>
		/// <returns>The new line identifier, that refers to the same location in the new coordinate systems as the old line identifer referes in the old coordinate system. If no such
		/// identifer could be found, null is returned.</returns>
		public static CSLineID FindCorrespondingCSLineIDWhenChangingCoordinateSystem(G3DCartesicCoordinateSystem oldCoordinateSystem, CSLineID oldLineID, G3DCartesicCoordinateSystem newCoordinateSystem)
		{
			var oldAxisLineVector = oldCoordinateSystem.GetTransformedAxisLineVector(oldLineID);

			for (int i = 0; i < 3; ++i)
			{
				for (int j = 0; j < 2; ++j)
				{
					for (int k = 0; k < 2; ++k)
					{
						var newLineID = new CSLineID(i, j, k);
						var newAxisLineVector = newCoordinateSystem.GetTransformedAxisLineVector(newLineID);
						if (oldAxisLineVector == newAxisLineVector)
							return newLineID;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Finds the corresponding new axis side when changing the coordinate system.
		/// </summary>
		/// <param name="oldCoordinateSystem">The old coordinate system system.</param>
		/// <param name="oldLineID">The old line identifier of the axis.</param>
		/// <param name="oldAxisSide">The old axis side.</param>
		/// <param name="newCoordinateSystem">The new coordinate system.</param>
		/// <param name="newLineID">The new line identifier of the axis (in the new coordinate system).</param>
		/// <returns>The new axis side. The new axis side as vector in the new coordinate system has the same direction as the old axis side vector in the old coordinate system.
		/// The return value is null if no axis side with the same direction could be found (this is the case for instance when exchanging x and y axis).</returns>
		public static CSAxisSide? FindCorrespondingAxisSideWhenChangingCoordinateSystem(G3DCartesicCoordinateSystem oldCoordinateSystem, CSLineID oldLineID, CSAxisSide oldAxisSide, G3DCartesicCoordinateSystem newCoordinateSystem, CSLineID newLineID)
		{
			var t_oldAxisSide = oldCoordinateSystem.GetTransformedAxisSideVector(oldLineID, oldAxisSide);
			var u_newAxisSide = newCoordinateSystem.VectorTransformation.InverseTransform(t_oldAxisSide);
			try
			{
				return GetAxisSide(newLineID, u_newAxisSide);
			}
			catch (Exception)
			{
				return null;
			}
		}

		#endregion Utility functions
	}
}