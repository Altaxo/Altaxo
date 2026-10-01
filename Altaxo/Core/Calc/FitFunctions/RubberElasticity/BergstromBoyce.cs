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
using System.Linq;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Main;

namespace Altaxo.Calc.FitFunctions.RubberElasticity
{
  /// <summary>
  /// Defines the loading mode, which determines how the lateral stretches arise from the principal stretch <c>lambda1</c>.
  /// </summary>
  public enum LoadingMode
  {
    /// <summary>
    /// Uniaxial loading with <c>lambda2 = lambda3 = 1 / sqrt(lambda1)</c>.
    /// </summary>
    Uniaxial,

    /// <summary>
    /// Equibiaxial loading with <c>lambda2 = lambda1</c> and <c>lambda3 = 1 / lambda1^2</c>.
    /// </summary>
    EquiBiaxial,

    /// <summary>
    /// Planar loading with <c>lambda2 = 1</c> and <c>lambda3 = 1 / lambda1</c>.
    /// </summary>
    Planar
  }

  /// <summary>
  /// Defines the parametrization of the Bergstrom-Boyce model, which determines how the material parameters are interpreted.
  /// </summary>
  public enum BergstromBoyceParametrization
  {
    /// <summary>
    /// The material parameters are interpreted as used in the PolyUMod software.
    /// </summary>
    PolyUMod,

    /// <summary>
    /// The material parameters are interpreted as used in the Ansys software.
    /// </summary>
    Ansys,

    /// <summary>
    /// The material parameters are interpreted as used in the Abaqus software.
    /// </summary>
    Abaqus
  }

  /// <summary>
  /// Bergstrom-Boyce incompressible hyperelastic model for generalized loading states.
  /// </summary>
  /// <remarks>
  /// The model evaluates the stress response as a function of the strain history using an 8-parameter constitutive description.
  /// Network A and network B may have distinct limiting chain stretches (<c>LambdaLA</c>, <c>LambdaLB</c>), which is required
  /// to represent the Ansys parametrization exactly; PolyUMod and Abaqus use a shared limiting stretch, i.e. <c>LambdaLA == LambdaLB</c>.
  /// <para>References:</para>
  /// <para>[1] J. S. Bergstrom and M. C. Boyce, “Constitutive modeling of the large strain time-dependent behavior of elastomers,” Journal of the Mechanics and Physics of Solids, vol. 46, no. 5, pp. 931–954, 1998.</para>
  /// <para>[2] E. M. Arruda and M. C. Boyce, “A three-dimensional constitutive model for the large stretch behavior of rubber elastic materials,” Journal of the Mechanics and Physics of Solids, vol. 41, no. 2, pp. 389–412, 1993.</para>
  /// <para>[3] J. S. Bergstrom and M. C. Boyce, “Constitutive modeling of the time-dependent and cyclic loading of elastomers and application to soft biological tissues,” Mechanics of Materials, vol. 33, pp. 523–530, 2001.</para>
  /// </remarks>
  [FitFunctionClass]
  public record BergstromBoyce : IFitFunction, IImmutable
  {
    /// <summary>
    /// Gets the cross-sectional area of the sample.
    /// </summary>
    public double CrossSectionArea { get; init; } = 1;

    /// <summary>
    /// Gets or sets the loading mode used to derive the lateral stretches from the principal stretch.
    /// </summary>
    public LoadingMode LoadingMode { get; init; } = LoadingMode.Uniaxial;

    /// <summary>
    /// Gets or sets the parametrization of the Bergstrom-Boyce model, which determines how the 8 raw fit parameters
    /// are interpreted and converted into the internal <see cref="BergstromBoyceParameters"/>.
    /// </summary>
    public BergstromBoyceParametrization Parametrization { get; init; } = BergstromBoyceParametrization.PolyUMod;

    /// <summary>
    /// Gets or sets whether the dependent variable is the Cauchy stress (true) or the nominal (engineering) stress (false).
    /// </summary>
    public bool DependentVariableIsCauchyStress { get; init; }

    /// <summary>
    /// Gets or sets whether to use the exact inverse Langevin function for the chain stretch normalization, or an approximation.
    /// </summary>
    public bool UseExactInverseLangevin { get; init; } = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="BergstromBoyce"/> class.
    /// </summary>
    public BergstromBoyce() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="BergstromBoyce"/> class with the specified loading mode and parametrization.
    /// </summary>
    /// <param name="mode">The loading mode to use.</param>
    /// <param name="parametrization">The parametrization to use.</param>
    public BergstromBoyce(LoadingMode mode, BergstromBoyceParametrization parametrization)
    {
      LoadingMode = mode;
      Parametrization = parametrization;
    }

