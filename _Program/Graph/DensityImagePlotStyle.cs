/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002 Dr. Dirk Lellinger
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

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;

namespace Altaxo.Graph
{
	/// <summary>
	/// This plot style is responsible for showing density plots as pixel image. Because of the limitation to a pixel image, each pixel is correlated
	/// with a single data point in the table. Splining of data is not implemented here. Beause of this limitation, the image can only be shown
	/// on linear axes.
	/// </summary>
	[SerializationSurrogate(0,typeof(DensityImagePlotStyle.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public class DensityImagePlotStyle 
		:
		System.ICloneable, 
		System.Runtime.Serialization.IDeserializationCallback, 
		Main.IChangedEventSource,
		Main.IDocumentNode
	{


			protected object m_Parent;

			/// <summary>
			/// The image which is shown during paint.
			/// </summary>
 			System.Drawing.Bitmap m_Image;

		
		/// <summary>
		/// Indicates that the cached data (i.e. the image in this case) is valid and up to date.
		/// </summary>
		bool m_bCachedDataValid=false;


		#region Serialization
		/// <summary>Used to serialize the XYLineScatterPlotStyle Version 0.</summary>
		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			/// <summary>
			/// Serializes XYLineScatterPlotStyle Version 0.
			/// </summary>
			/// <param name="obj">The DensityImagePlotStyle to serialize.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				DensityImagePlotStyle s = (DensityImagePlotStyle)obj;
			
				// nothing to save up to now
			}
			/// <summary>
			/// Deserializes the DensityImagePlotStyle Version 0.
			/// </summary>
			/// <param name="obj">The empty DensityImagePlotStyle to deserialize into.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			/// <param name="selector">The deserialization surrogate selector.</param>
			/// <returns>The deserialized DensityImagePlotStyle.</returns>
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				DensityImagePlotStyle s = (DensityImagePlotStyle)obj;
				s.InitializeMembers();

				// Nothing to deserialize in the moment

				return s;
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DensityImagePlotStyle),0)]
			public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				DensityImagePlotStyle s = (DensityImagePlotStyle)obj;
					
				// nothing to save up to now
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				
				DensityImagePlotStyle s = null!=o ? (DensityImagePlotStyle)o : new DensityImagePlotStyle();

				// Nothing to deserialize in the moment
			
