/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002 Dr. Dirk Lellinger
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

using System;
using System.ComponentModel;        
using System.Reflection;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;


namespace Altaxo.Graph
{
	/// <summary>
	/// Holds a bunch of layers by it's index.
	/// </summary>
	/// <remarks>The <see cref="GraphDocument"/> inherits from this class, but implements
	/// its own function for adding the layers and moving them, since it has to track
	/// all changes to the layers.</remarks>
	[SerializationSurrogate(0,typeof(XYPlotLayerCollection.SerializationSurrogate0))]
	[SerializationVersion(0)]
	public class XYPlotLayerCollection 
		:
		Altaxo.Data.CollectionBase,
		System.Runtime.Serialization.IDeserializationCallback, 
		Main.IChangedEventSource,
		Main.IChildChangedEventSink,
		System.ICloneable, 
		Main.IDocumentNode,
		Main.INamedObjectCollection
	{
		/// <summary>Fired when something in this collection changed, as for instance
		/// adding or deleting layers, or exchanging layers.</summary>
		public event System.EventHandler LayerCollectionChanged;
			

		/// <summary>
		/// Fired if either the layer collection changed or something in the layers changed
		/// </summary>
		public event System.EventHandler Changed;

		private object m_Parent;

		private RectangleF m_PrintableBounds; // do not serialize this value, its only cached


		#region "Serialization"

		/// <summary>Used to serialize the XYPlotLayerCollection Version 0.</summary>
		public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
		{
			/// <summary>
			/// Serializes XYPlotLayerCollection Version 0.
			/// </summary>
			/// <param name="obj">The XYPlotLayerCollection to serialize.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context	)
			{
				XYPlotLayerCollection s = (XYPlotLayerCollection)obj;
				info.AddValue("Data",s.myList);
			}

			/// <summary>
			/// Deserializes the XYPlotLayerCollection Version 0.
			/// </summary>
			/// <param name="obj">The empty GraphDocument object to deserialize into.</param>
			/// <param name="info">The serialization info.</param>
			/// <param name="context">The streaming context.</param>
			/// <param name="selector">The deserialization surrogate selector.</param>
			/// <returns>The deserialized GraphDocument.</returns>
			public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
			{
				XYPlotLayerCollection s = (XYPlotLayerCollection)obj;

				s.myList = (System.Collections.ArrayList)info.GetValue("Data",typeof(System.Collections.ArrayList));
				return s;
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(XYPlotLayerCollection),0)]
			public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				XYPlotLayerCollection s = (XYPlotLayerCollection)obj;
					
				info.CreateArray("LayerArray",s.Count);
				for(int i=0;i<s.Count;i++)
					info.AddValue("XYPlotLayer",s[i]);
				info.CommitArray();

			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				XYPlotLayerCollection s = null!=o ? (XYPlotLayerCollection)o : new XYPlotLayerCollection();
					
					
				int count = info.OpenArray();				
				for(int i=0;i<count;i++)
				{
					XYPlotLayer l = (XYPlotLayer)info.GetValue("XYPlotLayer",s);
					s.Add(l);
				}
				info.CloseArray(count);

				return s;
			}
		}


		/// <summary>
		/// Finale measures after deserialization.
		/// </summary>
		/// <param name="obj">Not used.</param>
		public virtual void OnDeserialization(object obj)
		{
			// set the parent and the number of all items
			for(int i=0;i<base.InnerList.Count;i++)
				this[i].SetParentAndNumber(this,i);
		}
		#endregion



		/// <summary>
		/// Creates an empty XYPlotLayerCollection without parent.
		/// </summary>
		public XYPlotLayerCollection()
		{
		}

		/// <summary>
		/// Copy constructor. Clones all objects in this collection.
		/// </summary>
		/// <param name="from">The collection to clone from.</param>
		public XYPlotLayerCollection(XYPlotLayerCollection from)
		{
			for(int i=0;i<from.Count;i++)
				this.Add((XYPlotLayer)from[i].Clone());
	
			// now we have to fix the linked layer list, since the LinkedLayer property of the Layers point to the original layers
			// and not to the cloned layers!
			for(int i=0;i<Count;i++)
			{
				if(null!=from[i].LinkedLayer)
				{
					this[i].LinkedLayer = this[from[i].LinkedLayer.Number];
				}
			}
		}
		
		public virtual object Clone()
		{
			return new XYPlotLayerCollection(this);
		}


		/// <summary>
		/// The boundaries of the printable area of the page in points (1/72 inch).
		/// </summary>
		public RectangleF PrintableGraphBounds
		{
			get { return m_PrintableBounds; }
		}
		public void SetPrintableGraphBounds(RectangleF val, bool bRescale)
		{
			RectangleF oldBounds = m_PrintableBounds;
			m_PrintableBounds=val;

			if(m_PrintableBounds!=oldBounds)
			{
				foreach(XYPlotLayer l in InnerList)
					l.SetPrintableGraphBounds( val, bRescale );
			}
		}
			
		/// <summary>
		/// References the layer at index i.
		/// </summary>
		/// <value>The layer at index <paramref name="i"/>.</value>
		public virtual XYPlotLayer this[int i]
		{
			get 
			{
				// for the getter, we can use the innerlist, since no actions are defined for that
				return (XYPlotLayer)base.InnerList[i];
			}
			set
			{
				// we use List here since we want to have custom actions defined below
				List[i] = value;
			}
		}

		/// <summary>
		/// This will exchange layer i and layer j.
		/// </summary>
		/// <param name="i">Index of the one element to exchange.</param>
		/// <param name="j">Index of the other element to exchange.</param>
		/// <remarks>To avoid the destruction of the linked layer connections, we avoid
		/// firing the custom list actions here by using the InnerList property and
		/// correct the layer numbers of the two exchanged elements directly.</remarks>
		public virtual void ExchangeElements(int i, int j)
		{
			// we use the inner list to do that because we do not want
			// to have custom actions (this is mainly because otherwise we have
			// a remove action that will destoy the linked layer connections

			object o = base.InnerList[i];
			base.InnerList[i] = base.InnerList[j];
			base.InnerList[j] = o;

			// correct the XYPlotLayer numbers for the two exchanged layers
			this[i].SetParentAndNumber(this,i);
			this[j].SetParentAndNumber(this,j);

			OnLayerCollectionChanged();
		}


		/// <summary>
		/// Adds a layer to this layer collection.
		/// </summary>
		/// <param name="l"></param>
		public void Add(XYPlotLayer l)
		{
			// we use List for adding since we want to have custom actions below
			List.Add(l);
			// since we use List, we don't need to have OnLayerCollectionChanged here!
		}

		/// <summary>
		/// Perform custom action on clearing: remove the parent attribute and the layer number from all the layers.
		/// </summary>
		protected override void OnClear()
		{
			foreach(XYPlotLayer l in InnerList)
				l.SetParentAndNumber(null,0);

			OnLayerCollectionChanged();
		}

		/// <summary>
		/// Perform custom action if one element removed: renumber the remaining elements.
		/// </summary>
		/// <param name="idx">The index where the element was removed. </param>
		/// <param name="oldValue">The removed element.</param>
		protected override void OnRemoveComplete(int idx, object oldValue)
		{
			((XYPlotLayer)oldValue).SetParentAndNumber(null,0);

			// renumber the layers from i to count
			for(int i=idx;i<Count;i++)
			{
				this[i].SetParentAndNumber(this,i);

				// fix linked layer connections if neccessary
				if(XYPlotLayer.ReferenceEquals(oldValue,this[i]))
					this[i].LinkedLayer=null;
			}
			OnLayerCollectionChanged();
		}

		/// <summary>
		/// Perform custom action if one element is set: set parent and number of the newly
		/// set element.
		/// </summary>
		/// <param name="index">The index where the element is set.</param>
		/// <param name="oldValue">The old value of the list element.</param>
		/// <param name="newValue">The new value this list element is set to.</param>
		protected override void OnSetComplete(int index, object oldValue,	object newValue	)
		{
			((XYPlotLayer)oldValue).SetParentAndNumber(null,0);
			((XYPlotLayer)newValue).SetParentAndNumber(this,index);
			((XYPlotLayer)newValue).SetPrintableGraphBounds(this.PrintableGraphBounds,true);

			for(int i=0;i<Count;i++)
			{
				// fix linked layer connections if neccessary
				if(XYPlotLayer.ReferenceEquals(oldValue,this[i]))
					this[i].LinkedLayer=null;
			}

			OnLayerCollectionChanged();
		}

		/// <summary>
		/// Perform custom action if an element is inserted: set parent and number
		/// of the inserted element and renumber the other elements.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="newValue"></param>
		protected override void OnInsertComplete(int index,object newValue)
		{
			// renumber the inserted and the following layers
			for(int i=index;i<Count;i++)
				this[i].SetParentAndNumber(this,i);

			OnLayerCollectionChanged();
		}


		protected virtual void OnLayerCollectionChanged()
		{
			if(null!=LayerCollectionChanged)
				LayerCollectionChanged(this,new EventArgs());
			
			OnChanged();
		}

		#region Change event handling

		protected System.EventArgs m_ChangeData=null;
		protected bool             m_ResumeInProgress=false;
		protected System.Collections.ArrayList m_SuspendedChildCollection=new System.Collections.ArrayList();

		
		public bool IsSuspended
		{
			get 
			{
				return false; // m_SuspendCount>0;
			}
		}

