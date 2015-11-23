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

using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph
{
	[Serializable]
	public class CSAxisInformation : ICloneable
	{
		private CSLineID _identifier;

		private string _nameOfAxisStyle;
		private string _nameOfFirstDownSide;
		private string _nameOfFirstUpSide;
		private string _nameOfSecondDownSide;
		private string _nameOfSecondUpSide;
		private CSAxisSide _preferedLabelSide;
		private bool _isShownByDefault;
		private bool _hasTitleByDefault;
		private bool _hasTicksByDefault = true;
		private bool _hasLabelsByDefault = true;

		/// <summary>This is the logical value where the axis starts. Normally, this is 0 (zero). For a segment of an axis, this might be any value.</summary>
		private double _logicalValueAxisOrg;

		/// <summary>This is the logical value where the axis ends. Normally, this is 1 (one). For a segment of an axis, this might be any value.</summary>
		private double _logicalValueAxisEnd = 1;

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
			if (object.ReferenceEquals(this, from))
				return;

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
			this._logicalValueAxisOrg = from._logicalValueAxisOrg;
			this._logicalValueAxisEnd = from._logicalValueAxisEnd;
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

			// Axis name
			if (_identifier.Is3DIdentifier)
			{
				_nameOfAxisStyle += string.Format(" (at {0}={1}; {2}={3})",
					_identifier.UsePhysicalValueOtherFirst ? "P1" : "L1",
					_identifier.UsePhysicalValueOtherFirst ? _identifier.PhysicalValueOtherFirst.ToString() : _identifier.LogicalValueOtherFirst.ToString(),
					_identifier.UsePhysicalValueOtherSecond ? "P2" : "L2",
					_identifier.UsePhysicalValueOtherSecond ? _identifier.PhysicalValueOtherSecond.ToString() : _identifier.LogicalValueOtherSecond.ToString());
			}
			else
			{
				if (_identifier.UsePhysicalValueOtherFirst)
					_nameOfAxisStyle = string.Format("{0}={1}", _identifier.ParallelAxisNumber == 0 ? "Y" : "X", _identifier.PhysicalValueOtherFirst);
				else
					_nameOfAxisStyle += string.Format(" (at L={0})", _identifier.LogicalValueOtherFirst.ToString());
			}

			// Axis sides
			_nameOfFirstDownSide = "FirstDown";
			_nameOfFirstUpSide = "FirstUp";
			if (_identifier.Is3DIdentifier)
			{
				_nameOfSecondDownSide = "SecondDown";
				_nameOfSecondUpSide = "SecondUp";
			}
			else
			{
				_nameOfSecondDownSide = null;
				_nameOfSecondUpSide = null;
			}

			// preferred label side
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

		public bool HasTicksByDefault
		{
			get { return _hasTicksByDefault; }
			set { _hasTicksByDefault = value; }
		}

		public bool HasLabelsByDefault
		{
			get { return _hasLabelsByDefault; }
			set { _hasLabelsByDefault = value; }
		}

		/// <summary>This is the logical value where the axis starts. Normally, this is 0 (zero). For a segment of an axis, this might be any value.</summary>
		public double LogicalValueAxisOrg
		{
			get
			{
				return _logicalValueAxisOrg;
			}
			set
			{
				_logicalValueAxisOrg = value;
			}
		}

		/// <summary>This is the logical value where the axis ends. Normally, this is 1 (one). For a segment of an axis, this might be be any value.</summary>
		public double LogicalValueAxisEnd
		{
			get
			{
				return _logicalValueAxisEnd;
			}
			set
			{
				_logicalValueAxisEnd = value;
			}
		}
	}
}