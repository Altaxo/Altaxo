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
using System.Collections.Generic;
using System.ComponentModel;        
using System.Reflection;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Graph.Gdi.Background;


namespace Altaxo.Graph.Gdi
{
  using Shapes;
  using Axis;
  using Plot;

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
    protected Gdi.Background.IBackgroundStyle _layerBackground;

    /// <summary>If true, the data are clipped to the frame.</summary>
    protected bool _clipDataToFrame=true;
  
    protected TextGraphic _legend;

    G2DScaleStyleCollection _scaleStyles;

    LinkedScaleCollection _axisProperties;

    protected GraphicCollection _graphObjects;

    protected PlotItemCollection _plotItems;

    protected XYPlotLayerPositionAndSize _location;


    /// <summary>
    /// The parent layer collection which contains this layer (or null if not member of such collection).
    /// </summary>
    protected object _parentLayerCollection;
    //    protected XYPlotLayerCollection _parentLayerCollection=null;

    /// <summary>
    /// The index inside the parent collection of this layer (or 0 if not member of such collection).
    /// </summary>
    protected int _layerNumber;

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
    public G2DScaleStyleCollection ScaleStyles
    {
      get { return _scaleStyles; }
      protected set
      {
        G2DScaleStyleCollection oldvalue = _scaleStyles;
        _scaleStyles = value;
        if (value != oldvalue)
        {
          if (null != oldvalue)
          {
            oldvalue.Changed -= new EventHandler(this.OnChildChangedEventHandler);
            oldvalue.SetParentLayer(null, true);
          }
          if (null != value)
          {
            value.SetParentLayer(this, true);
            value.Changed += new EventHandler(this.OnChildChangedEventHandler);
          }

          OnInvalidate();
        }
        

      }
    }

    public LinkedScaleCollection AxisProperties
    {
      get
      {
        return _axisProperties;
      }
      protected set
      {
        LinkedScaleCollection oldvalue = _axisProperties;
        _axisProperties = value;
        if (oldvalue != value)
        {
          if(null!=oldvalue)
          {
            oldvalue.Changed -= new EventHandler(OnChildChangedEventHandler);
            oldvalue.X.ScaleInstanceChanged -= new EventHandler(EhXAxisInstanceChanged);
            oldvalue.Y.ScaleInstanceChanged -= new EventHandler(EhYAxisInstanceChanged);
          }
          if (null != value)
          {
            value.Changed += new EventHandler(OnChildChangedEventHandler);
            value.X.ScaleInstanceChanged += new EventHandler(EhXAxisInstanceChanged);
            value.Y.ScaleInstanceChanged += new EventHandler(EhYAxisInstanceChanged);
          }
          OnInvalidate();
        }
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
        BrushX layerAreaFillBrush = (BrushX)info.GetValue("LayerAreaFillBrush",typeof(BrushX));

        if (fillLayerArea)
          s._layerBackground = new FilledRectangle(layerAreaFillBrush.Color);



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

        s._axisProperties.X.Scale = (Scale)info.GetValue("XAxis",typeof(Scale));
        s._axisProperties.Y.Scale = (Scale)info.GetValue("YAxis",typeof(Scale));
        s.AxisProperties.X.IsLinked = info.GetBoolean("LinkXAxis");
        s.AxisProperties.Y.IsLinked = info.GetBoolean("LinkYAxis");
        s.AxisProperties.X.LinkOrgA = info.GetDouble("LinkXAxisOrgA");
        s.AxisProperties.X.LinkOrgB = info.GetDouble("LinkXAxisOrgB");
        s.AxisProperties.X.LinkEndA = info.GetDouble("LinkXAxisEndA");
        s.AxisProperties.X.LinkEndB = info.GetDouble("LinkXAxisEndB");
        s.AxisProperties.Y.LinkOrgA = info.GetDouble("LinkYAxisOrgA");
        s.AxisProperties.Y.LinkOrgB = info.GetDouble("LinkYAxisOrgB");
        s.AxisProperties.Y.LinkEndA = info.GetDouble("LinkYAxisEndA");
        s.AxisProperties.Y.LinkEndB = info.GetDouble("LinkYAxisEndB");


        // Styles
        bool showLeft = info.GetBoolean("ShowLeftAxis");
        bool showBottom = info.GetBoolean("ShowBottomAxis");
        bool showRight = info.GetBoolean("ShowRightAxis");
        bool showTop = info.GetBoolean("ShowTopAxis");

        s._scaleStyles.AxisStyleEnsured(A2DAxisStyleIdentifier.Y0).AxisLineStyle = (AxisLineStyle)info.GetValue("LeftAxisStyle", typeof(AxisLineStyle));
        s._scaleStyles.AxisStyleEnsured(A2DAxisStyleIdentifier.X0).AxisLineStyle = (AxisLineStyle)info.GetValue("BottomAxisStyle", typeof(AxisLineStyle));
        s._scaleStyles.AxisStyleEnsured(A2DAxisStyleIdentifier.Y1).AxisLineStyle = (AxisLineStyle)info.GetValue("RightAxisStyle", typeof(AxisLineStyle));
        s._scaleStyles.AxisStyleEnsured(A2DAxisStyleIdentifier.X1).AxisLineStyle = (AxisLineStyle)info.GetValue("TopAxisStyle", typeof(AxisLineStyle));


        s._scaleStyles.AxisStyle(A2DAxisStyleIdentifier.Y0).MajorLabelStyle = (AxisLabelStyleBase)info.GetValue("LeftLabelStyle", typeof(AxisLabelStyleBase));
        s._scaleStyles.AxisStyle(A2DAxisStyleIdentifier.X0).MajorLabelStyle = (AxisLabelStyleBase)info.GetValue("BottomLabelStyle", typeof(AxisLabelStyleBase));
        s._scaleStyles.AxisStyle(A2DAxisStyleIdentifier.Y1).MajorLabelStyle = (AxisLabelStyleBase)info.GetValue("RightLabelStyle", typeof(AxisLabelStyleBase));
        s._scaleStyles.AxisStyle(A2DAxisStyleIdentifier.X1).MajorLabelStyle = (AxisLabelStyleBase)info.GetValue("TopLabelStyle", typeof(AxisLabelStyleBase));
      
      
        // Titles and legend
        s._scaleStyles.AxisStyle(A2DAxisStyleIdentifier.Y0).Title = (TextGraphic)info.GetValue("LeftAxisTitle", typeof(TextGraphic));
        s._scaleStyles.AxisStyle(A2DAxisStyleIdentifier.X0).Title = (TextGraphic)info.GetValue("BottomAxisTitle", typeof(TextGraphic));
        s._scaleStyles.AxisStyle(A2DAxisStyleIdentifier.Y1).Title = (TextGraphic)info.GetValue("RightAxisTitle", typeof(TextGraphic));
        s._scaleStyles.AxisStyle(A2DAxisStyleIdentifier.X1).Title = (TextGraphic)info.GetValue("TopAxisTitle", typeof(TextGraphic));
        
        if(!showLeft)
          s._scaleStyles.RemoveAxisStyle(A2DAxisStyleIdentifier.Y0);
        if (!showRight)
          s._scaleStyles.RemoveAxisStyle(A2DAxisStyleIdentifier.Y1);
        if (!showBottom)
          s._scaleStyles.RemoveAxisStyle(A2DAxisStyleIdentifier.X0);
        if (!showTop)
          s._scaleStyles.RemoveAxisStyle(A2DAxisStyleIdentifier.X1);
        
        
        s._legend = (TextGraphic)info.GetValue("Legend",typeof(TextGraphic));
      


        // XYPlotLayer specific
        s._linkedLayer.SetDocNode((XYPlotLayer)info.GetValue("LinkedLayer", typeof(XYPlotLayer)), s);

        s._graphObjects = (GraphicCollection)info.GetValue("GraphObjects",typeof(GraphicCollection));

        s._plotItems = (PlotItemCollection)info.GetValue("Plots",typeof(PlotItemCollection));

        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLayer", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLayer", 1)]
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
        BrushX layerAreaFillBrush = (BrushX)info.GetValue("LayerAreaFillBrush", typeof(BrushX));

