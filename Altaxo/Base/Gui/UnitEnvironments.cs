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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui
{

	public static class RelationEnvironment
	{
		static QuantityWithUnitGuiEnvironment _instance;

		static RelationEnvironment()
		{
			_instance = new QuantityWithUnitGuiEnvironment(GuiRelationUnits.Collection)
			{
				DefaultUnit = new Units.PrefixedUnit(Units.SIPrefix.None, Altaxo.Units.Dimensionless.Unity.Instance)
			};
		}

		/// <summary>
		/// Gets the common position environment for all position boxes.
		/// </summary>
		public static QuantityWithUnitGuiEnvironment Instance
		{
			get
			{
				return _instance;
			}
		}
	}

	public static class PositionEnvironment
	{
		static QuantityWithUnitGuiEnvironment _instance;

		static PositionEnvironment()
		{
			_instance = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection)
			{
				DefaultUnit = new Units.PrefixedUnit(Units.SIPrefix.None, Units.Length.Point.Instance)
			};
		}

		/// <summary>
		/// Gets the common position environment for all position boxes.
		/// </summary>
		public static QuantityWithUnitGuiEnvironment Instance
		{
			get
			{
				return _instance;
			}
		}
	}

	public static class SizeEnvironment
	{
		static QuantityWithUnitGuiEnvironment _instance;

		static SizeEnvironment()
		{
			_instance = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection)
			{
				DefaultUnit = new Units.PrefixedUnit(Units.SIPrefix.None, Units.Length.Point.Instance)
			};
		}

		/// <summary>
		/// Gets the common size environment for all size boxes.
		/// </summary>
		public static QuantityWithUnitGuiEnvironment Instance
		{
			get
			{
				return _instance;
			}
		}
	}


	public static class FontSizeEnvironment
	{
		static QuantityWithUnitGuiEnvironment _instance;

		static FontSizeEnvironment()
		{
			_instance = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection)
			{
				DefaultUnit = new Units.PrefixedUnit(Units.SIPrefix.None, Units.Length.Point.Instance)
			};
		}

		/// <summary>
		/// Gets the common position environment for all position boxes.
		/// </summary>
		public static QuantityWithUnitGuiEnvironment Instance
		{
			get
			{
				return _instance;
			}
		}
	}

	public static class LineCapSizeEnvironment
	{
		static QuantityWithUnitGuiEnvironment _instance;

		static LineCapSizeEnvironment()
		{
			_instance = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection)
			{
				DefaultUnit = new Units.PrefixedUnit(Units.SIPrefix.None, Units.Length.Point.Instance)
			};
		}

		/// <summary>
		/// Gets the common position environment for all position boxes.
		/// </summary>
		public static QuantityWithUnitGuiEnvironment Instance
		{
			get
			{
				return _instance;
			}
		}
	}

	public static class LineThicknessEnvironment
	{
		static QuantityWithUnitGuiEnvironment _instance;

		static LineThicknessEnvironment()
		{
			_instance = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection)
			{
				DefaultUnit = new Units.PrefixedUnit(Units.SIPrefix.None, Units.Length.Point.Instance)
			};
		}

		/// <summary>
		/// Gets the common position environment for all position boxes.
		/// </summary>
		public static QuantityWithUnitGuiEnvironment Instance
		{
			get
			{
				return _instance;
			}
		}
	}

	public static class MiterLimitEnvironment
	{
		static QuantityWithUnitGuiEnvironment _instance;

		static MiterLimitEnvironment()
		{
			_instance = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection)
			{
				DefaultUnit = new Units.PrefixedUnit(Units.SIPrefix.None, Units.Length.Point.Instance)
			};
		}

		/// <summary>
		/// Gets the common position environment for all position boxes.
		/// </summary>
		public static QuantityWithUnitGuiEnvironment Instance
		{
			get
			{
				return _instance;
			}
		}
	}

	public static class PaperMarginEnvironment
	{
		static QuantityWithUnitGuiEnvironment _instance;

		static PaperMarginEnvironment()
		{
			_instance = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection)
			{
				DefaultUnit = new Units.PrefixedUnit(Units.SIPrefix.None, Units.Length.Point.Instance)
			};
		}

		/// <summary>
		/// Gets the common position environment for all position boxes.
		/// </summary>
		public static QuantityWithUnitGuiEnvironment Instance
		{
			get
			{
				return _instance;
			}
		}
	}

}
