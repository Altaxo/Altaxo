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

using Altaxo.Drawing;
using Altaxo.Drawing.ColorManagement;
using Altaxo.Geometry;
using Altaxo.Gui.Graph.Gdi;
using Altaxo.Main;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
	public class GraphDocument : GraphDocumentBase
	{
		protected const double DefaultRootLayerSizeX = 697.68054;
		protected const double DefaultRootLayerSizeY = 451.44;

		private SingleGraphPrintOptions _printOptions;

		private HostLayer _rootLayer;

		/// <summary>The root layer size, cached here only for deciding whether to raise the <see cref="GraphDocumentBase.SizeChanged"/> event. Do not use it otherwise.</summary>
		[NonSerialized]
		private PointD2D _cachedRootLayerSize;

		#region Property keys

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
					var ctrl = new ItemLocationDirectController() { UseDocumentCopy = Gui.UseDocument.Copy };
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
				() => GdiFontManager.GetFontX(GdiFontManager.GenericSansSerifFontFamilyName, 18, FontXStyle.Regular))
				{
					EditingControllerCreation = (doc) =>
					{
						var ctrl = new Gui.Common.Drawing.FontXController { UseDocumentCopy = Gui.UseDocument.Copy };
						ctrl.InitializeDocument(doc);
						return ctrl;
					}
				};

		public static readonly Main.Properties.PropertyKey<Altaxo.Drawing.NamedColor> PropertyKeyDefaultForeColor =
			new Main.Properties.PropertyKey<Altaxo.Drawing.NamedColor>(
"2F138FDD-B96A-4C03-9BEF-83FC412E50B2",
"Graph\\Colors\\Default fore color",
Main.Properties.PropertyLevel.Document,
typeof(GraphDocument),
() => Altaxo.Drawing.NamedColors.Black
)
			{
				EditingControllerCreation = (doc) =>
				{
					var ctrl = new Gui.Graph.ColorManagement.NamedColorChoiceController { UseDocumentCopy = Gui.UseDocument.Copy };
					ctrl.InitializeDocument(doc);
					return ctrl;
				}
			};

		public static readonly Main.Properties.PropertyKey<Altaxo.Drawing.NamedColor> PropertyKeyDefaultBackColor =
	new Main.Properties.PropertyKey<Altaxo.Drawing.NamedColor>(
"90BB0E83-D1A4-40B7-9607-55D4B9C272C3",
"Graph\\Colors\\Default back color",
Main.Properties.PropertyLevel.Document,
typeof(GraphDocument),
() => Altaxo.Drawing.NamedColors.AliceBlue
)
	{
		EditingControllerCreation = (doc) =>
		{
			var ctrl = new Gui.Graph.ColorManagement.NamedColorChoiceController { UseDocumentCopy = Gui.UseDocument.Copy };
			ctrl.InitializeDocument(doc);
			return ctrl;
		}
	};

		public static readonly Main.Properties.PropertyKey<NamedColor> PropertyKeyDefaultPlotColor =
new Main.Properties.PropertyKey<NamedColor>(
"D5DB4695-2630-4B7D-83E3-71CA3873B362",
"Graph\\Colors\\Default plot color",
Main.Properties.PropertyLevel.Document,
typeof(GraphDocument),
() =>
{
	return ColorSetManager.Instance.BuiltinDarkPlotColors[0];
}
)
{
	EditingControllerCreation = (doc) =>
	{
		var ctrl = new Gui.Graph.ColorManagement.NamedColorChoiceController { UseDocumentCopy = Gui.UseDocument.Copy, ShowPlotColorsOnly = true };
		ctrl.InitializeDocument(doc);
		return ctrl;
	}
};

		#endregion Property keys

		public SingleGraphPrintOptions PrintOptions
		{
			get { return _printOptions; }
			set { _printOptions = value; }
		}

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
#pragma warning disable CS0618 // Type or member is obsolete
				var layers = (XYPlotLayer.XYPlotLayerCollection)info.GetValue("LayerList", s);
#pragma warning restore CS0618 // Type or member is obsolete
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
#pragma warning disable CS0618 // Type or member is obsolete
				var layers = (XYPlotLayer.XYPlotLayerCollection)info.GetValue("LayerList", s);