        if (fillLayerArea)
          s._layerBackground = new FilledRectangle(layerAreaFillBrush.Color);




        // size, position, rotation and scale
        
        s._location.WidthType  = (XYPlotLayerSizeType)info.GetValue("WidthType",typeof(XYPlotLayerSizeType));
        s._location.HeightType = (XYPlotLayerSizeType)info.GetValue("HeightType",typeof(XYPlotLayerSizeType));
        s._location.Width  = info.GetDouble("Width");
        s._location.Height = info.GetDouble("Height");
        s._cachedLayerSize   = (SizeF)info.GetValue("CachedSize",typeof(SizeF));
        s._coordinateSystem.UpdateAreaSize(s._cachedLayerSize);

        s._location.XPositionType = (XYPlotLayerPositionType)info.GetValue("XPositionType",typeof(XYPlotLayerPositionType));
        s._location.YPositionType = (XYPlotLayerPositionType)info.GetValue("YPositionType",typeof(XYPlotLayerPositionType));
        s._location.XPosition = info.GetDouble("XPosition");
        s._location.YPosition = info.GetDouble("YPosition");
        s._cachedLayerPosition = (PointF)info.GetValue("CachedPosition",typeof(PointF));

        s._location.Angle  = info.GetSingle("Rotation");
        s._location.Scale = info.GetSingle("Scale");

        // axis related

        s._axisProperties.X.Scale = (Scale)info.GetValue("XAxis",typeof(Scale));
        s._axisProperties.Y.Scale = (Scale)info.GetValue("YAxis",typeof(Scale));
        s._axisProperties.X.IsLinked = info.GetBoolean("LinkXAxis");
        s._axisProperties.Y.IsLinked = info.GetBoolean("LinkYAxis");
        s._axisProperties.X.LinkOrgA = info.GetDouble("LinkXAxisOrgA");
        s._axisProperties.X.LinkOrgB = info.GetDouble("LinkXAxisOrgB");
        s._axisProperties.X.LinkEndA = info.GetDouble("LinkXAxisEndA");
        s._axisProperties.X.LinkEndB = info.GetDouble("LinkXAxisEndB");
        s._axisProperties.Y.LinkOrgA = info.GetDouble("LinkYAxisOrgA");
        s._axisProperties.Y.LinkOrgB = info.GetDouble("LinkYAxisOrgB");
        s._axisProperties.Y.LinkEndA = info.GetDouble("LinkYAxisEndA");
        s._axisProperties.Y.LinkEndB = info.GetDouble("LinkYAxisEndB");


