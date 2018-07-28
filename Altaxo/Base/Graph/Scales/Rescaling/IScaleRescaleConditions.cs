#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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

using Altaxo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Scales.Rescaling
{
  /// <summary>
  /// Interface that has to be implemented by all rescaling condition classes.
  /// </summary>
  public interface IScaleRescaleConditions
      :
    Main.IDocumentLeafNode,
    Main.ICopyFrom
  {
    //AltaxoVariant ResultingOrgAsVariant { get; }
    //AltaxoVariant ResultingEndAsVariant { get; }

    //bool IsResultingOrgFixed { get; }
    //bool IsResultingEndFixed { get; }
  }

  public interface IUnboundNumericScaleRescaleConditions
      :
    IScaleRescaleConditions
  {
    void SetUserParameters(BoundaryRescaling orgRescaling, BoundariesRelativeTo orgRelativeTo, AltaxoVariant orgValue, BoundaryRescaling endRescaling, BoundariesRelativeTo endRelativeTo, AltaxoVariant endValue);
  }
}
