#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

namespace Altaxo.Drawing.DashPatterns
{
	public class Custom : DashPatternBase
	{
		private double[] _customDashPattern;
		private double _dashOffset;

		#region Serialization

		private Custom(double offset, double[] pattern)
		{
			_dashOffset = offset;
			_customDashPattern = pattern;
		}

		/// <summary>
		/// 2016-04-22 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Custom), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (Custom)obj;
				info.AddValue("DashOffset", s._dashOffset);
				info.CreateArray("Pattern", s._customDashPattern.Length);
				foreach (var v in s._customDashPattern)
					info.AddValue("e", v);
				info.CommitArray();

				SerializeV0(s, info);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				double offset = info.GetDouble("DashOffset");
				int count = info.OpenArray("Pattern");
				double[] pattern = new double[count];
				for (int i = 0; i < pattern.Length; ++i)
					pattern[i] = info.GetDouble("e");
				info.CloseArray(count);

				return DeserializeV0(new Custom(offset, pattern), info, parent);
			}
		}

		#endregion Serialization

		public Custom(IEnumerable<double> dashPattern)
		{
			if (null == dashPattern)
				throw new ArgumentNullException(nameof(dashPattern));

			_customDashPattern = dashPattern.ToArray();

			if (_customDashPattern.Length == 0)
				throw new ArgumentOutOfRangeException(nameof(dashPattern) + " is empty");
		}

		public Custom(IEnumerable<double> dashPattern, double dashOffset)
		{
			if (null == dashPattern)
				throw new ArgumentNullException(nameof(dashPattern));

			_customDashPattern = dashPattern.ToArray();

			if (_customDashPattern.Length == 0)
				throw new ArgumentOutOfRangeException(nameof(dashPattern) + " is empty");

			if (double.IsNaN(dashOffset) || double.IsInfinity(dashOffset))
				throw new ArgumentOutOfRangeException(nameof(dashOffset));

			_dashOffset = dashOffset;
		}

		public override double this[int index]
		{
			get
			{
				if (index < 0 || index >= _customDashPattern.Length)
					throw new IndexOutOfRangeException(nameof(index));

				return _customDashPattern[index];
			}
			set
			{
				throw new InvalidOperationException("Sorry, this class is read-only");
			}
		}

		public override int Count
		{
			get
			{
				return _customDashPattern.Length;
			}
		}

		public override double DashOffset
		{
			get
			{
				return _dashOffset;
			}
		}

		public override bool Equals(object obj)
		{
			var from = obj as Custom;
			if (null == from)
				return false;

			if (this._customDashPattern.Length != from._customDashPattern.Length)
				return false;

			for (int i = 0; i < _customDashPattern.Length; ++i)
				if (this._customDashPattern[i] != from._customDashPattern[i])
					return false;

			return true;
		}

		public override int GetHashCode()
		{
			return GetType().GetHashCode() + _customDashPattern.Length * 17 + _customDashPattern[0].GetHashCode();
		}
	}
}