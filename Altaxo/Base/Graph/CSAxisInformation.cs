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
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph
{
  [Serializable]
  public class CSAxisInformation : ICloneable
  {
    CSLineID _identifier;

    string _nameOfAxisStyle;
    string _nameOfFirstDownSide;
    string _nameOfFirstUpSide;
    string _nameOfSecondDownSide;
    string _nameOfSecondUpSide;
    CSAxisSide _preferedLabelSide;
    bool _isShownByDefault;

   
    bool _hasTitleByDefault;

   

    public CSAxisInformation(CSLineID identifier)
    {
      _identifier = identifier.Clone();
    }
    public CSAxisInformation(CSAxisInformation from)
    {
      CopyFrom(from);
    }
    public void CopyFrom(CSAxisInformation from)
    {
      this._identifier = from._identifier.Clone();
      CopyWithoutIdentifierFrom(from);
    }
    public void CopyWithoutIdentifierFrom(CSAxisInformation from)
    {
      this._nameOfAxisStyle = from._nameOfAxisStyle;
      this._nameOfFirstDownSide = from._nameOfFirstDownSide;
      this._nameOfFirstUpSide = from._nameOfFirstUpSide;
      this._nameOfSecondDownSide = from._nameOfSecondDownSide;
      this._nameOfSecondUpSide = from._nameOfSecondUpSide;
      this._preferedLabelSide = from._preferedLabelSide;
      this._isShownByDefault = from._isShownByDefault;
      this._hasTitleByDefault = from._hasTitleByDefault;
    }

    public void SetDefaultValues()
    {
      switch (_identifier.ParallelAxisNumber)
      {
        case 0:
          _nameOfAxisStyle = "X-Axis";
          break;
        case 1:
          _nameOfAxisStyle = "Y-Axis";
          break;
        case 2:
          _nameOfAxisStyle = "Z-Axis";
          break;
        default:
          _nameOfAxisStyle = "Axis" + _identifier.ParallelAxisNumber.ToString();
          break;
      }

      _nameOfAxisStyle += string.Format(" (at L={0})",_identifier.LogicalValueOtherFirst.ToString());
      _nameOfFirstDownSide = "FirstDown";
      _nameOfFirstUpSide = "FirstUp";
      _nameOfSecondDownSide = null;
      _nameOfSecondUpSide = null;
      _preferedLabelSide = CSAxisSide.FirstDown;


    }

    public CSAxisInformation Clone()
    {
      return new CSAxisInformation(this);
    }
    object ICloneable.Clone()
    {
      return new CSAxisInformation(this);
    }

    public CSLineID Identifier
    {
      get { return _identifier; }
    }

    /// <summary>
    /// Name of the axis style. For cartesian coordinates for instance left, right, bottom or top.
    /// </summary>
    public string NameOfAxisStyle
    {
      get { return _nameOfAxisStyle; }
      set { _nameOfAxisStyle = value; }
    }

    /// <summary>
    /// Name of the side (in the first alternate direction) of an axis style to lower logical values. For the bottom axis, this is for instance "outer".
    /// </summary>
    public string NameOfFirstDownSide
    {
      get { return _nameOfFirstDownSide; }
      set { _nameOfFirstDownSide = value; }
    }

    /// <summary>
    /// Name of the side (in the first alternate direction) of an axis style to higher logical values. For the bottom axis, this is for instance "inner".
    /// </summary>
    public string NameOfFirstUpSide
    {
      get { return _nameOfFirstUpSide; }
      set { _nameOfFirstUpSide = value; }
    }

    /// <summary>
    /// Name of the side (in the second alternate direction) of an axis style to lower logical values. For the bottom axis, this would be in the direction to the viewer.
    /// </summary>
    public string NameOfSecondDownSide
    {
      get { return _nameOfFirstDownSide; }
      set { _nameOfFirstDownSide = value; }
    }

    /// <summary>
    /// Name of the side (in the second alternate direction) of an axis style to higher logical values. For the bottom axis, this would be in the direction away from the viewer.
    /// </summary>
    public string NameOfSecondUpSide
    {
      get { return _nameOfFirstUpSide; }
      set { _nameOfFirstUpSide = value; }
    }


    /// <summary>
    /// Side of an axis style where the label is probably shown. For the bottom axis, this is for instance the right side, i.e. the outer side.
    /// </summary>
    public CSAxisSide PreferedLabelSide
    {
      get { return _preferedLabelSide; }
      set { _preferedLabelSide = value; }
    }

    public bool IsShownByDefault
    {
      get { return _isShownByDefault; }
      set { _isShownByDefault = value; }
    }

    public bool HasTitleByDefault
    {
      get { return _hasTitleByDefault; }
      set { _hasTitleByDefault = value; }
    }
  }
}
