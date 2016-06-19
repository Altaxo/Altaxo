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

using Altaxo.Drawing.D3D;
using Altaxo.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Drawing.D3D
{
	/// <summary>
	/// Represents the solid geometry of a polyline in 3D space.
	/// </summary>
	public struct SolidPolyline
	{
		private static readonly VectorD3D _xVector = new VectorD3D(1, 0, 0);
		private static readonly VectorD3D _yVector = new VectorD3D(0, 1, 0);
		private static readonly VectorD3D _zVector = new VectorD3D(0, 0, 1);

		private ICrossSectionOfLine _crossSection;
		private IList<PointD3D> _linePoints;

		private VectorD3D _endWestVector;
		private VectorD3D _endNorthVector;
		private VectorD3D _endAdvanceVector;

		public VectorD3D EndWestVector { get { return _endWestVector; } }
		public VectorD3D EndNorthVector { get { return _endNorthVector; } }
		public VectorD3D EndAdvanceVector { get { return _endAdvanceVector; } }

		private SolidPolylineDashSegment _dashSegment;

		public void AddWithNormals(
		Action<PointD3D, VectorD3D> AddPositionAndNormal,
		Action<int, int, int, bool> AddIndices,
		ref int vertexIndexOffset,
		PenX3D pen,
		IList<PointD3D> polylinePoints
		)
		{
			if (pen.DashPattern == null)
			{
				// draw without a dash pattern - we consider the whole line as one dash segment, but instead of dash caps, with line caps
				_dashSegment.Initialize(pen.CrossSection, pen.Thickness1, pen.Thickness2, pen.LineJoin, pen.MiterLimit, pen.LineStartCap, pen.LineEndCap);
				var westNorth = PolylineMath3D.GetWestNorthVectorAtStart(polylinePoints);
				_dashSegment.AddGeometry(AddPositionAndNormal, AddIndices, ref vertexIndexOffset, polylinePoints, westNorth.WestVector, westNorth.NorthVector, westNorth.ForwardVector, null, null);
			}
			else
			{
				// draw with a dash pattern
				_dashSegment.Initialize(pen);

				double dashOffset = 0;

				bool startCapForwardAndPositionProvided = false;
				bool startCapNeedsJoinSegment = false;
				PolylinePointD3DAsClass startCapCOS = new PolylinePointD3DAsClass();
				bool endCapForwardAndPositionProvided = false;
				bool endCapNeedsJoinSegment = false;
				PolylinePointD3DAsClass endCapCOS = new PolylinePointD3DAsClass();

				double startIndex = 0;
				double endIndex = polylinePoints.Count - 1;

				// calculate the real start and end of the line, taking the line start and end cap length into account
				if (null != pen.LineStartCap)
				{
					var v = pen.LineStartCap.GetAbsoluteBaseInset(pen.Thickness1, pen.Thickness2);

					if (v < 0)
					{
						dashOffset = -v;

						startIndex = PolylineMath3D.GetFractionalStartIndexOfPolylineWithCapInsetAbsolute(
							polylinePoints,
							-v,
							out startCapForwardAndPositionProvided,
							out startCapNeedsJoinSegment,
							startCapCOS);
					}
				}

				if (null != pen.LineEndCap)
				{
					var v = pen.LineEndCap.GetAbsoluteBaseInset(pen.Thickness1, pen.Thickness2);
					if (v < 0)
					{
						endIndex = PolylineMath3D.GetFractionalEndIndexOfPolylineWithCapInsetAbsolute(
							polylinePoints,
							-v,
							out endCapForwardAndPositionProvided,
							out endCapNeedsJoinSegment,
							endCapCOS);
					}
				}

				// now draw the individual dash segments

				bool wasLineStartCapDrawn = false;
				bool wasLineEndCapDrawn = false;

				var en = PolylineMath3D.DissectPolylineWithDashPattern(
							polylinePoints,
							startIndex, endIndex,
							pen.DashPattern,
							pen.DashPattern.DashOffset,
							Math.Max(pen.Thickness1, pen.Thickness2),
							dashOffset,
							startCapForwardAndPositionProvided,
							startCapNeedsJoinSegment,
							startCapCOS,
							endCapForwardAndPositionProvided,
							endCapNeedsJoinSegment,
							endCapCOS
							).GetEnumerator();

				if (!en.MoveNext())
				{
					// there is no segment at all in the list, but maybe we can draw the start and end line caps
				}
				else
				{
					var previousPointList = en.Current;
					var currentPointList = en.Current;

					if (en.MoveNext())
						currentPointList = en.Current;
					else
						currentPointList = null;

					// if current point list is null, then there is only one segment, namely previousPointList, we have to draw it with start line cap and end line cap.
					if (currentPointList == null)
					{
						// note start line cap and end line cap will be overridden for this segment, but only then if the seamless merge with the dash segment
						bool overrideLineStartCap = startCapForwardAndPositionProvided && previousPointList[0].Position == startCapCOS.Position;
						bool overrideLineEndCap = endCapForwardAndPositionProvided && previousPointList[previousPointList.Count - 1].Position == endCapCOS.Position;
						_dashSegment.AddGeometry(AddPositionAndNormal, AddIndices, ref vertexIndexOffset, previousPointList, overrideLineStartCap ? pen.LineStartCap : null, overrideLineEndCap ? pen.LineEndCap : null);
						wasLineStartCapDrawn = overrideLineStartCap;
						wasLineEndCapDrawn = overrideLineEndCap;
					}
					else // there are at least two segments
					{
						// this is the start of the line, thus we must use the lineStartCap instead of the dashStartCap

						// note start line cap will be overridden for this first segment, but only then if it seamlessly merge with the start of the dash segment
						bool overrideLineStartCap = startCapForwardAndPositionProvided && previousPointList[0].Position == startCapCOS.Position;
						_dashSegment.AddGeometry(AddPositionAndNormal, AddIndices, ref vertexIndexOffset, previousPointList, overrideLineStartCap ? pen.LineStartCap : null, null);
						wasLineStartCapDrawn = overrideLineStartCap;

						previousPointList = currentPointList;
						while (en.MoveNext())
						{
							var currentList = en.Current;

							// draw the previous list as a normal dashSegment, thus we can use dashStartCap and dashEndCap
							_dashSegment.AddGeometry(AddPositionAndNormal, AddIndices, ref vertexIndexOffset, previousPointList, null, null);
							previousPointList = currentList;
						}

						// now currentList is the last list, we can draw an endcap to this
						bool overrideLineEndCap = endCapForwardAndPositionProvided && previousPointList[previousPointList.Count - 1].Position == endCapCOS.Position;
						_dashSegment.AddGeometry(AddPositionAndNormal, AddIndices, ref vertexIndexOffset, previousPointList, null, overrideLineEndCap ? pen.LineEndCap : null);
						wasLineEndCapDrawn = overrideLineEndCap;
					}

					object temporaryStorageSpace = null;

					// if the start cap was not drawn before, it must be drawn now
					if (!wasLineStartCapDrawn && null != pen.LineStartCap)
					{
						pen.LineStartCap.AddGeometry(
							AddPositionAndNormal,
							AddIndices,
							ref vertexIndexOffset,
							true,
							startCapCOS.Position,
							startCapCOS.WestVector,
							startCapCOS.NorthVector,
							startCapCOS.ForwardVector,
							pen.CrossSection,
							null,
							null,
							ref temporaryStorageSpace);
					}

					// if the end cap was not drawn before, it must be drawn now
					if (!wasLineEndCapDrawn && null != pen.LineEndCap)
					{
						pen.LineEndCap.AddGeometry(
							AddPositionAndNormal,
							AddIndices,
							ref vertexIndexOffset,
							false,
							endCapCOS.Position,
							endCapCOS.WestVector,
							endCapCOS.NorthVector,
							endCapCOS.ForwardVector,
							pen.CrossSection,
							null,
							null,
							ref temporaryStorageSpace);
					}
				}
			}
		}
	}
}