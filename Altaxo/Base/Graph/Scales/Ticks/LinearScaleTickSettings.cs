using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Scales.Ticks
{
	/// <summary>
	/// Tick settings for a linear scale.
	/// </summary>
	public class LinearScaleTickSettings : ICloneable	
	{
		/// <summary>If set, gives the number of minor ticks choosen by the user.</summary>
		int? _minorTicks;

		/// <summary>If set, gives the physical value between two major ticks.</summary>
		double? _majorTick;


		public LinearScaleTickSettings()
		{
		}

		public LinearScaleTickSettings(LinearScaleTickSettings from)
		{
			CopyFrom(from);
		}

		public void CopyFrom(LinearScaleTickSettings from)
		{
			this._minorTicks = from._minorTicks;
			this._majorTick = from._majorTick;
		}

		public object Clone()
		{
			return new LinearScaleTickSettings(this);
		}
		
		public int? MinorTicks
		{
			get
			{
				return _minorTicks;
			}
			set
			{
				_minorTicks = value;
			}
		}

		public double? MajorTick
		{
			get
			{
				return _majorTick;
			}
			set
			{
				_majorTick = value;
			}
		}
	}
}
