using System;

namespace Altaxo.Calc
{
  /// <summary>
  /// An algorithm failed to converge.
  /// </summary>
  [Serializable]
  public class NonConvergenceException : Exception
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="NonConvergenceException"/> class.
    /// </summary>
    public NonConvergenceException() : base("An algorithm failed to converge.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NonConvergenceException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public NonConvergenceException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NonConvergenceException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that caused the current exception.</param>
    public NonConvergenceException(string message, Exception innerException) : base(message, innerException)
    {
    }
  }

  /// <summary>
  /// An algorithm failed to converge due to a numerical breakdown.
  /// </summary>
  [Serializable]
  public class NumericalBreakdownException : NonConvergenceException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="NumericalBreakdownException"/> class.
    /// </summary>
    public NumericalBreakdownException()
        : base("The algorithm experienced a numerical breakdown.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NumericalBreakdownException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public NumericalBreakdownException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NumericalBreakdownException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that caused the current exception.</param>
    public NumericalBreakdownException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
  }

  /// <summary>
  /// An error occurred calling native provider function.
  /// </summary>
  [Serializable]
  public abstract class NativeInterfaceException : Exception
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="NativeInterfaceException"/> class.
    /// </summary>
    protected NativeInterfaceException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NativeInterfaceException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    protected NativeInterfaceException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NativeInterfaceException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that caused the current exception.</param>
    protected NativeInterfaceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
  }

  /// <summary>
  /// An error occurred calling native provider function.
  /// </summary>
  [Serializable]
  public class InvalidParameterException : NativeInterfaceException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidParameterException"/> class.
    /// </summary>
    public InvalidParameterException()
        : base("An invalid parameter was passed to a native method.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidParameterException"/> class for a specific parameter.
    /// </summary>
    /// <param name="parameter">The one-based parameter index that caused the error.</param>
    public InvalidParameterException(int parameter)
        : base($"An invalid parameter was passed to a native method, parameter number : {parameter}")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidParameterException"/> class for a specific parameter and inner exception.
    /// </summary>
    /// <param name="parameter">The one-based parameter index that caused the error.</param>
    /// <param name="innerException">The exception that caused the current exception.</param>
    public InvalidParameterException(int parameter, Exception innerException)
        : base($"An invalid parameter was passed to a native method, parameter number : {parameter}", innerException)
    {
    }
  }

  /// <summary>
  /// Native provider was unable to allocate sufficient memory.
  /// </summary>
  [Serializable]
  public class MemoryAllocationException : NativeInterfaceException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryAllocationException"/> class.
    /// </summary>
    public MemoryAllocationException()
        : base("Unable to allocate native memory.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryAllocationException"/> class with an inner exception.
    /// </summary>
    /// <param name="innerException">The exception that caused the current exception.</param>
    public MemoryAllocationException(Exception innerException)
        : base("Unable to allocate native memory.", innerException)
    {
    }
  }

  /// <summary>
  /// Native provider failed LU inversion due to a singular U matrix.
  /// </summary>
  [Serializable]
  public class SingularUMatrixException : NativeInterfaceException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SingularUMatrixException"/> class.
    /// </summary>
    public SingularUMatrixException()
        : base("U is singular, and the inversion could not be completed.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SingularUMatrixException"/> class for a specific diagonal element.
    /// </summary>
    /// <param name="element">The one-based index of the zero diagonal element.</param>
    public SingularUMatrixException(int element)
        : base($"U is singular, and the inversion could not be completed. The {element}-th diagonal element of the factor U is zero.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SingularUMatrixException"/> class for a specific diagonal element and inner exception.
    /// </summary>
    /// <param name="element">The one-based index of the zero diagonal element.</param>
    /// <param name="innerException">The exception that caused the current exception.</param>
    public SingularUMatrixException(int element, Exception innerException)
        : base($"U is singular, and the inversion could not be completed. The {element}-th diagonal element of the factor U is zero.", innerException)
    {
    }
  }
}