#if false
		public void Suspend()
		{
			System.Diagnostics.Debug.Assert(m_SuspendCount>=0,"SuspendCount must always be greater or equal to zero");		

			++m_SuspendCount; // suspend one step higher
		}

		public void Resume()
		{
			System.Diagnostics.Debug.Assert(m_SuspendCount>=0,"SuspendCount must always be greater or equal to zero");		
			if(m_SuspendCount>0 && (--m_SuspendCount)==0)
			{
				this.m_ResumeInProgress = true;
				foreach(Main.ISuspendable obj in m_SuspendedChildCollection)
					obj.Resume();
				m_SuspendedChildCollection.Clear();
				this.m_ResumeInProgress = false;

				// send accumulated data if available and release it thereafter
				if(null!=m_ChangeData)
				{
					if(m_Parent is Main.IChildChangedEventSink)
					{
						((Main.IChildChangedEventSink)m_Parent).OnChildChanged(this, m_ChangeData);
					}
					if(!IsSuspended)
					{
						OnDataChanged(); // Fire the changed event
					}		
				}
			}
		}

#endif


		void AccumulateChildChangeData(object sender, EventArgs e)
		{
			if(sender!=null && m_ChangeData==null)
				this.m_ChangeData=new EventArgs();
		}		

		protected bool HandleImmediateChildChangeCases(object sender, EventArgs e)
		{
			return false; // not handled
		}

		protected virtual void OnSelfChanged()
		{
			OnChildChanged(null,EventArgs.Empty);
		}


		/// <summary>
		/// Handle the change notification from the child layers.
		/// </summary>
		/// <param name="sender">The sender of the change notification.</param>
		/// <param name="e">The change details.</param>
		public void OnChildChanged(object sender, System.EventArgs e)
		{
			if(HandleImmediateChildChangeCases(sender, e))
				return;

			if(this.IsSuspended &&  sender is Main.ISuspendable)
			{
				m_SuspendedChildCollection.Add(sender); // add sender to suspended child
				((Main.ISuspendable)sender).Suspend();
				return;
			}

			AccumulateChildChangeData(sender,e);	// AccumulateNotificationData
			
			if(m_ResumeInProgress || IsSuspended)
				return;

			if(m_Parent is Main.IChildChangedEventSink )
			{
				((Main.IChildChangedEventSink)m_Parent).OnChildChanged(this, m_ChangeData);
				if(IsSuspended) // maybe parent has suspended us now
				{
					this.OnChildChanged(sender, e); // we call the function recursively, but now we are suspended
					return;
				}
			}
			
			OnChanged(); // Fire the changed event
		}

		protected virtual void OnChanged()
		{
			if(null!=Changed)
				Changed(this, m_ChangeData);

			m_ChangeData=null;
		}

		#endregion

		#region IDocumentNode Members

		public object ParentObject
		{
			get
			{
				// TODO:  Add XYPlotLayerCollection.ParentObject getter implementation
				return m_Parent;
			}
			set 
			{
				m_Parent = value;
			}
		}

		public string Name
		{
			get
			{
				return "XYPlotLayer";
			}
		}

		#endregion

		#region INamedObjectCollection Members

		/// <summary>
		/// Returns the document name of the layer at index i. Actually, this is a name of the form L0, L1, L2 and so on.
		/// </summary>
		/// <param name="i">The layer index.</param>
		/// <returns>The name of the layer at index i.</returns>
		public static string GetNameOfLayer(int i)
		{
			return XYPlotLayer.GetDefaultNameOfLayer(i);
		}

		public object GetChildObjectNamed(string name)
		{
			for(int i=0;i<this.Count;i++)
			{
				if(GetNameOfLayer(i)==name)
					return this[i];
			}
			return null;
		}

		public string GetNameOfChildObject(object o)
		{
			for(int i=0;i<this.Count;i++)
			{
				if(object.ReferenceEquals(o,this[i]))
					return GetNameOfLayer(i);
			}
			return null;
		}

		#endregion
	}
	
}
