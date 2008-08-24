#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
using Altaxo.Calc;
using Altaxo.Calc.Interpolation;
using Altaxo.Calc.LinearAlgebra;
using NUnit.Framework;

namespace AltaxoTest.Calc.Interpolation
{
	/// <summary>
	/// Test for the cross validated cubic spline.
	/// </summary>
	/// <remarks> The test data originates from the output of Algorithm 642, which was translated by f2c into C and feeded with the test data below.
	/// </remarks>
	[TestFixture]
	public class TestCrossValidatedCubicSpline
	{
double[][] _testvals = {
new double[]{0.00843301352350958, -0.152243837374951},
new double[]{0.025019470131189, -0.119734055655511},
new double[]{0.0501815571171146, 0.461729355636735},
new double[]{0.0694124844388163, 0.0851220476980274},
new double[]{0.0892763556691863, 0.180380168471979},
new double[]{0.105400639177331, 0.389756960468003},
new double[]{0.13304193020319, 0.757836858329517},
new double[]{0.144587330384143, 0.704785058099651},
new double[]{0.166677919882355, 0.793994884037036},
new double[]{0.192998934131891, 1.01678524670068},
new double[]{0.214620181867521, 0.906192511335025},
new double[]{0.228451359764883, 0.665808010311522},
new double[]{0.251834099563693, 1.04051259747373},
new double[]{0.268998843539661, 0.742630635194807},
new double[]{0.290879450890956, 1.1810765143109},
new double[]{0.30715736285651, 1.02738155522738},
new double[]{0.333202886269367, 1.0417072436816},
new double[]{0.349254425425356, 0.721461550776648},
new double[]{0.367263622515492, 0.830680249332228},
new double[]{0.395793876672378, 1.17334020997532},
new double[]{0.413910668584166, 1.12441984632209},
new double[]{0.43034162488161, 0.920228278925585},
new double[]{0.450030182820448, 1.14215552834455},
new double[]{0.466854705948781, 0.788935908970938},
new double[]{0.496360513884432, 0.542289221010453},
new double[]{0.503539552750783, 0.488133260181762},
new double[]{0.523896284687285, 0.907649946408259},
new double[]{0.556218648496807, 0.668149586712674},
new double[]{0.573949241518951, 0.713550473948648},
new double[]{0.589557848376947, 0.550237275721738},
new double[]{0.609388991802025, 0.151980110647414},
new double[]{0.630538900131611, 0.235846260250003},
new double[]{0.654442123906272, 0.115629584585516},
new double[]{0.671535796514496, -0.227757721400102},
new double[]{0.687882554148269, -0.367649455893867},
new double[]{0.707735100750688, -0.485247286448983},
new double[]{0.732802827163973, -0.0880249669597303},
new double[]{0.744480400706865, -0.646258600064633},
new double[]{0.764294638162306, -0.236593751557764},
new double[]{0.791869410957149, -0.463411501001707},
new double[]{0.808997759337381, -0.347033662234902},
new double[]{0.82560029086452, -0.764451069370005},
new double[]{0.85426094111564, -0.994121364304904},
new double[]{0.870214382736112, -0.659071940610107},
new double[]{0.891374850047151, -0.776223930086174},
new double[]{0.906590191208101, -1.01820147789573},
new double[]{0.933629603868799, -1.18556910376756},
new double[]{0.948752005481511, -0.980323659089009},
new double[]{0.967142143027768, -0.739075684559176},
new double[]{0.992733618149565, -0.756103993847516}
													 };
		// expected y at the x given by testvals
double[] _refy = {
-0.131090596708801,
-0.025155914936931,
0.130926497660281,
0.241280844329335,
0.356777980825182,
0.454968188797009,
0.619212654824897,
0.679389857839982,
0.774522552179897,
0.851581548749066,
0.887872249139432,
0.904753661837447,
0.932232338574859,
0.950458645325399,
0.970376274499151,
0.977149934487782,
0.976968616138686,
0.977138916832685,
0.984496521419732,
0.998469765185854,
0.990577529585417,
0.966552438538821,
0.919057332471586,
0.865723252921441,
0.772068419648475,
0.753289417541231,
0.706807532356554,
0.611634345283875,
0.533778296253205,
0.446546874234237,
0.316797875562519,
0.169019670834313,
0.000604466751715702,
-0.112043082377132,
-0.203892451883197,
-0.288035351692333,
-0.359437869674954,
-0.387221118156361,
-0.432846107110799,
-0.513327897195207,
-0.57771910267756,
-0.64883122966336,
-0.767658085018699,
-0.82295140017917,
-0.884530305811163,
-0.91766943370851,
-0.938292116897269,
-0.924731111232022,
-0.890753030266603,
-0.831558418471905
};

double[][] _expectedCoefficients = {
new double[]{6.39641683,0,-34.890352},
new double[]{6.36762072,-1.73612193,-190.888418},
new double[]{5.91767991,-16.1455748,354.740895},
new double[]{5.69027111,4.32041429,97.1711227},
new double[]{5.97693483,10.1109983,-193.781071},
new double[]{6.15185515,0.737255516,-301.341078},
new double[]{5.50190175,-24.2511138,-72.6930263},
new double[]{4.912855,-26.768924,-30.8059466},
new double[]{3.68507301,-28.8104886,1.31189876},
new double[]{2.17115708,-28.7068971,273.800416},
new double[]{1.31378652,-10.9471772,304.018027},
new double[]{1.18543911,1.66760504,-90.1011548},
new double[]{1.11563636,-4.65283054,88.497312},
new double[]{1.03412854,-0.0957294283,-254.296142},
new double[]{0.664698094,-16.7881915,93.2347944},
new double[]{0.192258073,-12.2351882,176.087297},
new double[]{-0.0867290455,1.52366924,282.867792},
new double[]{0.180829708,15.1450596,-138.848835},
new double[]{0.591231413,7.64339144,-392.554826},
new double[]{0.0687772958,-25.9556754,-104.121887},
new double[]{-0.974213925,-31.6147391,116.638876},
new double[]{-1.91866551,-25.8652743,40.231376},
new double[]{-2.89037963,-23.488981,408.211617},
new double[]{-3.33411069,-2.88508359,281.557857},
new double[]{-2.76899749,22.0376926,-97.4420919},
new double[]{-2.46764467,19.9390709,-534.793126},
new double[]{-2.32070528,-12.72085,-203.521779},
new double[]{-3.78092027,-32.455765,-110.305013},
new double[]{-5.03587125,-38.3230849,186.21289},
new double[]{-6.09611082,-29.6035135,357.240803},
new double[]{-6.84877304,-8.35003272,85.3888475},
new double[]{-7.08738983,-2.93213383,195.613246},
new double[]{-6.89226557,11.0952277,385.336746},
new double[]{-6.17517034,30.8556883,194.475943},
new double[]{-5.01048789,40.3928416,-75.6263832},
new double[]{-3.49610478,35.8887127,-400.909584},
new double[]{-2.45259173,5.73903743,46.7613839},
new double[]{-2.29942571,7.37721592,-380.49745},
new double[]{-2.45523237,-15.2405845,-56.7969878},
new double[]{-3.42530353,-19.9390766,25.5355806},
new double[]{-4.08587552,-18.6269297,406.030293},
new double[]{-4.36862439,1.59646256,215.325853},
new double[]{-3.74648588,20.1105995,-158.204787},
new double[]{-3.22561452,12.538867,112.099519},
new double[]{-2.5443751,19.6551016,290.741063},
new double[]{-1.74433152,32.926275,124.922574},
new double[]{0.310286617,43.059774,-282.93839},
new double[]{1.41850799,30.2236501,-374.633256},
new double[]{2.15004244,9.55497879,-124.45523}
																			 };

double[] _expectedErrorEstimates = {
0.132517656,
0.102039214,
0.0846107124,
0.0820963087,
0.0816778206,
0.0816300283,
0.0811463661,
0.0811504917,
0.0819473196,
0.0817198582,
0.0803674678,
0.0798173652,
0.079606458,
0.079685341,
0.0800043175,
0.0803659393,
0.0806063385,
0.080735603,
0.081318591,
0.0808449048,
0.0796054188,
0.078969158,
0.0790163661,
0.0794348815,
0.0796531273,
0.0798887225,
0.0815401485,
0.0816898522,
0.0803337048,
0.0797964788,
0.080098875,
0.0804512571,
0.0797499864,
0.0790400737,
0.0789288896,
0.0792479218,
0.079256474,
0.0794847872,
0.0805881713,
0.0811427425,
0.0810255913,
0.0813916823,
0.0814648653,
0.0811561895,
0.081269507,
0.0816238509,
0.0819776448,
0.0841143803,
0.0953330856,
0.139785039
																			 };


void AreEqual(double expected, double current, double reldev, double absdev, string msg)
{
	double min, max;
	bool passes = false;
	if (expected == 0)
	{
		min = -Math.Abs(absdev);
		max = Math.Abs(absdev);
		passes = current >= min && current <= max;
	}
	else
	{
		double dev = Math.Abs(expected * reldev);
		if (dev < Math.Abs(absdev))
			dev = Math.Abs(absdev);

		min = expected - dev;
		max = expected + dev;
		passes = current >= min && current <= max;
	}

	if (!passes)
		Assert.Fail("Value {0} is not in the interval [{1},{2}], ({3})", current, min, max, msg);
}

