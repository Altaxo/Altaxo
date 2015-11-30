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

namespace Altaxo.Graph
{
	/// <summary>
	/// This is the base class of graphs.
	/// </summary>
	/// <remarks>The coordinate system of the graphs is in units of points (1/72 inch). </remarks>
	public abstract class GraphDocumentBase
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

		/// <summary>
		/// The name of the graph.
		/// </summary>
		protected string _name;

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
		protected volatile System.Threading.Thread _paintThread;

		/// <summary>Event fired if the size of this document (i.e. the size of the root layer) changed.</summary>
		[field: NonSerialized]
		public event EventHandler SizeChanged;

		[NonSerialized]
		protected bool _isFixupInternalDataStructuresActive;

		/// <summary>This flag is set if during fixup anything has changed.</summary>
		[NonSerialized]
		protected bool _hasFixupChangedAnything;

		/// <summary>
		/// Creates a empty GraphDocument with no layers and a standard size of A4 landscape.
		/// </summary>
		protected GraphDocumentBase()
		{
			_creationTime = _lastChangeTime = DateTime.UtcNow;
			_notes = new TextBackedConsole() { ParentObject = this };
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		/// A new object that is a copy of this instance.
		/// </returns>
		public abstract object Clone();

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
		public abstract void VisitDocumentReferences(DocNodeProxyReporter Report);

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
		/// Gets/sets the size of this graph in points (1/72 inch). The value returned is exactly the size in points that an image of the graph on 2D paper will take.
		/// </summary>
		/// <value>
		/// The size of the graph in points (1/72 inch).
		/// </value>
		public abstract PointD2D Size { get; set; }

		#region Change handling

		/// <summary>
		/// Fires the <see cref="SizeChanged" /> event.
		/// </summary>
		protected void OnSizeChanged()
		{
			if (SizeChanged != null)
				SizeChanged(this, EventArgs.Empty);
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

		protected override void AccumulateChangeData(object sender, EventArgs e)
		{
			if (sender != null && _accumulatedEventData == null)
				this._accumulatedEventData = EventArgs.Empty;
		}

		#endregion Change handling

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
	}
}