        // Styles
        bool showLeft = info.GetBoolean("ShowLeftAxis");
        bool showBottom = info.GetBoolean("ShowBottomAxis");
        bool showRight = info.GetBoolean("ShowRightAxis");
        bool showTop = info.GetBoolean("ShowTopAxis");

        s._scaleStyles.AxisStyleEnsured(A2DAxisStyleIdentifier.Y0).AxisLineStyle = (AxisLineStyle)info.GetValue("LeftAxisStyle", typeof(AxisLineStyle));
        s._scaleStyles.AxisStyleEnsured(A2DAxisStyleIdentifier.X0).AxisLineStyle = (AxisLineStyle)info.GetValue("BottomAxisStyle", typeof(AxisLineStyle));
        s._scaleStyles.AxisStyleEnsured(A2DAxisStyleIdentifier.Y1).AxisLineStyle = (AxisLineStyle)info.GetValue("RightAxisStyle", typeof(AxisLineStyle));
        s._scaleStyles.AxisStyleEnsured(A2DAxisStyleIdentifier.X1).AxisLineStyle = (AxisLineStyle)info.GetValue("TopAxisStyle", typeof(AxisLineStyle));


        s._scaleStyles.AxisStyle(A2DAxisStyleIdentifier.Y0).MajorLabelStyle = (AxisLabelStyleBase)info.GetValue("LeftLabelStyle", typeof(AxisLabelStyleBase));
        s._scaleStyles.AxisStyle(A2DAxisStyleIdentifier.X0).MajorLabelStyle = (AxisLabelStyleBase)info.GetValue("BottomLabelStyle", typeof(AxisLabelStyleBase));
        s._scaleStyles.AxisStyle(A2DAxisStyleIdentifier.Y1).MajorLabelStyle = (AxisLabelStyleBase)info.GetValue("RightLabelStyle", typeof(AxisLabelStyleBase));
        s._scaleStyles.AxisStyle(A2DAxisStyleIdentifier.X1).MajorLabelStyle = (AxisLabelStyleBase)info.GetValue("TopLabelStyle", typeof(AxisLabelStyleBase));


        // Titles and legend
        s._scaleStyles.AxisStyle(A2DAxisStyleIdentifier.Y0).Title = (TextGraphic)info.GetValue("LeftAxisTitle", typeof(TextGraphic));
        s._scaleStyles.AxisStyle(A2DAxisStyleIdentifier.X0).Title = (TextGraphic)info.GetValue("BottomAxisTitle", typeof(TextGraphic));
        s._scaleStyles.AxisStyle(A2DAxisStyleIdentifier.Y1).Title = (TextGraphic)info.GetValue("RightAxisTitle", typeof(TextGraphic));
        s._scaleStyles.AxisStyle(A2DAxisStyleIdentifier.X1).Title = (TextGraphic)info.GetValue("TopAxisTitle", typeof(TextGraphic));

        if (!showLeft)
          s._scaleStyles.RemoveAxisStyle(A2DAxisStyleIdentifier.Y0);
        if (!showRight)
          s._scaleStyles.RemoveAxisStyle(A2DAxisStyleIdentifier.Y1);
        if (!showBottom)
          s._scaleStyles.RemoveAxisStyle(A2DAxisStyleIdentifier.X0);
        if (!showTop)
          s._scaleStyles.RemoveAxisStyle(A2DAxisStyleIdentifier.X1);



        s._legend = (TextGraphic)info.GetValue("Legend",typeof(TextGraphic));
      
        // XYPlotLayer specific
        Main.DocumentPath linkedLayer = (Main.DocumentPath)info.GetValue("LinkedLayer",typeof(XYPlotLayer));
        if(linkedLayer!=null)
        {
          XmlSerializationSurrogate0 surr = new XmlSerializationSurrogate0();
          surr._Layer = s;
          surr._LinkedLayerPath = linkedLayer;
          info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(surr.EhDeserializationFinished);
        }

        s._graphObjects = (GraphicCollection)info.GetValue("GraphObjects",typeof(GraphicCollection));

        s._plotItems = (PlotItemCollection)info.GetValue("Plots",typeof(PlotItemCollection));
    

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


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLayer", 2)]
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


    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Graph.XYPlotLayer", 3)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotLayer), 4)]
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
        info.AddValue("AxisStyles", s._scaleStyles);

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
        s._layerBackground = (IBackgroundStyle)info.GetValue("Background",s);

        // size, position, rotation and scale
        s._location = (XYPlotLayerPositionAndSize)info.GetValue("LocationAndSize", s);
        s._cachedLayerSize   = (SizeF)info.GetValue("CachedSize",typeof(SizeF));
        s._cachedLayerPosition = (PointF)info.GetValue("CachedPosition",typeof(PointF));
        s._coordinateSystem.UpdateAreaSize(s._cachedLayerSize);


        // LayerProperties
         s._clipDataToFrame = info.GetBoolean("ClipDataToFrame");

        // axis related
        s._axisProperties = (LinkedScaleCollection)info.GetValue("AxisProperties", s);
 
        // Styles
        s._scaleStyles = (G2DScaleStyleCollection)info.GetValue("AxisStyles", s);

