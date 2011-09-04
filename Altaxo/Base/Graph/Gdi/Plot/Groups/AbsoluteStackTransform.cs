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
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Altaxo.Data;
using Altaxo.Graph.Scales.Boundaries;

namespace Altaxo.Graph.Gdi.Plot.Groups
{
	using Plot.Data;

	/// <summary>
	/// 
	/// </summary>
	public class AbsoluteStackTransform : ICoordinateTransformingGroupStyle
	{
		#region Serialization
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AbsoluteStackTransform), 0)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				AbsoluteStackTransform s = (AbsoluteStackTransform)obj;

			}


			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				AbsoluteStackTransform s = null != o ? (AbsoluteStackTransform)o : new AbsoluteStackTransform();
				return s;
			}
		}

		#endregion




		public AbsoluteStackTransform()
		{

		}

		public AbsoluteStackTransform(AbsoluteStackTransform from)
		{
		}

		#region ICoordinateTransformingGroupStyle Members

		public void MergeXBoundsInto(IPlotArea layer, IPhysicalBoundaries pb, PlotItemCollection coll)
		{
			CoordinateTransformingStyleBase.MergeXBoundsInto(pb, coll);

		}

		public void MergeYBoundsInto(IPlotArea layer, IPhysicalBoundaries pb, PlotItemCollection coll)
		{
			Dictionary<G2DPlotItem, Processed2DPlotData> plotDataList;
			IPhysicalBoundaries pbclone = (IPhysicalBoundaries)pb.Clone(); // before we can use CanUseStyle, we have to give physical y boundaries template
			CoordinateTransformingStyleBase.MergeYBoundsInto(pbclone, coll);
			if (!CanUseStyle(layer, coll, out plotDataList))
			{
				pb.Add(pbclone);
				return;
			}

			// we put zero into the y-Boundaries, since the addition starts with that value
			pb.Add(new AltaxoVariant(0.0));

			AltaxoVariant[] ySumArray = null;

			int idx = -1;
			foreach (IGPlotItem pi in coll)
			{
				if (pi is G2DPlotItem)
				{
					idx++;

					G2DPlotItem gpi = (G2DPlotItem)pi;
					Processed2DPlotData pdata = plotDataList[gpi];

					// Note: we can not use AddUp function here, since
					// when we have positive/negative items, the intermediate bounds
					// might be wider than the bounds of the end result

					if (ySumArray == null)
					{
						ySumArray = new AltaxoVariant[pdata.RangeList.PlotPointCount];

						int j = -1;
						foreach (int originalIndex in pdata.RangeList.OriginalRowIndices())
						{
							j++;
							ySumArray[j] = pdata.GetYPhysical(originalIndex);
							pb.Add(ySumArray[j]);
						}
					}
					else // this is not the first item
					{
						int j = -1;
						foreach (int originalIndex in pdata.RangeList.OriginalRowIndices())
						{
							j++;
							ySumArray[j] += pdata.GetYPhysical(originalIndex);
							pb.Add(ySumArray[j]);

						}
					}
				}
			}
		}



		/// <summary>
		/// Determines whether the plot items in <paramref name="coll"/> can be plotted as stack. Presumption is that all plot items 
		/// have the same number of plot points, and that all items have the same order of x values associated with the plot points.
		/// </summary>
		/// <param name="layer">Plot layer.</param>
		/// <param name="coll">Collection of plot items.</param>
		/// <param name="plotDataList">Output: dictionary with associates each plot item with a list of processed plot data.</param>
		/// <returns>Returns <c>true</c> if the plot items in <paramref name="coll"/> can be plotted as stack; otherwise, <c>false</c>.</returns>
		public static bool CanUseStyle(IPlotArea layer, PlotItemCollection coll, out Dictionary<G2DPlotItem, Processed2DPlotData> plotDataList)
		{
			plotDataList = new Dictionary<G2DPlotItem, Processed2DPlotData>();

			AltaxoVariant[] xArray = null;

			int idx = -1;
			foreach (IGPlotItem pi in coll)
			{
				if (pi is G2DPlotItem)
				{
					idx++;
					G2DPlotItem gpi = (G2DPlotItem)pi;
					Processed2DPlotData pdata = gpi.GetRangesAndPoints(layer);
					plotDataList.Add(gpi, pdata);

					if (xArray == null)
					{
						xArray = new AltaxoVariant[pdata.RangeList.PlotPointCount];

						int j = -1;
						foreach (int originalIndex in pdata.RangeList.OriginalRowIndices())
						{
							j++;
							xArray[j] = pdata.GetXPhysical(originalIndex);
						}
					}
					else // this is not the first item
					{
						if (pdata.RangeList.PlotPointCount != xArray.Length)
							return false;


						int j = -1;
						foreach (int originalIndex in pdata.RangeList.OriginalRowIndices())
						{
							j++;


							if (xArray[j] != pdata.GetXPhysical(originalIndex))
								return false;
						}
					}
				}
			}

			return idx >= 1;
		}


		/// <summary>Adds the y-values of a plot item to an array of y-values..</summary>
		/// <param name="yArray">The y array to be added to. If null, a new array will be allocated (and filled with the y-values of the plot item).</param>
		/// <param name="pdata">The pdata.</param>
		/// <returns>If the parameter <paramref name="yArray"/> was not null, then that <paramref name="yArray"/> is returned. Otherwise the newly allocated array is returned.</returns>
		public static AltaxoVariant[] AddUp(AltaxoVariant[] yArray, Processed2DPlotData pdata)
		{
			if (yArray == null)
			{
				yArray = new AltaxoVariant[pdata.RangeList.PlotPointCount];

				int j = -1;
				foreach (int originalIndex in pdata.RangeList.OriginalRowIndices())
				{
					j++;
					yArray[j] = pdata.GetYPhysical(originalIndex);
				}
			}
			else // this is not the first item
			{
				int j = -1;
				foreach (int originalIndex in pdata.RangeList.OriginalRowIndices())
				{
					j++;
					yArray[j] += pdata.GetYPhysical(originalIndex);
				}
			}
			return yArray;
		}

		/// <summary>Paints the plot items.</summary>
		/// <param name="g">Graphics context used for drawing.</param>
		/// <param name="layer">Plot layer.</param>
		/// <param name="coll">Collection of plot items to draw.</param>
		public void Paint(System.Drawing.Graphics g, IPlotArea layer, PlotItemCollection coll)
		{
			Dictionary<G2DPlotItem, Processed2DPlotData> plotDataDict;
			if (!CanUseStyle(layer, coll, out plotDataDict))
			{
				CoordinateTransformingStyleBase.Paint(g, layer, coll);
				return;
			}

			AltaxoVariant[] yArray = null;
			// First, add up all items since we start always with the last item
			int idx = -1;
			Processed2DPlotData previousItemData = null;
			foreach (IGPlotItem pi in coll)
			{

				if (pi is G2DPlotItem)
				{
					idx++;

					G2DPlotItem gpi = pi as G2DPlotItem;
					Processed2DPlotData pdata = plotDataDict[gpi];
					yArray = AddUp(yArray, pdata);

					if (idx > 0) // this is not the first item
					{
						int j = -1;
						foreach (int originalIndex in pdata.RangeList.OriginalRowIndices())
						{
							j++;
							Logical3D rel = new Logical3D(
							layer.XAxis.PhysicalVariantToNormal(pdata.GetXPhysical(originalIndex)),
							layer.YAxis.PhysicalVariantToNormal(yArray[j]));

							double xabs, yabs;
							layer.CoordinateSystem.LogicalToLayerCoordinates(rel, out xabs, out yabs);
							pdata.PlotPointsInAbsoluteLayerCoordinates[j] = new System.Drawing.PointF((float)xabs, (float)yabs);
						}
					}

					// we have also to exchange the accessor for the physical y value and replace it by our own one
					AltaxoVariant[] localArray = (AltaxoVariant[])yArray.Clone();
					LocalArrayHolder localArrayHolder = new LocalArrayHolder(localArray, pdata);
					pdata.YPhysicalAccessor = localArrayHolder.GetPhysical;
					pdata.PreviousItemData = previousItemData;
					previousItemData = pdata;
				}
			}

			Processed2DPlotData prevPlotData = null;
			Processed2DPlotData nextPlotData = null;
			Processed2DPlotData currPlotData = null;
			for (int i = coll.Count - 1; i >= 0; --i)
			{
				if (i > 0 && (coll[i - 1] is G2DPlotItem))
					nextPlotData = plotDataDict[coll[i - 1] as G2DPlotItem];
				else
					nextPlotData = null;


				if (coll[i] is G2DPlotItem)
				{
					var gpi = coll[i] as G2DPlotItem;
					currPlotData = plotDataDict[gpi];
					gpi.Paint(g, layer, currPlotData, prevPlotData, nextPlotData);
				}
				else
				{
					currPlotData = null;
					coll[i].Paint(g, layer, null, null);
				}

				prevPlotData = currPlotData;
				currPlotData = nextPlotData;
				nextPlotData = null;
			}

		}


		/// <summary>
		/// Private class to hold the transformed physical y values. The problem is that <see cref="GetPhysical"/> is called with the original row index (and not the plot index),
		/// thus a dictionary with translates the original row index to the index in the array is neccessary.
		/// </summary>
		private class LocalArrayHolder
		{
			AltaxoVariant[] _localArray;
			Dictionary<int, int> _originalRowIndexToPlotIndex;

			/// <summary>Initializes a new instance of the <see cref="LocalArrayHolder"/> class.</summary>
			/// <param name="localArray">The local array of transformed y values. The array length is equal to the number of plot points.</param>
			/// <param name="pdata">The processed plot data. The plot range member of this instance is used to build a dictionary which associates the original row indices to the indices of the <paramref name="localArray"/>.</param>
			public LocalArrayHolder(AltaxoVariant[] localArray, Processed2DPlotData pdata)
			{
				_localArray = localArray;
				_originalRowIndexToPlotIndex = pdata.RangeList.GetDictionaryOfOriginalRowIndicesToPlotIndices();
			}

			/// <summary>Gets the physical y value for a given original row index.</summary>
			/// <param name="originalRowIndex">Index of the original row.</param>
			/// <returns>The (now transformed) y value of the given original row index. If it is not found in the dictionary which associates the original row indices to the indices of the <paramref name="localArray"/>, a empty <see cref="AltaxoVariant"/> instance is returned.</returns>
			public AltaxoVariant GetPhysical(int originalRowIndex)
			{
				int idx;
				if (_originalRowIndexToPlotIndex.TryGetValue(originalRowIndex, out idx))
					return _localArray[idx];
				else
					return new AltaxoVariant();
			}
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			return new AbsoluteStackTransform(this);
		}

		#endregion

		#region ICoordinateTransformingGroupStyle Members



		#endregion
	}

}
