using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Science.Thermodynamics.Fluids
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class CASRegistryNumberAttribute : Attribute
	{
		public string CASRegistryNumber { get; private set; }

		public CASRegistryNumberAttribute(string casRegistryNumber)
		{
			this.CASRegistryNumber = casRegistryNumber;
		}
	}
}