		[Test]
		public void Test1()
		{
			int len = _testvals.Length;

			double[] x = new double[len];
			double[] y = new double[len];
			double[] dy = new double[len];


			for (int i = 0; i < len; i++)
			{
				x[i] = _testvals[i][0];
				y[i] = _testvals[i][1];
				dy[i] = 1;
			}


			CrossValidatedCubicSpline spline = new CrossValidatedCubicSpline();
			spline.CalculateErrorEstimates = true;

			spline.SetErrorVariance(VectorMath.ToROVector(dy), -1);
			spline.Interpolate(VectorMath.ToROVector(x), VectorMath.ToROVector(y));


			// Test the values y at the points x[i] - this are the coefficients of 0th order
			for (int i = 0; i < len; i++)
			{
				AreEqual(_refy[i], spline.Coefficient0[i], 1e-7,1e-7,"Coeff0["+i.ToString()+"]");
			}

	
			// test the higher order coefficients
			for (int i = 0; i < len-1; i++)
			{
				AreEqual(_expectedCoefficients[i][0], spline.Coefficient1[i], 1e-4, 1e-7, "Coeff1[" + i.ToString() + "]");
				AreEqual(_expectedCoefficients[i][1], spline.Coefficient2[i], 1e-4, 1e-7, "Coeff2[" + i.ToString() + "]");
				AreEqual(_expectedCoefficients[i][2], spline.Coefficient3[i], 1e-4, 1e-7, "Coeff3[" + i.ToString() + "]");
			}

			// test the standard error estimates
			for (int i = 0; i < len; i++)
			{
				AreEqual(_expectedErrorEstimates[i], spline.ErrorEstimate[i], 1e-4, 1e-7, "Error[" + i.ToString() + "]");
			}

			AreEqual(0.96143745, spline.SmoothingParameter, 1e-7, 1e-7,"SmoothingParameter");
			AreEqual(39.6239255, spline.EstimatedDegreesOfFreedom, 1e-7, 1e-7,"EstimatedDegreesofFreedom");
			AreEqual(0.0433056129, spline.GeneralizedCrossValidation, 1e-7, 1e-7,"GeneralizedCrossValidation");
			AreEqual(0.0271968858, spline.MeanSquareResidual, 1e-7, 1e-7,"MeanSquareResidual");
			AreEqual(0.00712188179, spline.EstimatedTrueMeanSquareError, 1e-7, 1e-7, "EstimatedTrueMeanSquareError");
			AreEqual(0.0343187676, spline.EstimatedErrorVariance, 1e-7, 1e-7,"EstimatedErrorVariance");
			AreEqual(1, spline.MeanSquareOfInputVariance, 1e-7, 1e-7, "MeanSquareOfInputVariance");
			

		}
	}
}
