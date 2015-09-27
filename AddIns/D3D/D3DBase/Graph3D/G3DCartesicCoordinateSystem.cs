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

using Altaxo.Graph;
using Altaxo.Graph3D.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Graph3D
{
	public class G3DCartesicCoordinateSystem : G3DCoordinateSystem
	{
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

		public override object Clone()
		{
			return new G3DCartesicCoordinateSystem();
		}

		public override ISweepPath3D GetIsoline(Logical3D r0, Logical3D r1)
		{
			PointD3D pt0, pt1;
			LogicalToLayerCoordinates(r0, out pt0);
			LogicalToLayerCoordinates(r1, out pt1);
			return new Primitives.StraightLineSweepPath3D(pt0, pt1);
		}

		public override bool LayerToLogicalCoordinates(PointD3D location, out Logical3D r)
		{
			r = new Logical3D(location.X / _layerSize.X, location.Y / _layerSize.Y, location.Z / _layerSize.Z);
			return true;
		}

		public override bool LogicalToLayerCoordinates(Logical3D r, out PointD3D location)
		{
			location = new PointD3D(r.RX * _layerSize.X, r.RY * _layerSize.Y, r.RZ * _layerSize.Z);
			return true;
		}

		public override bool LogicalToLayerCoordinatesAndDirection(Logical3D r0, Logical3D r1, double t, out PointD3D position, out VectorD3D direction)
		{
			PointD3D pt0, pt1, ptt;
			LogicalToLayerCoordinates(r0, out pt0);
			LogicalToLayerCoordinates(r1, out pt1);
			LogicalToLayerCoordinates(r0.InterpolateTo(r1, t), out position);
			direction = (pt1 - pt0);
			direction.Normalize();
			return true;
		}

		public override string GetAxisSideName(CSLineID id, CSAxisSide side)
		{
			string name = "Unknown";
			switch (id.ParallelAxisNumber)
			{
				case 0: // parallel axis is X
					{
						switch (side)
						{
							case CSAxisSide.FirstDown:
								name = "front";
								break;

							case CSAxisSide.FirstUp:
								name = "back";
								break;

							case CSAxisSide.SecondDown:
								name = "down";
								break;

							case CSAxisSide.SecondUp:
								name = "up";
								break;
						}
					}
					break;

				case 1: // parallel axis is Y
					{
						switch (side)
						{
							case CSAxisSide.FirstDown:
								name = "left";
								break;

							case CSAxisSide.FirstUp:
								name = "right";
								break;

							case CSAxisSide.SecondDown:
								name = "down";
								break;

							case CSAxisSide.SecondUp:
								name = "up";
								break;
						}
					}
					break;

				case 2: // parallel axis is Z
					{
						switch (side)
						{
							case CSAxisSide.FirstDown:
								name = "left";
								break;

							case CSAxisSide.FirstUp:
								name = "right";
								break;

							case CSAxisSide.SecondDown:
								name = "front";
								break;

							case CSAxisSide.SecondUp:
								name = "back";
								break;
						}
					}
					break;
			}

			return name;
		}

		public string GetAxisLineName(CSLineID id)
		{
			string basename = "";
			string specname = "";
			string name = "Unknown";
			switch (id.ParallelAxisNumber)
			{
				case 0: // parallel axis is X
					{
						basename = "X-axis";
						if (id.LogicalValueOtherFirst == 0 && id.LogicalValueOtherSecond == 0)
							specname = "bottom-front";
						else if (id.LogicalValueOtherFirst == 1 && id.LogicalValueOtherSecond == 0)
							specname = "bottom-back";
						else if (id.LogicalValueOtherFirst == 0 && id.LogicalValueOtherSecond == 1)
							specname = "top-front";
						else if (id.LogicalValueOtherFirst == 1 && id.LogicalValueOtherSecond == 1)
							specname = "top-back";
					}
					break;

				case 1: // parallel axis is Y
					{
						basename = "Y-axis";
						if (id.LogicalValueOtherFirst == 0 && id.LogicalValueOtherSecond == 0)
							specname = "left-bottom";
						else if (id.LogicalValueOtherFirst == 1 && id.LogicalValueOtherSecond == 0)
							specname = "right-bottom";
						else if (id.LogicalValueOtherFirst == 0 && id.LogicalValueOtherSecond == 1)
							specname = "left-top";
						else if (id.LogicalValueOtherFirst == 1 && id.LogicalValueOtherSecond == 1)
							specname = "right-top";
					}
					break;

				case 2: // parallel axis is Z
					{
						basename = "Z-axis";
						if (id.LogicalValueOtherFirst == 0 && id.LogicalValueOtherSecond == 0)
							specname = "left-front";
						else if (id.LogicalValueOtherFirst == 1 && id.LogicalValueOtherSecond == 0)
							specname = "right-front";
						else if (id.LogicalValueOtherFirst == 0 && id.LogicalValueOtherSecond == 1)
							specname = "left-back";
						else if (id.LogicalValueOtherFirst == 1 && id.LogicalValueOtherSecond == 1)
							specname = "right-back";
					}
					break;
			}

			return basename + "(" + specname + ")";
		}

		public CSAxisSide GetPreferredLabelSide(CSLineID id)
		{
			CSAxisSide axisSide = CSAxisSide.FirstDown;

			switch (id.ParallelAxisNumber)
			{
				case 0: // parallel axis is X
					{
						if (id.LogicalValueOtherFirst == 0 && id.LogicalValueOtherSecond == 0)
							axisSide = CSAxisSide.FirstDown; // "bottom-front";
						else if (id.LogicalValueOtherFirst == 1 && id.LogicalValueOtherSecond == 0)
							axisSide = CSAxisSide.FirstUp; //  "bottom-back";
						else if (id.LogicalValueOtherFirst == 0 && id.LogicalValueOtherSecond == 1)
							axisSide = CSAxisSide.SecondUp;// "top-front";
						else if (id.LogicalValueOtherFirst == 1 && id.LogicalValueOtherSecond == 1)
							axisSide = CSAxisSide.SecondUp; // "top-back";
					}
					break;

				case 1: // parallel axis is Y
					{
						if (id.LogicalValueOtherFirst == 0 && id.LogicalValueOtherSecond == 0)
							axisSide = CSAxisSide.FirstDown; // "left-bottom";
						else if (id.LogicalValueOtherFirst == 1 && id.LogicalValueOtherSecond == 0)
							axisSide = CSAxisSide.FirstUp; // "right-bottom";
						else if (id.LogicalValueOtherFirst == 0 && id.LogicalValueOtherSecond == 1)
							axisSide = CSAxisSide.FirstDown; //	"left-top";
						else if (id.LogicalValueOtherFirst == 1 && id.LogicalValueOtherSecond == 1)
							axisSide = CSAxisSide.FirstUp; // "right-top";
					}
					break;

				case 2: // parallel axis is Z
					{
						if (id.LogicalValueOtherFirst == 0 && id.LogicalValueOtherSecond == 0)
							axisSide = CSAxisSide.FirstDown; // "left-front";
						else if (id.LogicalValueOtherFirst == 1 && id.LogicalValueOtherSecond == 0)
							axisSide = CSAxisSide.FirstUp; // "right-front";
						else if (id.LogicalValueOtherFirst == 0 && id.LogicalValueOtherSecond == 1)
							axisSide = CSAxisSide.FirstDown; // "left-back";
						else if (id.LogicalValueOtherFirst == 1 && id.LogicalValueOtherSecond == 1)
							axisSide = CSAxisSide.FirstUp; // "right-back";
					}
					break;
			}

			return axisSide;
		}

		protected override void UpdateAxisInfo()
		{
			_axisStyleInformation.Clear();

			for (int axisnumber = 0; axisnumber <= 2; ++axisnumber)
			{
				for (int firstother = 0; firstother <= 1; ++firstother)
				{
					for (int secondother = 0; secondother <= 1; ++secondother)
					{
						var lineId = new CSLineID(axisnumber, firstother, secondother);
						var item = new CSAxisInformation(lineId);

						item.NameOfFirstDownSide = GetAxisSideName(lineId, CSAxisSide.FirstDown);
						item.NameOfFirstUpSide = GetAxisSideName(lineId, CSAxisSide.FirstUp);
						item.NameOfSecondDownSide = GetAxisSideName(lineId, CSAxisSide.SecondDown);
						item.NameOfSecondUpSide = GetAxisSideName(lineId, CSAxisSide.SecondUp);
						item.NameOfAxisStyle = GetAxisLineName(lineId);
						item.PreferedLabelSide = GetPreferredLabelSide(lineId);
						item.IsShownByDefault = lineId.LogicalValueOtherFirst == 0 && lineId.LogicalValueOtherSecond == 0;
						item.HasTitleByDefault = lineId.LogicalValueOtherFirst == 0 && lineId.LogicalValueOtherSecond == 0;

						_axisStyleInformation.Add(item);
					}
				}
			}
		}
	}
}