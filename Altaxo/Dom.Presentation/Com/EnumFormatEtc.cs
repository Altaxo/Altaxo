using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

using System.Runtime.InteropServices.ComTypes;

namespace Altaxo.Com
{
  /// <summary>
  /// Delegate to the procedure that renders data.
  /// </summary>
  /// <param name="tymed">The type of medium to render the data to.</param>
  /// <returns></returns>
  public delegate IntPtr RenderDataProcedure(TYMED tymed);

  /// <summary>
  /// Bundles a rendering format with the corresponding procedure to render this data.
  /// </summary>
  public struct Rendering
  {
    /// <summary>The rendering format.</summary>
    public FORMATETC format;

    /// <summary>The rendering procedure.</summary>
    public RenderDataProcedure renderer;

    public Rendering(short format, TYMED tymed, RenderDataProcedure renderer)
    {
      this.format = new FORMATETC()
      {
        cfFormat = format,
        ptd = IntPtr.Zero,
        dwAspect = DVASPECT.DVASPECT_CONTENT,
        lindex = -1, // all
        tymed = tymed
      };
      this.renderer = renderer;
    }
  }

  /// <summary>
  /// Helps enumerate the formats available in our DataObject class.
  /// </summary>
  [ComVisible(true)]
  public class EnumFormatEtc : ManagedEnumBase<FORMATETC>, IEnumFORMATETC
  {
    public EnumFormatEtc(IEnumerable<FORMATETC> formats)
      : base(formats)
    {
    }

    public EnumFormatEtc(EnumFormatEtc from)
      : base(from)
    {
    }

    public void Clone(out IEnumFORMATETC newEnum)
    {
      newEnum = new EnumFormatEtc(this);
    }
  }
}
