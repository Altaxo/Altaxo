using Complex = System.Numerics.Complex;

namespace Altaxo.Calc.Providers.SparseSolver
{
  /// <summary>
  /// Specifies the sparse matrix structure option.
  /// </summary>
  public enum DssMatrixStructure : int
  {
    /// <summary>
    /// Symmetric real matrix.
    /// </summary>
    Symmetric = 536870976,
    /// <summary>
    /// Symmetric real matrix structure only.
    /// </summary>
    SymmetricStructure = 536871040,
    /// <summary>
    /// Nonsymmetric real matrix.
    /// </summary>
    Nonsymmetric = 536871104,
    /// <summary>
    /// Symmetric complex matrix.
    /// </summary>
    SymmetricComplex = 536871168,
    /// <summary>
    /// Symmetric complex matrix structure only.
    /// </summary>
    SymmetricStructureComplex = 536871232,
    /// <summary>
    /// Nonsymmetric complex matrix.
    /// </summary>
    NonsymmetricComplex = 536871296,
  }

  /// <summary>
  /// Specifies the factorization option.
  /// </summary>
  public enum DssMatrixType : int
  {
    /// <summary>
    /// Positive-definite matrix.
    /// </summary>
    PositiveDefinite = 134217792,
    /// <summary>
    /// Indefinite matrix.
    /// </summary>
    Indefinite = 134217856,
    /// <summary>
    /// Hermitian positive-definite matrix.
    /// </summary>
    HermitianPositiveDefinite = 134217920,
    /// <summary>
    /// Hermitian indefinite matrix.
    /// </summary>
    HermitianIndefinite = 134217984
  }

  /// <summary>
  /// Solver step's substitution.
  /// </summary>
  public enum DssSystemType : int
  {
    /// <summary>
    /// Solve a system, Ax = b.
    /// </summary>
    DontTranspose = 0,
    /// <summary>
    /// Solve a transposed system, A'x = b
    /// </summary>
    Transpose = 262144,
    /// <summary>
    /// Solve a conjugate transposed system, A†x = b
    /// </summary>
    ConjugateTranspose = 524288,
  }

  /// <summary>
  /// Specifies sparse solver status values.
  /// </summary>
  public enum DssStatus : int
  {
    /// <summary>
    /// The operation was successful.
    /// </summary>
    MKL_DSS_SUCCESS = 0,
    /// <summary>
    /// A zero pivot was encountered.
    /// </summary>
    MKL_DSS_ZERO_PIVOT = -1,
    /// <summary>
    /// The solver ran out of memory.
    /// </summary>
    MKL_DSS_OUT_OF_MEMORY = -2,
    /// <summary>
    /// An unspecified failure occurred.
    /// </summary>
    MKL_DSS_FAILURE = -3,
    /// <summary>
    /// An invalid row index was encountered.
    /// </summary>
    MKL_DSS_ROW_ERR = -4,
    /// <summary>
    /// An invalid column index was encountered.
    /// </summary>
    MKL_DSS_COL_ERR = -5,
    /// <summary>
    /// Too few values were supplied.
    /// </summary>
    MKL_DSS_TOO_FEW_VALUES = -6,
    /// <summary>
    /// Too many values were supplied.
    /// </summary>
    MKL_DSS_TOO_MANY_VALUES = -7,
    /// <summary>
    /// The matrix is not square.
    /// </summary>
    MKL_DSS_NOT_SQUARE = -8,
    /// <summary>
    /// The solver is in an invalid state for the requested operation.
    /// </summary>
    MKL_DSS_STATE_ERR = -9,
    /// <summary>
    /// An invalid option was specified.
    /// </summary>
    MKL_DSS_INVALID_OPTION = -10,
    /// <summary>
    /// Specified options are in conflict.
    /// </summary>
    MKL_DSS_OPTION_CONFLICT = -11,
    /// <summary>
    /// Invalid message level.
    /// </summary>
    MKL_DSS_MSG_LVL_ERR = -12,
    /// <summary>
    /// Invalid termination level.
    /// </summary>
    MKL_DSS_TERM_LVL_ERR = -13,
    /// <summary>
    /// Invalid matrix structure.
    /// </summary>
    MKL_DSS_STRUCTURE_ERR = -14,
    /// <summary>
    /// Reordering failed.
    /// </summary>
    MKL_DSS_REORDER_ERR = -15,
    /// <summary>
    /// Invalid matrix values.
    /// </summary>
    MKL_DSS_VALUES_ERR = -16,
    /// <summary>
    /// Invalid matrix for statistics.
    /// </summary>
    MKL_DSS_STATISTICS_INVALID_MATRIX = -17,
    /// <summary>
    /// Invalid solver state for statistics.
    /// </summary>
    MKL_DSS_STATISTICS_INVALID_STATE = -18,
    /// <summary>
    /// Invalid statistics string.
    /// </summary>
    MKL_DSS_STATISTICS_INVALID_STRING = -19,
    /// <summary>
    /// First reordering phase failed.
    /// </summary>
    MKL_DSS_REORDER1_ERR = -20,
    /// <summary>
    /// Preordering failed.
    /// </summary>
    MKL_DSS_PREORDER_ERR = -21,
    /// <summary>
    /// Invalid diagonal data.
    /// </summary>
    MKL_DSS_DIAG_ERR = -22,
    /// <summary>
    /// A 32-bit integer overflow or incompatibility occurred.
    /// </summary>
    MKL_DSS_I32BIT_ERR = -23,
    /// <summary>
    /// Out-of-core memory error.
    /// </summary>
    MKL_DSS_OOC_MEM_ERR = -24,
    /// <summary>
    /// Out-of-core operation control error.
    /// </summary>
    MKL_DSS_OOC_OC_ERR = -25,
    /// <summary>
    /// Out-of-core read/write error.
    /// </summary>
    MKL_DSS_OOC_RW_ERR = -26,
  }

  /// <summary>
  /// Defines a non-generic sparse solver provider.
  /// </summary>
  public interface ISparseSolverProvider :
      ISparseSolverProvider<double>,
      ISparseSolverProvider<float>,
      ISparseSolverProvider<Complex>,
      ISparseSolverProvider<Complex32>
  {
    /// <summary>
    /// Try to find out whether the provider is available, at least in principle.
    /// Verification may still fail if available, but it will certainly fail if unavailable.
    /// </summary>
    public bool IsAvailable();

    /// <summary>
    /// Initialize and verify that the provider is indeed available. If not, fall back to alternatives like the managed provider.
    /// </summary>
    public void InitializeVerify();

    /// <summary>
    /// Frees memory buffers, caches and handles allocated in or to the provider.
    /// Does not unload the provider itself, it is still usable afterwards.
    /// </summary>
    public void FreeResources();
  }

  /// <summary>
  /// Defines a generic sparse solver provider.
  /// </summary>
  public interface ISparseSolverProvider<T>
      where T : struct
  {
    /// <summary>
    /// Solves a sparse linear system.
    /// </summary>
    public DssStatus Solve(DssMatrixStructure matrixStructure, DssMatrixType matrixType, DssSystemType systemType, int rows, int cols, int nnz, int[] rowIdx, int[] colPtr, T[] values, int nRhs, T[] rhs, T[] solution);
  }
}

