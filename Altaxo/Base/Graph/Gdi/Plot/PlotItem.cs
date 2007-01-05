#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
using System.Text;
using System.Drawing;

namespace Altaxo.Graph.Gdi.Plot
{
  using Groups;
  using Graph.Plot.Groups;

  [Serializable]
  public abstract class PlotItem
  :
    IGPlotItem,
    Main.IChangedEventSource,
    System.ICloneable,
    Main.IDocumentNode,
    Main.INamedObjectCollection
  {

    /// <summary>
    /// The parent object.
    /// </summary>
    [NonSerialized]
    protected object _parent;

    protected virtual void CopyFrom(PlotItem from)
    {
      this._parent = from._parent;
    }

    /// <summary>
    /// Get/sets the style object of this plot.
    /// </summary>
    public abstract object StyleObject { get; set; }


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
    public abstract void Paint(Graphics g, IPlotArea layer);

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


    public virtual object ParentObject
    {
      get { return _parent; }
      set { _parent = value; }
    }

    public virtual PlotItemCollection ParentCollection
    {
      get
      {
        return _parent as PlotItemCollection;
      }
    }

    public virtual string Name
    {
      get
      {
        Main.INamedObjectCollection noc = ParentObject as Main.INamedObjectCollection;
        return noc == null ? null : noc.GetNameOfChildObject(this);
      }
    }

    /// <summary>
    /// Fired if the data object changed or something inside the data object changed
    /// </summary>
    [field:NonSerialized]
    public event System.EventHandler DataChanged;

    /// <summary>
    /// Fired if the style object changed or something inside the style object changed
    /// </summary>
    [field:NonSerialized]
    public event System.EventHandler StyleChanged;

    /// <summary>
    /// Fired if either data or style has changed.
    /// </summary>
    [field:NonSerialized]
    public event System.EventHandler Changed;

    /// <summary>
    /// Intended to used by derived classes, fires the DataChanged event and the Changed event
    /// </summary>
    public virtual void OnDataChanged()
    {
      if (null != DataChanged)
        DataChanged(this, new System.EventArgs());

      OnChanged();
    }

    /// <summary>
    /// Intended to used by derived classes, fires the StyleChanged event and the Changed event
    /// </summary>
    public virtual void OnStyleChanged()
    {
      if (null != StyleChanged)
        StyleChanged(this, new System.EventArgs());

      OnChanged();
    }

    /// <summary>
    /// Intended to used by derived classes, fires the Changed event
    /// </summary>
    public virtual void OnChanged()
    {
      if (null != Changed)
        Changed(this, new System.EventArgs());


    }


    /// <summary>
    /// Intended to use by derived classes, serves as event sink for the Changed event from the Data object and fires the DataChanged event.
    /// </summary>
    /// <param name="sender">The sender of the event (the Data object).</param>
    /// <param name="e">EventArgs (not used).</param>
    public virtual void OnDataChangedEventHandler(object sender, System.EventArgs e)
    {
      OnDataChanged();
    }

    /// <summary>
    /// Intended to use by derived classes, serves as event sink for the Changed event from the Style object and fires the StyleChanged event.
    /// </summary>
    /// <param name="sender">The sender of the event (the Style object).</param>
    /// <param name="e">EventArgs (not used).</param>
    public virtual void OnStyleChangedEventHandler(object sender, System.EventArgs e)
    {
      OnStyleChanged();
    }

    /// <summary>
    /// retrieves the object with the name <code>name</code>.
    /// </summary>
    /// <param name="name">The objects name.</param>
    /// <returns>The object with the specified name.</returns>
    public virtual object GetChildObjectNamed(string name)
    {
      return null;
    }

    /// <summary>
    /// Retrieves the name of the provided object.
    /// </summary>
    /// <param name="o">The object for which the name should be found.</param>
    /// <returns>The name of the object. Null if the object is not found. String.Empty if the object is found but has no name.</returns>
    public virtual string GetNameOfChildObject(object o)
    {
      if (o == null)
        return null;

      else
        return null;
    }

    /// <summary>
    /// Test wether the mouse hits a plot item. The default implementation here returns null.
    /// If you want to have a reaction on mouse click on a curve, implement this function.
    /// </summary>
    /// <param name="layer">The layer in which this plot item is drawn into.</param>
    /// <param name="hitpoint">The point where the mouse is pressed.</param>
    /// <returns>Null if no hit, or a <see cref="IHitTestObject" /> if there was a hit.</returns>
    public virtual IHitTestObject HitTest(IPlotArea layer, PointF hitpoint)
    {
      return null;
    }


    #region IPlotItem Members

    public abstract void CollectStyles(PlotGroupStyleCollection styles);

    public abstract void PrepareStyles(PlotGroupStyleCollection externalGroups, IPlotArea layer);

    public abstract void ApplyStyles(PlotGroupStyleCollection externalGroups);

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

    #endregion

  } // end of class PlotItem
}
