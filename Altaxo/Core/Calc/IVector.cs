using System;

namespace Altaxo.Calc
{
  /// <summary>
  /// Interface for a read-only vector of double values.
  /// </summary>
  public interface IROVector
  {

    /// <summary>Gets the value at index i with LowerBound &lt;= i &lt;=UpperBound.</summary>
    /// <value>The element at index i.</value>
    double this[int i] { get; }
 
    /// <summary>The smallest valid index of this vector</summary>
    int LowerBound { get; }
    
    /// <summary>The greates valid index of this vector. Is by definition LowerBound+Length-1.</summary>
    int UpperBound { get; }
    
    /// <summary>The number of elements of this vector.</summary>
    int Length { get; }  // change this later to length property

  }

  /// <summary>
  /// Interface for a readable and writeable vector of double values.
  /// </summary>
  public interface IVector : IROVector
  {
    /// <summary>Read/write Accessor for the element at index i.</summary>
    /// <value>The element at index i.</value>
    new double this[int i] { get; set; }
  }
}
