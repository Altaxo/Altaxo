#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
using System.ComponentModel;        
using System.Reflection;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;
using Altaxo.Graph.Axes;
using Altaxo.Graph.Axes.Boundaries;


namespace Altaxo.Graph
{
  /// <summary>
  /// XYPlotLayer represents a rectangular area on the graph, which holds plot curves, axes and graphical elements.
  /// </summary>
  [SerializationSurrogate(0,typeof(XYPlotLayer.SerializationSurrogate0))]
  [SerializationVersion(0)]
  public class XYPlotLayer 
    :
    System.Runtime.Serialization.IDeserializationCallback, 
    System.ICloneable, 
    Altaxo.Main.IDocumentNode,
    IPlotArea
  {
   

    

    #region Cached member variables

    /// <summary>
    /// The size and position of the printable area of the entire graph document.
    /// Needed to calculate "relative to page size" layer size values.
    /// </summary>
    protected RectangleF _cachedPrintableGraphBounds;
    /// <summary>
    /// The cached layer position in points (1/72 inch) relative to the upper left corner
    /// of the graph document (upper left corner of the printable area).
    /// </summary>
    protected PointF _cachedLayerPosition = new PointF(0,0);


    /// <summary>
    /// The size of the layer in points (1/72 inch).
    /// </summary>
    /// <remarks>
    /// In case the size is absolute (see <see cref="XYPlotLayerSizeType"/>), this is the size of the layer. Otherwise
    /// it is only the cached value for the size, since the size is calculated then.
    /// </remarks>
    protected SizeF  _cachedLayerSize = new SizeF(0,0);

    protected Matrix _cachedForwardMatrix = new Matrix();  // forward transformation m_ForwardMatrix
    protected Matrix _cachedReverseMatrix = new Matrix(); // inverse transformation m_ForwardMatrix

    #endregion // Cached member variables

    #region Member variables


    /// <summary>
    /// The background style of the layer.
    /// </summary>
    protected BackgroundStyles.IBackgroundStyle _layerBackground;

    /// <summary>If true, the data are clipped to the frame.</summary>
    protected bool _clipDataToFrame=true;
  
    protected TextGraphics _legend = null;

    XYPlotLayerAxisStylesSummaryCollection _axisStyles = new XYPlotLayerAxisStylesSummaryCollection();

    XYPlotLayerAxisPropertiesCollection _axisProperties = new XYPlotLayerAxisPropertiesCollection();

    protected GraphicsObjectCollection _graphObjects = new GraphicsObjectCollection();

    protected Altaxo.Graph.PlotItemCollection _plotItems;

    protected XYPlotLayerPositionAndSize _location = new XYPlotLayerPositionAndSize();


    /// <summary>
    /// The parent layer collection which contains this layer (or null if not member of such collection).
    /// </summary>
    protected object _parentLayerCollection = null;
    //    protected XYPlotLayerCollection _parentLayerCollection=null;

    /// <summary>
    /// The index inside the parent collection of this layer (or 0 if not member of such collection).
    /// </summary>
    protected int _layerNumber = 0;

    /// <summary>
    /// The layer to which this layer is linked to, or null if this layer is not linked.
    /// </summary>
    protected Main.RelDocNodeProxy _linkedLayer;

   


    /// <summary>Number of times this event is disables, or 0 if it is enabled.</summary>
    int _plotAssociationXBoundariesChanged_EventSuspendCount;

    /// <summary>Number of times this event is disables, or 0 if it is enabled.</summary>
    int _plotAssociationYBoundariesChanged_EventSuspendCount;

   
    /// <summary>
    /// Collection of the axis styles for the left, bottom, right, and top axis.
    /// </summary>
    public XYPlotLayerAxisStylesSummaryCollection AxisStyles
    {
      get { return _axisStyles; }
    }

    public XYPlotLayerAxisPropertiesCollection AxisProperties
    {
      get
      {
        return _axisProperties;
      }
    }
 

    #endregion

    #region Event definitions

    /// <summary>Fired when the size of the layer changed.</summary>
    public event System.EventHandler SizeChanged;
  
    /// <summary>Fired when the position of the layer changed.</summary>
    public event System.EventHandler PositionChanged;

    #endregion

    #region Serialization

    /// <summary>Used to serialize the GraphDocument Version 0.</summary>
    public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
    {
      /// <summary>
      /// Serializes XYPlotLayer Version 0.
      /// </summary>
      /// <param name="obj">The XYPlotLayer to serialize.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        throw new ApplicationException("Calling of an outdated serialization routine");

        /*
        XYPlotLayer s = (XYPlotLayer)obj;


      
        // XYPlotLayer style
        info.AddValue("FillLayerArea",s._fillLayerArea);
        info.AddValue("LayerAreaFillBrush",s.m_LayerAreaFillBrush);

        // size, position, rotation and scale
        
        info.AddValue("WidthType",s._location.WidthType);
        info.AddValue("HeightType",s._location.HeightType);
        info.AddValue("Width",s._location.Width);
        info.AddValue("Height",s._location.Height);
        info.AddValue("CachedSize",s._cachedLayerSize);

        info.AddValue("XPositionType",s._location.XPositionType);
        info.AddValue("YPositionType",s._location.YPositionType);
        info.AddValue("XPosition",s._location.XPosition);
        info.AddValue("YPosition",s._location.YPosition);
        info.AddValue("CachedPosition",s._cachedLayerPosition);

        info.AddValue("Rotation",s._location.Angle);
        info.AddValue("Scale",s._location.Scale);

        // axis related

        info.AddValue("XAxis",s._axisProperties.X.Axis);
        info.AddValue("YAxis",s._axisProperties.Y.Axis);
        info.AddValue("LinkXAxis", s._axisProperties.X.IsLinked);
        info.AddValue("LinkYAxis", s._axisProperties.Y.IsLinked);
        info.AddValue("LinkXAxisOrgA", s._axisProperties.X.LinkAxisOrgA);
        info.AddValue("LinkXAxisOrgB", s._axisProperties.X.LinkAxisOrgB);
        info.AddValue("LinkXAxisEndA", s._axisProperties.X.LinkAxisEndA);
        info.AddValue("LinkXAxisEndB", s._axisProperties.X.LinkAxisEndB);
        info.AddValue("LinkYAxisOrgA", s._axisProperties.Y.LinkAxisOrgA);
        info.AddValue("LinkYAxisOrgB", s._axisProperties.Y.LinkAxisOrgB);
        info.AddValue("LinkYAxisEndA", s._axisProperties.Y.LinkAxisEndA);
        info.AddValue("LinkYAxisEndB", s._axisProperties.Y.LinkAxisEndB);

      
        // Styles
        info.AddValue("ShowLeftAxis",s._axisStyles[EdgeType.Left].ShowAxis);
        info.AddValue("ShowBottomAxis", s._axisStyles[EdgeType.Bottom].ShowAxis);
        info.AddValue("ShowRightAxis", s._axisStyles[EdgeType.Right].ShowAxis);
        info.AddValue("ShowTopAxis", s._axisStyles[EdgeType.Top].ShowAxis);

        info.AddValue("LeftAxisStyle", s._axisStyles[EdgeType.Left].AxisStyle);
        info.AddValue("BottomAxisStyle", s._axisStyles[EdgeType.Bottom].AxisStyle);
        info.AddValue("RightAxisStyle", s._axisStyles[EdgeType.Right].AxisStyle);
        info.AddValue("TopAxisStyle", s._axisStyles[EdgeType.Top].AxisStyle);
      
      
        info.AddValue("LeftLabelStyle",s._axisStyles[EdgeType.Left].MajorLabelStyle);
        info.AddValue("BottomLabelStyle", s._axisStyles[EdgeType.Bottom].MajorLabelStyle);
        info.AddValue("RightLabelStyle", s._axisStyles[EdgeType.Right].MajorLabelStyle);
        info.AddValue("TopLabelStyle", s._axisStyles[EdgeType.Top].MajorLabelStyle);
      
    
        // Titles and legend
        info.AddValue("LeftAxisTitle", s._axisStyles[EdgeType.Left].Title);
        info.AddValue("BottomAxisTitle", s._axisStyles[EdgeType.Bottom].Title);
        info.AddValue("RightAxisTitle", s._axisStyles[EdgeType.Right].Title);
        info.AddValue("TopAxisTitle", s._axisStyles[EdgeType.Top].Title);
        info.AddValue("Legend",s._legend);
      
        // XYPlotLayer specific
        info.AddValue("LinkedLayer",s.LinkedLayer);
        info.AddValue("GraphObjects",s._graphObjects);
        info.AddValue("Plots",s._plotItems);

        */

      }

      /// <summary>
      /// Deserializes the XYPlotLayer Version 0.
      /// </summary>
      /// <param name="obj">The empty XYPlotLayer object to deserialize into.</param>
      /// <param name="info">The serialization info.</param>
      /// <param name="context">The streaming context.</param>
      /// <param name="selector">The deserialization surrogate selector.</param>
      /// <returns>The deserialized XYPlotLayer.</returns>
      public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
      {
        XYPlotLayer s = (XYPlotLayer)obj;
      
        // XYPlotLayer style
        bool fillLayerArea = info.GetBoolean("FillLayerArea");
        BrushHolder layerAreaFillBrush = (BrushHolder)info.GetValue("LayerAreaFillBrush",typeof(BrushHolder));

        if (fillLayerArea)
          s._layerBackground = new BackgroundStyles.BackgroundColorStyle(layerAreaFillBrush.Color);



        // size, position, rotation and scale
        
        s._location.WidthType  = (XYPlotLayerSizeType)info.GetValue("WidthType",typeof(XYPlotLayerSizeType));
        s._location.HeightType = (XYPlotLayerSizeType)info.GetValue("HeightType",typeof(XYPlotLayerSizeType));
        s._location.Width  = info.GetDouble("Width");
        s._location.Height = info.GetDouble("Height");
        s._cachedLayerSize   = (SizeF)info.GetValue("CachedSize",typeof(SizeF));

        s._location.XPositionType = (XYPlotLayerPositionType)info.GetValue("XPositionType",typeof(XYPlotLayerPositionType));
        s._location.YPositionType = (XYPlotLayerPositionType)info.GetValue("YPositionType",typeof(XYPlotLayerPositionType));
        s._location.XPosition = info.GetDouble("XPosition");
        s._location.YPosition = info.GetDouble("YPosition");
        s._cachedLayerPosition = (PointF)info.GetValue("CachedPosition",typeof(PointF));

        s._location.Angle  = info.GetSingle("Rotation");
        s._location.Scale = info.GetSingle("Scale");

        // axis related

        s._axisProperties.X.Axis = (Axis)info.GetValue("XAxis",typeof(Axis));
        s._axisProperties.Y.Axis = (Axis)info.GetValue("YAxis",typeof(Axis));
        s.AxisProperties.X.IsLinked = info.GetBoolean("LinkXAxis");
        s.AxisProperties.Y.IsLinked = info.GetBoolean("LinkYAxis");
        s.AxisProperties.X.LinkAxisOrgA = info.GetDouble("LinkXAxisOrgA");
        s.AxisProperties.X.LinkAxisOrgB = info.GetDouble("LinkXAxisOrgB");
        s.AxisProperties.X.LinkAxisEndA = info.GetDouble("LinkXAxisEndA");
        s.AxisProperties.X.LinkAxisEndB = info.GetDouble("LinkXAxisEndB");
        s.AxisProperties.Y.LinkAxisOrgA = info.GetDouble("LinkYAxisOrgA");
        s.AxisProperties.Y.LinkAxisOrgB = info.GetDouble("LinkYAxisOrgB");
        s.AxisProperties.Y.LinkAxisEndA = info.GetDouble("LinkYAxisEndA");
        s.AxisProperties.Y.LinkAxisEndB = info.GetDouble("LinkYAxisEndB");


        // Styles
        s._axisStyles[EdgeType.Left].ShowAxis = info.GetBoolean("ShowLeftAxis");
        s._axisStyles[EdgeType.Bottom].ShowAxis = info.GetBoolean("ShowBottomAxis");
        s._axisStyles[EdgeType.Right].ShowAxis = info.GetBoolean("ShowRightAxis");
        s._axisStyles[EdgeType.Top].ShowAxis = info.GetBoolean("ShowTopAxis");

        s._axisStyles[EdgeType.Left].AxisStyle = (Graph.XYAxisStyle)info.GetValue("LeftAxisStyle", typeof(Graph.XYAxisStyle));
        s._axisStyles[EdgeType.Bottom].AxisStyle = (Graph.XYAxisStyle)info.GetValue("BottomAxisStyle", typeof(Graph.XYAxisStyle));
        s._axisStyles[EdgeType.Right].AxisStyle = (Graph.XYAxisStyle)info.GetValue("RightAxisStyle", typeof(Graph.XYAxisStyle));
        s._axisStyles[EdgeType.Top].AxisStyle = (Graph.XYAxisStyle)info.GetValue("TopAxisStyle", typeof(Graph.XYAxisStyle));
      
      
        s._axisStyles[EdgeType.Left].MajorLabelStyle = (Graph.AbstractXYAxisLabelStyle)info.GetValue("LeftLabelStyle",typeof(Graph.AbstractXYAxisLabelStyle));
        s._axisStyles[EdgeType.Bottom].MajorLabelStyle = (Graph.AbstractXYAxisLabelStyle)info.GetValue("BottomLabelStyle", typeof(Graph.AbstractXYAxisLabelStyle));
        s._axisStyles[EdgeType.Right].MajorLabelStyle = (Graph.AbstractXYAxisLabelStyle)info.GetValue("RightLabelStyle", typeof(Graph.AbstractXYAxisLabelStyle));
        s._axisStyles[EdgeType.Top].MajorLabelStyle = (Graph.AbstractXYAxisLabelStyle)info.GetValue("TopLabelStyle", typeof(Graph.AbstractXYAxisLabelStyle));
      
      
        // Titles and legend
        s._axisStyles[EdgeType.Left].Title = (Graph.TextGraphics)info.GetValue("LeftAxisTitle", typeof(Graph.TextGraphics));
        s._axisStyles[EdgeType.Bottom].Title = (Graph.TextGraphics)info.GetValue("BottomAxisTitle", typeof(Graph.TextGraphics));
        s._axisStyles[EdgeType.Right].Title = (Graph.TextGraphics)info.GetValue("RightAxisTitle", typeof(Graph.TextGraphics));
        s._axisStyles[EdgeType.Top].Title = (Graph.TextGraphics)info.GetValue("TopAxisTitle", typeof(Graph.TextGraphics));
        s._legend = (Graph.TextGraphics)info.GetValue("Legend",typeof(Graph.TextGraphics));
      
        // XYPlotLayer specific
        s._linkedLayer.SetDocNode((XYPlotLayer)info.GetValue("LinkedLayer", typeof(XYPlotLayer)), s);

        s._graphObjects = (Graph.GraphicsObjectCollection)info.GetValue("GraphObjects",typeof(Graph.GraphicsObjectCollection));

        s._plotItems = (Altaxo.Graph.PlotItemCollection)info.GetValue("Plots",typeof(Altaxo.Graph.PlotItemCollection));

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotLayer),0)]
      [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotLayer), 1)] // by accident this was never different from version 0
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new ApplicationException("Calling of an outdated serialization routine");
        /*
        XYPlotLayer s = (XYPlotLayer)obj;
        // XYPlotLayer style
        info.AddValue("FillLayerArea",s._fillLayerArea);
        info.AddValue("LayerAreaFillBrush",s.m_LayerAreaFillBrush);

        // size, position, rotation and scale
        
        info.AddValue("WidthType",s._location.WidthType);
        info.AddValue("HeightType",s._location.HeightType);
        info.AddValue("Width",s._location.Width);
        info.AddValue("Height",s._location.Height);
        info.AddValue("CachedSize",s._cachedLayerSize);

        info.AddValue("XPositionType",s._location.XPositionType);
        info.AddValue("YPositionType",s._location.YPositionType);
        info.AddValue("XPosition",s._location.XPosition);
        info.AddValue("YPosition",s._location.YPosition);
        info.AddValue("CachedPosition",s._cachedLayerPosition);

        info.AddValue("Rotation",s._location.Angle);
        info.AddValue("Scale",s._location.Scale);

        // axis related

        info.AddValue("XAxis",s._axisProperties.X.Axis);
        info.AddValue("YAxis",s._axisProperties.Y.Axis);
        info.AddValue("LinkXAxis",s._axisProperties.X.IsLinked);
        info.AddValue("LinkYAxis", s._axisProperties.Y.IsLinked);
        info.AddValue("LinkXAxisOrgA", s._axisProperties.X.LinkAxisOrgA);
        info.AddValue("LinkXAxisOrgB", s._axisProperties.X.LinkAxisOrgB);
        info.AddValue("LinkXAxisEndA", s._axisProperties.X.LinkAxisEndA);
        info.AddValue("LinkXAxisEndB", s._axisProperties.X.LinkAxisEndB);
        info.AddValue("LinkYAxisOrgA", s._axisProperties.Y.LinkAxisOrgA);
        info.AddValue("LinkYAxisOrgB", s._axisProperties.Y.LinkAxisOrgB);
        info.AddValue("LinkYAxisEndA", s._axisProperties.Y.LinkAxisEndA);
        info.AddValue("LinkYAxisEndB", s._axisProperties.Y.LinkAxisEndB);

      
        // Styles
        info.AddValue("ShowLeftAxis", s._axisStyles[EdgeType.Left].ShowAxis);
        info.AddValue("ShowBottomAxis", s._axisStyles[EdgeType.Bottom].ShowAxis);
        info.AddValue("ShowRightAxis", s._axisStyles[EdgeType.Right].ShowAxis);
        info.AddValue("ShowTopAxis", s._axisStyles[EdgeType.Top].ShowAxis);

        info.AddValue("LeftAxisStyle", s._axisStyles[EdgeType.Left].AxisStyle);
        info.AddValue("BottomAxisStyle", s._axisStyles[EdgeType.Bottom].AxisStyle);
        info.AddValue("RightAxisStyle", s._axisStyles[EdgeType.Right].AxisStyle);
        info.AddValue("TopAxisStyle", s._axisStyles[EdgeType.Top].AxisStyle);
      
      
        info.AddValue("LeftLabelStyle",s._axisStyles[EdgeType.Left].MajorLabelStyle);
        info.AddValue("BottomLabelStyle", s._axisStyles[EdgeType.Bottom].MajorLabelStyle);
        info.AddValue("RightLabelStyle", s._axisStyles[EdgeType.Right].MajorLabelStyle);
        info.AddValue("TopLabelStyle", s._axisStyles[EdgeType.Top].MajorLabelStyle);
      
    
        // Titles and legend
        info.AddValue("LeftAxisTitle", s._axisStyles[EdgeType.Left].Title);
        info.AddValue("BottomAxisTitle", s._axisStyles[EdgeType.Bottom].Title);
        info.AddValue("RightAxisTitle", s._axisStyles[EdgeType.Right].Title);
        info.AddValue("TopAxisTitle", s._axisStyles[EdgeType.Top].Title);
        info.AddValue("Legend",s._legend);
      
        // XYPlotLayer specific
        info.AddValue("LinkedLayer", null!=s._linkedLayer ? Main.DocumentPath.GetRelativePathFromTo(s,s._linkedLayer) : null);
      
        info.AddValue("GraphicsObjectCollection",s._graphObjects);
        info.AddValue("Plots",s._plotItems);
        */

      }

      protected XYPlotLayer _Layer;
      protected Main.DocumentPath _LinkedLayerPath;

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        XYPlotLayer s = SDeserialize(o, info, parent);


        s.CalculateMatrix();
        s.CreateEventLinks();

        return s;
      }


      protected virtual XYPlotLayer SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        XYPlotLayer s = null!=o ? (XYPlotLayer)o : new XYPlotLayer();

        bool fillLayerArea = info.GetBoolean("FillLayerArea");
        BrushHolder layerAreaFillBrush = (BrushHolder)info.GetValue("LayerAreaFillBrush", typeof(BrushHolder));

        if (fillLayerArea)
          s._layerBackground = new BackgroundStyles.BackgroundColorStyle(layerAreaFillBrush.Color);




        // size, position, rotation and scale
        
        s._location.WidthType  = (XYPlotLayerSizeType)info.GetValue("WidthType",typeof(XYPlotLayerSizeType));
        s._location.HeightType = (XYPlotLayerSizeType)info.GetValue("HeightType",typeof(XYPlotLayerSizeType));
        s._location.Width  = info.GetDouble("Width");
        s._location.Height = info.GetDouble("Height");
        s._cachedLayerSize   = (SizeF)info.GetValue("CachedSize",typeof(SizeF));

        s._location.XPositionType = (XYPlotLayerPositionType)info.GetValue("XPositionType",typeof(XYPlotLayerPositionType));
        s._location.YPositionType = (XYPlotLayerPositionType)info.GetValue("YPositionType",typeof(XYPlotLayerPositionType));
        s._location.XPosition = info.GetDouble("XPosition");
        s._location.YPosition = info.GetDouble("YPosition");
        s._cachedLayerPosition = (PointF)info.GetValue("CachedPosition",typeof(PointF));

        s._location.Angle  = info.GetSingle("Rotation");
        s._location.Scale = info.GetSingle("Scale");

        // axis related

        s._axisProperties.X.Axis = (Axis)info.GetValue("XAxis",typeof(Axis));
        s._axisProperties.Y.Axis = (Axis)info.GetValue("YAxis",typeof(Axis));
        s._axisProperties.X.IsLinked = info.GetBoolean("LinkXAxis");
        s._axisProperties.Y.IsLinked = info.GetBoolean("LinkYAxis");
        s._axisProperties.X.LinkAxisOrgA = info.GetDouble("LinkXAxisOrgA");
        s._axisProperties.X.LinkAxisOrgB = info.GetDouble("LinkXAxisOrgB");
        s._axisProperties.X.LinkAxisEndA = info.GetDouble("LinkXAxisEndA");
        s._axisProperties.X.LinkAxisEndB = info.GetDouble("LinkXAxisEndB");
        s._axisProperties.Y.LinkAxisOrgA = info.GetDouble("LinkYAxisOrgA");
        s._axisProperties.Y.LinkAxisOrgB = info.GetDouble("LinkYAxisOrgB");
        s._axisProperties.Y.LinkAxisEndA = info.GetDouble("LinkYAxisEndA");
        s._axisProperties.Y.LinkAxisEndB = info.GetDouble("LinkYAxisEndB");


        // Styles
        s._axisStyles[EdgeType.Left].ShowAxis = info.GetBoolean("ShowLeftAxis");
        s._axisStyles[EdgeType.Bottom].ShowAxis = info.GetBoolean("ShowBottomAxis");
        s._axisStyles[EdgeType.Right].ShowAxis = info.GetBoolean("ShowRightAxis");
        s._axisStyles[EdgeType.Top].ShowAxis = info.GetBoolean("ShowTopAxis");

        s._axisStyles[EdgeType.Left].AxisStyle = (Graph.XYAxisStyle)info.GetValue("LeftAxisStyle",typeof(Graph.XYAxisStyle));
        s._axisStyles[EdgeType.Bottom].AxisStyle = (Graph.XYAxisStyle)info.GetValue("BottomAxisStyle", typeof(Graph.XYAxisStyle));
        s._axisStyles[EdgeType.Right].AxisStyle = (Graph.XYAxisStyle)info.GetValue("RightAxisStyle", typeof(Graph.XYAxisStyle));
        s._axisStyles[EdgeType.Top].AxisStyle = (Graph.XYAxisStyle)info.GetValue("TopAxisStyle", typeof(Graph.XYAxisStyle));


        s._axisStyles[EdgeType.Left].MajorLabelStyle = (Graph.AbstractXYAxisLabelStyle)info.GetValue("LeftLabelStyle", typeof(Graph.AbstractXYAxisLabelStyle));
        s._axisStyles[EdgeType.Bottom].MajorLabelStyle = (Graph.AbstractXYAxisLabelStyle)info.GetValue("BottomLabelStyle", typeof(Graph.AbstractXYAxisLabelStyle));
        s._axisStyles[EdgeType.Right].MajorLabelStyle = (Graph.AbstractXYAxisLabelStyle)info.GetValue("RightLabelStyle", typeof(Graph.AbstractXYAxisLabelStyle));
        s._axisStyles[EdgeType.Top].MajorLabelStyle = (Graph.AbstractXYAxisLabelStyle)info.GetValue("TopLabelStyle", typeof(Graph.AbstractXYAxisLabelStyle));
      
      
        // Titles and legend
        s._axisStyles[EdgeType.Left].Title = (Graph.TextGraphics)info.GetValue("LeftAxisTitle", typeof(Graph.TextGraphics));
        s._axisStyles[EdgeType.Bottom].Title = (Graph.TextGraphics)info.GetValue("BottomAxisTitle", typeof(Graph.TextGraphics));
        s._axisStyles[EdgeType.Right].Title = (Graph.TextGraphics)info.GetValue("RightAxisTitle", typeof(Graph.TextGraphics));
        s._axisStyles[EdgeType.Top].Title = (Graph.TextGraphics)info.GetValue("TopAxisTitle", typeof(Graph.TextGraphics));
        s._legend = (Graph.TextGraphics)info.GetValue("Legend",typeof(Graph.TextGraphics));
      
        // XYPlotLayer specific
        Main.DocumentPath linkedLayer = (Main.DocumentPath)info.GetValue("LinkedLayer",typeof(XYPlotLayer));
        if(linkedLayer!=null)
        {
          XmlSerializationSurrogate0 surr = new XmlSerializationSurrogate0();
          surr._Layer = s;
          surr._LinkedLayerPath = linkedLayer;
          info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(surr.EhDeserializationFinished);
        }

        s._graphObjects = (Graph.GraphicsObjectCollection)info.GetValue("GraphObjects",typeof(Graph.GraphicsObjectCollection));

        s._plotItems = (Altaxo.Graph.PlotItemCollection)info.GetValue("Plots",typeof(Altaxo.Graph.PlotItemCollection));
    

        return s;
      }

      public void EhDeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object documentRoot)
      {
        bool bAllResolved = true;

        object linkedLayer = Main.DocumentPath.GetObject(this._LinkedLayerPath,this._Layer,documentRoot);

        if(linkedLayer is XYPlotLayer)
        {
          this._Layer.LinkedLayer = (XYPlotLayer)linkedLayer;
          this._LinkedLayerPath=null;
        }
        else
        {
          bAllResolved = false;
        }

        if(bAllResolved)
          info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(this.EhDeserializationFinished);

      }
    }

    
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotLayer),2)]
      public class XmlSerializationSurrogate2 : XmlSerializationSurrogate0
    {
      public override void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        base.Serialize(obj, info);

        XYPlotLayer s = (XYPlotLayer)obj;
        // XYPlotLayer style
        info.AddValue("ClipDataToFrame",s._clipDataToFrame);
      }

      protected override XYPlotLayer SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        XYPlotLayer s = base.SDeserialize(o,info,parent);

        s._clipDataToFrame = info.GetBoolean("ClipDataToFrame");

        return s;
      }
    }


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotLayer), 3)]
    public class XmlSerializationSurrogate3 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        XYPlotLayer s = (XYPlotLayer)obj;

        // Background
        info.AddValue("Background",s._layerBackground);

        // size, position, rotation and scale
        info.AddValue("LocationAndSize", s._location);
        info.AddValue("CachedSize", s._cachedLayerSize);
        info.AddValue("CachedPosition", s._cachedLayerPosition);

        // LayerProperties
        info.AddValue("ClipDataToFrame",s._clipDataToFrame);

        // axis related
        info.AddValue("AxisProperties", s._axisProperties);
 
        // Styles
        info.AddValue("AxisStyles", s._axisStyles);

        // Legends
        info.CreateArray("Legends",1);
        info.AddValue("e", s._legend);
        info.CommitArray();

        // XYPlotLayer specific
        info.CreateArray("LinkedLayers",1);
        info.AddValue("e", s._linkedLayer);
        info.CommitArray();

        info.AddValue("GraphicGlyphs", s._graphObjects);

        info.AddValue("Plots", s._plotItems);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        XYPlotLayer s = SDeserialize(o, info, parent);
        s.CalculateMatrix();
        s.CreateEventLinks();
        return s;
      }

      protected virtual XYPlotLayer SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {

        XYPlotLayer s = (o == null ? new XYPlotLayer() : (XYPlotLayer)o);
        int count;

         // Background
        s._layerBackground = (BackgroundStyles.IBackgroundStyle)info.GetValue("Background",s);

        // size, position, rotation and scale
        s._location = (XYPlotLayerPositionAndSize)info.GetValue("LocationAndSize", s);
        s._cachedLayerSize   = (SizeF)info.GetValue("CachedSize",typeof(SizeF));
        s._cachedLayerPosition = (PointF)info.GetValue("CachedPosition",typeof(PointF));


        // LayerProperties
         s._clipDataToFrame = info.GetBoolean("ClipDataToFrame");

        // axis related
        s._axisProperties = (XYPlotLayerAxisPropertiesCollection)info.GetValue("AxisProperties", s);
 
        // Styles
        s._axisStyles = (XYPlotLayerAxisStylesSummaryCollection)info.GetValue("AxisStyles", s);

        // Legends
        count = info.OpenArray("Legends");
        s._legend = (TextGraphics)info.GetValue("e", s._legend);
        info.CloseArray(count);

        // XYPlotLayer specific
        count = info.OpenArray("LinkedLayers");
        s._linkedLayer = (Main.RelDocNodeProxy)info.GetValue("e", s);
        info.CloseArray(count);

        s._graphObjects = (GraphicsObjectCollection)info.GetValue("GraphicGlyphs", s);

        s._plotItems = (PlotItemCollection)info.GetValue("Plots", s);

        return s;
      }
    }

    
    /// <summary>
    /// Finale measures after deserialization.
    /// </summary>
    /// <param name="obj">Not used.</param>
    public void OnDeserialization(object obj)
    {
      _cachedForwardMatrix = new Matrix();
      _cachedReverseMatrix = new Matrix();
      CalculateMatrix();

      CreateEventLinks();
    }
    #endregion

    #region Constructors

    /// <summary>
    /// The copy constructor.
    /// </summary>
    /// <param name="from"></param>
    public XYPlotLayer(XYPlotLayer from)
    {
      // XYPlotLayer style
      this._layerBackground = from._layerBackground == null ? null : (BackgroundStyles.IBackgroundStyle)from._layerBackground.Clone();

      // size, position, rotation and scale
      this._location = from._location.Clone();
      this._cachedLayerSize   = from._cachedLayerSize;
      this._cachedLayerPosition = from._cachedLayerPosition;

      this._LogicalToAreaConverter = new LogicalToAreaConverter(this);
      this._AreaToLogicalConverter = new AreaToLogicalConverter(this);




      // axis related

      this._axisProperties = (XYPlotLayerAxisPropertiesCollection)from._axisProperties.Clone();

      // Styles

      this._axisStyles = (XYPlotLayerAxisStylesSummaryCollection)from._axisStyles.Clone();

      this._legend = null==from._legend ? null : (Graph.TextGraphics)from._legend.Clone();
      
      // XYPlotLayer specific
      this._linkedLayer = from._linkedLayer; // do not clone here, parent collection's duty to fix this!
      
      this._graphObjects = null==from._graphObjects ? null : new GraphicsObjectCollection(from._graphObjects);

      this._plotItems = null==from._plotItems ? null : new Altaxo.Graph.PlotItemCollection(this,from._plotItems);

      // special way neccessary to handle plot groups
      //this.m_PlotGroups = null==from.m_PlotGroups ? null : from.m_PlotGroups.Clone(this._plotItems,from._plotItems);

      _cachedForwardMatrix = new Matrix();
      _cachedReverseMatrix = new Matrix();
      CalculateMatrix();

      CreateEventLinks();
    }


    public virtual object Clone()
    {
      return new XYPlotLayer(this);
    }

    /// <summary>
    /// Creates a layer with standard position and size using the size of the printable area.
    /// </summary>
    /// <param name="prtSize">Size of the printable area in points (1/72 inch).</param>
    public XYPlotLayer(SizeF prtSize)
      : this(new PointF(prtSize.Width*0.14f,prtSize.Height*0.14f),new SizeF(prtSize.Width*0.76f,prtSize.Height*0.7f))
    {
    }

    /// <summary>
    /// Constructor for deserialization purposes only.
    /// </summary>
    protected XYPlotLayer()
    {
      this._LogicalToAreaConverter = new LogicalToAreaConverter(this);
      this._AreaToLogicalConverter = new AreaToLogicalConverter(this);
    }
    /// <summary>
    /// Creates a layer with position <paramref name="position"/> and size <paramref name="size"/>.
    /// </summary>
    /// <param name="position">The position of the layer on the printable area in points (1/72 inch).</param>
    /// <param name="size">The size of the layer in points (1/72 inch).</param>
    public XYPlotLayer(PointF position, SizeF size)
    {
      this.Size     = size;
      this.Position = position;
      this._LogicalToAreaConverter = new LogicalToAreaConverter(this);
      this._AreaToLogicalConverter = new AreaToLogicalConverter(this);

      CalculateMatrix();

      _plotItems = new Altaxo.Graph.PlotItemCollection(this);
    
    
      LeftAxisTitleString = "Y axis";
      BottomAxisTitleString = "X axis";

      CreateEventLinks();
    
    }

  
  
    #endregion

    #region XYPlotLayer properties and methods


    void CreateEventLinks()
    {
      
      if(null!=_axisStyles) _axisStyles.Changed += new EventHandler(this.OnChildChangedEventHandler);

      if (null != _axisProperties)
      {
        _axisProperties.Changed += new EventHandler(OnChildChangedEventHandler);
        _axisProperties.X.AxisInstanceChanged += new EventHandler(EhXAxisInstanceChanged);
        _axisProperties.Y.AxisInstanceChanged += new EventHandler(EhYAxisInstanceChanged);
      }

      if(null!=_legend) _legend.Changed += new EventHandler(this.OnChildChangedEventHandler);

      if(null!=_linkedLayer)
        _linkedLayer.Changed += new EventHandler(this.EhLinkedLayerInstanceChanged);
    
      if(null!=_graphObjects) _graphObjects.Changed += new EventHandler(this.OnChildChangedEventHandler);


      if(null!=_plotItems)
      {
        _plotItems.SetParentLayer(this,true); // sets the parent layer, but suppresses the events following this.
        _plotItems.Changed += new EventHandler(this.OnChildChangedEventHandler);
      }
    }

    /// <summary>
    /// The layer number.
    /// </summary>
    /// <value>The layer number, i.e. the position of the layer in the layer collection.</value>
    public int Number
    {
      get { return this._layerNumber; } 
    }

    
    public XYPlotLayerCollection ParentLayerList
    {
      get { return _parentLayerCollection as XYPlotLayerCollection; }
    }

    public GraphicsObjectCollection GraphObjects
    {
      get { return _graphObjects; }
    }

    public void Remove(GraphicsObject go)
    {
      if (_axisStyles.Remove(go))
        return;
      
      else if(object.ReferenceEquals(go,this._legend))
        _legend=null;
      else if(_graphObjects.Contains(go))
        _graphObjects.Remove(go);

    }

    /// <summary>
    /// Get / sets the layer this layer is linked to.
    /// </summary>
    /// <value>The layer this layer is linked to, or null if not linked.</value>
    public XYPlotLayer LinkedLayer
    {
      get 
      {

        return _linkedLayer==null ? null : (XYPlotLayer)_linkedLayer.DocumentObject; 
      }
      set
      {
        
        // ignore the value if it would create a circular dependency
        if(IsLayerDependentOnMe(value))
          return;


        XYPlotLayer oldValue = this.LinkedLayer;
        if (_linkedLayer == null)
          _linkedLayer = new Main.RelDocNodeProxy();

        _linkedLayer.SetDocNode(value,this);

        if(!ReferenceEquals(oldValue,value))
        {
          // close the event handlers to the old layer
          if(null!=oldValue)
          {
            oldValue.SizeChanged -= new System.EventHandler(EhLinkedLayerSizeChanged);
            oldValue.PositionChanged -= new System.EventHandler(EhLinkedLayerPositionChanged);
            oldValue.AxisProperties.AxesChanged -= new System.EventHandler(EhLinkedLayerAxesChanged);
          }

          // link the events to the new layer
          if(null!=_linkedLayer)
          {
            value.SizeChanged     += new System.EventHandler(EhLinkedLayerSizeChanged);
            value.PositionChanged += new System.EventHandler(EhLinkedLayerPositionChanged);
            value.AxisProperties.AxesChanged     += new System.EventHandler(EhLinkedLayerAxesChanged);
          }

        }
      }
    }

    /// <summary>
    /// Is this layer linked to another layer?
    /// </summary>
    /// <value>True if this layer is linked to another layer. See <see cref="LinkedLayer"/> to
    /// find out to which layer this layer is linked to.</value>
    public bool IsLinked
    {
      get { return null!=_linkedLayer.DocumentObject; }
    }

    /// <summary>
    /// Checks if the provided layer or a linked layer of it is dependent on this layer.
    /// </summary>
    /// <param name="layer">The layer to check.</param>
    /// <returns>True if the provided layer or one of its linked layers is dependend on this layer.</returns>
    public bool IsLayerDependentOnMe(XYPlotLayer layer)
    {
      while(null!=layer)
      {
        if(XYPlotLayer.ReferenceEquals(layer,this))
        {
          // this means a circular dependency, so return true
          return true;
        }
        layer = layer.LinkedLayer;
      }
      return false; // no dependency detected
    }

    /// <summary>
    ///  Only intended to use by XYPlotLayerCollection! Sets the parent layer collection for this layer.
    /// </summary>
    /// <param name="lc">The layer collection this layer belongs to.</param>
    /// <param name="number">The layer number assigned to this layer.</param>
    protected internal void SetParentAndNumber(XYPlotLayerCollection lc, int number)
    {
      _parentLayerCollection = lc;
      _layerNumber = number;
      
      if(_parentLayerCollection==null)
        _linkedLayer=null;
    }


    public Altaxo.Graph.PlotItemCollection PlotItems
    {
      get 
      {
        return _plotItems; 
      }
    }

    /// <summary>
    /// Creates a new legend, removing the old one.
    /// </summary>
    /// <remarks>The position of the old legend is <b>only</b> used for the new legend if the old legend's position is
    /// inside the layer. This prevents a "stealth" legend in case it is not visible by accident.
    /// </remarks>
    public void CreateNewLayerLegend()
    {
      // remove the legend if there are no plot curves on the layer
      if(PlotItems.Count==0)
      {
        _legend=null;
        OnInvalidate();
        return;
      }

      TextGraphics tgo;

      if(_legend!=null)
        tgo = new TextGraphics(_legend);
      else
        tgo = new TextGraphics();


      System.Text.StringBuilder strg = new System.Text.StringBuilder();
      for(int i=0;i<this.PlotItems.Count;i++)
      {
        strg.AppendFormat("{0}\\L({1}) \\%({2})",(i==0?"":"\r\n"), i,i);
      }
      tgo.Text = strg.ToString();

      // if the position of the old legend is outside, use a new position
      if(null==_legend || _legend.Position.X<0 || _legend.Position.Y<0 || 
        _legend.Position.X>this.Size.Width || _legend.Position.Y>this.Size.Height)
        tgo.SetPosition(new PointF(0.1f*this.Size.Width,0.1f*this.Size.Height));
      else
        tgo.SetPosition(_legend.Position);

      _legend = tgo;

      OnInvalidate();
    }

    

    #endregion // XYPlotLayer Properties and Methods

    #region Position and Size

    const double _xDefPositionLandscape=0.14;
    const double _yDefPositionLandscape=0.14;
    const double _xDefSizeLandscape=0.76;
    const double _yDefSizeLandscape=0.7;

    const double _xDefPositionPortrait=0.14;
    const double _yDefPositionPortrait=0.14;
    const double _xDefSizePortrait=0.7;
    const double _yDefSizePortrait=0.76;


    /// <summary>
    /// Set this layer to the default size and position.
    /// </summary>
    /// <param name="prtSize">The size of the printable area of the page.</param>
    public void SizeToDefault(SizeF prtSize)
    {
      if(prtSize.Width>prtSize.Height)
      {
        this.Size = new SizeF(prtSize.Width*0.76f,prtSize.Height*0.7f);
        this.Position = new PointF(prtSize.Width*0.14f,prtSize.Height*0.14f);
      }
      else // Portrait
      {
        this.Size = new SizeF(prtSize.Width*0.76f,prtSize.Height*0.7f);
        this.Position = new PointF(prtSize.Width*0.14f,prtSize.Height*0.14f);
      }
      this.CalculateMatrix();
    }

    /// <summary>
    /// The boundaries of the printable area of the page in points (1/72 inch).
    /// </summary>
    public RectangleF PrintableGraphBounds
    {
      get { return _cachedPrintableGraphBounds; }
    }
    public SizeF PrintableGraphSize
    {
      get { return _cachedPrintableGraphBounds.Size; }
    }
    public void SetPrintableGraphBounds(RectangleF val, bool bRescale)
    {
      RectangleF oldBounds = _cachedPrintableGraphBounds;
      RectangleF newBounds = val;
      _cachedPrintableGraphBounds=val;

      

      if(_cachedPrintableGraphBounds!=oldBounds && bRescale)
      {
        SizeF      oldLayerSize     = this._cachedLayerSize;


        double oldxdefsize = oldBounds.Width*(oldBounds.Width>oldBounds.Height ? _xDefSizeLandscape : _xDefSizePortrait);
        double newxdefsize = newBounds.Width*(newBounds.Width>newBounds.Height ? _xDefSizeLandscape : _xDefSizePortrait);
        double oldydefsize = oldBounds.Height*(oldBounds.Width>oldBounds.Height ? _yDefSizeLandscape : _yDefSizePortrait);
        double newydefsize = newBounds.Height*(newBounds.Width>newBounds.Height ? _yDefSizeLandscape : _yDefSizePortrait);


        double oldxdeforg = oldBounds.Width*(oldBounds.Width>oldBounds.Height ? _xDefPositionLandscape : _xDefPositionPortrait);
        double newxdeforg = newBounds.Width*(newBounds.Width>newBounds.Height ? _xDefPositionLandscape : _xDefPositionPortrait);
        double oldydeforg = oldBounds.Height*(oldBounds.Width>oldBounds.Height ? _yDefPositionLandscape : _yDefPositionPortrait);
        double newydeforg = newBounds.Height*(newBounds.Width>newBounds.Height ? _yDefPositionLandscape : _yDefPositionPortrait);

        double xscale = newxdefsize/oldxdefsize;
        double yscale = newydefsize/oldydefsize;

        double xoffs = newxdeforg - oldxdeforg*xscale;
        double yoffs = newydeforg - oldydeforg*yscale;
        
        if(this._location.XPositionType == XYPlotLayerPositionType.AbsoluteValue)
          this._location.XPosition = xoffs + this._location.XPosition*xscale;
        

        if(this._location.WidthType==XYPlotLayerSizeType.AbsoluteValue)
          this._location.Width *= xscale;

        if(this._location.YPositionType == XYPlotLayerPositionType.AbsoluteValue)
          this._location.YPosition = yoffs + this._location.YPosition*yscale;

        if(this._location.HeightType == XYPlotLayerSizeType.AbsoluteValue)
          this._location.Height *= yscale;

        CalculateMatrix();
        this.CalculateCachedSize();
        this.CalculateCachedPosition();

        // scale the position of the inner items according to the ratio of the new size to the old size
        // note: only the size is important here, since all inner items are relative to the layer origin
        SizeF     newLayerSize     = this._cachedLayerSize;
        xscale = newLayerSize.Width/oldLayerSize.Width;
        yscale = newLayerSize.Height/oldLayerSize.Height;

        RescaleInnerItemPositions(xscale,yscale);
      }
    }

    /// <summary>
    /// Recalculates the positions of inner items in case the layer has changed its size.
    /// </summary>
    /// <param name="xscale">The ratio the layer has changed its size in horizontal direction.</param>
    /// <param name="yscale">The ratio the layer has changed its size in vertical direction.</param>
    public void RescaleInnerItemPositions(double xscale, double yscale)
    {
      GraphicsObject.ScalePosition(this._axisStyles[EdgeType.Left].Title,xscale,yscale);
      GraphicsObject.ScalePosition(this._axisStyles[EdgeType.Bottom].Title, xscale, yscale);
      GraphicsObject.ScalePosition(this._axisStyles[EdgeType.Right].Title, xscale, yscale);
      GraphicsObject.ScalePosition(this._axisStyles[EdgeType.Top].Title, xscale, yscale);
      GraphicsObject.ScalePosition(this._legend,xscale,yscale);
      this._graphObjects.ScalePosition(xscale,yscale);
    }

    public PointF Position
    {
      get { return this._cachedLayerPosition; }
      set
      {
        SetPosition(value.X,XYPlotLayerPositionType.AbsoluteValue,value.Y,XYPlotLayerPositionType.AbsoluteValue);
      }
    }

    public SizeF Size
    {
      get { return this._cachedLayerSize; }
      set
      {
        SetSize(value.Width,XYPlotLayerSizeType.AbsoluteValue, value.Height,XYPlotLayerSizeType.AbsoluteValue);
      }
    }

    public double Rotation
    {
      get { return this._location.Angle; }
      set
      {
        this._location.Angle = value;
        this.CalculateMatrix();
        this.OnInvalidate();
      }
    }

    public double Scale
    {
      get { return this._location.Scale; }
      set
      {
        this._location.Scale = value;
        this.CalculateMatrix();
        this.OnInvalidate();
      }
    }

    protected void CalculateMatrix()
    {
      _cachedForwardMatrix.Reset();
      _cachedForwardMatrix.Translate(_cachedLayerPosition.X,_cachedLayerPosition.Y);
      _cachedForwardMatrix.Scale((float)_location.Scale,(float)_location.Scale);
      _cachedForwardMatrix.Rotate((float)_location.Angle);
      _cachedReverseMatrix=_cachedForwardMatrix.Clone();
      _cachedReverseMatrix.Invert();
    }


    public PointF GraphToLayerCoordinates(PointF pagecoordinates)
    {
      PointF[] pf = { pagecoordinates };
      _cachedReverseMatrix.TransformPoints(pf);
      return pf[0];
    }


    /// <summary>
    /// This switches the graphics context from printable area coordinates to layer coordinates.
    /// </summary>
    /// <param name="g">The graphics state to change.</param>
    public void GraphToLayerCoordinates(Graphics g)
    {
      g.MultiplyTransform(_cachedForwardMatrix);
    }

    /// <summary>
    /// Converts X,Y differences in page units to X,Y differences in layer units
    /// </summary>
    /// <param name="pagediff">X,Y coordinate differences in graph units</param>
    /// <returns>the convertes X,Y coordinate differences in layer units</returns>
    public PointF GraphToLayerDifferences(PointF pagediff)
    {
      // not very intelligent, maybe there is a simpler way to transform without
      // taking the translation into account
      PointF[] pf = { new PointF(pagediff.X + this.Position.X, pagediff.Y + this.Position.Y) };
      _cachedReverseMatrix.TransformPoints(pf);
      return pf[0];
    }



    /// <summary>
    /// Transforms a graphics path from layer coordinates to graph (page) coordinates
    /// </summary>
    /// <param name="gp">the graphics path to convert</param>
    /// <returns>graphics path now in graph coordinates</returns>
    public GraphicsPath LayerToGraphCoordinates(GraphicsPath gp)
    {
      gp.Transform(_cachedForwardMatrix);
      return gp;
    }

    
    /// <summary>
    /// Transforms a <see cref="PointF" /> from layer coordinates to graph (=printable area) coordinates
    /// </summary>
    /// <param name="layerCoordinates">The layer coordinates to convert.</param>
    /// <returns>graphics path now in graph coordinates</returns>
    public PointF LayerToGraphCoordinates(PointF layerCoordinates)
    {
      PointF[]result = new PointF[]{layerCoordinates}; 
      _cachedForwardMatrix.TransformPoints(result);
      return result[0];
    }



    public void SetPosition(double x, XYPlotLayerPositionType xpostype, double y, XYPlotLayerPositionType ypostype)
    {
      this._location.XPosition = x;
      this._location.XPositionType = xpostype;
      this._location.YPosition = y;
      this._location.YPositionType = ypostype;

      CalculateCachedPosition();
    }

    /// <summary>
    /// Calculates from the x position value, which can be absolute or relative, the
    /// x position in points.
    /// </summary>
    /// <param name="x">The horizontal position value of type xpostype.</param>
    /// <param name="xpostype">The type of the horizontal position value, see <see cref="XYPlotLayerPositionType"/>.</param>
    /// <returns>Calculated absolute position of the layer in units of points (1/72 inch).</returns>
    /// <remarks>The function does not change the member variables of the layer and can therefore used
    /// for position calculations without changing the layer. The function is not static because it has to use either the parent
    /// graph or the linked layer for the calculations.</remarks>
    public double XPositionToPointUnits(double x, XYPlotLayerPositionType xpostype)
    {
      switch(xpostype)
      {
        case XYPlotLayerPositionType.AbsoluteValue:
          break;
        case XYPlotLayerPositionType.RelativeToGraphDocument:
          x = x*PrintableGraphSize.Width;
          break;
        case XYPlotLayerPositionType.RelativeThisNearToLinkedLayerNear:
          if(LinkedLayer!=null)
            x = LinkedLayer.Position.X + x*LinkedLayer.Size.Width;
          break;
        case XYPlotLayerPositionType.RelativeThisNearToLinkedLayerFar:
          if(LinkedLayer!=null)
            x = LinkedLayer.Position.X + (1+x)*LinkedLayer.Size.Width;
          break;
        case XYPlotLayerPositionType.RelativeThisFarToLinkedLayerNear:
          if(LinkedLayer!=null)
            x = LinkedLayer.Position.X - this.Size.Width + x*LinkedLayer.Size.Width;
          break;
        case XYPlotLayerPositionType.RelativeThisFarToLinkedLayerFar:
          if(LinkedLayer!=null)
            x = LinkedLayer.Position.X - this.Size.Width + (1+x)*LinkedLayer.Size.Width;
          break;
      }
      return x;
    }

    /// <summary>
    /// Calculates from the y position value, which can be absolute or relative, the
    ///  y position in points.
    /// </summary>
    /// <param name="y">The vertical position value of type xpostype.</param>
    /// <param name="ypostype">The type of the vertical position value, see <see cref="XYPlotLayerPositionType"/>.</param>
    /// <returns>Calculated absolute position of the layer in units of points (1/72 inch).</returns>
    /// <remarks>The function does not change the member variables of the layer and can therefore used
    /// for position calculations without changing the layer. The function is not static because it has to use either the parent
    /// graph or the linked layer for the calculations.</remarks>
    public double YPositionToPointUnits(double y, XYPlotLayerPositionType ypostype)
    {
      switch(ypostype)
      {
        case XYPlotLayerPositionType.AbsoluteValue:
          break;
        case XYPlotLayerPositionType.RelativeToGraphDocument:
          y = y*PrintableGraphSize.Height;
          break;
        case XYPlotLayerPositionType.RelativeThisNearToLinkedLayerNear:
          if(LinkedLayer!=null)
            y = LinkedLayer.Position.Y + y*LinkedLayer.Size.Height;
          break;
        case XYPlotLayerPositionType.RelativeThisNearToLinkedLayerFar:
          if(LinkedLayer!=null)
            y = LinkedLayer.Position.Y + (1+y)*LinkedLayer.Size.Height;
          break;
        case XYPlotLayerPositionType.RelativeThisFarToLinkedLayerNear:
          if(LinkedLayer!=null)
            y = LinkedLayer.Position.Y - this.Size.Height + y*LinkedLayer.Size.Height;
          break;
        case XYPlotLayerPositionType.RelativeThisFarToLinkedLayerFar:
          if(LinkedLayer!=null)
            y = LinkedLayer.Position.Y - this.Size.Height + (1+y)*LinkedLayer.Size.Height;
          break;
      }

      return y;
    }


    /// <summary>
    /// Calculates from the x position value in points (1/72 inch), the corresponding value in user units.
    /// </summary>
    /// <param name="x">The vertical position value in points.</param>
    /// <param name="xpostype_to_convert_to">The type of the vertical position value to convert to, see <see cref="XYPlotLayerPositionType"/>.</param>
    /// <returns>Calculated value of x in user units.</returns>
    /// <remarks>The function does not change the member variables of the layer and can therefore used
    /// for position calculations without changing the layer. The function is not static because it has to use either the parent
    /// graph or the linked layer for the calculations.</remarks>
    public double XPositionToUserUnits(double x, XYPlotLayerPositionType xpostype_to_convert_to)
    {

  
      switch(xpostype_to_convert_to)
      {
        case XYPlotLayerPositionType.AbsoluteValue:
          break;
        case XYPlotLayerPositionType.RelativeToGraphDocument:
          x = x/PrintableGraphSize.Width;
          break;
        case XYPlotLayerPositionType.RelativeThisNearToLinkedLayerNear:
          if(LinkedLayer!=null)
            x = (x-LinkedLayer.Position.X)/LinkedLayer.Size.Height;
          break;
        case XYPlotLayerPositionType.RelativeThisNearToLinkedLayerFar:
          if(LinkedLayer!=null)
            x = (x-LinkedLayer.Position.X)/LinkedLayer.Size.Width - 1;
          break;
        case XYPlotLayerPositionType.RelativeThisFarToLinkedLayerNear:
          if(LinkedLayer!=null)
            x = (x-LinkedLayer.Position.X + this.Size.Width)/LinkedLayer.Size.Width;
          break;
        case XYPlotLayerPositionType.RelativeThisFarToLinkedLayerFar:
          if(LinkedLayer!=null)
            x = (x-LinkedLayer.Position.X + this.Size.Width)/LinkedLayer.Size.Width - 1;
          break;
      }

      return x;
    }


    /// <summary>
    /// Calculates from the y position value in points (1/72 inch), the corresponding value in user units.
    /// </summary>
    /// <param name="y">The vertical position value in points.</param>
    /// <param name="ypostype_to_convert_to">The type of the vertical position value to convert to, see <see cref="XYPlotLayerPositionType"/>.</param>
    /// <returns>Calculated value of y in user units.</returns>
    /// <remarks>The function does not change the member variables of the layer and can therefore used
    /// for position calculations without changing the layer. The function is not static because it has to use either the parent
    /// graph or the linked layer for the calculations.</remarks>
    public double YPositionToUserUnits(double y, XYPlotLayerPositionType ypostype_to_convert_to)
    {
      switch(ypostype_to_convert_to)
      {
        case XYPlotLayerPositionType.AbsoluteValue:
          break;
        case XYPlotLayerPositionType.RelativeToGraphDocument:
          y = y/PrintableGraphSize.Height;
          break;
        case XYPlotLayerPositionType.RelativeThisNearToLinkedLayerNear:
          if(LinkedLayer!=null)
            y = (y-LinkedLayer.Position.Y)/LinkedLayer.Size.Height;
          break;
        case XYPlotLayerPositionType.RelativeThisNearToLinkedLayerFar:
          if(LinkedLayer!=null)
            y = (y-LinkedLayer.Position.Y)/LinkedLayer.Size.Height - 1;
          break;
        case XYPlotLayerPositionType.RelativeThisFarToLinkedLayerNear:
          if(LinkedLayer!=null)
            y = (y-LinkedLayer.Position.Y + this.Size.Height)/LinkedLayer.Size.Height;
          break;
        case XYPlotLayerPositionType.RelativeThisFarToLinkedLayerFar:
          if(LinkedLayer!=null)
            y = (y-LinkedLayer.Position.Y + this.Size.Height)/LinkedLayer.Size.Height - 1;
          break;
      }

      return y;
    }


    /// <summary>
    /// Sets the cached position value in <see cref="_cachedLayerPosition"/> by calculating it
    /// from the position values (<see cref="_location.XPosition"/> and <see cref="_location.YPosition"/>) 
    /// and the position types (<see cref="_location.XPositionType"/> and <see cref="_location.YPositionType"/>).
    /// </summary>
    protected void CalculateCachedPosition()
    {
      PointF newPos = new PointF(
        (float)XPositionToPointUnits(this._location.XPosition,this._location.XPositionType),
        (float)YPositionToPointUnits(this._location.YPosition, this._location.YPositionType));
      if(newPos != this._cachedLayerPosition)
      {
        this._cachedLayerPosition=newPos;
        this.CalculateMatrix();
        OnPositionChanged();
      }
    }


    public void SetSize(double width, XYPlotLayerSizeType widthtype, double height, XYPlotLayerSizeType heighttype)
    {
      this._location.Width = width;
      this._location.WidthType = widthtype;
      this._location.Height = height;
      this._location.HeightType = heighttype;

      CalculateCachedSize();
    }


    protected double WidthToPointUnits(double width, XYPlotLayerSizeType widthtype)
    {
      switch(widthtype)
      {
        case XYPlotLayerSizeType.RelativeToGraphDocument:
          width *= PrintableGraphSize.Width;
          break;
        case XYPlotLayerSizeType.RelativeToLinkedLayer:
          if(null!=LinkedLayer)
            width *= LinkedLayer.Size.Width;
          break;
      }
      return width;
    }

    protected double HeightToPointUnits(double height, XYPlotLayerSizeType heighttype)
    {
      switch(heighttype)
      {
        case XYPlotLayerSizeType.RelativeToGraphDocument:
          height *= PrintableGraphSize.Height;
          break;
        case XYPlotLayerSizeType.RelativeToLinkedLayer:
          if(null!=LinkedLayer)
            height *= LinkedLayer.Size.Height;
          break;
      }
      return height;
    }


    /// <summary>
    /// Convert the width in points (1/72 inch) to user units of the type <paramref name="widthtype_to_convert_to"/>.
    /// </summary>
    /// <param name="width">The height value to convert (in point units).</param>
    /// <param name="widthtype_to_convert_to">The user unit type to convert to.</param>
    /// <returns>The value of the width in user units.</returns>
    protected double WidthToUserUnits(double width, XYPlotLayerSizeType widthtype_to_convert_to)
    {
  
      switch(widthtype_to_convert_to)
      {
        case XYPlotLayerSizeType.RelativeToGraphDocument:
          width /= PrintableGraphSize.Width;
          break;
        case XYPlotLayerSizeType.RelativeToLinkedLayer:
          if(null!=LinkedLayer)
            width /= LinkedLayer.Size.Width;
          break;
      }
      return width;
    }


    /// <summary>
    /// Convert the heigth in points (1/72 inch) to user units of the type <paramref name="heighttype_to_convert_to"/>.
    /// </summary>
    /// <param name="height">The height value to convert (in point units).</param>
    /// <param name="heighttype_to_convert_to">The user unit type to convert to.</param>
    /// <returns>The value of the height in user units.</returns>
    protected double HeightToUserUnits(double height, XYPlotLayerSizeType heighttype_to_convert_to)
    {

      switch(heighttype_to_convert_to)
      {
        case XYPlotLayerSizeType.RelativeToGraphDocument:
          height /= PrintableGraphSize.Height;
          break;
        case XYPlotLayerSizeType.RelativeToLinkedLayer:
          if(null!=LinkedLayer)
            height /= LinkedLayer.Size.Height;
          break;
      }
      return height;
    }


    /// <summary>
    /// Sets the cached size value in <see cref="_cachedLayerSize"/> by calculating it
    /// from the position values (<see cref="_location.Width"/> and <see cref="_location.Height"/>) 
    /// and the size types (<see cref="_location.WidthType"/> and <see cref="_location.HeightType"/>).
    /// </summary>
    protected void CalculateCachedSize()
    {
      SizeF newSize = new SizeF(
        (float)WidthToPointUnits(this._location.Width,this._location.WidthType),
        (float)HeightToPointUnits(this._location.Height, this._location.HeightType));
      if(newSize != this._cachedLayerSize)
      {
        this._cachedLayerSize=newSize;
        this.CalculateMatrix();
        OnSizeChanged();
      }
    }


    /// <summary>Returns the user x position value of the layer.</summary>
    /// <value>User x position value of the layer.</value>
    public double UserXPosition
    {
      get { return this._location.XPosition; }
    }

    /// <summary>Returns the user y position value of the layer.</summary>
    /// <value>User y position value of the layer.</value>
    public double UserYPosition
    {
      get { return this._location.YPosition; }
    }

    /// <summary>Returns the user width value of the layer.</summary>
    /// <value>User width value of the layer.</value>
    public double UserWidth
    {
      get { return this._location.Width; }
    }

    /// <summary>Returns the user height value of the layer.</summary>
    /// <value>User height value of the layer.</value>
    public double UserHeight
    {
      get { return this._location.Height; }
    }

    /// <summary>Returns the type of the user x position value of the layer.</summary>
    /// <value>Type of the user x position value of the layer.</value>
    public XYPlotLayerPositionType UserXPositionType
    {
      get { return this._location.XPositionType; }
    }

    /// <summary>Returns the type of the user y position value of the layer.</summary>
    /// <value>Type of the User y position value of the layer.</value>
    public XYPlotLayerPositionType UserYPositionType
    {
      get { return this._location.YPositionType; }
    }

    /// <summary>Returns the type of the the user width value of the layer.</summary>
    /// <value>Type of the User width value of the layer.</value>
    public XYPlotLayerSizeType UserWidthType
    {
      get { return this._location.WidthType; }
    }

    /// <summary>Returns the the type of the user height value of the layer.</summary>
    /// <value>Type of the User height value of the layer.</value>
    public XYPlotLayerSizeType UserHeightType
    {
      get { return this._location.HeightType; }
    }



    /// <summary>
    /// Measures to do when the position of the linked layer changed.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The event args.</param>
    protected void EhLinkedLayerPositionChanged(object sender, System.EventArgs e)
    {
      CalculateCachedPosition();
    }

    /// <summary>
    /// Measures to do when the size of the linked layer changed.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The event args.</param>
    protected void EhLinkedLayerSizeChanged(object sender, System.EventArgs e)
    {
      CalculateCachedSize();
      CalculateCachedPosition();
    }


    #endregion // Position and Size

    #region Axis related

    /// <summary>Gets or sets the x axis of this layer.</summary>
    /// <value>The x axis of the layer.</value>
    public Axis XAxis
    {
      get 
      {
        return _axisProperties.X.Axis; 
      }
      set
      {
        _axisProperties.X.Axis = value;
      }
    }

    /// <summary>Indicates if x axis is linked to the linked layer x axis.</summary>
    /// <value>True if x axis is linked to the linked layer x axis.</value>
    public bool IsXAxisLinked
    {
      get
      {
        return this._axisProperties.X.IsLinked; 
      }
      set
      {
        _axisProperties.X.IsLinked = value;
      }
    }

    
    bool EhXAxisInterrogateBoundaryChangedEvent()
    {
      // do nothing here, for the future we can decide to change the linked axis boundaries
      return this.IsXAxisLinked;
    }

    /// <summary>
    /// Called when a new x-axis was set (not when the x-axis has changed its boundaries).
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void EhXAxisInstanceChanged(object sender, EventArgs e)
    {

      // now we have to inform all the PlotItems that a new axis was loaded
      if (this.IsXAxisLinked)
        this._axisProperties.X.EhLinkedLayerAxesChanged(LinkedLayer.AxisProperties.X.Axis);
      else
        RescaleXAxis();
    }

   

    public void RescaleXAxis()
    {
      // we have to disable our own Handler since if we change one DataBound of a association,
      //it generates a OnBoundaryChanged, and then all boundaries are merges into the axis boundary, 
      //but (alas!) not all boundaries are now of the new type!
      _plotAssociationXBoundariesChanged_EventSuspendCount++; 
        
      _axisProperties.X.Axis.DataBoundsObject.BeginUpdate(); // Suppress events from the y-axis now
      _axisProperties.X.Axis.DataBoundsObject.Reset();
      foreach(PlotItem pa in this.PlotItems)
      {
        if(pa is IXBoundsHolder)
        {
          // first ensure the right data bound object is set on the XYColumnPlotData
          ((IXBoundsHolder)pa).SetXBoundsFromTemplate(_axisProperties.X.Axis.DataBoundsObject); // ensure that data bound object is of the right type
          // now merge the bounds with x and yAxis
          ((IXBoundsHolder)pa).MergeXBoundsInto(_axisProperties.X.Axis.DataBoundsObject); // merge all x-boundaries in the x-axis boundary object
        
        }
      }
      _plotAssociationXBoundariesChanged_EventSuspendCount = Math.Max(0,_plotAssociationXBoundariesChanged_EventSuspendCount-1);
      _axisProperties.X.Axis.DataBoundsObject.EndUpdate();
      _axisProperties.X.Axis.ProcessDataBounds();
    }
   
  
    /// <summary>Gets or sets the y axis of this layer.</summary>
    /// <value>The y axis of the layer.</value>
    public Axis YAxis
    {
      get 
      {
        return _axisProperties.Y.Axis;
      }
      set
      {
        _axisProperties.Y.Axis = value;
      }
    }

    /// <summary>Indicates if y axis is linked to the linked layer y axis.</summary>
    /// <value>True if y axis is linked to the linked layer y axis.</value>
    public bool IsYAxisLinked
    {
      get 
      { 
        return this._axisProperties.Y.IsLinked; 
      }
      set
      {
        _axisProperties.Y.IsLinked = value;
        
      }
    }

    /// <summary>
    /// Called when a new x-axis was set (not when the x-axis has changed its boundaries).
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void EhYAxisInstanceChanged(object sender, EventArgs e)
    {

      // now we have to inform all the PlotItems that a new axis was loaded
      if (this.IsYAxisLinked)
        this._axisProperties.Y.EhLinkedLayerAxesChanged(LinkedLayer.AxisProperties.X.Axis);
      else
        RescaleYAxis();
    }

    public void RescaleYAxis()
    {
      // we have to disable our own Handler since if we change one DataBound of a association,
      //it generates a OnBoundaryChanged, and then all boundaries are merges into the axis boundary, 
      //but (alas!) not all boundaries are now of the new type!
      _plotAssociationYBoundariesChanged_EventSuspendCount++; 

      _axisProperties.Y.Axis.DataBoundsObject.BeginUpdate();
      _axisProperties.Y.Axis.DataBoundsObject.Reset();
      foreach(PlotItem pa in this.PlotItems)
      {
        if(pa is IYBoundsHolder)
        {
          // first ensure the right data bound object is set on the XYColumnPlotData
          ((IYBoundsHolder)pa).SetYBoundsFromTemplate(_axisProperties.Y.Axis.DataBoundsObject); // ensure that data bound object is of the right type
          // now merge the bounds with x and yAxis
          ((IYBoundsHolder)pa).MergeYBoundsInto(_axisProperties.Y.Axis.DataBoundsObject); // merge all x-boundaries in the x-axis boundary object
        
        }
      }
      _plotAssociationYBoundariesChanged_EventSuspendCount = Math.Max(0,_plotAssociationYBoundariesChanged_EventSuspendCount-1);
      _axisProperties.Y.Axis.DataBoundsObject.EndUpdate();
      _axisProperties.Y.Axis.ProcessDataBounds();
    }
    

    bool EhYAxisInterrogateBoundaryChangedEvent()
    {
      // do nothing here, for the future we can decide to change the linked axis boundaries
      return this.IsYAxisLinked;
    }


    /// <summary>
    /// Only needed after deserialization, when the first time the document node is resolved.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void EhLinkedLayerInstanceChanged(object sender, System.EventArgs e)
    {
      if (_linkedLayer.DocumentObject != null)
      {
        _linkedLayer.Changed -= new EventHandler(this.EhLinkedLayerInstanceChanged);
        this.LinkedLayer = (XYPlotLayer)_linkedLayer.DocumentObject;
      }
    }

    /// <summary>
    /// Measures to do when one of the axis of the linked layer changed.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The event args.</param>
    protected void EhLinkedLayerAxesChanged(object sender, System.EventArgs e)
    {
      if(null==LinkedLayer)
        return; // this should not happen, since what is sender then?

      if (_axisProperties.X.IsLinked && null != LinkedLayer)
      {
        _axisProperties.X.EhLinkedLayerAxesChanged(LinkedLayer.AxisProperties.X.Axis);
      }

      if (_axisProperties.Y.IsLinked && null != LinkedLayer)
      {
        _axisProperties.Y.EhLinkedLayerAxesChanged(LinkedLayer.AxisProperties.Y.Axis);
      }
    }


    /// <summary>
    /// Draws an isoline on the plot area.
    /// </summary>
    /// <param name="g">Graphics context.</param>
    /// <param name="pen">The style of the pen used to draw the line.</param>
    /// <param name="axis">Axis for which the isoline to draw.</param>
    /// <param name="relaxisval">Relative value (0..1) on this axis.</param>
    /// <param name="relaltstart">Relative value for the alternate axis of the start of the line.</param>
    /// <param name="relaltend">Relative value for the alternate axis of the end of the line.</param>
    public void DrawIsoLine(Graphics g, Pen pen, int axis, double relaxisval, double relaltstart, double relaltend)
    {
      double x1, y1, x2, y2;
      if (axis == 0)
      {
        this.LogicalToAreaConversion.Convert(relaxisval, relaltstart, out x1, out y1);
        this.LogicalToAreaConversion.Convert(relaxisval, relaltend, out x2, out y2);
      }
      else
      {
        this.LogicalToAreaConversion.Convert(relaltstart, relaxisval, out x1, out y1);
        this.LogicalToAreaConversion.Convert(relaltend, relaxisval, out x2, out y2);
      }

      g.DrawLine(pen, (float)x1, (float)y1, (float)x2, (float)y2);
    }



    #endregion // Axis related

    #region Style properties

    public bool ClipDataToFrame
    {
      get
      {
        return _clipDataToFrame;
      }
      set
      {
        bool oldvalue = _clipDataToFrame;
        _clipDataToFrame = value;

        if(value!=oldvalue)
          this.OnInvalidate();
      }
    }

    public XYAxisStyle LeftAxisStyle
    {
      get { return _axisStyles[EdgeType.Left].AxisStyle; }
    }
    public XYAxisStyle BottomAxisStyle
    {
      get { return _axisStyles[EdgeType.Bottom].AxisStyle; }
    }
    public XYAxisStyle RightAxisStyle
    {
      get { return _axisStyles[EdgeType.Right].AxisStyle; }
    }
    public XYAxisStyle TopAxisStyle
    {
      get { return _axisStyles[EdgeType.Top].AxisStyle; }
    }


    public AbstractXYAxisLabelStyle LeftLabelStyle
    {
      get { return this._axisStyles[EdgeType.Left].MajorLabelStyle; }
    }
    public AbstractXYAxisLabelStyle RightLabelStyle
    {
      get { return this._axisStyles[EdgeType.Right].MajorLabelStyle; }
    }
    public AbstractXYAxisLabelStyle BottomLabelStyle
    {
      get { return this._axisStyles[EdgeType.Bottom].MajorLabelStyle; }
    }
    public AbstractXYAxisLabelStyle TopLabelStyle
    {
      get { return this._axisStyles[EdgeType.Top].MajorLabelStyle; }
    }
    
    public bool LeftAxisEnabled
    {
      get 
      {
        return _axisStyles[EdgeType.Left].ShowAxis; 
      }
      set
      {
        _axisStyles[EdgeType.Left].ShowAxis = value;
      }
    }

    public bool BottomAxisEnabled
    {
      get
      {
        return _axisStyles[EdgeType.Bottom].ShowAxis;
      }
      set
      {
        _axisStyles[EdgeType.Bottom].ShowAxis = value;
      }
    }
    public bool RightAxisEnabled
    {
      get
      {
        return _axisStyles[EdgeType.Right].ShowAxis;
      }
      set
      {
        _axisStyles[EdgeType.Right].ShowAxis = value;
      }
    }
    public bool TopAxisEnabled
    {
      get
      {
        return _axisStyles[EdgeType.Top].ShowAxis;
      }
      set
      {
        _axisStyles[EdgeType.Top].ShowAxis = value;
      }
    }

    public TextGraphics LeftAxisTitle
    {
      get { return this._axisStyles[EdgeType.Left].Title; }
      set
      {
        this._axisStyles[EdgeType.Left].Title = value;
        //this.OnInvalidate();
      }
    }

    public TextGraphics RightAxisTitle
    {
      get { return this._axisStyles[EdgeType.Right].Title; }
      set
      {
        this._axisStyles[EdgeType.Right].Title = value;
        //this.OnInvalidate();
      }
    }

    public TextGraphics TopAxisTitle
    {
      get { return this._axisStyles[EdgeType.Top].Title; }
      set
      {
        this._axisStyles[EdgeType.Top].Title = value;
        //this.OnInvalidate();
      }
    }
    public TextGraphics BottomAxisTitle
    {
      get { return this._axisStyles[EdgeType.Bottom].Title; }
      set
      {
        this._axisStyles[EdgeType.Bottom].Title = value;
        //this.OnInvalidate();
      }
    }


    private string GetAxisTitleString(EdgeType edge)
    {
      return _axisStyles[edge].Title != null ? _axisStyles[edge].Title.Text : null; 
    }
    private void SetAxisTitleString(EdgeType edge, string value)
    {
      string oldtitle = _axisStyles[edge].Title == null ? null : _axisStyles[edge].Title.Text;
      string newtitle = (value == null || value == String.Empty) ? null : value;

      if (newtitle != oldtitle)
      {
        if (newtitle == null)
          _axisStyles[edge].Title = null;
        else if (_axisStyles[edge].Title != null)
          _axisStyles[edge].Title.Text = newtitle;
        else
        {
          TextGraphics tg = new TextGraphics();
          switch(edge)
          {
            case EdgeType.Left:
              tg.Rotation = -90;
              tg.XAnchor = TextGraphics.XAnchorPositionType.Center;
              tg.YAnchor = TextGraphics.YAnchorPositionType.Bottom;
              tg.Position = new PointF(-0.125f * Size.Width, 0.5f * Size.Height);
              break;
            case EdgeType.Bottom:
              tg.Rotation = 0;
              tg.XAnchor = TextGraphics.XAnchorPositionType.Center;
              tg.YAnchor = TextGraphics.YAnchorPositionType.Top;
              tg.Position = new PointF(0.5f * Size.Width, 1.125f * Size.Height);
              break;
            case EdgeType.Right:
              tg.Rotation = -90;
              tg.XAnchor = TextGraphics.XAnchorPositionType.Center;
              tg.YAnchor = TextGraphics.YAnchorPositionType.Top;
              tg.Position = new PointF(1.125f * Size.Width, 0.5f * Size.Height);
              break;
            case EdgeType.Top:
              tg.Rotation = 0;
              tg.XAnchor = TextGraphics.XAnchorPositionType.Center;
              tg.YAnchor = TextGraphics.YAnchorPositionType.Bottom;
              tg.Position = new PointF(0.5f * Size.Width, -0.125f * Size.Height);
              break;
          }
          tg.Text = newtitle;
          _axisStyles[edge].Title = tg;
        }
      }
    }

    public string LeftAxisTitleString
    {
      get
      {
        return GetAxisTitleString(EdgeType.Left);
      }
      set
      {
        SetAxisTitleString(EdgeType.Left,value);
      }
    }

    public string RightAxisTitleString
    {
      get
      {
        return GetAxisTitleString(EdgeType.Right);
      }
      set
      {
        SetAxisTitleString(EdgeType.Right,value);
      }
    }

    public string TopAxisTitleString
    {
      get
      {
        return GetAxisTitleString(EdgeType.Top);
      }
      set
      {
        SetAxisTitleString(EdgeType.Top, value);
      }
    }

    public string BottomAxisTitleString
    {
      get
      {
        return GetAxisTitleString(EdgeType.Bottom);
      }
      set
      {
        SetAxisTitleString(EdgeType.Bottom,value);
      }
    }


    #endregion // Style properties

    #region Painting and Hit testing
    public virtual void Paint(Graphics g)
    {
      GraphicsState savedgstate = g.Save();
      //g.TranslateTransform(m_LayerPosition.X,m_LayerPosition.Y);
      //g.RotateTransform(m_LayerAngle);
      
      g.MultiplyTransform(_cachedForwardMatrix);

      if(_layerBackground!=null)
        _layerBackground.Draw(g,new RectangleF(0,0,_cachedLayerSize.Width,_cachedLayerSize.Height));
       
      _graphObjects.DrawObjects(g,1,this);

      RectangleF layerBounds = new RectangleF(_cachedLayerPosition,_cachedLayerSize);


      // Before we paint the axis, we have to make sure that all plot items
      // had their data updated, so that the axes are updated before they are drawn!
      foreach(PlotItem pi in _plotItems)
      {
        pi.UpdateCachedData(this);
      }

      _axisStyles.Paint(g, this);

      if(_legend!=null)
        _legend.Paint(g,this);


      if(ClipDataToFrame)
      {
        g.SetClip(new RectangleF(new PointF(0,0),this._cachedLayerSize));
      }

      foreach(PlotItem pi in _plotItems)
      {
        pi.Paint(g,this);
      }


      g.Restore(savedgstate);
    }

    private IHitTestObject ForwardTransform(IHitTestObject o)
    {
      o.Transform(_cachedForwardMatrix);
      o.ParentLayer=this;
      return o;
    }

    public IHitTestObject HitTest(PointF pageC, bool plotItemsOnly)
    {
      IHitTestObject hit;

      PointF layerC = GraphToLayerCoordinates(pageC);


      GraphicsObject[] specObjects = 
          {
            _axisStyles[EdgeType.Left].Title,
            _axisStyles[EdgeType.Bottom].Title,
            _axisStyles[EdgeType.Top].Title,
            _axisStyles[EdgeType.Right].Title,
            _legend
          };

      if (!plotItemsOnly)
      {

        // do the hit test first for the special objects of the layer
        foreach (GraphicsObject go in specObjects)
        {
          if (null != go)
          {
            hit = go.HitTest(layerC);
            if (null != hit)
            {
              if (null == hit.Remove && (hit.HittedObject is GraphicsObject))
                hit.Remove = new DoubleClickHandler(EhTitlesOrLegend_Remove);
              return ForwardTransform(hit);
            }
          }
        }

        // first hit testing all four corners of the layer
        GraphicsPath layercorners = new GraphicsPath();
        float catchrange = 6;
        layercorners.AddEllipse(-catchrange, -catchrange, 2 * catchrange, 2 * catchrange);
        layercorners.AddEllipse(_cachedLayerSize.Width - catchrange, 0 - catchrange, 2 * catchrange, 2 * catchrange);
        layercorners.AddEllipse(0 - catchrange, _cachedLayerSize.Height - catchrange, 2 * catchrange, 2 * catchrange);
        layercorners.AddEllipse(_cachedLayerSize.Width - catchrange, _cachedLayerSize.Height - catchrange, 2 * catchrange, 2 * catchrange);
        layercorners.CloseAllFigures();
        if (layercorners.IsVisible(layerC))
        {
          hit = new HitTestObject(layercorners, this);
          hit.DoubleClick = LayerPositionEditorMethod;
          return ForwardTransform(hit);
        }



        // hit testing the axes - first a small area around the axis line
        // if hitting this, the editor for scaling the axis should be shown
        if (LeftAxisEnabled && null != (hit = LeftAxisStyle.HitTest(this, layerC, false)))
        {
          hit.DoubleClick = AxisScaleEditorMethod;
          return ForwardTransform(hit);
        }
        if (BottomAxisEnabled && null != (hit = BottomAxisStyle.HitTest(this, layerC, false)))
        {
          hit.DoubleClick = AxisScaleEditorMethod;
          return ForwardTransform(hit);
        }
        if (RightAxisEnabled && null != (hit = RightAxisStyle.HitTest(this, layerC, false)))
        {
          hit.DoubleClick = AxisScaleEditorMethod;
          return ForwardTransform(hit);
        }
        if (TopAxisEnabled && null != (hit = TopAxisStyle.HitTest(this, layerC, false)))
        {
          hit.DoubleClick = AxisScaleEditorMethod;
          return ForwardTransform(hit);
        }


        // hit testing the axes - secondly now wiht the ticks
        // in this case the TitleAndFormat editor for the axis should be shown
        if (LeftAxisEnabled && null != (hit = LeftAxisStyle.HitTest(this, layerC, true)))
        {
          if (hit.DoubleClick == null) hit.DoubleClick = AxisStyleEditorMethod;
          return ForwardTransform(hit);
        }
        if (BottomAxisEnabled && null != (hit = BottomAxisStyle.HitTest(this, layerC, true)))
        {
          if (hit.DoubleClick == null) hit.DoubleClick = AxisStyleEditorMethod;
          return ForwardTransform(hit);
        }
        if (RightAxisEnabled && null != (hit = RightAxisStyle.HitTest(this, layerC, true)))
        {
          if (hit.DoubleClick == null) hit.DoubleClick = AxisStyleEditorMethod;
          return ForwardTransform(hit);
        }
        if (TopAxisEnabled && null != (hit = TopAxisStyle.HitTest(this, layerC, true)))
        {
          if (hit.DoubleClick == null) hit.DoubleClick = AxisStyleEditorMethod;
          return ForwardTransform(hit);
        }

        // hit testing the axes labels
        if (LeftAxisEnabled && null != (hit = this.LeftLabelStyle.HitTest(this, layerC)))
        {
          if (hit.DoubleClick == null) hit.DoubleClick = AxisLabelStyleEditorMethod;
          return ForwardTransform(hit);
        }
        if (BottomAxisEnabled && null != (hit = BottomLabelStyle.HitTest(this, layerC)))
        {
          if (hit.DoubleClick == null) hit.DoubleClick = AxisLabelStyleEditorMethod;
          return ForwardTransform(hit);
        }
        if (RightAxisEnabled && null != (hit = RightLabelStyle.HitTest(this, layerC)))
        {
          if (hit.DoubleClick == null) hit.DoubleClick = AxisLabelStyleEditorMethod;
          return ForwardTransform(hit);
        }
        if (TopAxisEnabled && null != (hit = TopLabelStyle.HitTest(this, layerC)))
        {
          if (hit.DoubleClick == null) hit.DoubleClick = AxisLabelStyleEditorMethod;
          return ForwardTransform(hit);
        }

        // now hit testing the other objects in the layer
        foreach (GraphicsObject go in _graphObjects)
        {
          hit = go.HitTest(layerC);
          if (null != hit)
          {
            if (null == hit.Remove && (hit.HittedObject is GraphicsObject))
              hit.Remove = new DoubleClickHandler(EhGraphicsObject_Remove);
            return ForwardTransform(hit);
          }
        }
      }

      if(null!=(hit=_plotItems.HitTest(this,layerC)))
      {
        if(hit.DoubleClick==null) hit.DoubleClick=PlotItemEditorMethod;
        return ForwardTransform(hit);
      }
     
      return null;
    }


    #endregion // Painting and Hit testing

    #region Editor methods

    public static DoubleClickHandler AxisScaleEditorMethod;
    public static DoubleClickHandler AxisStyleEditorMethod;
    public static DoubleClickHandler AxisLabelStyleEditorMethod;
    public static DoubleClickHandler LayerPositionEditorMethod;
    public static DoubleClickHandler PlotItemEditorMethod;

    #endregion

    #region Event firing


    protected void OnSizeChanged()
    {
      if(null!=SizeChanged)
        SizeChanged(this,new System.EventArgs());

      OnInvalidate();
    }


    protected void OnPositionChanged()
    {
      if(null!=PositionChanged)
        PositionChanged(this,new System.EventArgs());

      OnInvalidate();
    }

   

    protected void OnInvalidate()
    {
      if(this._parentLayerCollection is Main.IChildChangedEventSink)
        ((Main.IChildChangedEventSink)this._parentLayerCollection).EhChildChanged(this,EventArgs.Empty);
    }

    #endregion

    #region Handler of child events

    static bool EhGraphicsObject_Remove(IHitTestObject o)
    {
      GraphicsObject go = (GraphicsObject)o.HittedObject;
      o.ParentLayer.GraphObjects.Remove(go);
      return true;
    }
    static bool EhTitlesOrLegend_Remove(IHitTestObject o)
    {
      GraphicsObject go = (GraphicsObject)o.HittedObject;
      XYPlotLayer layer = o.ParentLayer;

      if(object.ReferenceEquals(go,layer._legend))
      {
        layer._legend=null;
        return true;
      }
      else if(object.ReferenceEquals(go,layer._axisStyles[EdgeType.Left].Title))
      {
        layer._axisStyles[EdgeType.Left].Title = null;
        return true;
      }
      else if (object.ReferenceEquals(go, layer._axisStyles[EdgeType.Bottom].Title))
      {
        layer._axisStyles[EdgeType.Bottom].Title = null;
        return true;
      }
      else if (object.ReferenceEquals(go, layer._axisStyles[EdgeType.Right].Title))
      {
        layer._axisStyles[EdgeType.Right].Title = null;
        return true;
      }
      else if (object.ReferenceEquals(go, layer._axisStyles[EdgeType.Top].Title))
      {
        layer._axisStyles[EdgeType.Top].Title = null;
        return true;
      }

      return false;
    }
    /// <summary>
    /// This handler is called if a x-boundary from any of the plotassociations of this layer
    /// has changed. We then have to recalculate the boundaries.
    /// </summary>
    /// <param name="sender">The plotassociation that has caused the boundary changed event.</param>
    /// <param name="e">The boundary changed event args.</param>
    /// <remarks>Unfortunately we do not know if the boundary is extended or shrinked, if is is extended
    /// if would be possible to merge only the changed boundary into the x-axis boundary.
    /// But since we don't know about that, we have to completely recalculate the boundary be using the boundaries of
    /// all PlotAssociations of this layer.</remarks>
    public void OnPlotAssociationXBoundariesChanged(object sender, BoundariesChangedEventArgs e)
    {
      if(0==_plotAssociationXBoundariesChanged_EventSuspendCount)
      {
        // now we have to inform all the PlotAssociations that a new axis was loaded
        _axisProperties.X.Axis.DataBoundsObject.BeginUpdate();
        _axisProperties.X.Axis.DataBoundsObject.Reset();
        foreach(PlotItem pa in this.PlotItems)
        {
          if(pa is IXBoundsHolder)
          {
            // merge the bounds with x and yAxis
            ((IXBoundsHolder)pa).MergeXBoundsInto(_axisProperties.X.Axis.DataBoundsObject); // merge all x-boundaries in the x-axis boundary object
          }
        }
        _axisProperties.X.Axis.DataBoundsObject.EndUpdate();
      }
    }

    /// <summary>
    /// This handler is called if a y-boundary from any of the plotassociations of this layer
    /// has changed. We then have to recalculate the boundaries.
    /// </summary>
    /// <param name="sender">The plotassociation that has caused the boundary changed event.</param>
    /// <param name="e">The boundary changed event args.</param>
    /// <remarks>Unfortunately we do not know if the boundary is extended or shrinked, if is is extended
    /// if would be possible to merge only the changed boundary into the y-axis boundary.
    /// But since we don't know about that, we have to completely recalculate the boundary be using the boundaries of
    /// all PlotAssociations of this layer.</remarks>
    public void OnPlotAssociationYBoundariesChanged(object sender, BoundariesChangedEventArgs e)
    {
      if(0==_plotAssociationYBoundariesChanged_EventSuspendCount)
      {
        // now we have to inform all the PlotAssociations that a new axis was loaded
        _axisProperties.Y.Axis.DataBoundsObject.BeginUpdate();
        _axisProperties.Y.Axis.DataBoundsObject.Reset();
        foreach(PlotItem pa in this.PlotItems)
        {
          if(pa is IYBoundsHolder)
          {
            // merge the bounds with x and yAxis
            ((IYBoundsHolder)pa).MergeYBoundsInto(_axisProperties.Y.Axis.DataBoundsObject); // merge all x-boundaries in the x-axis boundary object
        
          }
        }
        _axisProperties.Y.Axis.DataBoundsObject.EndUpdate();
      }
    }
    
    
 
    
    protected virtual void OnChildChangedEventHandler(object sender, System.EventArgs e)
    {
      OnInvalidate();
    }
    
    #endregion

    #region IDocumentNode Members

    public object ParentObject
    {
      get
      {
        return this._parentLayerCollection;
      }
    }

    public string Name
    {
      get
      {
        if(ParentObject is Main.INamedObjectCollection)
          return ((Main.INamedObjectCollection)ParentObject).GetNameOfChildObject(this);
        else
          return GetDefaultNameOfLayer(this.Number);
      }
    }

    /// <summary>
    /// Returns the document name of the layer at index i. Actually, this is a name of the form L0, L1, L2 and so on.
    /// </summary>
    /// <param name="i">The layer index.</param>
    /// <returns>The name of the layer at index i.</returns>
    public static string GetDefaultNameOfLayer(int i)
    {
      return "L"+i.ToString(); // do not change it, since the name is used in serialization
    }

    #endregion

    #region Inner types

    public bool IsLinear { get { return XAxis is LinearAxis && YAxis is LinearAxis; }}
    public bool IsOrthogonal { get { return true; }}
    public bool IsAffine { get { return true; }}

    LogicalToAreaConverter _LogicalToAreaConverter;
    public I2DTo2DConverter LogicalToAreaConversion
    {
      get
      {
        _LogicalToAreaConverter.Update();
        return _LogicalToAreaConverter;
      }
    }

    AreaToLogicalConverter _AreaToLogicalConverter;
    public I2DTo2DConverter AreaToLogicalConversion 
    {
      get 
      {
        _AreaToLogicalConverter.Update();
        return _AreaToLogicalConverter; 
      }
    }

    public Region GetRegion()
    {
      return new Region(new RectangleF(new PointF(0,0),this.Size));
    }

    protected class LogicalToAreaConverter : I2DTo2DConverter
    {
      XYPlotLayer _layer;
      double _layerWidth;
      double _layerHeight;

      public LogicalToAreaConverter(XYPlotLayer layer)
      {
        _layer = layer;
        _layer.SizeChanged += new EventHandler(EhChanged);

        _layerWidth =  _layer.Size.Width;
        _layerHeight = _layer.Size.Height;
      }
      
      public void Update()
      {
        _layerWidth =  _layer.Size.Width;
        _layerHeight = _layer.Size.Height;
      }

      public void EhChanged(object sender, EventArgs e)
      {
        _layerWidth =  _layer.Size.Width;
        _layerHeight = _layer.Size.Height;

        if(null!=Changed)
          Changed(this,e);
      }
 
      /// <summary>
      /// Calculates from two logical values (values between 0 and 1) the coordinates of the point. Returns true if the conversion
      /// is possible, otherwise false.
      /// </summary>
      /// <param name="x_rel">The logical x value.</param>
      /// <param name="y_rel">The logical y value.</param>
      /// <param name="xlocation">On return, gives the x coordinate of the converted value (for instance location).</param>
      /// <param name="ylocation">On return, gives the y coordinate of the converted value (for instance location).</param>
      /// <returns>True if the conversion was successfull, false if the conversion was not possible.</returns>
      public bool Convert(double x_rel, double y_rel, out double xlocation, out double ylocation)
      {
        xlocation = _layerWidth * x_rel;
        ylocation = _layerHeight * (1-y_rel);
        return !double.IsNaN(xlocation) && !double.IsNaN(ylocation);
      }

      public event System.EventHandler Changed;

    }


    protected class AreaToLogicalConverter : I2DTo2DConverter
    {
      XYPlotLayer _layer;
      double _layerWidth;
      double _layerHeight;

      public AreaToLogicalConverter(XYPlotLayer layer)
      {
        _layer = layer;
        _layer.SizeChanged += new EventHandler(EhChanged);

        _layerWidth = _layer.Size.Width;
        _layerHeight = _layer.Size.Height;
      }
      public void Update()
      {
        _layerWidth = _layer.Size.Width;
        _layerHeight = _layer.Size.Height;
      }
      public void EhChanged(object sender, EventArgs e)
      {
        _layerWidth  = _layer.Size.Width;
        _layerHeight = _layer.Size.Height;

        if(null!=Changed)
          Changed(this,e);
      }
 
      public bool Convert(double xlocation, double ylocation, out double x_rel, out double y_rel)
      {
        x_rel = xlocation / _layerWidth;
        y_rel = 1-ylocation/_layerHeight;
        return !double.IsNaN(x_rel) && !double.IsNaN(y_rel);
      }
      public event System.EventHandler Changed;

    }


    #endregion

  
  }
}
