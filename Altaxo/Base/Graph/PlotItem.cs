#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;


namespace Altaxo.Graph
{

  /// <summary>
  /// PlotItem holds the pair of data and style neccessary to plot a curve, function,
  /// surface and so on.  
  /// </summary>
  public abstract class  PlotItem
    :
    Main.IChangedEventSource,
    System.ICloneable,
    Main.IDocumentNode,
    Main.INamedObjectCollection
  {

    /// <summary>
    /// The parent object.
    /// </summary>
    protected object m_Parent;

    /// <summary>
    /// Get/sets the data object of this plot.
    /// </summary>
    public abstract object Data { get; set; }
    /// <summary>
    /// Get/sets the style object of this plot.
    /// </summary>
    public abstract object Style { get; set; }
    /// <summary>
    /// The name of the plot. It can be of different length. An argument of zero or less
    /// returns the shortest possible name, higher values return more verbose names.
    /// </summary>
    /// <param name="level">The naming level, 0 returns the shortest possible name, 1 or more returns more
    /// verbose names.</param>
    /// <returns>The name of the plot.</returns>
    public abstract string GetName(int level);

    /// <summary>
    /// This paints the plot to the layer.
    /// </summary>
    /// <param name="g">The graphics context.</param>
    /// <param name="layer">The plot layer.</param>
    public abstract void Paint(Graphics g, Graph.XYPlotLayer layer);


    /// <summary>
    /// Creates a cloned copy of this object.
    /// </summary>
    /// <returns>The cloned copy of this object.</returns>
    /// <remarks>The data (DataColumns which belongs to a table in the document's DataTableCollection) are not cloned, only the reference to this columns is cloned.</remarks>
    public abstract object Clone();


    public virtual object ParentObject
    {
      get { return m_Parent; }
      set { m_Parent = value; }
    }

    public virtual string Name
    {
      get
      {
        Main.INamedObjectCollection noc = ParentObject as Main.INamedObjectCollection;
        return noc==null ? null : noc.GetNameOfChildObject(this);
      }
    }

    /// <summary>
    /// Fired if the data object changed or something inside the data object changed
    /// </summary>
    public event System.EventHandler DataChanged;

    /// <summary>
    /// Fired if the style object changed or something inside the style object changed
    /// </summary>
    public event System.EventHandler StyleChanged;

    /// <summary>
    /// Fired if either data or style has changed.
    /// </summary>
    public event System.EventHandler Changed;

    /// <summary>
    /// Intended to used by derived classes, fires the DataChanged event and the Changed event
    /// </summary>
    public virtual void OnDataChanged()
    {
      if(null!=DataChanged)
        DataChanged(this,new System.EventArgs());

      OnChanged();
    }

    /// <summary>
    /// Intended to used by derived classes, fires the StyleChanged event and the Changed event
    /// </summary>
    public virtual void OnStyleChanged()
    {
      if(null!=StyleChanged)
        StyleChanged(this,new System.EventArgs());
      
      OnChanged();}

    /// <summary>
    /// Intended to used by derived classes, fires the Changed event
    /// </summary>
    public virtual void OnChanged()
    {
      if(null!=Changed)
        Changed(this,new System.EventArgs());
    
    
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
      switch(name)
      {
        case "Data":
          return this.Data;
        case "Style":
          return this.Style;
      }
      return null;
    }

    /// <summary>
    /// Retrieves the name of the provided object.
    /// </summary>
    /// <param name="o">The object for which the name should be found.</param>
    /// <returns>The name of the object. Null if the object is not found. String.Empty if the object is found but has no name.</returns>
    public virtual string GetNameOfChildObject(object o)
    {
      if(o==null)
        return null;
      else if(o.Equals(this.Data))
        return "Data";
      else if(o.Equals(this.Style))
        return "Style";
      else
        return null;
    }

    /// <summary>
    /// Test wether the mouse hits a plot item. The default implementation here returns null.
    /// If you want to have a reaction on mouse click on a curve, implement this function.
    /// </summary>
    /// <param name="layer">The layer in which this plot item is drawn into.</param>
    /// <param name="hitpoint">The point where the mouse is pressed.</param>
    /// <returns>Null if no hit, or a <see>IHitTestObject</see> if there was a hit.</returns>
    public virtual IHitTestObject HitTest(XYPlotLayer layer, PointF hitpoint)
    {
      return null;
    }

  } // end of class PlotItem
}
