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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Altaxo.Graph.Gdi.Plot
{
	using Altaxo.Main;
	using Graph.Plot.Groups;
	using Groups;

	[Serializable]
	public abstract class PlotItem
	:
		Main.SuspendableDocumentNodeWithSetOfEventArgs,
		IGPlotItem,
		Main.INamedObjectCollection
	{
		public virtual bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			var from = obj as PlotItem;
			if (null != from)
			{
				//this._parent = from._parent;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Get/sets the style object of this plot.
		/// </summary>
		public abstract IDocumentLeafNode StyleObject { get; set; }

		public abstract IDocumentLeafNode DataObject { get; }

		/// <summary>
		/// The name of the plot. It can be of different length. An argument of zero or less
		/// returns the shortest possible name, higher values return more verbose names.
		/// </summary>
		/// <param name="level">The naming level, 0 returns the shortest possible name, 1 or more returns more
		/// verbose names.</param>
		/// <returns>The name of the plot.</returns>
		public abstract string GetName(int level);

		/// <summary>
		/// The name of the plot. The style how to find the name is determined by the style argument. The possible
		/// styles depend on the type of plot item.
		/// </summary>
		/// <param name="style">The style determines the "verbosity" of the plot name.</param>
		/// <returns>The name of the plot.</returns>
		public abstract string GetName(string style);

		/// <summary>
		/// This paints the plot to the layer.
		/// </summary>
		/// <param name="g">The graphics context.</param>
		/// <param name="layer">The plot layer.</param>
		/// <param name="previousPlotItem">Previous plot item.</param>
		/// <param name="nextPlotItem">Next plot item.</param>
		/// <returns>A data object, which can be used by the next plot item for some styles (like fill style).</returns>
		public abstract void Paint(Graphics g, IPlotArea layer, IGPlotItem previousPlotItem, IGPlotItem nextPlotItem);

		/// <summary>
		/// Called after painting has finished. Can be used to release resources. Must be overridden by a derived class.
		/// </summary>
		public virtual void PaintPostprocessing()
		{
		}

		/// <summary>
		/// This routine ensures that the plot item updates all its cached data and send the appropriate
		/// events if something has changed. Called before the layer paint routine paints the axes because
		/// it must be ensured that the axes are scaled correctly before the plots are painted.
		/// </summary>
		/// <param name="layer">The plot layer.</param>
		public abstract void PrepareScales(IPlotArea layer);

		/// <summary>
		/// Creates a cloned copy of this object.
		/// </summary>
		/// <returns>The cloned copy of this object.</returns>
		/// <remarks>The data (DataColumns which belongs to a table in the document's DataTableCollection) are not cloned, only the reference to this columns is cloned.</remarks>
		public abstract object Clone();

		public virtual PlotItemCollection ParentCollection
		{
			get
			{
				return _parent as PlotItemCollection;
			}
		}

		IGPlotItem INodeWithParentNode<IGPlotItem>.ParentNode
		{
			get
			{
				return _parent as PlotItemCollection;
			}
		}

		IList<IGPlotItem> ITreeListNode<IGPlotItem>.ChildNodes
		{
			get
			{
				return null; // PlotItems don't have parent nodes.
			}
		}

		IEnumerable<IGPlotItem> ITreeNode<IGPlotItem>.ChildNodes
		{
			get
			{
				return null; // PlotItems don't have parent nodes of type IGPlotItem
			}
		}

		/// <summary>
		/// Handles the case when a child changes, and a reaction is neccessary independently on the suspend state of the table. It is used here to change the event args
		/// coming from the StyleObject to <see cref="PlotItemStyleChangedEventArgs"/> and event args coming from the data object to <see cref="PlotItemDataChangedEventArgs"/>.
		/// </summary>
		/// <param name="sender">The sender of the event, usually a child of this object.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		/// <returns>
		/// True if the event will not change the state of the object and the handling of the event is completely done. Thus, if returning <c>true</c>, the object is considered as 'not changed'.
		/// If in doubt, return <c>false</c>. This will allow the further processing of the event.
		/// </returns>
		protected override bool HandleHighPriorityChildChangeCases(object sender, ref EventArgs e)
		{
			if (object.ReferenceEquals(sender, StyleObject) && e == EventArgs.Empty)
				e = PlotItemStyleChangedEventArgs.Empty;
			else if (object.ReferenceEquals(sender, DataObject) && e == EventArgs.Empty)
				e = PlotItemDataChangedEventArgs.Empty;

			return base.HandleHighPriorityChildChangeCases(sender, ref e);
		}

		/// <summary>
		/// retrieves the object with the name <code>name</code>.
		/// </summary>
		/// <param name="name">The objects name.</param>
		/// <returns>The object with the specified name.</returns>
		public override IDocumentLeafNode GetChildObjectNamed(string name)
		{
			switch (name)
			{
				case "Style":
					return StyleObject;

				case "Data":
					return DataObject;

				default:
					return null;
			}
		}

		/// <summary>
		/// Retrieves the name of the provided object.
		/// </summary>
		/// <param name="o">The object for which the name should be found.</param>
		/// <returns>The name of the object. Null if the object is not found. String.Empty if the object is found but has no name.</returns>
		public override string GetNameOfChildObject(IDocumentLeafNode o)
		{
			if (null == o)
				return null;
			else if (object.ReferenceEquals(o, StyleObject))
				return "Style";
			else if (object.ReferenceEquals(o, DataObject))
				return "Data";
			else
				return null;
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			if (null != StyleObject)
				yield return new Main.DocumentNodeAndName(StyleObject, "Style");
			if (null != DataObject)
				yield return new Main.DocumentNodeAndName(DataObject, "Data");
		}

		/// <summary>
		/// Test wether the mouse hits a plot item. The default implementation here returns null.
		/// If you want to have a reaction on mouse click on a curve, implement this function.
		/// </summary>
		/// <param name="layer">The layer in which this plot item is drawn into.</param>
		/// <param name="hitpoint">The point where the mouse is pressed.</param>
		/// <returns>Null if no hit, or a <see cref="IHitTestObject" /> if there was a hit.</returns>
		public virtual IHitTestObject HitTest(IPlotArea layer, PointD2D hitpoint)
		{
			return null;
		}

		#region IPlotItem Members

		public abstract void CollectStyles(PlotGroupStyleCollection styles);

		public abstract void PrepareGroupStyles(PlotGroupStyleCollection externalGroups, IPlotArea layer);

		public abstract void ApplyGroupStyles(PlotGroupStyleCollection externalGroups);

		/// <summary>
		/// Sets the plot style (or sub plot styles) in this item according to a template provided by the plot item in the template argument.
		/// </summary>
		/// <param name="template">The template item to copy the plot styles from.</param>
		/// <param name="strictness">Denotes the strictness the styles are copied from the template. See <see cref="PlotGroupStrictness" /> for more information.</param>
		public abstract void SetPlotStyleFromTemplate(IGPlotItem template, PlotGroupStrictness strictness);

		/// <summary>
		/// Paints a symbol for this plot item for use in a legend.
		/// </summary>
		/// <param name="g">The graphics context.</param>
		/// <param name="location">The rectangle where the symbol should be painted into.</param>
		public virtual void PaintSymbol(Graphics g, RectangleF location)
		{
		}

		/// <summary>
		/// Replaces path of items (intended for data items like tables and columns) by other paths. Thus it is possible
		/// to change a plot so that the plot items refer to another table.
		/// </summary>
		/// <param name="Report">Function that reports the found <see cref="DocNodeProxy"/> instances to the visitor.</param>
		public virtual void VisitDocumentReferences(DocNodeProxyReporter Report)
		{
		}

		#endregion IPlotItem Members
	} // end of class PlotItem
}