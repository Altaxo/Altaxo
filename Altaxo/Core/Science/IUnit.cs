using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Science
{
	/// <summary>
	/// Represents an arbitrary unit (SI or any other unit).
	/// </summary>
	public interface IUnit
	{
		/// <summary>Full name of the unit.</summary>
		string Name { get; }

		/// <summary>Usual shortcut of the unit.</summary>
		string ShortCut { get; }

/// <summary>
/// Converts x to the corresponding SI unit.
/// </summary>
/// <param name="x">Value to convert.</param>
/// <returns>The corresponding value of x in SI units.</returns>
		double ToSIUnit(double x);

		/// <summary>
		/// Converts x in SI units to the corresponding value in this unit.
		/// </summary>
		/// <param name="x">Value in SI units.</param>
		/// <returns>The corresponding value in this unit.</returns>
		double FromSIUnit(double x);

		/// <summary>
		/// Returns a list of possible prefixes for this unit (like µ, m, k, M, G..).
		/// </summary>
		ISIPrefixList Prefixes { get; }

		/// <summary>
		/// Returns the corresponding SI unit.
		/// </summary>
		SIUnit SIUnit { get; }
	}

	public class UnitLess : SIUnit
	{
		static readonly UnitLess _instance = new UnitLess();
		public static UnitLess Instance { get { return _instance; } }

		private UnitLess() : base(0, 0, 0, 0, 0, 0, 0) { }

		public override string Name
		{
			get { return ""; }
		}

		public override string ShortCut
		{
			get { return ""; }
		}

		public override ISIPrefixList Prefixes
		{
			get { return SIPrefix.ListWithNonePrefixOnly; }
		}
	}






}
