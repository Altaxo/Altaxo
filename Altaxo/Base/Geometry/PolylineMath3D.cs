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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Geometry
{
	public static class PolylineMath3D
	{
		private static readonly double Cos01Degree = Math.Cos(0.1 * Math.PI / 180);

		#region Get west and north vector for a line

		private static readonly double _northVectorMaxZComponent = Math.Cos(0.9 * Math.PI / 180);

		/// <summary>
		/// Gets a raw north vector for a straight line. Raw means that the returned vector is neither normalized, nor does it is perpendicular to the forward vector.
		/// It is only guaranteed that the returned vector is not colinear with the provided forward vector.
		/// </summary>
		/// <param name="forward">The line forward vector. Can be unnormalized.</param>
		/// <returns>The raw north vector.</returns>
		public static VectorD3D GetRawNorthVectorAtStart(VectorD3D forward)
		{
			double vLength = forward.Length;
			if (!(vLength > 0))
				throw new ArgumentException("Start vector of the line is invalid or empty", nameof(forward));
			forward = forward / vLength;

			if (Math.Abs(forward.Z) < _northVectorMaxZComponent)
				return new VectorD3D(0, 0, 1);
			else
				return new VectorD3D(0, -Math.Sign(forward.Z), 0);
		}

		/// <summary>
		/// Gets the west and north vector for a single straight line.
		/// </summary>
		/// <param name="forward">The line forward vector. Not required to be normalized.</param>
		/// <returns>The west and the north vector (Item1=west vector, Item2 = north vector).</returns>
		public static Tuple<VectorD3D, VectorD3D> GetWestNorthVectors(VectorD3D forward)
		{
			var n = GetRawNorthVectorAtStart(forward);
			var w = VectorD3D.CrossProduct(n, forward).Normalized;
			n = VectorD3D.CrossProduct(forward, w).Normalized;
			return new Tuple<VectorD3D, VectorD3D>(w, n);
		}

		/// <summary>
		/// Gets the west and north vector for a single straight line.
		/// </summary>
		/// <param name="line">The line.</param>
		/// <returns>The west and the north vector (Item1=west vector, Item2 = north vector).</returns>
		public static Tuple<VectorD3D, VectorD3D> GetWestNorthVectors(LineD3D line)
		{
			return GetWestNorthVectors(line.Vector);
		}

		public static PolylinePointD3D GetWestNorthVectorAtStart(IEnumerable<PointD3D> linePoints)
		{
			VectorD3D v = VectorD3D.Empty;
			bool isPrevPointValid = false;
			PointD3D prevPoint = PointD3D.Empty;
			foreach (var p in linePoints)
			{
				if (isPrevPointValid)
				{
					v = p - prevPoint;
					if (v.Length > 0)
						break;
				}
				prevPoint = p;
				isPrevPointValid = true;
			}

			if (!(v.Length > 0))
				throw new ArgumentException("Either too less points were given or the all the points fall together, thus no first valid vector could be determined", nameof(linePoints));

			var westNorth = GetWestNorthVectors(v);
			return new PolylinePointD3D(v.Normalized, westNorth.Item1, westNorth.Item2, prevPoint);
		}

		/// <summary>
		/// Amends a polyline, given by its polyline points, with an west and a north vector for each polyline point.
		/// </summary>
		/// <param name="linePoints">The line points.</param>
		/// <returns>The polyline points, amended with west and north vector (Item1: polyline point, Item2: west vector, Item3: north vector).
		/// The number of points may be smaller than the original number of points, because empty line segments are not returned.
		/// The west and north vectors are valid for the segment going from the previous point to the current point (thus for the first and the second point the returned west and north vectors are equal).</returns>
		public static IEnumerable<PolylinePointD3D> GetPolylinePointsWithWestAndNorth(IEnumerable<PointD3D> linePoints)
		{
			return GetPolylinePointsWithWestAndNorth(linePoints, false, VectorD3D.Empty, VectorD3D.Empty, VectorD3D.Empty);
		}

		/// <summary>
		/// Amends a polyline, given by its polyline points, with an west and a north vector for each polyline point.
		/// </summary>
		/// <param name="linePoints">The line points.</param>
		/// <param name="startWestVector">The start west vector. Has to be normalized. This is not checked!</param>
		/// <param name="startNorthVector">The start north vector. Has to be normalized. This is not checked!</param>
		/// <returns>The polyline points, amended with west and north vector (Item1: polyline point, Item2: west vector, Item3: north vector).
		/// The number of points may be smaller than the original number of points, because empty line segments are not returned.
		/// The west and north vectors are valid for the segment going from the previous point to the current point (thus for the first and the second point the returned west and north vectors are equal).</returns>
		public static IEnumerable<PolylinePointD3D> GetPolylinePointsWithWestAndNorth(IEnumerable<PointD3D> linePoints, VectorD3D startWestVector, VectorD3D startNorthVector, VectorD3D startForwardVector)
		{
			return GetPolylinePointsWithWestAndNorth(linePoints, true, startWestVector, startNorthVector, startForwardVector);
		}

		/// <summary>
		/// Amends a polyline, given by its polyline points, with an west and a north vector for each polyline point.
		/// </summary>
		/// <param name="linePoints">The line points.</param>
		/// <param name="startVectorsProvided">If true, the start west vector and start north vectors are provided in the following arguments.</param>
		/// <param name="startWestVector">The start west vector if provided (otherwise it may be VectorD3D.Empty).</param>
		/// <param name="startNorthVector">The start north vector if provided (otherwise it may be VectorD3D.Empty).</param>
		/// <returns>The polyline points, amended with west and north vector (Item1: polyline point, Item2: west vector, Item3: north vector).
		/// The number of points may be smaller than the original number of points, because empty line segments are not returned.
		/// The west and north vectors are valid for the segment going from the previous point to the current point (thus for the first and the second point the returned west and north vectors are equal).</returns>
		private static IEnumerable<PolylinePointD3D> GetPolylinePointsWithWestAndNorth(IEnumerable<PointD3D> linePoints, bool startVectorsProvided, VectorD3D startWestVector, VectorD3D startNorthVector, VectorD3D startForwardVector)
		{
			VectorD3D n = VectorD3D.Empty;
			VectorD3D w = VectorD3D.Empty;
			VectorD3D f = VectorD3D.Empty;

			var en = linePoints.GetEnumerator();
			if (!en.MoveNext())
				yield break; // no points in the enumeration

			PointD3D previousPoint = en.Current;
			VectorD3D previousSegment = VectorD3D.Empty;

			while (en.MoveNext())
			{
				var currentPoint = en.Current;
				var currentSegment = currentPoint - previousPoint;

				var len = currentSegment.Length;
				if (!(len > 0))
					continue; // leave prevPoint unchanged here, because the segment was empty
				currentSegment /= len; // current segment normalized

				if (previousSegment.IsEmpty)
				{
					if (startVectorsProvided)
					{
						w = startWestVector;
						n = startNorthVector;
						f = startForwardVector;
					}
					else
					{
						var entry = GetWestNorthVectors(currentSegment);
						w = entry.Item1;
						n = entry.Item2;
						f = currentSegment;
					}
				}

				yield return new PolylinePointD3D(f, w, n, previousPoint); // output the previous segment

				if (!previousSegment.IsEmpty)
				{
					// if there was a previous segment, then calculate the new west and north vectors
					GetWestAndNorthVectorsForNextSegment(previousSegment, currentSegment, ref w, ref n);
					f = currentSegment;
				}

				previousSegment = currentSegment;
				previousPoint = currentPoint;
			}

			if (!previousSegment.IsEmpty)
				yield return new PolylinePointD3D(f, w, n, previousPoint);
		}

		/// <summary>
		/// Calculates the west and north vectors for the next segment.
		/// </summary>
		/// <param name="previousSegment">The previous segment. Required to be normalized!</param>
		/// <param name="nextSegment">The next segment. Required to be normalized!</param>
		/// <param name="westVector">The west vector of the previous segment. At return, this will be the west vector of the next segment.</param>
		/// <param name="northVector">The north vector of the previous segment. At return, this will be the west vector of the next segment.</param>
		public static void GetWestAndNorthVectorsForNextSegment(VectorD3D previousSegment, VectorD3D nextSegment, ref VectorD3D westVector, ref VectorD3D northVector)
		{
			VectorD3D symmetryPlaneNormal = (nextSegment + previousSegment); // no need to normalize it
			double symmetryPlaneNormalLengthSqr = symmetryPlaneNormal.SquareOfLength;

			if (symmetryPlaneNormal.SquareOfLength < 1E-16) // current vector almost exactly in the opposite direction than previous vector
			{
				// in this case it is better to use the reflection plane
				VectorD3D reflectionPlaneNormal = (nextSegment - previousSegment);
				westVector = Math3D.GetVectorSymmetricalToPlane(-westVector, reflectionPlaneNormal); // we use -west as argument by convention, since reflection will reverse it
				westVector = Math3D.GetNormalizedVectorOrthogonalToVector(westVector, nextSegment); // make the north vector orthogonal (it should be already, but this corrects small deviations)
				northVector = VectorD3D.CrossProduct(nextSegment, westVector);
			}
			else
			{
				// normal case, use the symmetry plane
				westVector = Math3D.GetVectorSymmetricalToPlane(westVector, symmetryPlaneNormal);
				westVector = Math3D.GetNormalizedVectorOrthogonalToVector(westVector, nextSegment); // make the north vector orthogonal (it should be already, but this corrects small deviations)
				northVector = VectorD3D.CrossProduct(nextSegment, westVector);
			}
		}

		/// <summary>
		/// Calculates the west and north vectors for the previous segment.
		/// </summary>
		/// <param name="previousSegment">The previous segment. Required to be normalized!</param>
		/// <param name="nextSegment">The next segment. Required to be normalized!</param>
		/// <param name="westVector">The west vector of the next (!) segment. At return, this will be the west vector of the previous segment.</param>
		/// <param name="northVector">The north vector of the next (!) segment. At return, this will be the west vector of the previous segment.</param>
		public static void GetWestAndNorthVectorsForPreviousSegment(VectorD3D previousSegment, VectorD3D nextSegment, ref VectorD3D westVector, ref VectorD3D northVector)
		{
			GetWestAndNorthVectorsForNextSegment(nextSegment, previousSegment, ref westVector, ref northVector);
		}

		#endregion Get west and north vector for a line

		public static double GetFractionalStartIndexOfPolylineWithCapInsetAbsolute(
			IList<PolylinePointD3D> polylinePoints,
			double capInsetAbsolute,
			out bool startCapForwardAndPositionProvided,
			out bool startCapNeedsJoiningSegment,
			PolylinePointD3DAsClass startCapCOS)
		{
			var lineStart = polylinePoints[0];
			int polylinePointsCount = polylinePoints.Count;

			for (int i = 1; i < polylinePointsCount; ++i)
			{
				var curr = polylinePoints[i];
				double diff = (curr.Position - lineStart.Position).Length - capInsetAbsolute;

				if (diff == 0 && (i + 1) < polylinePointsCount) // OK, exactly here
				{
					startCapCOS.Position = curr.Position;
					startCapCOS.ForwardVector = (curr.Position - lineStart.Position).Normalized;
					startCapCOS.WestVector = curr.WestVector;
					startCapCOS.NorthVector = curr.NorthVector;
					PolylineMath3D.GetWestAndNorthVectorsForPreviousSegment(startCapCOS.ForwardVector, curr.ForwardVector, ref startCapCOS.WestVector, ref startCapCOS.NorthVector);
					startCapNeedsJoiningSegment = VectorD3D.DotProduct(startCapCOS.ForwardVector, (polylinePoints[i + 1].Position - curr.Position).Normalized) < Cos01Degree;
					startCapForwardAndPositionProvided = true;
					return i;
				}
				else if (diff > 0) // OK, sowewhere between previous point and here.
				{
					int baseIndex = i - 1;
					var prev = polylinePoints[baseIndex];
					var relIndex = Calc.RootFinding.QuickRootFinding.ByBrentsAlgorithm(
						(r) => (PointD3D.Interpolate(prev.Position, curr.Position, r) - lineStart.Position).Length - capInsetAbsolute,
						0, 1,
						1e-3, 0);

					startCapCOS.Position = PointD3D.Interpolate(prev.Position, curr.Position, relIndex);
					startCapCOS.ForwardVector = (startCapCOS.Position - lineStart.Position).Normalized;
					startCapCOS.WestVector = curr.WestVector;
					startCapCOS.NorthVector = curr.NorthVector;
					PolylineMath3D.GetWestAndNorthVectorsForPreviousSegment(startCapCOS.ForwardVector, curr.ForwardVector, ref startCapCOS.WestVector, ref startCapCOS.NorthVector);
					startCapNeedsJoiningSegment = VectorD3D.DotProduct(startCapCOS.ForwardVector, (curr.Position - prev.Position).Normalized) < Cos01Degree;
					startCapForwardAndPositionProvided = true;
					return baseIndex + relIndex;
				}
			}

			// the cap is too big, all the line is inside the cap
			// now search at least for a point which can serve as cap direction
			for (int i = polylinePoints.Count - 1; i > 0; --i)
			{
				if (polylinePoints[i].Position != lineStart.Position)
				{
					startCapCOS.ForwardVector = (polylinePoints[i].Position - lineStart.Position).Normalized;
					startCapCOS.Position = lineStart.Position + startCapCOS.ForwardVector * capInsetAbsolute;
					startCapCOS.WestVector = lineStart.WestVector;
					startCapCOS.NorthVector = lineStart.NorthVector;
					PolylineMath3D.GetWestAndNorthVectorsForNextSegment(lineStart.ForwardVector, startCapCOS.ForwardVector, ref startCapCOS.WestVector, ref startCapCOS.NorthVector);
					startCapNeedsJoiningSegment = false;
					startCapForwardAndPositionProvided = true;
					return double.NaN; ;
				}
			}

			startCapCOS.ForwardVector = VectorD3D.Empty;
			startCapCOS.WestVector = VectorD3D.Empty;
			startCapCOS.NorthVector = VectorD3D.Empty;
			startCapCOS.Position = lineStart.Position;
			startCapNeedsJoiningSegment = false;
			startCapForwardAndPositionProvided = false;
			return 0;
		}

		public static double GetFractionalStartIndexOfPolylineWithCapInsetAbsolute(
		IList<PointD3D> polylinePoints,
		double capInsetAbsolute,
		out bool startCapForwardAndPositionProvided,
		out bool startCapNeedsJoiningSegment,
		PolylinePointD3DAsClass startCapCOS)
		{
			var lineStart = polylinePoints[0];
			int polylinePointsCount = polylinePoints.Count;

			for (int i = 1; i < polylinePointsCount; ++i)
			{
				var curr = polylinePoints[i];
				double diff = (curr - lineStart).Length - capInsetAbsolute;

				if (diff == 0 && (i + 1) < polylinePointsCount) // OK, exactly here
				{
					startCapCOS.Position = curr;
					startCapCOS.ForwardVector = (curr - lineStart).Normalized;
					startCapNeedsJoiningSegment = VectorD3D.DotProduct(startCapCOS.ForwardVector, (polylinePoints[i + 1] - curr).Normalized) < Cos01Degree;
					startCapForwardAndPositionProvided = true;
					return i;
				}
				else if (diff > 0) // OK, sowewhere between previous point and here.
				{
					int baseIndex = i - 1;
					var prev = polylinePoints[baseIndex];
					var relIndex = Calc.RootFinding.QuickRootFinding.ByBrentsAlgorithm(
						(r) => (PointD3D.Interpolate(prev, curr, r) - lineStart).Length - capInsetAbsolute,
						0, 1,
						1e-3, 0);

					startCapCOS.Position = PointD3D.Interpolate(prev, curr, relIndex);
					startCapCOS.ForwardVector = (startCapCOS.Position - lineStart).Normalized;
					startCapNeedsJoiningSegment = VectorD3D.DotProduct(startCapCOS.ForwardVector, (curr - prev).Normalized) < Cos01Degree;
					startCapForwardAndPositionProvided = true;
					return baseIndex + relIndex;
				}
			}

			// the cap is too big, all the line is inside the cap
			// now search at least for a point which can serve as cap direction
			for (int i = polylinePoints.Count - 1; i > 0; --i)
			{
				if (polylinePoints[i] != lineStart)
				{
					startCapCOS.ForwardVector = (polylinePoints[i] - lineStart).Normalized;
					startCapCOS.Position = lineStart + startCapCOS.ForwardVector * capInsetAbsolute;
					startCapNeedsJoiningSegment = false;
					startCapForwardAndPositionProvided = true;
					return double.NaN; ;
				}
			}

			startCapCOS.ForwardVector = VectorD3D.Empty;
			startCapCOS.Position = lineStart;
			startCapNeedsJoiningSegment = false;
			startCapForwardAndPositionProvided = false;
			return 0;
		}

		public static double GetFractionalEndIndexOfPolylineWithCapInsetAbsolute(
		IList<PolylinePointD3D> polylinePoints,
		double capInsetAbsolute,
		out bool endCapForwardAndPositionProvided,
		out bool endCapNeedsJoiningSegment,
		PolylinePointD3DAsClass endCapCOS)
		{
			var lineEnd = polylinePoints[polylinePoints.Count - 1];
			for (int i = polylinePoints.Count - 2; i >= 0; --i)
			{
				var curr = polylinePoints[i];

				double diff = (curr.Position - lineEnd.Position).Length - capInsetAbsolute;

				if (diff == 0 && i > 0) // OK, exactely here
				{
					endCapCOS.Position = curr.Position;
					endCapCOS.ForwardVector = (lineEnd.Position - endCapCOS.Position).Normalized;
					endCapCOS.WestVector = curr.WestVector;
					endCapCOS.NorthVector = curr.NorthVector;
					PolylineMath3D.GetWestAndNorthVectorsForNextSegment(curr.ForwardVector, endCapCOS.ForwardVector, ref endCapCOS.WestVector, ref endCapCOS.NorthVector);
					endCapNeedsJoiningSegment = VectorD3D.DotProduct(endCapCOS.ForwardVector, (polylinePoints[i - 1].Position - curr.Position).Normalized) < Cos01Degree;
					endCapForwardAndPositionProvided = true;
					return i;
				}
				else if (diff > 0) // OK, sowewhere between previous point and here.
				{
					int baseIndex = i + 1;
					var prev = polylinePoints[baseIndex];
					var relIndex = Calc.RootFinding.QuickRootFinding.ByBrentsAlgorithm(
						(r) => (PointD3D.Interpolate(prev.Position, curr.Position, r) - lineEnd.Position).Length - capInsetAbsolute,
						0, 1,
						1e-3, 0);

					endCapCOS.Position = PointD3D.Interpolate(prev.Position, curr.Position, relIndex);
					endCapCOS.ForwardVector = (lineEnd.Position - endCapCOS.Position).Normalized;
					endCapCOS.WestVector = curr.WestVector;
					endCapCOS.NorthVector = curr.NorthVector;
					PolylineMath3D.GetWestAndNorthVectorsForNextSegment(curr.ForwardVector, endCapCOS.ForwardVector, ref endCapCOS.WestVector, ref endCapCOS.NorthVector);
					endCapNeedsJoiningSegment = VectorD3D.DotProduct(endCapCOS.ForwardVector, (prev.Position - curr.Position).Normalized) < Cos01Degree;
					endCapForwardAndPositionProvided = true;
					return baseIndex - relIndex;
				}
			}

			// the cap is too big, all the line is inside the cap
			// now search at least for a point which can serve as cap direction
			for (int i = 0; i < polylinePoints.Count; ++i)
			{
				if (polylinePoints[i].Position != lineEnd.Position)
				{
					endCapCOS.ForwardVector = (lineEnd.Position - polylinePoints[i].Position).Normalized;
					endCapCOS.Position = lineEnd.Position + endCapCOS.ForwardVector * capInsetAbsolute;
					endCapCOS.WestVector = polylinePoints[i].WestVector;
					endCapCOS.NorthVector = polylinePoints[i].NorthVector;
					PolylineMath3D.GetWestAndNorthVectorsForNextSegment(polylinePoints[i].ForwardVector, endCapCOS.ForwardVector, ref endCapCOS.WestVector, ref endCapCOS.NorthVector);
					endCapNeedsJoiningSegment = false;
					endCapForwardAndPositionProvided = true;
					return double.NaN;
				}
			}

			endCapCOS.ForwardVector = VectorD3D.Empty;
			endCapCOS.Position = lineEnd.Position;
			endCapNeedsJoiningSegment = false;
			endCapForwardAndPositionProvided = false;
			return polylinePoints.Count - 1;
		}

		public static double GetFractionalEndIndexOfPolylineWithCapInsetAbsolute(
			IList<PointD3D> polylinePoints,
			double capInsetAbsolute,
			out bool endCapForwardAndPositionProvided,
			out bool endCapNeedsJoiningSegment,
			PolylinePointD3DAsClass endCapCOS)
		{
			var lineEnd = polylinePoints[polylinePoints.Count - 1];
			for (int i = polylinePoints.Count - 2; i >= 0; --i)
			{
				var curr = polylinePoints[i];

				double diff = (curr - lineEnd).Length - capInsetAbsolute;

				if (diff == 0 && i > 0) // OK, exactely here
				{
					endCapCOS.Position = curr;
					endCapCOS.ForwardVector = (lineEnd - endCapCOS.Position).Normalized;
					endCapNeedsJoiningSegment = VectorD3D.DotProduct(endCapCOS.ForwardVector, (polylinePoints[i - 1] - curr).Normalized) < Cos01Degree;
					endCapForwardAndPositionProvided = true;
					return i;
				}
				else if (diff > 0) // OK, sowewhere between previous point and here.
				{
					int baseIndex = i + 1;
					var prev = polylinePoints[baseIndex];
					var relIndex = Calc.RootFinding.QuickRootFinding.ByBrentsAlgorithm(
						(r) => (PointD3D.Interpolate(prev, curr, r) - lineEnd).Length - capInsetAbsolute,
						0, 1,
						1e-3, 0);

					endCapCOS.Position = PointD3D.Interpolate(prev, curr, relIndex);
					endCapCOS.ForwardVector = (lineEnd - endCapCOS.Position).Normalized;
					endCapNeedsJoiningSegment = VectorD3D.DotProduct(endCapCOS.ForwardVector, (prev - curr).Normalized) < Cos01Degree;
					endCapForwardAndPositionProvided = true;
					return baseIndex - relIndex;
				}
			}

			// the cap is too big, all the line is inside the cap
			// now search at least for a point which can serve as cap direction
			for (int i = 0; i < polylinePoints.Count; ++i)
			{
				if (polylinePoints[i] != lineEnd)
				{
					endCapCOS.ForwardVector = (lineEnd - polylinePoints[i]).Normalized;
					endCapCOS.Position = lineEnd + endCapCOS.ForwardVector * capInsetAbsolute;
					endCapNeedsJoiningSegment = false;
					endCapForwardAndPositionProvided = true;
					return double.NaN;
				}
			}

			endCapCOS.ForwardVector = VectorD3D.Empty;
			endCapCOS.Position = lineEnd;
			endCapNeedsJoiningSegment = false;
			endCapForwardAndPositionProvided = false;
			return polylinePoints.Count - 1;
		}

		/// <summary>
		/// Returns an empty polyline (no points at all).
		/// </summary>
		/// <returns>Empty polyline.</returns>
		public static IEnumerable<PolylinePointD3D> GetEmptyPolyline()
		{
			yield break;
		}

		/// <summary>
		/// Gets a part of an polyline by providing start end end indices. The indices are allowed to be real values (for instance the start index 0.5 means that the
		/// start point of the returned polyline is excactly in the middle between the first point (index 0) and the second point (index 1) of the original polyline.
		/// </summary>
		/// <param name="originalPolyline">The original polyline.</param>
		/// <param name="westVector">The west vector at start of the original polyline.</param>
		/// <param name="northVector">The north vector at the start of the original polyline.</param>
		/// <param name="forwardVector">The forward vector at the start of the original polyline.</param>
		/// <param name="startIndex">The start index. Must be greater than or equal to zero.</param>
		/// <param name="endIndex">The end index Must be greater then the start index and smaller than or equal to originalPolyline.Count-1.</param>
		/// <param name="startCapForwardAndPositionProvided">If <c>true</c>, position and forward vector of the start cap were already calculated (but not west and north vector).</param>
		/// <param name="startCapNeedsJoiningSegment">If set to <c>true</c>, there is need for a joining segment between start cap and the rest of the polyline. This joining segment is returned in the enumeration. It has the same position as the start of the returned polyline, but the forward, west and north vectors of the start cap.</param>
		/// <param name="startCapCOS">Position, forward, west and north vectors of the start cap. This information will be filled in during the enumeration.</param>
		/// <param name="endCapForwardAndPositionProvided">If <c>true</c>, position and forward vector of the start cap were already calculated (but not west and north vector).</param>
		/// <param name="endCapNeedsJoiningSegment">If set to <c>true</c>, there is need for a joining segment between the end of the polyline and the end cap. This joining segment is returned in the enumeration. It has the same position as the end of the polyline, but the forward, west and north vectors of the end cap.</param>
		/// <param name="endCapCOS">Position, forward, west and north vectors of the end cap. This information will be filled in during the enumeration, and will be valid only after the enumeration has finished.</param>
		/// <returns>Enumeration of that part of the original polyline, that is described by the start and end index.</returns>
		public static IEnumerable<PolylinePointD3D> GetPolylineWithFractionalStartAndEndIndex(
			IEnumerable<PointD3D> originalPolyline,
			VectorD3D westVector,
			VectorD3D northVector,
			VectorD3D forwardVector,
			double startIndex, double endIndex,
			bool startCapForwardAndPositionProvided,
			bool startCapNeedsJoiningSegment,
			PolylinePointD3DAsClass startCapCOS,
			bool endCapForwardAndPositionProvided,
			bool endCapNeedsJoiningSegment,
			PolylinePointD3DAsClass endCapCOS
			)
		{
			var originalPolylineEnumerator = GetPolylinePointsWithWestAndNorth(originalPolyline, westVector, northVector, forwardVector).GetEnumerator();

			return GetPolylineWithFractionalStartAndEndIndex(
			originalPolylineEnumerator,
			startIndex, endIndex,
			startCapForwardAndPositionProvided,
			startCapNeedsJoiningSegment,
			startCapCOS,
			endCapForwardAndPositionProvided,
			endCapNeedsJoiningSegment,
			endCapCOS);
		}

		/// <summary>
		/// Gets a part of an polyline by providing start end end indices. The indices are allowed to be real values (for instance the start index 0.5 means that the
		/// start point of the returned polyline is excactly in the middle between the first point (index 0) and the second point (index 1) of the original polyline.
		/// </summary>
		/// <param name="originalPolyline">The original polyline.</param>
		/// <param name="startIndex">The start index. Must be greater than or equal to zero.</param>
		/// <param name="endIndex">The end index Must be greater then the start index and smaller than or equal to originalPolyline.Count-1.</param>
		/// <param name="startCapForwardAndPositionProvided">If <c>true</c>, position and forward vector of the start cap were already calculated (but not west and north vector).</param>
		/// <param name="startCapNeedsJoiningSegment">If set to <c>true</c>, there is need for a joining segment between start cap and the rest of the polyline. This joining segment is returned in the enumeration. It has the same position as the start of the returned polyline, but the forward, west and north vectors of the start cap.</param>
		/// <param name="startCapCOS">Position, forward, west and north vectors of the start cap. This information will be filled in during the enumeration.</param>
		/// <param name="endCapForwardAndPositionProvided">If <c>true</c>, position and forward vector of the start cap were already calculated (but not west and north vector).</param>
		/// <param name="endCapNeedsJoiningSegment">If set to <c>true</c>, there is need for a joining segment between the end of the polyline and the end cap. This joining segment is returned in the enumeration. It has the same position as the end of the polyline, but the forward, west and north vectors of the end cap.</param>
		/// <param name="endCapCOS">Position, forward, west and north vectors of the end cap. This information will be filled in during the enumeration, and will be valid only after the enumeration has finished.</param>
		/// <returns>Enumeration of that part of the original polyline, that is described by the start and end index.</returns>
		public static IEnumerable<PolylinePointD3D> GetPolylineWithFractionalStartAndEndIndex(
			IEnumerable<PolylinePointD3D> originalPolyline,
			double startIndex, double endIndex,
			bool startCapForwardAndPositionProvided,
			bool startCapNeedsJoiningSegment,
			PolylinePointD3DAsClass startCapCOS,
			bool endCapForwardAndPositionProvided,
			bool endCapNeedsJoiningSegment,
			PolylinePointD3DAsClass endCapCOS
			)
		{
			return GetPolylineWithFractionalStartAndEndIndex(
			originalPolyline.GetEnumerator(),
			startIndex, endIndex,
			startCapForwardAndPositionProvided,
			startCapNeedsJoiningSegment,
			startCapCOS,
			endCapForwardAndPositionProvided,
			endCapNeedsJoiningSegment,
			endCapCOS);
		}

		/// <summary>
		/// Gets a part of an polyline by providing start end end indices. The indices are allowed to be real values (for instance the start index 0.5 means that the
		/// start point of the returned polyline is excactly in the middle between the first point (index 0) and the second point (index 1) of the original polyline.
		/// </summary>
		/// <param name="originalPolyline">The original polyline.</param>
		/// <param name="westVector">The west vector at start of the original polyline.</param>
		/// <param name="northVector">The north vector at the start of the original polyline.</param>
		/// <param name="forwardVector">The forward vector at the start of the original polyline.</param>
		/// <param name="startIndex">The start index. Must be greater than or equal to zero.</param>
		/// <param name="endIndex">The end index Must be greater then the start index and smaller than or equal to originalPolyline.Count-1.</param>
		/// <param name="startCapForwardAndPositionProvided">If <c>true</c>, position and forward vector of the start cap were already calculated (but not west and north vector).</param>
		/// <param name="startCapNeedsJoiningSegment">If set to <c>true</c>, there is need for a joining segment between start cap and the rest of the polyline. This joining segment is returned in the enumeration. It has the same position as the start of the returned polyline, but the forward, west and north vectors of the start cap.</param>
		/// <param name="startCapCOS">Position, forward, west and north vectors of the start cap. This information will be filled in during the enumeration.</param>
		/// <param name="endCapForwardAndPositionProvided">If <c>true</c>, position and forward vector of the start cap were already calculated (but not west and north vector).</param>
		/// <param name="endCapNeedsJoiningSegment">If set to <c>true</c>, there is need for a joining segment between the end of the polyline and the end cap. This joining segment is returned in the enumeration. It has the same position as the end of the polyline, but the forward, west and north vectors of the end cap.</param>
		/// <param name="endCapCOS">Position, forward, west and north vectors of the end cap. This information will be filled in during the enumeration, and will be valid only after the enumeration has finished.</param>
		/// <returns>Enumeration of that part of the original polyline, that is described by the start and end index.</returns>
		public static IEnumerable<PolylinePointD3D> GetPolylineWithFractionalStartAndEndIndex(
			IEnumerator<PolylinePointD3D> originalPolylineEnumerator,
			double startIndex, double endIndex,
			bool startCapForwardAndPositionProvided,
			bool startCapNeedsJoiningSegment,
			PolylinePointD3DAsClass startCapCOS,
			bool endCapForwardAndPositionProvided,
			bool endCapNeedsJoiningSegment,
			PolylinePointD3DAsClass endCapCOS
			)
		{
			int startIndexInt = (int)Math.Floor(startIndex);
			double startIndexFrac = startIndex - startIndexInt;
			int endIndexInt = (int)Math.Floor(endIndex);
			double endIndexFrac = endIndex - endIndexInt;

			int i;
			for (i = -1; i < startIndexInt; ++i)
			{
				if (!originalPolylineEnumerator.MoveNext()) // Fast forward to the first item
					throw new ArgumentOutOfRangeException(nameof(startIndex) + " seems to be too high (not enough points in " + nameof(originalPolylineEnumerator) + ")");
			}
			var previousItem = originalPolylineEnumerator.Current;
			if (!originalPolylineEnumerator.MoveNext())
				throw new ArgumentOutOfRangeException(nameof(startIndex) + " seems to be too high (not enough points in " + nameof(originalPolylineEnumerator) + ")");
			++i;
			var currentItem = originalPolylineEnumerator.Current;

			var firstItem = previousItem;

			if (startIndexFrac != 0)
			{
				var newPoint = PointD3D.Interpolate(previousItem.Position, currentItem.Position, startIndexFrac);
				firstItem = new PolylinePointD3D(currentItem.ForwardVector, currentItem.WestVector, currentItem.NorthVector, newPoint); // return interpolated start point
			}

			if (startCapForwardAndPositionProvided)
			{
				// per convention the very first and the next returned item should have the same forward, west and north vectors
				// thus we change the first item now and also take it for the start cap
				GetWestAndNorthVectorsForPreviousSegment(startCapCOS.ForwardVector, firstItem.ForwardVector, ref firstItem.WestVector, ref firstItem.NorthVector);
				firstItem.ForwardVector = startCapCOS.ForwardVector;
				startCapCOS.WestVector = firstItem.WestVector;
				startCapCOS.NorthVector = firstItem.NorthVector;
				if (startCapNeedsJoiningSegment)
				{
					yield return firstItem; // yield return exactly the same as the first item -> but this will lead to the drawing of a joining segment between start cap and the rest of the polyline
				}
			}
			else
			{
				startCapCOS.Position = firstItem.Position;
				startCapCOS.WestVector = firstItem.WestVector;
				startCapCOS.NorthVector = firstItem.NorthVector;
				startCapCOS.ForwardVector = firstItem.ForwardVector;
			}

			yield return firstItem; // now yield return the first item

			if (endIndexInt > startIndexInt)
				yield return currentItem; // if start and end do not lie in the same segment, we can now yield return the second point

			// now for all other points before the end point
			for (; i < endIndexInt; ++i)
			{
				if (!originalPolylineEnumerator.MoveNext())
					throw new ArgumentOutOfRangeException(nameof(endIndex) + " seems to be too high (not enough points in " + nameof(originalPolylineEnumerator) + ")");

				yield return originalPolylineEnumerator.Current; // return intermediate points, maybe here the end point too (if endIndexFrac was zero)
			}

			// now the last item

			var lastItem = originalPolylineEnumerator.Current; // note: if endIndexFrac is 0, the last item was yield returned in the loop above

			if (endIndexFrac != 0)
			{
				if (endIndexInt != startIndexInt)
				{
					previousItem = originalPolylineEnumerator.Current;
					if (!originalPolylineEnumerator.MoveNext())
						throw new ArgumentOutOfRangeException(nameof(endIndex) + " seems to be too high (not enough points in " + nameof(originalPolylineEnumerator) + ")");
					currentItem = originalPolylineEnumerator.Current;
				}

				var newPoint = PointD3D.Interpolate(previousItem.Position, currentItem.Position, endIndexFrac);
				lastItem = new PolylinePointD3D(currentItem.ForwardVector, currentItem.WestVector, currentItem.NorthVector, newPoint);
				yield return lastItem; // here the last item was not yield returned, thus we do it now
			}

			if (endCapForwardAndPositionProvided)
			{
				endCapCOS.WestVector = lastItem.WestVector;
				endCapCOS.NorthVector = lastItem.NorthVector;
				GetWestAndNorthVectorsForNextSegment(lastItem.ForwardVector, endCapCOS.ForwardVector, ref endCapCOS.WestVector, ref endCapCOS.NorthVector);
				if (endCapNeedsJoiningSegment)
				{
					yield return new PolylinePointD3D(endCapCOS.ForwardVector, endCapCOS.WestVector, endCapCOS.NorthVector, lastItem.Position); // and maybe return a very last point which is the joining segment. Is has the same location as the previous point, and thus the forward vector must be used here!
				}
			}
			else
			{
				endCapCOS.Position = lastItem.Position;
				endCapCOS.WestVector = lastItem.WestVector;
				endCapCOS.NorthVector = lastItem.NorthVector;
				endCapCOS.ForwardVector = lastItem.ForwardVector;
			}
		}

		/// <summary>
		/// Dissects a polyline into multiple polylines using a dash pattern.
		/// </summary>
		/// <param name="linePoints">The line points of the polyline that is dissected.</param>
		/// <param name="dashPattern">The dash pattern used to dissect the polyline.</param>
		/// <param name="dashPatternScale">Length of one unit of the dash pattern..</param>
		/// <returns>Enumeration of polylines. The first item of the returned tuples is the list with the polyline points, the second item is the west vector for the first polyline point, and the third item is the north vector for the first polyline point.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// </exception>
		/// <exception cref="System.ArgumentException"></exception>
		public static IEnumerable<List<PolylinePointD3D>> DissectPolylineWithDashPattern(
			IEnumerable<PointD3D> linePoints,
			double startIndex, double endIndex,
			IList<double> dashPattern,
			double dashPatternOffset,
			double dashPatternScale,
			double dashPatternStartAbsolute,
			bool startCapForwardAndPositionProvided,
			bool startCapNeedsJoiningSegment,
			PolylinePointD3DAsClass startCapCOS,
			bool endCapForwardAndPositionProvided,
			bool endCapNeedsJoiningSegment,
			PolylinePointD3DAsClass endCapCOS)
		{
			if (null == dashPattern || dashPattern.Count == 0)
				throw new ArgumentOutOfRangeException(nameof(dashPattern) + " is null or empty");
			if (!(dashPatternScale > 0))
				throw new ArgumentOutOfRangeException(nameof(dashPatternScale) + " should be > 0");

			int dashIndex = 0;
			int dashCount = dashPattern.Count;

			// Fast forward in dash
			double remainingOffset = dashPatternOffset;
			double currDash = dashPattern[dashIndex];

			while (remainingOffset > 0)
			{
				if ((remainingOffset - currDash) >= 0)
				{
					dashIndex = (dashIndex + 1) % dashCount;
					remainingOffset = remainingOffset - currDash;
					currDash = dashPattern[dashIndex];
				}
				else
				{
					currDash -= remainingOffset;
					remainingOffset = 0;
				}
			}

			// now move forward to dashPatternStartAbsolute
			double remainingOffsetAbsolute = dashPatternStartAbsolute;
			while (remainingOffsetAbsolute > 0)
			{
				var diff = remainingOffsetAbsolute - currDash * dashPatternScale;
				if (diff >= 0)
				{
					dashIndex = (dashIndex + 1) % dashCount;
					remainingOffsetAbsolute = diff;
					currDash = dashPattern[dashIndex];
				}
				else
				{
					currDash -= remainingOffsetAbsolute / dashPatternScale;
					remainingOffsetAbsolute = 0;
				}
			}

			var westNorth = GetWestNorthVectorAtStart(linePoints);

			var en = GetPolylineWithFractionalStartAndEndIndex(
				linePoints,
				westNorth.WestVector, westNorth.NorthVector, westNorth.ForwardVector,
				startIndex, endIndex,
				startCapForwardAndPositionProvided, startCapNeedsJoiningSegment, startCapCOS,
				endCapForwardAndPositionProvided, endCapNeedsJoiningSegment, endCapCOS).GetEnumerator();

			if (false == en.MoveNext())
				throw new ArgumentException(nameof(linePoints) + " seems not to contain line points");

			var previousPoint = en.Current;

			double patternRemainingDistance = currDash * dashPatternScale; // remaining distance for the current pattern feature

			var outputPoints = new List<PolylinePointD3D>();

			if (0 == dashIndex % 2)
				outputPoints.Add(previousPoint);

			while (true == en.MoveNext())
			{
				bool isLineDissected = false; // false as long as the current line (from prev.Item1 to curr.Item1) is not entirely dissected
				while (!isLineDissected)
				{
					var currentPoint = en.Current;
					var vec = currentPoint.Position - previousPoint.Position;
					var currentDistance = vec.Length;

					if (patternRemainingDistance >= currentDistance) // if the remaining distance of this dash is greater than the current distance, we take this point completely.
					{
						if (0 == dashIndex % 2)
							outputPoints.Add(currentPoint);
						patternRemainingDistance -= currentDistance;
						isLineDissected = true;
					}
					else // if (patternRemainingDistance < currentDistance) // if the remaining distance in this dash is smaller than the current distance, we calculate the intermediate point on this line
					{
						var rel = patternRemainingDistance / currentDistance;
						var p = PointD3D.Interpolate(previousPoint.Position, currentPoint.Position, rel);
						if (0 == dashIndex % 2)
							outputPoints.Add(new PolylinePointD3D(currentPoint.ForwardVector, currentPoint.WestVector, currentPoint.NorthVector, p));

						// now output the list
						if (outputPoints.Count >= 2)
						{
							yield return outputPoints;
							outputPoints = new List<PolylinePointD3D>(); // don't recycle the list
						}

						currentPoint = new PolylinePointD3D(currentPoint.ForwardVector, currentPoint.WestVector, currentPoint.NorthVector, p);
						patternRemainingDistance = 0;
					}

					// now increment pattern pointer if the remaining pattern distance is zero
					if (patternRemainingDistance <= 0)
					{
						// increment pattern pointer
						++dashIndex;
						if (dashIndex >= dashCount)
						{
							dashIndex = 0;
						}
						patternRemainingDistance = dashPattern[dashIndex] * dashPatternScale;

						// if now the pattern is the start of a dash, store the starting west and north vector
						if (0 == dashIndex % 2)
						{
							outputPoints.Add(currentPoint);
						}
					}

					previousPoint = currentPoint;
				}
			}

			if (outputPoints.Count >= 2)
			{
				yield return outputPoints;
			}
		}
	}
}