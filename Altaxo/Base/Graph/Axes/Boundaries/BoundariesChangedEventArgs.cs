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
using Altaxo.Serialization;

namespace Altaxo.Graph.Axes.Boundaries
{
  public class BoundariesChangedEventArgs : System.EventArgs
  {
    protected bool m_bLowerBoundChanged, m_bUpperBoundChanged;

    public BoundariesChangedEventArgs(bool bLowerBound, bool bUpperBound)
    {
      this.m_bLowerBoundChanged = bLowerBound;
      this.m_bUpperBoundChanged = bUpperBound;
    }

    public bool LowerBoundChanged 
    {
      get { return this.m_bLowerBoundChanged; }
    }

    public bool UpperBoundChanged 
    {
      get { return this.m_bUpperBoundChanged; }
    }
  }

}
