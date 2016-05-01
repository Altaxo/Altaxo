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

namespace Altaxo.Drawing.D3D.LineCaps
{
	public abstract class ContourShapedLineCapBase : ILineCap
	{
		public abstract double BaseInset { get; }

		public virtual double MinimumRelativeSize
		{
			get
			{
				return 1;
			}
		}

		public virtual double MinimumAbsoluteSizePt
		{
			get
			{
				return 0;
			}
		}

		public virtual string Name
		{
			get
			{
				return this.GetType().FullName;
			}
		}

		public virtual ILineCap WithMinimumAbsoluteAndRelativeSize(double absoluteSizePt, double relativeSize)
		{
			return this;
		}

		public virtual double GetAbsoluteBaseInset(double thickness1, double thickness2)
		{
			return BaseInset * 0.5 * Math.Max(thickness1, thickness2);
		}

		/// <summary>
		/// we have 3 different situations here:
		/// 1st) the crossSection.Y is zero, thus this is the middle point (the normal should then go in z-direction) -> we need only one single vertex and normal for that
		/// 2nd) the crossSection.Y is zero but sharp, thus this is the middle point, but we need a normal for each triangle -> thus we need NumberOfVertices normals
		/// 3rd) the countour normal is in x-direction  -> we need only a point for each crossSection vertex, but not for each crossSectionNormal
		/// 4th) the regular case -> we need a point for each crossSection normal
		/// </summary>
		private enum CrossSectionCases
		{
			MiddlePointSmooth,
			MiddlePointSharp,
			VerticesOnly,
			Regular
		}

		public abstract void AddGeometry(
			Action<PointD3D, VectorD3D> AddPositionAndNormal,
			Action<int, int, int, bool> AddIndices,
			ref int vertexIndexOffset,
			bool isStartCap,
			PointD3D basePoint,
			VectorD3D eastVector,
			VectorD3D northVector,
			VectorD3D forwardVectorNormalized,
			ICrossSectionOfLine lineCrossSection,
			PointD3D[] baseCrossSectionPositions,
			VectorD3D[] baseCrossSectionNormals,
			ref object temporaryStorageSpace);

