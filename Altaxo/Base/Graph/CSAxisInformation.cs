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
	public class CSAxisInformation : Main.IImmutable
	{
		private CSLineID _identifier;

		private string _nameOfAxisStyle;
		private string _nameOfFirstDownSide;
		private string _nameOfFirstUpSide;
		private string _nameOfSecondDownSide;
		private string _nameOfSecondUpSide;
		private CSAxisSide _preferedLabelSide;
		private CSAxisSide _preferedTickSide;
		private bool _isShownByDefault;
		private bool _hasTitleByDefault;
		private bool _hasTicksByDefault = true;
		private bool _hasLabelsByDefault = true;

		/// <summary>This is the logical value where the axis starts. Normally, this is 0 (zero). For a segment of an axis, this might be any value.</summary>
		private double _logicalValueAxisOrg;

		/// <summary>This is the logical value where the axis ends. Normally, this is 1 (one). For a segment of an axis, this might be any value.</summary>
		private double _logicalValueAxisEnd = 1;

		/// <summary>
		/// Initializes a new instance of the <see cref="CSAxisInformation"/> class, with only the identifier, but no other information.
		/// </summary>
		/// <param name="identifier">The identifier.</param>
		public CSAxisInformation(CSLineID identifier)
		{
			_identifier = identifier;
		}

		public CSAxisInformation(
		CSLineID Identifier,
		string NameOfAxisStyle,
		string NameOfFirstDownSide,
		string NameOfFirstUpSide,
		CSAxisSide PreferredLabelSide
		)
		{
			_identifier = Identifier;
			_nameOfAxisStyle = NameOfAxisStyle;
			_nameOfFirstDownSide = NameOfFirstDownSide;
			_nameOfFirstUpSide = NameOfFirstUpSide;
			_preferedLabelSide = PreferredLabelSide;
		}

		public CSAxisInformation(
			CSLineID Identifier,
			string NameOfAxisStyle,
			string NameOfFirstDownSide,
			string NameOfFirstUpSide,
			CSAxisSide PreferredLabelSide,
			bool IsShownByDefault,
			bool HasTitleByDefault
			)
		{
			_identifier = Identifier;
			_nameOfAxisStyle = NameOfAxisStyle;
			_nameOfFirstDownSide = NameOfFirstDownSide;
			_nameOfFirstUpSide = NameOfFirstUpSide;
			_preferedLabelSide = PreferredLabelSide;
			_isShownByDefault = IsShownByDefault;
			_hasTitleByDefault = HasTitleByDefault;
		}

		public CSAxisInformation(
			CSLineID Identifier,
			string NameOfAxisStyle,
			string NameOfFirstDownSide,
			string NameOfFirstUpSide,
			string NameOfSecondDownSide,
			string NameOfSecondUpSide,
			CSAxisSide PreferredLabelSide,
			CSAxisSide PreferredTickSide,
			bool IsShownByDefault,
			bool HasTicksByDefault,
			bool HasLabelsByDefault,
			bool HasTitleByDefault
			)
		{
			_identifier = Identifier;
			_nameOfAxisStyle = NameOfAxisStyle;
			_nameOfFirstDownSide = NameOfFirstDownSide;
			_nameOfFirstUpSide = NameOfFirstUpSide;
			_nameOfSecondDownSide = NameOfSecondDownSide;
			_nameOfSecondUpSide = NameOfSecondUpSide;
			_preferedLabelSide = PreferredLabelSide;
			_preferedTickSide = PreferredTickSide;
			_isShownByDefault = IsShownByDefault;
			_hasTicksByDefault = HasTicksByDefault;
			_hasLabelsByDefault = HasLabelsByDefault;
			_hasTitleByDefault = HasTitleByDefault;
		}

		public CSAxisInformation WithIdentifier(CSLineID identifier)
		{
			if (_identifier == identifier)
			{
				return this;
			}
			else
			{
				if (null == identifier)
					throw new ArgumentNullException(nameof(identifier));

				var result = (CSAxisInformation)this.MemberwiseClone();
				result._identifier = identifier;
				return result;
			}
		}

		/// <summary>
		/// This is a private constructor that sets not only the identifier, but also default values. The second argument is ignored, but is essential
		/// to distinguish this contructor from the constructor that sets no default values.
		/// </summary>
		/// <param name="identifier">The identifier.</param>
		/// <param name="idIgnored">Ignored argument</param>
		private CSAxisInformation(CSLineID identifier, CSLineID idIgnored)
		{
			_identifier = identifier;
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

		public static CSAxisInformation NewWithDefaultValues(CSLineID identifier)
		{
			if (null == identifier)
				throw new ArgumentNullException(nameof(identifier));

			return new CSAxisInformation(identifier, identifier);
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
		}

		public CSAxisInformation WithNameOfAxisStyle(string nameOfAxisStyle)
		{
			if (_nameOfAxisStyle == nameOfAxisStyle)
			{
				return this;
			}
			else
			{
				var result = (CSAxisInformation)MemberwiseClone();
				result._nameOfAxisStyle = nameOfAxisStyle;
				return result;
			}
		}

		/// <summary>
		/// Name of the side (in the first alternate direction) of an axis style to lower logical values. For the bottom axis, this is for instance "outer".
		/// </summary>
		public string NameOfFirstDownSide
		{
			get { return _nameOfFirstDownSide; }
		}

		/// <summary>
		/// Name of the side (in the first alternate direction) of an axis style to higher logical values. For the bottom axis, this is for instance "inner".
		/// </summary>
		public string NameOfFirstUpSide
		{
			get { return _nameOfFirstUpSide; }
		}

		public CSAxisInformation WithNamesForFirstUpAndDownSides(string NameOfFirstUpSide, string NameOfFirstDownSide)
		{
			if (_nameOfFirstUpSide == NameOfFirstUpSide && _nameOfFirstDownSide == NameOfFirstDownSide)
			{
				return this;
			}
			else
			{
				var result = (CSAxisInformation)MemberwiseClone();
				result._nameOfFirstUpSide = NameOfFirstUpSide;
				result._nameOfFirstDownSide = NameOfFirstDownSide;
				return result;
			}
		}

		/// <summary>
		/// Name of the side (in the second alternate direction) of an axis style to lower logical values. For the bottom axis, this would be in the direction to the viewer.
		/// </summary>
		public string NameOfSecondDownSide
		{
			get { return _nameOfFirstDownSide; }
		}

		/// <summary>
		/// Name of the side (in the second alternate direction) of an axis style to higher logical values. For the bottom axis, this would be in the direction away from the viewer.
		/// </summary>
		public string NameOfSecondUpSide
		{
			get { return _nameOfFirstUpSide; }
		}

		public CSAxisInformation WithNamesForSecondUpAndDownSides(string NameOfSecondUpSide, string NameOfSecondDownSide)
		{
			if (_nameOfSecondUpSide == NameOfSecondUpSide && _nameOfSecondDownSide == NameOfSecondDownSide)
			{
				return this;
			}
			else
			{
				var result = (CSAxisInformation)MemberwiseClone();
				result._nameOfSecondUpSide = NameOfSecondUpSide;
				result._nameOfSecondDownSide = NameOfSecondDownSide;
				return result;
			}
		}

		/// <summary>
		/// Side of an axis style where the label is probably shown. For the bottom axis, this is for instance the right side, i.e. the outer side.
		/// </summary>
		public CSAxisSide PreferredLabelSide
		{
			get { return _preferedLabelSide; }
		}

		/// <summary>
		/// Side of an axis style where the ticks are probably shown. For the bottom axis, this is for instance the right side, i.e. the outer side.
		/// </summary>
		public CSAxisSide PreferredTickSide
		{
			get { return _preferedTickSide; }
		}

		public bool IsShownByDefault
		{
			get { return _isShownByDefault; }
		}

		public bool HasTitleByDefault
		{
			get { return _hasTitleByDefault; }
		}

		public bool HasTicksByDefault
		{
			get { return _hasTicksByDefault; }
		}

		public bool HasLabelsByDefault
		{
			get { return _hasLabelsByDefault; }
		}

		/// <summary>This is the logical value where the axis starts. Normally, this is 0 (zero). For a segment of an axis, this might be any value.</summary>
		public double LogicalValueAxisOrg
		{
			get
			{
				return _logicalValueAxisOrg;
			}
		}

		/// <summary>This is the logical value where the axis ends. Normally, this is 1 (one). For a segment of an axis, this might be be any value.</summary>
		public double LogicalValueAxisEnd
		{
			get
			{
				return _logicalValueAxisEnd;
			}
		}

		public CSAxisInformation WithLogicalValuesForAxisOrgAndEnd(double LogicalValueAxisOrg, double LogicalValueAxisEnd)
		{
			if (_logicalValueAxisOrg == LogicalValueAxisOrg && _logicalValueAxisEnd == LogicalValueAxisEnd)
			{
				return this;
			}
			else
			{
				var result = (CSAxisInformation)MemberwiseClone();
				result._logicalValueAxisOrg = LogicalValueAxisOrg;
				result._logicalValueAxisEnd = LogicalValueAxisEnd;
				return result;
			}
		}
	}
}