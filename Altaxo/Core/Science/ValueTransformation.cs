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
using System.Linq;
using System.Text;

namespace Altaxo.Science
{
	public interface IDoubleToDoubleValueTransformation
	{
		double Transform(double value);

		string StringRepresentation { get; }

		IDoubleToDoubleValueTransformation BackTransformation { get; }
	}

	public class InverseTransformation : IDoubleToDoubleValueTransformation
	{
		public double Transform(double value)
		{
			return 1 / value;
		}

		public string StringRepresentation
		{
			get { return "1/x"; }
		}

		public IDoubleToDoubleValueTransformation BackTransformation
		{
			get { return this; }
		}
	}

	public class NegateTransformation : IDoubleToDoubleValueTransformation
	{
		public double Transform(double value)
		{
			return -value;
		}

		public string StringRepresentation
		{
			get { return "-x"; }
		}

		public IDoubleToDoubleValueTransformation BackTransformation
		{
			get { return this; }
		}
	}

	public class OffsetTransformation : IDoubleToDoubleValueTransformation
	{
		private double _offsetValue;

		public double Transform(double value)
		{
			return value + _offsetValue;
		}

		public string StringRepresentation
		{
			get { return string.Format("(x+{0})", _offsetValue); }
		}

		public IDoubleToDoubleValueTransformation BackTransformation
		{
			get { return new OffsetTransformation() { _offsetValue = -this._offsetValue }; }
		}
	}

	public class ScaleTransformation : IDoubleToDoubleValueTransformation
	{
		private double _scaleValue;

		public double Transform(double value)
		{
			return value * _scaleValue;
		}

		public string StringRepresentation
		{
			get { return string.Format("({0}*x)", _scaleValue); }
		}

		public IDoubleToDoubleValueTransformation BackTransformation
		{
			get { return new ScaleTransformation() { _scaleValue = 1 / this._scaleValue }; }
		}
	}

	public class NaturalLogarithmTransformation : IDoubleToDoubleValueTransformation
	{
		public double Transform(double value)
		{
			return Math.Log(value);
		}

		public string StringRepresentation
		{
			get { return "ln(x)"; }
		}

		public IDoubleToDoubleValueTransformation BackTransformation
		{
			get { return new NaturalExponentialTransform(); }
		}
	}

	public class NaturalExponentialTransform : IDoubleToDoubleValueTransformation
	{
		public double Transform(double value)
		{
			return Math.Exp(value);
		}

		public string StringRepresentation
		{
			get { return "exp(x)"; }
		}

		public IDoubleToDoubleValueTransformation BackTransformation
		{
			get { return new NaturalLogarithmTransformation(); }
		}
	}

	public class CombinedTransform : IDoubleToDoubleValueTransformation
	{
		private List<IDoubleToDoubleValueTransformation> _transformations = new List<IDoubleToDoubleValueTransformation>();

		public double Transform(double value)
		{
			foreach (var item in _transformations)
				value = item.Transform(value);
			return value;
		}

		public string StringRepresentation
		{
			get { return "CombinedTransform"; }
		}

		public IDoubleToDoubleValueTransformation BackTransformation
		{
			get
			{
				CombinedTransform t = new CombinedTransform();
				for (int i = _transformations.Count - 1; i >= 0; i--)
					t._transformations.Add(_transformations[i].BackTransformation);
				return t;
			}
		}
	}

	public enum TransformedValueRepresentation
	{
		/// <summary>Value is used directly (no transformation).</summary>
		Original = 0,

		/// <summary>Value is used in form of its inverse.</summary>
		Inverse,

		/// <summary>Value is used in form of its negative.</summary>
		Negative,

		/// <summary>Value is used in the form of its decadic logarithm.</summary>
		DecadicLogarithm,

		/// <summary>Value is used in the form of its negative decadic logarithm.</summary>
		NegativeDecadicLogarithm,

		/// <summary>Value is used in the form of its natural logarithm.</summary>
		NaturalLogarithm,

		/// <summary>Value is used in the form of its negative natural logarithm.</summary>
		NegativeNaturalLogarithm
	}

	public struct TransformedValue
	{
		#region Transformations (static)

		public static double TransformedValueToBaseValue(double srcValue, TransformedValueRepresentation srcUnit)
		{
			switch (srcUnit)
			{
				case TransformedValueRepresentation.Original:
					return srcValue;

				case TransformedValueRepresentation.Inverse:
					return 1 / srcValue;

				case TransformedValueRepresentation.Negative:
					return -srcValue;

				case TransformedValueRepresentation.DecadicLogarithm:
					return Math.Pow(10, srcValue);

				case TransformedValueRepresentation.NegativeDecadicLogarithm:
					return Math.Pow(10, -srcValue);

				case TransformedValueRepresentation.NaturalLogarithm:
					return Math.Exp(srcValue);

				case TransformedValueRepresentation.NegativeNaturalLogarithm:
					return Math.Exp(-srcValue);

				default:
					throw new ArgumentOutOfRangeException("ValueTransformationType unknown: " + srcUnit.ToString());
			}
		}

		public static double BaseValueToTransformedValue(double baseValue, TransformedValueRepresentation destTransform)
		{
			switch (destTransform)
			{
				case TransformedValueRepresentation.Original:
					return baseValue;

				case TransformedValueRepresentation.Inverse:
					return 1 / baseValue;

				case TransformedValueRepresentation.Negative:
					return -baseValue;

				case TransformedValueRepresentation.DecadicLogarithm:
					return Math.Log10(baseValue);

				case TransformedValueRepresentation.NegativeDecadicLogarithm:
					return -Math.Log10(baseValue);

				case TransformedValueRepresentation.NaturalLogarithm:
					return Math.Log(baseValue);

				case TransformedValueRepresentation.NegativeNaturalLogarithm:
					return -Math.Log(baseValue);

				default:
					throw new ArgumentOutOfRangeException("ValueTransformationType unknown: " + destTransform.ToString());
			}
		}

		public static string GetFormula(string nameOfVariable, TransformedValueRepresentation transform)
		{
			switch (transform)
			{
				case TransformedValueRepresentation.Original:
					return nameOfVariable;

				case TransformedValueRepresentation.Inverse:
					return "1/" + nameOfVariable;

				case TransformedValueRepresentation.Negative:
					return "-" + nameOfVariable;

				case TransformedValueRepresentation.DecadicLogarithm:
					return "lg(" + nameOfVariable + ")";

				case TransformedValueRepresentation.NegativeDecadicLogarithm:
					return "-lg(" + nameOfVariable + ")";

				case TransformedValueRepresentation.NaturalLogarithm:
					return "ln(" + nameOfVariable + ")";

				case TransformedValueRepresentation.NegativeNaturalLogarithm:
					return "-ln(" + nameOfVariable + ")";

				default:
					throw new ArgumentOutOfRangeException("ValueTransformationType unknown: " + transform.ToString());
			}
		}

		public static double FromTo(double srcValue, TransformedValueRepresentation srcUnit, TransformedValueRepresentation destUnit)
		{
			if (srcUnit == destUnit)
				return srcValue;
			else
				return BaseValueToTransformedValue(TransformedValueToBaseValue(srcValue, srcUnit), destUnit);
		}

		#endregion Transformations (static)
	}
}