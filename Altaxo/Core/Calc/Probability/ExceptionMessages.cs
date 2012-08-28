using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Calc.Probability
{
	public static class ExceptionMessages
	{
		static readonly StringResourceKey SRKArgumentNull = new StringResourceKey("ArgumentNull", "{0} is a null reference (Nothing in Visual Basic).", "Describes that a particular argument named in the first argument is null");

		static readonly StringResourceKey SRKArgumentOutOfRangeGreaterEqual = new StringResourceKey("ArgumentOutOfRangeGreaterEqual" ,"{0} must be greater than or equal to {1}.", "Describes that a variable named in the first argument must be greater than or equal to a variable named in the second argument");

		static readonly StringResourceKey SRKArgumentRangeLessEqual = new StringResourceKey("ArgumentRangeLessEqual", "The range between {0} and {1} must be less than or equal to {2}.", "Designates that the difference between first and second argument must be less than or equal to the value given in the third argument.");

		public static string ArgumentNull { get { return StringResources.AltaxoCore.GetString(SRKArgumentNull); } }
		public static string ArgumentOutOfRangeGreaterEqual { get { return StringResources.AltaxoCore.GetString(SRKArgumentOutOfRangeGreaterEqual); } }
		public static string ArgumentRangeLessEqual { get { return StringResources.AltaxoCore.GetString(SRKArgumentRangeLessEqual); } }
	}
}
