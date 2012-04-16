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

using Altaxo.Data;

namespace Altaxo.Graph.Scales.Ticks
{
	/// <summary>
	/// Base class responsible for the spacing of ticks (major and minor) along a scale.
	/// </summary>
	public abstract class TickSpacing : Main.ICopyFrom, Main.IChangedEventSource
	{
		[field: NonSerialized]
		public event EventHandler Changed;

		[NonSerialized]
		protected Main.EventSuppressor _suppressorOfChangedEvent;

		protected TickSpacing()
		{
			_suppressorOfChangedEvent = new Main.EventSuppressor(OnChanged);
		}

		public TickSpacing(TickSpacing from)
		{
			_suppressorOfChangedEvent = new Main.EventSuppressor(OnChanged);
			CopyFrom(from);
		}

		public IDisposable BeginUpdate()
		{
			return _suppressorOfChangedEvent.Suspend();
		}


		public abstract object Clone();
		public virtual bool CopyFrom(object obj)
		{
			return obj is TickSpacing;
		}


		/// <summary>
		/// Decides giving a raw org and end value, whether or not the scale boundaries should be extended to
		/// have more 'nice' values. If the boundaries should be changed, the function return true, and the
		/// org and end argument contain the proposed new scale boundaries.
		/// </summary>
		/// <param name="org">Raw scale org.</param>
		/// <param name="end">Raw scale end.</param>
		/// <param name="isOrgExtendable">True when the org is allowed to be extended.</param>
		/// <param name="isEndExtendable">True when the scale end can be extended.</param>
		/// <returns>True when org or end are changed. False otherwise.</returns>
		public abstract bool PreProcessScaleBoundaries(ref AltaxoVariant org, ref AltaxoVariant end, bool isOrgExtendable, bool isEndExtendable);


		/// <summary>
		/// Calculates the ticks based on the org and end of the scale. Org and End now are given and can not be changed anymore.
		/// </summary>
		/// <param name="org">Scale origin.</param>
		/// <param name="end">Scale end.</param>
		/// <param name="scale">The scale.</param>
		public abstract void FinalProcessScaleBoundaries(AltaxoVariant org, AltaxoVariant end, Scale scale);

		/// <summary>
		/// This will return the the major ticks as <see cref="AltaxoVariant" />.
		/// </summary>
		/// <returns>The array with major tick values.</returns>
		public abstract AltaxoVariant[] GetMajorTicksAsVariant();


		/// <summary>
		/// This will return the minor ticks as array of <see cref="AltaxoVariant" />.
		/// </summary>
		/// <returns>The array with minor tick values.</returns>
		public abstract AltaxoVariant[] GetMinorTicksAsVariant();


		public virtual double[] GetMajorTicksNormal(Scale scale)
		{
			AltaxoVariant[] vars = GetMajorTicksAsVariant();
			double[] result = new double[vars.Length];
			for (int i = 0; i < vars.Length; i++)
				result[i] = scale.PhysicalVariantToNormal(vars[i]);

			return result;
		}

		public virtual double[] GetMinorTicksNormal(Scale scale)
		{
			AltaxoVariant[] vars = GetMinorTicksAsVariant();
			double[] result = new double[vars.Length];
			for (int i = 0; i < vars.Length; i++)
				result[i] = scale.PhysicalVariantToNormal(vars[i]);

			return result;
		}


		protected void OnChanged()
		{
			if (_suppressorOfChangedEvent.GetDisabledWithCounting())
				return;

			if (null != Changed)
				Changed(this, EventArgs.Empty);
		}
	}
}
