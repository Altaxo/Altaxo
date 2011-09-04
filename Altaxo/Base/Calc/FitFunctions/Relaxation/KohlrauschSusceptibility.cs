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
#endregion

using System;
using System.ComponentModel;
using Altaxo.Calc.Regression.Nonlinear;
namespace Altaxo.Calc.FitFunctions.Relaxation
{
	/// <summary>
	/// Kohlrausch function in the frequency domain to fit compliance or dielectric spectra.
	/// </summary>
	[FitFunctionClass]
	public class KohlrauschSusceptibility : IFitFunction
	{
		bool _useFrequencyInsteadOmega;
		bool _useFlowTerm;
		bool _isDielectricData;
		bool _invertViscosity = true;
		int _numberOfRelaxations = 1;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(KohlrauschSusceptibility), 0)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				KohlrauschSusceptibility s = (KohlrauschSusceptibility)obj;
				info.AddValue("UseFrequency", s._useFrequencyInsteadOmega);
				info.AddValue("FlowTerm", s._useFlowTerm);
				info.AddValue("IsDielectric", s._isDielectricData);
			}

			public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				KohlrauschSusceptibility s = o != null ? (KohlrauschSusceptibility)o : new KohlrauschSusceptibility();
				s._useFrequencyInsteadOmega = info.GetBoolean("UseFrequency");
				s._useFlowTerm = info.GetBoolean("FlowTerm");
				s._isDielectricData = info.GetBoolean("IsDielectric");
				return s;
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(KohlrauschSusceptibility), 1)]
		class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				KohlrauschSusceptibility s = (KohlrauschSusceptibility)obj;
				info.AddValue("UseFrequency", s._useFrequencyInsteadOmega);
				info.AddValue("FlowTerm", s._useFlowTerm);
				info.AddValue("IsDielectric", s._isDielectricData);
				info.AddValue("InvertViscosity", s._invertViscosity);
				info.AddValue("NumberOfRelaxations", s._numberOfRelaxations);
			}

			public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				KohlrauschSusceptibility s = o != null ? (KohlrauschSusceptibility)o : new KohlrauschSusceptibility();
				s._useFrequencyInsteadOmega = info.GetBoolean("UseFrequency");
				s._useFlowTerm = info.GetBoolean("FlowTerm");
				s._isDielectricData = info.GetBoolean("IsDielectric");
				s._invertViscosity = info.GetBoolean("InvertViscosity");
				s._numberOfRelaxations = info.GetInt32("NumberOfRelaxations");
				return s;
			}
		}

		#endregion


		public bool UseFrequencyInsteadOmega
		{
			get { return _useFrequencyInsteadOmega; }
			set { _useFrequencyInsteadOmega = value; }
		}

		public bool UseFlowTerm
		{
			get { return _useFlowTerm; }
			set { _useFlowTerm = value; }
		}

		public bool IsDielectricData
		{
			get { return _isDielectricData; }
			set { _isDielectricData = value; }
		}

		public bool InvertViscosity
		{
			get { return _invertViscosity; }
			set { _invertViscosity = value; }
		}

		public int NumberOfRelaxations
		{
			get { return _numberOfRelaxations; }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException("NumberOfRelaxations has to be a positive number");
				_numberOfRelaxations = value;
			}
		}

		public KohlrauschSusceptibility()
		{

		}

		public override string ToString()
		{
			return "Kohlrausch Complex " + (_useFrequencyInsteadOmega ? "(Freq)" : "(Omeg)");
		}

		[FitFunctionCreator("Kohlrausch Complex (Omega)", "Retardation/General", 1, 2, 4)]
		[Description("FitFunctions.Relaxation.Susceptibility.Introduction;XML.MML.GenericSusceptibility;FitFunctions.Relaxation.KohlrauschSusceptibility.Part2;XML.MML.KohlrauschTimeDomain;FitFunctions.IndependentVariable.Omega;FitFunctions.Relaxation.KohlrauschSusceptibility.Part3")]
		public static IFitFunction CreateGeneralFunctionOfOmega()
		{
			KohlrauschSusceptibility result = new KohlrauschSusceptibility();
			result._useFrequencyInsteadOmega = false;
			result._useFlowTerm = true;
			result._isDielectricData = true;
			return result;
		}

		[FitFunctionCreator("Kohlrausch Complex (Freq)", "Retardation/General", 1, 2, 4)]
		[Description("FitFunctions.Relaxation.Susceptibility.Introduction;XML.MML.GenericSusceptibility;FitFunctions.Relaxation.KohlrauschSusceptibility.Part2;XML.MML.KohlrauschTimeDomain;FitFunctions.IndependentVariable.FrequencyAsOmega;FitFunctions.Relaxation.KohlrauschSusceptibility.Part3")]
		public static IFitFunction CreateGeneralFunctionOfFrequency()
		{
			KohlrauschSusceptibility result = new KohlrauschSusceptibility();
			result._useFrequencyInsteadOmega = true;
			result._useFlowTerm = true;
			result._isDielectricData = true;
			return result;
		}

		[FitFunctionCreator("Kohlrausch Complex (Omega)", "Retardation/Dielectrics", 1, 2, 4)]
		[Description("FitFunctions.Relaxation.DielectricSusceptibility.Introduction;XML.MML.GenericDielectricSusceptibility;FitFunctions.Relaxation.KohlrauschSusceptibility.Part2;XML.MML.KohlrauschTimeDomain;FitFunctions.IndependentVariable.Omega;FitFunctions.Relaxation.KohlrauschDielectricSusceptibility.Part3")]
		public static IFitFunction CreateDielectricFunctionOfOmega()
		{
			KohlrauschSusceptibility result = new KohlrauschSusceptibility();
			result._useFrequencyInsteadOmega = false;
			result._useFlowTerm = true;
			result._isDielectricData = true;
			return result;
		}

		[FitFunctionCreator("Kohlrausch Complex (Freq)", "Retardation/Dielectrics", 1, 2, 4)]
		[Description("FitFunctions.Relaxation.DielectricSusceptibility.Introduction;XML.MML.GenericDielectricSusceptibility;FitFunctions.Relaxation.KohlrauschSusceptibility.Part2;XML.MML.KohlrauschTimeDomain;FitFunctions.IndependentVariable.FrequencyAsOmega;FitFunctions.Relaxation.KohlrauschDielectricSusceptibility.Part3")]
		public static IFitFunction CreateDielectricFunctionOfFrequency()
		{
			KohlrauschSusceptibility result = new KohlrauschSusceptibility();
			result._useFrequencyInsteadOmega = true;
			result._useFlowTerm = true;
			result._isDielectricData = true;
			return result;
		}


		#region IFitFunction Members

		#region independent variable definition

		public int NumberOfIndependentVariables
		{
			get
			{
				return 1;
			}
		}
		public string IndependentVariableName(int i)
		{
			return this._useFrequencyInsteadOmega ? "Frequency" : "Omega";
		}
		#endregion

		#region dependent variable definition
		private string[] _dependentVariableName = new string[] { "re", "im" };
		public int NumberOfDependentVariables
		{
			get
			{
				return _dependentVariableName.Length;
			}
		}
		public string DependentVariableName(int i)
		{
			return _dependentVariableName[i];
		}
		#endregion

		#region parameter definition
		string[] _parameterNameC = new string[] { "chi_inf", "delta_chi", "tau", "beta", "invviscosity" };
		string[] _parameterNameD = new string[] { "eps_inf", "delta_eps", "tau", "beta", "conductivity", };
		public int NumberOfParameters
		{
			get
			{
				if (_useFlowTerm)
					return 2 + 3 * _numberOfRelaxations;
				else
					return 1 + 3 * _numberOfRelaxations;
			}
		}
		public string ParameterName(int i)
		{
			string[] names = _isDielectricData ? _parameterNameD : _parameterNameC;
			if (_numberOfRelaxations == 1)
				return names[i];
			else if (_numberOfRelaxations == 0)
				return i == 1 ? names[4] : names[0];
			else
			{
				if (i == 0)
					return names[0];
				else if (i == NumberOfParameters - 1)
					return names[4];
				else
				{
					int k = (i - 1) / 3;
					int l = (i - 1) % 3;
					return names[l + 1] + (k + 1).ToString();
				}
			}
		}

		public double DefaultParameterValue(int i)
		{
			if (_useFlowTerm && i == (NumberOfParameters - 1))
				return _isDielectricData ? 0 : _invertViscosity ? 0 : double.PositiveInfinity;
			else
				return 1;
		}

		public IVarianceScaling DefaultVarianceScaling(int i)
		{
			return null;
		}

		#endregion

		public void Evaluate(double[] X, double[] P, double[] Y)
		{
			double x = X[0];
			if (_useFrequencyInsteadOmega)
				x *= (2 * Math.PI);


			Complex result = P[0];

			int iPar = 1;
			int i;
			for (i = 0, iPar = 1; i < _numberOfRelaxations; i++, iPar += 3)
			{
				result += P[0 + iPar] * Kohlrausch.ReIm(P[2 + iPar], P[1 + iPar] * x);
			}

			Y[0] = result.Re;

			if (this._useFlowTerm)
			{
				if (this._isDielectricData)
					Y[1] = -result.Im + P[iPar] / (x * 8.854187817e-12);
				else if (this._invertViscosity)
					Y[1] = -result.Im + P[iPar] / (x);
				else
					Y[1] = -result.Im + 1 / (P[iPar] * x);
			}
			else
			{
				Y[1] = -result.Im;
			}
		}

		#endregion
	}

}