        // Legends
        count = info.OpenArray("Legends");
        s._legend = (TextGraphic)info.GetValue("e", s._legend);
        info.CloseArray(count);

        // XYPlotLayer specific
        count = info.OpenArray("LinkedLayers");
        s._linkedLayer = (Main.RelDocNodeProxy)info.GetValue("e", s);
        info.CloseArray(count);

        s._graphObjects = (GraphicCollection)info.GetValue("GraphicGlyphs", s);

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
      CopyFrom(from);
    }

    public void CopyFrom(XYPlotLayer from)
    {
      // XYPlotLayer style
      this._layerBackground = from._layerBackground == null ? null : (IBackgroundStyle)from._layerBackground.Clone();

      // size, position, rotation and scale
      this._location = from._location.Clone();
      this._cachedLayerSize   = from._cachedLayerSize;
      this._cachedLayerPosition = from._cachedLayerPosition;

      this.CoordinateSystem = (G2DCoordinateSystem)from.CoordinateSystem.Clone();


      // axis related

      this.AxisProperties = (LinkedScaleCollection)from._axisProperties.Clone();

      // Styles

      this.ScaleStyles = (G2DScaleStyleCollection)from._scaleStyles.Clone();

      this.Legend = null==from._legend ? null : (TextGraphic)from._legend.Clone();
      
      // XYPlotLayer specific
      this.LinkedLayerLink = from._linkedLayer.ClonePathOnly(this);
      
      this.GraphObjects = null==from._graphObjects ? null : new GraphicCollection(from._graphObjects);

      this.PlotItems = null==from._plotItems ? null : new PlotItemCollection(this,from._plotItems);

      // special way neccessary to handle plot groups
      //this.m_PlotGroups = null==from.m_PlotGroups ? null : from.m_PlotGroups.Clone(this._plotItems,from._plotItems);

      _cachedForwardMatrix = new Matrix();
      _cachedReverseMatrix = new Matrix();
      CalculateMatrix();
    }

    void CreateEventLinks()
    {

      if (null != _scaleStyles) _scaleStyles.Changed += new EventHandler(this.OnChildChangedEventHandler);

      if (null != _axisProperties)
      {
        _axisProperties.Changed += new EventHandler(OnChildChangedEventHandler);
        _axisProperties.X.ScaleInstanceChanged += new EventHandler(EhXAxisInstanceChanged);
        _axisProperties.Y.ScaleInstanceChanged += new EventHandler(EhYAxisInstanceChanged);
      }

      if (null != _legend) _legend.Changed += new EventHandler(this.OnChildChangedEventHandler);

      if (null != _linkedLayer)
        _linkedLayer.DocumentInstanceChanged += new Main.DocumentInstanceChangedEventHandler(this.EhLinkedLayerInstanceChanged);

      if (null != _graphObjects) _graphObjects.Changed += new EventHandler(this.OnChildChangedEventHandler);


      if (null != _plotItems)
      {
        _plotItems.SetParentLayer(this, true); // sets the parent layer, but suppresses the events following this.
        _plotItems.Changed += new EventHandler(this.OnChildChangedEventHandler);
      }
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
      this.CoordinateSystem = new G2DCartesicCoordinateSystem();
      this.ScaleStyles = new G2DScaleStyleCollection();
      this.AxisProperties = new LinkedScaleCollection();
      this.GraphObjects = new GraphicCollection();
      this._location = new XYPlotLayerPositionAndSize();
     
    }
    /// <summary>
    /// Creates a layer with position <paramref name="position"/> and size <paramref name="size"/>.
    /// </summary>
    /// <param name="position">The position of the layer on the printable area in points (1/72 inch).</param>
    /// <param name="size">The size of the layer in points (1/72 inch).</param>
    public XYPlotLayer(PointF position, SizeF size)
    {
      this._location = new XYPlotLayerPositionAndSize();

      this.CoordinateSystem = new G2DCartesicCoordinateSystem();
     // this.CoordinateSystem = new G2DPolarCoordinateSystem();

      this.Size = size;
      this.Position = position;

   
     
      this.ScaleStyles = new G2DScaleStyleCollection();
      this.AxisProperties = new LinkedScaleCollection();
      this.GraphObjects = new GraphicCollection();
      

      CalculateMatrix();

      LinkedLayerLink = new Main.RelDocNodeProxy(null, this);
      PlotItems = new PlotItemCollection(this);


     // CreateDefaultAxes();

      //DefaultYAxisTitleString = "Y axis";
      //DefaultXAxisTitleString = "X axis";
    }

  
  
    #endregion

    #region XYPlotLayer properties and methods


   
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

    public GraphicCollection GraphObjects
    {
      get { return _graphObjects; }
      protected set
      {
        GraphicCollection oldvalue = _graphObjects;
        _graphObjects = value;
        if (!object.ReferenceEquals(value, oldvalue))
        {
          if (null != oldvalue)
            value.Changed -= new EventHandler(this.OnChildChangedEventHandler);

          if (null != value)
            value.Changed += new EventHandler(this.OnChildChangedEventHandler);

          OnInvalidate();
        }
      }
    }

    public TextGraphic Legend
    {
      get
      {
        return _legend;
      }
      set
      {
        TextGraphic oldvalue = _legend;
        _legend = value;
        if (!object.ReferenceEquals(value, oldvalue))
        {
          if (oldvalue != null)
          {
            oldvalue.Changed -= new EventHandler(this.OnChildChangedEventHandler);
          }
          if (value != null)
          {
            value.Changed += new EventHandler(this.OnChildChangedEventHandler);
          }

          OnInvalidate();
        }
      }
    }
    

    public void Remove(GraphicBase go)
    {
      if (_scaleStyles.Remove(go))
        return;
      
      else if(object.ReferenceEquals(go,this._legend))
        _legend=null;
      else if(_graphObjects.Contains(go))
        _graphObjects.Remove(go);

    }

    private Altaxo.Main.RelDocNodeProxy LinkedLayerLink
    {
      set
      {
        Altaxo.Main.RelDocNodeProxy oldvalue = _linkedLayer;
        _linkedLayer = value;
        if (!object.ReferenceEquals(oldvalue, value))
        {
          if (null != oldvalue)
            oldvalue.DocumentInstanceChanged -= new Main.DocumentInstanceChangedEventHandler(this.EhLinkedLayerInstanceChanged);
          if (null != value)
            value.DocumentInstanceChanged += new Main.DocumentInstanceChangedEventHandler(this.EhLinkedLayerInstanceChanged);

          OnInvalidate();
        }
      }
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

        if (_linkedLayer == null)
          _linkedLayer = new Main.RelDocNodeProxy();

        _linkedLayer.SetDocNode(value,this);
        // Note here: the connection/disconnection to the event handlers of the linked layer
        // is done in this.EhLinkedLayerInstanceChanged, which was called automatically when the previous statement
        // was executed
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


    public PlotItemCollection PlotItems
    {
      get 
      {
        return _plotItems; 
      }
      protected set
      {
        PlotItemCollection oldvalue = _plotItems;
        _plotItems = value;
        if (!object.ReferenceEquals(value, oldvalue))
        {
          if (null != oldvalue)
          {
            oldvalue.Changed -= new EventHandler(this.OnChildChangedEventHandler);
            oldvalue.SetParentLayer(null, true); // sets the parent layer, but suppresses the events following this.
          }
          if (null != value)
          {
            value.SetParentLayer(this, true); // sets the parent layer, but suppresses the events following this.
            value.Changed += new EventHandler(this.OnChildChangedEventHandler);
          }
          OnInvalidate();
        }
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
      if(PlotItems.Flattened.Length==0)
      {
        _legend=null;
        OnInvalidate();
        return;
      }

      TextGraphic tgo;

      if(_legend!=null)
        tgo = new TextGraphic(_legend);
      else
        tgo = new TextGraphic();


      System.Text.StringBuilder strg = new System.Text.StringBuilder();
      for(int i=0;i<this.PlotItems.Flattened.Length;i++)
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
    /// This will create the default axes styles that are given by the coordinate system.
    /// </summary>
    public void CreateDefaultAxes()
    {
      foreach (A2DAxisStyleInformation info in CoordinateSystem.AxisStyles)
      {
        if (info.IsShownByDefault)
        {
          this.ScaleStyles.AxisStyleEnsured(info.Identifier);

          if (info.HasTitleByDefault)
          {
            this.SetAxisTitleString(info.Identifier, info.Identifier.AxisNumber==0 ? "X axis" : "Y axis");
          }
        }
       
      }
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
      foreach (AxisStyle style in this.ScaleStyles.AxisStyles)
      {
        GraphicBase.ScalePosition(style.Title, xscale, yscale);
      }

      GraphicBase.ScalePosition(this._legend,xscale,yscale);
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
    public Scale XAxis
    {
      get 
      {
        return _axisProperties.X.Scale; 
      }
      set
      {
        _axisProperties.X.Scale = value;
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
        this._axisProperties.X.EhLinkedLayerAxesChanged(LinkedLayer.AxisProperties.X.Scale);
      else
        RescaleXAxis();
    }

   

    public void RescaleXAxis()
    {
      if (null == this.PlotItems)
        return; // can happen during deserialization

      // we have to disable our own Handler since if we change one DataBound of a association,
      //it generates a OnBoundaryChanged, and then all boundaries are merges into the axis boundary, 
      //but (alas!) not all boundaries are now of the new type!
      _plotAssociationXBoundariesChanged_EventSuspendCount++; 
        
      _axisProperties.X.Scale.DataBoundsObject.BeginUpdate(); // Suppress events from the y-axis now
      _axisProperties.X.Scale.DataBoundsObject.Reset();
      foreach(IGPlotItem pa in this.PlotItems)
      {
        if(pa is IXBoundsHolder)
        {
          // merge the bounds with x and yAxis
          ((IXBoundsHolder)pa).MergeXBoundsInto(_axisProperties.X.Scale.DataBoundsObject); // merge all x-boundaries in the x-axis boundary object
        }
      }

      // take also the axis styles with physical values into account
      foreach (A2DAxisStyleIdentifier id in _scaleStyles.AxisStyleIDs)
      {
        if (id.AxisNumber == 0 && id.UsePhysicalValue)
          _axisProperties.X.Scale.DataBoundsObject.Add(id.PhysicalValue);
      }

      _plotAssociationXBoundariesChanged_EventSuspendCount = Math.Max(0,_plotAssociationXBoundariesChanged_EventSuspendCount-1);
      _axisProperties.X.Scale.DataBoundsObject.EndUpdate();
      _axisProperties.X.Scale.ProcessDataBounds();
    }
   
  
    /// <summary>Gets or sets the y axis of this layer.</summary>
    /// <value>The y axis of the layer.</value>
    public Scale YAxis
    {
      get 
      {
        return _axisProperties.Y.Scale;
      }
      set
      {
        _axisProperties.Y.Scale = value;
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
        this._axisProperties.Y.EhLinkedLayerAxesChanged(LinkedLayer.AxisProperties.X.Scale);
      else
        RescaleYAxis();
    }

    public void RescaleYAxis()
    {
      if (null == this.PlotItems)
        return; // can happen during deserialization

      // we have to disable our own Handler since if we change one DataBound of a association,
      //it generates a OnBoundaryChanged, and then all boundaries are merges into the axis boundary, 
      //but (alas!) not all boundaries are now of the new type!
      _plotAssociationYBoundariesChanged_EventSuspendCount++; 

      _axisProperties.Y.Scale.DataBoundsObject.BeginUpdate();
      _axisProperties.Y.Scale.DataBoundsObject.Reset();
      foreach(IGPlotItem pa in this.PlotItems)
      {
        if(pa is IYBoundsHolder)
        {
          // merge the bounds with x and yAxis
          ((IYBoundsHolder)pa).MergeYBoundsInto(_axisProperties.Y.Scale.DataBoundsObject); // merge all x-boundaries in the x-axis boundary object
        }
      }
      // take also the axis styles with physical values into account
      foreach (A2DAxisStyleIdentifier id in _scaleStyles.AxisStyleIDs)
      {
        if (id.AxisNumber == 1 && id.UsePhysicalValue)
          _axisProperties.Y.Scale.DataBoundsObject.Add(id.PhysicalValue);
      }

      _plotAssociationYBoundariesChanged_EventSuspendCount = Math.Max(0,_plotAssociationYBoundariesChanged_EventSuspendCount-1);
      _axisProperties.Y.Scale.DataBoundsObject.EndUpdate();
      _axisProperties.Y.Scale.ProcessDataBounds();
    }
    

    bool EhYAxisInterrogateBoundaryChangedEvent()
    {
      // do nothing here, for the future we can decide to change the linked axis boundaries
      return this.IsYAxisLinked;
    }


    /// <summary>
    /// Called by the proxy, when the instance of the linked layer has changed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void EhLinkedLayerInstanceChanged(object sender, object oldvalue, object newvalue)
    {
      XYPlotLayer oldValue = (XYPlotLayer)oldvalue;
      XYPlotLayer newValue = (XYPlotLayer)newvalue;
      if (!ReferenceEquals(oldValue, newValue))
      {
        // close the event handlers to the old layer
        if (null != oldValue)
        {
          oldValue.SizeChanged -= new System.EventHandler(EhLinkedLayerSizeChanged);
          oldValue.PositionChanged -= new System.EventHandler(EhLinkedLayerPositionChanged);
          oldValue.AxisProperties.ScalesChanged -= new System.EventHandler(EhLinkedLayerAxesChanged);
        }

        // link the events to the new layer
        if (null != newValue)
        {
          newValue.SizeChanged += new System.EventHandler(EhLinkedLayerSizeChanged);
          newValue.PositionChanged += new System.EventHandler(EhLinkedLayerPositionChanged);
          newValue.AxisProperties.ScalesChanged += new System.EventHandler(EhLinkedLayerAxesChanged);
        }
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

      try
      {
        if (_axisProperties.X.IsLinked && null != LinkedLayer)
        {
          _axisProperties.X.EhLinkedLayerAxesChanged(LinkedLayer.AxisProperties.X.Scale);
        }

        if (_axisProperties.Y.IsLinked && null != LinkedLayer)
        {
          _axisProperties.Y.EhLinkedLayerAxesChanged(LinkedLayer.AxisProperties.Y.Scale);
        }
      }
      catch (Exception )
      {
        string linkedlayername = this.LinkedLayer.Name;
        this.LinkedLayer = null;
        Current.Gui.ErrorMessageBox(string.Format("Link of layer {0} to layer {1} was removed, because the axes seem to be incompatible!", this.Name, linkedlayername));
      }
    }

    /*
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
        this.CoordinateSystem.LogicalToLayerCoordinates(relaxisval, relaltstart, out x1, out y1);
        this.CoordinateSystem.LogicalToLayerCoordinates(relaxisval, relaltend, out x2, out y2);
      }
      else
      {
        this.CoordinateSystem.LogicalToLayerCoordinates(relaltstart, relaxisval, out x1, out y1);
        this.CoordinateSystem.LogicalToLayerCoordinates(relaltend, relaxisval, out x2, out y2);
      }

      g.DrawLine(pen, (float)x1, (float)y1, (float)x2, (float)y2);
    }
    */


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



    public IEnumerable<A2DAxisStyleIdentifier> UsedAxisStyleIdentifier
    {
      get
      {
        foreach (AxisStyle style in this._scaleStyles.AxisStyles)
          yield return style.StyleID;
      }
    }
   
    
  

    
    


    private string GetAxisTitleString(A2DAxisStyleIdentifier id)
    {
      return _scaleStyles.AxisStyle(id) !=null && _scaleStyles.AxisStyle(id).Title != null ? _scaleStyles.AxisStyle(id).Title.Text : null; 
    }

    private void SetAxisTitleString(A2DAxisStyleIdentifier id, string value)
    {
      AxisStyle style = _scaleStyles.AxisStyle(id);
      string oldtitle = (style==null ||  style.Title == null) ? null : style.Title.Text;
      string newtitle = (value == null || value == String.Empty) ? null : value;

      if (newtitle != oldtitle)
      {
        if (newtitle == null)
        {
          if(style!=null)
            style.Title = null;
        }
        else if (_scaleStyles.AxisStyleEnsured(id).Title != null)
        {
          _scaleStyles.AxisStyle(id).Title.Text = newtitle;
        }
        else
        {
          TextGraphic tg = new TextGraphic();
          A2DAxisStyleInformation info = CoordinateSystem.GetAxisStyleInformation(id);

          // find out the position and orientation of the item
          double rx0 = 0, rx1 = 1, ry0 = 0, ry1 = 1;
          if (id.AxisNumber == 0)
            ry0 = ry1 = id.LogicalValue;
          else
            rx0 = rx1 = id.LogicalValue;

          PointF normDirection;
          PointF location = CoordinateSystem.GetNormalizedDirection(rx0, ry0, rx1, ry1, 0.5, info.PreferedLabelSide == A2DAxisSide.Left ? 90 : -90, out normDirection);
          double angle = Math.Atan2(normDirection.Y, normDirection.X) * 180 / Math.PI;

          float distance = 0;
          AxisStyle axisStyle = _scaleStyles.AxisStyle(id);
          if (null != axisStyle.AxisLineStyle)
            distance += axisStyle.AxisLineStyle.GetOuterDistance(info.PreferedLabelSide);
          float labelFontSize = 0;
          if (axisStyle.ShowMajorLabels)
            labelFontSize = Math.Max(labelFontSize, axisStyle.MajorLabelStyle.FontSize);
          if (axisStyle.ShowMinorLabels)
            labelFontSize = Math.Max(labelFontSize, axisStyle.MinorLabelStyle.FontSize);
          const float scaleFontWidth = 4;
          const float scaleFontHeight = 1.5f;

          if (angle <= 45 && angle >= -45)
          {
            //case EdgeType.Right:
            tg.Rotation = -90;
            tg.XAnchor = XAnchorPositionType.Center;
            tg.YAnchor = YAnchorPositionType.Top;
            distance += scaleFontWidth * labelFontSize;
          }
          else if (angle <= -45 && angle >= -135)
          {
            //case Top:
            tg.Rotation = 0;
            tg.XAnchor = XAnchorPositionType.Center;
            tg.YAnchor = YAnchorPositionType.Bottom;
            distance += scaleFontHeight * labelFontSize;
          }
          else if (angle <= 135 && angle >= 45)
          {
            //case EdgeType.Bottom:
            tg.Rotation = 0;
            tg.XAnchor = XAnchorPositionType.Center;
            tg.YAnchor = YAnchorPositionType.Top;
            distance += scaleFontHeight * labelFontSize;
          }
          else
          {
            //case EdgeType.Left:

            tg.Rotation = -90;
            tg.XAnchor = XAnchorPositionType.Center;
            tg.YAnchor = YAnchorPositionType.Bottom;
            distance += scaleFontWidth * labelFontSize;
          }

          tg.Position = new PointF(location.X + distance * normDirection.X, location.Y + distance * normDirection.Y);
          tg.Text = newtitle;
          _scaleStyles.AxisStyleEnsured(id).Title = tg;
        }
      }
    }

    public string DefaultYAxisTitleString
    {
      get
      {
        return GetAxisTitleString(A2DAxisStyleIdentifier.Y0);
      }
      set
      {
        SetAxisTitleString(A2DAxisStyleIdentifier.Y0, value);
      }
    }

   

   

    public string DefaultXAxisTitleString
    {
      get
      {
        return GetAxisTitleString(A2DAxisStyleIdentifier.X0);
      }
      set
      {
        SetAxisTitleString(A2DAxisStyleIdentifier.X0, value);
      }
    }


    #endregion // Style properties

    #region Painting and Hit testing

    /// <summary>
    /// This function is called by the graph document before _any_ layer is painted. We have to make sure that all of our cached data becomes valid.
    /// 
    /// </summary>

    public virtual void PreparePainting()
    {

      // update the logical values of the physical axes before
      foreach (A2DAxisStyleIdentifier id in _scaleStyles.AxisStyleIDs)
      {
        if (id.UsePhysicalValue)
        {
          // then update the logical value of this identifier
          double logicalValue = this._axisProperties.Scale(id.AxisNumber).PhysicalVariantToNormal(id.PhysicalValue);
          id.LogicalValue = logicalValue;
        }
      }
      
      // Before we paint the axis, we have to make sure that all plot items
      // had their data updated, so that the axes are updated before they are drawn!
      _plotItems.PrepareStyles(null);
      _plotItems.ApplyStyles(null);
      _plotItems.PreparePainting(this);
      
    }


    public virtual void Paint(Graphics g)
    {
      GraphicsState savedgstate = g.Save();
      //g.TranslateTransform(m_LayerPosition.X,m_LayerPosition.Y);
      //g.RotateTransform(m_LayerAngle);
      
      g.MultiplyTransform(_cachedForwardMatrix);

      if(_layerBackground!=null)
        _layerBackground.Draw(g,new RectangleF(0,0,_cachedLayerSize.Width,_cachedLayerSize.Height));
       
     

      RectangleF layerBounds = new RectangleF(_cachedLayerPosition,_cachedLayerSize);

      _scaleStyles.Paint(g, this);

      if (ClipDataToFrame)
      {
        g.SetClip(new RectangleF(new PointF(0, 0), this._cachedLayerSize));
      }
      foreach (IGPlotItem pi in _plotItems)
      {
        pi.Paint(g, this);
      }
      if (ClipDataToFrame)
      {
        g.ResetClip();
      }

      _graphObjects.DrawObjects(g, 1, this);

      if(_legend!=null)
        _legend.Paint(g,this);


      if(ClipDataToFrame)
      {
        g.SetClip(new RectangleF(new PointF(0,0),this._cachedLayerSize));
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


      List<GraphicBase> specObjects = new List<GraphicBase>();
      foreach(AxisStyle style in _scaleStyles.AxisStyles)
        specObjects.Add(style.Title);
      specObjects.Add(_legend);

      if (!plotItemsOnly)
      {

        // do the hit test first for the special objects of the layer
        foreach (GraphicBase go in specObjects)
        {
          if (null != go)
          {
            hit = go.HitTest(layerC);
            if (null != hit)
            {
              if (null == hit.Remove && (hit.HittedObject is GraphicBase))
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
        foreach (AxisStyle style in this._scaleStyles.AxisStyles)
        {
          if (style.ShowAxisLine && null != (hit = style.AxisLineStyle.HitTest(this, layerC, false)))
          {
            hit.DoubleClick = AxisScaleEditorMethod;
            return ForwardTransform(hit);
          }
        }


        // hit testing the axes - secondly now wiht the ticks
        // in this case the TitleAndFormat editor for the axis should be shown
        foreach (AxisStyle style in this._scaleStyles.AxisStyles)
        {
          if (style.ShowAxisLine && null != (hit = style.AxisLineStyle.HitTest(this, layerC, true)))
          {
            hit.DoubleClick = AxisStyleEditorMethod;
            return ForwardTransform(hit);
          }
        }
       

        // hit testing the axes labels
        foreach (AxisStyle style in this._scaleStyles.AxisStyles)
        {
          if (style.ShowAxisLine && null != (hit = style.MajorLabelStyle.HitTest(this, layerC)))
          {
            hit.DoubleClick = AxisLabelStyleEditorMethod;
            return ForwardTransform(hit);
          }
        }

      
        // now hit testing the other objects in the layer
        foreach (GraphicBase go in _graphObjects)
        {
          hit = go.HitTest(layerC);
          if (null != hit)
          {
            if (null == hit.Remove && (hit.HittedObject is GraphicBase))
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
      // first update out direct childs
      CoordinateSystem.UpdateAreaSize(this.Size);

      // now inform other listeners
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
      GraphicBase go = (GraphicBase)o.HittedObject;
      o.ParentLayer.GraphObjects.Remove(go);
      return true;
    }
    static bool EhTitlesOrLegend_Remove(IHitTestObject o)
    {
      GraphicBase go = (GraphicBase)o.HittedObject;
      XYPlotLayer layer = o.ParentLayer;

      if(object.ReferenceEquals(go,layer._legend))
      {
        layer._legend=null;
        return true;
      }
        foreach(AxisStyle style in layer._scaleStyles.AxisStyles)
        {
          if(object.ReferenceEquals(go, style.Title))
          {
            style.Title=null;
            return true;
          }
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
        _axisProperties.X.Scale.DataBoundsObject.BeginUpdate();
        _axisProperties.X.Scale.DataBoundsObject.Reset();
        foreach(IGPlotItem pa in this.PlotItems)
        {
          if(pa is IXBoundsHolder)
          {
            // merge the bounds with x and yAxis
            ((IXBoundsHolder)pa).MergeXBoundsInto(_axisProperties.X.Scale.DataBoundsObject); // merge all x-boundaries in the x-axis boundary object
          }
        }
        _axisProperties.X.Scale.DataBoundsObject.EndUpdate();
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
        _axisProperties.Y.Scale.DataBoundsObject.BeginUpdate();
        _axisProperties.Y.Scale.DataBoundsObject.Reset();
        foreach(IGPlotItem pa in this.PlotItems)
        {
          if(pa is IYBoundsHolder)
          {
            // merge the bounds with x and yAxis
            ((IYBoundsHolder)pa).MergeYBoundsInto(_axisProperties.Y.Scale.DataBoundsObject); // merge all x-boundaries in the x-axis boundary object
        
          }
        }
        _axisProperties.Y.Scale.DataBoundsObject.EndUpdate();
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

    public bool IsLinear { get { return XAxis is LinearScale && YAxis is LinearScale; }}
   

    G2DCoordinateSystem _coordinateSystem;
    public G2DCoordinateSystem CoordinateSystem
    {
      get
      {
        return _coordinateSystem;
      }
      set
      {
        G2DCoordinateSystem oldValue = _coordinateSystem;
        _coordinateSystem = value;

        if (oldValue != value)
        {
          _coordinateSystem.Parent = this;
          _coordinateSystem.UpdateAreaSize(this.Size);
          OnInvalidate();
        }



      }
    }

   

    

  




    #endregion

  
  }
}
