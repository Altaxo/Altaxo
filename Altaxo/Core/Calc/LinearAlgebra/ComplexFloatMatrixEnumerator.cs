/*
 * ComplexFloatMatrixEnumerator.cs
 * 
 * Copyright (c) 2004, dnAnalytics Project. All rights reserved.
*/

using System;
using System.Collections;

namespace Altaxo.Calc.LinearAlgebra {
	///<summary>
	/// Defines an Enumerator for the ComplexFloatMatrix that supports 
	/// simple iteration over each vector component.
	///</summary>
	
	sealed internal class ComplexFloatMatrixEnumerator: IEnumerator {
		private ComplexFloatMatrix m;
		private int index;
		private int length;

		///<summary> Constructor </summary>
		public ComplexFloatMatrixEnumerator (ComplexFloatMatrix matrix) {
			m=matrix;
			index=-1;
			length=m.RowLength*m.ColumnLength;
		}
		
		///<summary> Return the current <c>ComplexFloatMatrix</c> component</summary>
		public ComplexFloat Current {
			get {
				if (index<0 || index>=length)
					throw new InvalidOperationException();
				return m[index % m.RowLength, index / m.RowLength];
			}
		}
		object IEnumerator.Current {
			get { return Current; }
		}
		
		///<summary> Move the index to the next component </summary>
		public bool MoveNext() {
			if (length!=m.RowLength*m.ColumnLength)
				throw new InvalidOperationException();
			index ++;
			if (index>=length)
			{
				index=length;
				return false;
			}
			else
			{
				return true;
			}
		}
		
		///<summary> Set the enumerator to it initial position </summary>
		public void Reset() {
			index=-1;
		}
	}
}
