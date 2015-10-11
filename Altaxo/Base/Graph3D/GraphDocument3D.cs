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

using Altaxo.Graph;
using Altaxo.Graph3D.GraphicsContext;
using Altaxo.Main;
using Altaxo.Main.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Graph3D
{
	public class GraphDocument3D
		:
		Main.SuspendableDocumentNodeWithSingleAccumulatedData<EventArgs>,
		IProjectItem,
		System.ICloneable,
		IChangedEventSource,
		Main.INameOwner,
		Main.Properties.IPropertyBagOwner
	{
		// following default unit is point (1/72 inch)
		/// <summary>For the graph elements all the units are in points. One point is 1/72 inch.</summary>
		protected const float UnitPerInch = 72;

		protected const double DefaultRootLayerSizeX = 697.68054;
		protected const double DefaultRootLayerSizeY = 451.44;
		protected const double DefaultRootLayerSizeZ = 451.44;

		#region Member variables

		private HostLayer3D _rootLayer;

		private string _name;

		private SceneSettings _sceneSettings;

		/// <summary>
		/// The date/time of creation of this graph.
		/// </summary>
		protected DateTime _creationTime;

		/// <summary>
		/// The date/time when this graph was changed.
		/// </summary>
		protected DateTime _lastChangeTime;

		/// <summary>
		/// Notes concerning this graph.
		/// </summary>
		protected Main.TextBackedConsole _notes;

		/// <summary>
		/// An identifier that can be shown on the graph and that is searchable.
		/// </summary>
		protected string _graphIdentifier;

		/// <summary>
		/// The graph properties, key is a string, value is a property (arbitrary object) you want to store here.
		/// </summary>
		/// <remarks>The properties are saved on disc (with exception of those that starts with "tmp/".
		/// If the property you want to store is only temporary, the properties name should therefore
		/// start with "tmp/".</remarks>
		protected Main.Properties.PropertyBag _graphProperties;

		/// <summary>Events which are fired from this thread are not distributed.</summary>
		[NonSerialized]
		private volatile System.Threading.Thread _paintThread;

		/// <summary>Event fired if the size of this document (i.e. the size of the root layer) changed.</summary>
		[field: NonSerialized]
		public event EventHandler SizeChanged;

		/// <summary>The root layer size, cached here only for deciding whether to raise the <see cref="SizeChanged"/> event. Do not use it otherwise.</summary>
		[NonSerialized]
		private VectorD3D _cachedRootLayerSize;

		[NonSerialized]
		private bool _isFixupInternalDataStructuresActive;

		/// <summary>This flag is set if during fixup anything has changed.</summary>
		[NonSerialized]
		private bool _hasFixupChangedAnything;

		#endregion Member variables

		#region Properties and Property-Keys

		public static FontX3D GetDefaultFont(IReadOnlyPropertyBag context)
		{
			var font = Altaxo.Graph.Gdi.GraphDocument.GetDefaultFont(context);

			return new FontX3D(font, font.Size * 0.0625);
		}

		public static double GetDefaultPenWidth(IReadOnlyPropertyBag context)
		{
			return Altaxo.Graph.Gdi.GraphDocument.GetDefaultPenWidth(context);
		}

		public static Altaxo.Graph.NamedColor GetDefaultForeColor(IReadOnlyPropertyBag context)
		{
			return Altaxo.Graph.Gdi.GraphDocument.GetDefaultForeColor(context);
		}

		public static NamedColor GetDefaultBackColor(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
		{
			return Altaxo.Graph.Gdi.GraphDocument.GetDefaultBackColor(context);
		}

		public static NamedColor GetDefaultPlotColor(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
		{
			return Altaxo.Graph.Gdi.GraphDocument.GetDefaultPlotColor(context);
		}

		/// <summary>
		/// Gets the default plot symbol size for all graphics in this graph, using the provided property context.
		/// </summary>
		/// <param name="context">The property context.</param>
		/// <returns>Default plot symbol size in points (1/72 inch).</returns>
		public static double GetDefaultSymbolSize(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
		{
			return Altaxo.Graph.Gdi.GraphDocument.GetDefaultSymbolSize(context);
		}

		public static double GetDefaultMajorTickLength(IReadOnlyPropertyBag context)
		{
			return Altaxo.Graph.Gdi.GraphDocument.GetDefaultMajorTickLength(context);
		}

		#endregion Properties and Property-Keys

		#region Construction / Copying

		/// <summary>
		/// Creates a empty GraphDocument with no layers and a standard size of A4 landscape.
		/// </summary>
		public GraphDocument3D()
		{
			_creationTime = _lastChangeTime = DateTime.UtcNow;
			_notes = new TextBackedConsole() { ParentObject = this };
			_sceneSettings = new SceneSettings();
			this.RootLayer = new HostLayer3D() { ParentObject = this };
			this.RootLayer.Location = new ItemLocationDirect3D
			{
				SizeX = RADouble.NewAbs(DefaultRootLayerSizeX),
				SizeY = RADouble.NewAbs(DefaultRootLayerSizeY),
				SizeZ = RADouble.NewAbs(DefaultRootLayerSizeZ)
			};
			_sceneSettings = new SceneSettings() { ParentObject = this };
		}

		public GraphDocument3D(GraphDocument3D from)
		{
			using (var suppressToken = SuspendGetToken())
			{
				_creationTime = _lastChangeTime = DateTime.UtcNow;
				this.RootLayer = new HostLayer3D(null, new ItemLocationDirect3D { SizeX = RADouble.NewAbs(DefaultRootLayerSizeX), SizeY = RADouble.NewAbs(DefaultRootLayerSizeY), SizeZ = RADouble.NewAbs(DefaultRootLayerSizeZ) });

				CopyFrom(from, Altaxo.Graph.Gdi.GraphCopyOptions.All);

				suppressToken.ResumeSilently();
			}
		}

		public void CopyFrom(GraphDocument3D from, Altaxo.Graph.Gdi.GraphCopyOptions options)
		{
			if (object.ReferenceEquals(this, from))
				return;

			using (var suspendToken = SuspendGetToken())
			{
				if (0 != (options & Altaxo.Graph.Gdi.GraphCopyOptions.CloneNotes))
				{
					ChildCopyToMember(ref _notes, from._notes);
				}

				if (0 != (options & Altaxo.Graph.Gdi.GraphCopyOptions.CloneProperties))
				{
					// Clone also the graph properties
					if (from._graphProperties != null && from._graphProperties.Count > 0)
					{
						PropertyBagNotNull.CopyFrom(from._graphProperties);
					}
					else
					{
						this._graphProperties = null;
					}
				}

				// the order is important here: clone the layers only before setting the printable graph bounds and other
				// properties, otherwise some errors will happen
				var newRootLayer = RootLayer;
				if (Altaxo.Graph.Gdi.GraphCopyOptions.CopyLayerAll == (options & Altaxo.Graph.Gdi.GraphCopyOptions.CopyLayerAll))
				{
					newRootLayer = (HostLayer3D)from._rootLayer.Clone();
				}
				else if (0 != (options & Altaxo.Graph.Gdi.GraphCopyOptions.CopyLayerAll))
				{
					// don't clone the layers, but copy the style of each each of the souce layers to the destination layers - this has to be done recursively
					newRootLayer.CopyFrom(from._rootLayer, options);
				}
				this.RootLayer = newRootLayer;

				suspendToken.Resume();
			}
		}

		public object Clone()
		{
			return new GraphDocument3D(this);
		}

		#endregion Construction / Copying

		#region Infrastructure

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _rootLayer)
				yield return new Main.DocumentNodeAndName(_rootLayer, () => _rootLayer = null, "RootLayer");

			if (null != _graphProperties)
				yield return new Main.DocumentNodeAndName(_graphProperties, () => _graphProperties = null, "GraphProperties");

			if (null != _notes)
				yield return new Main.DocumentNodeAndName(_notes, () => _notes = null, "Notes");
		}

		/// <summary>
		/// Get / sets the parent object of this table.
		/// </summary>
		public override Main.IDocumentNode ParentObject
		{
			get
			{
				return _parent;
			}
			set
			{
				if (object.ReferenceEquals(_parent, value))
					return;

				var oldParent = _parent;
				base.ParentObject = value;

				var parentAs = _parent as Main.IParentOfINameOwnerChildNodes;
				if (null != parentAs)
					parentAs.EhChild_ParentChanged(this, oldParent);
			}
		}

		public override string Name
		{
			get { return _name; }
			set
			{
				if (null == value)
					throw new ArgumentNullException("New name is null");
				if (_name == value)
					return; // nothing changed

				var canBeRenamed = true;
				var parentAs = _parent as Main.IParentOfINameOwnerChildNodes;
				if (null != parentAs)
				{
					canBeRenamed = parentAs.EhChild_CanBeRenamed(this, value);
				}

				if (canBeRenamed)
				{
					var oldName = _name;
					_name = value;

					if (null != parentAs)
						parentAs.EhChild_HasBeenRenamed(this, oldName);

					OnNameChanged(oldName);
				}
				else
				{
					throw new ApplicationException(string.Format("Renaming of graph {0} into {1} not possible, because name exists already", _name, value));
				}
			}
		}

		public SceneSettings Scene
		{
			get
			{
				return _sceneSettings;
			}
		}

		/// <summary>
		/// Fires both a Changed and a TunnelingEvent when the name has changed.
		/// The event arg of the Changed event is an instance of <see cref="T:Altaxo.Main.NamedObjectCollectionChangedEventArgs"/>.
		/// The event arg of the Tunneling event is an instance of <see cref="T:Altaxo.Main.DocumentPathChangedEventArgs"/>.
		/// </summary>
		/// <param name="oldName">The name of the table before it has changed the name.</param>
		protected virtual void OnNameChanged(string oldName)
		{
			EhSelfTunnelingEventHappened(Main.DocumentPathChangedEventArgs.Empty);
			EhSelfChanged(Main.NamedObjectCollectionChangedEventArgs.FromItemRenamed(this, oldName));
		}

		/// <summary>
		/// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
		/// to change a plot so that the plot items refer to another table.
		/// </summary>
		/// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
		public void VisitDocumentReferences(DocNodeProxyReporter Report)
		{
			_rootLayer.VisitDocumentReferences(Report);
		}

		#endregion Infrastructure

		#region Other graph properties

		/// <summary>
		/// The date/time of creation of this graph.
		/// </summary>
		public DateTime CreationTimeUtc
		{
			get
			{
				return _creationTime;
			}
		}

		/// <summary>
		/// The date/time when this graph was changed.
		/// </summary>
		public DateTime LastChangeTimeUtc
		{
			get
			{
				return _lastChangeTime;
			}
		}

		/// <summary>
		/// Notes concerning this graph.
		/// </summary>
		public Main.ITextBackedConsole Notes
		{
			get
			{
				return _notes;
			}
		}

		/// <summary>
		/// Gets an arbitrary object that was stored as graph property by <see cref="SetGraphProperty" />.
		/// </summary>
		/// <param name="key">Name of the property.</param>
		/// <returns>The object, or null if no object under the provided name was stored here.</returns>
		public object GetGraphProperty(string key)
		{
			object result = null;
			if (_graphProperties != null)
				_graphProperties.TryGetValue(key, out result);
			return result;
		}

		public T GetPropertyValue<T>(Altaxo.Main.Properties.PropertyKey<T> key, Func<T> resultCreationIfNotFound)
		{
			return PropertyExtensions.GetPropertyValue(this, key, resultCreationIfNotFound);
		}

		/// <summary>
		/// The table properties, key is a string, val is a object you want to store here.
		/// </summary>
		/// <remarks>The properties are saved on disc (with exception of those who's name starts with "tmp/".
		/// If the property you want to store is only temporary, the property name should therefore
		/// start with "tmp/".</remarks>
		public void SetGraphProperty(string key, object val)
		{
			PropertyBagNotNull.SetValue(key, val);
		}

		/// <summary>
		/// Gets/sets the size of this graph in points (1/72 inch). The value returned is exactly the size of the root layer.
		/// </summary>
		/// <value>
		/// The size of the graph in points (1/72 inch).
		/// </value>
		public VectorD3D Size
		{
			get
			{
				var r = GetBounds();

				return r.Size;
			}
			set
			{
				_rootLayer.Size = value;
			}
		}

		/// <summary>
		/// The collection of layers of the graph.
		/// </summary>
		public HostLayer3D RootLayer
		{
			get { return _rootLayer; }
			private set
			{
				_rootLayer = value;
				_rootLayer.ParentObject = this;
			}
		}

		/// <summary>
		/// Gets the bounds of the root layer.
		/// </summary>
		/// <returns></returns>
		private RectangleD3D GetBounds()
		{
			var s = _rootLayer.Size;
			var p1 = _rootLayer.TransformCoordinatesFromHereToParent(new PointD3D(0, 0, 0));
			var p2 = _rootLayer.TransformCoordinatesFromHereToParent(new PointD3D(s.X, 0, 0));
			var p3 = _rootLayer.TransformCoordinatesFromHereToParent(new PointD3D(0, s.Y, 0));
			var p4 = _rootLayer.TransformCoordinatesFromHereToParent(new PointD3D(s.X, s.Y, 0));
			var p5 = _rootLayer.TransformCoordinatesFromHereToParent(new PointD3D(0, 0, s.Z));
			var p6 = _rootLayer.TransformCoordinatesFromHereToParent(new PointD3D(s.X, 0, s.Z));
			var p7 = _rootLayer.TransformCoordinatesFromHereToParent(new PointD3D(0, s.Y, s.Z));
			var p8 = _rootLayer.TransformCoordinatesFromHereToParent(new PointD3D(s.X, s.Y, s.Z));

			var r = new RectangleD3D(p1, VectorD3D.Empty);
			r.ExpandToInclude(p2);
			r.ExpandToInclude(p3);
			r.ExpandToInclude(p4);
			r.ExpandToInclude(p5);
			r.ExpandToInclude(p6);
			r.ExpandToInclude(p7);
			r.ExpandToInclude(p8);
			return r;
		}

		#endregion Other graph properties

		#region Event handler

		private void EhNotesChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			EhChildChanged(sender, e);
		}

		protected override bool HandleHighPriorityChildChangeCases(object sender, ref EventArgs e)
		{
			if (null != _paintThread && object.ReferenceEquals(_paintThread, System.Threading.Thread.CurrentThread))
			{
				if (_isFixupInternalDataStructuresActive)
				{
					_hasFixupChangedAnything = true;
					return false; // no further handling neccessary
				}
				else
				{
					var stb = new System.Text.StringBuilder();
					var st = new System.Diagnostics.StackTrace(true);

					var len = Math.Min(30, st.FrameCount);
					for (int i = 1; i < len; ++i)
					{
						var frame = st.GetFrame(i);
						var method = frame.GetMethod();

						if (i > 2) stb.Append("\r\n\tin ");

						stb.Append(method.DeclaringType.FullName);
						stb.Append("|");
						stb.Append(method.Name);
						stb.Append("(L");
						stb.Append(frame.GetFileLineNumber());
						stb.Append(")");
					}

					Current.Console.WriteLine("Graph has changed during painting. Stacktrace:");
					Current.Console.WriteLine(stb.ToString());
				}
			}

			return base.HandleHighPriorityChildChangeCases(sender, ref e);
		}

		/// <summary>
		/// Fires the <see cref="SizeChanged" /> event.
		/// </summary>
		protected void OnSizeChanged()
		{
			if (SizeChanged != null)
				SizeChanged(this, EventArgs.Empty);
		}

		/// <summary>
		/// Fires the Invalidate event.
		/// </summary>
		/// <param name="sender">The layer which needs to be repainted.</param>
		protected internal virtual void OnInvalidate(XYPlotLayer3D sender)
		{
			EhSelfChanged(EventArgs.Empty);
		}

		protected override void AccumulateChangeData(object sender, EventArgs e)
		{
			if (sender != null && _accumulatedEventData == null)
				this._accumulatedEventData = EventArgs.Empty;
		}

		protected override void OnChanged(EventArgs e)
		{
			if (_cachedRootLayerSize != _rootLayer.Size)
			{
				_cachedRootLayerSize = _rootLayer.Size;
				OnSizeChanged();
			}

			if (!_isFixupInternalDataStructuresActive)
				base.OnChanged(e);
		}

		#endregion Event handler

		#region IPropertyBagOwner

		public Main.Properties.PropertyBag PropertyBag
		{
			get { return _graphProperties; }
			protected set
			{
				_graphProperties = value;
				if (null != _graphProperties)
					_graphProperties.ParentObject = this;
			}
		}

		public Main.Properties.PropertyBag PropertyBagNotNull
		{
			get
			{
				if (null == _graphProperties)
					PropertyBag = new Main.Properties.PropertyBag();
				return _graphProperties;
			}
		}

		#endregion IPropertyBagOwner

		#region Convenience functions

		/// <summary>
		/// Gets the first layer in the graph that has the provided type. If such a layer is not found, an exception is thrown.
		/// </summary>
		/// <typeparam name="TLayer">The type of the layer.</typeparam>
		/// <returns>The first layer in the graph with the provided type.</returns>
		public TLayer GetFirstLayerOfType<TLayer>() where TLayer : HostLayer3D
		{
			return RootLayer.Layers.OfType<TLayer>().First();
		}

		/// <summary>
		/// Gets the first xy plot layer of the graph. If such a layer is not found, an exception is thrown.
		/// </summary>
		/// <returns>The first xy plot layer.</returns>
		public XYPlotLayer3D GetFirstXYPlotLayer()
		{
			return GetFirstLayerOfType<XYPlotLayer3D>();
		}

		#endregion Convenience functions

		private void AdjustRootLayerPositionToFitIntoZeroOffsetRectangle()
		{
			var r = GetBounds();

			_rootLayer.Position = _rootLayer.Position - (VectorD3D)r.Location;
		}

		public void Paint(IGraphicContext3D g)
		{
			var paintContext = new Altaxo.Graph.Gdi.GdiPaintContext();

			// DrawHostLayerSizedCube(g);

			RootLayer.PaintPreprocessing(paintContext);

			RootLayer.Paint(g, paintContext);

			RootLayer.PaintPostprocessing();

			//DrawSomething2(g);
			DrawSomeStuff(g);
		}

		public void DrawSomeStuff(IGraphicContext3D gc)
		{
			var penRed = new PenX3D(NamedColors.Red, 4);
			gc.DrawLine(penRed, new PointD3D(0, 0, 0), new PointD3D(30, 0, 0));
			var penGreen = new PenX3D(NamedColors.Green, 4);
			gc.DrawLine(penGreen, new PointD3D(0, 0, 0), new PointD3D(0, 30, 0));
			var penBlue = new PenX3D(NamedColors.Blue, 4);
			gc.DrawLine(penBlue, new PointD3D(0, 0, 0), new PointD3D(0, 0, 30));
		}

		public void DrawSomething2(IGraphicContext3D gc)
		{
			var gcc = (GraphicContext3DBase)gc;

			var penRed = new PenX3D(NamedColors.Red, 4);
			gcc.DrawTriangle(Materials.GetSolidMaterial(NamedColors.Red), new PointD3D(0, 0, 0), new PointD3D(50, 50, 0), new PointD3D(0, 50, 0));
		}
	}
}