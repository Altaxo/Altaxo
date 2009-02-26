using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Altaxo.Data;

namespace Altaxo.Graph.Scales.Ticks
{
	public abstract class NumericTickSpacing : TickSpacing
	{


		/// <summary>
		/// Returns the physical values
		/// at which major ticks should occur
		/// </summary>
		/// <returns>physical values for the major ticks</returns>
		public virtual double[] GetMajorTicks()
		{
			return new double[] { }; // return a empty array per default
		}

		/// <summary>
		/// GetMinorTicks returns the physical values
		/// at which minor ticks should occur
		/// </summary>
		/// <returns>physical values for the minor ticks</returns>
		public virtual double[] GetMinorTicks()
		{
			return new double[] { }; // return a empty array per default
		}


		public override double[] GetMajorTicksNormal(Scale scale)
		{
			double[] ticks = GetMajorTicks();
			for (int i = 0; i < ticks.Length; i++)
			{
				ticks[i] = scale.PhysicalVariantToNormal(ticks[i]);
			}
			return ticks;
		}
		public override double[] GetMinorTicksNormal(Scale scale)
		{
			double[] ticks = GetMinorTicks();
			for (int i = 0; i < ticks.Length; i++)
			{
				ticks[i] = scale.PhysicalVariantToNormal(ticks[i]);
			}
			return ticks;
		}
		public override AltaxoVariant[] GetMajorTicksAsVariant()
		{
			double[] ticks = GetMajorTicks();
			AltaxoVariant[] vticks = new AltaxoVariant[ticks.Length];
			for (int i = 0; i < ticks.Length; ++i)
				vticks[i] = ticks[i];
			return vticks;
		}
		public override AltaxoVariant[] GetMinorTicksAsVariant()
		{
			double[] ticks = GetMinorTicks();
			AltaxoVariant[] vticks = new AltaxoVariant[ticks.Length];
			for (int i = 0; i < ticks.Length; ++i)
				vticks[i] = ticks[i];
			return vticks;
		}


	}
}
