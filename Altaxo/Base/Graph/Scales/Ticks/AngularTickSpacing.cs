using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Scales.Ticks
{
	public abstract class AngularTickSpacing : NumericTickSpacing
	{
		/// <summary>
		/// Denotes the possible dividers of 360° to form ticks.
		/// </summary>
		protected static readonly int[] _possibleDividers = 
      {
        1,   // 360°
        2,   // 180°
        3,   // 120°
        4,   // 90°
        6,   // 60°
        8,   // 45°
        12,  // 30°
        16,  // 22.5°
        24,  // 15°
        36,  // 10°
        72,  // 5°
        360  // 1°
      };

		/// <summary>Major tick divider. Should be one of the values of the table <see cref="_possibleDividers"/></summary>
		protected int _majorTickDivider = 8;
		/// <summary>Minor tick divider. Should be one of the values of the table <see cref="_possibleDividers"/></summary>
		protected int _minorTickDivider = 24;
		/// <summary>If true, the scale uses positive and negative values (-180..180°) instead of only positive values (0..360°).</summary>
		protected bool _usePositiveNegativeAngles;


		List<double> _majorTicks;
		List<double> _minorTicks;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AngularTickSpacing), 0)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				AngularTickSpacing s = (AngularTickSpacing)obj;


			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				AngularTickSpacing s = SDeserialize(o, info, parent);
				return s;
			}


			protected virtual AngularTickSpacing SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				AngularTickSpacing s = (AngularTickSpacing)o;

				return s;
			}
		}
		#endregion


		public AngularTickSpacing()
		{
		}

		public AngularTickSpacing(NumericTickSpacing from)
		{
		}

		

		public int[] GetPossibleDividers()
		{
			return (int[])_possibleDividers.Clone();
		}

		public int MajorTickDivider
		{
			get
			{
				return _majorTickDivider;
			}
			set
			{
				_majorTickDivider = value;
			}
		}

		public int MinorTickDivider
		{
			get
			{
				return _minorTickDivider;
			}
			set
			{
				_minorTickDivider = value;
			}
		}

		/// <summary>If true, use degree instead of radian.</summary>
		public abstract bool UseDegree { get; }


		public bool UseSignedValues
		{
			get
			{
				return _usePositiveNegativeAngles;
			}
			set
			{
				_usePositiveNegativeAngles = value;
			}
		}



		public override bool PreProcessScaleBoundaries(ref Altaxo.Data.AltaxoVariant org, ref Altaxo.Data.AltaxoVariant end, bool isOrgExtendable, bool isEndExtendable)
		{
			return false; // no change of the proposed boundaries
		}

		public override void FinalProcessScaleBoundaries(Altaxo.Data.AltaxoVariant org, Altaxo.Data.AltaxoVariant end, Scale scale)
		{
			InternalGetMajorTicks(org, end);
			InternalGetMinorTicks(org, end);
		}

		double GetAngleInDegrees(double org)
		{
			if (UseDegree)
				return org;
			else
				return org * 180 / Math.PI;
		}

		void InternalGetMajorTicks(double org, double end)
		{
			_majorTicks.Clear();
			double start = GetAngleInDegrees(org);

			for (int i = 0; i < _majorTickDivider; i++)
			{
				double angle = start + i * 360.0 / _majorTickDivider;
				angle = Math.IEEERemainder(angle, 360);
				if (_usePositiveNegativeAngles)
				{
					if (angle > 180)
						angle -= 360;
				}
				else
				{
					if (angle < 0)
						angle += 360;
				}
				_majorTicks.Add(UseDegree ? angle : angle * Math.PI / 180);
			}
		}


		void InternalGetMinorTicks(double org, double end)
		{
			_minorTicks.Clear();

			if (_minorTickDivider <= 0)
				return;
			if (_minorTickDivider <= _majorTickDivider)
				return;
			if (_minorTickDivider % _majorTickDivider != 0)
			{
				// look for a minor tick divider greater than the _majortickdivider
				for (int i = 0; i < _possibleDividers.Length; i++)
				{
					if (_possibleDividers[i] > _majorTickDivider && _possibleDividers[i] % _majorTickDivider == 0)
					{
						_minorTickDivider = _possibleDividers[i];
						break;
					}
				}
			}
			if (_minorTickDivider % _majorTickDivider != 0)
				return;

			int majorTicksEvery = _minorTickDivider / _majorTickDivider;

			double start = GetAngleInDegrees(org);
			for (int i = 1; i < _minorTickDivider; i++)
			{
				if (i % majorTicksEvery == 0)
					continue;

				double angle = start + i * 360.0 / _minorTickDivider;
				angle = Math.IEEERemainder(angle, 360);
				if (_usePositiveNegativeAngles)
				{
					if (angle > 180)
						angle -= 360;
				}
				else
				{
					if (angle < 0)
						angle += 360;
				}
				_minorTicks.Add(UseDegree ? angle : angle * Math.PI / 180);
			}
		}


	}
}
