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
  /// Handler type to process double click events
  /// </summary>
  public delegate bool DoubleClickHandler(IHitTestObject o);

	/// <summary>
	/// IHitTestObject is used as a return type for hit testing in the graph area.
	/// </summary>
	public interface IHitTestObject
	{
    

    /// <summary>
    /// This will return the selection path for the object.
    /// </summary>
    /// <returns></returns>
    GraphicsPath SelectionPath {get;}


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
    object  HittedObject { get; }

    /// <summary>
    /// Shifts the position of the object according to the x and y values.
    /// </summary>
    /// <param name="x">Shift value in x direction.</param>
    /// <param name="y">Shift value in y direction.</param>
    void ShiftPosition(float x, float y);


    /// <summary>
    /// Delegate to handle double click events.
    /// </summary>
    DoubleClickHandler DoubleClick { get; set; }

    /// <summary>
    /// This function is called if a double click to the object occured.
    /// </summary>
    /// <returns>False normally, true if this hit test object should be deleted from the list (for instance if the object itself was deleted).</returns>
    bool OnDoubleClick();
	}

  public class HitTestObject : IHitTestObject
  {
    GraphicsPath _gp;
    Matrix _matrix;
    Matrix _inversematrix;
    object _hitobject;

    #region IHitTestObject Members

    public HitTestObject(GraphicsPath gp, object hitobject)
    {
      _gp = gp;
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

      _gp.Transform(x);
    }

    public GraphicsPath SelectionPath
    {
      get { return _gp; }
    }

    public object HittedObject
    {
      get { return _hitobject; }
    }

    public virtual void ShiftPosition(float x, float y)
    {
    
      if(_hitobject is GraphicsObject)
      {
        Matrix mat = new Matrix();
        mat.Translate(x,y);
        _gp.Transform(mat);

        PointF[] pos = new PointF[]{new PointF(x,y)};
        _inversematrix.TransformVectors(pos);

        ((GraphicsObject)_hitobject).X += pos[0].X;
        ((GraphicsObject)_hitobject).Y += pos[0].Y;
      }
    }

   
    DoubleClickHandler _DoubleClick;
    public DoubleClickHandler DoubleClick
    {
      get { return _DoubleClick; }
      set { _DoubleClick=value; }
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

    #endregion
  }
}
