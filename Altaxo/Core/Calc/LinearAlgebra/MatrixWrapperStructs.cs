using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Calc.LinearAlgebra
{
	/// <summary>
	/// Very thin wrapper around a jagged array just to provided number of rows and columns along with the array itself.
	/// The ridge array is oriented vertically, thus access to the array is down by array[row][column].
	/// </summary>
	/// <typeparam name="T">Type of scaler value.</typeparam>
	public struct BEJaggedArrayMatrixWrapper<T> : IMatrix<T>
	{
		public T[][] Array { get; private set; }
		public int Rows { get; private set; }
		public int Columns { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BEJaggedArrayMatrixWrapper{T}"/> struct by wrapping the provided
		/// array.
		/// </summary>
		/// <param name="array">The array to wrap.</param>
		/// <param name="rows">Number of rows.</param>
		/// <param name="cols">Number of columns.</param>
		public BEJaggedArrayMatrixWrapper(T[][] array, int rows, int cols)
		{
			if (null == array)
				throw new ArgumentNullException(nameof(array));
			if (rows < 0 || rows > array.Length)
				throw new ArgumentOutOfRangeException(nameof(rows), "Provided array is shorter than number of rows");
			if (cols < 0 || (rows > 0 && cols > array[0].Length))
				throw new ArgumentOutOfRangeException(nameof(cols), "Provided array is shorter than number of cols");

			Array = array;
			Rows = rows;
			Columns = cols;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BEJaggedArrayMatrixWrapper{T}"/> struct and creates
		/// the jagged matrix array.
		/// </summary>
		/// <param name="rows">The number of rows.</param>
		/// <param name="cols">The number of columns.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// rows - Number of rows has to be >=0
		/// or
		/// cols - Number of cols has to be >=0
		/// </exception>
		public BEJaggedArrayMatrixWrapper(int rows, int cols)
		{
			if (!(rows >= 0))
				throw new ArgumentOutOfRangeException(nameof(rows), "Number of rows has to be >=0");
			if (!(cols >= 0))
				throw new ArgumentOutOfRangeException(nameof(cols), "Number of cols has to be >=0");
			Rows = rows;
			Columns = cols;
			Array = new T[Rows][];
			for (int i = 0; i < Rows; ++i)
			{
				Array[i] = new T[Columns];
			}
		}

		T IROMatrix<T>.this[int row, int col]
		{
			get
			{
				return Array[row][col];
			}
		}

		public T this[int row, int col]
		{
			get
			{
				return Array[row][col];
			}
			set
			{
				Array[row][col] = value;
			}
		}
	}
}