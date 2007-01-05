#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;
using Altaxo.Graph.Scales.Boundaries;

namespace Altaxo.Graph.Gdi.Plot.Styles
{
  using Plot.Data;
  using Graph.Plot.Data;

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


  

    /// <summary>
    /// The image which is shown during paint.
    /// </summary>
    System.Drawing.Bitmap m_Image;

    
    /// <summary>
    /// Indicates that the cached data (i.e. the image in this case) is valid and up to date.
    /// </summary>
    bool m_bCachedDataValid=false;

    /// <summary>The lower bound of the plot range</summary>
    double m_RangeFrom = double.NaN; 

    /// <summary>The upper bound of the plot range</summary>
    double m_RangeTo = double.NaN;

    /// <summary>If true, the image is clipped to the layer boundaries.</summary>
    bool   m_ClipToLayer = true;

    /// <summary>The color used if the values are below the lower bound.</summary>
    System.Drawing.Color m_ColorBelow = System.Drawing.Color.Black;

    /// <summary>The color used if the values are above the upper bound.</summary>
    System.Drawing.Color m_ColorAbove = System.Drawing.Color.Snow;

    /// <summary>The color used for invalid values (missing values).</summary>
    System.Drawing.Color m_ColorInvalid = System.Drawing.Color.Transparent;

    /// <summary>
    /// The kind of scaling of the values between from and to.
    /// </summary>
    public enum ScalingStyle 
    {
      /// <summary>Linear scale, i.e. color changes linear between from and to.</summary>
      Linear,

      /// <summary>Logarithmic style, i.e. color changes with log(value) between log(from) and log(to).</summary>
      Logarithmic 
    };

    /// <summary>The style for scaling of the values between from and to.</summary>
    ScalingStyle m_ScalingStyle = ScalingStyle.Linear;

    [NonSerialized]
    protected object m_Parent;

