#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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
    #region Enumerations

    /// <summary>
    /// The type of the size (i.e. width and height values.
    /// </summary>
    [Serializable]
      public enum SizeType 
    {
      /// <summary>
      ///  the value is a absolute value (not relative) in points (1/72 inch).
      /// </summary>
      AbsoluteValue,
      /// <summary>
      /// The value is relative to the graph document. This means that for instance the width of the layer
      /// is relative to the width of the graph document.
      /// </summary>
      RelativeToGraphDocument,
      /// <summary>
      /// The value is relative to the linked layer. This means that for instance the width of the layer
      /// is relative to the width of the linked layer.
      /// </summary>
      RelativeToLinkedLayer
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SizeType),0)]
      public class SizeTypeXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.SetNodeContent(obj.ToString()); 
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        string val = info.GetNodeContent();
        return System.Enum.Parse(typeof(SizeType),val,true);
      }
    }

    /// <summary>
    /// The type of the position values  (i.e. x and y position of the layer).
    /// </summary>
    [Serializable]
      public enum PositionType 
    {
      /// <summary>
      /// The value is a absolute value (not relative) in points (1/72 inch).
      /// </summary>
      [Description("Absolute value (points)")]
      AbsoluteValue,


      /// <summary>
      /// The value is relative to the graph document. This means that for instance the x position of the layer
      /// is relative to the width of the graph document. A x value of 0 would position the layer at the left edge of the
      /// graph document, a value of 1 on the right edge of the graph.
      /// </summary>
      [Description("Relative to graph size (0..1)")]
      RelativeToGraphDocument,

      /// <summary>
      /// The value relates the near edge (either upper or left) of this layer to the near edge of the linked layer.
      /// </summary>
      /// <remarks> The values are relative to the size of the linked layer.
      /// This means that for instance for a x position value of 0 the left edges of both layers are on the same position, for a value of 1
      /// this means that the left edge of this layer is on the right edge of the linked layer.
      /// </remarks>
      RelativeThisNearToLinkedLayerNear,
      
      
      /// <summary>
      /// The value relates the near edge (either upper or left) of this layer to the far edge (either right or bottom) of the linked layer.
      /// </summary>
      /// <remarks> The values are relative to the size of the linked layer.
      /// This means that for instance for a x position value of 0 the left edges of this layer is on the right edge of the linked layer,
      /// for a value of 1
      /// this means that the left edge of this layer is one width away from the right edge of the linked layer.
      /// </remarks>
      RelativeThisNearToLinkedLayerFar,
      /// <summary>
      /// The value relates the far edge (either right or bottom) of this layer to the near edge (either left or top) of the linked layer.
      /// </summary>
      /// <remarks> The values are relative to the size of the linked layer.
      /// This means that for instance for a x position value of 0 the right edge of this layer is on the left edge of the linked layer,
      /// for a value of 1
      /// this means that the right edge of this layer is one width away (to the right) from the leftt edge of the linked layer.
      /// </remarks>
      RelativeThisFarToLinkedLayerNear,
      /// <summary>
      /// The value relates the far edge (either right or bottom) of this layer to the far edge (either right or bottom) of the linked layer.
      /// </summary>
      /// <remarks> The values are relative to the size of the linked layer.
      /// This means that for instance for a x position value of 0 the right edge of this layer is on the right edge of the linked layer,
      /// for a value of 1
      /// this means that the right edge of this layer is one width away from the right edge of the linked layer, for a x value of -1 this
      /// means that the right edge of this layer is one width away to the left from the right edge of the linked layer and this falls together
      /// with the left edge of the linked layer.
      /// </remarks>
      RelativeThisFarToLinkedLayerFar
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PositionType),0)]
      public class PositionTypeXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.SetNodeContent(obj.ToString());
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        string val = info.GetNodeContent();
        return System.Enum.Parse(typeof(PositionType),val,true);
      }
    }
    /// <summary>
    /// Provides how the axis is linked to the corresponding axis on the linked layer.
    /// </summary>
    [Serializable]
      public enum AxisLinkType
    {
      /// <summary>
      /// The axis is not linked, i.e. independent.
      /// </summary>
      None,
      /// <summary>
      /// The axis is linked straight, i.e. it has the same origin and end value as the corresponding axis of the linked layer.
      /// </summary>
      Straight,
      /// <summary>
      /// The axis is linked custom, i.e. origin and end of axis are translated linearly using formulas org'=a1+b1*org, end'=a2+b2*end.
      /// </summary>
      Custom
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AxisLinkType),0)]
      public class AxisLinkTypeXmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.SetNodeContent(obj.ToString()); 
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        string val = info.GetNodeContent();
        return System.Enum.Parse(typeof(AxisLinkType),val,true);
      }
    }

    #endregion

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
    /// In case the size is absolute (see <see cref="SizeType"/>), this is the size of the layer. Otherwise
    /// it is only the cached value for the size, since the size is calculated then.
    /// </remarks>
    protected SizeF  _cachedLayerSize = new SizeF(0,0);

    protected Matrix _cachedForwardMatrix = new Matrix();  // forward transformation m_ForwardMatrix
    protected Matrix _cachedReverseMatrix = new Matrix(); // inverse transformation m_ForwardMatrix

    #endregion // Cached member variables

    #region Member variables

    /// <summary>True if the layer area should be filled with a background brush.</summary>
    protected bool _fillLayerArea=false;
    /// <summary>The background brush for the layer area.</summary>
    protected BrushHolder m_LayerAreaFillBrush = new BrushHolder(Color.Aqua);


    /// <summary>
    /// The layers x position value, either absolute or relative, as determined by <see cref="_layerXPositionType"/>.
    /// </summary>
    protected double _layerXPosition = 0;

    /// <summary>
    /// The type of the x position value, see <see cref="PositionType"/>.
    /// </summary>
    protected PositionType _layerXPositionType=PositionType.AbsoluteValue;

    /// <summary>
    /// The layers y position value, either absolute or relative, as determined by <see cref="_layerYPositionType"/>.
    /// </summary>
    protected double _layerYPosition = 0;

    /// <summary>
    /// The type of the y position value, see <see cref="PositionType"/>.
    /// </summary>
    protected PositionType _layerYPositionType=PositionType.AbsoluteValue;


    /// <summary>
    /// The width of the layer, either as absolute value in point (1/72 inch), or as 
    /// relative value as pointed out by <see cref="_layerWidthType"/>.
    /// </summary>
    protected double _layerWidth=0;

    /// <summary>
    /// The type of the value for the layer width, see <see cref="SizeType"/>.
    /// </summary>
    protected SizeType _layerWidthType=SizeType.AbsoluteValue;

    /// <summary>
    /// The height of the layer, either as absolute value in point (1/72 inch), or as 
    /// relative value as pointed out by <see cref="_layerHeightType"/>.
    /// </summary>
    protected double _layerHeight= 0;

    /// <summary>
    /// The type of the value for the layer height, see <see cref="SizeType"/>.
    /// </summary>
    protected SizeType _layerHeightType=SizeType.AbsoluteValue;

    /// <summary>The rotation angle (in degrees) of the layer.</summary>
    protected float  _layerAngle=0; // Rotation
    
    /// <summary>The scaling factor of the layer, normally 1.</summary>
    protected float  _layerScale=1;  // Scale

    /// <summary>If true, the data are clipped to the frame.</summary>
    protected bool _clipDataToFrame=true;

    /// <summary>The horizontal axis of the layer.</summary>
    protected Axis _xAxis; // the X-Axis
    
    /// <summary>The vertical axis of the layer.</summary>
    protected Axis _yAxis; // the Y-Axis

    /// <summary>True if the left axis should be drawn.</summary>
    protected bool _showLeftAxis = true;
    /// <summary>True if the bottom axis should be drawn.</summary>
    protected bool _showBottomAxis = true;
    /// <summary>True if the right axis should be drawn.</summary>
    protected bool _showRightAxis = true;
    /// <summary>True if the top axis should be drawn.</summary>
    protected bool _showTopAxis = true;

    protected XYAxisStyle _leftAxisStyle = new XYAxisStyle(EdgeType.Left);
    protected XYAxisStyle _bottomAxisStyle = new XYAxisStyle(EdgeType.Bottom);
    protected XYAxisStyle _rightAxisStyle = new XYAxisStyle(EdgeType.Right);
    protected XYAxisStyle _topAxisStyle = new XYAxisStyle(EdgeType.Top);

    protected AbstractXYAxisLabelStyle _leftLabelStyle = new XYAxisLabelStyle(EdgeType.Left);
    protected AbstractXYAxisLabelStyle _bottomLabelStyle = new XYAxisLabelStyle(EdgeType.Bottom);
    protected AbstractXYAxisLabelStyle _rightLabelStyle = new XYAxisLabelStyle(EdgeType.Right);
    protected AbstractXYAxisLabelStyle _topLabelStyle = new XYAxisLabelStyle(EdgeType.Top);

    protected TextGraphics _leftAxisTitle = null;
    protected TextGraphics _bottomAxisTitle = null;
    protected TextGraphics _rightAxisTitle = null;
    protected TextGraphics _topAxisTitle = null;

    protected TextGraphics _legend = null;

    /// <summary>
    /// This class summarizes all members that are belonging to one edge of the layer.
    /// </summary>
    public class XYPlotLayerEdgeProperties
    {
      EdgeType _edgeType;
      bool _showAxis;
      protected XYAxisStyle _axisStyle = new XYAxisStyle(EdgeType.Left);
      AbstractXYAxisLabelStyle _labelStyle = new XYAxisLabelStyle(EdgeType.Left);
      TextGraphics _axisTitle = null;

      public XYPlotLayerEdgeProperties(EdgeType type)
      {
        _edgeType = type;
        _showAxis = true;
        _axisStyle = new XYAxisStyle(_edgeType);
        _labelStyle = new XYAxisLabelStyle(_edgeType);
        _axisTitle = null;
      }
    }

    class XYLayerAxisProperties
    {
      protected Axis _axis; // the X-Axis

      /// <summary>Indicate if x-axis is linked to the linked layer x axis.</summary>
      protected bool _linkAxis;

      /// <summary>The value a of x-axis link for link of origin: org' = a + b*org.</summary>
      protected double _linkAxisOrgA;

      /// <summary>The value b of x-axis link for link of origin: org' = a + b*org.</summary>
      protected double _linkAxisOrgB;

      /// <summary>The value a of x-axis link for link of end: end' = a + b*end.</summary>
      protected double _linkAxisEndA;

      /// <summary>The value b of x-axis link for link of end: end' = a + b*end.</summary>
      protected double _linkAxisEndB;
    }

    protected GraphicsObjectCollection m_GraphObjects = new GraphicsObjectCollection();

    protected Altaxo.Graph.PlotItemCollection m_PlotItems;

    //protected PlotGroup.Collection m_PlotGroups = new PlotGroup.Collection();

    /// <summary>
    /// The parent layer collection which contains this layer (or null if not member of such collection).
    /// </summary>
    protected object m_ParentLayerCollection=null;
    //    protected XYPlotLayerCollection m_ParentLayerCollection=null;
  
    /// <summary>
    /// The index inside the parent collection of this layer (or 0 if not member of such collection).
    /// </summary>
    protected int             m_LayerNumber=0;

    /// <summary>
    /// The layer to which this layer is linked to, or null if this layer is not linked.
    /// </summary>
    protected XYPlotLayer           m_LinkedLayer;

    /// <summary>Indicate if x-axis is linked to the linked layer x axis.</summary>
    protected bool            m_LinkXAxis;
  
    /// <summary>The value a of x-axis link for link of origin: org' = a + b*org.</summary>
    protected double          m_LinkXAxisOrgA;
  
    /// <summary>The value b of x-axis link for link of origin: org' = a + b*org.</summary>
    protected double          m_LinkXAxisOrgB;

    /// <summary>The value a of x-axis link for link of end: end' = a + b*end.</summary>
    protected double          m_LinkXAxisEndA;
  
    /// <summary>The value b of x-axis link for link of end: end' = a + b*end.</summary>
    protected double          m_LinkXAxisEndB;

    /// <summary>Indicate if y-axis is linked to the linked layer y axis.</summary>
    protected bool            m_LinkYAxis;

    /// <summary>The value a of y-axis link for link of origin: org' = a + b*org.</summary>
    protected double          m_LinkYAxisOrgA;
  
    /// <summary>The value b of y-axis link for link of origin: org' = a + b*org.</summary>
    protected double          m_LinkYAxisOrgB;
  
    /// <summary>The value a of y-axis link for link of end: end' = a + b*end.</summary>
    protected double          m_LinkYAxisEndA;
  
    /// <summary>The value b of y-axis link for link of end: end' = a + b*end.</summary>
    protected double          m_LinkYAxisEndB;


    /// <summary>Number of times this event is disables, or 0 if it is enabled.</summary>
    int m_PlotAssociationXBoundariesChanged_EventSuspendCount;
    
    /// <summary>Number of times this event is disables, or 0 if it is enabled.</summary>
    int m_PlotAssociationYBoundariesChanged_EventSuspendCount;




    #endregion

    #region Event definitions

    /// <summary>Fired when the size of the layer changed.</summary>
    public event System.EventHandler SizeChanged;
  
    /// <summary>Fired when the position of the layer changed.</summary>
    public event System.EventHandler PositionChanged;
  
    /// <summary>Fired when one of the axis changed its origin or its end.</summary>
    public event System.EventHandler AxesChanged;

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
        XYPlotLayer s = (XYPlotLayer)obj;

      
        // XYPlotLayer style
        info.AddValue("FillLayerArea",s._fillLayerArea);
        info.AddValue("LayerAreaFillBrush",s.m_LayerAreaFillBrush);

        // size, position, rotation and scale
        
        info.AddValue("WidthType",s._layerWidthType);
        info.AddValue("HeightType",s._layerHeightType);
        info.AddValue("Width",s._layerWidth);
        info.AddValue("Height",s._layerHeight);
        info.AddValue("CachedSize",s._cachedLayerSize);

        info.AddValue("XPositionType",s._layerXPositionType);
        info.AddValue("YPositionType",s._layerYPositionType);
        info.AddValue("XPosition",s._layerXPosition);
        info.AddValue("YPosition",s._layerYPosition);
        info.AddValue("CachedPosition",s._cachedLayerPosition);

        info.AddValue("Rotation",s._layerAngle);
        info.AddValue("Scale",s._layerScale);

        // axis related

        info.AddValue("XAxis",s._xAxis);
        info.AddValue("YAxis",s._yAxis);
        info.AddValue("LinkXAxis",s.m_LinkXAxis);
        info.AddValue("LinkYAxis",s.m_LinkYAxis);
        info.AddValue("LinkXAxisOrgA",s.m_LinkXAxisOrgA);
        info.AddValue("LinkXAxisOrgB",s.m_LinkXAxisOrgB);
        info.AddValue("LinkXAxisEndA",s.m_LinkXAxisEndA);
        info.AddValue("LinkXAxisEndB",s.m_LinkXAxisEndB);
        info.AddValue("LinkYAxisOrgA",s.m_LinkYAxisOrgA);
        info.AddValue("LinkYAxisOrgB",s.m_LinkYAxisOrgB);
        info.AddValue("LinkYAxisEndA",s.m_LinkYAxisEndA);
        info.AddValue("LinkYAxisEndB",s.m_LinkYAxisEndB);

      
        // Styles
        info.AddValue("ShowLeftAxis",s._showLeftAxis);
        info.AddValue("ShowBottomAxis",s._showBottomAxis);
        info.AddValue("ShowRightAxis",s._showRightAxis);
        info.AddValue("ShowTopAxis",s._showTopAxis);

        info.AddValue("LeftAxisStyle",s._leftAxisStyle);
        info.AddValue("BottomAxisStyle",s._bottomAxisStyle);
        info.AddValue("RightAxisStyle",s._rightAxisStyle);
        info.AddValue("TopAxisStyle",s._topAxisStyle);
      
      
        info.AddValue("LeftLabelStyle",s._leftLabelStyle);
        info.AddValue("BottomLabelStyle",s._bottomLabelStyle);
        info.AddValue("RightLabelStyle",s._rightLabelStyle);
        info.AddValue("TopLabelStyle",s._topLabelStyle);
      
    
        // Titles and legend
        info.AddValue("LeftAxisTitle",s._leftAxisTitle);
        info.AddValue("BottomAxisTitle",s._bottomAxisTitle);
        info.AddValue("RightAxisTitle",s._rightAxisTitle);
        info.AddValue("TopAxisTitle",s._topAxisTitle);
        info.AddValue("Legend",s._legend);
      
        // XYPlotLayer specific
        info.AddValue("LinkedLayer",s.LinkedLayer);
        info.AddValue("GraphObjects",s.m_GraphObjects);
        info.AddValue("Plots",s.m_PlotItems);



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
        s._fillLayerArea = info.GetBoolean("FillLayerArea");
        s.m_LayerAreaFillBrush = (BrushHolder)info.GetValue("LayerAreaFillBrush",typeof(BrushHolder));



        // size, position, rotation and scale
        
        s._layerWidthType  = (SizeType)info.GetValue("WidthType",typeof(SizeType));
        s._layerHeightType = (SizeType)info.GetValue("HeightType",typeof(SizeType));
        s._layerWidth  = info.GetDouble("Width");
        s._layerHeight = info.GetDouble("Height");
        s._cachedLayerSize   = (SizeF)info.GetValue("CachedSize",typeof(SizeF));

        s._layerXPositionType = (PositionType)info.GetValue("XPositionType",typeof(PositionType));
        s._layerYPositionType = (PositionType)info.GetValue("YPositionType",typeof(PositionType));
        s._layerXPosition = info.GetDouble("XPosition");
        s._layerYPosition = info.GetDouble("YPosition");
        s._cachedLayerPosition = (PointF)info.GetValue("CachedPosition",typeof(PointF));

        s._layerAngle  = info.GetSingle("Rotation");
        s._layerScale = info.GetSingle("Scale");

        // axis related

        s._xAxis = (Axis)info.GetValue("XAxis",typeof(Axis));
        s._yAxis = (Axis)info.GetValue("YAxis",typeof(Axis));
        s.m_LinkXAxis = info.GetBoolean("LinkXAxis");
        s.m_LinkYAxis = info.GetBoolean("LinkYAxis");
        s.m_LinkXAxisOrgA = info.GetDouble("LinkXAxisOrgA");
        s.m_LinkXAxisOrgB = info.GetDouble("LinkXAxisOrgB");
        s.m_LinkXAxisEndA = info.GetDouble("LinkXAxisEndA");
        s.m_LinkXAxisEndB = info.GetDouble("LinkXAxisEndB");
        s.m_LinkYAxisOrgA = info.GetDouble("LinkYAxisOrgA");
        s.m_LinkYAxisOrgB = info.GetDouble("LinkYAxisOrgB");
        s.m_LinkYAxisEndA = info.GetDouble("LinkYAxisEndA");
        s.m_LinkYAxisEndB = info.GetDouble("LinkYAxisEndB");


        // Styles
        s._showLeftAxis = info.GetBoolean("ShowLeftAxis");
        s._showBottomAxis = info.GetBoolean("ShowBottomAxis");
        s._showRightAxis = info.GetBoolean("ShowRightAxis");
        s._showTopAxis = info.GetBoolean("ShowTopAxis");

        s._leftAxisStyle = (Graph.XYAxisStyle)info.GetValue("LeftAxisStyle",typeof(Graph.XYAxisStyle));
        s._bottomAxisStyle = (Graph.XYAxisStyle)info.GetValue("BottomAxisStyle",typeof(Graph.XYAxisStyle));
        s._rightAxisStyle = (Graph.XYAxisStyle)info.GetValue("RightAxisStyle",typeof(Graph.XYAxisStyle));
        s._topAxisStyle = (Graph.XYAxisStyle)info.GetValue("TopAxisStyle",typeof(Graph.XYAxisStyle));
      
      
        s._leftLabelStyle = (Graph.AbstractXYAxisLabelStyle)info.GetValue("LeftLabelStyle",typeof(Graph.AbstractXYAxisLabelStyle));
        s._bottomLabelStyle = (Graph.AbstractXYAxisLabelStyle)info.GetValue("BottomLabelStyle",typeof(Graph.AbstractXYAxisLabelStyle));
        s._rightLabelStyle = (Graph.AbstractXYAxisLabelStyle)info.GetValue("RightLabelStyle",typeof(Graph.AbstractXYAxisLabelStyle));
        s._topLabelStyle = (Graph.AbstractXYAxisLabelStyle)info.GetValue("TopLabelStyle",typeof(Graph.AbstractXYAxisLabelStyle));
      
      
        // Titles and legend
        s._leftAxisTitle = (Graph.TextGraphics)info.GetValue("LeftAxisTitle",typeof(Graph.TextGraphics));
        s._bottomAxisTitle = (Graph.TextGraphics)info.GetValue("BottomAxisTitle",typeof(Graph.TextGraphics));
        s._rightAxisTitle = (Graph.TextGraphics)info.GetValue("RightAxisTitle",typeof(Graph.TextGraphics));
        s._topAxisTitle = (Graph.TextGraphics)info.GetValue("TopAxisTitle",typeof(Graph.TextGraphics));
        s._legend = (Graph.TextGraphics)info.GetValue("Legend",typeof(Graph.TextGraphics));
      
        // XYPlotLayer specific
        s.m_LinkedLayer = (XYPlotLayer)info.GetValue("LinkedLayer",typeof(XYPlotLayer));

        s.m_GraphObjects = (Graph.GraphicsObjectCollection)info.GetValue("GraphObjects",typeof(Graph.GraphicsObjectCollection));

        s.m_PlotItems = (Altaxo.Graph.PlotItemCollection)info.GetValue("Plots",typeof(Altaxo.Graph.PlotItemCollection));

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotLayer),0)]
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        XYPlotLayer s = (XYPlotLayer)obj;
        // XYPlotLayer style
        info.AddValue("FillLayerArea",s._fillLayerArea);
        info.AddValue("LayerAreaFillBrush",s.m_LayerAreaFillBrush);

        // size, position, rotation and scale
        
        info.AddValue("WidthType",s._layerWidthType);
        info.AddValue("HeightType",s._layerHeightType);
        info.AddValue("Width",s._layerWidth);
        info.AddValue("Height",s._layerHeight);
        info.AddValue("CachedSize",s._cachedLayerSize);

        info.AddValue("XPositionType",s._layerXPositionType);
        info.AddValue("YPositionType",s._layerYPositionType);
        info.AddValue("XPosition",s._layerXPosition);
        info.AddValue("YPosition",s._layerYPosition);
        info.AddValue("CachedPosition",s._cachedLayerPosition);

        info.AddValue("Rotation",s._layerAngle);
        info.AddValue("Scale",s._layerScale);

        // axis related

        info.AddValue("XAxis",s._xAxis);
        info.AddValue("YAxis",s._yAxis);
        info.AddValue("LinkXAxis",s.m_LinkXAxis);
        info.AddValue("LinkYAxis",s.m_LinkYAxis);
        info.AddValue("LinkXAxisOrgA",s.m_LinkXAxisOrgA);
        info.AddValue("LinkXAxisOrgB",s.m_LinkXAxisOrgB);
        info.AddValue("LinkXAxisEndA",s.m_LinkXAxisEndA);
        info.AddValue("LinkXAxisEndB",s.m_LinkXAxisEndB);
        info.AddValue("LinkYAxisOrgA",s.m_LinkYAxisOrgA);
        info.AddValue("LinkYAxisOrgB",s.m_LinkYAxisOrgB);
        info.AddValue("LinkYAxisEndA",s.m_LinkYAxisEndA);
        info.AddValue("LinkYAxisEndB",s.m_LinkYAxisEndB);

      
        // Styles
        info.AddValue("ShowLeftAxis",s._showLeftAxis);
        info.AddValue("ShowBottomAxis",s._showBottomAxis);
        info.AddValue("ShowRightAxis",s._showRightAxis);
        info.AddValue("ShowTopAxis",s._showTopAxis);

        info.AddValue("LeftAxisStyle",s._leftAxisStyle);
        info.AddValue("BottomAxisStyle",s._bottomAxisStyle);
        info.AddValue("RightAxisStyle",s._rightAxisStyle);
        info.AddValue("TopAxisStyle",s._topAxisStyle);
      
      
        info.AddValue("LeftLabelStyle",s._leftLabelStyle);
        info.AddValue("BottomLabelStyle",s._bottomLabelStyle);
        info.AddValue("RightLabelStyle",s._rightLabelStyle);
        info.AddValue("TopLabelStyle",s._topLabelStyle);
      
    
        // Titles and legend
        info.AddValue("LeftAxisTitle",s._leftAxisTitle);
        info.AddValue("BottomAxisTitle",s._bottomAxisTitle);
        info.AddValue("RightAxisTitle",s._rightAxisTitle);
        info.AddValue("TopAxisTitle",s._topAxisTitle);
        info.AddValue("Legend",s._legend);
      
        // XYPlotLayer specific
        info.AddValue("LinkedLayer", null!=s.m_LinkedLayer ? Main.DocumentPath.GetRelativePathFromTo(s,s.m_LinkedLayer) : null);
      
        info.AddValue("GraphicsObjectCollection",s.m_GraphObjects);
        info.AddValue("Plots",s.m_PlotItems);


      }

      protected XYPlotLayer _Layer;
      protected Main.DocumentPath _LinkedLayerPath;

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        XYPlotLayer s = null!=o ? (XYPlotLayer)o : new XYPlotLayer();

        // XYPlotLayer style
        s._fillLayerArea = info.GetBoolean("FillLayerArea");
        s.m_LayerAreaFillBrush = (BrushHolder)info.GetValue("LayerAreaFillBrush",typeof(BrushHolder));



        // size, position, rotation and scale
        
        s._layerWidthType  = (SizeType)info.GetValue("WidthType",typeof(SizeType));
        s._layerHeightType = (SizeType)info.GetValue("HeightType",typeof(SizeType));
        s._layerWidth  = info.GetDouble("Width");
        s._layerHeight = info.GetDouble("Height");
        s._cachedLayerSize   = (SizeF)info.GetValue("CachedSize",typeof(SizeF));

        s._layerXPositionType = (PositionType)info.GetValue("XPositionType",typeof(PositionType));
        s._layerYPositionType = (PositionType)info.GetValue("YPositionType",typeof(PositionType));
        s._layerXPosition = info.GetDouble("XPosition");
        s._layerYPosition = info.GetDouble("YPosition");
        s._cachedLayerPosition = (PointF)info.GetValue("CachedPosition",typeof(PointF));

        s._layerAngle  = info.GetSingle("Rotation");
        s._layerScale = info.GetSingle("Scale");

        // axis related

        s._xAxis = (Axis)info.GetValue("XAxis",typeof(Axis));
        s._yAxis = (Axis)info.GetValue("YAxis",typeof(Axis));
        s.m_LinkXAxis = info.GetBoolean("LinkXAxis");
        s.m_LinkYAxis = info.GetBoolean("LinkYAxis");
        s.m_LinkXAxisOrgA = info.GetDouble("LinkXAxisOrgA");
        s.m_LinkXAxisOrgB = info.GetDouble("LinkXAxisOrgB");
        s.m_LinkXAxisEndA = info.GetDouble("LinkXAxisEndA");
        s.m_LinkXAxisEndB = info.GetDouble("LinkXAxisEndB");
        s.m_LinkYAxisOrgA = info.GetDouble("LinkYAxisOrgA");
        s.m_LinkYAxisOrgB = info.GetDouble("LinkYAxisOrgB");
        s.m_LinkYAxisEndA = info.GetDouble("LinkYAxisEndA");
        s.m_LinkYAxisEndB = info.GetDouble("LinkYAxisEndB");


        // Styles
        s._showLeftAxis = info.GetBoolean("ShowLeftAxis");
        s._showBottomAxis = info.GetBoolean("ShowBottomAxis");
        s._showRightAxis = info.GetBoolean("ShowRightAxis");
        s._showTopAxis = info.GetBoolean("ShowTopAxis");

        s._leftAxisStyle = (Graph.XYAxisStyle)info.GetValue("LeftAxisStyle",typeof(Graph.XYAxisStyle));
        s._bottomAxisStyle = (Graph.XYAxisStyle)info.GetValue("BottomAxisStyle",typeof(Graph.XYAxisStyle));
        s._rightAxisStyle = (Graph.XYAxisStyle)info.GetValue("RightAxisStyle",typeof(Graph.XYAxisStyle));
        s._topAxisStyle = (Graph.XYAxisStyle)info.GetValue("TopAxisStyle",typeof(Graph.XYAxisStyle));
      
      
        s._leftLabelStyle = (Graph.AbstractXYAxisLabelStyle)info.GetValue("LeftLabelStyle",typeof(Graph.AbstractXYAxisLabelStyle));
        s._bottomLabelStyle = (Graph.AbstractXYAxisLabelStyle)info.GetValue("BottomLabelStyle",typeof(Graph.AbstractXYAxisLabelStyle));
        s._rightLabelStyle = (Graph.AbstractXYAxisLabelStyle)info.GetValue("RightLabelStyle",typeof(Graph.AbstractXYAxisLabelStyle));
        s._topLabelStyle = (Graph.AbstractXYAxisLabelStyle)info.GetValue("TopLabelStyle",typeof(Graph.AbstractXYAxisLabelStyle));
      
      
        // Titles and legend
        s._leftAxisTitle = (Graph.TextGraphics)info.GetValue("LeftAxisTitle",typeof(Graph.TextGraphics));
        s._bottomAxisTitle = (Graph.TextGraphics)info.GetValue("BottomAxisTitle",typeof(Graph.TextGraphics));
        s._rightAxisTitle = (Graph.TextGraphics)info.GetValue("RightAxisTitle",typeof(Graph.TextGraphics));
        s._topAxisTitle = (Graph.TextGraphics)info.GetValue("TopAxisTitle",typeof(Graph.TextGraphics));
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

        s.m_GraphObjects = (Graph.GraphicsObjectCollection)info.GetValue("GraphObjects",typeof(Graph.GraphicsObjectCollection));

        s.m_PlotItems = (Altaxo.Graph.PlotItemCollection)info.GetValue("Plots",typeof(Altaxo.Graph.PlotItemCollection));
    
        s.CalculateMatrix();
        s.CreateEventLinks();

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


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotLayer),1)]
      public class XmlSerializationSurrogate1 : XmlSerializationSurrogate0
    {
      public new void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        base.Serialize(obj, info);

        XYPlotLayer s = (XYPlotLayer)obj;
        // XYPlotLayer style
        info.AddValue("ClipDataToFrame",s._clipDataToFrame);
      }

      public new object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        
        XYPlotLayer s = null!=o ? (XYPlotLayer)o : new XYPlotLayer();
        base.Deserialize(o,info,parent);

        s._clipDataToFrame = info.GetBoolean("ClipDataToFrame");

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
      this._fillLayerArea = from._fillLayerArea;
      this.m_LayerAreaFillBrush = null==from.m_LayerAreaFillBrush ? null : (BrushHolder)from.m_LayerAreaFillBrush.Clone();

      // size, position, rotation and scale
      this._layerWidthType  = from._layerWidthType;
      this._layerHeightType = from._layerHeightType;
      this._layerWidth  = from._layerWidth;
      this._layerHeight = from._layerHeight ;
      this._cachedLayerSize   = from._cachedLayerSize;

      this._LogicalToAreaConverter = new LogicalToAreaConverter(this);
      this._AreaToLogicalConverter = new AreaToLogicalConverter(this);

      this._layerXPositionType = from._layerXPositionType;
      this._layerYPositionType = from._layerYPositionType;
      this._layerXPosition = from._layerXPosition ;
      this._layerYPosition = from._layerYPosition;
      this._cachedLayerPosition = from._cachedLayerPosition;

      this._layerAngle  =from._layerAngle;
      this._layerScale = from._layerScale;

      // axis related

      this._xAxis = null==from._xAxis ? null : (Axis)from._xAxis.Clone() ;
      this._yAxis = null==from._yAxis ? null : (Axis)from._yAxis.Clone() ;
      this.m_LinkXAxis = from.m_LinkXAxis;
      this.m_LinkYAxis = from.m_LinkYAxis;
      this.m_LinkXAxisOrgA = from.m_LinkXAxisOrgA;
      this.m_LinkXAxisOrgB =  from.m_LinkXAxisOrgB;
      this.m_LinkXAxisEndA = from.m_LinkXAxisEndA;
      this.m_LinkXAxisEndB =from.m_LinkXAxisEndB;
      this.m_LinkYAxisOrgA = from.m_LinkYAxisOrgA;
      this.m_LinkYAxisOrgB = from.m_LinkYAxisOrgB;
      this.m_LinkYAxisEndA = from.m_LinkYAxisEndA ;
      this.m_LinkYAxisEndB = from.m_LinkYAxisEndB;


      // Styles
      this._showLeftAxis = from._showLeftAxis;
      this._showBottomAxis = from._showBottomAxis;
      this._showRightAxis = from._showRightAxis;
      this._showTopAxis = from._showTopAxis;

      this._leftAxisStyle = null==from._leftAxisStyle ? null : (Graph.XYAxisStyle)from._leftAxisStyle.Clone();
      this._bottomAxisStyle = null==from._bottomAxisStyle ? null : (Graph.XYAxisStyle)from._bottomAxisStyle.Clone();
      this._rightAxisStyle = null==from._rightAxisStyle ? null : (Graph.XYAxisStyle)from._rightAxisStyle.Clone();
      this._topAxisStyle = null==from._topAxisStyle ? null : (Graph.XYAxisStyle)from._topAxisStyle.Clone();
      
      
      this._leftLabelStyle = null==from._leftLabelStyle ? null : (Graph.AbstractXYAxisLabelStyle)from._leftLabelStyle.Clone();
      this._bottomLabelStyle = null==from._bottomLabelStyle ? null : (Graph.AbstractXYAxisLabelStyle)from._bottomLabelStyle.Clone();
      this._rightLabelStyle = null==from._rightLabelStyle ? null : (Graph.AbstractXYAxisLabelStyle)from._rightLabelStyle.Clone();
      this._topLabelStyle = null==from._topLabelStyle ? null : (Graph.AbstractXYAxisLabelStyle)from._topLabelStyle.Clone();
      
      
      // Titles and legend
      this._leftAxisTitle = null==from._leftAxisTitle ? null : (Graph.TextGraphics)from._leftAxisTitle.Clone();
      this._bottomAxisTitle = null==from._bottomAxisTitle ? null : (Graph.TextGraphics)from._bottomAxisTitle.Clone();
      this._rightAxisTitle = null==from._rightAxisTitle ? null : (Graph.TextGraphics)from._rightAxisTitle.Clone();
      this._topAxisTitle = null==from._topAxisTitle ? null : (Graph.TextGraphics)from._topAxisTitle.Clone();
      this._legend = null==from._legend ? null : (Graph.TextGraphics)from._legend.Clone();
      
      // XYPlotLayer specific
      this.m_LinkedLayer = from.m_LinkedLayer; // do not clone here, parent collection's duty to fix this!
      
      this.m_GraphObjects = null==from.m_GraphObjects ? null : new GraphicsObjectCollection(from.m_GraphObjects);

      this.m_PlotItems = null==from.m_PlotItems ? null : new Altaxo.Graph.PlotItemCollection(this,from.m_PlotItems);

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

      m_PlotItems = new Altaxo.Graph.PlotItemCollection(this);

      // create axes and add event handlers to them
      _xAxis = new LinearAxis(); // the X-Axis
      _yAxis = new LinearAxis(); // the Y-Axis

    
    
      LeftAxisTitleString = "Y axis";
      BottomAxisTitleString = "X axis";

      CreateEventLinks();
    
    }

  
  
    #endregion

    #region XYPlotLayer properties and methods


    void CreateEventLinks()
    {
      // restore the event chain
      if(null!=_xAxis) _xAxis.Changed += new EventHandler(this.OnXAxisChangedEventHandler);
      if(null!=_yAxis) _yAxis.Changed += new EventHandler(this.OnYAxisChangedEventHandler);

      if(null!=_leftAxisStyle) _leftAxisStyle.Changed += new EventHandler(this.OnChildChangedEventHandler);
      if(null!=_bottomAxisStyle) _bottomAxisStyle.Changed += new EventHandler(this.OnChildChangedEventHandler);
      if(null!=_rightAxisStyle) _rightAxisStyle.Changed += new EventHandler(this.OnChildChangedEventHandler);
      if(null!=_topAxisStyle) _topAxisStyle.Changed += new EventHandler(this.OnChildChangedEventHandler);

      if(null!=_leftLabelStyle) _leftLabelStyle.Changed += new EventHandler(this.OnChildChangedEventHandler);
      if(null!=_bottomLabelStyle) _bottomLabelStyle.Changed += new EventHandler(this.OnChildChangedEventHandler);
      if(null!=_rightLabelStyle) _rightLabelStyle.Changed += new EventHandler(this.OnChildChangedEventHandler);
      if(null!=_topLabelStyle) _topLabelStyle.Changed += new EventHandler(this.OnChildChangedEventHandler);

    
      if(null!=_leftAxisTitle) _leftAxisTitle.Changed += new EventHandler(this.OnChildChangedEventHandler);
      if(null!=_bottomAxisTitle) _bottomAxisTitle.Changed += new EventHandler(this.OnChildChangedEventHandler);
      if(null!=_rightAxisTitle) _rightAxisTitle.Changed += new EventHandler(this.OnChildChangedEventHandler);
      if(null!=_topAxisTitle) _topAxisTitle.Changed += new EventHandler(this.OnChildChangedEventHandler);

      if(null!=_legend) _legend.Changed += new EventHandler(this.OnChildChangedEventHandler);

      if(null!=m_LinkedLayer) m_LinkedLayer.AxesChanged += new EventHandler(this.OnLinkedLayerAxesChanged);
    
      if(null!=m_GraphObjects) m_GraphObjects.Changed += new EventHandler(this.OnChildChangedEventHandler);

      //if(null!=m_PlotGroups) m_PlotGroups.Changed += new EventHandler(this.OnChildChangedEventHandler);

      if(null!=m_PlotItems)
      {
        m_PlotItems.SetParentLayer(this,true); // sets the parent layer, but suppresses the events following this.
        m_PlotItems.Changed += new EventHandler(this.OnChildChangedEventHandler);
      }
    }

    /// <summary>
    /// The layer number.
    /// </summary>
    /// <value>The layer number, i.e. the position of the layer in the layer collection.</value>
    public int Number
    {
      get { return this.m_LayerNumber; } 
    }

    
    public XYPlotLayerCollection ParentLayerList
    {
      get { return m_ParentLayerCollection as XYPlotLayerCollection; }
    }

    public GraphicsObjectCollection GraphObjects
    {
      get { return m_GraphObjects; }
    }

    public void Remove(GraphicsObject go)
    {
      // test our own objects for removal (only that that _are_ removable)
      if(object.ReferenceEquals(go,this._leftAxisTitle))
        _leftAxisTitle=null;
      else if(object.ReferenceEquals(go,this._topAxisTitle))
        _topAxisTitle=null;
      else if(object.ReferenceEquals(go,this._rightAxisTitle))
        _rightAxisTitle=null;
      else if(object.ReferenceEquals(go,this._bottomAxisTitle))
        _bottomAxisTitle=null;
      else if(object.ReferenceEquals(go,this._legend))
        _legend=null;
      else if(m_GraphObjects.Contains(go))
        m_GraphObjects.Remove(go);

    }

    /// <summary>
    /// Get / sets the layer this layer is linked to.
    /// </summary>
    /// <value>The layer this layer is linked to, or null if not linked.</value>
    public XYPlotLayer LinkedLayer
    {
      get { return m_LinkedLayer; }
      set
      {

        // ignore the value if it would create a circular dependency
        if(IsLayerDependentOnMe(value))
          return;


        XYPlotLayer oldValue = this.m_LinkedLayer;
        m_LinkedLayer =  value;

        if(!ReferenceEquals(oldValue,m_LinkedLayer))
        {
          // close the event handlers to the old layer
          if(null!=oldValue)
          {
            oldValue.SizeChanged -= new System.EventHandler(OnLinkedLayerSizeChanged);
            oldValue.PositionChanged -= new System.EventHandler(OnLinkedLayerPositionChanged);
            oldValue.AxesChanged -= new System.EventHandler(OnLinkedLayerAxesChanged);
          }

          // link the events to the new layer
          if(null!=m_LinkedLayer)
          {
            m_LinkedLayer.SizeChanged     += new System.EventHandler(OnLinkedLayerSizeChanged);
            m_LinkedLayer.PositionChanged += new System.EventHandler(OnLinkedLayerPositionChanged);
            m_LinkedLayer.AxesChanged     += new System.EventHandler(OnLinkedLayerAxesChanged);
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
      get { return null!=m_LinkedLayer; }
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
      m_ParentLayerCollection = lc;
      m_LayerNumber = number;
      
      if(m_ParentLayerCollection==null)
        m_LinkedLayer=null;
    }


    public Altaxo.Graph.PlotItemCollection PlotItems
    {
      get { return m_PlotItems; }
    }

    /*
    public PlotGroup.Collection PlotGroups
    {
      get { return m_PlotGroups; }
    }
    */

    public void AddPlotAssociation(XYColumnPlotData[] pal)
    {
      foreach(XYColumnPlotData pa in pal)
        this.m_PlotItems.Add(new XYColumnPlotItem(pa,new XYPlotStyleCollection()));
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

    /// <summary>
    /// Retrieves the description of the enumeration value <b>value</b>.
    /// </summary>
    /// <param name="value">The enumeration value.</param>
    /// <returns>The description of this value. If no description is available, the ToString() method is used
    /// to return the name of the value.</returns>
    public static string GetDescription(Enum value)
    {
      FieldInfo fi= value.GetType().GetField(value.ToString()); 
      DescriptionAttribute[] attributes = 
        (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
      return (attributes.Length>0)?attributes[0].Description:value.ToString();
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
        
        if(this._layerXPositionType == PositionType.AbsoluteValue)
          this._layerXPosition = xoffs + this._layerXPosition*xscale;

        if(this._layerWidthType==SizeType.AbsoluteValue)
          this._layerWidth *= xscale;

        if(this._layerYPositionType == PositionType.AbsoluteValue)
          this._layerYPosition = yoffs + this._layerYPosition*yscale;

        if(this._layerHeightType == SizeType.AbsoluteValue)
          this._layerHeight *= yscale;

        CalculateMatrix();
        this.CalculateCachedSize();

        // scale the position of the inner items according to the ratio of the new size to the old size
        // note: only the size is important here, since all inner items are relative to the layer origin
        SizeF     newLayerSize     = this._cachedLayerSize;
        xscale = newLayerSize.Width/oldLayerSize.Width;
        yscale = newLayerSize.Height/oldLayerSize.Height;
        GraphicsObject.ScalePosition(this._leftAxisTitle,xscale,yscale);
        GraphicsObject.ScalePosition(this._bottomAxisTitle,xscale,yscale);
        GraphicsObject.ScalePosition(this._rightAxisTitle,xscale,yscale);
        GraphicsObject.ScalePosition(this._topAxisTitle,xscale,yscale);
        GraphicsObject.ScalePosition(this._legend,xscale,yscale);
        this.m_GraphObjects.ScalePosition(xscale,yscale);
      }
    }

    public PointF Position
    {
      get { return this._cachedLayerPosition; }
      set
      {
        SetPosition(value.X,PositionType.AbsoluteValue,value.Y,PositionType.AbsoluteValue);
      }
    }

    public SizeF Size
    {
      get { return this._cachedLayerSize; }
      set
      {
        SetSize(value.Width,SizeType.AbsoluteValue, value.Height,SizeType.AbsoluteValue);
      }
    }

    public float Rotation
    {
      get { return this._layerAngle; }
      set
      {
        this._layerAngle = value;
        this.CalculateMatrix();
        this.OnInvalidate();
      }
    }

    public float Scale
    {
      get { return this._layerScale; }
      set
      {
        this._layerScale = value;
        this.CalculateMatrix();
        this.OnInvalidate();
      }
    }

    protected void CalculateMatrix()
    {
      _cachedForwardMatrix.Reset();
      _cachedForwardMatrix.Translate(_cachedLayerPosition.X,_cachedLayerPosition.Y);
      _cachedForwardMatrix.Scale(_layerScale,_layerScale);
      _cachedForwardMatrix.Rotate(_layerAngle);
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
    /// Transforms a <see>PointF</see> from layer coordinates to graph (=printable area) coordinates
    /// </summary>
    /// <param name="layerCoordinates">The layer coordinates to convert.</param>
    /// <returns>graphics path now in graph coordinates</returns>
    public PointF LayerToGraphCoordinates(PointF layerCoordinates)
    {
      PointF[]result = new PointF[]{layerCoordinates}; 
      _cachedForwardMatrix.TransformPoints(result);
      return result[0];
    }



    public void SetPosition(double x, PositionType xpostype, double y, PositionType ypostype)
    {
      this._layerXPosition = x;
      this._layerXPositionType = xpostype;
      this._layerYPosition = y;
      this._layerYPositionType = ypostype;

      CalculateCachedPosition();
    }

    /// <summary>
    /// Calculates from the x position value, which can be absolute or relative, the
    /// x position in points.
    /// </summary>
    /// <param name="x">The horizontal position value of type xpostype.</param>
    /// <param name="xpostype">The type of the horizontal position value, see <see cref="PositionType"/>.</param>
    /// <returns>Calculated absolute position of the layer in units of points (1/72 inch).</returns>
    /// <remarks>The function does not change the member variables of the layer and can therefore used
    /// for position calculations without changing the layer. The function is not static because it has to use either the parent
    /// graph or the linked layer for the calculations.</remarks>
    public double XPositionToPointUnits(double x, PositionType xpostype)
    {
      switch(xpostype)
      {
        case PositionType.AbsoluteValue:
          break;
        case PositionType.RelativeToGraphDocument:
          x = x*PrintableGraphSize.Width;
          break;
        case PositionType.RelativeThisNearToLinkedLayerNear:
          if(LinkedLayer!=null)
            x = LinkedLayer.Position.X + x*LinkedLayer.Size.Width;
          break;
        case PositionType.RelativeThisNearToLinkedLayerFar:
          if(LinkedLayer!=null)
            x = LinkedLayer.Position.X + (1+x)*LinkedLayer.Size.Width;
          break;
        case PositionType.RelativeThisFarToLinkedLayerNear:
          if(LinkedLayer!=null)
            x = LinkedLayer.Position.X - this.Size.Width + x*LinkedLayer.Size.Width;
          break;
        case PositionType.RelativeThisFarToLinkedLayerFar:
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
    /// <param name="ypostype">The type of the vertical position value, see <see cref="PositionType"/>.</param>
    /// <returns>Calculated absolute position of the layer in units of points (1/72 inch).</returns>
    /// <remarks>The function does not change the member variables of the layer and can therefore used
    /// for position calculations without changing the layer. The function is not static because it has to use either the parent
    /// graph or the linked layer for the calculations.</remarks>
    public double YPositionToPointUnits(double y, PositionType ypostype)
    {
      switch(ypostype)
      {
        case PositionType.AbsoluteValue:
          break;
        case PositionType.RelativeToGraphDocument:
          y = y*PrintableGraphSize.Height;
          break;
        case PositionType.RelativeThisNearToLinkedLayerNear:
          if(LinkedLayer!=null)
            y = LinkedLayer.Position.Y + y*LinkedLayer.Size.Height;
          break;
        case PositionType.RelativeThisNearToLinkedLayerFar:
          if(LinkedLayer!=null)
            y = LinkedLayer.Position.Y + (1+y)*LinkedLayer.Size.Height;
          break;
        case PositionType.RelativeThisFarToLinkedLayerNear:
          if(LinkedLayer!=null)
            y = LinkedLayer.Position.Y - this.Size.Height + y*LinkedLayer.Size.Height;
          break;
        case PositionType.RelativeThisFarToLinkedLayerFar:
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
    /// <param name="xpostype_to_convert_to">The type of the vertical position value to convert to, see <see cref="PositionType"/>.</param>
    /// <returns>Calculated value of x in user units.</returns>
    /// <remarks>The function does not change the member variables of the layer and can therefore used
    /// for position calculations without changing the layer. The function is not static because it has to use either the parent
    /// graph or the linked layer for the calculations.</remarks>
    public double XPositionToUserUnits(double x, PositionType xpostype_to_convert_to)
    {

  
      switch(xpostype_to_convert_to)
      {
        case PositionType.AbsoluteValue:
          break;
        case PositionType.RelativeToGraphDocument:
          x = x/PrintableGraphSize.Width;
          break;
        case PositionType.RelativeThisNearToLinkedLayerNear:
          if(LinkedLayer!=null)
            x = (x-LinkedLayer.Position.X)/LinkedLayer.Size.Height;
          break;
        case PositionType.RelativeThisNearToLinkedLayerFar:
          if(LinkedLayer!=null)
            x = (x-LinkedLayer.Position.X)/LinkedLayer.Size.Width - 1;
          break;
        case PositionType.RelativeThisFarToLinkedLayerNear:
          if(LinkedLayer!=null)
            x = (x-LinkedLayer.Position.X + this.Size.Width)/LinkedLayer.Size.Width;
          break;
        case PositionType.RelativeThisFarToLinkedLayerFar:
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
    /// <param name="ypostype_to_convert_to">The type of the vertical position value to convert to, see <see cref="PositionType"/>.</param>
    /// <returns>Calculated value of y in user units.</returns>
    /// <remarks>The function does not change the member variables of the layer and can therefore used
    /// for position calculations without changing the layer. The function is not static because it has to use either the parent
    /// graph or the linked layer for the calculations.</remarks>
    public double YPositionToUserUnits(double y, PositionType ypostype_to_convert_to)
    {
      switch(ypostype_to_convert_to)
      {
        case PositionType.AbsoluteValue:
          break;
        case PositionType.RelativeToGraphDocument:
          y = y/PrintableGraphSize.Height;
          break;
        case PositionType.RelativeThisNearToLinkedLayerNear:
          if(LinkedLayer!=null)
            y = (y-LinkedLayer.Position.Y)/LinkedLayer.Size.Height;
          break;
        case PositionType.RelativeThisNearToLinkedLayerFar:
          if(LinkedLayer!=null)
            y = (y-LinkedLayer.Position.Y)/LinkedLayer.Size.Height - 1;
          break;
        case PositionType.RelativeThisFarToLinkedLayerNear:
          if(LinkedLayer!=null)
            y = (y-LinkedLayer.Position.Y + this.Size.Height)/LinkedLayer.Size.Height;
          break;
        case PositionType.RelativeThisFarToLinkedLayerFar:
          if(LinkedLayer!=null)
            y = (y-LinkedLayer.Position.Y + this.Size.Height)/LinkedLayer.Size.Height - 1;
          break;
      }

      return y;
    }


    /// <summary>
    /// Sets the cached position value in <see cref="_cachedLayerPosition"/> by calculating it
    /// from the position values (<see cref="_layerXPosition"/> and <see cref="_layerYPosition"/>) 
    /// and the position types (<see cref="_layerXPositionType"/> and <see cref="_layerYPositionType"/>).
    /// </summary>
    protected void CalculateCachedPosition()
    {
      PointF newPos = new PointF(
        (float)XPositionToPointUnits(this._layerXPosition,this._layerXPositionType),
        (float)YPositionToPointUnits(this._layerYPosition, this._layerYPositionType));
      if(newPos != this._cachedLayerPosition)
      {
        this._cachedLayerPosition=newPos;
        this.CalculateMatrix();
        OnPositionChanged();
      }
    }


    public void SetSize(double width, SizeType widthtype, double height, SizeType heighttype)
    {
      this._layerWidth = width;
      this._layerWidthType = widthtype;
      this._layerHeight = height;
      this._layerHeightType = heighttype;

      CalculateCachedSize();
    }


    protected double WidthToPointUnits(double width, SizeType widthtype)
    {
      switch(widthtype)
      {
        case SizeType.RelativeToGraphDocument:
          width *= PrintableGraphSize.Width;
          break;
        case SizeType.RelativeToLinkedLayer:
          if(null!=LinkedLayer)
            width *= LinkedLayer.Size.Width;
          break;
      }
      return width;
    }

    protected double HeightToPointUnits(double height, SizeType heighttype)
    {
      switch(heighttype)
      {
        case SizeType.RelativeToGraphDocument:
          height *= PrintableGraphSize.Height;
          break;
        case SizeType.RelativeToLinkedLayer:
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
    protected double WidthToUserUnits(double width, SizeType widthtype_to_convert_to)
    {
  
      switch(widthtype_to_convert_to)
      {
        case SizeType.RelativeToGraphDocument:
          width /= PrintableGraphSize.Width;
          break;
        case SizeType.RelativeToLinkedLayer:
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
    protected double HeightToUserUnits(double height, SizeType heighttype_to_convert_to)
    {

      switch(heighttype_to_convert_to)
      {
        case SizeType.RelativeToGraphDocument:
          height /= PrintableGraphSize.Height;
          break;
        case SizeType.RelativeToLinkedLayer:
          if(null!=LinkedLayer)
            height /= LinkedLayer.Size.Height;
          break;
      }
      return height;
    }


    /// <summary>
    /// Sets the cached size value in <see cref="_cachedLayerSize"/> by calculating it
    /// from the position values (<see cref="_layerWidth"/> and <see cref="_layerHeight"/>) 
    /// and the size types (<see cref="_layerWidthType"/> and <see cref="_layerHeightType"/>).
    /// </summary>
    protected void CalculateCachedSize()
    {
      SizeF newSize = new SizeF(
        (float)WidthToPointUnits(this._layerWidth,this._layerWidthType),
        (float)HeightToPointUnits(this._layerHeight, this._layerHeightType));
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
      get { return this._layerXPosition; }
    }

    /// <summary>Returns the user y position value of the layer.</summary>
    /// <value>User y position value of the layer.</value>
    public double UserYPosition
    {
      get { return this._layerYPosition; }
    }

    /// <summary>Returns the user width value of the layer.</summary>
    /// <value>User width value of the layer.</value>
    public double UserWidth
    {
      get { return this._layerWidth; }
    }

    /// <summary>Returns the user height value of the layer.</summary>
    /// <value>User height value of the layer.</value>
    public double UserHeight
    {
      get { return this._layerHeight; }
    }

    /// <summary>Returns the type of the user x position value of the layer.</summary>
    /// <value>Type of the user x position value of the layer.</value>
    public PositionType UserXPositionType
    {
      get { return this._layerXPositionType; }
    }

    /// <summary>Returns the type of the user y position value of the layer.</summary>
    /// <value>Type of the User y position value of the layer.</value>
    public PositionType UserYPositionType
    {
      get { return this._layerYPositionType; }
    }

    /// <summary>Returns the type of the the user width value of the layer.</summary>
    /// <value>Type of the User width value of the layer.</value>
    public SizeType UserWidthType
    {
      get { return this._layerWidthType; }
    }

    /// <summary>Returns the the type of the user height value of the layer.</summary>
    /// <value>Type of the User height value of the layer.</value>
    public SizeType UserHeightType
    {
      get { return this._layerHeightType; }
    }



    /// <summary>
    /// Measures to do when the position of the linked layer changed.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The event args.</param>
    protected void OnLinkedLayerPositionChanged(object sender, System.EventArgs e)
    {
      CalculateCachedPosition();
    }

    /// <summary>
    /// Measures to do when the size of the linked layer changed.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The event args.</param>
    protected void OnLinkedLayerSizeChanged(object sender, System.EventArgs e)
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
      get { return _xAxis; }
      set
      {
        if(!object.ReferenceEquals(_xAxis,value))
        {
          if(null!=_xAxis)
            _xAxis.Changed -= new System.EventHandler(this.OnXAxisChangedEventHandler);
        
          _xAxis = value;

          if(null!=_xAxis)
            _xAxis.Changed += new System.EventHandler(this.OnXAxisChangedEventHandler);


          // now we have to inform all the PlotItems that a new axis was loaded
          RescaleXAxis();
        }
      }
    }

    public void RescaleXAxis()
    {
      // we have to disable our own Handler since if we change one DataBound of a association,
      //it generates a OnBoundaryChanged, and then all boundaries are merges into the axis boundary, 
      //but (alas!) not all boundaries are now of the new type!
      m_PlotAssociationXBoundariesChanged_EventSuspendCount++; 
        
      _xAxis.DataBoundsObject.BeginUpdate(); // Suppress events from the y-axis now
      _xAxis.DataBoundsObject.Reset();
      foreach(PlotItem pa in this.PlotItems)
      {
        if(pa is IXBoundsHolder)
        {
          // first ensure the right data bound object is set on the XYColumnPlotData
          ((IXBoundsHolder)pa).SetXBoundsFromTemplate(_xAxis.DataBoundsObject); // ensure that data bound object is of the right type
          // now merge the bounds with x and yAxis
          ((IXBoundsHolder)pa).MergeXBoundsInto(_xAxis.DataBoundsObject); // merge all x-boundaries in the x-axis boundary object
        
        }
      }
      m_PlotAssociationXBoundariesChanged_EventSuspendCount = Math.Max(0,m_PlotAssociationXBoundariesChanged_EventSuspendCount-1);
      _xAxis.DataBoundsObject.EndUpdate();
      _xAxis.ProcessDataBounds();
    }
   
  
    /// <summary>Gets or sets the y axis of this layer.</summary>
    /// <value>The y axis of the layer.</value>
    public Axis YAxis
    {
      get { return _yAxis; }
      set
      {
        if(!object.ReferenceEquals(_yAxis,value))
        {
          if(null!=_yAxis)
            _yAxis.Changed -= new System.EventHandler(this.OnYAxisChangedEventHandler);
        
          _yAxis = value;

          if(null!=_yAxis)
            _yAxis.Changed += new System.EventHandler(this.OnYAxisChangedEventHandler);


          // now we have to inform all the PlotAssociations that a new axis was loaded
          RescaleYAxis();
        }
      }
    }


    public void RescaleYAxis()
    {
      // we have to disable our own Handler since if we change one DataBound of a association,
      //it generates a OnBoundaryChanged, and then all boundaries are merges into the axis boundary, 
      //but (alas!) not all boundaries are now of the new type!
      m_PlotAssociationYBoundariesChanged_EventSuspendCount++; 

      _yAxis.DataBoundsObject.BeginUpdate();
      _yAxis.DataBoundsObject.Reset();
      foreach(PlotItem pa in this.PlotItems)
      {
        if(pa is IYBoundsHolder)
        {
          // first ensure the right data bound object is set on the XYColumnPlotData
          ((IYBoundsHolder)pa).SetYBoundsFromTemplate(_yAxis.DataBoundsObject); // ensure that data bound object is of the right type
          // now merge the bounds with x and yAxis
          ((IYBoundsHolder)pa).MergeYBoundsInto(_yAxis.DataBoundsObject); // merge all x-boundaries in the x-axis boundary object
        
        }
      }
      m_PlotAssociationYBoundariesChanged_EventSuspendCount = Math.Max(0,m_PlotAssociationYBoundariesChanged_EventSuspendCount-1);
      _yAxis.DataBoundsObject.EndUpdate();
      _yAxis.ProcessDataBounds();
    }
    

    /// <summary>Indicates if x axis is linked to the linked layer x axis.</summary>
    /// <value>True if x axis is linked to the linked layer x axis.</value>
    public bool IsXAxisLinked
    {
      get { return this.m_LinkXAxis; }
      set
      {
        bool oldValue = this.m_LinkXAxis;
        m_LinkXAxis = value;
        if(value!=oldValue && value==true)
        {
          if(null!=LinkedLayer)
            OnLinkedLayerAxesChanged(LinkedLayer, new System.EventArgs());
        }
      }
    }

  
    /// <summary>Indicates if y axis is linked to the linked layer y axis.</summary>
    /// <value>True if y axis is linked to the linked layer y axis.</value>
    public bool IsYAxisLinked
    {
      get { return this.m_LinkYAxis; }
      set
      {
        bool oldValue = this.m_LinkYAxis;
        m_LinkYAxis = value;
        if(value!=oldValue && value==true)
        {
          if(null!=LinkedLayer)
            OnLinkedLayerAxesChanged(LinkedLayer, new System.EventArgs());
        }
      }
    }


    /// <summary>The type of x axis link.</summary>
    /// <value>Can be either None, Straight or Custom link.</value>
    public AxisLinkType XAxisLinkType
    {
      get 
      {
        if(!this.m_LinkXAxis)
          return AxisLinkType.None;
        else if(this.m_LinkXAxisOrgA==0 && this.m_LinkXAxisOrgB==1 && this.m_LinkXAxisEndA==0 && this.m_LinkXAxisEndB==1)
          return AxisLinkType.Straight;
        else return AxisLinkType.Custom;
      }
      set 
      {
        if(value==AxisLinkType.None)
          this.m_LinkXAxis = false;
        else
        {
          this.m_LinkXAxis = true;
          if(value==AxisLinkType.Straight)
          {
            this.m_LinkXAxisOrgA=0;
            this.m_LinkXAxisOrgB=1;
            this.m_LinkXAxisEndA=0;
            this.m_LinkXAxisEndB=1;
          }
          if(null!=LinkedLayer)
            OnLinkedLayerAxesChanged(LinkedLayer, new System.EventArgs());
        }
      }
    }

    
    /// <summary>The type of y axis link.</summary>
    /// <value>Can be either None, Straight or Custom link.</value>
    public AxisLinkType YAxisLinkType
    {
      get 
      {
        if(!this.m_LinkYAxis)
          return AxisLinkType.None;
        else if(this.m_LinkYAxisOrgA==0 && this.m_LinkYAxisOrgB==1 && this.m_LinkYAxisEndA==0 && this.m_LinkYAxisEndB==1)
          return AxisLinkType.Straight;
        else return AxisLinkType.Custom;
      }
      set 
      {
        if(value==AxisLinkType.None)
        {
          this.m_LinkYAxis = false;
        }
        else
        {
          this.m_LinkYAxis = true;
          if(value==AxisLinkType.Straight)
          {
            this.m_LinkYAxisOrgA=0;
            this.m_LinkYAxisOrgB=1;
            this.m_LinkYAxisEndA=0;
            this.m_LinkYAxisEndB=1;
          }
          if(null!=LinkedLayer)
            OnLinkedLayerAxesChanged(LinkedLayer, new System.EventArgs());
        }
      }
    }


    /// <summary>
    /// Set all parameters of the x axis link by once.
    /// </summary>
    /// <param name="linktype">The type of the axis link, i.e. None, Straight or Custom.</param>
    /// <param name="orgA">The value a of x-axis link for link of axis origin: org' = a + b*org.</param>
    /// <param name="orgB">The value b of x-axis link for link of axis origin: org' = a + b*org.</param>
    /// <param name="endA">The value a of x-axis link for link of axis end: end' = a + b*end.</param>
    /// <param name="endB">The value b of x-axis link for link of axis end: end' = a + b*end.</param>
    public void SetXAxisLinkParameter(AxisLinkType linktype, double orgA, double orgB, double endA, double endB)
    {
      if(linktype==AxisLinkType.Straight)
      {
        orgA=0;
        orgB=1;
        endA=0;
        endB=1;
      }

      bool linkaxis = (linktype!=AxisLinkType.None);

      if(
        (linkaxis != m_LinkXAxis) ||
        (orgA!=m_LinkXAxisOrgA) ||
        (orgB!=m_LinkXAxisOrgB) ||
        (endA!=m_LinkXAxisEndA) ||
        (endB!=m_LinkXAxisEndB) )
      {
        m_LinkXAxis     = linkaxis;
        m_LinkXAxisOrgA = orgA;
        m_LinkXAxisOrgB = orgB;
        m_LinkXAxisEndA = endA;
        m_LinkXAxisEndB = endB;
          
        if(IsLinked)
          OnLinkedLayerAxesChanged(LinkedLayer,new EventArgs());
      }
    }

    /// <summary>The value a of x-axis link for link of origin: org' = a + b*org.</summary>
    /// <value>The value a of x-axis link for link of origin: org' = a + b*org.</value>
    public double LinkXAxisOrgA
    {
      get { return m_LinkXAxisOrgA; }
      set
      {
        if(m_LinkXAxisOrgA!=value)
        {
          m_LinkXAxisOrgA = value;
          
          if(IsLinked)
            OnLinkedLayerAxesChanged(LinkedLayer,new EventArgs());
        }
      }
    }

    /// <summary>The value b of x-axis link for link of origin: org' = a + b*org.</summary>
    /// <value>The value b of x-axis link for link of origin: org' = a + b*org.</value>
    public double LinkXAxisOrgB
    {
      get { return m_LinkXAxisOrgB; }
      set
      {
        if(m_LinkXAxisOrgB!=value)
        {
          m_LinkXAxisOrgB = value;
          
          if(IsLinked)
            OnLinkedLayerAxesChanged(LinkedLayer,new EventArgs());
        }
      }
    }

    /// <summary>The value a of x-axis link for link of axis end: end' = a + b*end.</summary>
    /// <value>The value a of x-axis link for link of axis end: end' = a + b*end.</value>
    public double LinkXAxisEndA
    {
      get { return m_LinkXAxisEndA; }
      set
      {
        if(m_LinkXAxisEndA!=value)
        {
          m_LinkXAxisEndA = value;
          
          if(IsLinked)
            OnLinkedLayerAxesChanged(LinkedLayer,new EventArgs());
        }
      }
    }


    /// <summary>The value b of x-axis link for link of axis end: end' = a + b*end.</summary>
    /// <value>The value b of x-axis link for link of axis end: end' = a + b*end.</value>
    public double LinkXAxisEndB
    {
      get { return m_LinkXAxisEndB; }
      set
      {
        if(m_LinkXAxisEndB!=value)
        {
          m_LinkXAxisEndB = value;
          
          if(IsLinked)
            OnLinkedLayerAxesChanged(LinkedLayer,new EventArgs());
        }
      }
    }


    /// <summary>
    /// Set all parameters of the y axis link by once.
    /// </summary>
    /// <param name="linktype">The type of the axis link, i.e. None, Straight or Custom.</param>
    /// <param name="orgA">The value a of y-axis link for link of axis origin: org' = a + b*org.</param>
    /// <param name="orgB">The value b of y-axis link for link of axis origin: org' = a + b*org.</param>
    /// <param name="endA">The value a of y-axis link for link of axis end: end' = a + b*end.</param>
    /// <param name="endB">The value b of y-axis link for link of axis end: end' = a + b*end.</param>
    public void SetYAxisLinkParameter(AxisLinkType linktype, double orgA, double orgB, double endA, double endB)
    {
      if(linktype==AxisLinkType.Straight)
      {
        orgA=0;
        orgB=1;
        endA=0;
        endB=1;
      }

      bool linkaxis = (linktype!=AxisLinkType.None);


      if(
        (linkaxis != m_LinkYAxis) ||
        (orgA!=m_LinkYAxisOrgA) ||
        (orgB!=m_LinkYAxisOrgB) ||
        (endA!=m_LinkYAxisEndA) ||
        (endB!=m_LinkYAxisEndB) )
      {
        m_LinkYAxis     = linkaxis;
        m_LinkYAxisOrgA = orgA;
        m_LinkYAxisOrgB = orgB;
        m_LinkYAxisEndA = endA;
        m_LinkYAxisEndB = endB;
          
        if(IsLinked)
          OnLinkedLayerAxesChanged(LinkedLayer,new EventArgs());
      }
    }


    /// <summary>The value a of y-axis link for link of origin: org' = a + b*org.</summary>
    /// <value>The value a of y-axis link for link of origin: org' = a + b*org.</value>
    public double LinkYAxisOrgA
    {
      get { return m_LinkYAxisOrgA; }
      set
      {
        if(m_LinkYAxisOrgA!=value)
        {
          m_LinkYAxisOrgA = value;
          
          if(IsLinked)
            OnLinkedLayerAxesChanged(LinkedLayer,new EventArgs());
        }
      }
    }

    /// <summary>The value b of y-axis link for link of origin: org' = a + b*org.</summary>
    /// <value>The value b of y-axis link for link of origin: org' = a + b*org.</value>
    public double LinkYAxisOrgB
    {
      get { return m_LinkYAxisOrgB; }
      set
      {
        if(m_LinkYAxisOrgB!=value)
        {
          m_LinkYAxisOrgB = value;
          
          if(IsLinked)
            OnLinkedLayerAxesChanged(LinkedLayer,new EventArgs());
        }
      }
    }

    /// <summary>The value a of y-axis link for link of axis end: end' = a + b*end.</summary>
    /// <value>The value a of y-axis link for link of axis end: end' = a + b*end.</value>
    public double LinkYAxisEndA
    {
      get { return m_LinkYAxisEndA; }
      set
      {
        if(m_LinkYAxisEndA!=value)
        {
          m_LinkYAxisEndA = value;
          
          if(IsLinked)
            OnLinkedLayerAxesChanged(LinkedLayer,new EventArgs());
        }
      }
    }


    /// <summary>The value b of y-axis link for link of axis end: end' = a + b*end.</summary>
    /// <value>The value b of y-axis link for link of axis end: end' = a + b*end.</value>
    public double LinkYAxisEndB
    {
      get { return m_LinkYAxisEndB; }
      set
      {
        if(m_LinkYAxisEndB!=value)
        {
          m_LinkYAxisEndB = value;
          
          if(IsLinked)
            OnLinkedLayerAxesChanged(LinkedLayer,new EventArgs());
        }
      }
    }


    /// <summary>
    /// Measures to do when one of the axis of the linked layer changed.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The event args.</param>
    protected void OnLinkedLayerAxesChanged(object sender, System.EventArgs e)
    {
      if(null==LinkedLayer)
        return; // this should not happen, since what is sender then?

      if(IsXAxisLinked && null!=LinkedLayer)
      {
        this._xAxis.ProcessDataBounds( 
          m_LinkXAxisOrgA+m_LinkXAxisOrgB*LinkedLayer.XAxis.OrgAsVariant,true,
          m_LinkXAxisEndA+m_LinkXAxisEndB*LinkedLayer.XAxis.EndAsVariant,true);

      }

      if(IsYAxisLinked && null!=LinkedLayer)
      {
        this._yAxis.ProcessDataBounds( 
          m_LinkYAxisOrgA+m_LinkYAxisOrgB*LinkedLayer.YAxis.OrgAsVariant,true,
          m_LinkYAxisEndA+m_LinkYAxisEndB*LinkedLayer.YAxis.EndAsVariant,true);
      }

      // indicate that the axes have changed
      if(IsXAxisLinked || IsYAxisLinked)
      {
        this.OnAxesChanged();
      }
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
      get { return _leftAxisStyle; }
    }
    public XYAxisStyle BottomAxisStyle
    {
      get { return _bottomAxisStyle; }
    }
    public XYAxisStyle RightAxisStyle
    {
      get { return _rightAxisStyle; }
    }
    public XYAxisStyle TopAxisStyle
    {
      get { return _topAxisStyle; }
    }


    public AbstractXYAxisLabelStyle LeftLabelStyle
    {
      get { return _leftLabelStyle; }
    }
    public AbstractXYAxisLabelStyle RightLabelStyle
    {
      get { return _rightLabelStyle; }
    }
    public AbstractXYAxisLabelStyle BottomLabelStyle
    {
      get { return _bottomLabelStyle; }
    }
    public AbstractXYAxisLabelStyle TopLabelStyle
    {
      get { return _topLabelStyle; }
    }
    
    public bool LeftAxisEnabled
    {
      get { return this._showLeftAxis; }
      set
      {
        if(value!=this._showLeftAxis)
        {
          _showLeftAxis = value;
          this.OnInvalidate();
        }
      }
    }

    public bool BottomAxisEnabled
    {
      get { return this._showBottomAxis; }
      set
      {
        if(value!=this._showBottomAxis)
        {
          _showBottomAxis = value;
          this.OnInvalidate();
        }
      }
    }
    public bool RightAxisEnabled
    {
      get { return this._showRightAxis; }
      set
      {
        if(value!=this._showRightAxis)
        {
          _showRightAxis = value;
          this.OnInvalidate();
        }
      }
    }
    public bool TopAxisEnabled
    {
      get { return this._showTopAxis; }
      set
      {
        if(value!=this._showTopAxis)
        {
          _showTopAxis = value;
          this.OnInvalidate();
        }
      }
    }

    public TextGraphics LeftAxisTitle
    {
      get { return this._leftAxisTitle; }
      set
      {
        this._leftAxisTitle = value;
        this.OnInvalidate();
      }
    }

    public TextGraphics RightAxisTitle
    {
      get { return this._rightAxisTitle; }
      set
      {
        this._rightAxisTitle = value;
        this.OnInvalidate();
      }
    }

    public TextGraphics TopAxisTitle
    {
      get { return this._topAxisTitle; }
      set
      {
        this._topAxisTitle = value;
        this.OnInvalidate();
      }
    }
    public TextGraphics BottomAxisTitle
    {
      get { return this._bottomAxisTitle; }
      set
      {
        this._bottomAxisTitle = value;
        this.OnInvalidate();
      }
    }

    public string LeftAxisTitleString
    {
      get { return _leftAxisTitle!=null ? _leftAxisTitle.Text : null; }
      set
      {
        string newtitle = (value==null || value==String.Empty) ? null : value;
        bool bChanged = !string.Equals(_leftAxisTitle,newtitle);
        if(_leftAxisTitle==null && newtitle!=null)
          if(_leftAxisTitle==null)
          {
            _leftAxisTitle = new TextGraphics();
            _leftAxisTitle.Rotation=-90;
            _leftAxisTitle.XAnchor = TextGraphics.XAnchorPositionType.Center;
            _leftAxisTitle.YAnchor = TextGraphics.YAnchorPositionType.Bottom;
            _leftAxisTitle.Position = new PointF(-0.125f*Size.Width,0.5f*Size.Height);
          }

        if(newtitle!=null)
          _leftAxisTitle.Text = newtitle;
        else
          _leftAxisTitle = null;

        if(bChanged)
          this.OnInvalidate();  
      }
    }

    public string RightAxisTitleString
    {
      get { return _rightAxisTitle!=null ? _rightAxisTitle.Text : null; }
      set
      {
        string newtitle = (value==null || value==String.Empty) ? null : value;
        bool bChanged = !string.Equals(_rightAxisTitle,newtitle);
        if(_rightAxisTitle==null && newtitle!=null)
        {
          _rightAxisTitle = new TextGraphics();
          _rightAxisTitle.Rotation=-90;
          _rightAxisTitle.XAnchor = TextGraphics.XAnchorPositionType.Center;
          _rightAxisTitle.YAnchor = TextGraphics.YAnchorPositionType.Top;
          _rightAxisTitle.Position = new PointF(1.125f*Size.Width,0.5f*Size.Height);
        }

        if(newtitle!=null)
          _rightAxisTitle.Text = newtitle;
        else
          _rightAxisTitle = null;

        if(bChanged)
          this.OnInvalidate();        
      }
    }

    public string TopAxisTitleString
    {
      get { return _topAxisTitle!=null ? _topAxisTitle.Text : null; }
      set
      {
        string newtitle = (value==null || value==String.Empty) ? null : value;
        bool bChanged = !string.Equals(_topAxisTitle,newtitle);
        if(_topAxisTitle==null && newtitle!=null)
        {
          _topAxisTitle = new TextGraphics();
          _topAxisTitle.Rotation=0;
          _topAxisTitle.XAnchor = TextGraphics.XAnchorPositionType.Center;
          _topAxisTitle.YAnchor = TextGraphics.YAnchorPositionType.Bottom;
          _topAxisTitle.Position = new PointF(0.5f*Size.Width,-0.125f*Size.Height);
        }

        if(newtitle!=null)
          _topAxisTitle.Text = newtitle;
        else
          _topAxisTitle = null;

        if(bChanged)
          this.OnInvalidate();

        
      }
    }

    public string BottomAxisTitleString
    {
      get { return _bottomAxisTitle!=null ? _bottomAxisTitle.Text : null; }
      set
      {
        string newtitle = (value==null || value==String.Empty) ? null : value;
        bool bChanged = !string.Equals(_bottomAxisTitle,newtitle);
        if(_bottomAxisTitle==null && newtitle!=null)
        {
          _bottomAxisTitle = new TextGraphics();
          _bottomAxisTitle.Rotation=0;
          _bottomAxisTitle.XAnchor = TextGraphics.XAnchorPositionType.Center;
          _bottomAxisTitle.YAnchor = TextGraphics.YAnchorPositionType.Top;
          _bottomAxisTitle.Position = new PointF(0.5f*Size.Width,1.125f*Size.Height);
        }

        if(newtitle!=null)
          _bottomAxisTitle.Text = newtitle;
        else
          _bottomAxisTitle = null;

        if(bChanged)
          this.OnInvalidate();
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

      if(_fillLayerArea)
        g.FillRectangle(m_LayerAreaFillBrush,0,0,_cachedLayerSize.Width,_cachedLayerSize.Height);

      m_GraphObjects.DrawObjects(g,1,this);

      RectangleF layerBounds = new RectangleF(_cachedLayerPosition,_cachedLayerSize);


      // Before we paint the axis, we have to make sure that all plot items
      // had their data updated, so that the axes are updated before they are drawn!
      foreach(PlotItem pi in m_PlotItems)
      {
        pi.UpdateCachedData(this);
      }


      if(_showLeftAxis)
        _leftAxisStyle.Paint(g,this,this._yAxis);
      if(_showLeftAxis)
        _leftLabelStyle.Paint(g,this,this._yAxis,_leftAxisStyle);
      if(_showLeftAxis && null!=_leftAxisTitle)
        _leftAxisTitle.Paint(g,this);
      if(_showBottomAxis)
        _bottomAxisStyle.Paint(g,this,this._xAxis);
      if(_showBottomAxis)
        _bottomLabelStyle.Paint(g,this,this._xAxis,_bottomAxisStyle);
      if(_showBottomAxis && null!=_bottomAxisTitle)
        _bottomAxisTitle.Paint(g,this);
      if(_showRightAxis)
        _rightAxisStyle.Paint(g,this,this._yAxis);
      if(_showRightAxis)
        _rightLabelStyle.Paint(g,this,this._yAxis,_rightAxisStyle);
      if(_showRightAxis && null!=_rightAxisTitle)
        _rightAxisTitle.Paint(g,this);
      if(_showTopAxis)
        _topAxisStyle.Paint(g,this,this._xAxis);
      if(_showTopAxis)
        _topLabelStyle.Paint(g,this,this._xAxis,_topAxisStyle);
      if(_showTopAxis && null!=_topAxisTitle)
        _topAxisTitle.Paint(g,this);
      if(_legend!=null)
        _legend.Paint(g,this);


      if(ClipDataToFrame)
      {
        g.SetClip(new RectangleF(new PointF(0,0),this._cachedLayerSize));
      }

      foreach(PlotItem pi in m_PlotItems)
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

    public IHitTestObject HitTest(PointF pageC)
    {
      IHitTestObject hit;

      PointF layerC = GraphToLayerCoordinates(pageC);


      GraphicsObject[] specObjects = 
          {
            _leftAxisTitle,
            _bottomAxisTitle,
            _topAxisTitle,
            _rightAxisTitle,
            _legend
          };


      // do the hit test first for the special objects of the layer
      foreach(GraphicsObject go in specObjects)
      {
        if(null!=go)
        {
          hit = go.HitTest(layerC);
          if(null!=hit)
          {
            if(null==hit.Remove && (hit.HittedObject is GraphicsObject))
              hit.Remove = new DoubleClickHandler(EhTitlesOrLegend_Remove);
            return ForwardTransform(hit);
          }
        }
      }

      // first hit testing all four corners of the layer
      GraphicsPath layercorners = new GraphicsPath();
      float catchrange = 6;
      layercorners.AddEllipse(-catchrange,-catchrange,2*catchrange,2*catchrange);
      layercorners.AddEllipse(_cachedLayerSize.Width-catchrange,0-catchrange,2*catchrange,2*catchrange);
      layercorners.AddEllipse(0-catchrange,_cachedLayerSize.Height-catchrange,2*catchrange,2*catchrange);
      layercorners.AddEllipse(_cachedLayerSize.Width-catchrange,_cachedLayerSize.Height-catchrange,2*catchrange,2*catchrange);
      layercorners.CloseAllFigures();
      if(layercorners.IsVisible(layerC))
      {
        hit = new HitTestObject(layercorners,this);
        hit.DoubleClick = LayerPositionEditorMethod;
        return ForwardTransform(hit);
      }



      // hit testing the axes - first a small area around the axis line
      // if hitting this, the editor for scaling the axis should be shown
      if(_showLeftAxis && null!=(hit = _leftAxisStyle.HitTest(this,layerC,false)))
      {
        hit.DoubleClick=AxisScaleEditorMethod;
        return ForwardTransform(hit);
      }
      if(_showBottomAxis && null!=(hit = _bottomAxisStyle.HitTest(this,layerC,false)))
      {
        hit.DoubleClick=AxisScaleEditorMethod;
        return ForwardTransform(hit);
      }
      if(_showRightAxis && null!=(hit = _rightAxisStyle.HitTest(this,layerC,false)))
      {
        hit.DoubleClick=AxisScaleEditorMethod;
        return ForwardTransform(hit);
      }
      if(_showTopAxis && null!=(hit = _topAxisStyle.HitTest(this,layerC,false)))
      {
        hit.DoubleClick=AxisScaleEditorMethod;
        return ForwardTransform(hit);
      }


      // hit testing the axes - secondly now wiht the ticks
      // in this case the TitleAndFormat editor for the axis should be shown
      if(_showLeftAxis && null!=(hit = _leftAxisStyle.HitTest(this,layerC,true)))
      {
        if(hit.DoubleClick==null) hit.DoubleClick=AxisStyleEditorMethod;
        return ForwardTransform(hit);
      }
      if(_showBottomAxis && null!=(hit = _bottomAxisStyle.HitTest(this,layerC,true)))
      {
        if(hit.DoubleClick==null) hit.DoubleClick=AxisStyleEditorMethod;
        return ForwardTransform(hit);
      }
      if(_showRightAxis && null!=(hit = _rightAxisStyle.HitTest(this,layerC,true)))
      {
        if(hit.DoubleClick==null) hit.DoubleClick=AxisStyleEditorMethod;
        return ForwardTransform(hit);
      }
      if(_showTopAxis && null!=(hit = _topAxisStyle.HitTest(this,layerC,true)))
      {
        if(hit.DoubleClick==null) hit.DoubleClick=AxisStyleEditorMethod;
        return ForwardTransform(hit);
      }

      // hit testing the axes labels
      if(_showLeftAxis && null!=(hit = this._leftLabelStyle.HitTest(this,layerC)))
      {
        if(hit.DoubleClick==null) hit.DoubleClick=AxisLabelStyleEditorMethod;
        return ForwardTransform(hit);
      }
      if(_showBottomAxis && null!=(hit = _bottomLabelStyle.HitTest(this,layerC)))
      {
        if(hit.DoubleClick==null) hit.DoubleClick=AxisLabelStyleEditorMethod;
        return ForwardTransform(hit);
      }
      if(_showRightAxis && null!=(hit = _rightLabelStyle.HitTest(this,layerC)))
      {
        if(hit.DoubleClick==null) hit.DoubleClick=AxisLabelStyleEditorMethod;
        return ForwardTransform(hit);
      }
      if(_showTopAxis && null!=(hit = _topLabelStyle.HitTest(this,layerC)))
      {
        if(hit.DoubleClick==null) hit.DoubleClick=AxisLabelStyleEditorMethod;
        return ForwardTransform(hit);
      }

      // now hit testing the other objects in the layer
      foreach(GraphicsObject go in m_GraphObjects)
      {
        hit = go.HitTest(layerC);
        if(null!=hit)
        {
          if(null==hit.Remove && (hit.HittedObject is GraphicsObject)) 
            hit.Remove = new DoubleClickHandler(EhGraphicsObject_Remove);
          return ForwardTransform(hit);
        }
      }

      if(null!=(hit=m_PlotItems.HitTest(this,layerC)))
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
    }


    protected void OnPositionChanged()
    {
      if(null!=PositionChanged)
        PositionChanged(this,new System.EventArgs());
    }

    protected virtual void OnAxesChanged()
    {
      if(null!=AxesChanged)
        AxesChanged(this,new System.EventArgs());
    }

    protected void OnInvalidate()
    {
      if(this.m_ParentLayerCollection is Main.IChildChangedEventSink)
        ((Main.IChildChangedEventSink)this.m_ParentLayerCollection).OnChildChanged(this,EventArgs.Empty);
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
      else if(object.ReferenceEquals(go,layer._leftAxisTitle))
      {
        layer._leftAxisTitle=null;
        return true;
      }
      else if(object.ReferenceEquals(go,layer._bottomAxisTitle))
      {
        layer._bottomAxisTitle=null;
        return true;
      }
      else if(object.ReferenceEquals(go,layer._rightAxisTitle))
      {
        layer._rightAxisTitle=null;
        return true;
      }
      else if(object.ReferenceEquals(go,layer._topAxisTitle))
      {
        layer._topAxisTitle=null;
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
      if(0==m_PlotAssociationXBoundariesChanged_EventSuspendCount)
      {
        // now we have to inform all the PlotAssociations that a new axis was loaded
        _xAxis.DataBoundsObject.BeginUpdate();
        _xAxis.DataBoundsObject.Reset();
        foreach(PlotItem pa in this.PlotItems)
        {
          if(pa is IXBoundsHolder)
          {
            // merge the bounds with x and yAxis
            ((IXBoundsHolder)pa).MergeXBoundsInto(_xAxis.DataBoundsObject); // merge all x-boundaries in the x-axis boundary object
          }
        }
        _xAxis.DataBoundsObject.EndUpdate();
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
      if(0==m_PlotAssociationYBoundariesChanged_EventSuspendCount)
      {
        // now we have to inform all the PlotAssociations that a new axis was loaded
        _yAxis.DataBoundsObject.BeginUpdate();
        _yAxis.DataBoundsObject.Reset();
        foreach(PlotItem pa in this.PlotItems)
        {
          if(pa is IYBoundsHolder)
          {
            // merge the bounds with x and yAxis
            ((IYBoundsHolder)pa).MergeYBoundsInto(_yAxis.DataBoundsObject); // merge all x-boundaries in the x-axis boundary object
        
          }
        }
        _yAxis.DataBoundsObject.EndUpdate();
      }
    }
    
    protected virtual void OnXAxisChangedEventHandler(object sender, System.EventArgs e)
    {
      // inform linked layers
      if(null!=AxesChanged)
        AxesChanged(this,new EventArgs());
      
      // renew the picture
      OnInvalidate();
    }

    protected virtual void OnYAxisChangedEventHandler(object sender, System.EventArgs e)
    {
      // inform linked layers 
      if(null!=AxesChanged)
        AxesChanged(this,new EventArgs());

      // renew the picture
      OnInvalidate();
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
        return this.m_ParentLayerCollection;
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
 
      public bool Convert(double xlocation, double ylocation, out double x_rel, out double y_rel)
      {
        x_rel = xlocation/_layerWidth;
        y_rel = 1-ylocation/_layerHeight;
        return !double.IsNaN(x_rel) && !double.IsNaN(y_rel);
      }
      public event System.EventHandler Changed;

    }


    #endregion
  
  }
}
