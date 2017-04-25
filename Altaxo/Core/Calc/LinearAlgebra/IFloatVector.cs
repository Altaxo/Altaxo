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

#endregion Copyright

using System;
using System.Collections;
using System.Collections.Generic;

namespace Altaxo.Calc.LinearAlgebra
{
	public abstract class AbstractROFloatVector : IROFloatVector
	{
		static public implicit operator AbstractROFloatVector(float[] src)
		{
			return new ROFloatVector(src);
		}

		#region IROVector Members

		public abstract int Length
		{
			get;
		}

		public int Count { get { return Length; } }

		#endregion IROVector Members

		#region INumericSequence Members

		public abstract float this[int i]
		{
			get;
		}

		public IEnumerator<float> GetEnumerator()
		{
			var len = Length;
			for (int i = 0; i < len; ++i)
				yield return this[i];
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			var len = Length;
			for (int i = 0; i < len; ++i)
				yield return this[i];
		}

		#endregion INumericSequence Members
	}

	public class ROFloatVector : AbstractROFloatVector
	{
		private float[] _data;

		public ROFloatVector(float[] array)
		{
			_data = array;
		}

		static public implicit operator ROFloatVector(float[] src)
		{
			return new ROFloatVector(src);
		}

		#region IROVector Members

		public override int Length
		{
			get { return _data.Length; }
		}

		#endregion IROVector Members

		#region INumericSequence Members

		public override float this[int i]
		{
			get { return _data[i]; }
		}

		#endregion INumericSequence Members
	}
}