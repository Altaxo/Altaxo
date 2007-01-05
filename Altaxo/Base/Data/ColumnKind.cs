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
using Altaxo.Serialization;

namespace Altaxo.Data
{
  

  /// <summary>
  /// The main purpose of the column.
  /// </summary>
  [Serializable]
  public enum ColumnKind
  {
    /// <summary>
    /// Column values are the dependent variable (usually y in 2D-Plots, z in 3D-plots) 
    /// </summary>
    V=0,
    /// <summary>
    /// Column values are the first independent variable.
    /// </summary>
    X=1,
    /// <summary>
    /// Column values are the second independent variable.
    /// </summary>
    Y=2,      
    /// <summary>
    /// Column values are the third independent variable.
    /// </summary>
    Z=3,                
    /// <summary>
    /// Column values are +- error values.
    /// </summary>
    Err=4,    
    /// <summary>
    /// Column values are + error values.
    /// </summary>
    pErr=5,
    /// <summary>
    /// Column values are - error values.
    /// </summary>
    mErr=6,
    /// <summary>
    /// Column values are labels.
    /// </summary>
    Label=7,
    /// <summary>
    /// Column values are the plot condition, i.e. if zero, the row is ignored during plotting.
    /// </summary>
    Condition=8
  }


  public class ColumnKindChangeEventArgs : System.EventArgs
  {
    ColumnKind _oldKind;
    ColumnKind _newKind;
    
    public ColumnKindChangeEventArgs(ColumnKind oldKind, ColumnKind newKind)
    {
      _oldKind = oldKind;
      _newKind = newKind;
    }

    public ColumnKind OldKind { get { return _oldKind; }}
    public ColumnKind NewKind { get { return _newKind; }}
    

  }


}
