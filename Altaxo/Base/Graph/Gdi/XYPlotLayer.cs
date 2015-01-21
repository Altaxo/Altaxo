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

#endregion Copyright

using Altaxo.Collections;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Graph.Scales.Ticks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Altaxo.Graph.Gdi
{
	using Altaxo.Main.Properties;
	using Axis;
	using Plot;
	using Shapes;

	/// <summary>
	/// XYPlotLayer represents a rectangular area on the graph, which holds plot curves, axes and graphical elements.
	/// </summary>
	public partial class XYPlotLayer
		:
		HostLayer,
		IPlotArea
	{
		#region Member variables

		protected G2DCoordinateSystem _coordinateSystem;

		private ScaleCollection _scales;

		protected GridPlaneCollection _gridPlanes;

		protected AxisStyleCollection _axisStyles;

		/// <summary>If true, the data are clipped to the frame.</summary>
		protected LayerDataClipping _dataClipping = LayerDataClipping.StrictToCS;

		protected PlotItemCollection _plotItems;

		/// <summary>Number of times this event is disables, or 0 if it is enabled.</summary>
		[NonSerialized]
		private Main.SuspendableObject _plotAssociationXBoundariesChanged_EventSuspender = new Main.SuspendableObject();

		/// <summary>Number of times this event is disables, or 0 if it is enabled.</summary>
		[NonSerialized]
		private Main.SuspendableObject _plotAssociationYBoundariesChanged_EventSuspender = new Main.SuspendableObject();

		/// <summary>
		/// Partial list of all <see cref="PlaceHolder"/> instances in <see cref="HostLayer.GraphObjects"/>.
		/// </summary>
		[NonSerialized]
		private IObservableList<PlaceHolder> _placeHolders;

		[NonSerialized]
		private IObservableList<PlotItemPlaceHolder> _plotItemPlaceHolders;

		#endregion Member variables

		#region Constructors

		#region Copying

		/// <summary>
		/// The copy constructor.
		/// </summary>
		/// <param name="from"></param>
		public XYPlotLayer(XYPlotLayer from)
			: base(from)
		{
		}

		/// <summary>
		/// Internal copy from operation. It is presumed, that the events are already suspended. Additionally,
		/// it is not neccessary to call the OnChanged event, since this is called in the calling routine.
		/// </summary>
		/// <param name="obj">The object (layer) from which to copy.</param>
		/// <param name="options">Copy options.</param>
		protected override void InternalCopyFrom(HostLayer obj, GraphCopyOptions options)
		{
			base.InternalCopyFrom(obj, options); // base copy, but keep in mind that InternalCopyGraphItems is overridden in this class

			var from = obj as XYPlotLayer;
			if (null == from)
				return;

			if (0 != (options & GraphCopyOptions.CopyLayerScales))
			{
				this.CoordinateSystem = (G2DCoordinateSystem)from.CoordinateSystem.Clone();

				this.Scales = (ScaleCollection)from._scales.Clone();
				this._dataClipping = from._dataClipping;
			}

			// Coordinate Systems size must be updated in any case
			this.CoordinateSystem.UpdateAreaSize(this._cachedLayerSize);

			if (0 != (options & GraphCopyOptions.CopyLayerGrid))
			{
				this.GridPlanes = from._gridPlanes.Clone();
			}

			// Styles

			if (0 != (options & GraphCopyOptions.CopyLayerAxes))
			{
				this.AxisStyles = (AxisStyleCollection)from._axisStyles.Clone();
			}

			// Plot items
			if (0 != (options & GraphCopyOptions.CopyLayerPlotItems))
			{
				this.PlotItems = null == from._plotItems ? null : new PlotItemCollection(this, from._plotItems);
			}
			else if (0 != (options & GraphCopyOptions.CopyLayerPlotStyles))
			{
				// TODO apply the styles from from._plotItems to the PlotItems here
				this.PlotItems.CopyFrom(from._plotItems, options);
			}
		}

		protected override void InternalCopyGraphItems(HostLayer from, GraphCopyOptions options)
		{
			bool bGraphItems = options.HasFlag(GraphCopyOptions.CopyLayerGraphItems);
			bool bChildLayers = options.HasFlag(GraphCopyOptions.CopyChildLayers);
			bool bLegends = options.HasFlag(GraphCopyOptions.CopyLayerLegends);

			var criterium = new Func<IGraphicBase, bool>(x =>
			{
				if (x is HostLayer)
					return bChildLayers;

				if (x is LegendText)
					return bLegends;

				return bGraphItems;
			});

			InternalCopyGraphItems(from, options, criterium);
		}

		public override object Clone()
		{
			return new XYPlotLayer(this);
		}

		#endregion Copying

		/// <summary>
		/// Constructor for deserialization purposes only.
		/// </summary>
		protected XYPlotLayer(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
			: base(info)
		{
			this.CoordinateSystem = new CS.G2DCartesicCoordinateSystem();
			this.AxisStyles = new AxisStyleCollection();
			this.Scales = new ScaleCollection();
			this.Location = new ItemLocationDirect();
			this.GridPlanes = new GridPlaneCollection();
			this.GridPlanes.Add(new GridPlane(CSPlaneID.Front));
		}

		public XYPlotLayer(HostLayer parentLayer)
			: this(parentLayer, GetChildLayerDefaultLocation(), new CS.G2DCartesicCoordinateSystem())
		{
		}

		public XYPlotLayer(HostLayer parentLayer, G2DCoordinateSystem coordinateSystem)
			: this(parentLayer, GetChildLayerDefaultLocation(), coordinateSystem)
		{
		}

		/// <summary>
		/// Creates a layer at the designated <paramref name="location"/>
		/// </summary>
		/// <param name="parentLayer">The parent layer of the constructed layer.</param>
		/// <param name="location">The location of the constructed layer.</param>
		public XYPlotLayer(HostLayer parentLayer, IItemLocation location)
			: this(parentLayer, location, new CS.G2DCartesicCoordinateSystem())
		{
		}

		/// <summary>
		/// Creates a layer at the provided <paramref name="location"/>.
		/// </summary>
		/// <param name="parentLayer">The parent layer of the newly created layer.</param>
		/// <param name="location">The position of the layer on the printable area in points (1/72 inch).</param>
		/// <param name="coordinateSystem">The coordinate system to use for the layer.</param>
		public XYPlotLayer(HostLayer parentLayer, IItemLocation location, G2DCoordinateSystem coordinateSystem)
			: base(parentLayer, location)
		{
			this.CoordinateSystem = coordinateSystem;
			this.AxisStyles = new AxisStyleCollection();
			this.Scales = new ScaleCollection();
			this.GridPlanes = new GridPlaneCollection();
			this.GridPlanes.Add(new GridPlane(CSPlaneID.Front));
			this.PlotItems = new PlotItemCollection(this);
		}

		#endregion Constructors

		private void EnsureAppropriateGridAndAxisStylePlaceHolders()
		{
			var gridIndex = _graphObjects.IndexOfFirst(x => x is GridPlanesPlaceHolder);
			if (gridIndex < 0)
			{
				_graphObjects.Insert(0, new GridPlanesPlaceHolder());
				gridIndex = 0;
			}
			else
			{
				// make sure that no more GridPlanesPlaceHolders are in the collection
				_graphObjects.RemoveWhere((item, i) => (i > gridIndex) && (item is GridPlanesPlaceHolder));
			}

			// we try to place AxisStyleLinePlaceHolders after the GridPlanesPlaceHolder
			// find the first of any placeholders for axis line styles, store as insert position, if not found use insert position after grid place holder
			// from i= to count look for placeholders for axis line styles i, if not found insert it at insert position, store next position as insert position

			var insertIdx = gridIndex + 1;
			insertIdx = InsertMissingAxisStylePlaceHolders<AxisStyleLinePlaceHolder>(insertIdx);
			insertIdx = InsertMissingAxisStylePlaceHolders<AxisStyleMajorLabelPlaceHolder>(insertIdx);
			insertIdx = InsertMissingAxisStylePlaceHolders<AxisStyleMinorLabelPlaceHolder>(insertIdx);
			insertIdx = InsertMissingAxisStylePlaceHolders<AxisStyleTitlePlaceHolder>(insertIdx);

			// remove superfluous place holders
			int maxCount = _axisStyles.Count;
			_graphObjects.RemoveWhere(x => { var s = x as AxisStylePlaceHolderBase; return null != s && s.Index > maxCount; });
		}

		private int InsertMissingAxisStylePlaceHolders<T>(int insertIdx) where T : AxisStylePlaceHolderBase, new()
		{
			for (int i = 0; i < _axisStyles.Count; ++i)
			{
				var idx = _graphObjects.IndexOfFirst(x => x is T && (x as T).Index == i);
				if (idx >= 0)
				{
					insertIdx = idx + 1;
				}
				else
				{
					_graphObjects.Insert(insertIdx, new T { Index = i });
					++insertIdx;
				}
			}
			return insertIdx;
		}

		#region IPlotLayer methods

		public bool Is3D { get { return false; } }

		public Scale ZAxis { get { return null; } }

		public Scale GetScale(int i)
		{
			return _scales.Scale(i);
		}

		public Logical3D GetLogical3D(I3DPhysicalVariantAccessor acc, int idx)
		{
			Logical3D r;
			r.RX = XAxis.PhysicalVariantToNormal(acc.GetXPhysical(idx));
			r.RY = YAxis.PhysicalVariantToNormal(acc.GetYPhysical(idx));
			r.RZ = Is3D ? ZAxis.PhysicalVariantToNormal(acc.GetZPhysical(idx)) : 0;
			return r;
		}

		/// <summary>
		/// Returns a list of the used axis style ids for this layer.
		/// </summary>
		public System.Collections.Generic.IEnumerable<CSLineID> AxisStyleIDs
		{
			get { return AxisStyles.AxisStyleIDs; }
		}

		/// <summary>
		/// Updates the logical value of a plane id in case it uses a physical value.
		/// </summary>
		/// <param name="id">The plane identifier</param>
		public void UpdateCSPlaneID(CSPlaneID id)
		{
			if (id.UsePhysicalValue)
			{
				double l = this.Scales.Scale(id.PerpendicularAxisNumber).PhysicalVariantToNormal(id.PhysicalValue);
				id.LogicalValue = l;
			}
		}

		#endregion IPlotLayer methods

		#region XYPlotLayer properties and methods

		/// <summary>
		/// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
		/// to change a plot so that the plot items refer to another table.
		/// </summary>
		/// <param name="Report">Function that reports the found <see cref="C:Altaxo.Main.DocNodeProxy"/> instances to the visitor.</param>
		public override void VisitDocumentReferences(Altaxo.Main.DocNodeProxyReporter Report)
		{
			base.VisitDocumentReferences(Report);
			PlotItems.VisitDocumentReferences(Report);
		}

		/// <summary>
		/// Collection of the axis styles for the left, bottom, right, and top axis.
		/// </summary>
		public AxisStyleCollection AxisStyles
		{
			get
			{
				return _axisStyles;
			}
			protected set
			{
				AxisStyleCollection oldvalue = _axisStyles;
				_axisStyles = value;
				value.ParentObject = this;
				value.UpdateCoordinateSystem(this.CoordinateSystem);

				if (!object.ReferenceEquals(oldvalue, value))
				{
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		public ScaleCollection Scales
		{
			get
			{
				return _scales;
			}
			protected set
			{
				if (object.ReferenceEquals(value, _scales))
					return;

				ScaleCollection oldscales = _scales;
				_scales = value;

				if (null != _scales)
				{
					_scales.ParentObject = this;
				}

				using (var suspendToken = SuspendGetToken())
				{
					for (int i = 0; i < _scales.Count; i++)
					{
						Scale oldScale = oldscales == null ? null : oldscales[i].Scale;
						Scale newScale = _scales[i].Scale;
						if (!object.ReferenceEquals(oldScale, newScale))
							EhSelfChanged(new ScaleInstanceChangedEventArgs(oldScale, newScale) { ScaleIndex = i });
					}

					if (null != oldscales)
						oldscales.Dispose();

					suspendToken.Resume();
				}
			}
		}

		public TextGraphic Legend
		{
			get
			{
				return (LegendText)_graphObjects.FirstOrDefault(item => item is LegendText);
			}
			set
			{
				var idx = _graphObjects.IndexOfFirst(item => item is LegendText);
				TextGraphic oldvalue = idx >= 0 ? (TextGraphic)_graphObjects[idx] : null;

				if (value != null)
				{
					if (idx < 0)
						_graphObjects.Add(value);
					else
						_graphObjects[idx] = value;
				}
				else
				{
					if (idx >= 0)
						_graphObjects.RemoveAt(idx);
				}

				if (!object.ReferenceEquals(value, oldvalue))
				{
					EhSelfChanged(EventArgs.Empty);
				}
			}
		}

		public override void Remove(GraphicBase go)
		{
			if (_axisStyles.Remove(go))
				return;
			else
				base.Remove(go);
		}

		protected override void OnGraphObjectsCollectionInstanceInitialized()
		{
			base.OnGraphObjectsCollectionInstanceInitialized();

			if (null != _placeHolders)
			{
			}
			_placeHolders = _graphObjects.CreatePartialViewOfType<PlaceHolder>();
			_plotItemPlaceHolders = _graphObjects.CreatePartialViewOfType<PlotItemPlaceHolder>();
			if (null != _placeHolders)
			{
			}
		}

		public PlotItemCollection PlotItems
		{
			get
			{
				return _plotItems;
			}
			protected set
			{
				if (null == value)
					throw new ArgumentNullException("value");

				if (ChildSetMember(ref _plotItems, value))
					EhSelfChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Clears all legends from this layer.
		/// </summary>
		public void ClearLegends()
		{
			for (int i = this.GraphObjects.Count - 1; i >= 0; --i)
			{
				if (GraphObjects[i] is LegendText)
					GraphObjects.RemoveAt(i);
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
			if (PlotItems.Flattened.Length == 0)
			{
				ClearLegends();
				EhSelfChanged(EventArgs.Empty);

				return;
			}

			TextGraphic tgo;

			var existingLegendIndex = GraphObjects.IndexOfFirst(x => x is LegendText);
			var existingLegend = existingLegendIndex >= 0 ? (LegendText)GraphObjects[existingLegendIndex] : null;

			if (existingLegend != null)
				tgo = new TextGraphic(existingLegend);
			else
				tgo = new TextGraphic(this.GetPropertyContext());

			System.Text.StringBuilder strg = new System.Text.StringBuilder();
			for (int i = 0; i < this.PlotItems.Flattened.Length; i++)
			{
				strg.AppendFormat("{0}\\L({1}) \\%({2})", (i == 0 ? "" : "\r\n"), i, i);
			}
			tgo.Text = strg.ToString();

			// if the position of the old legend is outside, use a new position
			if (null == existingLegend || existingLegend.Position.X < 0 || existingLegend.Position.Y < 0 ||
				existingLegend.Position.X > this.Size.X || existingLegend.Position.Y > this.Size.Y)
				tgo.Position = new PointD2D(0.1 * this.Size.X, 0.1 * this.Size.Y);
			else
				tgo.Position = existingLegend.Position;

			if (existingLegendIndex >= 0)
				GraphObjects[existingLegendIndex] = tgo;
			else
				GraphObjects.Add(tgo);

			EhSelfChanged(EventArgs.Empty);
		}

		/// <summary>
		/// This will create the default axes styles that are given by the coordinate system.
		/// </summary>
		public void CreateDefaultAxes(IReadOnlyPropertyBag context)
		{
			foreach (CSAxisInformation info in CoordinateSystem.AxisStyles)
			{
				if (info.IsShownByDefault)
				{
					this.AxisStyles.CreateDefault(info.Identifier, context);

					if (info.HasTitleByDefault)
					{
						this.SetAxisTitleString(info.Identifier, info.Identifier.ParallelAxisNumber == 0 ? "X axis" : "Y axis");
					}
				}
			}
		}

		#endregion XYPlotLayer properties and methods

		#region Scale related

		public TickSpacing XTicks
		{
			get
			{
				return Scales[0].TickSpacing;
			}
		}

		public TickSpacing YTicks
		{
			get
			{
				return Scales[1].TickSpacing;
			}
		}

		/// <summary>Gets or sets the x axis of this layer.</summary>
		/// <value>The x axis of the layer.</value>
		public Scale XAxis
		{
			get
			{
				return _scales.X.Scale;
			}
			set
			{
				_scales.X.Scale = value;
			}
		}

		/// <summary>Indicates if x axis is linked to the linked layer x axis.</summary>
		/// <value>True if x axis is linked to the linked layer x axis.</value>
		public bool IsXAxisLinked
		{
			get
			{
				return this._scales.X.Scale is LinkedScale;
			}
		}

		private bool EhXAxisInterrogateBoundaryChangedEvent()
		{
			// do nothing here, for the future we can decide to change the linked axis boundaries
			return this.IsXAxisLinked;
		}

		public void RescaleXAxis()
		{
			if (null == this.PlotItems)
				return; // can happen during deserialization

			var scaleBounds = _scales.X.Scale.DataBoundsObject;
			if (null != scaleBounds)
			{
				// we have to disable our own Handler since by calling MergeXBoundsInto, it is possible that the type of DataBound of the plot item has to change, and that
				// generates a OnBoundaryChanged event, and by handling this the boundaries of all other plot items are merged into the axis boundary,
				// but (alas!) not all boundaries are now of the new type!
				using (var xBoundariesChangedSuspendToken = _plotAssociationXBoundariesChanged_EventSuspender.SuspendGetToken())
				{
					using (var suspendToken = scaleBounds.SuspendGetToken())
					{
						scaleBounds.Reset();
						foreach (IGPlotItem pa in this.PlotItems)
						{
							if (pa is IXBoundsHolder)
							{
								// merge the bounds with x and yAxis
								((IXBoundsHolder)pa).MergeXBoundsInto(scaleBounds); // merge all x-boundaries in the x-axis boundary object
							}
						}

						// take also the axis styles with physical values into account
						foreach (CSLineID id in _axisStyles.AxisStyleIDs)
						{
							if (id.ParallelAxisNumber != 0 && id.UsePhysicalValueOtherFirst)
								scaleBounds.Add(id.PhysicalValueOtherFirst);
						}

						suspendToken.Resume();
					}
					xBoundariesChangedSuspendToken.Resume();
				}
				_scales.X.Scale.Rescale();
			}
			// _linkedScales.X.Scale.ProcessDataBounds();
		}

		/// <summary>Gets or sets the y axis of this layer.</summary>
		/// <value>The y axis of the layer.</value>
		public Scale YAxis
		{
			get
			{
				return _scales.Y.Scale;
			}
			set
			{
				_scales.Y.Scale = value;
			}
		}

		/// <summary>Indicates if y axis is linked to the linked layer y axis.</summary>
		/// <value>True if y axis is linked to the linked layer y axis.</value>
		public bool IsYAxisLinked
		{
			get
			{
				return this._scales.Y.Scale is LinkedScale;
			}
		}

		public void RescaleYAxis()
		{
			if (null == this.PlotItems)
				return; // can happen during deserialization

			var scaleBounds = _scales.Y.Scale.DataBoundsObject;

			if (null != scaleBounds)
			{
				// we have to disable our own Handler since if we change one DataBound of a association,
				//it generates a OnBoundaryChanged, and then all boundaries are merges into the axis boundary,
				//but (alas!) not all boundaries are now of the new type!
				using (var yBoundariesChangedSuspendToken = _plotAssociationYBoundariesChanged_EventSuspender.SuspendGetToken())
				{
					using (var suspendToken = scaleBounds.SuspendGetToken())
					{
						scaleBounds.Reset();
						foreach (IGPlotItem pa in this.PlotItems)
						{
							if (pa is IYBoundsHolder)
							{
								// merge the bounds with x and yAxis
								((IYBoundsHolder)pa).MergeYBoundsInto(scaleBounds); // merge all x-boundaries in the x-axis boundary object
							}
						}
						// take also the axis styles with physical values into account
						foreach (CSLineID id in _axisStyles.AxisStyleIDs)
						{
							if (id.ParallelAxisNumber == 0 && id.UsePhysicalValueOtherFirst)
								scaleBounds.Add(id.PhysicalValueOtherFirst);
							else if (id.ParallelAxisNumber == 2 && id.UsePhysicalValueOtherSecond)
								scaleBounds.Add(id.PhysicalValueOtherSecond);
						}

						suspendToken.Resume();
					}
					yBoundariesChangedSuspendToken.Resume();
				}
				_scales.Y.Scale.Rescale();
			}
			// _linkedScales.Y.Scale.ProcessDataBounds();
		}

		private bool EhYAxisInterrogateBoundaryChangedEvent()
		{
			// do nothing here, for the future we can decide to change the linked axis boundaries
			return this.IsYAxisLinked;
		}

		/// <summary>
		/// Ensures that all linked scales have their scalesLinkedTo instances updated (in case the layer instance or the scale instance has changed in the meantime).
		/// Note that here we should not enforce the link properties (like xOrg = SomeCalculation depending on scaleLinkedTo). This is done later, after the plot items are updated
		/// </summary>
		protected void UpdateScaleLinks()
		{
			foreach (var swt in Scales.Where(s => s.Scale is LinkedScale))
			{
				UpdateScaleLink(swt);
			}
		}

		/// <summary>
		/// Updates the scale link of a <see cref="ScaleWithTicks"/> where the Scale is of type <see cref="LinkedScale"/>
		/// </summary>
		/// <param name="swt">The <see cref="ScaleWithTicks"/> instance.</param>
		/// <remarks>
		/// <para>This updates either the scaleLinkedTo and/or the scale number and layer number.</para>
		/// <para>The scaleLinkedTo has precedence: if it still exist in any of the sibling layers, the layer number and scale number will be updated and the scaleLinked to will be preserved</para>
		/// <para>The other case is when the scaleLinkedTo no longer exists in any of the sibling layers: then it is tried to find a layer with the stored layer number and the scale with the stored scale number</para>
		/// <para>If both cases fail, then the scale is transformed from a linked scale to a normal scale.</para>
		/// </remarks>
		protected void UpdateScaleLink(ScaleWithTicks swt)
		{
			/*
			LinkedScale ls = (LinkedScale)swt.Scale;
			Scale scaleLinkedTo = ls.ScaleLinkedTo;

			var layerLinkedTo = Main.DocumentPath.GetRootNodeImplementing<XYPlotLayer>(scaleLinkedTo);
			int layerLinkedToIndex, scaleLinkedToIndex;

			if (layerLinkedTo != null &&
					object.ReferenceEquals(this.ParentLayer, layerLinkedTo.ParentLayer) &&
					!object.ReferenceEquals(this, layerLinkedTo) &&
					(scaleLinkedToIndex = layerLinkedTo.Scales.IndexOfFirst(x => object.ReferenceEquals(x.Scale, scaleLinkedTo))) >= 0
				)
			{
				// then we have the first case: the linked layer still exist
				layerLinkedToIndex = layerLinkedTo.LayerNumber;
				ls.LinkedLayerIndex = layerLinkedToIndex;
				ls.LinkedScaleIndex = scaleLinkedToIndex;
				return; // first case handled
			}

			// we assume the second case and try to find a layer with the stored layer index, and therein a scale with the stored scale index
			layerLinkedToIndex = ls.LinkedLayerIndex;
			scaleLinkedToIndex = ls.LinkedScaleIndex;

			scaleLinkedTo = null;
			layerLinkedTo = null;
			if (layerLinkedToIndex >= 0 && layerLinkedToIndex < ParentLayerList.Count)
				layerLinkedTo = ParentLayerList[layerLinkedToIndex] as XYPlotLayer;

			if (null != layerLinkedTo && scaleLinkedToIndex >= 0 && scaleLinkedToIndex < layerLinkedTo.Scales.Count)
				scaleLinkedTo = layerLinkedTo.Scales[scaleLinkedToIndex].Scale;

			if (scaleLinkedTo != null)
			{
				ls.ScaleLinkedTo = scaleLinkedTo;
				return; // second case successfully handled
			}

			// both cases fail, so we must convert the linked scale to a normal scale
			swt.Scale = ls.WrappedScale; // set the scale to the wrapped scale
			ls.ScaleLinkedTo = null; // free the event wiring
			*/
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

		#endregion Scale related

		#region Style properties

		public LayerDataClipping ClipDataToFrame
		{
			get
			{
				return _dataClipping;
			}
			set
			{
				LayerDataClipping oldvalue = _dataClipping;
				_dataClipping = value;

				if (value != oldvalue)
					EhSelfChanged(EventArgs.Empty);
			}
		}

		public GridPlaneCollection GridPlanes
		{
			get
			{
				return _gridPlanes;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException();

				if (ChildSetMember(ref _gridPlanes, value))
					EhSelfChanged(EventArgs.Empty);
			}
		}

		private string GetAxisTitleString(CSLineID id)
		{
			return _axisStyles[id] != null && _axisStyles[id].Title != null ? _axisStyles[id].Title.Text : null;
		}

		private void SetAxisTitleString(CSLineID id, string value)
		{
			AxisStyle style = _axisStyles[id];
			string oldtitle = (style == null || style.Title == null) ? null : style.Title.Text;
			string newtitle = (value == null || value == String.Empty) ? null : value;

			if (newtitle != oldtitle)
			{
				if (newtitle == null)
				{
					if (style != null)
						style.Title = null;
				}
				else if (_axisStyles.AxisStyleEnsured(id).Title != null)
				{
					_axisStyles[id].Title.Text = newtitle;
				}
				else
				{
					TextGraphic tg = new TextGraphic(this.GetPropertyContext());
					tg.SetParentSize(this.Size, false);

					CSAxisInformation info = CoordinateSystem.GetAxisStyleInformation(id);

					// find out the position and orientation of the item
					double rx0 = 0, rx1 = 1, ry0 = 0, ry1 = 1;
					if (id.ParallelAxisNumber == 0)
						ry0 = ry1 = id.LogicalValueOtherFirst;
					else
						rx0 = rx1 = id.LogicalValueOtherFirst;

					PointD2D normDirection;
					Logical3D tdirection = CoordinateSystem.GetLogicalDirection(info.Identifier.ParallelAxisNumber, info.PreferedLabelSide);
					var location = CoordinateSystem.GetNormalizedDirection(new Logical3D(rx0, ry0), new Logical3D(rx1, ry1), 0.5, tdirection, out normDirection);
					double angle = Math.Atan2(normDirection.Y, normDirection.X) * 180 / Math.PI;

					tg.Location.ParentAnchorX = RADouble.NewRel(location.X / this.Size.X); // set the x anchor of the parent
					tg.Location.ParentAnchorY = RADouble.NewRel(location.Y / this.Size.Y); // set the y anchor of the parent

					double distance = 0;
					AxisStyle axisStyle = _axisStyles[id];
					if (null != axisStyle.AxisLineStyle)
						distance += axisStyle.AxisLineStyle.GetOuterDistance(info.PreferedLabelSide);
					double labelFontSize = 0;
					if (axisStyle.AreMajorLabelsEnabled)
						labelFontSize = Math.Max(labelFontSize, axisStyle.MajorLabelStyle.FontSize);
					if (axisStyle.AreMinorLabelsEnabled)
						labelFontSize = Math.Max(labelFontSize, axisStyle.MinorLabelStyle.FontSize);
					const double scaleFontWidth = 4;
					const double scaleFontHeight = 1.5;

					if (-45 <= angle && angle <= 45)
					{
						//case EdgeType.Right:
						tg.Rotation = 90;
						tg.Location.LocalAnchorX = RADouble.NewRel(0.5); // Center
						tg.Location.LocalAnchorY = RADouble.NewRel(0); // Top
						distance += scaleFontWidth * labelFontSize;
					}
					else if (-135 <= angle && angle <= -45)
					{
						//case Top:
						tg.Rotation = 0;
						tg.Location.LocalAnchorX = RADouble.NewRel(0.5); // Center
						tg.Location.LocalAnchorY = RADouble.NewRel(1); // Bottom
						distance += scaleFontHeight * labelFontSize;
					}
					else if (45 <= angle && angle <= 135)
					{
						//case EdgeType.Bottom:
						tg.Rotation = 0;
						tg.Location.LocalAnchorX = RADouble.NewRel(0.5); // Center
						tg.Location.LocalAnchorY = RADouble.NewRel(0); // Top
						distance += scaleFontHeight * labelFontSize;
					}
					else
					{
						//case EdgeType.Left:

						tg.Rotation = 90;
						tg.Location.LocalAnchorX = RADouble.NewRel(0.5); // Center
						tg.Location.LocalAnchorY = RADouble.NewRel(1); // Bottom
						distance += scaleFontWidth * labelFontSize;
					}

					tg.Location.PositionX = RADouble.NewAbs(distance * normDirection.X); // because this is relative to the reference point, we don't need to take the location into account here, it is set above
					tg.Location.PositionY = RADouble.NewAbs(distance * normDirection.Y);
					tg.Text = newtitle;
					_axisStyles.AxisStyleEnsured(id).Title = tg;
				}
			}
		}

		public string DefaultYAxisTitleString
		{
			get
			{
				return GetAxisTitleString(CSLineID.Y0);
			}
			set
			{
				SetAxisTitleString(CSLineID.Y0, value);
			}
		}

		public string DefaultXAxisTitleString
		{
			get
			{
				return GetAxisTitleString(CSLineID.X0);
			}
			set
			{
				SetAxisTitleString(CSLineID.X0, value);
			}
		}

		#endregion Style properties

		#region Painting and Hit testing

		/// <summary>
		/// This function is called by the graph document before _any_ layer is painted. We have to make sure that all of our cached data becomes valid.
		///
		/// </summary>

		private void EnsureAppropriatePlotItemPlaceHolders()
		{
			using (var token = _graphObjects.GetEventDisableToken())
			{
				if (0 == _plotItemPlaceHolders.Count) // take special measures if not one plot item place holder -> this can happen when deserializing old versions prior to the introduction of place holders
				{
					InsertTheVeryFirstPlotItemPlaceHolder();
				}

				int idx = -1;
				int maxIdx = _plotItemPlaceHolders.Count;

				foreach (var ele in Altaxo.Collections.TreeNodeExtensions.TakeFromHereToLeavesWithIndex<IGPlotItem>(
					_plotItems,
					0,
					true, x => x is PlotItemCollection ? ((PlotItemCollection)x).ChildIndexDirection : IndexDirection.Ascending))
				{
					PlotItemPlaceHolder placeHolder;
					++idx;
					if (idx < maxIdx)
						placeHolder = _plotItemPlaceHolders[idx];
					else
						_plotItemPlaceHolders.Add(placeHolder = new PlotItemPlaceHolder());

					placeHolder.PlotItemParent = ele.Item1.ParentCollection;
					placeHolder.PlotItemIndex = ele.Item2;
				}

				// items from 0 including to idx are in use, thus we can delete items from idx-1 up to the end

				for (int i = _plotItemPlaceHolders.Count - 1; i > idx; --i)
					_plotItemPlaceHolders.RemoveAt(i);
			}
		}

		/// <summary>
		/// Inserts the very first plot item place holder at the right place. It is not tested if this is the first - Thus make sure that no plot item place holder is there.
		/// </summary>
		private void InsertTheVeryFirstPlotItemPlaceHolder()
		{
			// we should place the first plot item place holder after all of these types:
			// Background, all kind of axis styles

			var predicate = new Func<IGraphicBase, int, bool>(
				(item, i) =>
				{
					return (item is GridPlanesPlaceHolder) || (item is AxisStylePlaceHolderBase);
				}
				);

			int idx = _graphObjects.IndexOfLast(predicate);

			if (idx < 0)
				idx = 0; // insert at first position if nothing was found
			else
				++idx; // insert after the found item

			_graphObjects.Insert(idx, new PlotItemPlaceHolder());
		}

		public override void PaintPreprocessing(object parentObject)
		{
			base.PaintPreprocessing(parentObject);

			UpdateScaleLinks();

			// Before we paint the axis, we have to make sure that all plot items
			// had their data updated, so that the axes are updated before they are drawn!
			_plotItems.PrepareScales(this);

			// after deserialisation the data bounds object of the scale is empty:
			// then we have to rescale the axis
			if (Scales.X.Scale.DataBoundsObject.IsEmpty)
				RescaleXAxis();
			if (Scales.Y.Scale.DataBoundsObject.IsEmpty)
				RescaleYAxis();

			_plotItems.PrepareGroupStyles(null, this);
			_plotItems.ApplyGroupStyles(null);

			_axisStyles.PaintPreprocessing(this);

			EnsureAppropriateGridAndAxisStylePlaceHolders();
			EnsureAppropriatePlotItemPlaceHolders();
		}

		protected override void PaintInternal(Graphics g)
		{
			// paint the background very first
			_gridPlanes.PaintBackground(g, this);

			// then paint the graph items
			base.PaintInternal(g);
		}

		/// <summary>
		/// This function is called when painting is finished. Can be used to release the resources
		/// not neccessary any more.
		/// </summary>
		public override void PaintPostprocessing()
		{
			_plotItems.PaintPostprocessing();

			base.PaintPostprocessing();
		}

		public override IHitTestObject HitTest(HitTestPointData parentHitTestData, bool plotItemsOnly)
		{
			IHitTestObject hit;

			// first test the items in the child layers, since they are plotted on top of our own items
			if (null != (hit = base.HitTest(parentHitTestData, plotItemsOnly)))
				return hit;

			/*
				if (plotItemsOnly)
				{
					HitTestPointData layerHitTestData = parentHitTestData.NewFromAdditionalTransformation(_transformation);
					var layerCoord = layerHitTestData.GetHittedPointInWorldCoord();
					if (null != (hit = _plotItems.HitTest(this, layerCoord)))
					{
						if (hit.ParentLayer == null)
							hit.ParentLayer = this;

						if (hit.DoubleClick == null)
							hit.DoubleClick = PlotItemEditorMethod;
						return ForwardTransform(hit);
					}
				}
			 */

			if (plotItemsOnly)
			{
				HitTestPointData localCoord = parentHitTestData.NewFromAdditionalTransformation(this._transformation);

				// hit testing all graph objects, this is done in reverse order compared to the painting, so the "upper" items are found first.
				for (int i = _graphObjects.Count - 1; i >= 0; --i)
				{
					var plotItemPlaceHolder = _graphObjects[i] as PlotItemPlaceHolder;
					if (null == plotItemPlaceHolder)
						continue;

					hit = plotItemPlaceHolder.HitTest(localCoord);
					if (null != hit)
					{
						hit.ParentLayer = this;
						return ForwardTransform(hit);
					}
				}
			}

			return null;
		}

		#endregion Painting and Hit testing

		#region Editor methods

		public static DoubleClickHandler AxisScaleEditorMethod;
		public static DoubleClickHandler AxisStyleEditorMethod;
		public static DoubleClickHandler AxisLabelMajorStyleEditorMethod;
		public static DoubleClickHandler AxisLabelMinorStyleEditorMethod;
		public static DoubleClickHandler PlotItemEditorMethod;

		private bool EhAxisLabelMajorStyleRemove(IHitTestObject o)
		{
			AxisLabelStyle als = o.HittedObject as AxisLabelStyle;
			AxisStyle axisStyle = als == null ? null : als.ParentObject as AxisStyle;
			if (axisStyle != null)
			{
				axisStyle.HideMajorLabels();
				return true;
			}
			return false;
		}

		private bool EhAxisLabelMinorStyleRemove(IHitTestObject o)
		{
			AxisLabelStyle als = o.HittedObject as AxisLabelStyle;
			AxisStyle axisStyle = als == null ? null : als.ParentObject as AxisStyle;
			if (axisStyle != null)
			{
				axisStyle.HideMinorLabels();
				return true;
			}
			return false;
		}

		#endregion Editor methods

		#region Event firing

		protected override void OnSizeChanged()
		{
			// first update out direct childs
			if (null != CoordinateSystem)
				CoordinateSystem.UpdateAreaSize(this.Size);

			base.OnSizeChanged();
		}

		#endregion Event firing

		#region Handler of child events

		protected override void OnChanged(EventArgs e)
		{
			if (e is BoundariesChangedEventArgs)
				EhBoundaryChangedEventFromPlotItem((BoundariesChangedEventArgs)e);
			else if (e is ScaleInstanceChangedEventArgs)
				EhScaleInstanceChanged((ScaleInstanceChangedEventArgs)e);

			base.OnChanged(e);
		}

		protected void EhBoundaryChangedEventFromPlotItem(BoundariesChangedEventArgs boundaryChangedEventArgs)
		{
			var data = boundaryChangedEventArgs.Data;
			if (data.HasFlag(BoundariesChangedData.XBoundariesChanged))
				EhXBoundaryChangedEventFromPlotItem();
			if (data.HasFlag(BoundariesChangedData.YBoundariesChanged))
				EhYBoundaryChangedEventFromPlotItem();
		}

		/// <summary>
		/// This handler is called if a x-boundary from any of the plotassociations of this layer
		/// has changed. We then have to recalculate the boundaries.
		/// </summary>
		/// <remarks>Unfortunately we do not know if the boundary is extended or shrinked, if is is extended
		/// if would be possible to merge only the changed boundary into the x-axis boundary.
		/// But since we don't know about that, we have to completely recalculate the boundary be using the boundaries of
		/// all PlotAssociations of this layer.</remarks>
		protected void EhXBoundaryChangedEventFromPlotItem()
		{
			if (!_plotAssociationXBoundariesChanged_EventSuspender.IsSuspended)
			{
				// now we have to inform all the PlotAssociations that a new axis was loaded
				using (var suspendToken = _scales.X.Scale.DataBoundsObject.SuspendGetToken())
				{
					_scales.X.Scale.DataBoundsObject.Reset();
					foreach (IGPlotItem pa in this.PlotItems)
					{
						if (pa is IXBoundsHolder)
						{
							// merge the bounds with x and yAxis
							((IXBoundsHolder)pa).MergeXBoundsInto(_scales.X.Scale.DataBoundsObject); // merge all x-boundaries in the x-axis boundary object
						}
					}
					suspendToken.Resume();
				}
			}
		}

		/// <summary>
		/// This handler is called if a y-boundary from any of the plotassociations of this layer
		/// has changed. We then have to recalculate the boundaries.
		/// </summary>
		/// <remarks>Unfortunately we do not know if the boundary is extended or shrinked, if is is extended
		/// if would be possible to merge only the changed boundary into the y-axis boundary.
		/// But since we don't know about that, we have to completely recalculate the boundary be using the boundaries of
		/// all PlotAssociations of this layer.</remarks>
		protected void EhYBoundaryChangedEventFromPlotItem()
		{
			if (!_plotAssociationYBoundariesChanged_EventSuspender.IsSuspended)
			{
				// now we have to inform all the PlotAssociations that a new axis was loaded
				using (var suspendToken = _scales.Y.Scale.DataBoundsObject.SuspendGetToken())
				{
					_scales.Y.Scale.DataBoundsObject.Reset();
					foreach (IGPlotItem pa in this.PlotItems)
					{
						if (pa is IYBoundsHolder)
						{
							// merge the bounds with x and yAxis
							((IYBoundsHolder)pa).MergeYBoundsInto(_scales.Y.Scale.DataBoundsObject); // merge all x-boundaries in the x-axis boundary object
						}
					}
					suspendToken.Resume();
				}
			}
		}

		/// <summary>
		/// Absorbs the event from the ScaleCollection and distributes it further.
		/// </summary>
		/// <param name="e">The event data of the scale.</param>
		private void EhScaleInstanceChanged(ScaleInstanceChangedEventArgs e)
		{
			if (object.ReferenceEquals(_scales.X.Scale, e.NewScale))
				RescaleXAxis();

			if (object.ReferenceEquals(_scales.Y.Scale, e.NewScale))
				RescaleYAxis();
		}

		#endregion Handler of child events

		#region IDocumentNode Members

		/// <summary>
		/// retrieves the object with the name <code>name</code>.
		/// </summary>
		/// <param name="name">The objects name.</param>
		/// <returns>The object with the specified name.</returns>
		public override Main.IDocumentLeafNode GetChildObjectNamed(string name)
		{
			if (name == "PlotItems")
				return _plotItems;

			return base.GetChildObjectNamed(name);
		}

		/// <summary>
		/// Retrieves the name of the provided object.
		/// </summary>
		/// <param name="o">The object for which the name should be found.</param>
		/// <returns>The name of the object. Null if the object is not found. String.Empty if the object is found but has no name.</returns>
		public override string GetNameOfChildObject(Main.IDocumentLeafNode o)
		{
			if (object.ReferenceEquals(_plotItems, o))
				return "PlotItems";

			return base.GetNameOfChildObject(o);
		}

		private IEnumerable<Main.DocumentNodeAndName> GetMyDocumentNodeChildrenWithName()
		{
			if (null != _scales)
				yield return new Main.DocumentNodeAndName(_scales, "Scales");

			if (null != _plotItems)
				yield return new Main.DocumentNodeAndName(_plotItems, "PlotItems");

			if (null != _axisStyles)
				yield return new Main.DocumentNodeAndName(_axisStyles, "AxisStyles");

			if (null != _gridPlanes)
				yield return new Main.DocumentNodeAndName(_gridPlanes, "Grids");

			if (null != _coordinateSystem)
				yield return new Main.DocumentNodeAndName(_coordinateSystem, "CoordinateSystem");
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			return GetMyDocumentNodeChildrenWithName().Concat(base.GetDocumentNodeChildrenWithName());
		}

		protected override void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				ChildDisposeMember(ref _scales);
				ChildDisposeMember(ref _plotItems);
				ChildDisposeMember(ref _axisStyles);
				ChildDisposeMember(ref _gridPlanes);
				ChildDisposeMember(ref _coordinateSystem);
			}
			base.Dispose(isDisposing);
		}

		#endregion IDocumentNode Members

		#region Inner types

		public bool IsLinear { get { return XAxis is LinearScale && YAxis is LinearScale; } }

		public G2DCoordinateSystem CoordinateSystem
		{
			get
			{
				return _coordinateSystem;
			}
			set
			{
				if (null == value)
					throw new ArgumentNullException();

				if (object.ReferenceEquals(_coordinateSystem, value))
					return;

				_coordinateSystem = value;
				_coordinateSystem.ParentObject = this;

				_coordinateSystem.UpdateAreaSize(this.Size);

				if (null != AxisStyles)
					AxisStyles.UpdateCoordinateSystem(value);

				EhSelfChanged(EventArgs.Empty);
			}
		}

		#endregion Inner types

		#region Old types no longer in use but needed for deserialization

		/// <summary>
		/// AxisStylesSummary collects all styles that correspond to one axis scale (i.e. either x-axis or y-axis)
		/// in one class. This contains the grid style of the axis, and one or more axis styles
		/// </summary>
		private class ScaleStyle
		{
			private GridStyle _gridStyle;
			private List<AxisStyle> _axisStyles;

			//G2DCoordinateSystem _cachedCoordinateSystem;

			#region Serialization

			[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLayerAxisStylesSummary", 0)]
			private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
			{
				public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
				{
					throw new NotSupportedException("Serialization of old versions not supported - probably a programming error");
					/*
					XYPlotLayerAxisStylesSummary s = (XYPlotLayerAxisStylesSummary)obj;
					info.AddValue("Grid", s._gridStyle);

					info.CreateArray("Edges", s._edges.Length);
					for (int i = 0; i < s._edges.Length; ++i)
						info.AddEnum("e", s._edges[i]);
					info.CommitArray();

					info.CreateArray("AxisStyles",s._axisStyles.Length);
					for(int i=0;i<s._axisStyles.Length;++i)
						info.AddValue("e",s._axisStyles[i]);
					info.CommitArray();
					*/
				}

				public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
				{
					ScaleStyle s = SDeserialize(o, info, parent);
					return s;
				}

				protected virtual ScaleStyle SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
				{
					ScaleStyle s = null != o ? (ScaleStyle)o : new ScaleStyle();

					s.GridStyle = (GridStyle)info.GetValue("Grid", s);

					int count = info.OpenArray();
					//s._edges = new EdgeType[count];
					for (int i = 0; i < count; ++i)
						info.GetEnum("e", typeof(EdgeType));
					info.CloseArray(count);

					count = info.OpenArray();
					//s._axisStyles = new XYPlotLayerAxisStyleProperties[count];
					for (int i = 0; i < count; ++i)
						s._axisStyles.Add((AxisStyle)info.GetValue("e", s));
					info.CloseArray(count);

					return s;
				}
			}

			// 2006-09-08 - renaming to G2DScaleStyle
			[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ScaleStyle), 1)]
			private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
			{
				public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
				{
					throw new NotImplementedException("Serialization of old versions is not supported");
					/*
					ScaleStyle s = (ScaleStyle)obj;

					info.AddValue("Grid", s._gridStyle);

					info.CreateArray("AxisStyles", s._axisStyles.Count);
					for (int i = 0; i < s._axisStyles.Count; ++i)
						info.AddValue("e", s._axisStyles[i]);
					info.CommitArray();
					*/
				}

				public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
				{
					ScaleStyle s = SDeserialize(o, info, parent);
					return s;
				}

				protected virtual ScaleStyle SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
				{
					ScaleStyle s = null != o ? (ScaleStyle)o : new ScaleStyle();

					s.GridStyle = (GridStyle)info.GetValue("Grid", s);

					int count = info.OpenArray();
					//s._axisStyles = new XYPlotLayerAxisStyleProperties[count];
					for (int i = 0; i < count; ++i)
						s._axisStyles.Add((AxisStyle)info.GetValue("e", s));
					info.CloseArray(count);

					return s;
				}
			}

			#endregion Serialization

			/// <summary>
			/// Default constructor. Defines neither a grid style nor an axis style.
			/// </summary>
			public ScaleStyle()
			{
				_axisStyles = new List<AxisStyle>();
			}

			private void CopyFrom(ScaleStyle from)
			{
				if (object.ReferenceEquals(this, from))
					return;

				this.GridStyle = from._gridStyle == null ? null : (GridStyle)from._gridStyle.Clone();

				this._axisStyles.Clear();
				for (int i = 0; i < _axisStyles.Count; ++i)
				{
					this.AddAxisStyle((AxisStyle)from._axisStyles[i].Clone());
				}
			}

			public void AddAxisStyle(AxisStyle value)
			{
				if (value != null)
				{
					_axisStyles.Add(value);
				}
			}

			public void RemoveAxisStyle(CSLineID id)
			{
				int idx = -1;
				for (int i = 0; i < _axisStyles.Count; i++)
				{
					if (_axisStyles[i].StyleID == id)
					{
						idx = i;
						break;
					}
				}

				if (idx > 0)
					_axisStyles.RemoveAt(idx);
			}

			public AxisStyle AxisStyleEnsured(CSLineID id)
			{
				AxisStyle prop = AxisStyle(id);
				if (prop == null)
				{
					prop = new AxisStyle(id, false, false, false, null, null);
					// prop.CachedAxisInformation = _cachedCoordinateSystem.GetAxisStyleInformation(id);
					AddAxisStyle(prop);
				}
				return prop;
			}

			public bool ContainsAxisStyle(CSLineID id)
			{
				return null != AxisStyle(id);
			}

			public AxisStyle AxisStyle(CSLineID id)
			{
				foreach (AxisStyle p in _axisStyles)
					if (p.StyleID == id)
						return p;

				return null;
			}

			public IEnumerable<AxisStyle> AxisStyles
			{
				get
				{
					return _axisStyles;
				}
			}

			public GridStyle GridStyle
			{
				get { return _gridStyle; }
				set
				{
					GridStyle oldvalue = _gridStyle;
					_gridStyle = value;
				}
			}
		}

		/// <summary>
		/// This class holds the (normally two for 2D) AxisStylesSummaries - for every axis scale one summary.
		/// </summary>
		private class G2DScaleStyleCollection
		{
			private ScaleStyle[] _styles;

			#region Serialization

			[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.XYPlotLayerAxisStylesSummaryCollection", 0)]
			// 2006-09-08 renamed to G2DScaleStyleCollection
			[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(G2DScaleStyleCollection), 1)]
			private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
			{
				public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
				{
					throw new NotImplementedException("Serialization of old versions is not supported");
					/*
					G2DScaleStyleCollection s = (G2DScaleStyleCollection)obj;

					info.CreateArray("Styles", s._styles.Length);
					for (int i = 0; i < s._styles.Length; ++i)
						info.AddValue("e", s._styles[i]);
					info.CommitArray();
					*/
				}

				public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
				{
					G2DScaleStyleCollection s = SDeserialize(o, info, parent);
					return s;
				}

				protected virtual G2DScaleStyleCollection SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
				{
					G2DScaleStyleCollection s = null != o ? (G2DScaleStyleCollection)o : new G2DScaleStyleCollection();

					int count = info.OpenArray();
					s._styles = new ScaleStyle[count];
					for (int i = 0; i < count; ++i)
						s.SetScaleStyle((ScaleStyle)info.GetValue("e", s), i);
					info.CloseArray(count);

					return s;
				}
			}

			#endregion Serialization

			public G2DScaleStyleCollection()
			{
				_styles = new ScaleStyle[2];

				this._styles[0] = new ScaleStyle();

				this._styles[1] = new ScaleStyle();
			}

			/// <summary>
			/// Return the axis style with the given id. If this style is not present, the return value is null.
			/// </summary>
			/// <param name="id"></param>
			/// <returns></returns>
			public AxisStyle AxisStyle(CSLineID id)
			{
				ScaleStyle scaleStyle = _styles[id.ParallelAxisNumber];
				return scaleStyle.AxisStyle(id);
			}

			/// <summary>
			/// This will return an axis style with the given id. If not present, this axis style will be created, added to the collection, and returned.
			/// </summary>
			/// <param name="id"></param>
			/// <returns></returns>
			public AxisStyle AxisStyleEnsured(CSLineID id)
			{
				ScaleStyle scaleStyle = _styles[id.ParallelAxisNumber];
				return scaleStyle.AxisStyleEnsured(id);
			}

			public void RemoveAxisStyle(CSLineID id)
			{
				ScaleStyle scaleStyle = _styles[id.ParallelAxisNumber];
				scaleStyle.RemoveAxisStyle(id);
			}

			public IEnumerable<AxisStyle> AxisStyles
			{
				get
				{
					for (int i = 0; i < _styles.Length; i++)
					{
						foreach (AxisStyle style in _styles[i].AxisStyles)
							yield return style;
					}
				}
			}

			public IEnumerable<CSLineID> AxisStyleIDs
			{
				get
				{
					for (int i = 0; i < _styles.Length; i++)
					{
						foreach (AxisStyle style in _styles[i].AxisStyles)
							yield return style.StyleID;
					}
				}
			}

			public bool ContainsAxisStyle(CSLineID id)
			{
				ScaleStyle scalestyle = _styles[id.ParallelAxisNumber];
				return scalestyle.ContainsAxisStyle(id);
			}

			public ScaleStyle ScaleStyle(int i)
			{
				return _styles[i];
			}

			public void SetScaleStyle(ScaleStyle value, int i)
			{
				if (i < 0)
					throw new ArgumentOutOfRangeException("Index i is negative");
				if (i >= _styles.Length)
					throw new ArgumentOutOfRangeException("Index i is greater than length of internal array");

				ScaleStyle oldvalue = _styles[i];
				_styles[i] = value;
			}

			public ScaleStyle X
			{
				get
				{
					return _styles[0];
				}
			}

			public ScaleStyle Y
			{
				get
				{
					return _styles[1];
				}
			}
		}

		#endregion Old types no longer in use but needed for deserialization

		#region IGraphicShape placeholder for items in XYPlotLayer

		private abstract class PlaceHolder : Main.SuspendableDocumentLeafNodeWithEventArgs, IGraphicBase
		{
			/// <summary>
			/// Determines whether this graphical object is compatible with the parent specified in the argument.
			/// </summary>
			/// <param name="parentObject">The parent object.</param>
			/// <returns>
			///   <c>True</c> if this object is compatible with the parent object; otherwise <c>false</c>.
			/// </returns>
			public virtual bool IsCompatibleWithParent(object parentObject)
			{
				return parentObject is XYPlotLayer;
			}

			public virtual void SetParentSize(PointD2D parentSize, bool isTriggeringChangedEvent)
			{
			}

			public virtual IHitTestObject HitTest(HitTestPointData hitData)
			{
				return null;
			}

			public virtual void PaintPreprocessing(object parentObject)
			{
				if (!object.ReferenceEquals(parentObject, this.ParentObject))
					throw new InvalidOperationException(string.Format("Cached parentObject and parentObject in document do not match! This={0}, ParentCached={1}, ParentArg={2}", this.GetType(), ParentObject, parentObject));
			}

			public abstract void Paint(Graphics g, object obj);

			public virtual PointD2D Position
			{
				get
				{
					return new PointD2D(0, 0);
				}
				set
				{
				}
			}

			public virtual bool CopyFrom(object obj)
			{
				if (object.ReferenceEquals(this, obj))
					return true;
				var from = obj as PlaceHolder;
				if (null != from)
				{
					//this.ParentObject = from.ParentObject;
					return true;
				}
				return false;
			}

			public abstract object Clone();
		}

		private abstract class AxisStylePlaceHolderBase : PlaceHolder
		{
			public int Index { get; set; }

			public override bool CopyFrom(object obj)
			{
				if (!base.CopyFrom(obj))
					return false;

				var from = obj as AxisStylePlaceHolderBase;
				if (null != from)
					this.Index = from.Index;

				return true;
			}

			/// <summary>
			/// Gets the axis style this place holder substitutes.
			/// </summary>
			/// <returns></returns>
			protected AxisStyle GetAxisStyle()
			{
				var layer = ParentObject as XYPlotLayer;
				if (null != layer && Index >= 0 && Index < layer._axisStyles.Count)
					return layer._axisStyles.ItemAt(Index);
				else
					return null;
			}
		}

		private class AxisStyleLinePlaceHolder : AxisStylePlaceHolderBase
		{
			#region Serialization

			/// <summary>
			/// 2013-11-27 Initial version
			/// </summary>
			[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AxisStyleLinePlaceHolder), 0)]
			private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
			{
				public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
				{
					var s = (AxisStyleLinePlaceHolder)obj;
					info.AddValue("Index", s.Index);
				}

				public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
				{
					var s = null != o ? (AxisStyleLinePlaceHolder)o : new AxisStyleLinePlaceHolder();
					s.Index = info.GetInt32("Index");
					return s;
				}
			}

			#endregion Serialization

			public override string ToString()
			{
				return string.Format("Axis line style #{0}", Index);
			}

			public override object Clone()
			{
				var r = new AxisStyleLinePlaceHolder();
				r.CopyFrom(this);
				return r;
			}

			public override void Paint(Graphics g, object obj)
			{
				var layer = ParentObject as XYPlotLayer;
				if (null != layer && Index >= 0 && Index < layer._axisStyles.Count)
					layer._axisStyles.ItemAt(Index).PaintLine(g, layer);
			}

			public override IHitTestObject HitTest(HitTestPointData hitData)
			{
				IHitTestObject hit = null;
				var layer = ParentObject as XYPlotLayer;
				if (null != layer && Index >= 0 && Index < layer._axisStyles.Count)
				{
					var axisStyle = layer._axisStyles.ItemAt(Index);
					if (axisStyle.IsAxisLineEnabled && null != (hit = axisStyle.AxisLineStyle.HitTest(layer, hitData.GetHittedPointInWorldCoord(), false)))
					{
						hit.DoubleClick = AxisScaleEditorMethod;
						return hit;
					}

					// hit testing the axes - secondly now with the ticks
					// in this case the TitleAndFormat editor for the axis should be shown
					if (axisStyle.IsAxisLineEnabled && null != (hit = axisStyle.AxisLineStyle.HitTest(layer, hitData.GetHittedPointInWorldCoord(), true)))
					{
						hit.DoubleClick = AxisStyleEditorMethod;
						return hit;
					}
				}
				return base.HitTest(hitData);
			}

			public override PointD2D Position // Position of the line is fixed
			{
				get
				{
					return new PointD2D(0, 0);
				}
				set
				{
				}
			}
		}

		private class AxisStyleMajorLabelPlaceHolder : AxisStylePlaceHolderBase
		{
			#region Serialization

			/// <summary>
			/// 2013-11-27 Initial version
			/// </summary>
			[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AxisStyleMajorLabelPlaceHolder), 0)]
			private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
			{
				public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
				{
					var s = (AxisStyleMajorLabelPlaceHolder)obj;
					info.AddValue("Index", s.Index);
				}

				public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
				{
					var s = null != o ? (AxisStyleMajorLabelPlaceHolder)o : new AxisStyleMajorLabelPlaceHolder();
					s.Index = info.GetInt32("Index");
					return s;
				}
			}

			#endregion Serialization

			public override string ToString()
			{
				return string.Format("Axis major labels #{0}", Index);
			}

			public override object Clone()
			{
				var r = new AxisStyleMajorLabelPlaceHolder();
				r.CopyFrom(this);
				return r;
			}

			public override IHitTestObject HitTest(HitTestPointData hitData)
			{
				IHitTestObject hit = null;
				var layer = ParentObject as XYPlotLayer;
				if (null != layer && Index >= 0 && Index < layer._axisStyles.Count)
				{
					var axisStyle = layer._axisStyles.ItemAt(Index);
					if (axisStyle.AreMajorLabelsEnabled && null != (hit = axisStyle.MajorLabelStyle.HitTest(layer, hitData.GetHittedPointInWorldCoord())))
					{
						hit.DoubleClick = AxisLabelMajorStyleEditorMethod;
						hit.Remove = layer.EhAxisLabelMajorStyleRemove;
						return hit;
					}
				}
				return base.HitTest(hitData);
			}

			public override void Paint(Graphics g, object obj)
			{
				var layer = ParentObject as XYPlotLayer;
				if (null != layer)
				{
					if (Index >= 0 && Index < layer._axisStyles.Count)
						layer._axisStyles.ItemAt(Index).PaintMajorLabels(g, layer);
				}
			}

			public override PointD2D Position // Position of the line is fixed
			{
				get
				{
					return new PointD2D(0, 0);
				}
				set
				{
				}
			}
		}

		private class AxisStyleMinorLabelPlaceHolder : AxisStylePlaceHolderBase
		{
			#region Serialization

			/// <summary>
			/// 2013-11-27 Initial version
			/// </summary>
			[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AxisStyleMinorLabelPlaceHolder), 0)]
			private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
			{
				public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
				{
					var s = (AxisStyleMinorLabelPlaceHolder)obj;
					info.AddValue("Index", s.Index);
				}

				public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
				{
					var s = null != o ? (AxisStyleMinorLabelPlaceHolder)o : new AxisStyleMinorLabelPlaceHolder();
					s.Index = info.GetInt32("Index");
					return s;
				}
			}

			#endregion Serialization

			public override string ToString()
			{
				return string.Format("Axis minor labels #{0}", Index);
			}

			public override object Clone()
			{
				var r = new AxisStyleMinorLabelPlaceHolder();
				r.CopyFrom(this);
				return r;
			}

			public override IHitTestObject HitTest(HitTestPointData hitData)
			{
				IHitTestObject hit = null;
				var layer = ParentObject as XYPlotLayer;
				if (null != layer && Index >= 0 && Index < layer._axisStyles.Count)
				{
					var axisStyle = layer._axisStyles.ItemAt(Index);
					if (axisStyle.AreMinorLabelsEnabled && null != (hit = axisStyle.MinorLabelStyle.HitTest(layer, hitData.GetHittedPointInWorldCoord())))
					{
						hit.DoubleClick = AxisLabelMinorStyleEditorMethod;
						hit.Remove = layer.EhAxisLabelMinorStyleRemove;
						return hit;
					}
				}
				return base.HitTest(hitData);
			}

			public override void Paint(Graphics g, object obj)
			{
				var layer = ParentObject as XYPlotLayer;
				if (null != layer)
				{
					if (Index >= 0 && Index < layer._axisStyles.Count)
						layer._axisStyles.ItemAt(Index).PaintMinorLabels(g, layer);
				}
			}

			public override PointD2D Position // Position of the line is fixed
			{
				get
				{
					return new PointD2D(0, 0);
				}
				set
				{
				}
			}
		}

		private class AxisStyleTitlePlaceHolder : AxisStylePlaceHolderBase
		{
			#region Serialization

			/// <summary>
			/// 2013-11-27 Initial version
			/// </summary>
			[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AxisStyleTitlePlaceHolder), 0)]
			private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
			{
				public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
				{
					var s = (AxisStyleTitlePlaceHolder)obj;
					info.AddValue("Index", s.Index);
				}

				public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
				{
					var s = null != o ? (AxisStyleTitlePlaceHolder)o : new AxisStyleTitlePlaceHolder();
					s.Index = info.GetInt32("Index");
					return s;
				}
			}

			#endregion Serialization

			public AxisStyleTitlePlaceHolder()
			{
			}

			public override string ToString()
			{
				return string.Format("Axis title #{0}", Index);
			}

			public override object Clone()
			{
				var r = new AxisStyleTitlePlaceHolder();
				r.CopyFrom(this);
				return r;
			}

			public override void SetParentSize(PointD2D parentSize, bool isTriggeringChangedEvent)
			{
				var layer = ParentObject as XYPlotLayer;
				if (null != layer && null != layer._axisStyles)
				{
					if (Index >= 0 && Index < layer._axisStyles.Count)
					{
						var title = layer._axisStyles.ItemAt(Index).Title;
						if (null != title)
							title.SetParentSize(parentSize, isTriggeringChangedEvent);
					}
				}
			}

			public override IHitTestObject HitTest(HitTestPointData hitData)
			{
				IHitTestObject hit = null;
				var layer = ParentObject as XYPlotLayer;
				if (null != layer && Index >= 0 && Index < layer._axisStyles.Count)
				{
					var axisStyle = layer._axisStyles.ItemAt(Index);
					if (null != axisStyle && null != axisStyle.Title && null != (hit = axisStyle.Title.HitTest(hitData)))
					{
						if (null == hit.Remove)
							hit.Remove = EhRemoveAxisStyleTitle;
						return hit;
					}
				}
				return base.HitTest(hitData);
			}

			private static bool EhRemoveAxisStyleTitle(IHitTestObject o)
			{
				GraphicBase go = (GraphicBase)o.HittedObject;
				var layer = o.ParentLayer as XYPlotLayer;
				if (null != layer)
				{
					foreach (AxisStyle style in layer._axisStyles)
					{
						if (object.ReferenceEquals(go, style.Title))
						{
							style.Title = null;
							return true;
						}
					}
				}
				return false;
			}

			public override void Paint(Graphics g, object obj)
			{
				var layer = ParentObject as XYPlotLayer;
				if (null != layer)
				{
					if (Index >= 0 && Index < layer._axisStyles.Count)
						layer._axisStyles.ItemAt(Index).PaintTitle(g, layer);
				}
			}

			public override PointD2D Position // Position of the line is fixed
			{
				get
				{
					var layer = ParentObject as XYPlotLayer;
					if (null != layer && Index >= 0 && Index < layer._axisStyles.Count)
						return layer._axisStyles.ItemAt(Index).Title.Position;
					else
						return PointD2D.Empty;
				}
				set
				{
				}
			}
		}

		private class GridPlanesPlaceHolder : PlaceHolder
		{
			#region Serialization

			/// <summary>
			/// 2013-11-27 Initial version
			/// </summary>
			[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GridPlanesPlaceHolder), 0)]
			private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
			{
				public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
				{
					var s = (GridPlanesPlaceHolder)obj;
				}

				public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
				{
					var s = null != o ? (GridPlanesPlaceHolder)o : new GridPlanesPlaceHolder();
					return s;
				}
			}

			#endregion Serialization

			public override string ToString()
			{
				return string.Format("Grid plane(s)");
			}

			public override void Paint(Graphics g, object obj)
			{
				var layer = ParentObject as XYPlotLayer;
				if (null != layer)
				{
					layer._gridPlanes.PaintGrid(g, layer);
				}
			}

			public override object Clone()
			{
				var r = new GridPlanesPlaceHolder();
				r.CopyFrom(this);
				return r;
			}
		}

		private class PlotItemPlaceHolder : PlaceHolder
		{
			public PlotItemCollection PlotItemParent { get; set; }

			public int PlotItemIndex { get; set; }

			#region Serialization

			/// <summary>
			/// 2013-11-27 Initial version
			/// </summary>
			[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PlotItemPlaceHolder), 0)]
			private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
			{
				public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
				{
					// Note there is no need to serialize PlotItemParent nor PlotItemIndex, since these variables are used only temporarily
				}

				public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
				{
					var s = null != o ? (PlotItemPlaceHolder)o : new PlotItemPlaceHolder();
					return s;
				}
			}

			#endregion Serialization

			public override bool CopyFrom(object obj)
			{
				if (!base.CopyFrom(obj))
					return false;

				var from = obj as PlotItemPlaceHolder;
				if (null != from)
				{
					this.PlotItemParent = from.PlotItemParent;
					this.PlotItemIndex = from.PlotItemIndex;
				}

				return true;
			}

			public override string ToString()
			{
				if (null == PlotItemParent)
				{
					return "Plot item collection (root)";
				}
				else
				{
					if (0 <= PlotItemIndex && PlotItemIndex < PlotItemParent.Count)
					{
						var pi = PlotItemParent[PlotItemIndex];
						if (pi is PlotItemCollection)
							return "Plot item collection";
						else
							return "Plot item " + pi.ToString();
					}
				}
				return "Plot item";
			}

			public override void Paint(Graphics g, object obj)
			{
				var layer = ParentObject as XYPlotLayer;
				if (null != layer)
				{
					if (layer.ClipDataToFrame == LayerDataClipping.StrictToCS)
					{
						g.Clip = layer.CoordinateSystem.GetRegion();
					}

					if (null == PlotItemParent) // this is the root plot item of the layer
					{
						if (layer._plotItems == null)
						{
							throw new InvalidOperationException("The member _plotItems is null on this layer!");
						}

						layer._plotItems.Paint(g, layer, null, null);
					}
					else
					{
						PlotItemParent.PaintChild(g, layer, PlotItemIndex);
					}

					if (layer.ClipDataToFrame == LayerDataClipping.StrictToCS)
					{
						g.ResetClip();
					}
				}
			}

			public override object Clone()
			{
				var r = new PlotItemPlaceHolder();
				r.CopyFrom(this);
				return r;
			}

			public override IHitTestObject HitTest(HitTestPointData hitData)
			{
				if (null == PlotItemParent)
					return null;
				if (PlotItemIndex < 0 || PlotItemIndex >= PlotItemParent.Count)
					return null;
				var layer = ParentObject as IPlotArea;
				if (null == layer)
					return null;

				var result = PlotItemParent[PlotItemIndex].HitTest(layer, hitData.GetHittedPointInWorldCoord());
				if (null != result)
				{
					if (null == result.DoubleClick)
						result.DoubleClick = PlotItemEditorMethod;
					if (null == result.Remove)
						result.Remove = PlotItemParent.EhHitTestObject_Remove;
				}
				return result;
			}
		}

		private class LegendText : TextGraphic
		{
			#region Serialization

			/// <summary>
			/// 2013-11-27 Initial version
			/// </summary>
			[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LegendText), 0)]
			private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
			{
				public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
				{
					var s = (LegendText)obj;
					info.AddBaseValueEmbedded(s, typeof(LegendText).BaseType);
				}

				public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
				{
					var s = null != o ? (LegendText)o : new LegendText(info);
					info.GetBaseValueEmbedded(s, typeof(LegendText).BaseType, this);
					return s;
				}
			}

			#endregion Serialization

			public LegendText(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
				: base(context)
			{
			}

			protected LegendText(Serialization.Xml.IXmlDeserializationInfo info)
				: base(info)
			{
			}

			public LegendText(TextGraphic from)
				: base(from)
			{
			}

			public override string ToString()
			{
				return string.Format("Legend text");
			}
		}

		#endregion IGraphicShape placeholder for items in XYPlotLayer
	}
}