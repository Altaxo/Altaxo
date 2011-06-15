using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui
{
	public static class PositionEnvironment
	{
		static QuantityWithUnitGuiEnvironment _instance;

		static PositionEnvironment()
		{
			_instance = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection)
			{
				DefaultUnit = new Science.PrefixedUnit(Science.SIPrefix.None, Science.LengthUnitPoint.Instance)
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

	public static class FontSizeEnvironment
	{
		static QuantityWithUnitGuiEnvironment _instance;

		static FontSizeEnvironment()
		{
			_instance = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection)
			{
				DefaultUnit = new Science.PrefixedUnit(Science.SIPrefix.None, Science.LengthUnitPoint.Instance)
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
				DefaultUnit = new Science.PrefixedUnit(Science.SIPrefix.None, Science.LengthUnitPoint.Instance)
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
				DefaultUnit = new Science.PrefixedUnit(Science.SIPrefix.None, Science.LengthUnitPoint.Instance)
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
				DefaultUnit = new Science.PrefixedUnit(Science.SIPrefix.None, Science.LengthUnitPoint.Instance)
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
				DefaultUnit = new Science.PrefixedUnit(Science.SIPrefix.None, Science.LengthUnitPoint.Instance)
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