		public static void Add(
			Action<PointD3D, VectorD3D> AddPositionAndNormal,
			Action<int, int, int, bool> AddIndices,
			ref int vertexIndexOffset,
			bool isStartCap,
			PointD3D basePoint,
			VectorD3D westVector,
			VectorD3D northVector,
			VectorD3D forwardVectorNormalized,
			ICrossSectionOfLine lineCrossSection,
			PointD3D[] crossSectionPositions,
			VectorD3D[] crossSectionNormals,
			ref object temporaryStorageSpace,
			ILineCapContour capContour
			)
		{
			var crossSectionVertexCount = lineCrossSection.NumberOfVertices;
			var crossSectionNormalCount = lineCrossSection.NumberOfNormals;
			var contourZScale = 0.5 * Math.Max(lineCrossSection.Size1, lineCrossSection.Size2);

			if (isStartCap)
				forwardVectorNormalized = -forwardVectorNormalized;

			var contourVertexCount = capContour.NumberOfVertices;
			var contourNormalCount = capContour.NumberOfNormals;

			/*
			// do we need a flat end on the other side of the cap ?
			if (null == crossSectionPositions) // if lineCrossSectionPositions are null, it means that our cap is not connected to the line and needs a flat end
			{
				// the parameter isStartCap must be negated, because this flat cap is the "counterpart" of our cap to draw
				Flat.AddGeometry(AddPositionAndNormal, AddIndices, ref vertexIndexOffset, !isStartCap, basePoint, forwardVectorNormalized, capCrossSectionPositions);
			}
			*/

			// now the calculation can start

			CrossSectionCases previousCrossSectionType = CrossSectionCases.MiddlePointSmooth;
			int previousGeneratedPoints = 0;
			int previousContourVertexIndex = 0;
			bool isOnSecondSideOfContourVertexSharp = true;
			for (int contourVertexIndex = 0, contourNormalIndex = 0; contourVertexIndex < contourVertexCount; ++contourVertexIndex, ++contourNormalIndex)
			{
				// we have 4 different situations here:
				// 1st) the crossSection.Y is zero, thus this is the middle point (the normal should then go in z-direction) -> we need only one single vertex and normal for that
				// 2nd) the countour normal is in x-direction  -> we need only a point for each crossSection vertex, but not for each crossSectionNormal
				// 3rd) the regular case -> we need a point for each crossSection normal

				var capContourVertex = capContour.Vertices(contourVertexIndex);
				var capContourNormal = capContour.Normals(contourNormalIndex);

				CrossSectionCases currentCrossSectionType;
				if (capContourVertex.Y == 0 && capContourNormal.Y == 0)
					currentCrossSectionType = CrossSectionCases.MiddlePointSmooth;
				else if (capContourVertex.Y == 0)
					currentCrossSectionType = CrossSectionCases.MiddlePointSharp;
				else if (0 == capContourNormal.Y)
					currentCrossSectionType = CrossSectionCases.VerticesOnly;
				else
					currentCrossSectionType = CrossSectionCases.Regular;

				var currentLocation = basePoint + forwardVectorNormalized * capContourVertex.X * contourZScale;
				var matrix = Matrix4x3.NewFromBasisVectorsAndLocation(westVector, northVector, forwardVectorNormalized, currentLocation);

				int currentGeneratedPoints = 0;
				switch (currentCrossSectionType)
				{
					case CrossSectionCases.MiddlePointSmooth:
						{
							var position = matrix.Transform(PointD2D.Empty);
							var normal = matrix.Transform(new VectorD3D(0, 0, capContourNormal.X));
							AddPositionAndNormal(position, normal);
							currentGeneratedPoints = 1;
						}
						break;

					case CrossSectionCases.MiddlePointSharp:
						{
							for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
							{
								var normal1 = (i == 0) ? lineCrossSection.Normals(crossSectionNormalCount - 1) : lineCrossSection.Normals(j - 1);
								var normal2 = lineCrossSection.Normals(j);
								var sn = (normal1 + normal2).Normalized;
								var utNormal = GetNormalVector(lineCrossSection.Vertices(i), sn, capContourNormal, contourZScale);
								AddPositionAndNormal(currentLocation, matrix.Transform(utNormal)); // store the tip point with the averaged normal
								if (lineCrossSection.IsVertexSharp(i))
								{
									++j;
								}
							}
							currentGeneratedPoints = crossSectionVertexCount;
						}
						break;

					case CrossSectionCases.VerticesOnly:
						{
							var commonNormal = matrix.Transform(new VectorD3D(0, 0, capContourNormal.X));
							for (int i = 0; i < crossSectionVertexCount; ++i)
							{
								var position = matrix.Transform(lineCrossSection.Vertices(i) * capContourVertex.Y);
								AddPositionAndNormal(position, commonNormal);
							}
							currentGeneratedPoints = crossSectionVertexCount;
						}
						break;

					case CrossSectionCases.Regular:
						{
							for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
							{
								var sp = lineCrossSection.Vertices(i);
								var sn = lineCrossSection.Normals(j);
								var utNormal = GetNormalVector(sp, sn, capContourNormal, contourZScale);
								var position = matrix.Transform(sp * capContourVertex.Y);
								var normal = matrix.Transform(utNormal);

								AddPositionAndNormal(position, normal);

								if (lineCrossSection.IsVertexSharp(i))
								{
									++j;
									sn = lineCrossSection.Normals(j);
									utNormal = GetNormalVector(sp, sn, capContourNormal, contourZScale);
									normal = matrix.Transform(utNormal);
									AddPositionAndNormal(position, normal);
								}
							}
							currentGeneratedPoints = crossSectionNormalCount;
						}
						break;

					default:
						throw new NotImplementedException();
				}
				vertexIndexOffset += currentGeneratedPoints;

				// now we start generating triangles

				if (contourVertexIndex > previousContourVertexIndex)
				{
					int voffset1 = vertexIndexOffset - currentGeneratedPoints;
					int voffset0 = voffset1 - previousGeneratedPoints;
					switch (previousCrossSectionType)
					{
						case CrossSectionCases.MiddlePointSmooth:
							{
								switch (currentCrossSectionType)
								{
									case CrossSectionCases.MiddlePointSmooth: // Middle point to middle point
										{
											// no triangles, since from middle point to middle point we have an infinity thin line
										}
										break;

									case CrossSectionCases.MiddlePointSharp: // Middle point to middle point
										{
											// no triangles, since from middle point to middle point we have an infinity thin line
										}
										break;

									case CrossSectionCases.VerticesOnly: // Middle point to vertices only
										{
											for (int i = 0; i < crossSectionVertexCount; ++i)
											{
												AddIndices(voffset0, voffset1 + i, voffset1 + (i + 1) % crossSectionVertexCount, isStartCap);
											}
										}
										break;

									case CrossSectionCases.Regular: // Middle point to regular
										{
											for (int i = 0; i < crossSectionNormalCount; ++i)
											{
												AddIndices(voffset0, voffset1 + i, voffset1 + (i + 1) % crossSectionNormalCount, isStartCap);
											}
										}
										break;

									default:
										throw new NotImplementedException();
								}
							}
							break;

						case CrossSectionCases.MiddlePointSharp:
							{
								switch (currentCrossSectionType)
								{
									case CrossSectionCases.MiddlePointSmooth: // Middle point to middle point
										{
											// no triangles, since from middle point to middle point we have an infinity thin line
										}
										break;

									case CrossSectionCases.MiddlePointSharp: // Middle point to middle point
										{
											// no triangles, since from middle point to middle point we have an infinity thin line
										}
										break;

									case CrossSectionCases.VerticesOnly: // MiddlePointSharp to VerticesOnly
										{
											for (int i = 0; i < crossSectionVertexCount; ++i)
											{
												AddIndices(voffset0, voffset1 + i, voffset1 + (i + 1) % crossSectionVertexCount, isStartCap);
											}
										}
										break;

									case CrossSectionCases.Regular: // MiddlePointSharp to Regular
										{
											for (int i = 0, j = 0; i < crossSectionNormalCount; ++i, ++j)
											{
												AddIndices(voffset0 + i, voffset1 + i, voffset1 + (i + 1) % crossSectionNormalCount, isStartCap);
											}
										}
										break;

									default:
										throw new NotImplementedException();
								}
							}
							break;

						case CrossSectionCases.VerticesOnly:
							{
								switch (currentCrossSectionType)
								{
									case CrossSectionCases.MiddlePointSmooth: // VerticesOnly to MiddlePoint
										{
											for (int i = 0; i < crossSectionVertexCount; ++i)
											{
												AddIndices(voffset1, voffset0 + i, voffset0 + (i + 1) % crossSectionVertexCount, isStartCap);
											}
										}
										break;

									case CrossSectionCases.VerticesOnly: // VerticesOnly to VerticesOnly
										{
											for (int i = 0; i < crossSectionVertexCount; ++i)
											{
												AddIndices(voffset0 + ((i == 0) ? crossSectionVertexCount - 1 : i - 1), voffset0 + i, voffset1 + i, isStartCap);
												AddIndices(voffset0 + ((i == 0) ? crossSectionVertexCount - 1 : i - 1), voffset1 + i, voffset1 + ((i == 0) ? crossSectionVertexCount - 1 : i - 1), isStartCap);
											}
										}
										break;

									case CrossSectionCases.Regular: // VerticesOnly to regular
										{
										}
										break;

									default:
										throw new NotImplementedException();
								}
							}
							break;

						case CrossSectionCases.Regular:
							{
								switch (currentCrossSectionType)
								{
									case CrossSectionCases.MiddlePointSmooth: // Regular to MiddlePointOnly
										{
											for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
											{
												AddIndices(voffset0 + ((j == 0) ? crossSectionNormalCount - 1 : j - 1), voffset0 + j, voffset1, isStartCap);

												if (lineCrossSection.IsVertexSharp(i))
													++j;
											}
										}
										break;

									case CrossSectionCases.MiddlePointSharp: // Regular to MiddlePointSharp
										{
											for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
											{
												AddIndices(voffset0 + ((j == 0) ? crossSectionNormalCount - 1 : j - 1), voffset0 + j, voffset1 + i, isStartCap);

												if (lineCrossSection.IsVertexSharp(i))
													++j;
											}
										}
										break;

									case CrossSectionCases.VerticesOnly: // Regular to VerticesOnly
										{
											for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
											{
												AddIndices(voffset0 + ((j == 0) ? crossSectionNormalCount - 1 : j - 1), voffset0 + j, voffset1 + i, isStartCap);
												AddIndices(voffset0 + ((j == 0) ? crossSectionNormalCount - 1 : j - 1), voffset1 + i, voffset1 + ((i == 0) ? crossSectionVertexCount - 1 : i - 1), isStartCap);

												if (lineCrossSection.IsVertexSharp(i))
													++j;
											}
										}
										break;

									case CrossSectionCases.Regular: // Regular to Regular
										{
											for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
											{
												AddIndices(voffset0 + ((j == 0) ? crossSectionNormalCount - 1 : j - 1), voffset0 + j, voffset1 + j, isStartCap);
												AddIndices(voffset0 + ((j == 0) ? crossSectionNormalCount - 1 : j - 1), voffset1 + j, voffset1 + ((j == 0) ? crossSectionNormalCount - 1 : j - 1), isStartCap);

												if (lineCrossSection.IsVertexSharp(i))
													++j;
											}
										}
										break;

									default:
										throw new NotImplementedException();
								}
							}
							break;

						default:
							throw new NotImplementedException();
					}
				}

				if (!isOnSecondSideOfContourVertexSharp && capContour.IsVertexSharp(contourVertexIndex) && contourVertexIndex < (contourVertexCount - 1))
				{
					previousContourVertexIndex = contourVertexIndex;
					--contourVertexIndex; // trick: decrement the vertex index, it is incremented then again in the following for loop, so that contourVertexIndex stays constant
					isOnSecondSideOfContourVertexSharp = true;
					continue;
				}
				isOnSecondSideOfContourVertexSharp = false;

				// now we switch the current calculated positions and normals with the old ones
				previousCrossSectionType = currentCrossSectionType;
				previousGeneratedPoints = currentGeneratedPoints;
				previousContourVertexIndex = contourVertexIndex;
			}
		}