    #region Serialization

    /// <summary>
    /// V0: 2026-09-18 initial version.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(BergstromBoyce), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public virtual void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (BergstromBoyce)o;
        info.AddValue("CrossSectionArea", s.CrossSectionArea);
        info.AddEnum("LoadingMode", s.LoadingMode);
        info.AddEnum("Parametrization", s.Parametrization);
        info.AddValue("DependentVariableIsCauchyStress", s.DependentVariableIsCauchyStress);
        info.AddValue("UseExactInverseLangevin", s.UseExactInverseLangevin);
      }

      /// <inheritdoc/>
      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var crossSectionArea = info.GetDouble("CrossSectionArea");
        var loadingMode = info.GetEnum<LoadingMode>("LoadingMode");
        var parametrization = info.GetEnum<BergstromBoyceParametrization>("Parametrization");
        var dependentVariableIsCauchyStress = info.GetBoolean("DependentVariableIsCauchyStress");
        var useExactInverseLangevin = info.GetBoolean("UseExactInverseLangevin");
        return new BergstromBoyce()
        {
          CrossSectionArea = crossSectionArea,
          LoadingMode = loadingMode,
          Parametrization = parametrization,
          DependentVariableIsCauchyStress = dependentVariableIsCauchyStress,
          UseExactInverseLangevin = useExactInverseLangevin
        };
      }
    }

    #endregion Serialization


    /// <summary>
    /// Creates a new instance of the fit function.
    /// </summary>
    /// <returns>A new <see cref="BergstromBoyce"/> instance for uniaxial loading.</returns>
    [FitFunctionCreator("Bergström-Boyce (uniaxial loading)", "RubberElasticity", 2, 1, 8)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.RubberElasticity.BergstromBoyce}")]
    public static IFitFunction CreateUniaxial()
    {
      return new BergstromBoyce() { LoadingMode = LoadingMode.Uniaxial, Parametrization = BergstromBoyceParametrization.PolyUMod };
    }

    /// <summary>
    /// Creates a new instance of the fit function.
    /// </summary>
    /// <returns>A new <see cref="BergstromBoyce"/> instance for biaxial loading.</returns>
    [FitFunctionCreator("Bergström-Boyce (biaxial loading)", "RubberElasticity", 2, 1, 8)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.RubberElasticity.BergstromBoyce}")]
    public static IFitFunction CreateBiaxial()
    {
      return new BergstromBoyce() { LoadingMode = LoadingMode.EquiBiaxial, Parametrization = BergstromBoyceParametrization.PolyUMod };
    }

    /// <summary>
    /// Creates a new instance of the fit function.
    /// </summary>
    /// <returns>A new <see cref="BergstromBoyce"/> instance for biaxial loading.</returns>
    [FitFunctionCreator("Bergström-Boyce (planar loading)", "RubberElasticity", 2, 1, 8)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.RubberElasticity.BergstromBoyce}")]
    public static IFitFunction CreatePlanar()
    {
      return new BergstromBoyce() { LoadingMode = LoadingMode.Planar, Parametrization = BergstromBoyceParametrization.PolyUMod };
    }



    /// <inheritdoc/>
    public int NumberOfIndependentVariables => 2;

    /// <inheritdoc/>
    public int NumberOfDependentVariables => 1;

    /// <inheritdoc/>
    public int NumberOfParameters => Parametrization switch
    {
      BergstromBoyceParametrization.Ansys => 7,
      BergstromBoyceParametrization.Abaqus => 7,
      _ => 8, // PolyUMod
    };

    /// <inheritdoc/>
    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    /// <inheritdoc/>
    public string IndependentVariableName(int i)
    {
      return i switch
      {
        0 => "Time",
        1 => "EngineeringStrain",
        _ => throw new ArgumentOutOfRangeException(nameof(i), $"Independent variable index {i} is out of range.")
      };
    }

    /// <inheritdoc/>
    public string DependentVariableName(int i)
    {
      return i switch
      {
        0 => DependentVariableIsCauchyStress ? "CauchyStress" : "EngineeringStress",
        _ => throw new ArgumentOutOfRangeException(nameof(i), $"Dependent variable index {i} is out of range.")
      };
    }

    /// <summary>
    /// Gets the name of the raw fit parameter at index <paramref name="i"/>. The meaning of some slots depends on
    /// <see cref="Parametrization"/> - see the remarks on <see cref="GetEffectiveParameters(IReadOnlyList{double})"/>.
    /// </summary>
    /// <inheritdoc/>
    public string ParameterName(int i)
    {
      return Parametrization switch
      {
        BergstromBoyceParametrization.Ansys => i switch
        {
          0 => "MuA",
          1 => "LambdaLA",
          2 => "MuB",
          3 => "LambdaLB",
          4 => "C",
          5 => "TauHat",
          6 => "M",
          _ => throw new ArgumentOutOfRangeException(nameof(i), $"Parameter index {i} is out of range.")
        },
        BergstromBoyceParametrization.Abaqus => i switch
        {
          0 => "Mu",
          1 => "LambdaL",
          2 => "S",
          3 => "E",
          4 => "C",
          5 => "A",
          6 => "M",
          _ => throw new ArgumentOutOfRangeException(nameof(i), $"Parameter index {i} is out of range.")
        },
        _ => i switch // PolyUMod
        {
          0 => "Mu", // Shear modulus of network A
          1 => "LambdaL", // Limiting chain stretch, shared by network A and B
          2 => "S", // Ratio of the shear modulus of network B to network A
          3 => "Xi", // Offset for the creep term in the flow rate of network B
          4 => "C",
          5 => "TauBase", // Base stress for the flow rate of network B (flow resistance)
          6 => "M", // Exponent for the flow rate of network B (flow resistance)
          7 => "TauCut", // Cut-off stress for the flow rate of network B (flow resistance)
          _ => throw new ArgumentOutOfRangeException(nameof(i), $"Parameter index {i} is out of range.")
        },
      };
    }

    /// <inheritdoc/>
    public double DefaultParameterValue(int i)
    {
      return Parametrization switch
      {
        BergstromBoyceParametrization.Ansys => i switch
        {
          0 => 1E6, // MuA
          1 => 3.0,  // LambdaLA
          2 => 1E6, // MuB
          3 => 3.0, // LambdaLB
          4 => -1, // C
          5 => 2E5, // TauHat
          6 => 3, // M
          _ => throw new ArgumentOutOfRangeException(nameof(i), $"Parameter index {i} is out of range.")
        },
        BergstromBoyceParametrization.Abaqus => i switch
        {
          0 => 1E6, // Mu
          1 => 3.58,  // LambdaL
          2 => 1.0, // S
          3 => 0.01, // E
          4 => -1, // C
          5 => 1.0, // A (units depend on M!)
          6 => 3, // M
          _ => throw new ArgumentOutOfRangeException(nameof(i), $"Parameter index {i} is out of range.")
        },
        _ => i switch // PolyUMod
        {
          0 => 1E6, // Mu
          1 => 3.58,  // LambdaL
          2 => 1.0, // S
          3 => 0.05, // Xi
          4 => -1, // C
          5 => 2E5, // TauBase, 200 kPa
          6 => 3, // M
          7 => 0, // TauCut
          _ => throw new ArgumentOutOfRangeException(nameof(i), $"Parameter index {i} is out of range.")
        },
      };
    }



    /// <summary>
    /// Stores the internal (PolyUMod-style) material parameters used to evaluate the Bergstrom-Boyce model,
    /// after conversion from the raw fit parameters according to <see cref="BergstromBoyceParametrization"/>.
    /// </summary>
    public struct BergstromBoyceParameters
    {
      /// <summary>
      /// Gets or sets the shear modulus of network A in pascals.
      /// </summary>
      public double Mu;

      /// <summary>
      /// Gets or sets the limiting chain stretch of network A.
      /// </summary>
      public double LambdaLA;

      /// <summary>
      /// Gets or sets the limiting chain stretch of network B. Equal to <see cref="LambdaLA"/> for the
      /// PolyUMod and Abaqus parametrizations (shared hyperelastic potential); may differ for Ansys.
      /// </summary>
      public double LambdaLB;

      /// <summary>
      /// Gets or sets the relative stiffness of network B, where <c>mu_B = S * Mu</c>.
      /// </summary>
      public double S;

      /// <summary>
      /// Gets or sets the regularization near <c>lambdaBv = 1</c>.
      /// </summary>
      public double Xi;

      /// <summary>
      /// Gets or sets the strain exponent in the flow law.
      /// </summary>
      public double C;

      /// <summary>
      /// Gets or sets the flow resistance in pascals.
      /// </summary>
      public double TauBase;

      /// <summary>
      /// Gets or sets the stress exponent in the flow law.
      /// </summary>
      public double M;

      /// <summary>
      /// Gets or sets the normalized threshold stress for flow.
      /// </summary>
      public double TauCut;
    }

    /// <summary>
    /// Converts the raw fit parameters (7 for Ansys/Abaqus, 8 for PolyUMod - see <see cref="NumberOfParameters"/>)
    /// into the internal <see cref="BergstromBoyceParameters"/> and the two inverse-Langevin normalization
    /// constants <c>l0A</c>, <c>l0B</c>, according to <see cref="Parametrization"/>.
    /// </summary>
    /// <remarks>
    /// <para><b>PolyUMod</b> (8 parameters): slots map 1:1 to Mu, LambdaL (shared), S, Xi, C, TauBase, M, TauCut.</para>
    /// <para><b>Ansys</b> (7 parameters: MuA, LambdaLA, MuB, LambdaLB, C, TauHat, M - no Xi, no TauCut):
    /// slot 2 holds the raw shear modulus MuB (not the ratio S - it is divided by Mu here), slot 3 holds the
    /// separate LambdaLB.</para>
    /// <para><b>Abaqus</b> (7 parameters: Mu, LambdaL (shared), S, E, C, A, M - no TauCut):
    /// slot 3 holds Abaqus' regularization constant E (= Xi), slot 5 holds the raw Abaqus creep constant A,
    /// which is converted to TauBase = A^(-1/M).</para>
    /// </remarks>
    /// <param name="parameters">The raw fit parameters, as many as <see cref="NumberOfParameters"/> indicates.</param>
    /// <returns>The internal material parameters and the corresponding l0A, l0B normalization constants.</returns>
    private (BergstromBoyceParameters p, double l0A, double l0B) GetEffectiveParameters(IReadOnlyList<double> parameters)
    {
      BergstromBoyceParameters p;
      const double DefaultRegularization = 1e-3;

      switch (Parametrization)
      {
        case BergstromBoyceParametrization.Ansys:
          {
            double muA = parameters[0];
            double lambdaLA = parameters[1];
            double muB = parameters[2];      // raw shear modulus of network B (Ansys has no ratio parameter)
            double lambdaLB = parameters[3]; // separate limiting stretch of network B
            double c = parameters[4];
            double tauHat = parameters[5];
            double m = parameters[6];

            p = new BergstromBoyceParameters
            {
              Mu = muA,
              LambdaLA = lambdaLA,
              LambdaLB = lambdaLB,
              S = muB / muA,
              Xi = DefaultRegularization,
              C = c,
              TauBase = tauHat,
              M = m,
              TauCut = 0.0,
            };
            break;
          }

        case BergstromBoyceParametrization.Abaqus:
          {
            double mu = parameters[0];
            double lambdaL = parameters[1];
            double s = parameters[2];
            double e = parameters[3];  // Abaqus regularization constant E, plays the role of Xi
            double c = parameters[4];
            double a = parameters[5];  // raw Abaqus creep constant A, units (Pa)^-M (s)^-1
            double m = parameters[6];

            p = new BergstromBoyceParameters
            {
              Mu = mu,
              LambdaLA = lambdaL,
              LambdaLB = lambdaL,
              S = s,
              Xi = e,
              C = c,
              TauBase = Math.Pow(a, -1.0 / m), // tauHat = A^(-1/m)
              M = m,
              TauCut = 0.0,
            };
            break;
          }

        default: // PolyUMod
          p = new BergstromBoyceParameters
          {
            Mu = parameters[0],
            LambdaLA = parameters[1],
            LambdaLB = parameters[1],
            S = parameters[2],
            Xi = parameters[3],
            C = parameters[4],
            TauBase = parameters[5],
            M = parameters[6],
            TauCut = parameters[7],
          };
          break;
      }

      double l0A = Hyperbolic.InverseLangevin(1.0 / p.LambdaLA);
      double l0B = Hyperbolic.InverseLangevin(1.0 / p.LambdaLB);
      return (p, l0A, l0B);
    }

    /// <summary>
    /// Computes the lateral stretches (2,3) from the principal stretch <paramref name="l1"/>, depending on the loading mode.
    /// </summary>
    /// <param name="l1">The principal stretch in the loading direction.</param>
    /// <returns>The lateral stretches <c>(l2, l3)</c>.</returns>
    protected (double l2, double l3) LateralStretches(double l1) => LoadingMode switch
    {
      LoadingMode.Uniaxial => (1.0 / Math.Sqrt(l1), 1.0 / Math.Sqrt(l1)),
      LoadingMode.EquiBiaxial => (l1, 1.0 / (l1 * l1)),
      LoadingMode.Planar => (1.0, 1.0 / l1),
      _ => throw new NotSupportedException($"Unknown LoadingMode: {LoadingMode}")
    };

    /// <summary>
    /// Computes the chain stretch of the 8-chain network from the full stretch triplet.
    /// </summary>
    /// <param name="l1">The principal stretch in direction 1.</param>
    /// <param name="l2">The principal stretch in direction 2.</param>
    /// <param name="l3">The principal stretch in direction 3.</param>
    /// <returns>The chain stretch.</returns>
    private static double ChainStretch(double l1, double l2, double l3) =>
        Math.Sqrt((l1 * l1 + l2 * l2 + l3 * l3) / 3.0);

    /// <summary>
    /// Computes the true (Cauchy) stress difference of network A, <c>sigma_1 - sigma_3</c> with <c>sigma_3 = 0</c> in the free/thin direction.
    /// </summary>
    /// <param name="lambda1">The current principal stretch in direction 1.</param>
    /// <param name="p">The material parameters.</param>
    /// <param name="l0A">The inverse Langevin term for the limiting stretch of network A, <c>LambdaLA</c>.</param>
    /// <returns>The true Cauchy stress difference of network A.</returns>
    internal double NetworkAStress(double lambda1, BergstromBoyceParameters p, double l0A)
    {
      var (l2, l3) = LateralStretches(lambda1);
      double lc = ChainStretch(lambda1, l2, l3);
      double factor = (p.Mu / lc) * (Hyperbolic.InverseLangevin(lc / p.LambdaLA) / l0A);
      return factor * (lambda1 * lambda1 - l3 * l3);
    }

    /// <summary>
    /// Computes the principal (Cauchy) stresses of network B for its elastic principal stretch <paramref name="lambda1Be"/>.
    /// </summary>
    /// <param name="lambda1Be">The elastic principal stretch in direction 1.</param>
    /// <param name="p">The material parameters.</param>
    /// <param name="l0B">The inverse Langevin term for the limiting stretch of network B, <c>LambdaLB</c>.</param>
    /// <returns>The principal Cauchy stress components <c>(s1, s2, s3)</c>.</returns>
    private (double s1, double s2, double s3) NetworkBPrincipalStresses(double lambda1Be, BergstromBoyceParameters p, double l0B)
    {
      var (l2, l3) = LateralStretches(lambda1Be);
      double lc = ChainStretch(lambda1Be, l2, l3);
      double factor = (p.S * p.Mu / lc) * (Hyperbolic.InverseLangevin(lc / p.LambdaLB) / l0B);
      double lBar2 = lc * lc; // = (l1^2 + l2^2 + l3^2) / 3
      return (
          factor * (lambda1Be * lambda1Be - lBar2),
          factor * (l2 * l2 - lBar2),
          factor * (l3 * l3 - lBar2));
    }

    /// <summary>
    /// Computes the observable (Cauchy) stress difference of network B, <c>sigma_1 - sigma_3</c> with the free direction set to zero.
    /// </summary>
    /// <param name="lambda1Be">The elastic principal stretch in direction 1.</param>
    /// <param name="p">The material parameters.</param>
    /// <param name="l0B">The inverse Langevin term for the limiting stretch of network B, <c>LambdaLB</c>.</param>
    /// <returns>The observable Cauchy stress difference of network B.</returns>
    internal double NetworkBStress(double lambda1Be, BergstromBoyceParameters p, double l0B)
    {
      var (s1, _, s3) = NetworkBPrincipalStresses(lambda1Be, p, l0B);
      return s1 - s3;
    }

    /// <summary>
    /// Computes the flow rate of network B using the PolyUMod evolution law.
    /// </summary>
    /// <param name="lambdaBv">The viscous stretch of network B.</param>
    /// <param name="tau">The full Frobenius norm of the deviatoric principal stresses of network B.</param>
    /// <param name="p">The material parameters.</param>
    /// <returns>The flow rate.</returns>
    /// <remarks>Does not depend on l0A or l0B: the flow rule uses <c>(lambdaBvChain - 1 + Xi)</c> directly, without inverse-Langevin normalization.</remarks>
    internal double FlowRate(double lambdaBv, double tau, BergstromBoyceParameters p)
    {
      var (l2v, l3v) = LateralStretches(lambdaBv);
      double lambdaBvChain = ChainStretch(lambdaBv, l2v, l3v);

      double creepTerm = Math.Pow(Math.Max(lambdaBvChain - 1.0 + p.Xi, 1e-12), p.C);
      double stressRatio = tau / p.TauBase - p.TauCut;
      double rampTerm = stressRatio > 0.0 ? Math.Pow(stressRatio, p.M) : 0.0;

      return creepTerm * rampTerm; // [1/s]; gammaDot0 = 1 (1/s) already included
    }

    /// <summary>
    /// Computes the rate of change of <paramref name="lambdaBv"/> in direction 1 for the current total stretch and state.
    /// </summary>
    /// <param name="lambda1">The current total stretch in direction 1.</param>
    /// <param name="lambdaBv">The current viscous stretch of network B.</param>
    /// <param name="p">The material parameters.</param>
    /// <param name="l0B">The inverse Langevin term for the limiting stretch of network B, <c>LambdaLB</c>.</param>
    /// <returns>The rate of change of the viscous stretch.</returns>
    private double LambdaBvRate(double lambda1, double lambdaBv, BergstromBoyceParameters p, double l0B)
    {
      double lambda1Be = lambda1 / lambdaBv;
      var (s1, s2, s3) = NetworkBPrincipalStresses(lambda1Be, p, l0B);
      double tau = Math.Sqrt(s1 * s1 + s2 * s2 + s3 * s3);
      double gammaDot = FlowRate(lambdaBv, tau, p);
      double n1 = tau > 1e-12 ? s1 / tau : 0.0;
      return lambdaBv * gammaDot * n1;
    }



    /// <summary>
    /// Right-hand side of the ODE, <c>dy/dt = F(t, y)</c>, for an external solver.
    /// </summary>
    /// <remarks>
    /// <c>y[0] = lambdaBv</c> (viscous stretch of network B). The array has one element because the model has only one internal state variable.
    /// Only network B's normalization constant is needed here, since the state evolution depends only on network B.
    /// </remarks>
    /// <param name="t">The current time.</param>
    /// <param name="y">The state vector.</param>
    /// <param name="lambdaOfT">A function returning the imposed total stretch <c>lambda(t)</c>.</param>
    /// <param name="p">The material parameters.</param>
    /// <param name="l0B">The inverse Langevin term for the limiting stretch of network B, <c>LambdaLB</c>.</param>
    /// <returns>The derivative vector.</returns>
    public double[] Derivative(double t, double[] y, Func<double, double> lambdaOfT, BergstromBoyceParameters p, double l0B)
    {
      double lambdaBv = Math.Max(y[0], 1e-6); // Prevent non-positive values.
      double lambda = lambdaOfT(t);
      return new[] { LambdaBvRate(lambda, lambdaBv, p, l0B) };
    }

    /// <summary>
    /// Computes the numerical Jacobian <c>dF/dy</c> using central differences when an implicit solver requires a Jacobian matrix.
    /// </summary>
    /// <param name="t">The current time.</param>
    /// <param name="y">The state vector.</param>
    /// <param name="lambdaOfT">A function returning the imposed total stretch <c>lambda(t)</c>.</param>
    /// <param name="p">The material parameters.</param>
    /// <param name="l0B">The inverse Langevin term for the limiting stretch of network B, <c>LambdaLB</c>.</param>
    /// <param name="h">The finite-difference step size.</param>
    /// <returns>The 1x1 Jacobian matrix.</returns>
    public double[,] NumericalJacobian(double t, double[] y, Func<double, double> lambdaOfT, BergstromBoyceParameters p, double l0B, double h = 1e-6)
    {
      double yPlus = y[0] + h;
      double yMinus = Math.Max(y[0] - h, 1e-9);

      double fPlus = Derivative(t, new[] { yPlus }, lambdaOfT, p, l0B)[0];
      double fMinus = Derivative(t, new[] { yMinus }, lambdaOfT, p, l0B)[0];

      return new[,] { { (fPlus - fMinus) / (yPlus - yMinus) } };
    }

    /// <summary>
    /// Computes the algebraic auxiliary quantity <c>sigma(t) = sigma_A(lambda(t)) + sigma_B(t, y)</c> after the solver provides the state vector <paramref name="y"/>.
    /// </summary>
    /// <param name="t">The current time.</param>
    /// <param name="y">The state vector.</param>
    /// <param name="lambdaOfT">A function returning the imposed total stretch <c>lambda(t)</c>.</param>
    /// <param name="p">The material parameters.</param>
    /// <param name="l0A">The inverse Langevin term for the limiting stretch of network A, <c>LambdaLA</c>.</param>
    /// <param name="l0B">The inverse Langevin term for the limiting stretch of network B, <c>LambdaLB</c>.</param>
    /// <returns>The total Cauchy stress corresponding to the given state.</returns>
    public double StressFromState(double t, double[] y, Func<double, double> lambdaOfT, BergstromBoyceParameters p, double l0A, double l0B)
    {
      double lambda = lambdaOfT(t);
      double lambdaBe = lambda / Math.Max(y[0], 1e-6);
      return NetworkAStress(lambda, p, l0A) + NetworkBStress(lambdaBe, p, l0B);
    }

    /// <summary>One classic RK4 step with a precomputed k1 = rate(lambda0, y).</summary>
    /// <param name="lambda0" >The stretch at the beginning of the time step.</param>
    /// <param name="lambda1" >The stretch at the end of the time step.</param>
    /// <param name="dt">The time step.</param>
    /// <param name="y">The current value of the state variable.</param>
    /// <param name="k1">The precomputed rate at the beginning of the time step.</param>
    /// <param name="p">The material parameters.</param>
    /// <param name="l0B">The inverse Langevin term for the limiting stretch of network B, <c>LambdaLB</c>.</param>
    /// <returns>The updated value of the state variable after one RK4 step.</returns>
    private double Rk4(double lambda0, double lambda1, double dt, double y, double k1,
      BergstromBoyceParameters p, double l0B)
    {
      double lamMid = 0.5 * (lambda0 + lambda1);
      double k2 = LambdaBvRate(lamMid, y + 0.5 * dt * k1, p, l0B);
      double k3 = LambdaBvRate(lamMid, y + 0.5 * dt * k2, p, l0B);
      double k4 = LambdaBvRate(lambda1, y + dt * k3, p, l0B);

      return y + (dt / 6.0) * (k1 + 2 * k2 + 2 * k3 + k4);
    }

    const double RelativeTolerance = 1e-6;
    const double AbsoluteTolerance = 1e-12;
    const int MaximumRecursionDepth = 10;

    /// <summary>
    /// Advances the internal state <paramref name="y"/> over a time step <paramref name="dt"/> using an adaptive Runge-Kutta RK4 method with Richardson extrapolation.
    /// </summary>
    /// <param name="lambda0">The stretch at the beginning of the time step.</param>
    /// <param name="lambda1">The stretch at the end of the time step.</param>
    /// <param name="dt">The time step.</param>
    /// <param name="y">The current value of the state variable.</param>
    /// <param name="k1">The precomputed rate at the beginning of the time step.</param>
    /// <param name="p">The material parameters.</param>
    /// <param name="l0B">The inverse Langevin term for the limiting stretch of network B, <c>LambdaLB</c>.</param>
    /// <param name="depth">The current recursion depth for the adaptive step.</param>
    /// <returns>The updated value of the state variable after one adaptive RK4 step.</returns>
    private double LambdaBv_AdvanceAdaptive(double lambda0, double lambda1, double dt, double y, double k1,
      BergstromBoyceParameters p, double l0B, int depth)
    {
      double lamMid = 0.5 * (lambda0 + lambda1);
      double h = 0.5 * dt;

      // One full step (3 new evaluations, k1 is given)
      double yFull = Rk4(lambda0, lambda1, dt, y, k1, p, l0B);

      // Two half steps: the first reuses k1 (3 new evaluations),
      // the second needs its own k1 (4 new evaluations)
      double yMid = Rk4(lambda0, lamMid, h, y, k1, p, l0B);
      double k1Mid = LambdaBvRate(lamMid, yMid, p, l0B);
      double yTwo = Rk4(lamMid, lambda1, h, yMid, k1Mid, p, l0B);

      double diff = yTwo - yFull;
      double err = Math.Abs(diff) / 15.0;
      double tol = RelativeTolerance * Math.Max(Math.Abs(yTwo), AbsoluteTolerance);

      // NaN/Inf makes the comparison false, so it forces bisection
      if (err <= tol || depth >= MaximumRecursionDepth)
        return yTwo + diff / 15.0; // Richardson extrapolation (5th order)

      // Reject: bisect, reusing k1 for the left half
      double yLeft = LambdaBv_AdvanceAdaptive(lambda0, lamMid, h, y, k1, p, l0B, depth + 1);
      double k1Right = LambdaBvRate(lamMid, yLeft, p, l0B);
      return LambdaBv_AdvanceAdaptive(lamMid, lambda1, h, yLeft, k1Right, p, l0B, depth + 1);
    }

    /// <summary>
    /// Integrates the internal state over a time step <paramref name="dt"/> using Runge-Kutta RK4, where the stretch is linearly interpolated between <paramref name="lambda0"/> and <paramref name="lambda1"/>.
    /// </summary>
    /// <param name="lambda0">The stretch at the beginning of the time step.</param>
    /// <param name="lambda1">The stretch at the end of the time step.</param>
    /// <param name="dt">The time step.</param>
    /// <param name="LambdaBv">The viscous stretch of network B, updated in place.</param>
    /// <param name="p">The material parameters.</param>
    /// <param name="l0A">The inverse Langevin term for the limiting stretch of network A, <c>LambdaLA</c>.</param>
    /// <param name="l0B">The inverse Langevin term for the limiting stretch of network B, <c>LambdaLB</c>.</param>
    /// <returns>The total true Cauchy stress at <paramref name="lambda1"/>.</returns>
    public double RungeKuttaStep(double lambda0, double lambda1, double dt, ref double LambdaBv,
      BergstromBoyceParameters p, double l0A, double l0B)
    {
      double k1 = LambdaBvRate(lambda0, LambdaBv, p, l0B);
      LambdaBv = LambdaBv_AdvanceAdaptive(lambda0, lambda1, dt, LambdaBv, k1, p, l0B, 0);
      LambdaBv = Math.Max(LambdaBv, 1e-6);

      double lambdaBe = lambda1 / LambdaBv;
      return NetworkAStress(lambda1, p, l0A) + NetworkBStress(lambdaBe, p, l0B);
    }

    /// <summary>
    /// Computes the true uniaxial stress response for a prescribed strain history <c>lambda(t)</c> evaluated at the time points in <paramref name="time"/>.
    /// </summary>
    /// <param name="timeLambda">Enumeration of (time, lambda) points. The time must be strictly increasing.</param>
    /// <param name="p">The material parameters.</param>
    /// <returns>The course of the Cauchy stress for the given lambda versus time course.</returns>
    public IEnumerable<(double time, double lambda, double cauchyStress, int i)> EvaluateCauchyStress(IEnumerable<(double time, double lambda)> timeLambda, BergstromBoyceParameters p)
    {
      var l0A = Hyperbolic.InverseLangevin(1.0 / p.LambdaLA);
      var l0B = Hyperbolic.InverseLangevin(1.0 / p.LambdaLB);

      double lambdaBv = 1.0; // Initial viscous stretch of network B (at t=0, lambdaBv = 1)

      var (previousTime, previousLambda) = timeLambda.First();


      yield return (previousTime, previousLambda, NetworkAStress(previousLambda, p, l0A) + NetworkBStress(previousLambda / lambdaBv, p, l0B), 0);

      int index = 1;
      foreach (var (currentTime, currentLambda) in timeLambda.Skip(1))
      {
        double dt = currentTime - previousTime;
        if (!(dt >= 0))
        {
          throw new ArgumentException($"Time must be strictly increasing. Found previous time {previousTime} and current time {currentTime}.");
        }

        double stress = RungeKuttaStep(previousLambda, currentLambda, dt, ref lambdaBv, p, l0A, l0B);
        yield return (currentTime, currentLambda, stress, index);
        previousTime = currentTime;
        previousLambda = currentLambda;
        ++index;
      }
    }

    /// <inheritdoc/>
    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> parameters, IVector<double> dependent, IReadOnlyList<bool>? dependentVariableChoice)
    {
      var (p, l0A, l0B) = GetEffectiveParameters(parameters);

      var timeLambda = Enumerable.Range(0, independent.RowCount).Select(i => (time: independent[i, 0], lambda: 1.0 + independent[i, 1]));

      foreach (var point in EvaluateCauchyStress(timeLambda, p))
      {
        dependent[point.i] = DependentVariableIsCauchyStress ? point.cauchyStress : CrossSectionArea * point.cauchyStress / point.lambda;
      }
    }

    /// <inheritdoc/>
    public void Evaluate(ReadOnlySpan<double> independent, ReadOnlySpan<double> parameters, Span<double> dependent)
    {
      throw new InvalidOperationException("Unable to evaluate one point of this model since the complete strain history is needed.");
    }


    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesHardLimit()
    {
      return (new double?[] { 0, 1 }, null);
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesSoftLimit()
    {
      return (new double?[] { 0, 1 }, null);
    }
  }
}