#pragma warning restore CS0618 // Type or member is obsolete
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
					object propval = info.GetValue("Value", s.PropertyBagNotNull);
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
				s.PropertyBag = (Main.Properties.PropertyBag)info.GetValue("Properties", s);
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
			this.RootLayer = new HostLayer() { ParentObject = this };
			this.RootLayer.Location = new ItemLocationDirect { SizeX = RADouble.NewAbs(DefaultRootLayerSizeX), SizeY = RADouble.NewAbs(DefaultRootLayerSizeY) };
		}

		private void EhNotesChanged(object sender, PropertyChangedEventArgs e)
		{
			EhChildChanged(sender, e);
		}

		public GraphDocument(GraphDocument from)
		{
			using (var suppressToken = SuspendGetToken())
			{
				_creationTime = _lastChangeTime = DateTime.UtcNow;
				this.RootLayer = new HostLayer(null, new ItemLocationDirect { SizeX = RADouble.NewAbs(814), SizeY = RADouble.NewAbs(567) });

				CopyFrom(from, GraphCopyOptions.All);

				suppressToken.ResumeSilently();
			}
		}

		public void CopyFrom(GraphDocument from, GraphCopyOptions options)
		{
			if (object.ReferenceEquals(this, from))
				return;

			using (var suspendToken = SuspendGetToken())
			{
				if (0 != (options & GraphCopyOptions.CloneNotes))
				{
					ChildCopyToMember(ref _notes, from._notes);
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

				suspendToken.Resume();
			}
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != _rootLayer)
				yield return new Main.DocumentNodeAndName(_rootLayer, () => _rootLayer = null, "RootLayer");

			if (null != _graphProperties)
				yield return new Main.DocumentNodeAndName(_graphProperties, () => _graphProperties = null, "GraphProperties");

			if (null != _notes)
				yield return new Main.DocumentNodeAndName(_notes, () => _notes = null, "Notes");
		}

		public override object Clone()
		{
			return new GraphDocument(this);
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
		private RectangleD2D GetBounds()
		{
			var s = _rootLayer.Size;
			var p1 = _rootLayer.TransformCoordinatesFromHereToParent(new PointD2D(0, 0));
			var p2 = _rootLayer.TransformCoordinatesFromHereToParent(new PointD2D(s.X, 0));
			var p3 = _rootLayer.TransformCoordinatesFromHereToParent(new PointD2D(0, s.Y));
			var p4 = _rootLayer.TransformCoordinatesFromHereToParent(new PointD2D(s.X, s.Y));

			var r = new RectangleD2D(p1, PointD2D.Empty);
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
		/// The collection of layers of the graph.
		/// </summary>
		public HostLayer RootLayer
		{
			get { return _rootLayer; }
			private set
			{
				ChildSetMember(ref _rootLayer, value);
			}
		}

		private object _paintLock = new object();

		/// <summary>
		/// Paints the graph.
		/// </summary>
		/// <param name="g">The graphic contents to paint to.</param>
		/// <param name="bForPrinting">Indicates if the painting is for printing purposes. Not used for the moment.</param>
		/// <remarks>The reference point (0,0) of the GraphDocument
		/// is the top left corner of the printable area (and not of the page area!). The hosting class has to translate the graphics origin
		/// to the top left corner of the printable area before calling this routine.</remarks>
		public void Paint(Graphics g, bool bForPrinting)
		{
			const int MaxFixupRetries = 10;

			if (System.Threading.Thread.CurrentThread == _paintThread)
				throw new InvalidOperationException("DoPaint is called reentrant (i.e. from the same thread that is already executing DoPaint");

			lock (_paintLock)
			{
				if (!(null == _paintThread))
					throw new InvalidProgramException("We waited, thus _paintThread should be null");

				_paintThread = System.Threading.Thread.CurrentThread; // Suppress events that are fired during paint

				try
				{
					// First set the current thread's document culture
					_paintThread.CurrentCulture = this.GetPropertyValue(Altaxo.Settings.CultureSettings.PropertyKeyDocumentCulture, null).Culture;

					try
					{
						_isFixupInternalDataStructuresActive = true;

						for (int ithRetry = 1; ithRetry <= MaxFixupRetries; ++ithRetry)
						{
							try
							{
								_hasFixupChangedAnything = false;
								AdjustRootLayerPositionToFitIntoZeroOffsetRectangle();
								RootLayer.FixupInternalDataStructures();
								if (!_hasFixupChangedAnything)
									break;
#if DEBUG
								if (ithRetry == MaxFixupRetries)
								{
									Current.Console.WriteLine("Warning: MaxFixupRetries exceeded during painting of graph {0}.", this.Name);
								}
#endif
							}
							catch (Exception)
							{
								if (ithRetry == MaxFixupRetries)
								{
									throw;
								}
							}
						}
					}
					finally
					{
						_isFixupInternalDataStructuresActive = false;
					}

					var paintContext = new GdiPaintContext();

					RootLayer.PaintPreprocessing(paintContext);

					RootLayer.Paint(g, paintContext);

					RootLayer.PaintPostprocessing();
				}
				finally
				{
					_paintThread = null;
				}
			} // end of lock
		} // end of function DoPaint

		public static FontX GetDefaultFont(Altaxo.Main.Properties.IReadOnlyPropertyBag context)
		{
			if (null != context)
				return context.GetValue(PropertyKeyDefaultFont);
			else
				return GdiFontManager.GetFontX(GdiFontManager.GenericSansSerifFontFamilyName, 18, FontXStyle.Regular);
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
					path.AddString("-", GdiFontManager.ToGdi(font).FontFamily, (int)font.Style, (float)font.Size, new PointF(0, 0), StringFormat.GenericTypographic);
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
				return font.Size;
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
					path.AddString("0", GdiFontManager.ToGdi(font).FontFamily, (int)font.Style, (float)font.Size, new PointF(0, 0), StringFormat.GenericTypographic);
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

		/// <summary>
		/// Fires the Invalidate event.
		/// </summary>
		/// <param name="sender">The layer which needs to be repainted.</param>
		protected internal virtual void OnInvalidate(XYPlotLayer sender)
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

		#endregion Change event handling

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
