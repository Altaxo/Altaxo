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

#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Drawing;

namespace Altaxo.Graph.Gdi.LineCaps
{
  /// <summary>
  /// Base class for Gdi+ rendering implementations of <see cref="ILineCap"/> types.
  /// </summary>
  public abstract class GdiLineCapBase
  {
    /// <summary>
    /// Gets the type of line cap from the <see cref="Altaxo.Drawing.LineCaps"/> namespace that this class is rendering.
    /// </summary>
    /// <value>
    /// The type of the extends.
    /// </value>
    public abstract Type ExtendsType { get; }

    /// <summary>
    /// Creates the custom GDI line cap used for rendering.
    /// </summary>
    /// <param name="pen">The pen that will use the cap.</param>
    /// <param name="size">The requested cap size.</param>
    /// <param name="isEndCap"><see langword="true"/> to create an end cap; otherwise, a start cap.</param>
    /// <returns>The custom line cap instance.</returns>
    protected abstract CustomLineCap GetCustomLineCap(Pen pen, float size, bool isEndCap);

    /// <summary>
    /// Sets the start cap on the specified pen.
    /// </summary>
    /// <param name="pen">The pen to modify.</param>
    /// <param name="size">The cap size.</param>
    public virtual void SetStartCap(Pen pen, float size)
    {
      pen.StartCap = LineCap.Custom;
      pen.CustomStartCap = GetCustomLineCap(pen, size, false);
    }

    /// <summary>
    /// Sets the end cap on the specified pen.
    /// </summary>
    /// <param name="pen">The pen to modify.</param>
    /// <param name="size">The cap size.</param>
    public virtual void SetEndCap(Pen pen, float size)
    {
      pen.EndCap = LineCap.Custom;
      pen.CustomEndCap = GetCustomLineCap(pen, size, true);
    }


    #region Cap styles registry

    /// <summary>
    /// Sets the start cap on the specified pen using the registered GDI implementation.
    /// </summary>
    /// <param name="pen">The pen to modify.</param>
    /// <param name="cap">The logical line cap.</param>
    public static void SetStartCap(Pen pen, ILineCap cap)
    {
      if (cap is null || !_registeredStyles.TryGetValue(cap.GetType(), out var implementation))
      {
        pen.StartCap = LineCap.Flat;
      }
      else
      {
        implementation.SetStartCap(pen, (float)Math.Max(cap.MinimumAbsoluteSizePt, pen.Width * cap.MinimumRelativeSize));
      }
    }

    /// <summary>
    /// Sets the end cap on the specified pen using the registered GDI implementation.
    /// </summary>
    /// <param name="pen">The pen to modify.</param>
    /// <param name="cap">The logical line cap.</param>
    public static void SetEndCap(Pen pen, ILineCap cap)
    {
      if (cap is null || !_registeredStyles.TryGetValue(cap.GetType(), out var implementation))
      {
        pen.StartCap = LineCap.Flat;
      }
      else
      {
        implementation.SetEndCap(pen, (float)Math.Max(cap.MinimumAbsoluteSizePt, pen.Width * cap.MinimumRelativeSize));
      }
    }


    private static Dictionary<Type, GdiLineCapBase> _registeredStyles;



    static GdiLineCapBase()
    {
      // now the other linecaps
      _registeredStyles = new Dictionary<Type, GdiLineCapBase>();

      var types = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(GdiLineCapBase));
      foreach (var t in types)
      {
        var ex = (GdiLineCapBase)(Activator.CreateInstance(t) ?? throw new InvalidProgramException($"Unable to create type {t} with parameterless constructor"));
        _registeredStyles.Add(ex.ExtendsType, ex);
      }
    }

    #endregion Cap styles registry
  }
}
