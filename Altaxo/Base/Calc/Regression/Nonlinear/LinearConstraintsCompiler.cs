#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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
using System.Globalization;
using System.Linq;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Optimization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Altaxo.Calc.Regression.Nonlinear
{
  // =========================================================================
  // Public result types
  // =========================================================================

  /// <summary>
  /// A single parsed constraint with its row vector and RHS value.
  /// </summary>
  public class ParsedConstraint
  {
    /// <summary>The original expression string.</summary>
    public string Expression { get; init; } = "";

    /// <summary>The constraint kind as written by the user.</summary>
    public ConstraintKind Kind { get; init; }

    /// <summary>
    /// Coefficient vector for the parameters (length = number of parameters).
    /// Represents the left-hand side after moving everything to LHS:
    ///   coefficients · p  [op]  Rhs
    /// The op corresponds to Kind.
    /// </summary>
    public double[] Coefficients { get; init; } = Array.Empty<double>();

    /// <summary>Right-hand side scalar.</summary>
    public double Rhs { get; init; }
  }

  /// <summary>
  /// Kind of constraint relation as written by the user.
  /// </summary>
  public enum ConstraintKind
  {
    /// <summary>
    /// Specifies that the left-hand side must equal the right-hand side.
    /// </summary>
    Equality,
    /// <summary>
    /// Specifies that the left-hand side must be greater than or equal to the right-hand side.
    /// </summary>
    GreaterOrEqual,
    /// <summary>
    /// Specifies that the left-hand side must be less than or equal to the right-hand side.
    /// </summary>
    LessOrEqual
  }

  /// <summary>
  /// The two matrices/vectors that the simplified LinearConstraintsProjector accepts:
  ///   A·x  =  b      (equality)
  ///   C·x  &lt;= d     (inequality — all kinds folded in, geq rows negated)
  /// Null means that constraint type is absent.
  /// </summary>
  public class ProjectorMatrices
  {
    /// <summary>
    /// Gets the matrix used for equality comparison.
    /// </summary>
    public Matrix<double>? A { get; init; }   // equality

    /// <summary>
    /// Gets the vector used for equality comparison.
    /// </summary>
    public Vector<double>? B { get; init; }

    /// <summary>
    /// Gets the matrix representing the coefficients of the linear inequality constraints.
    /// </summary>
    /// <remarks>Each row of the matrix corresponds to a linear constraint of the form C·x ≤ d, where C is
    /// this matrix, x is the variable vector, and d is the constraint bound vector. The dimensions of the matrix must
    /// be compatible with the variable vector and the constraint vector.</remarks>
    public Matrix<double>? C { get; init; }   // C·x <= d

    /// <summary>
    /// Gets the vector of coefficients D for the model or operation.
    /// </summary>
    /// <remarks>The value may be null if the coefficients have not been set or are not applicable for the
    /// current context.</remarks>
    public Vector<double>? D { get; init; }

    /// <summary>
    /// Constructs a projector directly from these matrices.
    /// </summary>
    /// <returns>A <see cref="LinearConstraintsProjector"/> initialized from the available matrices.</returns>
    public LinearConstraintsProjector ToProjector() => new LinearConstraintsProjector(A, B, C, D);

    /// <summary>
    /// Try to construct a projector directly from these matrices.
    /// </summary>
    /// <returns>A tuple of a <see cref="LinearConstraintsProjector"/> initialized from the available matrices, and an error message. Either the projector is not <c>null</c>, or the error message contains a descriptive error.</returns>
    public (LinearConstraintsProjector? projector, string? errorMessage) TryConvertToProjector() => LinearConstraintsProjector.TryCreate(A, B, C, D);

  }



  /// <summary>
  /// Result of compiling a set of constraint expression strings.
  /// </summary>
  public class CompilationResult
  {
    /// <summary>Successfully parsed constraints.</summary>
    public IReadOnlyList<ParsedConstraint> Constraints { get; init; }
        = Array.Empty<ParsedConstraint>();

    /// <summary>Diagnostics for expressions that could not be parsed.</summary>
    public IReadOnlyList<ConstraintDiagnostic> Diagnostics { get; init; }
        = Array.Empty<ConstraintDiagnostic>();

    /// <summary>True if all expressions parsed without errors.</summary>
    public bool IsSuccess => Diagnostics.Count == 0;

    /// <summary>
    /// Builds the two matrix pairs needed by the simplified LinearConstraintsProjector.
    ///
    /// Reduction rules applied automatically:
    ///   Equality  (==)  →  A row  (kept as-is)
    ///   LessOrEqual (&lt;=) →  C row  (kept as-is)
    ///   GreaterOrEqual (>=) →  C row negated  (-coeff · p &lt;= -rhs)
    /// </summary>
    public ProjectorMatrices ToProjectorMatrices()
    {
      var eq = Constraints.Where(c => c.Kind == ConstraintKind.Equality).ToList();

      // Both <= and >= go into the single inequality system; >= rows are negated
      var leq = Constraints
          .Where(c => c.Kind == ConstraintKind.LessOrEqual)
          .Select(c => (coeffs: c.Coefficients, rhs: c.Rhs))
          .ToList();

      var geq = Constraints
          .Where(c => c.Kind == ConstraintKind.GreaterOrEqual)
          .Select(c => (coeffs: c.Coefficients.Select(v => -v).ToArray(), rhs: -c.Rhs))
          .ToList();

      var ineq = leq.Concat(geq).ToList();

      return new ProjectorMatrices
      {
        A = eq.Count > 0 ? ToMatrix(eq.Select(c => c.Coefficients)) : null,
        B = eq.Count > 0 ? ToVector(eq.Select(c => c.Rhs)) : null,
        C = ineq.Count > 0 ? ToMatrix(ineq.Select(t => t.coeffs)) : null,
        D = ineq.Count > 0 ? ToVector(ineq.Select(t => t.rhs)) : null,
      };
    }

    /// <summary>
    /// Creates a new instance of a linear constraint projector based on the current set of constraints.
    /// </summary>
    /// <returns>A <see cref="LinearConstraintsProjector"/> that projects vectors according to the defined linear constraints.</returns>
    public LinearConstraintsProjector ToProjector()
    {
      var matrices = ToProjectorMatrices();
      return matrices.ToProjector();
    }

    /// <summary>
    /// Creates a new instance of a linear constraint projector based on the current set of constraints.
    /// </summary>
    /// <returns>A tuple of a <see cref="LinearConstraintsProjector"/> that projects vectors according to the defined linear constraints, and an error message. Either the projector is not <c>null</c>, or the error message contains a descriptive error.</returns>
    public (LinearConstraintsProjector? projector, string? errorMessage) TryConvertToProjector()
    {
      var matrices = ToProjectorMatrices();
      return matrices.TryConvertToProjector();
    }

    /// <summary>
    /// Creates a dense matrix from a sequence of row arrays.
    /// </summary>
    /// <param name="rows">The row arrays to include in the matrix.</param>
    /// <returns>A dense matrix containing the supplied rows.</returns>
    private static Matrix<double> ToMatrix(IEnumerable<double[]> rows)
        => Matrix<double>.Build.DenseOfRowArrays(rows);

    /// <summary>
    /// Creates a dense vector from a sequence of values.
    /// </summary>
    /// <param name="values">The values to include in the vector.</param>
    /// <returns>A dense vector containing the supplied values.</returns>
    private static Vector<double> ToVector(IEnumerable<double> values)
        => Vector<double>.Build.DenseOfEnumerable(values);
  }

  /// <summary>
  /// Describes why a constraint expression could not be compiled.
  /// </summary>
  public class ConstraintDiagnostic
  {
    /// <summary>
    /// Gets the expression represented as a string.
    /// </summary>
    public string Expression { get; init; } = "";

    /// <summary>
    /// Gets the message content associated with this instance.
    /// </summary>
    public string Message { get; init; } = "";

    /// <summary>
    /// Gets the severity level of the diagnostic message.
    /// </summary>
    public DiagnosticSeverity Severity { get; init; } = DiagnosticSeverity.Error;

    /// <inheritdoc />
    public override string ToString()
        => $"[{Severity}] \"{Expression}\": {Message}";
  }

  // =========================================================================
  // Compiler
  // =========================================================================

  /// <summary>
  /// Compiles C# constraint expression strings into matrices for
  /// LinearConstraintsProjector, using Roslyn syntax-tree walking only
  /// (no compilation or code execution).
  ///
  /// Supported syntax:
  ///   Operators  : +  -  *  /  unary-
  ///   Grouping   : parentheses, arbitrarily nested
  ///   Literals   : integer and floating-point numeric literals
  ///   Identifiers: parameter names, named constants
  ///   Relations  : ==   >=   &lt;=   >  (treated as >=)   &lt;  (treated as &lt;=)
  ///                !=   is rejected
  ///
  /// Linearity requirement: after constant folding, each side of the
  /// relation must reduce to a linear combination of parameters plus a
  /// constant. Multiplication of two parameter-containing sub-expressions
  /// is rejected as nonlinear.
  /// </summary>
  public class LinearConstraintsCompiler
  {
    /// <summary>
    /// Stores the parameter names in their declared order.
    /// </summary>
    private readonly string[] _paramNames;

    /// <summary>
    /// Maps each parameter name to its coefficient index.
    /// </summary>
    private readonly Dictionary<string, int> _paramIndex;

    /// <summary>
    /// Stores the named constant values that may appear in constraint expressions.
    /// </summary>
    private readonly Dictionary<string, double> _constants;

    /// <summary>
    /// Stores the implicit parameter bounds that are always included in compilation results.
    /// </summary>
    private readonly ParsedConstraint[] _implicitConstraints = Array.Empty<ParsedConstraint>();

    /// <summary>
    /// Initializes a new instance of the <see cref="LinearConstraintsCompiler"/> class.
    /// </summary>
    /// <param name="parameterNames">
    ///   Ordered list of parameter names (must be valid C# identifiers).
    /// </param>
    /// <param name="constants">
    ///   Optional named constants (e.g. {"maxVal", 5.0}).
    ///   A name may not appear in both parameterNames and constants.
    /// </param>
    public LinearConstraintsCompiler(
        IEnumerable<string> parameterNames,
        IDictionary<string, double>? constants = null)
    {
      _paramNames = parameterNames.ToArray();
      _paramIndex = _paramNames
          .Select((name, idx) => (name, idx))
          .ToDictionary(t => t.name, t => t.idx);
      _constants = constants != null
          ? new Dictionary<string, double>(constants)
          : new Dictionary<string, double>();

      var overlap = _paramIndex.Keys.Intersect(_constants.Keys).ToList();
      if (overlap.Count > 0)
        throw new ArgumentException(
            $"Names appear in both parameters and constants: {string.Join(", ", overlap)}");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinearConstraintsCompiler"/> class from a fit function.
    /// </summary>
    /// <param name="fitFunction">The fit function that provides parameter names and parameter bounds.</param>
    /// <param name="constants">
    ///   Optional named constants (e.g. {"maxVal", 5.0}).
    ///   A name may not appear in both the fit-function parameter names and constants.
    /// </param>
    /// <param name="includeSoftParameterBoundaries">
    ///   If <see langword="true"/>, soft parameter boundaries are combined with the hard parameter boundaries.
    /// </param>
    public LinearConstraintsCompiler(
        IFitFunction fitFunction,
        IDictionary<string, double>? constants = null,
        bool includeSoftParameterBoundaries = false)
        : this(GetParameterNames(fitFunction), constants)
    {
      _implicitConstraints = CreateImplicitConstraints(fitFunction, includeSoftParameterBoundaries, _paramNames);
    }

    /// <summary>
    /// Initializes a new instance of the LinearConstraintsCompiler class using the specified parameter set and optional
    /// constants.
    /// </summary>
    /// <remarks>This constructor extracts parameter names from the provided parameter set and creates implicit
    /// constraints based on those parameters.</remarks>
    /// <param name="parameters">The set of parameters that define the variables available for constraint compilation. Cannot be null.</param>
    /// <param name="constants">An optional dictionary containing constant values to be used during constraint compilation. Keys represent
    /// constant names and values represent their corresponding numeric values. May be null.</param>
    public LinearConstraintsCompiler(ParameterSet parameters,
        IDictionary<string, double>? constants = null)
        : this(parameters.Select(x => x.Name).ToArray(), constants)
    {
      _implicitConstraints = CreateImplicitConstraints(parameters);
    }

    // ---------------------------------------------------------------
    // Public entry points
    // ---------------------------------------------------------------

    /// <summary>
    /// Compiles a collection of constraint expression strings.
    /// </summary>
    /// <param name="expressions">The expressions to compile.</param>
    /// <returns>The successfully compiled constraints together with any diagnostics.</returns>
    public CompilationResult Compile(IEnumerable<string> expressions)
    {
      var constraints = new List<ParsedConstraint>();
      var diagnostics = new List<ConstraintDiagnostic>();

      foreach (var expr in expressions)
      {
        var (c, d) = CompileOne_Internal(expr);
        if (d != null) diagnostics.Add(d);
        else constraints.Add(c!);
      }

      AddImplicitConstraints(constraints);

      return new CompilationResult
      {
        Constraints = constraints,
        Diagnostics = diagnostics,
      };
    }

    /// <summary>
    /// Compiles a single expression string.
    /// </summary>
    /// <param name="expression">The expression to compile.</param>
    /// <returns>The compilation result containing either one constraint or one diagnostic.</returns>
    public CompilationResult CompileOne(string expression)
    {
      var (c, d) = CompileOne_Internal(expression);
      var constraints = new List<ParsedConstraint>();
      if (c is not null)
        constraints.Add(c);

      AddImplicitConstraints(constraints);

      return new CompilationResult
      {
        Constraints = constraints,
        Diagnostics = d != null
              ? new[] { d }
              : Array.Empty<ConstraintDiagnostic>(),
      };
    }

    // ---------------------------------------------------------------
    // Internal single-expression compiler
    // ---------------------------------------------------------------

    /// <summary>
    /// Compiles a single expression string into either a parsed constraint or a diagnostic.
    /// </summary>
    /// <param name="expression">The expression to compile.</param>
    /// <returns>A tuple containing the parsed constraint or the generated diagnostic.</returns>
    private (ParsedConstraint? constraint, ConstraintDiagnostic? diag)
        CompileOne_Internal(string expression)
    {
      // Wrap in a dummy class so Roslyn parses it as a valid expression
      var src = $"class __C {{ void __M() {{ var __r = {expression}; }} }}";
      var tree = CSharpSyntaxTree.ParseText(src);
      var root = tree.GetRoot();

      var varDecl = root.DescendantNodes()
                        .OfType<VariableDeclaratorSyntax>()
                        .FirstOrDefault();

      if (varDecl?.Initializer?.Value == null)
        return Error(expression, "Could not parse expression.");

      var topExpr = varDecl.Initializer.Value;

      if (topExpr is not BinaryExpressionSyntax rel || !IsRelational(rel.Kind()))
        return Error(expression,
            "Expression must be a relation: ==, >=, <=, >, <. " +
            $"Got: {topExpr.GetType().Name} ({topExpr}).");

      // Reject != explicitly
      if (rel.Kind() == SyntaxKind.NotEqualsExpression)
        return Error(expression, "Not-equal (!=) constraints are not supported.");

      ConstraintKind kind = rel.Kind() switch
      {
        SyntaxKind.EqualsExpression => ConstraintKind.Equality,
        SyntaxKind.GreaterThanOrEqualExpression => ConstraintKind.GreaterOrEqual,
        SyntaxKind.GreaterThanExpression => ConstraintKind.GreaterOrEqual,
        SyntaxKind.LessThanOrEqualExpression => ConstraintKind.LessOrEqual,
        SyntaxKind.LessThanExpression => ConstraintKind.LessOrEqual,
        _ => throw new InvalidOperationException("Unexpected relational operator.")
      };

      var lhsResult = WalkLinear(rel.Left, expression);
      if (lhsResult.error != null) return Error(expression, lhsResult.error);

      var rhsResult = WalkLinear(rel.Right, expression);
      if (rhsResult.error != null) return Error(expression, rhsResult.error);

      // Move everything to LHS: (LHS - RHS) [op] 0
      var coeffs = new double[_paramNames.Length];
      for (int i = 0; i < _paramNames.Length; i++)
        coeffs[i] = lhsResult.coeffs[i] - rhsResult.coeffs[i];

      double rhs = rhsResult.constant - lhsResult.constant;

      return (new ParsedConstraint
      {
        Expression = expression,
        Kind = kind,
        Coefficients = coeffs,
        Rhs = rhs,
      }, null);
    }

    // ---------------------------------------------------------------
    // Recursive linear-form extractor
    // ---------------------------------------------------------------

    /// <summary>
    /// Reduces a syntax node to a linear form when possible.
    /// </summary>
    /// <param name="node">The syntax node to analyze.</param>
    /// <param name="expr">The original expression text for context.</param>
    /// <returns>A linear form or an error description if the node is not linear.</returns>
    private LinearForm WalkLinear(SyntaxNode node, string expr)
    {
      switch (node)
      {
        case LiteralExpressionSyntax lit
            when lit.Kind() == SyntaxKind.NumericLiteralExpression:
          {
            if (!TryParseNumeric(lit.Token.ValueText, out double val))
              return LinearForm.Err($"Cannot parse numeric literal: {lit.Token.ValueText}");
            return LinearForm.Constant(val, _paramNames.Length);
          }

        case IdentifierNameSyntax id:
          {
            string name = id.Identifier.ValueText;
            if (_paramIndex.TryGetValue(name, out int idx))
              return LinearForm.Param(idx, _paramNames.Length);
            if (_constants.TryGetValue(name, out double cval))
              return LinearForm.Constant(cval, _paramNames.Length);
            return LinearForm.Err(
                $"Unknown identifier '{name}'. " +
                $"Known parameters: [{string.Join(", ", _paramNames)}]. " +
                $"Known constants: [{string.Join(", ", _constants.Keys)}].");
          }

        case PrefixUnaryExpressionSyntax unary
            when unary.Kind() == SyntaxKind.UnaryMinusExpression:
          {
            var inner = WalkLinear(unary.Operand, expr);
            return inner.error != null ? inner : inner.Negate();
          }

        case PrefixUnaryExpressionSyntax unary
              when unary.Kind() == SyntaxKind.UnaryPlusExpression:
          return WalkLinear(unary.Operand, expr);

        case ParenthesizedExpressionSyntax paren:
          return WalkLinear(paren.Expression, expr);

        case BinaryExpressionSyntax bin:
          {
            if (IsRelational(bin.Kind()))
              return LinearForm.Err("Nested relational operators are not supported.");

            var left = WalkLinear(bin.Left, expr);
            if (left.error != null) return left;
            var right = WalkLinear(bin.Right, expr);
            if (right.error != null) return right;

            return bin.Kind() switch
            {
              SyntaxKind.AddExpression =>
                  left.Add(right),

              SyntaxKind.SubtractExpression =>
                  left.Add(right.Negate()),

              SyntaxKind.MultiplyExpression =>
                  left.IsConstant ? right.Scale(left.constant) :
                  right.IsConstant ? left.Scale(right.constant) :
                  LinearForm.Err(
                      $"Nonlinear term: ({bin.Left}) * ({bin.Right}). " +
                      "Both sides contain parameters."),

              SyntaxKind.DivideExpression =>
                  !right.IsConstant
                      ? LinearForm.Err(
                          $"Nonlinear term: ({bin.Left}) / ({bin.Right}). " +
                          "Divisor contains a parameter.")
                      : Math.Abs(right.constant) < 1e-300
                          ? LinearForm.Err("Division by zero.")
                          : left.Scale(1.0 / right.constant),

              _ => LinearForm.Err($"Unsupported operator: {bin.OperatorToken}.")
            };
          }

        case CastExpressionSyntax cast:
          return WalkLinear(cast.Expression, expr);

        default:
          return LinearForm.Err(
              $"Unsupported syntax node: {node.GetType().Name} ({node}).");
      }
    }

    // ---------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------

    /// <summary>
    /// Determines whether a syntax kind represents a supported relational operator.
    /// </summary>
    /// <param name="kind">The syntax kind to inspect.</param>
    /// <returns><see langword="true"/> if the kind is relational; otherwise, <see langword="false"/>.</returns>
    private static bool IsRelational(SyntaxKind kind) => kind is
        SyntaxKind.EqualsExpression or
        SyntaxKind.NotEqualsExpression or
        SyntaxKind.GreaterThanExpression or
        SyntaxKind.GreaterThanOrEqualExpression or
        SyntaxKind.LessThanExpression or
        SyntaxKind.LessThanOrEqualExpression;

    /// <summary>
    /// Tries to parse a numeric literal token into a <see cref="double"/> value.
    /// </summary>
    /// <param name="text">The literal token text.</param>
    /// <param name="value">When this method returns, contains the parsed value if parsing succeeded.</param>
    /// <returns><see langword="true"/> if parsing succeeded; otherwise, <see langword="false"/>.</returns>
    private static bool TryParseNumeric(string text, out double value)
    {
      var clean = text.TrimEnd('f', 'F', 'd', 'D', 'm', 'M', 'l', 'L', 'u', 'U');
      return double.TryParse(clean,
          System.Globalization.NumberStyles.Any,
          System.Globalization.CultureInfo.InvariantCulture,
          out value);
    }

    /// <summary>
    /// Creates an error result for a failed compilation.
    /// </summary>
    /// <param name="expr">The expression that failed to compile.</param>
    /// <param name="msg">The diagnostic message to report.</param>
    /// <returns>A tuple containing a null constraint and the generated diagnostic.</returns>
    private static (ParsedConstraint? constraint, ConstraintDiagnostic? diag)
        Error(string expr, string msg)
        => (null, new ConstraintDiagnostic
        {
          Expression = expr,
          Message = msg,
          Severity = DiagnosticSeverity.Error,
        });

    /// <summary>
    /// Adds clones of the implicit bound constraints to the specified list.
    /// </summary>
    /// <param name="constraints">The list that receives the implicit constraints.</param>
    private void AddImplicitConstraints(List<ParsedConstraint> constraints)
    {
      foreach (var constraint in _implicitConstraints)
        constraints.Add(CloneConstraint(constraint));
    }

    /// <summary>
    /// Creates a deep copy of a parsed constraint.
    /// </summary>
    /// <param name="constraint">The constraint to clone.</param>
    /// <returns>A cloned constraint instance.</returns>
    private static ParsedConstraint CloneConstraint(ParsedConstraint constraint)
        => new ParsedConstraint
        {
          Expression = constraint.Expression,
          Kind = constraint.Kind,
          Coefficients = constraint.Coefficients.ToArray(),
          Rhs = constraint.Rhs,
        };

    /// <summary>
    /// Gets the parameter names from a fit function.
    /// </summary>
    /// <param name="fitFunction">The fit function that provides the parameter names.</param>
    /// <returns>The parameter names in declaration order.</returns>
    private static string[] GetParameterNames(IFitFunction fitFunction)
    {
      ArgumentNullException.ThrowIfNull(fitFunction);

      return Enumerable.Range(0, fitFunction.NumberOfParameters)
          .Select(fitFunction.ParameterName)
          .ToArray();
    }

    /// <summary>
    /// Creates the implicit parameter-bound constraints contributed by a fit function.
    /// </summary>
    /// <param name="fitFunction">The fit function that provides parameter boundaries.</param>
    /// <param name="includeSoftParameterBoundaries">Determines whether soft bounds are combined with hard bounds.</param>
    /// <param name="parameterNames">The ordered parameter names.</param>
    /// <returns>The implicit bound constraints.</returns>
    private static ParsedConstraint[] CreateImplicitConstraints(
        IFitFunction fitFunction,
        bool includeSoftParameterBoundaries,
        IReadOnlyList<string> parameterNames)
    {
      var lowerBounds = NormalizeBounds(
          fitFunction.GetParameterBoundariesHardLimit().LowerBounds,
          parameterNames.Count,
          "hard lower");

      var upperBounds = NormalizeBounds(
          fitFunction.GetParameterBoundariesHardLimit().UpperBounds,
          parameterNames.Count,
          "hard upper");

      if (includeSoftParameterBoundaries)
      {
        var (softLowerBounds, softUpperBounds) = fitFunction.GetParameterBoundariesSoftLimit();
        MergeLowerBounds(lowerBounds, NormalizeBounds(softLowerBounds, parameterNames.Count, "soft lower"));
        MergeUpperBounds(upperBounds, NormalizeBounds(softUpperBounds, parameterNames.Count, "soft upper"));
      }

      ValidateBounds(lowerBounds, upperBounds, parameterNames);

      var constraints = new List<ParsedConstraint>();
      for (int i = 0; i < parameterNames.Count; i++)
      {
        if (lowerBounds[i].HasValue)
          constraints.Add(CreateBoundConstraint(parameterNames[i], i, parameterNames.Count, ConstraintKind.GreaterOrEqual, lowerBounds[i].Value));

        if (upperBounds[i].HasValue)
          constraints.Add(CreateBoundConstraint(parameterNames[i], i, parameterNames.Count, ConstraintKind.LessOrEqual, upperBounds[i].Value));
      }

      return constraints.ToArray();
    }

    /// <summary>
    /// Creates the implicit parameter-bound constraints contributed by a fit function.
    /// </summary>
    /// <param name="parameters">The current parameters.</param>
    /// <returns>The implicit bound constraints.</returns>
    private static ParsedConstraint[] CreateImplicitConstraints(ParameterSet parameters)
    {
      var count = parameters.Count;
      var parameterNames = parameters.Select(p => p.Name).ToArray();

      var lower = parameters.Select(p => p.LowerBound).ToArray();

      var lowerBounds = NormalizeBounds(
          parameters.Select(p => p.LowerBound).ToArray(),
          count,
          "lower bounds");

      var upperBounds = NormalizeBounds(
          parameters.Select(p => p.UpperBound).ToArray(),
          count,
          "upper bounds");

      ValidateBounds(lowerBounds, upperBounds, parameterNames);

      var constraints = new List<ParsedConstraint>();
      for (int i = 0; i < count; i++)
      {
        if (!parameters[i].Vary)
          constraints.Add(CreateFixedConstraint(parameterNames[i], i, count, parameters[i].Parameter));

        if (lowerBounds[i].HasValue)
          constraints.Add(CreateBoundConstraint(parameterNames[i], i, count, ConstraintKind.GreaterOrEqual, lowerBounds[i].Value));

        if (upperBounds[i].HasValue)
          constraints.Add(CreateBoundConstraint(parameterNames[i], i, count, ConstraintKind.LessOrEqual, upperBounds[i].Value));
      }

      return constraints.ToArray();
    }

    /// <summary>
    /// Normalizes an optional sequence of bounds into an array of nullable values.
    /// </summary>
    /// <param name="bounds">The bounds to normalize.</param>
    /// <param name="parameterCount">The expected number of parameters.</param>
    /// <param name="kind">A textual description of the bound kind for error reporting.</param>
    /// <returns>An array of nullable bounds.</returns>
    private static double?[] NormalizeBounds(IReadOnlyList<double?>? bounds, int parameterCount, string kind)
    {
      var result = new double?[parameterCount];
      if (bounds is null)
        return result;

      if (bounds.Count != parameterCount)
        throw new ArgumentException($"The {kind} parameter bounds have length {bounds.Count}, but the fit function has {parameterCount} parameters.");

      for (int i = 0; i < parameterCount; i++)
        result[i] = bounds[i];

      return result;
    }

    /// <summary>
    /// Merges additional lower bounds into the current lower bounds using the stricter value.
    /// </summary>
    /// <param name="currentBounds">The current lower bounds.</param>
    /// <param name="additionalBounds">The additional lower bounds.</param>
    private static void MergeLowerBounds(double?[] currentBounds, double?[] additionalBounds)
    {
      for (int i = 0; i < currentBounds.Length; i++)
      {
        if (!additionalBounds[i].HasValue)
          continue;

        currentBounds[i] = currentBounds[i].HasValue
            ? Math.Max(currentBounds[i].Value, additionalBounds[i].Value)
            : additionalBounds[i];
      }
    }

    /// <summary>
    /// Merges additional upper bounds into the current upper bounds using the stricter value.
    /// </summary>
    /// <param name="currentBounds">The current upper bounds.</param>
    /// <param name="additionalBounds">The additional upper bounds.</param>
    private static void MergeUpperBounds(double?[] currentBounds, double?[] additionalBounds)
    {
      for (int i = 0; i < currentBounds.Length; i++)
      {
        if (!additionalBounds[i].HasValue)
          continue;

        currentBounds[i] = currentBounds[i].HasValue
            ? Math.Min(currentBounds[i].Value, additionalBounds[i].Value)
            : additionalBounds[i];
      }
    }

    /// <summary>
    /// Validates that all effective bounds are internally consistent.
    /// </summary>
    /// <param name="lowerBounds">The effective lower bounds.</param>
    /// <param name="upperBounds">The effective upper bounds.</param>
    /// <param name="parameterNames">The ordered parameter names.</param>
    private static void ValidateBounds(double?[] lowerBounds, double?[] upperBounds, IReadOnlyList<string> parameterNames)
    {
      for (int i = 0; i < parameterNames.Count; i++)
      {
        if (lowerBounds[i].HasValue && upperBounds[i].HasValue && lowerBounds[i].Value > upperBounds[i].Value)
          throw new ArgumentException($"Parameter '{parameterNames[i]}' has inconsistent lower and upper bounds.");
      }
    }

    /// <summary>
    /// Creates a parsed constraint for a single parameter bound.
    /// </summary>
    /// <param name="parameterName">The parameter name.</param>
    /// <param name="parameterIndex">The parameter index.</param>
    /// <param name="parameterCount">The total number of parameters.</param>
    /// <param name="kind">The bound kind.</param>
    /// <param name="bound">The bound value.</param>
    /// <returns>A parsed constraint representing the parameter bound.</returns>
    private static ParsedConstraint CreateBoundConstraint(
        string parameterName,
        int parameterIndex,
        int parameterCount,
        ConstraintKind kind,
        double bound)
    {
      var coefficients = new double[parameterCount];
      coefficients[parameterIndex] = 1.0;

      var op = kind == ConstraintKind.GreaterOrEqual ? ">=" : "<=";

      return new ParsedConstraint
      {
        Expression = $"{parameterName} {op} {bound.ToString("R", CultureInfo.InvariantCulture)}",
        Kind = kind,
        Coefficients = coefficients,
        Rhs = bound,
      };
    }

    /// <summary>
    /// Creates a parsed constraint for a parameter fixed to a certain value.
    /// </summary>
    /// <param name="parameterName">The parameter name.</param>
    /// <param name="parameterIndex">The parameter index.</param>
    /// <param name="parameterCount">The total number of parameters.</param>
    /// <param name="parameterValue">The fixed parameter value.</param>
    /// <returns>A parsed constraint representing the parameter bound.</returns>
    private static ParsedConstraint CreateFixedConstraint(
        string parameterName,
        int parameterIndex,
        int parameterCount,
        double parameterValue)
    {
      var coefficients = new double[parameterCount];
      coefficients[parameterIndex] = 1.0;

      var op = "==";

      return new ParsedConstraint
      {
        Expression = $"{parameterName} {op} {parameterValue.ToString("R", CultureInfo.InvariantCulture)}",
        Kind = ConstraintKind.Equality,
        Coefficients = coefficients,
        Rhs = parameterValue,
      };
    }
  }

  // =========================================================================
  // Internal linear form: sum_i coeffs[i]*p_i + constant
  // =========================================================================

  /// <summary>
  /// Represents a linear expression of the form Σ(coeffs[i] * p_i) + constant.
  /// </summary>
  internal class LinearForm
  {
    /// <summary>
    /// Stores the parameter coefficients of the linear form.
    /// </summary>
    public double[] coeffs;

    /// <summary>
    /// Stores the constant term of the linear form.
    /// </summary>
    public double constant;

    /// <summary>
    /// Stores an error message when the form could not be constructed.
    /// </summary>
    public string? error;

    /// <summary>
    /// Gets a value indicating whether the form contains no parameter terms.
    /// </summary>
    public bool IsConstant => coeffs.All(c => c == 0.0);

    /// <summary>
    /// Initializes a new instance of the <see cref="LinearForm"/> class.
    /// </summary>
    /// <param name="n">The number of parameter coefficients to allocate.</param>
    private LinearForm(int n) { coeffs = new double[n]; }

    /// <summary>
    /// Creates a constant linear form.
    /// </summary>
    /// <param name="value">The constant value.</param>
    /// <param name="n">The number of parameter coefficients.</param>
    /// <returns>A linear form representing a constant.</returns>
    public static LinearForm Constant(double value, int n)
        => new LinearForm(n) { constant = value };

    /// <summary>
    /// Creates a linear form for a single parameter coefficient.
    /// </summary>
    /// <param name="idx">The parameter index that receives coefficient 1.</param>
    /// <param name="n">The number of parameter coefficients.</param>
    /// <returns>A linear form representing one parameter term.</returns>
    public static LinearForm Param(int idx, int n)
    {
      var f = new LinearForm(n);
      f.coeffs[idx] = 1.0;
      return f;
    }

    /// <summary>
    /// Creates an invalid linear form containing an error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <returns>An invalid linear form.</returns>
    public static LinearForm Err(string message)
        => new LinearForm(0) { error = message };

    /// <summary>
    /// Adds another linear form to this instance.
    /// </summary>
    /// <param name="other">The linear form to add.</param>
    /// <returns>A new linear form representing the sum.</returns>
    public LinearForm Add(LinearForm other)
    {
      var r = new LinearForm(coeffs.Length);
      for (int i = 0; i < coeffs.Length; i++)
        r.coeffs[i] = coeffs[i] + other.coeffs[i];
      r.constant = constant + other.constant;
      return r;
    }

    /// <summary>
    /// Negates this linear form.
    /// </summary>
    /// <returns>A new linear form representing the negated value.</returns>
    public LinearForm Negate()
    {
      var r = new LinearForm(coeffs.Length);
      for (int i = 0; i < coeffs.Length; i++)
        r.coeffs[i] = -coeffs[i];
      r.constant = -constant;
      return r;
    }

    /// <summary>
    /// Scales this linear form by a scalar value.
    /// </summary>
    /// <param name="scalar">The scaling factor.</param>
    /// <returns>A new linear form representing the scaled value.</returns>
    public LinearForm Scale(double scalar)
    {
      var r = new LinearForm(coeffs.Length);
      for (int i = 0; i < coeffs.Length; i++)
        r.coeffs[i] = coeffs[i] * scalar;
      r.constant = constant * scalar;
      return r;
    }
  }
}