    [field: NonSerialized]
    public event System.EventHandler Changed;



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
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
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

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.DensityImagePlotStyle",0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.DensityImagePlotStyle",1)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DensityImagePlotStyle),2)]
      class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        DensityImagePlotStyle s = (DensityImagePlotStyle)obj;
        
        info.AddEnum("ScalingStyle",s.m_ScalingStyle);
        info.AddValue("RangeFrom",s.m_RangeFrom);
        info.AddValue("RangeTo",s.m_RangeTo);
        info.AddValue("ClipToLayer",s.m_ClipToLayer);
        info.AddValue("ColorBelow",s.m_ColorBelow);
        info.AddValue("ColorAbove",s.m_ColorAbove);
        info.AddValue("ColorInvalid",s.m_ColorInvalid);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        DensityImagePlotStyle s = null!=o ? (DensityImagePlotStyle)o : new DensityImagePlotStyle();
        
        s.m_ScalingStyle = (DensityImagePlotStyle.ScalingStyle)info.GetEnum("ScalingStyle",s.m_ScalingStyle.GetType());
        s.m_RangeFrom    = info.GetDouble("RangeFrom");
        s.m_RangeTo      = info.GetDouble("RangeTo");
        s.m_ClipToLayer  = info.GetBoolean("ClipToLayer");
        s.m_ColorBelow   = (System.Drawing.Color)info.GetValue("ColorBelow",parent);
        s.m_ColorAbove   = (System.Drawing.Color)info.GetValue("ColorAbove",parent);
        s.m_ColorInvalid = (System.Drawing.Color)info.GetValue("ColorInvalid",parent);

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

      this.m_ClipToLayer = from.m_ClipToLayer;
      this.m_RangeFrom = from.m_RangeFrom;
      this.m_RangeTo   = from.m_RangeTo;
      this.m_ColorAbove = from.m_ColorAbove;
      this.m_ColorBelow = from.m_ColorBelow;
      this.m_ColorInvalid = from.m_ColorInvalid;
      this.m_ScalingStyle = from.m_ScalingStyle;

      this.m_Parent = from.m_Parent;

      this.m_bCachedDataValid = false;
    }


    public object Clone()
    {
      return new DensityImagePlotStyle(this);
    }


    public ScalingStyle Scaling
    {
      get { return m_ScalingStyle; }
      set
      {
        ScalingStyle oldValue = m_ScalingStyle;
        
        m_ScalingStyle = value;
        if(m_ScalingStyle != oldValue)
        {
          m_bCachedDataValid = false;
          OnChanged();
        }
      }
    }


    public bool ClipToLayer
    {
      get { return m_ClipToLayer; }
      set
      {
        bool oldValue = m_ClipToLayer;
        m_ClipToLayer = value;

        if(m_ClipToLayer != oldValue)
        {
          OnChanged();
        }

      }
    }


    private bool NaNEqual(double a, double b)
    {
      if(double.IsNaN(a) && double.IsNaN(b))
        return true;

      return a==b;
    }

    public double RangeFrom
    {
      get { return m_RangeFrom; }
      set
      {
        double oldValue = m_RangeFrom;
        m_RangeFrom = value;

        if(!NaNEqual(m_RangeFrom,oldValue))
        {
          m_bCachedDataValid = false;
          OnChanged();
        }
      }
    }

    public double RangeTo
    {
      get { return m_RangeTo; }
      set
      {
        double oldValue = m_RangeTo;
        m_RangeTo = value;
        if(!NaNEqual(m_RangeTo,oldValue))
        {
          m_bCachedDataValid = false;
          OnChanged();
        }
      }
    }


    public System.Drawing.Color ColorBelow
    {
      get { return m_ColorBelow; }
      set
      {
        Color oldValue = m_ColorBelow;
        m_ColorBelow = value;

        if(m_ColorBelow != oldValue)
        {
          m_bCachedDataValid = false;
          OnChanged();
        }

      }
    }

    public System.Drawing.Color ColorAbove
    {
      get { return m_ColorAbove; }
      set
      {
        Color oldValue = m_ColorAbove;
  
        m_ColorAbove = value;
        if(m_ColorAbove != oldValue)
        {
          m_bCachedDataValid = false;
          OnChanged();
        }

      }
    }

    public System.Drawing.Color ColorInvalid
    {
      get { return m_ColorInvalid; }
      set
      {
        Color oldValue = m_ColorInvalid;
        m_ColorInvalid = value;
        if(m_ColorInvalid != oldValue)
        {
          m_bCachedDataValid = false;
          OnChanged();
        }

      }
    }

    /// <summary>
    /// Called by the parent plot item to indicate that the associated data has changed. Used to invalidate the cached bitmap to force
    /// rebuilding the bitmap from new data.
    /// </summary>
    /// <param name="sender">The sender of the message.</param>
    public void EhDataChanged(object sender)
    {
      m_bCachedDataValid = false;
      OnChanged();
    }
  


    /// <summary>
    /// Paint the density image in the layer.
    /// </summary>
    /// <param name="gfrx">The graphics context painting in.</param>
    /// <param name="gl">The layer painting in.</param>
    /// <param name="plotObject">The data to plot.</param>
    public void Paint(Graphics gfrx, IPlotArea gl, object plotObject) // plots the curve with the choosen style
    {
      if(!(plotObject is XYZMeshedColumnPlotData))
        return; // we cannot plot any other than a TwoDimMeshDataAssociation now

      XYZMeshedColumnPlotData myPlotAssociation = (XYZMeshedColumnPlotData)plotObject;

      Altaxo.Data.INumericColumn xColumn = myPlotAssociation.XColumn as Altaxo.Data.INumericColumn;
      Altaxo.Data.INumericColumn yColumn = myPlotAssociation.YColumn as Altaxo.Data.INumericColumn;

      if(null==xColumn || null==yColumn)
        return; // this plotitem is only for x and y double columns



      //double layerWidth = gl.Size.Width;
      //double layerHeight = gl.Size.Height;

      int cols = myPlotAssociation.ColumnCount;
      int rows = myPlotAssociation.RowCount;


      if(cols<=0 || rows<=0)
        return; // we cannot show a picture if one length is zero


      // there is a need for rebuilding the bitmap only if the data are invalid for some reason
      if(!m_bCachedDataValid)
      {
        System.Diagnostics.Trace.WriteLine("DensityImagePlotStyle.Paint, calculate image data...");

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

        NumericalBoundaries pb = m_ScalingStyle==ScalingStyle.Logarithmic ? (NumericalBoundaries)new PositiveFiniteNumericalBoundaries() :  (NumericalBoundaries)new FiniteNumericalBoundaries();
        myPlotAssociation.SetVBoundsFromTemplate(pb); // ensure that the right v-boundary type is set
        myPlotAssociation.MergeVBoundsInto(pb);

        double vmin = double.IsNaN(this.m_RangeFrom) ? pb.LowerBound : Math.Max(pb.LowerBound,this.m_RangeFrom);
        double vmax = double.IsNaN(this.m_RangeTo) ? pb.UpperBound : Math.Min(pb.UpperBound, this.m_RangeTo);
        double lowerBound = vmin;
        double upperBound = vmax;

        if(this.m_ScalingStyle == ScalingStyle.Logarithmic)
        {
          // Ensure that min and max >0
          vmin = lowerBound = Math.Max(lowerBound,double.Epsilon);
          vmax = upperBound = Math.Max(lowerBound,upperBound); // lowerBound is ok, to ensure that upperBound>=lowerBound
          
          vmin = Math.Log(vmin);
          vmax = vmax>0 ? Math.Log(vmax) : vmin;
        }

        // double vmid = (vmin+vmax)*0.5;
        double vscal = vmax<=vmin ? 1 : 255.0/(vmax - vmin);

        int r,g,b;

        for(int i=0;i<cols;i++)
        {
          Altaxo.Data.INumericColumn col = myPlotAssociation.DataColumns[i] as Altaxo.Data.INumericColumn;
          if(null==col)
            continue;

          for(int j=0;j<rows;j++)
          {
            double val = col[j];
            if(double.IsNaN(val))
            {
              m_Image.SetPixel(j,cols-i-1,m_ColorInvalid); // invalid pixels are transparent
            }
            else if(val<lowerBound)
            {
              m_Image.SetPixel(j,cols-i-1,m_ColorBelow); // below the lower bound
            }
            else if(val>upperBound)
            {
              m_Image.SetPixel(j,cols-i-1,m_ColorAbove); // above the upper bound
            }
            else // a valid value
            {
              double relval;
              // calculate a relative value between 0 and 255 from the borders and the scaling style
              if(this.m_ScalingStyle == ScalingStyle.Logarithmic)
              {
                relval = (Math.Log(val) - vmin ) * vscal;
              }
              else // ScalingStyle is linear
              {
                relval = (val - vmin) * vscal;
              }


              r =((int)(Math.Abs(relval)))%256;
              g=((int)(Math.Abs(relval+relval)))%256;
              b=((int)(Math.Abs(255-relval)))%256;
              m_Image.SetPixel(j,cols-i-1,System.Drawing.Color.FromArgb(r,g,b));
            }
          } // for all pixel of a column
        } // for all columns


        m_bCachedDataValid = true; // now the bitmap is valid
      }


      double x_rel_left = gl.XAxis.PhysicalVariantToNormal(xColumn[0]);
      double x_rel_right = gl.XAxis.PhysicalVariantToNormal(xColumn[rows-1]);

      double y_rel_bottom = gl.YAxis.PhysicalVariantToNormal(yColumn[0]);
      double y_rel_top = gl.YAxis.PhysicalVariantToNormal(yColumn[cols-1]);

      double xleft, xright, ytop, ybottom;
      if (gl.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(x_rel_left, y_rel_top), out xleft, out ytop) &&
        gl.CoordinateSystem.LogicalToLayerCoordinates(new Logical3D(x_rel_right, y_rel_bottom), out xright, out ybottom))
      {

        GraphicsState savedGraphicsState = gfrx.Save();

        if(this.m_ClipToLayer)
          gfrx.Clip = gl.CoordinateSystem.GetRegion();

        gfrx.DrawImage(m_Image,(float)xleft,(float)ytop,(float)(xright-xleft),(float)(ybottom-ytop));
      
        gfrx.Restore(savedGraphicsState);
      }
    }

    
    #region IChangedEventSource Members


    protected virtual void OnChanged()
    {
      if (m_Parent is Main.IChildChangedEventSink)
        ((Main.IChildChangedEventSink)m_Parent).EhChildChanged(this, EventArgs.Empty);
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
