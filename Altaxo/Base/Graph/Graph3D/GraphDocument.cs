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

using Altaxo.Drawing;
using Altaxo.Geometry;

//using Altaxo.Graph;
using Altaxo.Main;
using Altaxo.Main.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Graph.Graph3D
{
	using Drawing.D3D;
	using GraphicsContext;

	public class GraphDocument : GraphDocumentBase
	{
		protected const double DefaultRootLayerSizeX = 697.68054;
		protected const double DefaultRootLayerSizeY = 451.44;
		protected const double DefaultRootLayerSizeZ = 451.44;

		#region Member variables

		private HostLayer _rootLayer;

		private SceneSettings _sceneSettings;

		/// <summary>The root layer size, cached here only for deciding whether to raise the <see cref="GraphDocumentBase.SizeChanged"/> event. Do not use it otherwise.</summary>
		[NonSerialized]
		private VectorD3D _cachedRootLayerSize;

		/// <summary>
		/// Occurs when the geometry has changed. This includes events that are able to change the geometry implicitly, for instance changing the properties of the graph.
		/// </summary>
		public event EventHandler GeometryChanged;

		/// <summary>
		/// Occurs when only the camera has changed. This does not require a new buildup of the geometry.
		/// </summary>
		public event EventHandler CameraChanged;

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

		public static NamedColor GetDefaultForeColor(IReadOnlyPropertyBag context)
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

		#region "Serialization"

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GraphDocument), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (GraphDocument)obj;

				info.AddValue("Name", s._name);
				info.AddValue("GraphIdentifier", s._graphIdentifier);
				info.AddValue("CreationTime", s._creationTime.ToLocalTime());
				info.AddValue("LastChangeTime", s._lastChangeTime.ToLocalTime());
				info.AddValue("Notes", s._notes.Text);
				info.AddValue("RootLayer", s._rootLayer);
				info.AddValue("Properties", s._graphProperties);
				info.AddValue("SceneSettings", s._sceneSettings);
			}

			public void Deserialize(GraphDocument s, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				s._name = info.GetString("Name");
				s._graphIdentifier = info.GetString("GraphIdentifier");
				s._creationTime = info.GetDateTime("CreationTime").ToUniversalTime();
				s._lastChangeTime = info.GetDateTime("LastChangeTime").ToUniversalTime();
				s._notes.Text = info.GetString("Notes");
				s.RootLayer = (HostLayer)info.GetValue("RootLayer", s);
				s.PropertyBag = (Main.Properties.PropertyBag)info.GetValue("Properties", s);
				s.ChildSetMember(ref s._sceneSettings, (SceneSettings)info.GetValue("SceneSetting", s));
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (GraphDocument)o ?? new GraphDocument();
				Deserialize(s, info, parent);
				return s;
			}
		}

		#endregion "Serialization"

		#region Construction / Copying

		/// <summary>
		/// Creates a empty GraphDocument with no layers and a standard size of A4 landscape.
		/// </summary>
		public GraphDocument()
		{
			_sceneSettings = new SceneSettings();
			this.RootLayer = new HostLayer() { ParentObject = this };
			this.RootLayer.Location = new ItemLocationDirect
			{
				SizeX = RADouble.NewAbs(DefaultRootLayerSizeX),
				SizeY = RADouble.NewAbs(DefaultRootLayerSizeY),
				SizeZ = RADouble.NewAbs(DefaultRootLayerSizeZ)
			};
			_sceneSettings = new SceneSettings() { ParentObject = this };
		}

		public GraphDocument(GraphDocument from)
		{
			using (var suppressToken = SuspendGetToken())
			{
				_creationTime = _lastChangeTime = DateTime.UtcNow;
				this.RootLayer = new HostLayer(null, new ItemLocationDirect { SizeX = RADouble.NewAbs(DefaultRootLayerSizeX), SizeY = RADouble.NewAbs(DefaultRootLayerSizeY), SizeZ = RADouble.NewAbs(DefaultRootLayerSizeZ) });

				CopyFrom(from, Altaxo.Graph.Gdi.GraphCopyOptions.All);

				suppressToken.ResumeSilently();
			}
		}

		public void CopyFrom(GraphDocument from, Altaxo.Graph.Gdi.GraphCopyOptions options)
		{
			if (object.ReferenceEquals(this, from))
				return;

			using (var suspendToken = SuspendGetToken())
			{
				ChildCopyToMember(ref _sceneSettings, from._sceneSettings);

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
					newRootLayer = (HostLayer)from._rootLayer.Clone();
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

		public override object Clone()
		{
			return new GraphDocument(this);
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

			if (null != _sceneSettings)
				yield return new Main.DocumentNodeAndName(_sceneSettings, () => _sceneSettings = null, "Scene");
		}

		public SceneSettings Scene
		{
			get
			{
				return _sceneSettings;
			}
		}

		/// <summary>
		/// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
		/// to change a plot so that the plot items refer to another table.
		/// </summary>
		/// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
		public override void VisitDocumentReferences(DocNodeProxyReporter Report)
		{
			_rootLayer.VisitDocumentReferences(Report);
		}

		#endregion Infrastructure

		#region Other graph properties

		/// <summary>
		/// Gets/sets the size of this graph in points (1/72 inch). The value returned is exactly the size of the root layer.
		/// </summary>
		/// <value>
		/// The size of the graph in points (1/72 inch).
		/// </value>
		public override PointD2D Size
		{
			get
			{
				var r = GetViewBounds();

				return new PointD2D(r.Size.X, r.Size.Y);
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// The collection of layers of the graph.
		/// </summary>
		public HostLayer RootLayer
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

		/// <summary>
		/// Gets the bounds of the root layer.
		/// </summary>
		/// <returns></returns>
		private RectangleD3D GetViewBounds()
		{
			var matrix = Scene.Camera.LookAtRHMatrix;
			var rect = new RectangleD3D(PointD3D.Empty, RootLayer.Size);
			var bounds = RectangleD3D.NewRectangleIncludingAllPoints(rect.Vertices.Select(x => matrix.Transform(x)));
			return bounds;
		}

		#endregion Other graph properties

		#region Event handler

		/// <summary>
		/// Fires the Invalidate event.
		/// </summary>
		/// <param name="sender">The layer which needs to be repainted.</param>
		protected internal virtual void OnInvalidate(XYZPlotLayer sender)
		{
			EhSelfChanged(EventArgs.Empty);
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

		#region Convenience functions

		/// <summary>
		/// Gets the first layer in the graph that has the provided type. If such a layer is not found, an exception is thrown.
		/// </summary>
		/// <typeparam name="TLayer">The type of the layer.</typeparam>
		/// <returns>The first layer in the graph with the provided type.</returns>
		public TLayer GetFirstLayerOfType<TLayer>() where TLayer : HostLayer
		{
			return RootLayer.Layers.OfType<TLayer>().First();
		}

		/// <summary>
		/// Gets the first xy plot layer of the graph. If such a layer is not found, an exception is thrown.
		/// </summary>
		/// <returns>The first xy plot layer.</returns>
		public XYZPlotLayer GetFirstXYPlotLayer()
		{
			return GetFirstLayerOfType<XYZPlotLayer>();
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
		}

		/// <summary>
		/// Make the views to look at the root layer center. The scale is choosen so that the size of the plot will be maximal.
		/// </summary>
		/// <param name="toEyeVector">The To-Eye vector (vector from the target to the camera position).</param>
		/// <param name="cameraUpVector">The camera up vector.</param>
		/// <param name="aspectRatio">The aspect ratio of the view port. If in doubt, use 1.</param>
		/// <exception cref="System.NotImplementedException"></exception>
		public void ViewToRootLayerCenter(VectorD3D toEyeVector, VectorD3D cameraUpVector, double aspectRatio)
		{
			var upVector = cameraUpVector.Normalized;
			var targetPosition = (PointD3D)(0.5 * RootLayer.Size);
			var cameraDistance = 10 * RootLayer.Size.Length;
			var eyePosition = cameraDistance * toEyeVector.Normalized + targetPosition;

			var newCamera = Scene.Camera.WithUpEyeTargetZNearZFar(upVector, eyePosition, targetPosition, cameraDistance / 8, cameraDistance * 2);

			var orthoCamera = newCamera as Camera.OrthographicCamera;

			if (null != orthoCamera)
			{
				orthoCamera = orthoCamera.WithScale(1);

				var mx = orthoCamera.GetLookAtRHTimesOrthoRHMatrix(aspectRatio);
				// to get the resulting scale, we transform all vertices of the root layer (the destination range would be -1..1, but now is not in range -1..1)
				// then we search for the maximum of the absulute value of x and y. This is our scale.
				double absmax = 0;
				foreach (var p in new RectangleD3D(RootLayer.Position, RootLayer.Size).Vertices)
				{
					var ps = mx.TransformPoint(p);
					absmax = Math.Max(absmax, Math.Abs(ps.X));
					absmax = Math.Max(absmax, Math.Abs(ps.Y));
				}
				newCamera = orthoCamera.WithScale(absmax);
			}
			else
			{
				throw new NotImplementedException();
			}

			Scene.Camera = newCamera;
		}
	}
}