		/// <summary>
		/// Calculates the normal to a extruded contour. Here the extrusion direction is fixed to z-direction.
		/// </summary>
		/// <param name="crossSectionPoint">Original cross section point coordinates.</param>
		/// <param name="crossSectionNormal">Original cross section normal.</param>
		/// <param name="contourNormal">Original contour normal.</param>
		/// <param name="contourZScale">Factor that is multiplied with the x-coordinate of the contour point to return the z-coordinate of the resulting contour.</param>
		/// <returns>The normal of the extruded contour (x-y is the cross section plane, z the extrusion direction). This vector has then to be transformed into the 3D-space of the body.</returns>
		protected static VectorD3D GetNormalVector(PointD2D crossSectionPoint, VectorD2D crossSectionNormal, VectorD2D contourNormal, double contourZScale)
		{
			return VectorD3D.CreateNormalized
				(
				 contourNormal.Y * crossSectionNormal.X * contourZScale,
				 contourNormal.Y * crossSectionNormal.Y * contourZScale,
				 contourNormal.X * (crossSectionNormal.X * crossSectionPoint.X + crossSectionNormal.Y * crossSectionPoint.Y)
				);
		}

		protected class PositionAndNormalStorageSpace
		{
			public PointD3D[] Positions;
			public VectorD3D[] Normals;
		}
	}
}