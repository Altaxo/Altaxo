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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Altaxo.Graph;
using Altaxo.Serialization;

namespace Altaxo.Graph.GUI.GraphControllerMouseHandlers
{
  #region abstract mouse state handler
  /// <summary>
  /// The abstract base class of all MouseStateHandlers.
  /// </summary>
  /// <remarks>The mouse state handler are used to handle the mouse events of the graph view in different contexts,
  /// depending on which GraphTool is choosen by the user.</remarks>
  public abstract class MouseStateHandler
  {
    /// <summary>Stores the mouse position of the last mouse up event.</summary>
    protected PointF m_LastMouseUp;
    /// <summary>Stores the mouse position of the last mouse down event.</summary>
    protected PointF m_LastMouseDown;

    /// <summary>
    /// Handles the mouse move event.
    /// </summary>
    /// <param name="sender">The GraphController that sends this event.</param>
    /// <param name="e">MouseEventArgs as provided by the view.</param>
    /// <returns>The next mouse state handler that should handle mouse events.</returns>
    public virtual MouseStateHandler OnMouseMove(GraphController sender, System.Windows.Forms.MouseEventArgs e)
    {
      return this;
    }

    /// <summary>
    /// Handles the mouse up event. Stores the position of the mouse into <see cref="m_LastMouseUp"/>.
    /// </summary>
    /// <param name="sender">The GraphController that sends this event.</param>
    /// <param name="e">MouseEventArgs as provided by the view.</param>
    /// <returns>The next mouse state handler that should handle mouse events.</returns>
    public virtual MouseStateHandler OnMouseUp(GraphController sender, System.Windows.Forms.MouseEventArgs e)
    {
      m_LastMouseUp = new Point(e.X,e.Y);
      return this;
    }

    /// <summary>
    /// Handles the mouse down event. Stores the position of the mouse into <see cref="m_LastMouseDown"/>.
    /// </summary>
    /// <param name="sender">The GraphController that sends this event.</param>
    /// <param name="e">MouseEventArgs as provided by the view.</param>
    /// <returns>The next mouse state handler that should handle mouse events.</returns>
    public virtual MouseStateHandler OnMouseDown(GraphController sender, System.Windows.Forms.MouseEventArgs e)
    {
      m_LastMouseDown = new Point(e.X,e.Y);
      return this;
    }
      
    /// <summary>
    /// Handles the mouse click event.
    /// </summary>
    /// <param name="sender">The GraphController that sends this event.</param>
    /// <param name="e">EventArgs as provided by the view.</param>
    /// <returns>The next mouse state handler that should handle mouse events.</returns>
    public virtual MouseStateHandler OnClick(GraphController sender, System.EventArgs e)
    {
      return this;
    }
      
    /// <summary>
    /// Handles the mouse doubleclick event.
    /// </summary>
    /// <param name="sender">The GraphController that sends this event.</param>
    /// <param name="e">EventArgs as provided by the view.</param>
    /// <returns>The next mouse state handler that should handle mouse events.</returns>
    public virtual MouseStateHandler OnDoubleClick(GraphController sender, System.EventArgs e)
    {
      return this;
    }

    /// <summary>
    /// This function is called just after the paint event.
    /// </summary>
    /// <param name="grac"></param>
    /// <param name="g"></param>
    public virtual void AfterPaint(GraphController grac, Graphics g)
    {
    }

    /// <summary>
    /// This function is called if a key is pressed.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="keyData"></param>
    /// <returns></returns>
    public virtual bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      return false; // per default the key is not processed
    }
  }
  #endregion // abstract mouse state handler

}
