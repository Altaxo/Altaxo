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
	}

  public class HitTestObject : IHitTestObject
  {
    GraphicsPath _gp;
    object _hitobject;

    #region IHitTestObject Members

    public HitTestObject(GraphicsPath gp, object hitobject)
    {
      _gp = gp;
      _hitobject = hitobject;
    }
    public GraphicsPath SelectionPath
    {
      get { return _gp; }
    }

    public object HittedObject
    {
      get { return _hitobject; }
    }

    public void ShiftPosition(float x, float y)
    {
    
      if(_hitobject is GraphicsObject)
      {
        Matrix mat = new Matrix();
        mat.Translate(x,y);
        _gp.Transform(mat);

        ((GraphicsObject)_hitobject).X += x;
        ((GraphicsObject)_hitobject).Y += y;
      }
    }

    #endregion
  }
}
