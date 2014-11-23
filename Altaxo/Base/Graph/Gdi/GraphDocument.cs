#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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

using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Ticks;
using Altaxo.Main;
using Altaxo.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Graph.Gdi
{
	/// <summary>
	/// This is the class that holds all elements of a graph, especially one ore more layers.
	/// </summary>
	/// <remarks>The coordinate system of the graph is in units of points (1/72 inch). The origin (0,0) of the graph
	/// is the top left corner of the printable area (and therefore _not_ the page bounds). The value of the page
	/// bounds is stored inside the class only to know what the original page size of the document was.</remarks>
	public class GraphDocument
		:
		IProjectItem,
		System.ICloneable,
		IChangedEventSource,
		Main.IChildChangedEventSink,
		Main.IDocumentNode,
		Main.INameOwner,
		Main.Properties.IPropertyBagOwner
	{
		// following default unit is point (1/72 inch)
		/// <summary>For the graph elements all the units are in points. One point is 1/72 inch.</summary>
		protected const float UnitPerInch = 72;

		protected const double DefaultRootLayerSizeX = 697.68054;
		protected const double DefaultRootLayerSizeY = 451.44;

		public static readonly Main.Properties.PropertyKey<ItemLocationDirect> PropertyKeyDefaultRootLayerSize =
			new Main.Properties.PropertyKey<ItemLocationDirect>(
				"22F853C9-A011-46FA-8021-8668AB4EE1C6",
				"Graph\\DefaultRootLayerSize",
				Main.Properties.PropertyLevel.All,
				typeof(GraphDocument),
				() => new ItemLocationDirect() { SizeX = RADouble.NewAbs(DefaultRootLayerSizeX), SizeY = RADouble.NewAbs(DefaultRootLayerSizeY) })
				{
					EditingControllerCreation = (doc) =>
					{
						var ctrl = new Gui.Graph.ItemLocationDirectController() { UseDocumentCopy = Gui.UseDocument.Copy };
						ctrl.ShowPositionElements(false, false);
						ctrl.ShowAnchorElements(false, false);
						ctrl.InitializeDocument(doc);
						return ctrl;
					}
				};

		public static readonly Main.Properties.PropertyKey<FontX> PropertyKeyDefaultFont =
				new Main.Properties.PropertyKey<FontX>(
				"2CFD57CF-25D5-456E-9E45-D7D8823F4A54",
				"Graph\\DefaultFont",
				Main.Properties.PropertyLevel.All,
				typeof(GraphDocument),
				() => GdiFontManager.GetFont(FontFamily.GenericSansSerif, 18, FontStyle.Regular))
				{
					EditingControllerCreation = (doc) =>
					{
						var ctrl = new Gui.Common.Drawing.FontXController { UseDocumentCopy = Gui.UseDocument.Copy };
						ctrl.InitializeDocument(doc);
						return ctrl;
					}
				};

		public static readonly Main.Properties.PropertyKey<Altaxo.Graph.NamedColor> PropertyKeyDefaultForeColor =
			new Main.Properties.PropertyKey<Altaxo.Graph.NamedColor>(
"2F138FDD-B96A-4C03-9BEF-83FC412E50B2",
"Graph\\Colors\\Default fore color",
Main.Properties.PropertyLevel.Document,
typeof(GraphDocument),
() => Altaxo.Graph.NamedColors.Black
)
{
	EditingControllerCreation = (doc) =>
	{
		var ctrl = new Gui.Graph.ColorManagement.NamedColorChoiceController { UseDocumentCopy = Gui.UseDocument.Copy };
		ctrl.InitializeDocument(doc);
		return ctrl;
	}
};

		public static readonly Main.Properties.PropertyKey<Altaxo.Graph.NamedColor> PropertyKeyDefaultBackColor =
	new Main.Properties.PropertyKey<Altaxo.Graph.NamedColor>(
"90BB0E83-D1A4-40B7-9607-55D4B9C272C3",
"Graph\\Colors\\Default back color",
Main.Properties.PropertyLevel.Document,
typeof(GraphDocument),
() => Altaxo.Graph.NamedColors.AliceBlue
)
	{
		EditingControllerCreation = (doc) =>
		{
			var ctrl = new Gui.Graph.ColorManagement.NamedColorChoiceController { UseDocumentCopy = Gui.UseDocument.Copy };
			ctrl.InitializeDocument(doc);
			return ctrl;
		}
	};

		public static readonly Main.Properties.PropertyKey<Altaxo.Graph.NamedColor> PropertyKeyDefaultPlotColor =
new Main.Properties.PropertyKey<Altaxo.Graph.NamedColor>(
"D5DB4695-2630-4B7D-83E3-71CA3873B362",
"Graph\\Colors\\Default plot color",
Main.Properties.PropertyLevel.Document,
typeof(GraphDocument),
() => Altaxo.Graph.ColorManagement.ColorSetManager.Instance.BuiltinDarkPlotColors[0]
)
{
	EditingControllerCreation = (doc) =>
	{
		var ctrl = new Gui.Graph.ColorManagement.NamedColorChoiceController { UseDocumentCopy = Gui.UseDocument.Copy, ShowPlotColorsOnly = true };
		ctrl.InitializeDocument(doc);
		return ctrl;
	}
};

		private SingleGraphPrintOptions _printOptions;

		public SingleGraphPrintOptions PrintOptions
		{
			get { return _printOptions; }
			set { _printOptions = value; }
		}

		private HostLayer _rootLayer;

		private string _name;

		[NonSerialized]
		private object _parent;

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

		/// <summary>Event fired when anything here changed.</summary>
		[field: NonSerialized]
		public event System.EventHandler Changed;

		[NonSerialized]
		private Main.EventSuppressor _changedEventSuppressor;

		/// <summary>Events which are fired from this thread are not distributed.</summary>
		[NonSerialized]
		private volatile System.Threading.Thread _paintThread;

		/// <summary>
		/// Event to signal that the name of this object has changed.
		/// </summary>
		[field: NonSerialized]
		public event Action<Main.INameOwner, string> NameChanged;

		/// <summary>
		/// Fired for instance if this instance is about to be disposed and is disposed.
		/// </summary>
		[field: NonSerialized]
		public event Action<object, object, Main.TunnelingEventArgs> TunneledEvent;

		/// <summary>
		/// Fired before the name of this object is changed.
		/// </summary>
		[field: NonSerialized]
		public event Action<Main.INameOwner, string, System.ComponentModel.CancelEventArgs> PreviewNameChange;

		/// <summary>Event fired if the size of this document (i.e. the size of the root layer) changed.</summary>
		[field: NonSerialized]
		public event EventHandler SizeChanged;

		/// <summary>The root layer size, cached here only for deciding whether to raise the <see cref="SizeChanged"/> event. Do not use it otherwise.</summary>
		[NonSerialized]
		private PointD2D _cachedRootLayerSize;

		protected System.EventArgs _changeEventData = null;

		protected bool _isResumeInProgress = false;

		protected System.Collections.ArrayList _suspendedChildCollection = new System.Collections.ArrayList();

		#region "Serialization"

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.GraphDocument", 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new InvalidOperationException("Serialization of old version");
				/*
				GraphDocument s = (GraphDocument)obj;

				// info.AddBaseValueEmbedded(s,typeof(GraphDocument).BaseType);
				// now the data of our class
				info.AddValue("Name", s._name);
				info.AddValue("PageBounds", s._pageBounds);
				info.AddValue("PrintableBounds", s._printableBounds);
				info.AddValue("Layers", s._rootLayer);
				*/
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				GraphDocument s = null != o ? (GraphDocument)o : new GraphDocument();

				//  info.GetBaseValueEmbedded(s,typeof(GraphDocument).BaseType,parent);
				s._name = info.GetString("Name");
				var pageBounds = (RectangleF)info.GetValue("PageBounds", s);
				var printableBounds = (RectangleF)info.GetValue("PrintableBounds", s);
				var layers = (XYPlotLayer.XYPlotLayerCollection)info.GetValue("LayerList", s);
				s._rootLayer.Location = new ItemLocationDirect { SizeX = RADouble.NewAbs(printableBounds.Size.Width), SizeY = RADouble.NewAbs(printableBounds.Size.Height) };
				foreach (var l in layers)
					s._rootLayer.Layers.Add(l);
				return s;
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.GraphDocument", 1)]
		private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new InvalidOperationException("Serialization of old version");
				/*
				GraphDocument s = (GraphDocument)obj;

				// info.AddBaseValueEmbedded(s,typeof(GraphDocument).BaseType);
				// now the data of our class
				info.AddValue("Name", s._name);
				info.AddValue("PageBounds", s._pageBounds);
				info.AddValue("PrintableBounds", s._printableBounds);
				info.AddValue("Layers", s._rootLayer);

				// new in version 1 - Add graph properties
				int numberproperties = s._graphProperties == null ? 0 : s._graphProperties.Keys.Count;
				info.CreateArray("TableProperties", numberproperties);
				if (s._graphProperties != null)
				{
					foreach (string propkey in s._graphProperties.Keys)
					{
						if (propkey.StartsWith("tmp/"))
							continue;
						info.CreateElement("e");
						info.AddValue("Key", propkey);
						object val = s._graphProperties[propkey];
						info.AddValue("Value", info.IsSerializable(val) ? val : null);
						info.CommitElement();
					}
				}
				info.CommitArray();
				*/
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				GraphDocument s = null != o ? (GraphDocument)o : new GraphDocument();
				Deserialize(s, info, parent);
				return s;
			}

			public virtual void Deserialize(GraphDocument s, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				//  info.GetBaseValueEmbedded(s,typeof(GraphDocument).BaseType,parent);
				s._name = info.GetString("Name");
				var pageBounds = (RectangleF)info.GetValue("PageBounds", s);
				var printableBounds = (RectangleF)info.GetValue("PrintableBounds", s);
				var layers = (XYPlotLayer.XYPlotLayerCollection)info.GetValue("LayerList", s);
				if (layers.GraphSize.IsEmpty)
					s._rootLayer.Location = new ItemLocationDirect { SizeX = RADouble.NewAbs(printableBounds.Size.Width), SizeY = RADouble.NewAbs(printableBounds.Size.Height) };
				else
					s._rootLayer.Location = new ItemLocationDirect { SizeX = RADouble.NewAbs(layers.GraphSize.Width), SizeY = RADouble.NewAbs(layers.GraphSize.Height) };

				foreach (var l in layers)
					s._rootLayer.Layers.Add(l);

				// new in version 1 - Add graph properties
				int numberproperties = info.OpenArray(); // "GraphProperties"
				var pb = numberproperties == 0 ? null : s.PropertyBagNotNull;
				for (int i = 0; i < numberproperties; i++)
				{
					info.OpenElement(); // "e"
					string propkey = info.GetString("Key");
					object propval = info.GetValue("Value", parent);
					info.CloseElement(); // "e"
					pb.SetValue(propkey, propval);
				}
				info.CloseArray(numberproperties);
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.GraphDocument", 2)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Graph.Gdi.GraphDocument", 3)]
		private class XmlSerializationSurrogate2 : XmlSerializationSurrogate1
		{
			public override void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				throw new InvalidOperationException("Serialization of old version");
				/*
				base.Serialize(obj, info);
				GraphDocument s = (GraphDocument)obj;
				info.AddValue("GraphIdentifier", s._graphIdentifier);
				info.AddValue("Notes", s._notes.Text);
				info.AddValue("CreationTime", s._creationTime.ToLocalTime());
				info.AddValue("LastChangeTime", s._lastChangeTime.ToLocalTime());
				*/
			}

			public override void Deserialize(GraphDocument s, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				base.Deserialize(s, info, parent);
				s._graphIdentifier = info.GetString("GraphIdentifier");
				s._notes.Text = info.GetString("Notes");
				s._creationTime = info.GetDateTime("CreationTime").ToUniversalTime();
				s._lastChangeTime = info.GetDateTime("LastChangeTime").ToUniversalTime();
			}
		}

		/// <summary>
		/// 2013-11-27 Reflect the new situation that we have now only a host layer, but not printable bounds and so on.
		/// 2014-01-26 replace the _graphProperties dictionary by a PropertyBag.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GraphDocument), 4)]
		private class XmlSerializationSurrogate4 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				GraphDocument s = (GraphDocument)obj;

				info.AddValue("Name", s._name);
				info.AddValue("GraphIdentifier", s._graphIdentifier);
				info.AddValue("CreationTime", s._creationTime.ToLocalTime());
				info.AddValue("LastChangeTime", s._lastChangeTime.ToLocalTime());
				info.AddValue("Notes", s._notes.Text);
				info.AddValue("RootLayer", s._rootLayer);

				info.AddValue("Properties", s._graphProperties);
			}

			public void Deserialize(GraphDocument s, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				s._name = info.GetString("Name");
				s._graphIdentifier = info.GetString("GraphIdentifier");
				s._creationTime = info.GetDateTime("CreationTime").ToUniversalTime();
				s._lastChangeTime = info.GetDateTime("LastChangeTime").ToUniversalTime();
				s._notes.Text = info.GetString("Notes");
				s.RootLayer = (HostLayer)info.GetValue("RootLayer", s);
				s.PropertyBag = (Main.Properties.PropertyBag)info.GetValue("Properties");
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				GraphDocument s = null != o ? (GraphDocument)o : new GraphDocument();
				Deserialize(s, info, parent);
				return s;
			}
		}

		#endregion "Serialization"

		/// <summary>
		/// Creates a empty GraphDocument with no layers and a standard size of A4 landscape.
		/// </summary>
		public GraphDocument()
		{
			this._changedEventSuppressor = new EventSuppressor(this.EhChangedEventResumes);
			_creationTime = _lastChangeTime = DateTime.UtcNow;
			_notes = new TextBackedConsole();
			_notes.PropertyChanged += EhNotesChanged;
			this.RootLayer = new HostLayer();
			this.RootLayer.Location = new ItemLocationDirect { SizeX = RADouble.NewAbs(DefaultRootLayerSizeX), SizeY = RADouble.NewAbs(DefaultRootLayerSizeY) };
		}

		private void EhNotesChanged(object sender, PropertyChangedEventArgs e)
		{
			OnChanged();
		}

		public GraphDocument(GraphDocument from)
		{
			this._changedEventSuppressor = new EventSuppressor(this.EhChangedEventResumes);
			var suppressToken = _changedEventSuppressor.Suspend();
			try
			{
				_creationTime = _lastChangeTime = DateTime.UtcNow;
				this.RootLayer = new HostLayer(null, new ItemLocationDirect { SizeX = RADouble.NewAbs(814), SizeY = RADouble.NewAbs(567) });

				CopyFrom(from, GraphCopyOptions.All);
			}
			finally
			{
				_changedEventSuppressor.Resume(ref suppressToken, Main.EventFiring.Suppressed);
			}
		}

		public void CopyFrom(GraphDocument from, GraphCopyOptions options)
		{
			if (object.ReferenceEquals(this, from))
				return;

			if (0 != (options & GraphCopyOptions.CloneNotes))
			{
				if (null != _notes) _notes.PropertyChanged -= EhNotesChanged;
				this._notes = from._notes.Clone();
				this._notes.PropertyChanged += EhNotesChanged;
			}

			if (0 != (options & GraphCopyOptions.CloneProperties))
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
			if (GraphCopyOptions.CopyLayerAll == (options & GraphCopyOptions.CopyLayerAll))
			{
				newRootLayer = (HostLayer)from._rootLayer.Clone();
			}
			else if (0 != (options & GraphCopyOptions.CopyLayerAll))
			{
				// don't clone the layers, but copy the style of each each of the souce layers to the destination layers - this has to be done recursively
				newRootLayer.CopyFrom(from._rootLayer, options);
			}
			this.RootLayer = newRootLayer;
		}

		public object Clone()
		{
			return new GraphDocument(this);
		}

		public string Name
		{
			get { return _name; }
			set
			{
				if (null == value)
					throw new ArgumentNullException("New name is null");

				if (_name != value)
				{
					// test if an object with this name is already in the parent
					Altaxo.Main.INamedObjectCollection parentColl = ParentObject as Altaxo.Main.INamedObjectCollection;
					if (null != parentColl && null != parentColl.GetChildObjectNamed(value))
						throw new ApplicationException(string.Format("The graph {0} can not be renamed to {1}, since another graph with the same name already exists", this.Name, value));

					string oldValue = _name;
					_name = value;
					OnNameChanged(oldValue);
				}
			}
		}

		public virtual void OnNameChanged(string oldValue)
		{
			if (NameChanged != null)
				NameChanged(this, oldValue);
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

		public object ParentObject
		{
			get { return _parent; }
			set
			{
				_parent = value;
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
		public PointD2D Size
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
		/// Gets the bounds of the root layer.
		/// </summary>
		/// <returns></returns>
		private RectangleD GetBounds()
		{
			var s = _rootLayer.Size;
			var p1 = _rootLayer.TransformCoordinatesFromHereToParent(new PointD2D(0, 0));
			var p2 = _rootLayer.TransformCoordinatesFromHereToParent(new PointD2D(s.X, 0));
			var p3 = _rootLayer.TransformCoordinatesFromHereToParent(new PointD2D(0, s.Y));
			var p4 = _rootLayer.TransformCoordinatesFromHereToParent(new PointD2D(s.X, s.Y));

			var r = new RectangleD(p1, PointD2D.Empty);
			r.ExpandToInclude(p2);
			r.ExpandToInclude(p3);
			r.ExpandToInclude(p4);
			return r;
		}

		private void AdjustRootLayerPositionToFitIntoZeroOffsetRectangle()
		{
			var r = GetBounds();

			_rootLayer.Position = _rootLayer.Position - r.LeftTop;
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
		/// Paints the graph.
		/// </summary>
		/// <param name="g">The graphic contents to paint to.</param>
		/// <param name="bForPrinting">Indicates if the painting is for printing purposes. Not used for the moment.</param>
		/// <remarks>The reference point (0,0) of the GraphDocument
		/// is the top left corner of the printable area (and not of the page area!). The hosting class has to translate the graphics origin
		/// to the top left corner of the printable area before calling this routine.</remarks>
		public void DoPaint(Graphics g, bool bForPrinting)
		{
			if (System.Threading.Thread.CurrentThread == _paintThread)
				throw new InvalidOperationException("DoPaint is called reentrant (i.e. from the same thread that is already executing DoPaint");

			while (null != _paintThread) // seems that another thread also wants to paint, this can happen when a Com Container want to have a image at the same time as the user interface
				System.Threading.Thread.Sleep(1); // then the other thread must wait, until the first paint operation is finished

			System.Diagnostics.Debug.Assert(null == _paintThread, "We waited, thus _paintThread should be null");

			_paintThread = System.Threading.Thread.CurrentThread; // Suppress events that are fired during paint

			try
			{
				// First set the current thread's document culture
				_paintThread.CurrentCulture = this.GetPropertyValue(Altaxo.Settings.CultureSettings.PropertyKeyDocumentCulture, null).Culture;

				AdjustRootLayerPositionToFitIntoZeroOffsetRectangle();

				RootLayer.PaintPreprocessing(this);

				RootLayer.Paint(g, bForPrinting);

				RootLayer.PaintPostprocessing();
			}
			finally
			{
				_paintThread = null;
			}
		} // end of function DoPaint

		public static FontX GetDefaultFont(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
		{
			if (null != context)
				return context.GetValue(PropertyKeyDefaultFont);
			else
				return GdiFontManager.GetFont(System.Drawing.FontFamily.GenericSansSerif, 18, FontStyle.Regular);
		}

		/// <summary>
		/// Gets the default width of the pen for all graphics in this graph, using the provided property context.
		/// </summary>
		/// <param name="context">The property context.</param>
		/// <returns>Default pen with in points (1/72 inch).</returns>
		public static double GetDefaultPenWidth(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
		{
			double result = 1;

			if (null != context)
			{
				var font = context.GetValue(PropertyKeyDefaultFont);
				using (var path = new GraphicsPath())
				{
					path.AddString("-", font.GdiFontFamily(), (int)font.Style, (float)font.Size, new PointF(0, 0), StringFormat.GenericTypographic);
					var bounds = path.GetBounds();

					if (bounds.Height > 0)
					{
						result = Calc.Rounding.RoundToNumberOfSignificantDigits(bounds.Height, 2, MidpointRounding.ToEven);
					}
				}
			}
			return result;
		}

		public static NamedColor GetDefaultForeColor(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
		{
			if (null == context)
				context = PropertyExtensions.GetPropertyContextOfProject();

			return context.GetValue<NamedColor>(PropertyKeyDefaultForeColor);
		}

		public static NamedColor GetDefaultBackColor(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
		{
			if (null == context)
				context = PropertyExtensions.GetPropertyContextOfProject();

			return context.GetValue<NamedColor>(PropertyKeyDefaultBackColor);
		}

		public static NamedColor GetDefaultPlotColor(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
		{
			if (null == context)
				context = PropertyExtensions.GetPropertyContextOfProject();

			return context.GetValue<NamedColor>(PropertyKeyDefaultPlotColor);
		}

		/// <summary>
		/// Gets the default plot symbol size for all graphics in this graph, using the provided property context.
		/// </summary>
		/// <param name="context">The property context.</param>
		/// <returns>Default plot symbol size in points (1/72 inch).</returns>
		public static double GetDefaultSymbolSize(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
		{
			double result = 1;

			if (null != context)
			{
				var font = context.GetValue(PropertyKeyDefaultFont);
				using (var path = new GraphicsPath())
				{
					path.AddString("x", font.GdiFontFamily(), (int)font.Style, (float)font.Size, new PointF(0, 0), StringFormat.GenericTypographic);
					var bounds = path.GetBounds();

					if (bounds.Height > 0)
					{
						result = Calc.Rounding.RoundToNumberOfSignificantDigits(bounds.Height, 2, MidpointRounding.ToEven);
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Gets the default major tick length for all graphics in this graph, using the provided property context.
		/// </summary>
		/// <param name="context">The property context.</param>
		/// <returns>Default major tick length in points (1/72 inch).</returns>
		public static double GetDefaultMajorTickLength(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
		{
			double result = 8;

			if (null != context)
			{
				var font = context.GetValue(PropertyKeyDefaultFont);
				using (var path = new GraphicsPath())
				{
					path.AddString("0", font.GdiFontFamily(), (int)font.Style, (float)font.Size, new PointF(0, 0), StringFormat.GenericTypographic);
					var bounds = path.GetBounds();

					if (bounds.Width > 0)
					{
						result = 2 * Calc.Rounding.RoundToNumberOfSignificantDigits(bounds.Width / 2, 2, MidpointRounding.ToEven);
					}
				}
			}
			return result;
		}

		#region Change event handling

		public IDisposable BeginUpdate()
		{
			return _changedEventSuppressor.Suspend();
		}

		public void EndUpdate(ref ISuppressToken locker)
		{
			_changedEventSuppressor.Resume(ref locker);
		}

		public bool IsSuspended
		{
			get
			{
				return _changedEventSuppressor.GetDisabledWithCounting();
			}
		}

		/// <summary>
		/// Fires the Invalidate event.
		/// </summary>
		/// <param name="sender">The layer which needs to be repainted.</param>
		protected internal virtual void OnInvalidate(XYPlotLayer sender)
		{
			OnChanged();
		}

		private void AccumulateChildChangeData(object sender, EventArgs e)
		{
			if (sender != null && _changeEventData == null)
				this._changeEventData = new EventArgs();
		}

		protected bool HandleImmediateChildChangeCases(object sender, EventArgs e)
		{
			return false; // not handled
		}

		protected virtual void OnSelfChanged()
		{
			EhChildChanged(null, EventArgs.Empty);
		}

		/// <summary>
		/// Handle the change notification from the child layers.
		/// </summary>
		/// <param name="sender">The sender of the change notification.</param>
		/// <param name="e">The change details.</param>
		public void EhChildChanged(object sender, System.EventArgs e)
		{
			if (System.Threading.Thread.CurrentThread == _paintThread)
				return;

			if (HandleImmediateChildChangeCases(sender, e))
				return;

			if (this.IsSuspended && sender is Main.ISuspendable)
			{
				_suspendedChildCollection.Add(sender); // add sender to suspended child
				((Main.ISuspendable)sender).Suspend();
				return;
			}

			AccumulateChildChangeData(sender, e);  // AccumulateNotificationData

			if (_isResumeInProgress || IsSuspended)
				return;

			if (_parent is Main.IChildChangedEventSink)
			{
				((Main.IChildChangedEventSink)_parent).EhChildChanged(this, _changeEventData);
				if (IsSuspended) // maybe parent has suspended us now
				{
					this.EhChildChanged(sender, e); // we call the function recursively, but now we are suspended
					return;
				}
			}

			if (_cachedRootLayerSize != _rootLayer.Size)
			{
				_cachedRootLayerSize = _rootLayer.Size;
				OnSizeChanged();
			}

			OnChanged(); // Fire the changed event
		}

		private void EhChangedEventResumes()
		{
			if (null != Changed)
				Changed(this, _changeEventData);
			_changeEventData = null;
		}

		protected virtual void OnChanged()
		{
			if (_changedEventSuppressor.GetEnabledWithCounting())
			{
				if (null != Changed)
					Changed(this, _changeEventData);

				_changeEventData = null;
			}
		}

		#endregion Change event handling

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

		public void Dispose()
		{
			if (null != TunneledEvent)
				TunneledEvent(this, this, Main.PreviewDisposeEventArgs.Empty);

			// Add dispose code for the child elements

			if (null != TunneledEvent)
				TunneledEvent(this, this, Main.DisposeEventArgs.Empty);
		}

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
		public XYPlotLayer GetFirstXYPlotLayer()
		{
			return GetFirstLayerOfType<XYPlotLayer>();
		}

		#endregion Convenience functions
	} // end of class GraphDocument
} // end of namespace