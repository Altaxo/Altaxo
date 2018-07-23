using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Science.Thermodynamics.Fluids
{
	/// <summary>
	/// Associates a CAS registry number to a (pure) fluid or two CAS registry numbers to a binary mixture.
	/// </summary>
	/// <seealso cref="System.Attribute" />
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class CASRegistryNumberAttribute : Attribute
	{
		/// <summary>
		/// Gets the CAS registry number.
		/// </summary>
		/// <value>
		/// The CAS registry number.
		/// </value>
		public string CASRegistryNumber { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CASRegistryNumberAttribute"/> class.
		/// </summary>
		/// <param name="casRegistryNumber">The CAS registry number.</param>
		public CASRegistryNumberAttribute(string casRegistryNumber)
		{
			this.CASRegistryNumber = casRegistryNumber;
		}
	}
}
