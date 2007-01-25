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
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Serialization;


namespace Altaxo.Graph.Gdi
{
  using Shapes;

  /// <summary>
  /// Handler type to process double click events
  /// </summary>
  public delegate bool DoubleClickHandler(IHitTestObject o);

  /// <summary>
  /// IHitTestObject is used as a return type for hit testing in the graph area.
  /// </summary>
  public interface IHitTestObject
  {
    

    /// <summary>
    /// This will return the selection path for the object. This is a closed
    /// path where when hit into with the mouse, the object is selected.
    /// </summary>
    /// <returns>Selection path.</returns>
    GraphicsPath SelectionPath {get;}


    /// <summary>
    /// This will return the object path for the object. This is a closed
    /// path which fully encloses the object. In case of lines, the width of this path is equal to the line width.
    /// </summary>
    /// <returns>Selection path.</returns>
    GraphicsPath ObjectPath { get; }

    /// <summary>
    /// This will return the transformation matrix.
    /// </summary>
    Matrix Transformation {get;}

    /// <summary>
    /// Transform the internal positions according to the provided transformation matrix.
    /// </summary>
    /// <param name="x"></param>
    void Transform(Matrix x);

    /// <summary>
    /// This will return the object itself, i.e. the object which corresponds to the selection path.
    /// </summary>
    /// <returns></returns>
    object  HittedObject { get; set;}

    XYPlotLayer ParentLayer { get; set; }

    /// <summary>
    /// Shifts the position of the object according to the x and y values.
    /// </summary>
    /// <param name="x">Shift value in x direction.</param>
    /// <param name="y">Shift value in y direction.</param>
    void ShiftPosition(float x, float y);


    /// <summary>
    /// Delegate to handle double click events. Should return true if the object was removed during the processing. Otherwise returns false.
    /// </summary>
    DoubleClickHandler DoubleClick { get; set; }

    /// <summary>
    /// Handler to remove the hitted object. Should return true if the object is removed, otherwise false.
    /// </summary>
    DoubleClickHandler Remove { get; set; }

    /// <summary>
    /// This function is called if a double click to the object occured.
    /// </summary>
    /// <returns>False normally, true if this hit test object should be deleted from the list (for instance if the object itself was deleted).</returns>
    bool OnDoubleClick();
  }

  public class HitTestObject : IHitTestObject
  {
    GraphicsPath _objectPath;
    GraphicsPath _selectionPath; // can be null, in this case the object path is used

    Matrix _matrix;
    Matrix _inversematrix;
    object _hitobject;

    #region IHitTestObject Members

    public HitTestObject(GraphicsPath gp, object hitobject)
      : this(gp,null,hitobject)
    {
    }

    public HitTestObject(GraphicsPath gp, GraphicsPath selectionPath, object hitobject)
    {
      _objectPath = gp;
      _selectionPath = selectionPath;
      _hitobject = hitobject;
      _matrix = new Matrix();
      _inversematrix = new Matrix();
    }


    public Matrix Transformation
    {
      get { return _matrix; }
    }

    public void Transform(Matrix x)
    {
      _matrix.Multiply(x);
      _inversematrix = (Matrix)_matrix.Clone();
      _inversematrix.Invert();

      _objectPath.Transform(x);
      if (_selectionPath != null) _selectionPath.Transform(x);
    }

    public GraphicsPath SelectionPath
    {
      get 
      {
        return _selectionPath!=null ? _selectionPath : _objectPath; 
      }
    }

    public GraphicsPath ObjectPath
    {
      get
      {
        return _objectPath;
      }
    }


    public object HittedObject
    {
      get { return _hitobject; }
      set { _hitobject = value; }
    }

    public virtual void ShiftPosition(float x, float y)
    {
    
      if(_hitobject is GraphicBase)
      {
        Matrix mat = new Matrix();
        mat.Translate(x,y);
        _objectPath.Transform(mat);
        if (null != _selectionPath) _selectionPath.Transform(mat);

        PointF[] pos = new PointF[]{new PointF(x,y)};
        _inversematrix.TransformVectors(pos);

        ((GraphicBase)_hitobject).X += pos[0].X;
        ((GraphicBase)_hitobject).Y += pos[0].Y;
      }
    }

   
    DoubleClickHandler _DoubleClick;
    public DoubleClickHandler DoubleClick
    {
      get { return _DoubleClick; }
      set { _DoubleClick=value; }
    }

    /// <summary>
    /// Handler to remove the hitted object. Should return true if the object is removed, otherwise false.
    /// </summary>
    DoubleClickHandler _Remove;
    /// <summary>
    /// Handler to remove the hitted object. Should return true if the object is removed, otherwise false.
    /// </summary>
    public DoubleClickHandler Remove
    {
      get { return _Remove; }
      set { _Remove=value; }
    }
    /// <summary>
    /// This handles the double-click event
    /// </summary>
    /// <returns>False normally, true if the HitTestObject should be removed from the list of selected objects (i.e. because the object was deleted).</returns>
    public virtual bool OnDoubleClick()
    {
      if(DoubleClick!=null)
        return DoubleClick(this);
      else
        return false;
    }

    XYPlotLayer _parentLayer;
    public XYPlotLayer ParentLayer
    {
      get { return _parentLayer; }
      set { _parentLayer = value; }
    }

    #endregion
  }
}
