using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Science.Thermodynamics.Fluids
{
	/// <summary>
	/// Consistency test(s) for classes derived from <see cref="BinaryMixtureDefinitionBase"/>.
	/// </summary>
	[TestFixture]
	public class MixtureConsistencyTests
	{
		/// <summary>
		/// Tests the consistence between the presence/absence of a departure function
		/// and the value of F of the binary mixture.
		/// </summary>
		[Test]
		public void TestPresenceOfDepartureFunction()
		{
			var assembly = typeof(MixtureOfFluids).Assembly;

			foreach (var type in assembly.GetTypes().Where(t => typeof(BinaryMixtureDefinitionBase).IsAssignableFrom(t) && !t.IsAbstract))
			{
				var m = type.GetProperty("Instance").GetGetMethod();
				var instance = (BinaryMixtureDefinitionBase)m.Invoke(null, new object[0]);

				if (instance.F == 0) // if F is null, then no departure function should be defined.
				{
					var d1 = instance.DepartureFunction_OfReducedVariables(1, 1);
					var d2 = instance.DepartureFunction_OfReducedVariables(2, 2);

					Assert.IsTrue(0 == d1 && 0 == d2, "Mixture {0}: Since F is 0, there should be no departure function defined", instance.GetType());
				}
				else // F F is not null, then there should be a departure function defined
				{
					var d1 = instance.DepartureFunction_OfReducedVariables(1, 1);
					var d2 = instance.DepartureFunction_OfReducedVariables(2, 2);

					Assert.IsTrue(0 != d1 || 0 != d2, "Mixture {0}: Since F is not 0, there should be a departure function defined", instance.GetType());
				}
			}
		}
	}
}
