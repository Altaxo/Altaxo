using System;

namespace Altaxo.Calc
{
	/// <summary>
	/// Summary description for RelativeOrAbsoluteValue.
	/// </summary>
	public struct RelativeOrAbsoluteValue
	{
		private bool m_bIsRelative; // per default, m_bRelative is false, so the value is interpreted as absolute
		private double m_Value;

		public RelativeOrAbsoluteValue(double val)
		{
			m_bIsRelative=false;
			m_Value = val;
		}

		public RelativeOrAbsoluteValue(double val, bool isRelative)
		{
			m_bIsRelative= isRelative;
			m_Value = val;
		}


		public bool IsRelative
		{
			get { return m_bIsRelative; }
			set { m_bIsRelative = value; }
		}
		public double Value
		{
			get { return m_Value; }
			set { Value = value; }
		}

		public double GetValueRelativeTo(double r)
		{
			return m_bIsRelative ? r*m_Value : m_Value;
		}
	}
}
