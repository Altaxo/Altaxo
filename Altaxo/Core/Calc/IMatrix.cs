using System;

namespace Altaxo.Calc
{
  /// <summary>
  /// IROMatrix represents a read-only matrix of double values.
  /// </summary>
  public interface IROMatrix
  {
    /// <summary>Gets an element of the matrix at (row, col).</summary>
    double this[int row, int col] { get; }
    /// <summary>The number of rows of the matrix.</summary>
    int Rows { get; }
    /// <summary>The number of columns of the matrix.</summary>
    int Columns { get; }
  }

  /// <summary>
  /// IMatrix represents the simplest form of a 2D matrix, which is readable and writeable.
  /// </summary>
  public interface IMatrix : IROMatrix
  {
    /// <summary>Get / sets an element of the matrix at (row, col).</summary>
    new double this[int row, int col] { get; set; }
  
  }

  /// <summary>
  /// IRightExtensibleMatrix extends IMatrix in a way that another matrix of appropriate dimensions
  /// can be appended to the right of the matrix. 
  /// </summary>
  public interface IRightExtensibleMatrix : IMatrix
  {
    /// <summary>
    /// Append matrix a to the right edge of this matrix. Matrix a must have the same number of rows than this matrix, except this matrix
    /// is still empty, in which case the right dimension of this matrix is set.
    /// </summary>
    /// <param name="a">The matrix to append.</param>
    void AppendRight(IMatrix a);
  }


  /// <summary>
  /// IBottomExtensibleMatrix extends IMatrix in a way that another matrix of appropriate dimensions
  /// can be appended to the bottom of the matrix. 
  /// </summary>
  public interface IBottomExtensibleMatrix : IMatrix
  {
    /// <summary>
    /// Append matrix a to the bottom of this matrix. Matrix a must have the same number of columns than this matrix, except this matrix
    /// is still empty, in which case the right dimension of this matrix is set.
    /// </summary>
    /// <param name="a">The matrix to append.</param>
    void AppendBottom(IMatrix a);
  }

  /// <summary>
  /// IExtensibleMatrix extends IMatrix in a way that another matrix of appropriate dimensions
  /// can be appended either to the right or to the bottom of the matrix. 
  /// </summary>
  public interface IExtensibleMatrix : IRightExtensibleMatrix, IBottomExtensibleMatrix
  {
  }

 


}