				return s;
			}
		}

		/// <summary>
		/// Finale measures after deserialization.
		/// </summary>
		/// <param name="obj">Not used.</param>
		public virtual void OnDeserialization(object obj)
		{
			// At the moment, there is nothing to do here
		}
		#endregion


		/// <summary>
		/// Initialized the member variables to default values.
		/// </summary>
		protected void InitializeMembers()
		{
			m_Image = null;
			m_bCachedDataValid = false;
		}

		/// <summary>
		/// Initializes the style to default values.
		/// </summary>
		public DensityImagePlotStyle()
		{
			InitializeMembers();
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="from">The style to copy from.</param>
		public DensityImagePlotStyle(DensityImagePlotStyle from)
		{
			InitializeMembers();
		}


		public object Clone()
		{
			return new DensityImagePlotStyle(this);
		}

		/// <summary>
		/// Called by the parent plot item to indicate that the associated data has changed. Used to invalidate the cached bitmap to force
		/// rebuilding the bitmap from new data.
		/// </summary>
		/// <param name="sender">The sender of the message.</param>
		public void EhDataChanged(object sender)
		{
			m_bCachedDataValid = false;
		}
	


		/// <summary>
		/// Paint the density image in the layer.
		/// </summary>
		/// <param name="gfrx">The graphics context painting in.</param>
		/// <param name="gl">The layer painting in.</param>
		/// <param name="plotObject">The data to plot.</param>
		public void Paint(Graphics gfrx, Graph.XYPlotLayer gl, object plotObject) // plots the curve with the choosen style
			{
			if(!(plotObject is XYZEquidistantMeshColumnPlotData))
				return; // we cannot plot any other than a TwoDimMeshDataAssociation now

				XYZEquidistantMeshColumnPlotData myPlotAssociation = (XYZEquidistantMeshColumnPlotData)plotObject;

				Altaxo.Data.INumericColumn xColumn = myPlotAssociation.XColumn as Altaxo.Data.INumericColumn;
				Altaxo.Data.INumericColumn yColumn = myPlotAssociation.YColumn as Altaxo.Data.INumericColumn;

				if(null==xColumn || null==yColumn)
					return; // this plotstyle is only for x and y double columns



				double layerWidth = gl.Size.Width;
				double layerHeight = gl.Size.Height;

				int cols = myPlotAssociation.ColumnCount;
				int rows = myPlotAssociation.RowCount;


				if(cols<=0 || rows<=0)
					return; // we cannot show a picture if one length is zero


			// there is a need for rebuilding the bitmap only if the data are invalid for some reason
			if(!m_bCachedDataValid)
			{

				// look if the image has the right dimensions
				if(null==m_Image || m_Image.Width != cols || m_Image.Height != rows)
				{
					if(null!=m_Image)
						m_Image.Dispose();

					// please notice: the horizontal direction of the image is related to the row index!!! (this will turn the image in relation to the table)
					// and the vertical direction of the image is related to the column index
					m_Image = new System.Drawing.Bitmap(rows,cols,System.Drawing.Imaging.PixelFormat.Format32bppArgb);
				}

				// now we can fill the image with our data

				PhysicalBoundaries pb = new FinitePhysicalBoundaries();
				myPlotAssociation.MergeVBoundsInto(pb);

				double vmin = pb.LowerBound;
				double vmax = pb.UpperBound;
				double vmid = (vmin+vmax)*0.5;
				double vscal = vmax<=vmin ? 1 : 255.0/(vmax - vmin);
				double vscal2 = 2*vscal;

				int r,g,b;

				for(int i=0;i<cols;i++)
				{
					Altaxo.Data.INumericColumn col = myPlotAssociation.DataColumns[i] as Altaxo.Data.INumericColumn;
					if(null==col)
						continue;

					for(int j=0;j<rows;j++)
					{
						double val = col.GetDoubleAt(j);
						if(double.IsNaN(val))
						{
							m_Image.SetPixel(j,cols-i-1,System.Drawing.Color.Transparent); // invalid pixels are transparent
						}
						else // a valid pixel
						{
							r =((int)(Math.Abs((val-vmin)*vscal)))%256;
							g=((int)(Math.Abs((val-vmin)*vscal2)))%256;
							b=((int)(Math.Abs((vmax-val)*vscal)))%256;
							m_Image.SetPixel(j,cols-i-1,System.Drawing.Color.FromArgb(r,g,b));
						}
					} // for all pixel of a column
				} // for all columns


				m_bCachedDataValid = true; // now the bitmap is valid
			}


			double x_rel_left = gl.XAxis.PhysicalToNormal(xColumn.GetDoubleAt(0));
			double x_rel_right = gl.XAxis.PhysicalToNormal(xColumn.GetDoubleAt(rows-1));

			double y_rel_bottom = gl.YAxis.PhysicalToNormal(yColumn.GetDoubleAt(0));
			double y_rel_top = gl.YAxis.PhysicalToNormal(yColumn.GetDoubleAt(cols-1));

			gfrx.DrawImage(m_Image,(float)(layerWidth*x_rel_left),(float)(layerHeight*(1-y_rel_top)),(float)(layerWidth*(x_rel_right-x_rel_left)),(float)(layerHeight*(y_rel_top-y_rel_bottom)));
			}

		
		#region IChangedEventSource Members

		public event System.EventHandler Changed;

		protected virtual void OnChanged()
		{
			if(null!=Changed)
				Changed(this,new EventArgs());
		}

		#endregion

		public virtual object ParentObject
		{
			get { return m_Parent; }
			set { m_Parent = value; }
		}

		public virtual string Name
		{
			get
			{
				Main.INamedObjectCollection noc = ParentObject as Main.INamedObjectCollection;
				return null==noc ? null : noc.GetNameOfChildObject(this);
			}
		}


	} // end of class DensityImagePlotStyle
}
