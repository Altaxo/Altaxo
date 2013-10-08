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
		System.ICloneable,
		IChangedEventSource,
		Main.IChildChangedEventSink,
		Main.IDocumentNode,
		Main.INameOwner
	{
		// following default unit is point (1/72 inch)
		/// <summary>For the graph elements all the units are in points. One point is 1/72 inch.</summary>
		protected const float UnitPerInch = 72;

		protected const double DefaultRootLayerSizeX = 697.68054;
		protected const double DefaultRootLayerSizeY = 451.44;

		/// <summary>
		/// Overall size of the page (usually the size of the sheet of paper that is selected as printing document) in point (1/72 inch)
		/// </summary>
		/// <remarks>The value is only used by hosting classes, since the reference point (0,0) of the GraphDocument
		/// is the top left corner of the printable area. The hosting class has to translate the graphics origin
		/// to that point before calling the painting routine <see cref="DoPaint"/>.</remarks>
		//private RectangleF _pageBounds = new RectangleF(0, 0, 842, 595);

		/// <summary>
		/// The printable area of the document, i.e. the page size minus the margins at each side in points (1/72 inch)
		/// </summary>
		//private RectangleF _printableBounds = new RectangleF(14, 14, 814, 567);

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
		protected Dictionary<string, object> _graphProperties;

		/// <summary>Event fired when anything here changed.</summary>
		[field: NonSerialized]
		public event System.EventHandler Changed;

		[NonSerialized]
		private Main.EventSuppressor _changedEventSuppressor;

		/// <summary>Events which are fired from this thread are not distributed.</summary>
		[NonSerialized]
		private System.Threading.Thread _paintThread;

		/// <summary>
		/// Event to signal that the name of this object has changed.
		/// </summary>
		[field: NonSerialized]
		public event Action<Main.INameOwner, string> NameChanged;

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

				var layers = (IList<XYPlotLayer>)info.GetValue("LayerList", s);
				s._rootLayer.Size = printableBounds.Size;
				s._rootLayer.Location = new ItemLocationDirect { SizeX = Calc.RelativeOrAbsoluteValue.NewAbsoluteValue(printableBounds.Size.Width), SizeY = Calc.RelativeOrAbsoluteValue.NewAbsoluteValue(printableBounds.Size.Height) };
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
				s._rootLayer.Location = new ItemLocationDirect { SizeX = Calc.RelativeOrAbsoluteValue.NewAbsoluteValue(printableBounds.Size.Width), SizeY = Calc.RelativeOrAbsoluteValue.NewAbsoluteValue(printableBounds.Size.Height) };
				var layers = (IList<XYPlotLayer>)info.GetValue("LayerList", s);
				foreach (var l in layers)
					s._rootLayer.Layers.Add(l);

				// new in version 1 - Add graph properties
				int numberproperties = info.OpenArray(); // "GraphProperties"
				for (int i = 0; i < numberproperties; i++)
				{
					info.OpenElement(); // "e"
					string propkey = info.GetString("Key");
					object propval = info.GetValue("Value", parent);
					info.CloseElement(); // "e"
					s.SetGraphProperty(propkey, propval);
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
			this._rootLayer = new HostLayer();
			this._rootLayer.ParentObject = this;
			this._rootLayer.Location = new ItemLocationDirect { SizeX = Calc.RelativeOrAbsoluteValue.NewAbsoluteValue(DefaultRootLayerSizeX), SizeY = Calc.RelativeOrAbsoluteValue.NewAbsoluteValue(DefaultRootLayerSizeY) };
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
				this._rootLayer = new HostLayer(null, new ItemLocationDirect { SizeX = Calc.RelativeOrAbsoluteValue.NewAbsoluteValue(814), SizeY = Calc.RelativeOrAbsoluteValue.NewAbsoluteValue(567) });
				this._rootLayer.ParentObject = this;

				CopyFrom(from, GraphCopyOptions.All);
			}
			finally
			{
				_changedEventSuppressor.Resume(ref suppressToken, true);
			}
		}

		public void CopyFrom(GraphDocument from, GraphCopyOptions options)
		{
			if (object.ReferenceEquals(this, from))
				return;

			if (0 != (options & GraphCopyOptions.CopyGraphSize))
			{
				this._rootLayer.Location = (IItemLocation)from._rootLayer.Location.Clone();
			}

			if (0 != (options & GraphCopyOptions.CloneNotes))
			{
				if (null != _notes) _notes.PropertyChanged -= EhNotesChanged;
				this._notes = from._notes.Clone();
				this._notes.PropertyChanged += EhNotesChanged;
			}

			if (0 != (options & GraphCopyOptions.CloneProperties))
			{
				// Clone also the table properties (deep copy)
				if (from._graphProperties != null)
				{
					foreach (string key in from._graphProperties.Keys)
					{
						ICloneable val = from._graphProperties[key] as ICloneable;
						if (null != val)
							this.SetGraphProperty(key, val.Clone());
					}
				}
			}

			// the order is important here: clone the layers only before setting the printable graph bounds and other
			// properties, otherwise some errors will happen
			if (GraphCopyOptions.CopyLayerAll == (options & GraphCopyOptions.CopyLayerAll))
			{
				this._rootLayer = (HostLayer)from._rootLayer.Clone();
			}
			else if (0 != (options & GraphCopyOptions.CopyLayerAll))
			{
				// don't clone the layers, but copy the style of each each of the souce layers to the destination layers - this is to be done recursively
				this._rootLayer.CopyFrom(from._rootLayer, options);
				this._rootLayer.ParentLayer = this._rootLayer;
			}
			this._rootLayer.ParentObject = this;
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
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException("New name is null or empty");

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

		/// <summary>
		/// The table properties, key is a string, val is a object you want to store here.
		/// </summary>
		/// <remarks>The properties are saved on disc (with exception of those who's name starts with "tmp/".
		/// If the property you want to store is only temporary, the property name should therefore
		/// start with "tmp/".</remarks>
		public void SetGraphProperty(string key, object val)
		{
			if (_graphProperties == null)
				_graphProperties = new Dictionary<string, object>();

			if (_graphProperties.ContainsKey(key))
				_graphProperties[key] = val;
			else
				_graphProperties.Add(key, val);
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
			System.Diagnostics.Debug.Assert(null == _paintThread, "DoPaint was called reentrant or from different threads");
			_paintThread = System.Threading.Thread.CurrentThread; // Suppress events that are fired during paint
			try
			{
				RootLayer.PaintPreprocessing();

				RootLayer.Paint(g, bForPrinting);

				RootLayer.PaintPostprocessing();
			}
			finally
			{
				_paintThread = null;
			}
		} // end of function DoPaint

		#region Change event handling

		public IDisposable BeginUpdate()
		{
			return _changedEventSuppressor.Suspend();
		}

		public void EndUpdate(ref IDisposable locker)
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

		#region IChangedEventSource Members

		#endregion IChangedEventSource Members
	} // end of class GraphDocument
} // end of namespace